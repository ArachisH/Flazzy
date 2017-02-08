using System.Text;

using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class FrameLabelTag : TagItem
    {
        public string Name { get; set; }

        public FrameLabelTag()
            : base(TagKind.FrameLabel)
        { }
        public FrameLabelTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Name = input.ReadNullString();
        }

        public override int GetBodySize()
        {
            return (Encoding.UTF8.GetByteCount(Name) + 1);
        }

        protected override void WriteBodyTo(FlashWriter output)
        {
            output.WriteNullString(Name);
        }
    }
}