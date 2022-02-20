using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class DeletePropertyIns : ASInstruction
    {
        public int PropertyNameIndex { get; set; }
        public ASMultiname PropertyName => ABC.Pool.Multinames[PropertyNameIndex];

        public DeletePropertyIns(ABCFile abc)
            : base(OPCode.DeleteProperty, abc)
        { }
        public DeletePropertyIns(ABCFile abc, ref FlashReader input)
            : this(abc)
        {
            PropertyNameIndex = input.ReadEncodedInt();
        }
        public DeletePropertyIns(ABCFile abc, int propertyNameIndex)
            : this(abc)
        {
            PropertyNameIndex = propertyNameIndex;
        }

        public override int GetPopCount()
        {
            return ResolveMultinamePops(PropertyName) + 1;
        }
        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            ResolveMultiname(machine, PropertyName);
            object obj = machine.Values.Pop();
            machine.Values.Push(null);
        }

        protected override int GetBodySize()
        {
            return FlashWriter.GetEncodedIntSize(PropertyNameIndex);
        }
        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(PropertyNameIndex);
        }
    }
}