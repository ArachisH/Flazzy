﻿using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushTrueIns : OperandPusher
    {
        public override object Value
        {
            get { return true; }
            set { throw new NotSupportedException(); }
        }

        public PushTrueIns()
            : base(OPCode.PushTrue)
        { }
    }
}