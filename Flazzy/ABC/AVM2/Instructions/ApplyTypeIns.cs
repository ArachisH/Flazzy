using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class ApplyTypeIns : ASInstruction
    {
        public int ParamCount { get; set; }

        public ApplyTypeIns()
            : base(OPCode.ApplyType)
        { }
        public ApplyTypeIns(int paramCount)
            : this()
        {
            ParamCount = paramCount;
        }
        public ApplyTypeIns(FlashReader input)
            : this()
        {
            ParamCount = input.ReadInt30();
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(ParamCount);
        }
    }
}