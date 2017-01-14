using System;

using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfEqualIns : Brancher
    {
        public IfEqualIns()
            : base(OPCode.IfEq)
        { }
        public IfEqualIns(FlashReader input)
            : base(OPCode.IfEq, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var right = (machine.Values.Pop() as IComparable);
            var left = (machine.Values.Pop() as IComparable);
            if (left == null || right == null) return null;

            return (left.CompareTo(right) == 0);
        }
    }
}