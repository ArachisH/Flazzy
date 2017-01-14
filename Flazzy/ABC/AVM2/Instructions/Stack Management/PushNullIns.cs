using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushNullIns : OperandPusher
    {
        public override object Value
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }

        public PushNullIns()
            : base(OPCode.PushNull)
        { }
    }
}