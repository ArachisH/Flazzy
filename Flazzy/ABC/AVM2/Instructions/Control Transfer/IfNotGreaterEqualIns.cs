using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class IfNotGreaterEqualIns : Jumper
    {
        public IfNotGreaterEqualIns()
            : base(OPCode.IfNGe)
        { }
        public IfNotGreaterEqualIns(ref FlashReader input)
            : base(OPCode.IfNGe, ref input)
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