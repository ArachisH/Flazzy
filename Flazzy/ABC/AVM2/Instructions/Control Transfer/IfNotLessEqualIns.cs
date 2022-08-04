using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfNotLessEqualIns : Jumper
    {
        public IfNotLessEqualIns()
            : base(OPCode.IfNLe)
        { }
        public IfNotLessEqualIns(FlashReader input)
            : base(OPCode.IfNLe, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var right = machine.Values.Pop();
            var left = machine.Values.Pop();
            if (left == null || right == null) return null;

            return !(Convert.ToDouble(left) <= Convert.ToDouble(right));
        }
    }
}