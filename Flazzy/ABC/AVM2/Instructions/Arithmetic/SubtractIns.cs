using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class SubtractIns : ArithmeticInstruction
    {
        public SubtractIns()
            : base(OPCode.Subtract)
        { }

        protected override object Execute(object left, object right)
        {
            var dLeft = Convert.ToDouble(left);
            var dRight = Convert.ToDouble(right);

            return (dLeft - dRight);
        }
    }
}