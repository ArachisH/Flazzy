using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class UnknownTag : TagItem
    {
        public byte[] Data { get; set; }

        public UnknownTag(TagKind kind)
            : base(kind)
        {
            Data = new byte[0];
        }
        public UnknownTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Data = input.ReadBytes(header.Length);
        }

        public override int GetBodySize()
        {
            return Data.Length;
        }

        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write(Data);
        }
    }
}