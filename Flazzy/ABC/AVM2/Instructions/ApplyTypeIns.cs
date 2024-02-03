using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class ApplyTypeIns : ASInstruction
{
    public int ParamCount { get; set; }

    public ApplyTypeIns()
        : base(OPCode.ApplyType)
    { }
    public ApplyTypeIns(int paramCount)
        : this()
    {
        ParamCount = paramCount;
    }
    public ApplyTypeIns(ref SpanFlashReader input)
        : this()
    {
        ParamCount = input.ReadEncodedInt();
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(ParamCount);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(ParamCount);
    }
}