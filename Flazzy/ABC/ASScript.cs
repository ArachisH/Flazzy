using System;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASScript : AS3Item
    {
        public ASScript(ABCFile abc) : base(abc)
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