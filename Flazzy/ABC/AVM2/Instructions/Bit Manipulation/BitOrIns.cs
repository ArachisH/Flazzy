﻿namespace Flazzy.ABC.AVM2.Instructions;

public sealed class BitOrIns : ASInstruction
{
    public BitOrIns()
        : base(OPCode.BitOr)
    { }

    public override int GetPopCount() => 2;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object result = null;
        object right = machine.Values.Pop();
        object left = machine.Values.Pop();
        if (right != null && left != null)
        {
            int iLeft = Convert.ToInt32(left);
            int iRight = Convert.ToInt32(right);
            result = iLeft | iRight;
        }
        machine.Values.Push(result);
    }
}