﻿using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class IfEqualIns : Jumper
{
    public IfEqualIns()
        : base(OPCode.IfEq)
    { }
    public IfEqualIns(ref SpanFlashReader input)
        : base(OPCode.IfEq, ref input)
    { }

    public override bool? RunCondition(ASMachine machine)
    {
        dynamic right = machine.Values.Pop();
        dynamic left = machine.Values.Pop();
        if (left == null || right == null) return null;

        return (left == right);
    }
}