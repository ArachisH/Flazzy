using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfStrictNotEqualIns : Jumper
    {
        public IfStrictNotEqualIns()
            : base(OPCode.IfStrictNE)
        { }
        public IfStrictNotEqualIns(ref FlashReader input)
            : base(OPCode.IfStrictNE, ref input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            //var right = (machine.Values.Pop() as IComparable);
            //var left = (machine.Values.Pop() as IComparable);
            //if (left == null || right == null) return null;

            //return !left.Equals(right);
            return null;
        }
    }
}