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
        public DebugLineIns(FlashReader input)
            : this()
        {
            LineNumber = input.ReadInt30();
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(LineNumber);
        }
    }
}