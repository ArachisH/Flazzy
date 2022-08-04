using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfNotGreaterEqualIns : Jumper
    {
        public IfNotGreaterEqualIns()
            : base(OPCode.IfNGe)
        { }
        public IfNotGreaterEqualIns(FlashReader input)
            : base(OPCode.IfNGe, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var right = machine.Values.Pop();
            var left = machine.Values.Pop();
            if (left == null || right == null) return null;

            return !(Convert.ToDouble(left) >= Convert.ToDouble(right));
        }
    }
}