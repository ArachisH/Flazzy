using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class DebugFileIns : ASInstruction
{
    public int FileNameIndex { get; set; }
    public string FileName => ABC.Pool.Strings[FileNameIndex];

    public DebugFileIns(ABCFile abc)
        : base(OPCode.DebugFile, abc)
    { }
    public DebugFileIns(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        FileNameIndex = input.ReadEncodedInt();
    }
    public DebugFileIns(ABCFile abc, int fileNameIndex)
        : this(abc)
    {
        FileNameIndex = fileNameIndex;
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(FileNameIndex);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(FileNameIndex);
    }
}