using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class ProductInfoTag : TagItem
    {
        public FlashEdition Edition { get; set; }
        public FlashProduct Product { get; set; }

        public byte MajorVersion { get; set; }
        public byte MinorVersion { get; set; }

        public uint BuildLow { get; set; }
        public uint BuildHigh { get; set; }

        public DateTime CompilationDate { get; set; }

        public ProductInfoTag()
            :base(TagKind.ProductInfo)
        {
            CompilationDate = DateTime.Now;
        }
        public ProductInfoTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Product = (FlashProduct)input.ReadUInt32();
            Edition = (FlashEdition)input.ReadUInt32();

            MajorVersion = input.ReadByte();
            MinorVersion = input.ReadByte();

            BuildLow = input.ReadUInt32();
            BuildHigh = input.ReadUInt32();

            CompilationDate = FlashTools.Epoch
                .AddMilliseconds(input.ReadUInt64());
        }

        public override int GetBodySize()
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

        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write((uint)Product);
            output.Write((uint)Edition);
            output.Write(MajorVersion);
            output.Write(MinorVersion);
            output.Write(BuildLow);
            output.Write(BuildHigh);

            TimeSpan sinceEpoch = (CompilationDate - FlashTools.Epoch);
            output.Write((ulong)sinceEpoch.TotalMilliseconds);
        }
    }
}