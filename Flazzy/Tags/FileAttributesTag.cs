using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class FileAttributesTag : TagItem
    {
        public bool UseDirectBlit { get; set; }
        public bool UseGPU { get; set; }
        public bool HasMetadata { get; set; }
        public bool ActionScript3 { get; set; }
        public bool NoCrossDomainCache { get; set; }
        public bool UseNetwork { get; set; }

        public FileAttributesTag()
            : base(TagKind.FileAttributes)
        { }
        public FileAttributesTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            input.Align();

            input.ReadUB(1); // Reserved | Must equal '0'.
            UseDirectBlit = (input.ReadUB(1) == 1);
            UseGPU = (input.ReadUB(1) == 1);
            HasMetadata = (input.ReadUB(1) == 1);
            ActionScript3 = (input.ReadUB(1) == 1);
            NoCrossDomainCache = (input.ReadUB(1) == 1);
            input.ReadUB(1); // Reserved | Must equal '0'.
            UseNetwork = (input.ReadUB(1) == 1);
            input.ReadUB(24); // Reserved | Must equal '0'.
        }

        public override int GetBodySize()
        {
            return sizeof(int); // 32 Bits
        }

        protected override void WriteBodyTo(FlashWriter output)
        {
            output.WriteBits(1, 0); // Reserved | Must equal '0'.
            output.WriteBits(1, (UseDirectBlit ? 1 : 0));
            output.WriteBits(1, (UseGPU ? 1 : 0));
            output.WriteBits(1, (HasMetadata ? 1 : 0));
            output.WriteBits(1, (ActionScript3 ? 1 : 0));
            output.WriteBits(1, (NoCrossDomainCache ? 1 : 0));
            output.WriteBits(1, 0); // Reserved | Must equal '0'.
            output.WriteBits(1, (UseNetwork ? 1 : 0));
            output.WriteBits(24, 0); // Reserved | Must equal '0'.

            output.Flush();
        }
    }
}