﻿using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class IfNotEqualIns : Jumper
{
    public IfNotEqualIns()
        : base(OPCode.IfNe)
    { }
    public IfNotEqualIns(ref SpanFlashReader input)
        : base(OPCode.IfNe, ref input)
    { }

    public override bool? RunCondition(ASMachine machine)
    {
        dynamic right = machine.Values.Pop();
        dynamic left = machine.Values.Pop();
        if (left == null || right == null) return null;

        return left != right;
    }
}