using System.Text;

using Flazzy.IO;

namespace Flazzy.Tags
{
    public class DefineFontNameTag : ITagItem
    {
        public TagKind Kind => TagKind.DefineFontName;

        public ushort Id { get; set; }
        public string Name { get; set; }
        public string Copyright { get; set; }

        public DefineFontNameTag()
        { }
        public DefineFontNameTag(ref FlashReader input)
        {
            Id = input.ReadUInt16();
            Name = input.ReadNullString();
            Copyright = input.ReadNullString();
        }

        public int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += Encoding.UTF8.GetByteCount(Name) + 1;
            size += Encoding.UTF8.GetByteCount(Copyright) + 1;
            return size;
        }

        public void WriteBodyTo(ref FlashWriter output)
        {
            output.Write(Id);
            output.WriteNullString(Name);
            output.WriteNullString(Copyright);
        }
    }
}