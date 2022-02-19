using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class DebugLineIns : ASInstruction
    {
        public int LineNumber { get; set; }

        public DebugLineIns()
            : base(OPCode.DebugLine)
        { }
        public DebugLineIns(int lineNumber)
            : this()
        {
            LineNumber = lineNumber;
        }
        public DebugLineIns(ref FlashReader input)
            : this()
        {
            LineNumber = input.ReadEncodedInt();
        }

        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(LineNumber);
        }
    }
}