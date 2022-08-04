using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfNotLessThanIns : Jumper
    {
        public IfNotLessThanIns()
            : base(OPCode.IfNLt)
        { }
        public IfNotLessThanIns(FlashReader input)
            : base(OPCode.IfNLt, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var right = machine.Values.Pop();
            var left = machine.Values.Pop();
            if (left == null || right == null) return null;

            return !(Convert.ToDouble(left) < Convert.ToDouble(right));
        }
    }
}