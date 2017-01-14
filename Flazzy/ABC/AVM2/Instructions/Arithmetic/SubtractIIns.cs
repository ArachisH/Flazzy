using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class SubtractIIns : ArithmeticInstruction
    {
        public SubtractIIns()
            : base(OPCode.Subtract_i)
        { }

        protected override object Execute(object left, object right)
        {
            var dLeft = Convert.ToInt32(left);
            var dRight = Convert.ToInt32(right);

            return (dLeft - dRight);
        }
    }
}