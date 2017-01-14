using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class MultiplyIns : ArithmeticInstruction
    {
        public MultiplyIns()
            : base(OPCode.Multiply)
        { }

        protected override object Execute(object left, object right)
        {
            var iLeft = Convert.ToDouble(left);
            var iRight = Convert.ToDouble(right);
            return (iLeft * iRight);
        }
    }
}