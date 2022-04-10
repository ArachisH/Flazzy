using System.Drawing;

using Flazzy.IO;

namespace Flazzy.Tags;

public class SetBackgroundColorTag : ITagItem
{
    public TagKind Kind => TagKind.SetBackgroundColor;

    public Color BackgroundColor { get; set; }

    public SetBackgroundColorTag(ref FlashReader input)
    {
        byte r = input.ReadByte();
        byte g = input.ReadByte();
        byte b = input.ReadByte();
        BackgroundColor = Color.FromArgb(r, g, b);
    }

    public int GetBodySize()
    {
        int size = 0;
        size += sizeof(byte);
        size += sizeof(byte);
        size += sizeof(byte);
        return size;
    }

    public void WriteBodyTo(ref FlashWriter output)
    {
        output.Write(BackgroundColor.R);
        output.Write(BackgroundColor.G);
        output.Write(BackgroundColor.B);
    }
}
