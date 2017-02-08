using System.Text;

using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class DefineFontNameTag : TagItem
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public string Copyright { get; set; }

        public DefineFontNameTag()
            : base(TagKind.DefineFontName)
        { }
        public DefineFontNameTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Id = input.ReadUInt16();
            Name = input.ReadNullString();
            Copyright = input.ReadNullString();
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += (Encoding.UTF8.GetByteCount(Name) + 1);
            size += (Encoding.UTF8.GetByteCount(Copyright) + 1);
            return size;
        }

        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write(Id);
            output.WriteNullString(Name);
            output.WriteNullString(Copyright);
        }
    }
}