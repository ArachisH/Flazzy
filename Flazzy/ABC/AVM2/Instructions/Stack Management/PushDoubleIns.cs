﻿using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class PushDoubleIns : Primitive
{
    private double _value;
    new public double Value
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
            _value = ABC.Pool.Doubles[value];

            base.Value = _value;
        }
    }

    public PushDoubleIns(ABCFile abc)
        : base(OPCode.PushDouble, abc)
    { }
    public PushDoubleIns(ABCFile abc, double value)
        : this(abc)
    {
        Value = value;
    }
    public PushDoubleIns(ABCFile abc, ref SpanFlashReader input)
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