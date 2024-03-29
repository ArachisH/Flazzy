﻿namespace Flazzy.ABC.AVM2.Instructions;

public sealed class IncrementIIns : ASInstruction
{
    public IncrementIIns()
        : base(OPCode.Increment_i)
    { }

    public override int GetPopCount() => 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object result = null;
        object value = machine.Values.Pop();
        if (value != null)
        {
            result = Convert.ToInt32(value) + 1;
        }
        machine.Values.Push(result);
    }
}