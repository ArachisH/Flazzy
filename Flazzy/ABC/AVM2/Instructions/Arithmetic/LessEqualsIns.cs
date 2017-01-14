using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class LessEqualsIns : ArithmeticInstruction
    {
        public LessEqualsIns()
            : base(OPCode.LessEquals)
        { }

        protected override object Execute(object left, object right)
        {
            var cLeft = (left as IComparable);
            var cRight = (right as IComparable);
            if (cLeft == null || cRight == null) return null;

            return (cLeft.CompareTo(cRight) <= 0);
        }
    }
}