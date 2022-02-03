using Flazzy.IO;

namespace Flazzy.Tags
{
    public class DefineBinaryDataTag : ITagItem
    {
        public TagKind Kind => TagKind.DefineBinaryData;

        public ushort Id { get; set; }
        public byte[] Data { get; set; }

        public DefineBinaryDataTag()
        {
            Data = Array.Empty<byte>();
        }
        public DefineBinaryDataTag(ref FlashReader input)
        {
            Id = input.ReadUInt16();
            input.ReadUInt32(); // Reserved | Must equal '0'.

            Data = new byte[input.Length - 6];
            input.ReadBytes(Data);
        }

        public int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(uint);
            size += Data.Length;
            return size;
        }

        public void WriteBodyTo(FlashWriter output)
        {
            output.Write(Id);
            output.Write(0); // Reserved | Must equal '0'.
            output.Write(Data);
        }
    }
}