using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushNaNIns : OperandPusher
    {
        public override object Value
        {
            get { return double.NaN; }
            set { throw new NotSupportedException(); }
        }

        public PushNaNIns()
            : base(OPCode.PushNan)
        { }
    }
}