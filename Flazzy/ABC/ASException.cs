using Flazzy.IO;

namespace Flazzy.ABC;

public class ASException : IFlashItem
{
    public ABCFile ABC { get; }

    public int To { get; set; }
    public int From { get; set; }
    public int Target { get; set; }

    public int VariableNameIndex { get; set; }
    public ASMultiname VariableName => ABC.Pool.Multinames[VariableNameIndex];

    public int ExceptionTypeIndex { get; set; }
    public ASMultiname ExceptionType => ABC.Pool.Multinames[ExceptionTypeIndex];

    public ASException(ABCFile abc)
    {
        ABC = abc;
    }
    public ASException(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        From = input.ReadEncodedInt();
        To = input.ReadEncodedInt();
        Target = input.ReadEncodedInt();
        ExceptionTypeIndex = input.ReadEncodedInt();
        VariableNameIndex = input.ReadEncodedInt();
    }

    public int GetSize()
    {
        int size = 0;
        size += SpanFlashWriter.GetEncodedIntSize(From);
        size += SpanFlashWriter.GetEncodedIntSize(To);
        size += SpanFlashWriter.GetEncodedIntSize(Target);
        size += SpanFlashWriter.GetEncodedIntSize(ExceptionTypeIndex);
        size += SpanFlashWriter.GetEncodedIntSize(VariableNameIndex);
        return size;
    }
    public void WriteTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(From);
        output.WriteEncodedInt(To);
        output.WriteEncodedInt(Target);
        output.WriteEncodedInt(ExceptionTypeIndex);
        output.WriteEncodedInt(VariableNameIndex);
    }
}