﻿namespace Flazzy.ABC.AVM2.Instructions;

public sealed class ConvertUIns : ASInstruction
{
    public ConvertUIns()
        : base(OPCode.Convert_u)
    { }

    public override int GetPopCount() => 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object result = null;
        dynamic value = machine.Values.Pop();
        if (value != null)
        {
            result = (uint)Convert.ToDouble(value);
        }
        machine.Values.Push(result);
    }
}