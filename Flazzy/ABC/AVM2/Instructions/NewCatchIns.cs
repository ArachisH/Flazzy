using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class NewCatchIns : ASInstruction
    {
        public int ExceptionIndex { get; set; }

        public NewCatchIns()
            : base(OPCode.NewCatch)
        { }
        public NewCatchIns(FlashReader input)
            : this()
        {
            ExceptionIndex = input.ReadInt30();
        }
        public NewCatchIns(int exceptionIndex)
            : this()
        {
            ExceptionIndex = exceptionIndex;
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
            output.WriteInt30(ExceptionIndex);
        }
    }
}