using System.Diagnostics;
using System.Collections;

using Flazzy.IO;
using Flazzy.ABC.AVM2.Instructions;

namespace Flazzy.ABC.AVM2
{
    [DebuggerDisplay("Count = {Count}")]
    public class ASCode : FlashItem, IList<ASInstruction>
    {
        private readonly ABCFile _abc;
        private readonly ASMethodBody _body;

        private readonly List<ASInstruction> _instructions;
        private readonly Dictionary<ASInstruction, int> _indices;
        private readonly Dictionary<OPCode, List<ASInstruction>> _opGroups;

        public Dictionary<Jumper, ASInstruction> JumpExits { get; }
        public Dictionary<LookUpSwitchIns, ASInstruction[]> SwitchExits { get; }

        public bool IsReadOnly => false;
        public int Count => _instructions.Count;

        public ASInstruction this[int index]
        {
            get => _instructions[index];
            set
            {
                ASInstruction previous = _instructions[index];

                Jumper jumper = GetJumperEntry(previous);
                if (jumper != null)
                {
                    JumpExits[jumper] = value;
                }
                foreach (LookUpSwitchIns @switch in SwitchExits.Keys)
                {
                    ASInstruction[] exits = SwitchExits[@switch];
                    for (int i = 0; i < exits.Length; i++)
                    {
                        ASInstruction exit = exits[i];
                        if (previous != exit) continue;

                        exits[i] = value;
                    }
                }

                _indices.Remove(previous);
                _indices.Add(value, index);
                _instructions[index] = value;
            }
        }

        public ASCode(ABCFile abc, ASMethodBody body)
        {
            _abc = abc;
            _body = body;

            _instructions = new List<ASInstruction>();
            _indices = new Dictionary<ASInstruction, int>();
            _opGroups = new Dictionary<OPCode, List<ASInstruction>>();

            JumpExits = new Dictionary<Jumper, ASInstruction>();
            SwitchExits = new Dictionary<LookUpSwitchIns, ASInstruction[]>();

            LoadInstructions();
        }

        public void Add(ASInstruction instruction)
        {
            Insert(_instructions.Count, instruction);
        }
        public bool Remove(ASInstruction instruction)
        {
            if (_indices.TryGetValue(instruction, out int index))
            {
                RemoveAt(index);
            }
            return index > -1;
        }

        public void RemoveAt(int index)
        {
            RemoveRange(index, 1);
        }
        public void Insert(int index, ASInstruction instruction)
        {
            InsertRange(index, new[] { instruction });
        }

        public void RemoveRange(int index, int count)
        {
            if ((index + count) <= _instructions.Count)
            {
                for (int i = 0; i < count; i++)
                {
                    ASInstruction instruction = _instructions[index];
                    _indices.Remove(instruction);
                    _instructions.RemoveAt(index);

                    List<ASInstruction> group = _opGroups[instruction.OP];
                    if (group.Count == 1)
                    {
                        _opGroups.Remove(instruction.OP);
                    }
                    else group.Remove(instruction);

                    Jumper entry = GetJumperEntry(instruction);
                    if (entry != null)
                    {
                        if (index != _instructions.Count)
                        {
                            JumpExits[entry] = _instructions[index];
                        }
                        else JumpExits[entry] = null;
                    }
                    foreach (LookUpSwitchIns @switch in SwitchExits.Keys)
                    {
                        ASInstruction[] exits = SwitchExits[@switch];
                        for (int j = 0; j < exits.Length; j++)
                        {
                            ASInstruction exit = exits[j];
                            if (instruction != exit) continue;

                            exits[j] = _instructions[index];
                        }
                    }

                    if (Jumper.IsValid(instruction.OP))
                    {
                        JumpExits.Remove((Jumper)instruction);
                    }
                    else if (instruction.OP == OPCode.LookUpSwitch)
                    {
                        SwitchExits.Remove((LookUpSwitchIns)instruction);
                    }
                }
                for (int i = index; i < _indices.Count; i++)
                {
                    ASInstruction toPull = _instructions[i];
                    _indices[toPull] -= count;
                }
            }
        }
        public void AddRange(IEnumerable<ASInstruction> collection)
        {
            InsertRange(_instructions.Count, collection);
        }
        public void InsertRange(int index, IEnumerable<ASInstruction> collection)
        {
            if (index <= _instructions.Count && collection.Any())
            {
                var deadJumps = new Stack<Jumper>(JumpExits
                    .Where(je => je.Value == null)
                    .Select(je => je.Key));

                int count = _instructions.Count;
                if (index == _instructions.Count)
                {
                    _instructions.AddRange(collection);
                }
                else _instructions.InsertRange(index, collection);
                int collectionCount = (_instructions.Count - count);

                while (deadJumps.Count > 0)
                {
                    JumpExits[deadJumps.Pop()] = collection.First();
                }

                for (int i = (index + collectionCount - 1); i >= index; i--)
                {
                    ASInstruction instruction = _instructions[i];
                    _indices.Add(instruction, i);

                    if (!_opGroups.TryGetValue(instruction.OP, out List<ASInstruction> instructions))
                    {
                        instructions = new List<ASInstruction>();
                        _opGroups.Add(instruction.OP, instructions);
                    }
                    instructions.Add(instruction);
                }
                for (int i = (index + collectionCount); i < _instructions.Count; i++)
                {
                    ASInstruction toPush = _instructions[i];
                    _indices[toPush] += collectionCount;
                }
            }
        }

        public void Clear()
        {
            _indices.Clear();
            _opGroups.Clear();
            _instructions.Clear();

            JumpExits.Clear();
            SwitchExits.Clear();
        }
        public bool Contains(OPCode op)
        {
            return _opGroups.ContainsKey(op);
        }
        public bool Contains(ASInstruction instruction)
        {
            if (_opGroups.TryGetValue(instruction.OP, out List<ASInstruction> group))
            {
                return group.Contains(instruction);
            }
            return false;
        }

        public void CopyTo(ASInstruction[] array)
        {
            CopyTo(array, 0);
        }
        public void CopyTo(ASInstruction[] array, int arrayIndex)
        {
            CopyTo(0, array, arrayIndex, array.Length);
        }
        public void CopyTo(int index, ASInstruction[] array, int arrayIndex, int count)
        {
            _instructions.CopyTo(index, array, arrayIndex, count);
        }

        public int IndexOf(OPCode op)
        {
            if (_opGroups.ContainsKey(op))
            {
                List<ASInstruction> instructions = _opGroups[op];
                return _indices[instructions[0]];
            }
            return -1;
        }
        public int IndexOf(ASInstruction instruction)
        {
            if (_indices.ContainsKey(instruction))
            {
                return _indices[instruction];
            }
            return -1;
        }
        public bool StartsWith(params OPCode[] operations)
        {
            for (int i = 0; i < operations.Length; i++)
            {
                if (_instructions[i].OP != operations[i])
                {
                    return false;
                }
            }
            return (operations.Length > 0);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_instructions).GetEnumerator();
        }
        public IEnumerator<ASInstruction> GetEnumerator()
        {
            return _instructions.GetEnumerator();
        }

        public void Deobfuscate()
        {
            var machine = new ASMachine(_body.LocalCount + _body.Method.Parameters.Count + 1); // Scope + Parameters + Function Locals
            for (int i = 0; i <= _body.Method.Parameters.Count; i++) // Add the instance scope, and the method parameters to the register.
            {
                machine.Registers.Add(i, null);
            }

            var cleaned = new List<ASInstruction>(_instructions.Count);
            var valuePushers = new Stack<ASInstruction>(_body.MaxStack);
            var localReferences = new Dictionary<int, List<ASInstruction>>();
            var localConversions = new Dictionary<ASInstruction, List<Local>>();
            var swappedValues = new Dictionary<ASInstruction, ASInstruction[]>();
            KeyValuePair<Jumper, ASInstruction>[] jumpExits = JumpExits.ToArray();
            KeyValuePair<LookUpSwitchIns, ASInstruction[]>[] switchExits = SwitchExits.ToArray();
            for (int i = 0; i < _instructions.Count; i++)
            {
                ASInstruction instruction = _instructions[i];
                if (Jumper.IsValid(instruction.OP))
                {
                    i += GetFinalJumpCount(machine, (Jumper)instruction, cleaned, localReferences, valuePushers);
                }
                else
                {
                    if (instruction.OP == OPCode.NewFunction && _instructions[i + 1].OP == OPCode.Pop)
                    {
                        i++; // This function is not utilized, based on the upcoming instruction; Skip them both.
                        continue;
                    }

                    if (instruction.OP == OPCode.Not)
                    {
                        ASInstruction previousIns = cleaned[^1];
                        if (previousIns.OP == OPCode.PushTrue || previousIns.OP == OPCode.PushFalse)
                        {
                            valuePushers.Pop();
                            ASInstruction replacementIns = previousIns.OP == OPCode.PushTrue ? new PushFalseIns() : new PushTrueIns();

                            valuePushers.Push(replacementIns);
                            cleaned[^1] = replacementIns;

                            machine.Values.Pop();
                            replacementIns.Execute(machine);
                            continue;
                        }
                    }
                    if (Local.IsGetLocal(instruction.OP))
                    {
                        var local = (Local)instruction;
                        if (!machine.Registers.ContainsKey(local.Register))
                        {
                            instruction = new PushFalseIns();
                        }
                    }

                    instruction.Execute(machine);
                    #region Arithmetic Optimization
                    if (Computation.IsValid(instruction.OP))
                    {
                        object result = machine.Values.Pop();
                        ASInstruction rightPusher = valuePushers.Pop();
                        ASInstruction leftPusher = valuePushers.Pop();

                        if (!IsRelyingOnLocals(leftPusher, localConversions) && !IsRelyingOnLocals(rightPusher, localConversions) &&
                            !Local.IsValid(leftPusher.OP) && !Local.IsValid(rightPusher.OP) && result != null)
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
                            foreach (KeyValuePair<LookUpSwitchIns, ASInstruction[]> switchExit in switchExits)
                            {
                                ASInstruction[] cases = switchExit.Value;
                                for (int j = 0; j < cases.Length; j++)
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
                            // Do not attempt to optimize when a local is being used, because these values can still change.
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
                        if (Local.IsValid(pusher.OP) && instruction.OP != OPCode.PushScope)
                        {
                            if (!localConversions.TryGetValue(instruction, out List<Local> locals))
                            {
                                locals = new List<Local>();
                                localConversions.Add(instruction, locals);
                            }
                            locals.Add((Local)pusher);
                        }

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
                if (local.Key == 0) continue; // Scope
                if (local.Key <= (_body.Method.Parameters.Count)) continue; // Register == Param #(Non-zero based)
                List<ASInstruction> references = localReferences[local.Key];

                bool isNeeded = false;
                foreach (ASInstruction reference in references)
                {
                    // This checks if the local is being referenced by something else that retrieves the value.
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

            switchExits = SwitchExits.ToArray();
            foreach (KeyValuePair<LookUpSwitchIns, ASInstruction[]> switchExit in switchExits)
            {
                foreach (ASInstruction exit in switchExit.Value)
                {
                    if (IndexOf(switchExit.Key) > IndexOf(exit)) continue;
                    if (cleaned.Contains(switchExit.Key) && !cleaned.Contains(exit))
                    {
                        // TODO: Handle missing switch cases.
                    }
                }
            }

            jumpExits = JumpExits.ToArray(); // Global property could have been updated.
            foreach (KeyValuePair<Jumper, ASInstruction> jumpExit in jumpExits)
            {
                // This is not a forward jump instruction, since the exit appears before the jump.
                if (IndexOf(jumpExit.Key) > IndexOf(jumpExit.Value)) continue;

                // True if it still has the jump instruction, but the instruction needed to determine the final instruction to jump is not present.
                if (cleaned.Contains(jumpExit.Key) && !cleaned.Contains(jumpExit.Value))
                {
                    // Start at the index of the instruction that should come after the final instruction to jump.
                    for (int j = (_indices[jumpExit.Value] + 1); j < _instructions.Count; j++)
                    {
                        ASInstruction afterEnd = _instructions[j];
                        int exitIndex = cleaned.IndexOf(afterEnd);
                        if (exitIndex != -1) // Does this instruction exist in the cleaned output?; Otherwise, get instruction that comes after it.
                        {
                            JumpExits[jumpExit.Key] = cleaned[exitIndex];
                            break;
                        }
                    }
                }
            }

            _instructions.Clear();
            _instructions.AddRange(cleaned);
            Recalibrate();
        }
        public bool IsBackwardsJump(Jumper jumper)
        {
            return IndexOf(jumper) > IndexOf(JumpExits[jumper]);
        }
        public Jumper GetJumperEntry(ASInstruction exit)
        {
            foreach (KeyValuePair<Jumper, ASInstruction> jumpExit in JumpExits)
            {
                if (jumpExit.Value != exit) continue;
                return jumpExit.Key;
            }
            return null;
        }
        public ASInstruction[] GetJumpBlock(Jumper jumper)
        {
            int blockStart = (_indices[jumper] + 1);
            int scopeEnd = _indices[JumpExits[jumper]];

            var body = new ASInstruction[scopeEnd - blockStart];
            _instructions.CopyTo(blockStart, body, 0, body.Length);

            return body;
        }
        public LookUpSwitchIns GetSwitchEntry(ASInstruction exit)
        {
            foreach (KeyValuePair<LookUpSwitchIns, ASInstruction[]> switchExit in SwitchExits)
            {
                if (!switchExit.Value.Contains(exit)) continue;
                return switchExit.Key;
            }
            return null;
        }

        public IEnumerable<ASInstruction> GetOPGroup(OPCode op)
        {
            if (_opGroups.ContainsKey(op))
            {
                return _opGroups[op];
            }
            return Enumerable.Empty<ASInstruction>();
        }
        public SortedDictionary<OPCode, List<ASInstruction>> GetOPGroups()
        {
            var groups = new SortedDictionary<OPCode, List<ASInstruction>>();
            foreach (OPCode op in _opGroups.Keys)
            {
                groups[op] = new List<ASInstruction>();
                groups[op].AddRange(_opGroups[op]);
            }
            return groups;
        }

        private void Recalibrate()
        {
            _indices.Clear();
            _opGroups.Clear();
            for (int i = 0; i < _instructions.Count; i++)
            {
                ASInstruction instruction = _instructions[i];
                _indices.Add(instruction, i);

                if (!_opGroups.TryGetValue(instruction.OP, out List<ASInstruction> instructions))
                {
                    instructions = new List<ASInstruction>();
                    _opGroups.Add(instruction.OP, instructions);
                }
                instructions.Add(instruction);
            }
        }
        private void LoadInstructions()
        {
            var marks = new Dictionary<long, ASInstruction>();
            var sharedExits = new Dictionary<long, List<Jumper>>();
            var switchCases = new Dictionary<long, List<(LookUpSwitchIns, int)>>();
            using (var input = new FlashReader(_body.Code))
            {
                while (input.IsDataAvailable)
                {
                    long previousPosition = input.Position;
                    var instruction = ASInstruction.Create(_abc, input);
                    marks[previousPosition] = instruction;

                    _indices.Add(instruction, _indices.Count);
                    _instructions.Add(instruction);

                    if (!_opGroups.TryGetValue(instruction.OP, out List<ASInstruction> instructions))
                    {
                        instructions = new List<ASInstruction>();
                        _opGroups.Add(instruction.OP, instructions);
                    }
                    instructions.Add(instruction);

                    if (sharedExits.TryGetValue(previousPosition, out List<Jumper> jumpers))
                    {
                        // This is an exit position two or more jump instructions.
                        foreach (Jumper jumper in jumpers)
                        {
                            JumpExits.Add(jumper, instruction);
                        }
                        sharedExits.Remove(previousPosition);
                    }

                    if (switchCases.TryGetValue(previousPosition, out List<(LookUpSwitchIns, int)> caseExits))
                    {
                        foreach ((LookUpSwitchIns owner, int index) in caseExits)
                        {
                            SwitchExits[owner][index] = instruction;
                        }
                        switchCases.Remove(previousPosition);
                    }

                    if (instruction.OP == OPCode.LookUpSwitch)
                    {
                        var lookUpSwitchIns = (LookUpSwitchIns)instruction;
                        var offsets = new List<uint>(lookUpSwitchIns.CaseOffsets) { lookUpSwitchIns.DefaultOffset };

                        var exits = new ASInstruction[offsets.Count];
                        for (int i = 0; i < offsets.Count; i++)
                        {
                            long exitPosition = previousPosition + offsets[i];
                            if (exitPosition <= input.Length)
                            {
                                if (!switchCases.TryGetValue(exitPosition, out caseExits))
                                {
                                    caseExits = new List<(LookUpSwitchIns, int)>();
                                    switchCases.Add(exitPosition, caseExits);
                                }
                                caseExits.Add((lookUpSwitchIns, i));
                            }
                            else exits[i] = marks[exitPosition - uint.MaxValue - 1];
                        }
                        SwitchExits.Add(lookUpSwitchIns, exits);
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
                            if (!sharedExits.TryGetValue(exitPosition, out jumpers))
                            {
                                jumpers = new List<Jumper>();
                                sharedExits.Add(exitPosition, jumpers);
                            }
                            jumpers.Add(jumper);
                        }
                        else // Backwards jump.
                        {
                            long markIndex = exitPosition - uint.MaxValue - 1;
                            if (marks[markIndex].OP == OPCode.Label)
                            {
                                var label = (LabelIns)marks[markIndex];
                                JumpExits.Add(jumper, label);
                            }
                            // TODO: Check if not adding an impossible label is fine...
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
        private bool IsRelyingOnLocals(ASInstruction instruction, Dictionary<ASInstruction, List<Local>> conversions) => conversions.GetValueOrDefault(instruction) != null;
        private int GetFinalJumpCount(ASMachine machine, Jumper jumper, List<ASInstruction> cleaned, Dictionary<int, List<ASInstruction>> localReferences, Stack<ASInstruction> valuePushers)
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
                    else if (Primitive.IsValid(pusher.OP) || pusher.OP == OPCode.Dup)
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
            bool isBackwardsJump = false;
            IEnumerable<ASInstruction> block = null;
            if (!JumpExits.TryGetValue(jumper, out ASInstruction exit))
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
                block = jumper.Offset > 0 ? GetJumpBlock(jumper) : null;
            }

            if (isJumping == true && block != null)
            {
                if (isBackwardsJump)
                {
                    // Check if any of the locals used by the jump instruction is being set within the body.
                    // If the answer is yes, removing the jump instruction is a bad idea, since the output of the condition could change.
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

                    ASInstruction afterLast = _instructions[_indices[block.Last()] + 1];
                    bool isExitAfterLast = (jumpExit.Value == afterLast); // Does the exit of the jump that is in the block come after the final instruction of the current block?

                    if (hasEntry && !hasExit && !isExitAfterLast ||
                        hasExit && !hasEntry)
                    {
                        // Keep the jump instruction, since it will corrupt the other jump instruction that is using it.
                        cleaned.Add(jumper);
                        return 0;
                    }
                }
                foreach (KeyValuePair<LookUpSwitchIns, ASInstruction[]> switchExit in SwitchExits)
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

        public override void WriteTo(FlashWriter output)
        {
            var marks = new Dictionary<ASInstruction, long>();
            var sharedExits = new Dictionary<ASInstruction, List<ASInstruction>>();
            var switchCases = new Dictionary<ASInstruction, (LookUpSwitchIns, int)>();
            foreach (ASInstruction instruction in _instructions)
            {
                long previousPosition = output.Position;
                marks.Add(instruction, previousPosition);
                instruction.WriteTo(output);

                if (sharedExits.TryGetValue(instruction, out List<ASInstruction> jumpers))
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

                if (switchCases.TryGetValue(instruction, out (LookUpSwitchIns owner, int index) caseExits))
                {
                    long position = marks[caseExits.owner];
                    var fixedOffset = (uint)(previousPosition - position); // Where the instruction starts.
                    int defaultIndex = caseExits.owner.CaseOffsets.IndexOf(caseExits.owner.DefaultOffset);

                    if (caseExits.index == defaultIndex || caseExits.index == caseExits.owner.CaseOffsets.Count)
                    {
                        caseExits.owner.DefaultOffset = fixedOffset;
                    }
                    else
                    {
                        caseExits.owner.CaseOffsets[caseExits.index] = fixedOffset;
                    }
                    Rewrite(output, caseExits.owner, position);
                }

                if (instruction.OP == OPCode.LookUpSwitch)
                {
                    bool requiresRewrite = false;
                    var lookUpSwitch = (LookUpSwitchIns)instruction;

                    ASInstruction[] cases = SwitchExits[lookUpSwitch];
                    for (int i = 0; i < cases.Length; i++)
                    {
                        ASInstruction exit = cases[i];
                        if (exit.OP != OPCode.Label)
                        {
                            switchCases.Add(exit, (lookUpSwitch, i));
                        }
                        else if (exit.OP == OPCode.Label)
                        {
                            requiresRewrite = true;
                            long exitPosition = marks[exit];
                            long jumpCount = (previousPosition - (exitPosition + 1));

                            uint fixedOffset = (uint)(uint.MaxValue - jumpCount);
                            if (i == (cases.Length - 1))
                            {
                                lookUpSwitch.DefaultOffset = fixedOffset;
                            }
                            else
                            {
                                lookUpSwitch.CaseOffsets[i] = fixedOffset;
                            }
                        }
                    }
                    if (requiresRewrite)
                    {
                        Rewrite(output, lookUpSwitch, previousPosition);
                    }
                }
                else if (Jumper.IsValid(instruction.OP))
                {
                    var jumper = (Jumper)instruction;
                    if (jumper.Offset == 0) continue;

                    if (!JumpExits.TryGetValue(jumper, out ASInstruction exit))
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