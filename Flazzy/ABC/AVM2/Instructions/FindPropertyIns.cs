using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class FindPropertyIns : Instruction
    {
        public ASMultiname PropertyName
        {
            get { return ABC.Pool.Multinames[PropertyNameIndex]; }
        }
        public int PropertyNameIndex { get; set; }

        public FindPropertyIns(ABCFile abc)
            : base(OPCode.FindProperty, abc)
        { }
        public FindPropertyIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            PropertyNameIndex = input.ReadInt30();
        }

        public override int GetPopCount()
        {
            return ResolveMultinamePops(PropertyName);
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            ResolveMultiname(machine, PropertyName);
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(PropertyNameIndex);
        }
    }
}