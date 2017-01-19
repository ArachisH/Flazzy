using System;

using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfEqualIns : Jumper
    {
        public IfEqualIns()
            : base(OPCode.IfEq)
        { }
        public IfEqualIns(FlashReader input)
            : base(OPCode.IfEq, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var right = machine.Values.Pop();
            var left = machine.Values.Pop();
            if (left == null || right == null) return null;

            return (Convert.ToDouble(left) == Convert.ToDouble(right));
        }
    }
}