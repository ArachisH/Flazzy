using Flazzy.IO;
using Flazzy.ABC.AVM2.Instructions.Containers;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class CallPropVoidIns : ASInstruction, IPropertyContainer
    {
        public int PropertyNameIndex { get; set; }
        public ASMultiname PropertyName => ABC.Pool.Multinames[PropertyNameIndex];

        public int ArgCount { get; set; }

        public CallPropVoidIns(ABCFile abc)
            : base(OPCode.CallPropVoid, abc)
        { }
        public CallPropVoidIns(ABCFile abc, ref FlashReader input)
            : this(abc)
        {
            PropertyNameIndex = input.ReadEncodedInt();
            ArgCount = input.ReadEncodedInt();
        }
        public CallPropVoidIns(ABCFile abc, int propertyNameIndex)
            : this(abc)
        {
            PropertyNameIndex = propertyNameIndex;
        }
        public CallPropVoidIns(ABCFile abc, int propertyNameIndex, int argCount)
            : this(abc)
        {
            PropertyNameIndex = propertyNameIndex;
            ArgCount = argCount;
        }

        public override int GetPopCount()
        {
            return (ArgCount + ResolveMultinamePops(PropertyName) + 1);
        }
        public override void Execute(ASMachine machine)
        {
            for (int i = 0; i < ArgCount; i++)
            {
                machine.Values.Pop();
            }
            ResolveMultiname(machine, PropertyName);
            object obj = machine.Values.Pop();
        }

        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(PropertyNameIndex);
            output.WriteEncodedInt(ArgCount);
        }
    }
}