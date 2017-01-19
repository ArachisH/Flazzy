using System;
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
        private readonly List<ExceptionProfile> _exceptions;
        private readonly Dictionary<Jumper, ASInstruction> _backExits;
        private readonly Dictionary<Jumper, ASInstruction> _forwardExits;
        private readonly Dictionary<ASInstruction, List<ASInstruction>> _switchExits;

        public List<ASInstruction> Instructions { get; }

        public ASCode(ABCFile abc, ASMethodBody body)
        {
            _abc = abc;
            _body = body;
            _exceptions = new List<ExceptionProfile>();
            _forwardExits = new Dictionary<Jumper, ASInstruction>();
            _backExits = new Dictionary<Jumper, ASInstruction>();
            _switchExits = new Dictionary<ASInstruction, List<ASInstruction>>();

            Instructions = new List<ASInstruction>();

            LoadInstructions();
        }

        public void Deobfuscate()
        {
            var machine = new ASMachine(this, _body.LocalCount);
            var cleaned = new List<ASInstruction>(Instructions.Count);
            var valuePushers = new Stack<ASInstruction>(_body.MaxStack);
            for (int i = 0; i < Instructions.Count; i++)
            {
                ASInstruction instruction = Instructions[i];
                if (Jumper.IsValid(instruction.OP))
                {
                    i += VerifyJumper(machine,
                        (Jumper)instruction, cleaned, valuePushers);
                }
                else
                {
                    instruction.Execute(machine);
                    cleaned.Add(instruction);

                    int popCount = instruction.GetPopCount();
                    for (int j = 0; j < popCount; j++)
                    {
                        valuePushers.Pop();
                    }
                    int pushCount = instruction.GetPushCount();
                    for (int j = 0; j < pushCount; j++)
                    {
                        valuePushers.Push(instruction);
                    }
                }
            }

            #region Jump Instruction Fixing
            ASInstruction[] jumpers = _forwardExits.Keys.ToArray();
            foreach (Jumper jumper in jumpers)
            {
                ASInstruction[] body = GetBody(jumper);
                ASInstruction final = _forwardExits[jumper];

                // True if it still has the jump instruction, but the instruction
                // needed to determine the final instruction to jump is not present.
                if (cleaned.Contains(jumper) && !cleaned.Contains(final))
                {
                    // Start at the index of the instruction that should come after the final instruction to jump.
                    for (int j = (Instructions.IndexOf(final) + 1); j < Instructions.Count; j++)
                    {
                        ASInstruction afterEnd = Instructions[j];
                        if (cleaned.Contains(afterEnd)) // Check if this instruction wasn't removed in the cleaning process, otherwise, get the next instruction.
                        {
                            int finalIndex = (cleaned.IndexOf(afterEnd) - 1); // This should be the new final instruction index to jump.
                            SetEndOfJump(jumper, cleaned[finalIndex]);
                            break;
                        }
                    }
                }
            }
            #endregion

            Instructions.Clear();
            Instructions.AddRange(cleaned);
        }
        public int FindOPIndex(OPCode op)
        {
            return Instructions.FindIndex(i => i.OP == op);
        }
        public ASInstruction[] GetBody(Jumper jumper)
        {
            int scopeStart = (Instructions.IndexOf(jumper) + 1);
            int scopeEnd = (Instructions.IndexOf(_forwardExits[jumper]) + 1);

            ASInstruction[] body = new ASInstruction[scopeEnd - scopeStart];
            Instructions.CopyTo(scopeStart, body, 0, body.Length);
            return body;
        }
        public void SetEndOfJump(Jumper jumper, ASInstruction end)
        {
            if (!Instructions.Contains(end))
            {
                throw new ArgumentException(
                    "The given instruction must first be added to the Instructions property.",
                    nameof(end));
            }
            _forwardExits[jumper] = end;
        }

        private void LoadInstructions()
        {
            var labels = new Dictionary<long, ASInstruction>();
            var jumperSets = new Dictionary<long, List<Jumper>>();
            var instructions = new Dictionary<long, ASInstruction>();
            var forwardExits = new Dictionary<long, List<ASInstruction>>();
            var debugInstructions = new SortedDictionary<int, DebugIns>();
            using (var inCode = new FlashReader(_body.Code))
            {
                while (inCode.IsDataAvailable)
                {
                    long previousPos = inCode.Position;
                    var instruct = ASInstruction.Create(_abc, inCode);
                    Instructions.Add(instruct);

                    switch (instruct.OP)
                    {
                        case OPCode.Label:
                        {
                            labels.Add(inCode.Position, instruct);
                            break;
                        }
                        case OPCode.Debug:
                        {
                            var debugIns = (DebugIns)instruct;
                            debugInstructions[debugIns.RegisterIndex] = debugIns;
                            break;
                        }
                    }

                    if (_body.Exceptions.Count > 0)
                    {
                        instructions[previousPos] = instruct;
                    }
                    else if (instructions.ContainsKey(inCode.Position))
                    {
                        instructions[inCode.Position] = instruct;
                    }

                    if (instruct.OP == OPCode.LookUpSwitch)
                    {
                        var lookUp = (LookUpSwitchIns)instruct;
                        var switchExits2 = new List<ASInstruction>();
                        for (int j = 0; j < lookUp.CaseOffsets.Count; j++)
                        {
                            uint offset = lookUp.CaseOffsets[j];
                            long jumpExit = (previousPos + offset);
                            switchExits2.Add(labels[jumpExit - uint.MaxValue]);
                        }

                        uint defaultOffset = lookUp.DefaultOffset;
                        long defaultJumpExit = (previousPos + defaultOffset);
                        if (defaultJumpExit <= inCode.Length)
                        {
                            forwardExits[defaultJumpExit] = switchExits2;
                        }
                        else switchExits2.Add(labels[defaultJumpExit - uint.MaxValue]);

                        _switchExits[instruct] = switchExits2;
                        continue;
                    }

                    List<ASInstruction> switchExits = null;
                    if (forwardExits.TryGetValue(previousPos, out switchExits))
                    {
                        switchExits.Add(instruct);
                    }

                    List<Jumper> jumpers = null;
                    if (jumperSets.TryGetValue(inCode.Position, out jumpers))
                    {
                        jumpers.ForEach(
                            j => _forwardExits.Add(j, instruct));

                        jumperSets.Remove(inCode.Position);
                    }

                    if (Jumper.IsValid(instruct.OP))
                    {
                        var jumper = (Jumper)instruct;
                        if (jumper.Offset == 0) continue;

                        long jumpExit = (inCode.Position + jumper.Offset);
                        if (jumpExit <= inCode.Length)
                        {
                            jumpers = null;
                            if (!jumperSets.TryGetValue(jumpExit, out jumpers))
                            {
                                jumpers = new List<Jumper>();
                                jumperSets.Add(jumpExit, jumpers);
                            }
                            jumpers.Add(jumper);
                        }
                        else
                        {
                            ASInstruction labelIns = labels[jumpExit - uint.MaxValue];
                            _backExits[jumper] = labelIns;
                        }
                    }
                }
            }

            if (_body.Exceptions.Count > 0)
            {
                _exceptions.AddRange(
                    _body.Exceptions.Select(e => new ExceptionProfile(e)));

                foreach (ExceptionProfile exception in _exceptions)
                {
                    exception.To = instructions[exception.Exception.To];
                    exception.From = instructions[exception.Exception.From];
                    exception.Target = instructions[exception.Exception.Target];
                }
            }
        }
        private bool IsReverseJump(Jumper jumper)
        {
            return (_backExits.ContainsKey(jumper));
        }
        protected int VerifyJumper(ASMachine machine, Jumper jumper, List<ASInstruction> cleaned, Stack<ASInstruction> valuePushers)
        {
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
                    if (Local.IsGetLocal(pusher.OP))
                    {
                        locals.Add((Local)pusher);
                    }
                }
            }

            if (isJumping == null) // Output is not known, keep the instruction.
            {
                cleaned.Add(jumper);
                return 0;
            }

            // Gather information about the jump instruction, and it's 'block' of instructions that are being jumped over(if 'isJumping = true').
            ASInstruction exit = null;
            bool isBackwardsJump = false;
            IEnumerable<ASInstruction> block = null;
            Dictionary<Jumper, ASInstruction> exits = null;
            if (IsReverseJump(jumper))
            {
                isBackwardsJump = true;
                exit = _backExits[jumper];
                exits = _backExits;

                block = cleaned
                    .Skip(cleaned.IndexOf(exit) + 1)
                    .TakeWhile(i => i != jumper);
            }
            else
            {
                block = (jumper.Offset > 0 ? GetBody(jumper) : null);

                exit = _forwardExits[jumper];
                exits = _forwardExits;
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

                IEnumerable<KeyValuePair<Jumper, ASInstruction>> exitSets = _forwardExits.Concat(_backExits);
                foreach (KeyValuePair<Jumper, ASInstruction> exitSet in exitSets)
                {
                    if (exitSet.Key == jumper) continue;
                    // Is another jump instruction(or exit) inside of the 'block' we're going to remove?
                    // If a full jump instruction is within this jump body, don't worry about removing it since it will never be used.
                    if (block.Contains(exitSet.Key) && !block.Contains(exitSet.Value) ||
                        block.Contains(exitSet.Value) && !block.Contains(exitSet.Key))
                    {
                        // Keep the jump instruction, since it will corrupt the other jump instruction that is using it.
                        cleaned.Add(jumper);
                        return 0;
                    }
                }
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
            exits.Remove(jumper);
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
            var offsets = new Dictionary<ASInstruction, long>();
            var jumperStarts = new Dictionary<Jumper, long>();
            var forwardExits = new Dictionary<ASInstruction, long>();
            var backJumpExits = new Dictionary<ASInstruction, long>();
            var jumperSets = new Dictionary<ASInstruction, List<Jumper>>();
            for (int i = 0; i < Instructions.Count; i++)
            {
                ASInstruction instruction = Instructions[i];
                if (instruction.OP == OPCode.LookUpSwitch)
                {
                    var lookUpIns = (LookUpSwitchIns)instruction;
                    List<ASInstruction> switchLabels = _switchExits[lookUpIns];
                    for (int j = 0; j < (switchLabels.Count - 1); j++)
                    {
                        ASInstruction label = switchLabels[j];

                        long offset = backJumpExits[label];
                        long jumpCount = (output.Position - offset);
                        lookUpIns.CaseOffsets[j] = (uint)(uint.MaxValue - jumpCount);
                    }

                    ASInstruction defaultExit = switchLabels[switchLabels.Count - 1];
                    if (defaultExit.OP == OPCode.Label)
                    {
                        long offset = backJumpExits[defaultExit];
                        long jumpCount = (output.Position - offset);
                        lookUpIns.DefaultOffset = (uint)(uint.MaxValue - jumpCount);
                    }
                    else
                    {
                        forwardExits[defaultExit] = (output.Position + 1);
                    }
                }
                if (_body.Exceptions.Count > 0)
                {
                    offsets[instruction] = output.Position;
                }
                instruction.WriteTo(output);

                if (instruction.OP == OPCode.Label)
                {
                    backJumpExits.Add(instruction, output.Position);
                }

                long defaultOffsetPos = 0;
                if (forwardExits.TryGetValue(instruction, out defaultOffsetPos))
                {
                    long offset = (output.Position - defaultOffsetPos);
                    long curPos = output.Position;

                    output.Position = defaultOffsetPos;
                    output.WriteUInt24((uint)offset);
                    output.Position = curPos;
                }

                List<Jumper> jumpers = null;
                if (jumperSets.TryGetValue(instruction, out jumpers))
                {
                    foreach (Jumper jumper in jumpers)
                    {
                        long jumpStart = jumperStarts[jumper];
                        jumper.Offset = (uint)(output.Position - (jumpStart + 4));

                        long curPosition = output.Position;
                        output.Position = jumpStart;

                        jumper.WriteTo(output);
                        output.Position = curPosition;
                    }
                }

                if (Jumper.IsValid(instruction.OP))
                {
                    ASInstruction endInstruction = null;
                    var jumper = (Jumper)instruction;
                    if (_forwardExits.TryGetValue(jumper, out endInstruction))
                    {
                        jumpers = null;
                        if (!jumperSets.TryGetValue(endInstruction, out jumpers))
                        {
                            jumpers = new List<Jumper>();
                            jumperSets.Add(endInstruction, jumpers);
                        }
                        jumpers.Add(jumper);
                        jumperStarts.Add(jumper, output.Position - 4);
                    }
                    else if (_backExits.TryGetValue(jumper, out endInstruction))
                    {
                        long jumpEnter = backJumpExits[endInstruction];
                        long jumpCount = (output.Position - jumpEnter);
                        jumper.Offset = (uint)(uint.MaxValue - jumpCount);

                        output.Position -= 4;
                        jumper.WriteTo(output);
                    }
                }
            }

            foreach (ExceptionProfile profile in _exceptions)
            {
                profile.Exception.To = (int)offsets[profile.To];
                profile.Exception.From = (int)offsets[profile.From];
                profile.Exception.Target = (int)offsets[profile.Target];
            }
        }

        private class ExceptionProfile
        {
            public ASException Exception { get; }

            public ASInstruction To { get; set; }
            public ASInstruction From { get; set; }
            public ASInstruction Target { get; set; }

            public ExceptionProfile(ASException exception)
            {
                Exception = exception;
            }
        }
    }
}