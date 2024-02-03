using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class DebugIns : ASInstruction
{
    public int NameIndex { get; set; }
    public string Name => ABC.Pool.Strings[NameIndex];

    public int Extra { get; set; }
    public byte DebugType { get; set; }
    public byte RegisterIndex { get; set; }

    public DebugIns(ABCFile abc)
        : base(OPCode.Debug, abc)
    { }
    public DebugIns(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        DebugType = input.ReadByte();
        NameIndex = input.ReadEncodedInt();
        RegisterIndex = input.ReadByte();
        Extra = input.ReadEncodedInt();
    }
    public DebugIns(ABCFile abc, int nameIndex, byte debugType, byte registerIndex)
        : this(abc)
    {
        NameIndex = nameIndex;
        DebugType = debugType;
        RegisterIndex = registerIndex;
    }

    protected override int GetBodySize()
    {
        int size = 0;
        size += sizeof(byte);
        size += SpanFlashWriter.GetEncodedIntSize(NameIndex);
        size += sizeof(byte);
        size += SpanFlashWriter.GetEncodedIntSize(Extra);
        return size;
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.Write(DebugType);
        output.WriteEncodedInt(NameIndex);
        output.Write(RegisterIndex);
        output.WriteEncodedInt(Extra);
    }
}