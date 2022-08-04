using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfNotGreaterThanIns : Jumper
    {
        public IfNotGreaterThanIns()
            : base(OPCode.IfNGt)
        { }
        public IfNotGreaterThanIns(FlashReader input)
            : base(OPCode.IfNGt, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var right = machine.Values.Pop();
            var left = machine.Values.Pop();
            if (left == null || right == null) return null;

            return !(Convert.ToDouble(left) > Convert.ToDouble(right));
        }
    }
}