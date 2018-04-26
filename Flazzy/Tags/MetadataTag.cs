using System.Text;

using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class MetadataTag : TagItem
    {
        public string Metadata { get; set; }

        public MetadataTag()
            : base(TagKind.Metadata)
        { }
        public MetadataTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Metadata = input.ReadNullString();
        }

        public override int GetBodySize() => (Encoding.UTF8.GetByteCount(Metadata) + 1);
        protected override void WriteBodyTo(FlashWriter output)
        {
            output.WriteNullString(Metadata);
        }
    }
}