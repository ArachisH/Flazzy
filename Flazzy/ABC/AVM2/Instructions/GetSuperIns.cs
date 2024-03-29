﻿using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class GetSuperIns : ASInstruction
{
    public int PropertyNameIndex { get; set; }
    public ASMultiname PropertyName => ABC.Pool.Multinames[PropertyNameIndex];

    public GetSuperIns(ABCFile abc)
        : base(OPCode.GetSuper, abc)
    { }
    public GetSuperIns(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        PropertyNameIndex = input.ReadEncodedInt();
    }
    public GetSuperIns(ABCFile abc, int propertyNameIndex)
        : this(abc)
    {
        PropertyNameIndex = propertyNameIndex;
    }

    public override int GetPopCount()
    {
        return ResolveMultinamePops(PropertyName) + 1;
    }
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        ResolveMultiname(machine, PropertyName);
        object obj = machine.Values.Pop();
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(PropertyNameIndex);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(PropertyNameIndex);
    }
}