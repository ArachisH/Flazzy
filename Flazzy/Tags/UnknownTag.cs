using Flazzy.IO;

namespace Flazzy.Tags
{
    public class UnknownTag : ITagItem
    {
        public TagKind Kind { get; }

        public byte[] Data { get; set; }

        public UnknownTag(TagKind kind)
        {
            Kind = kind;
            Data = Array.Empty<byte>();
        }
        public UnknownTag(ref FlashReader input, TagKind kind)
            : this(kind)
        {
            Data = new byte[input.Length];
            input.ReadBytes(Data);
        }

        public int GetBodySize()
        {
            return Data.Length;
        }

        public void WriteBodyTo(FlashWriter output)
        {
            output.Write(Data);
        }
    }
}