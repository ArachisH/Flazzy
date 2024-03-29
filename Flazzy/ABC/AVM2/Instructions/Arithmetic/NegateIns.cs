﻿namespace Flazzy.ABC.AVM2.Instructions;

public sealed class NegateIns : ASInstruction
{
    public NegateIns()
        : base(OPCode.Negate)
    { }

    public override int GetPopCount() => 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object result = null;
        object value = machine.Values.Pop();
        if (value != null)
        {
            result = Convert.ToDouble(value) * -1;
        }
        machine.Values.Push(result);
    }
}