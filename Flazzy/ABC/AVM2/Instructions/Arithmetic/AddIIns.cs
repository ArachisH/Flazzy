using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class AddIIns : ArithmeticInstruction
    {
        public AddIIns()
            : base(OPCode.Add_i)
        { }

        protected override object Execute(object left, object right)
        {
            var iLeft = Convert.ToInt32(left);
            var iRight = Convert.ToInt32(right);

            return (iLeft + iRight);
        }
    }
}