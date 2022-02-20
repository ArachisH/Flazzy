using Flazzy.IO;
using Flazzy.ABC.AVM2.Instructions.Containers;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class CallPropertyIns : ASInstruction, IPropertyContainer
    {
        public int PropertyNameIndex { get; set; }
        public ASMultiname PropertyName => ABC.Pool.Multinames[PropertyNameIndex];

        public int ArgCount { get; set; }

        public CallPropertyIns(ABCFile abc)
            : base(OPCode.CallProperty, abc)
        { }
        public CallPropertyIns(ABCFile abc, ref FlashReader input)
            : this(abc)
        {
            PropertyNameIndex = input.ReadEncodedInt();
            ArgCount = input.ReadEncodedInt();
        }
        public CallPropertyIns(ABCFile abc, int propertyNameIndex)
            : this(abc)
        {
            PropertyNameIndex = propertyNameIndex;
        }
        public CallPropertyIns(ABCFile abc, int propertyNameIndex, int argCount)
           : this(abc)
        {
            PropertyNameIndex = propertyNameIndex;
            ArgCount = argCount;
        }

        public override int GetPopCount()
        {
            return ArgCount + ResolveMultinamePops(PropertyName) + 1;
        }
        public override int GetPushCount() => 1;
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

        protected override int GetBodySize()
        {
            int size = 0;
            size += FlashWriter.GetEncodedIntSize(PropertyNameIndex);
            size += FlashWriter.GetEncodedIntSize(ArgCount);
            return size;
        }
        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(PropertyNameIndex);
            output.WriteEncodedInt(ArgCount);
        }
    }
}