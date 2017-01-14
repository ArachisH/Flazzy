using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class ModuloIns : ArithmeticInstruction
    {
        public ModuloIns()
            : base(OPCode.Modulo)
        { }

        protected override object Execute(object left, object right)
        {
            var dLeft = Convert.ToDouble(left);
            var dRight = Convert.ToDouble(right);
            return (dLeft % dRight);
        }
    }
}