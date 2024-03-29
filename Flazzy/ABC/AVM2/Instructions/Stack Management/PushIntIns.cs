﻿using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class PushIntIns : Primitive
{
    private int _value;
    new public int Value
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
            _value = ABC.Pool.Integers[value];

            base.Value = _value;
        }
    }

    public PushIntIns(ABCFile abc)
        : base(OPCode.PushInt, abc)
    { }
    public PushIntIns(ABCFile abc, int value)
        : this(abc)
    {
        Value = value;
    }
    public PushIntIns(ABCFile abc, ref SpanFlashReader input)
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