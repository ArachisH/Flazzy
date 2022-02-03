using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class ProductInfoTag : ITagItem
    {
        public TagKind Kind => TagKind.ProductInfo;

        public FlashEdition Edition { get; set; }
        public FlashProduct Product { get; set; }

        public byte MajorVersion { get; set; }
        public byte MinorVersion { get; set; }

        public uint BuildLow { get; set; }
        public uint BuildHigh { get; set; }

        public DateTime CompilationDate { get; set; }

        public ProductInfoTag()
        {
            CompilationDate = DateTime.Now;
        }
        public ProductInfoTag(ref FlashReader input)
        {
            Product = (FlashProduct)input.ReadUInt32();
            Edition = (FlashEdition)input.ReadUInt32();

            MajorVersion = input.ReadByte();
            MinorVersion = input.ReadByte();

            BuildLow = input.ReadUInt32();
            BuildHigh = input.ReadUInt32();

            CompilationDate = DateTime.UnixEpoch.AddMilliseconds(input.ReadUInt64());
        }

        public int GetBodySize()
        {
            int size = 0;
            size += sizeof(uint);
            size += sizeof(uint);
            size += sizeof(byte);
            size += sizeof(byte);
            size += sizeof(uint);
            size += sizeof(uint);
            size += sizeof(ulong);
            return size;
        }

        public void WriteBodyTo(FlashWriter output)
        {
            output.Write((uint)Product);
            output.Write((uint)Edition);
            output.Write(MajorVersion);
            output.Write(MinorVersion);
            output.Write(BuildLow);
            output.Write(BuildHigh);

            TimeSpan sinceEpoch = CompilationDate - DateTime.UnixEpoch;
            output.Write((ulong)sinceEpoch.TotalMilliseconds);
        }
    }
}