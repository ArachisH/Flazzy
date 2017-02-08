using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class DefineBinaryDataTag : TagItem
    {
        public ushort Id { get; set; }
        public byte[] Data { get; set; }

        public DefineBinaryDataTag()
            : base(TagKind.DefineBinaryData)
        {
            Data = new byte[0];
        }
        public DefineBinaryDataTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Id = input.ReadUInt16();
            input.ReadUInt32(); // Reserved | Must equal '0'.
            Data = input.ReadBytes(header.Length - 6);
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(uint);
            size += Data.Length;
            return size;
        }

        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write(Id);
            output.Write(0); // Reserved | Must equal '0'.
            output.Write(Data);
        }
    }
}