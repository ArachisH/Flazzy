﻿namespace Flazzy.ABC.AVM2.Instructions;

public sealed class MultiplyIIns : Computation
{
    public MultiplyIIns()
        : base(OPCode.Multiply_i)
    { }

    protected override object Execute(dynamic left, dynamic right) => left * right;
}