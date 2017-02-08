using System.Drawing;

using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class SetBackgroundColorTag : TagItem
    {
        public Color BackgroundColor { get; set; }

        public SetBackgroundColorTag()
            : base(TagKind.SetBackgroundColor)
        { }
        public SetBackgroundColorTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            byte r = input.ReadByte();
            byte g = input.ReadByte();
            byte b = input.ReadByte();
            BackgroundColor = Color.FromArgb(r, g, b);
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(byte);
            size += sizeof(byte);
            size += sizeof(byte);
            return size;
        }

        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write(BackgroundColor.R);
            output.Write(BackgroundColor.G);
            output.Write(BackgroundColor.B);
        }
    }
}