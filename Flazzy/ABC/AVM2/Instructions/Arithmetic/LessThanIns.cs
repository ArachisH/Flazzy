﻿namespace Flazzy.ABC.AVM2.Instructions;

public sealed class LessThanIns : Computation
{
    public LessThanIns()
        : base(OPCode.LessThan)
    { }

    protected override object Execute(dynamic left, dynamic right) => left < right;
}