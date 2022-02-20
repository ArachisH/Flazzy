using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class NewCatchIns : ASInstruction
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

        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(null);
        }

        protected override int GetBodySize()
        {
            return FlashWriter.GetEncodedIntSize(ExceptionIndex);
        }
        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(ExceptionIndex);
        }
    }
}