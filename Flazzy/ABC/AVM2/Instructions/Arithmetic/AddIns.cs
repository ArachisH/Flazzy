using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class AddIns : ArithmeticInstruction
    {
        public AddIns()
            : base(OPCode.Add)
        { }

        protected override object Execute(object left, object right)
        {
            var iLeft = Convert.ToInt32(left);
            var iRight = Convert.ToInt32(right);

            return (iLeft + iRight);
        }
    }
}