using System.Text;

using Flazzy.IO;

namespace Flazzy.Tags
{
    public class MetadataTag : ITagItem
    {
        public TagKind Kind => TagKind.Metadata;

        public string Metadata { get; set; }

        public MetadataTag()
        { }
        public MetadataTag(ref FlashReader input)
        {
            Metadata = input.ReadNullString();
        }

        public int GetBodySize() => (Encoding.UTF8.GetByteCount(Metadata) + 1);
        public void WriteBodyTo(FlashWriter output)
        {
            output.WriteNullString(Metadata);
        }
    }
}