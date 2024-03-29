﻿namespace Flazzy.ABC.AVM2.Instructions;

public sealed class PushUndefinedIns : ASInstruction
{
    public PushUndefinedIns()
        : base(OPCode.PushUndefined)
    { }

    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        machine.Values.Push(null);
    }
}