﻿using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class PushUIntIns : Primitive
{
    private uint _value;
    new public uint Value
    {
        get => _value;
        set
        {
            _value = value;
            _valueIndex = ABC.Pool.AddConstant(value);

            base.Value = value;
        }
    }

    private int _valueIndex;
    public int ValueIndex
    {
        get => _valueIndex;
        set
        {
            _valueIndex = value;
            _value = ABC.Pool.UIntegers[value];

            base.Value = _value;
        }
    }

    public PushUIntIns(ABCFile abc)
        : base(OPCode.PushUInt, abc)
    { }
    public PushUIntIns(ABCFile abc, uint value)
        : this(abc)
    {
        Value = value;
    }
    public PushUIntIns(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        ValueIndex = input.ReadEncodedInt();
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(ValueIndex);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(ValueIndex);
    }
}