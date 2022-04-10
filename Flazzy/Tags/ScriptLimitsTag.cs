using Flazzy.IO;

namespace Flazzy.Tags;

public class ScriptLimitsTag : ITagItem
{
    public TagKind Kind => TagKind.ScriptLimits;

    public ushort MaxRecursionDepth { get; set; }
    public ushort ScriptTimeoutSeconds { get; set; }

    public ScriptLimitsTag(ref FlashReader input)
    {
        MaxRecursionDepth = input.ReadUInt16();
        ScriptTimeoutSeconds = input.ReadUInt16();
    }

    public int GetBodySize()
    {
        int size = 0;
        size += sizeof(ushort);
        size += sizeof(ushort);
        return size;
    }

    public void WriteBodyTo(ref FlashWriter output)
    {
        output.Write(MaxRecursionDepth);
        output.Write(ScriptTimeoutSeconds);
    }
}
