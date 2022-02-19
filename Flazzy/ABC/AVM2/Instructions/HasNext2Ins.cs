using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class HasNext2Ins : ASInstruction
    {
        public int ObjectIndex { get; set; }
        public int RegisterIndex { get; set; }

        public HasNext2Ins()
            : base(OPCode.HasNext2)
        { }
        public HasNext2Ins(ref FlashReader input)
            : this()
        {
            ObjectIndex = input.ReadEncodedInt();
            RegisterIndex = input.ReadEncodedInt();
        }
        public HasNext2Ins(int objectIndex, int registerIndex)
            : this()
        {
            ObjectIndex = objectIndex;
            RegisterIndex = registerIndex;
        }

        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(ObjectIndex);
            output.WriteEncodedInt(RegisterIndex);
        }
    }
}