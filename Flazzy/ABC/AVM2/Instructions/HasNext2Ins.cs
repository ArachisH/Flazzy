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
        public HasNext2Ins(FlashReader input)
            : this()
        {
            ObjectIndex = input.ReadInt30();
            RegisterIndex = input.ReadInt30();
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

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(ObjectIndex);
            output.WriteInt30(RegisterIndex);
        }
    }
}