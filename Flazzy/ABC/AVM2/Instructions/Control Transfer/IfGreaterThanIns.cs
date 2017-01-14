using System;

using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfGreaterThanIns : Brancher
    {
        public IfGreaterThanIns()
            : base(OPCode.IfGt)
        { }
        public IfGreaterThanIns(FlashReader input)
            : base(OPCode.IfGt, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var right = (machine.Values.Pop() as IComparable);
            var left = (machine.Values.Pop() as IComparable);
            if (left == null || right == null) return null;

            return (left.CompareTo(right) > 0);
        }
    }
}