using System.Text;

using Flazzy.IO;

namespace Flazzy.Tags;

public class FrameLabelTag : ITagItem
{
    public TagKind Kind => TagKind.FrameLabel;

    public string Name { get; set; }

    public FrameLabelTag()
    { }
    public FrameLabelTag(ref FlashReader input)
    {
        Name = input.ReadNullString();
    }

    public int GetBodySize() => Encoding.UTF8.GetByteCount(Name) + 1;
    public void WriteBodyTo(ref FlashWriter output)
    {
        output.WriteNullString(Name);
    }
}
