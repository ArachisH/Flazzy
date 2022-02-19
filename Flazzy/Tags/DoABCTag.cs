using System.Text;

using Flazzy.IO;

namespace Flazzy.Tags
{
    public class DoABCTag : ITagItem
    {
        public TagKind Kind => TagKind.DoABC;

        public uint Flags { get; set; }
        public string Name { get; set; }
        public byte[] ABCData { get; set; }

        public DoABCTag()
        {
            ABCData = Array.Empty<byte>();
        }
        public DoABCTag(ref FlashReader input)
        {
            Flags = input.ReadUInt32();
            Name = input.ReadNullString();

            ABCData = new byte[input.Length - input.Position];
            input.ReadBytes(ABCData);
        }

        public int GetBodySize()
        {
            int size = 0;
            size += sizeof(uint);
            size += Encoding.UTF8.GetByteCount(Name) + 1;
            size += ABCData.Length;
            return size;
        }

        public void WriteBodyTo(ref FlashWriter output)
        {
            output.Write(Flags);
            output.WriteNullString(Name);
            output.Write(ABCData);
        }
    }
}