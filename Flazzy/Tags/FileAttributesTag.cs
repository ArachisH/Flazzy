using Flazzy.IO;

namespace Flazzy.Tags
{
    public class FileAttributesTag : ITagItem
    {
        public TagKind Kind => TagKind.FileAttributes;
        public FileAttributes Attributes { get; set; }

        public FileAttributesTag()
        { }
        public FileAttributesTag(ref FlashReader input)
        {
            Attributes = (FileAttributes)input.ReadInt32();
        }

        public int GetBodySize() => sizeof(int);
        public void WriteBodyTo(FlashWriter output) => output.Write((int)Attributes);
    }
}