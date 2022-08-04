using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfLessEqualIns : Jumper
    {
        public IfLessEqualIns()
            : base(OPCode.IfLe)
        { }
        public IfLessEqualIns(FlashReader input)
            : base(OPCode.IfLe, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var right = machine.Values.Pop();
            var left = machine.Values.Pop();
            if (left == null || right == null) return null;

            return (Convert.ToDouble(left) <= Convert.ToDouble(right));
        }
    }
}