using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class SetSuperIns : ASInstruction
    {
        public ASMultiname PropertyName
        {
            get { return ABC.Pool.Multinames[PropertyNameIndex]; }
        }
        public int PropertyNameIndex { get; set; }

        public SetSuperIns(ABCFile abc)
            : base(OPCode.SetSuper, abc)
        { }
        public SetSuperIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            PropertyNameIndex = input.ReadInt30();
        }

        public override int GetPopCount()
        {
            return (2 + ResolveMultinamePops(PropertyName));
        }
        public override void Execute(ASMachine machine)
        {
            object value = machine.Values.Pop();
            ResolveMultiname(machine, PropertyName);
            object obj = machine.Values.Pop();
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(PropertyNameIndex);
        }
    }
}