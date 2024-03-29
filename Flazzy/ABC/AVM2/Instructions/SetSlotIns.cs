﻿using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class SetSlotIns : ASInstruction
{
    public int SlotIndex { get; set; }

    public SetSlotIns()
        : base(OPCode.SetSlot)
    { }
    public SetSlotIns(int slotIndex)
        : this()
    {
        SlotIndex = slotIndex;
    }
    public SetSlotIns(ref SpanFlashReader input)
        : this()
    {
        SlotIndex = input.ReadEncodedInt();
    }

    public override int GetPopCount() => 2;
    public override void Execute(ASMachine machine)
    {
        object value = machine.Values.Pop();
        object obj = machine.Values.Pop();
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(SlotIndex);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(SlotIndex);
    }
}