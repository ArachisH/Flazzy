﻿namespace Flazzy.ABC.AVM2.Instructions
{
    public class ModuloIns : Computation
    {
        public ModuloIns()
            : base(OPCode.Modulo)
        { }

        protected override object Execute(dynamic left, dynamic right)
        {
            return (left % right);
        }
    }
}