﻿using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class ConstructPropIns : ASInstruction
    {
        public int PropertyNameIndex { get; set; }
        public ASMultiname PropertyName => ABC.Pool.Multinames[PropertyNameIndex];

        public int ArgCount { get; set; }

        public ConstructPropIns(ABCFile abc)
            : base(OPCode.ConstructProp, abc)
        { }
        public ConstructPropIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            PropertyNameIndex = input.ReadInt30();
            ArgCount = input.ReadInt30();
        }
        public ConstructPropIns(ABCFile abc, int propertyNameIndex)
            :this(abc)
        {
            PropertyNameIndex = propertyNameIndex;
        }
        public ConstructPropIns(ABCFile abc, int propertyNameIndex, int argCount)
            : this(abc)
        {
            PropertyNameIndex = propertyNameIndex;
            ArgCount = argCount;
        }

        public override int GetPopCount()
        {
            return (ArgCount + ResolveMultinamePops(PropertyName) + 1);
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            for (int i = 0; i < ArgCount; i++)
            {
                machine.Values.Pop();
            }
            ResolveMultiname(machine, PropertyName);
            object obj = machine.Values.Pop();
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(PropertyNameIndex);
            output.WriteInt30(ArgCount);
        }
    }
}