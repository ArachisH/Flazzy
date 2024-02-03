using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class PushShortIns : Primitive
{
    private int _value;
    new public int Value
    {
        get => _value;
        set
        {
            _value = value;
            base.Value = value;
        }
    }

    public PushShortIns()
        : base(OPCode.PushShort)
    { }
    public PushShortIns(int value)
        : this()
    {
        Value = value;
    }
    public PushShortIns(ref SpanFlashReader input)
        : this()
    {
        Value = input.ReadEncodedInt();
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(Value);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(Value);
    }
}