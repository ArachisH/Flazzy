using System.IO;
using System.Linq;
using System.Collections.Generic;

using Flazzy.IO;
using Flazzy.ABC.AVM2.Instructions;

namespace Flazzy.ABC.AVM2
{
    public class ASCode : FlashItem
    {
        private readonly ABCFile _abc;
        private readonly ASMethodBody _body;

        public List<ASInstruction> Instructions { get; }
        public Dictionary<Jumper, ASInstruction> JumpExits { get; }
        public Dictionary<LookUpSwitchIns, List<ASInstruction>> SwitchExits { get; }

        public ASCode(ABCFile abc, ASMethodBody body)
        {
            _abc = abc;
            _body = body;

            Instructions = new List<ASInstruction>();
            JumpExits = new Dictionary<Jumper, ASInstruction>();
            SwitchExits = new Dictionary<LookUpSwitchIns, List<ASInstruction>>();
            LoadInstructions();
        }

        public void Deobfuscate()
        {
            var machine = new ASMachine(this, _body.LocalCount);
            var cleaned = new List<ASInstruction>(Instructions.Count);
            var valuePushers = new Stack<ASInstruction>(_body.MaxStack);
            var localReferences = new Dictionary<int, List<ASInstruction>>();
            var swappedValues = new Dictionary<ASInstruction, ASInstruction[]>();
            KeyValuePair<Jumper, ASInstruction>[] jumpExits = JumpExits.ToArray();
            KeyValuePair<LookUpSwitchIns, List<ASInstruction>>[] switchExits = SwitchExits.ToArray();
            for (int i = 0; i < Instructions.Count; i++)
            {
                ASInstruction instruction = Instructions[i];
                if (Jumper.IsValid(instruction.OP))
                {
                    i += GetFinalJumpCount(machine,
                        (Jumper)instruction, cleaned, localReferences, valuePushers);
                }
                else
                {
                    instruction.Execute(machine);
                    #region Arithmetic Optimization
                    if (Computation.IsValid(instruction.OP))
                    {
                        object result = machine.Values.Pop();
                        ASInstruction rightPusher = valuePushers.Pop();
                        ASInstruction leftPusher = valuePushers.Pop();
                        if (!Local.IsValid(leftPusher.OP) && !Local.IsValid(rightPusher.OP) && result != null)
                        {
                            // Constant values found, push result instead of having it do the calculation everytime.
                            cleaned.Remove(leftPusher);
                            cleaned.Remove(rightPusher);
                            instruction = Primitive.Create(_abc, result);

                            foreach (KeyValuePair<Jumper, ASInstruction> jumpExit in jumpExits)
                            {
                                if (leftPusher == jumpExit.Value)
                                {
                                    JumpExits[jumpExit.Key] = instruction;
                                    // Do not break, another jump instruction can share the same exit.
                                }
                            }
                            foreach (KeyValuePair<LookUpSwitchIns, List<ASInstruction>> switchExit in switchExits)
                            {
                                List<ASInstruction> cases = switchExit.Value;
                                for (int j = 0; j < cases.Count; j++)
                                {
                                    ASInstruction exit = cases[j];
                                    if (leftPusher == exit)
                                    {
                                        cases[j] = instruction;
                                    }
                                }
                            }
                            instruction.Execute(machine);
                        }
                        else
                        {
                            // Do not attempt to optimize when a local is being use, as these values can change.
                            valuePushers.Push(leftPusher);
                            valuePushers.Push(rightPusher);
                            machine.Values.Push(result);
                        }
                    }
                    #endregion
                    cleaned.Add(instruction);

                    ASInstruction[] swaps = null;
                    List<ASInstruction> references = null;
                    if (Local.IsValid(instruction.OP))
                    {
                        var local = (Local)instruction;
                        if (!localReferences.TryGetValue(local.Register, out references))
                        {
                            references = new List<ASInstruction>();
                            localReferences[local.Register] = references;
                        }
                        references.Add(instruction);
                    }
                    else if (instruction.OP == OPCode.Swap)
                    {
                        swaps = new ASInstruction[2];
                        swappedValues[instruction] = swaps;
                    }

                    int popCount = instruction.GetPopCount();
                    for (int j = 0; j < popCount; j++)
                    {
                        ASInstruction pusher = valuePushers.Pop();
                        references?.Add(pusher);
                        if (swaps != null)
                        {
                            swaps[j] = pusher;
                        }
                    }
                    int pushCount = instruction.GetPushCount();
                    for (int j = 0; j < pushCount; j++)
                    {
                        valuePushers.Push(instruction);
                    }
                }
            }

            // Remove dead locals.
            foreach (KeyValuePair<int, List<ASInstruction>> local in localReferences)
            {
                if (local.Key < (_body.Method.Parameters.Count + 1)) continue;
                List<ASInstruction> references = localReferences[local.Key];

                bool isNeeded = false;
                foreach (ASInstruction reference in references)
                {
                    // This checks if the local is being referenced by something else that retreives the value.
                    if (Local.IsValid(reference.OP) && !Local.IsSetLocal(reference.OP))
                    {
                        isNeeded = true;
                        break;
                    }
                }
                if (!isNeeded)
                {
                    foreach (ASInstruction reference in references)
                    {
                        if (reference.OP == OPCode.Swap)
                        {
                            ASInstruction[] swaps = swappedValues[reference];
                            foreach (ASInstruction swap in swaps)
                            {
                                cleaned.Remove(swap);
                            }
                        }
                        cleaned.Remove(reference);
                    }
                }
            }

            jumpExits = JumpExits.ToArray(); // Global property could have been updated.
            foreach (KeyValuePair<Jumper, ASInstruction> jumpExit in jumpExits)
            {
                if (Instructions.IndexOf(jumpExit.Key) >
                    Instructions.IndexOf(jumpExit.Value))
                {
                    // This is not a forward jump instruction, since the exit appears before the jump.
                    continue;
                }

                // True if it still has the jump instruction, but the instruction
                // needed to determine the final instruction to jump is not present.
                if (cleaned.Contains(jumpExit.Key) && !cleaned.Contains(jumpExit.Value))
                {
                    // Start at the index of the instruction that should come after the final instruction to jump.
                    for (int j = (Instructions.IndexOf(jumpExit.Value) + 1); j < Instructions.Count; j++)
                    {
                        ASInstruction afterEnd = Instructions[j];
                        int exitIndex = cleaned.IndexOf(afterEnd);
                        if (exitIndex != -1) // Does this instruction exist in the cleaned output?; Otherwise, get instruction that comes after it.
                        {
                            JumpExits[jumpExit.Key] = cleaned[exitIndex];
                            break;
                        }
                    }
                }
            }

            Instructions.Clear();
            Instructions.AddRange(cleaned);
        }
        public int IndexOf(OPCode op)
        {
            return Instructions.FindIndex(i => i.OP == op);
        }
        public bool IsBackwardsJump(Jumper jumper)
        {
            return (JumpExits[jumper].OP == OPCode.Label);
        }
        public int IndexOf(ASInstruction instruction)
        {
            return Instructions.IndexOf(instruction);
        }
        public ASInstruction[] GetJumpBlock(Jumper jumper)
        {
            int blockStart = (Instructions.IndexOf(jumper) + 1);
            int scopeEnd = Instructions.IndexOf(JumpExits[jumper]);

            ASInstruction[] body = new ASInstruction[scopeEnd - blockStart];
            Instructions.CopyTo(blockStart, body, 0, body.Length);
            return body;
        }

        private void LoadInstructions()
        {
            var marks = new Dictionary<long, ASInstruction>();
            var sharedExits = new Dictionary<long, List<Jumper>>();
            using (var input = new FlashReader(_body.Code))
            {
                while (input.IsDataAvailable)
                {
                    long previousPosition = input.Position;
                    var instruction = ASInstruction.Create(_abc, input);

                    marks[previousPosition] = instruction;
                    Instructions.Add(instruction);

                    List<Jumper> jumpers = null;
                    if (sharedExits.TryGetValue(previousPosition, out jumpers))
                    {
                        // This is an exit position for a jump instruction, or more.
                        foreach (Jumper jumper in jumpers)
                        {
                            JumpExits.Add(jumper, instruction);
                        }
                        sharedExits.Remove(previousPosition);
                    }

                    if (instruction.OP == OPCode.LookUpSwitch)
                    {
                        var lookUp = (LookUpSwitchIns)instruction;
                        var offsets = new List<uint>(lookUp.CaseOffsets);
                        offsets.Add(lookUp.DefaultOffset);

                        var cases = new List<ASInstruction>();
                        foreach (uint offset in offsets)
                        {
                            ASInstruction exit = null;
                            long exitPosition = (previousPosition + offset);
                            if (exitPosition <= input.Length)
                            {
                                // TODO
                            }
                            else
                            {
                                exit = marks[(exitPosition - uint.MaxValue) - 1];
                            }
                            cases.Add(exit);
                        }
                        SwitchExits.Add(lookUp, cases);
                    }
                    else if (Jumper.IsValid(instruction.OP))
                    {
                        var jumper = (Jumper)instruction;
                        if (jumper.Offset == 0) continue;

                        long exitPosition = (input.Position + jumper.Offset);
                        if (exitPosition == input.Length)
                        {
                            // Jump exit does not exist at this (non-existent)index, do not look for exit.
                            continue;
                        }
                        else if (exitPosition < input.Length) // Forward jump.
                        {
                            jumpers = null;
                            if (!sharedExits.TryGetValue(exitPosition, out jumpers))
                            {
                                jumpers = new List<Jumper>();
                                sharedExits.Add(exitPosition, jumpers);
                            }
                            jumpers.Add(jumper);
                        }
                        else // Backwards jump.
                        {
                            var label = (LabelIns)marks[(exitPosition - uint.MaxValue) - 1];
                            JumpExits.Add(jumper, label);
                        }
                    }
                }
            }
        }
        private void Rewrite(FlashWriter output, ASInstruction instruction, long position)
        {
            long currentPosition = output.Position;
            output.Position = position;

            instruction.WriteTo(output);
            output.Position = currentPosition;
        }
        private int GetFinalJumpCount(ASMachine machine, Jumper jumper, List<ASInstruction> cleaned,
            Dictionary<int, List<ASInstruction>> localReferences, Stack<ASInstruction> valuePushers)
        {
            var magicCount = 0;
            var locals = new List<Local>();
            var pushers = new List<ASInstruction>();
            bool? isJumping = jumper.RunCondition(machine);

            // Get the instructions that pushed the values the jump instruction used.
            int popCount = jumper.GetPopCount();
            for (int i = 0; i < popCount; i++)
            {
                ASInstruction pusher = valuePushers.Pop();
                if (!pushers.Contains(pusher))
                {
                    pushers.Add(pusher);
                    // Get the instructions that were pushed by a GetLocal/N.
                    // These are used to determine whether the jump should be kept, since a local register could change within the jump body.
                    if (Local.IsValid(pusher.OP))
                    {
                        locals.Add((Local)pusher);
                    }
                    else if (Primitive.IsValid(pusher.OP) ||
                        pusher.OP == OPCode.Dup)
                    {
                        magicCount++;
                    }
                }
            }

            // Output is not known, keep the instruction.
            if (isJumping == null)
            {
                cleaned.Add(jumper);
                return 0;
            }

            if (pushers.Count != (magicCount + locals.Count))
            {
                // One or more push instructions are wildcards, they have a 'history' of being modified.
                // Keep this jump instruction, result could change.
                cleaned.Add(jumper);
                return 0;
            }

            // Gather information about the jump instruction, and it's 'block' of instructions that are being jumped over(if 'isJumping = true').
            ASInstruction exit = null;
            bool isBackwardsJump = false;
            IEnumerable<ASInstruction> block = null;
            if (!JumpExits.TryGetValue(jumper, out exit))
            {
                // This jump instruction should not be 'cleaned', keep it.
                cleaned.Add(jumper);
                return 0;
            }
            if (IsBackwardsJump(jumper))
            {
                isBackwardsJump = true;

                block = cleaned
                    .Skip(cleaned.IndexOf(exit) + 1)
                    .TakeWhile(i => i != jumper);
            }
            else
            {
                block = (jumper.Offset > 0 ? GetJumpBlock(jumper) : null);
            }


            if (isJumping == true && block != null)
            {
                if (isBackwardsJump)
                {
                    // Check if any of the locals used by the jump instruction is being set within the body.
                    // If the anwser is yes, removing the jump instruction is a bad idea, since the output of the condition could change.
                    foreach (Local local in locals)
                    {
                        foreach (ASInstruction instruction in block)
                        {
                            if (!Local.IsValid(instruction.OP)) continue;
                            if (Local.IsGetLocal(instruction.OP)) continue;
                            var bodyLocal = (Local)instruction;

                            if (bodyLocal.Register == local.Register)
                            {
                                // Do not remove the jump instruction, condition result may change.
                                cleaned.Add(jumper);
                                return 0;
                            }
                        }
                    }
                }

                foreach (KeyValuePair<Jumper, ASInstruction> jumpExit in JumpExits)
                {
                    if (jumpExit.Key == jumper) continue;

                    bool hasEntry = block.Contains(jumpExit.Key); // Does a jump instruction begin somewhere in the block?
                    bool hasExit = block.Contains(jumpExit.Value); // Does the jump instruction end somewhere in the block?

                    ASInstruction afterLast = Instructions[Instructions.IndexOf(block.Last()) + 1];
                    bool isExitAfterLast = (jumpExit.Value == afterLast); // Does the exit of the jump that is in the block come after the final instruction of the current block?

                    if (hasEntry && !hasExit && !isExitAfterLast ||
                        hasExit && !hasEntry)
                    {
                        // Keep the jump instruction, since it will corrupt the other jump instruction that is using it.
                        cleaned.Add(jumper);
                        return 0;
                    }
                }
                foreach (KeyValuePair<LookUpSwitchIns, List<ASInstruction>> switchExit in SwitchExits)
                {
                    foreach (ASInstruction caseExit in switchExit.Value)
                    {
                        if (block.Contains(caseExit))
                        {
                            cleaned.Add(jumper);
                            return 0;
                        }
                    }
                }
            }

            foreach (Local local in locals)
            {
                List<ASInstruction> references = localReferences[local.Register];
                references.Remove(local);
            }

            // Remove the instructions that pushed values for the jump instruction that is to be removed.
            foreach (ASInstruction pusher in pushers)
            {
                cleaned.Remove(pusher);
            }

            if (isJumping == false || isBackwardsJump)
            {
                block = null;
            }
            JumpExits.Remove(jumper);
            return (block?.Count() ?? 0);
        }

        public byte[] ToArray()
        {
            using (var outMem = new MemoryStream())
            using (var outCode = new FlashWriter(outMem))
            {
                WriteTo(outCode);
                return outMem.ToArray();
            }
        }
        public override void WriteTo(FlashWriter output)
        {
            var marks = new Dictionary<ASInstruction, long>();
            var sharedExits = new Dictionary<ASInstruction, List<ASInstruction>>();
            for (int i = 0; i < Instructions.Count; i++)
            {
                long previousPosition = output.Position;
                ASInstruction instruction = Instructions[i];

                marks.Add(instruction, previousPosition);
                instruction.WriteTo(output);

                List<ASInstruction> jumpers = null;
                if (sharedExits.TryGetValue(instruction, out jumpers))
                {
                    foreach (ASInstruction jumper in jumpers)
                    {
                        long position = marks[jumper];
                        var fixedOffset = (uint)(previousPosition - (position + 4));

                        ((Jumper)jumper).Offset = fixedOffset;
                        Rewrite(output, jumper, position);
                    }
                    sharedExits.Remove(instruction);
                }

                if (instruction.OP == OPCode.LookUpSwitch)
                {
                    var lookUp = (LookUpSwitchIns)instruction;
                    List<ASInstruction> cases = SwitchExits[lookUp];
                    for (int j = 0; j < cases.Count; j++)
                    {
                        ASInstruction exit = cases[j];
                        long exitPosition = marks[exit];

                        uint fixedOffset = 0;
                        if (exit.OP != OPCode.Label)
                        {
                            // TODO
                        }
                        else
                        {
                            long jumpCount = (previousPosition - (exitPosition + 1));
                            fixedOffset = (uint)(uint.MaxValue - jumpCount);
                        }

                        if (j == (cases.Count - 1))
                        {
                            lookUp.DefaultOffset = fixedOffset;
                        }
                        else
                        {
                            lookUp.CaseOffsets[j] = fixedOffset;
                        }
                    }
                    Rewrite(output, lookUp, previousPosition);
                }
                else if (Jumper.IsValid(instruction.OP))
                {
                    var jumper = (Jumper)instruction;
                    if (jumper.Offset == 0) continue;

                    ASInstruction exit = null;
                    if (!JumpExits.TryGetValue(jumper, out exit))
                    {
                        // An exit for this jump instruction could not be found, perhaps its' offset exceeds the index limit?
                        continue;
                    }
                    else if (exit.OP != OPCode.Label || !marks.ContainsKey(exit)) // Forward jumps can have a label for an exit, as long as it is located ahead.
                    {
                        jumpers = null;
                        if (!sharedExits.TryGetValue(exit, out jumpers))
                        {
                            jumpers = new List<ASInstruction>();
                            sharedExits.Add(exit, jumpers);
                        }
                        jumpers.Add(jumper);
                    }
                    else // Backward jumps must always have a label for an exit, and must be behind this instruction.
                    {
                        long exitPosition = marks[exit];
                        long jumpCount = (output.Position - (exitPosition + 1));

                        var fixedOffset = (uint)(uint.MaxValue - jumpCount);
                        jumper.Offset = fixedOffset;

                        Rewrite(output, jumper, previousPosition);
                    }
                }
            }
        }
    }
}