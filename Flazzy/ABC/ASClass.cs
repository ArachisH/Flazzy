﻿using System;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASClass : AS3Item
    {
        public ASClass(ABCFile abc)
            : base(abc)
        {
        }

        public override string ToAS3()
        {
            throw new NotImplementedException();
        }

        public override void WriteTo(FlashWriter output)
        {
            throw new NotImplementedException();
        }
    }
}