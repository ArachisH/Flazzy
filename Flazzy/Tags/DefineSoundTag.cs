using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class DefineSoundTag : TagItem
    {
        public int Rate { get; set; }
        public int Size { get; set; }
        public ushort Id { get; set; }
        public int Format { get; set; }
        public int SoundType { get; set; }
        public byte[] SoundData { get; set; }
        public uint SoundSampleCount { get; set; }

        public DefineSoundTag()
            : base(TagKind.DefineSound)
        {
            SoundData = new byte[0];
        }
        public DefineSoundTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Id = input.ReadUInt16();
            Format = input.ReadUB(4);
            Rate = input.ReadUB(2);
            Size = input.ReadUB(1);
            SoundType = input.ReadUB(1);
            SoundSampleCount = input.ReadUInt32();
            SoundData = input.ReadBytes(header.Length - 7);
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(byte);
            size += sizeof(uint);
            size += SoundData.Length;
            return size;
        }

        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write(Id);
            output.WriteBits(4, Format);
            output.WriteBits(2, Rate);
            output.WriteBits(1, Size);
            output.WriteBits(1, SoundType);
            output.Write(SoundSampleCount);
            output.Write(SoundData);
        }
    }
}