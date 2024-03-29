﻿namespace Flazzy.ABC.AVM2.Instructions;

public sealed class EqualsIns : Computation
{
    public EqualsIns()
        : base(OPCode.Equals)
    { }

    protected override object Execute(dynamic left, dynamic right) => left == right;
}