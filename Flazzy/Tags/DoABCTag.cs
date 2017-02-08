using System.Text;

using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class DoABCTag : TagItem
    {
        public uint Flags { get; set; }
        public string Name { get; set; }
        public byte[] ABCData { get; set; }

        public DoABCTag()
            : base(TagKind.DoABC)
        {
            ABCData = new byte[0];
        }
        public DoABCTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Flags = input.ReadUInt32();
            Name = input.ReadNullString();

            int partialLength = (Encoding.UTF8.GetByteCount(Name) + 5);
            ABCData = input.ReadBytes(header.Length - partialLength);
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(uint);
            size += (Encoding.UTF8.GetByteCount(Name) + 1);
            size += ABCData.Length;
            return size;
        }

        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write(Flags);
            output.WriteNullString(Name);
            output.Write(ABCData);
        }
    }
}