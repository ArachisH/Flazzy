using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class MultiplyIIns : ArithmeticInstruction
    {
        public MultiplyIIns()
            : base(OPCode.Multiply_i)
        { }

        protected override object Execute(object left, object right)
        {
            var iLeft = Convert.ToInt32(left);
            var iRight = Convert.ToInt32(right);
            return (iLeft * iRight);
        }
    }
}