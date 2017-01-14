using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class DivideIns : ArithmeticInstruction
    {
        public DivideIns()
            : base(OPCode.Divide)
        { }

        protected override object Execute(object left, object right)
        {
            double dLeft = Convert.ToDouble(left);
            double dRight = Convert.ToDouble(right);

            return (dLeft / dRight);
        }
    }
}