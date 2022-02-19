﻿using Flazzy.IO;

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
        public ApplyTypeIns(ref FlashReader input)
            : this()
        {
            ParamCount = input.ReadEncodedInt();
        }

        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(ParamCount);
        }
    }
}