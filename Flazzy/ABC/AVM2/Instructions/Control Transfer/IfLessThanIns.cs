using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfLessThanIns : Jumper
    {
        public IfLessThanIns()
            : base(OPCode.IfLt)
        { }
        public IfLessThanIns(FlashReader input)
            : base(OPCode.IfLt, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var right = machine.Values.Pop();
            var left = machine.Values.Pop();
            if (left == null || right == null) return null;

            return Convert.ToDouble(left) < Convert.ToDouble(right);
        }
    }
}