using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class NewCatchIns : ASInstruction
    {
        public int ExceptionIndex { get; set; }

        public NewCatchIns()
            : base(OPCode.NewCatch)
        { }
        public NewCatchIns(ref FlashReader input)
            : this()
        {
            ExceptionIndex = input.ReadEncodedInt();
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

        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(ExceptionIndex);
        }
    }
}