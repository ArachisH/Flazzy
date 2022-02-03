using Flazzy.IO;

namespace Flazzy.Tags
{
    public class DefineSoundTag : ITagItem
    {
        public TagKind Kind => TagKind.DefineSound;

        public int Rate { get; set; }
        public int Size { get; set; }
        public ushort Id { get; set; }
        public int Format { get; set; }
        public int SoundType { get; set; }
        public byte[] SoundData { get; set; }
        public uint SoundSampleCount { get; set; }

        public DefineSoundTag()
        {
            SoundData = Array.Empty<byte>();
        }
        public DefineSoundTag(ref FlashReader input)
        {
            Id = input.ReadUInt16();

            byte flags = input.ReadByte();
            Format = flags >> 4;
            Rate = (flags & 0b1100) >> 2 switch
            {
                0 => 5512,
                1 => 11025,
                2 => 22050,
                3 => 44100,

                _ => throw new InvalidDataException("Invalid sample rate value.")
            };
            Size = flags & 2;
            SoundType = flags & 1;

            SoundSampleCount = input.ReadUInt32();

            SoundData = new byte[input.Length - 7];
            input.ReadBytes(SoundData);
        }

        public int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(byte);
            size += sizeof(uint);
            size += SoundData.Length;
            return size;
        }

        public void WriteBodyTo(FlashWriter output)
        {
            output.Write(Id);

            var bits = new BitWriter();
            bits.WriteBits(output, 4, Format);
            bits.WriteBits(output, 2, Rate switch
            {
                5512 => 0,
                11025 => 1,
                22050 => 2,
                44100 => 3,

                _ => throw new InvalidDataException("Invalid sample rate value.")
            });
            bits.WriteBits(output, 1, Size);
            bits.WriteBits(output, 1, SoundType);
            //TODO: Flush?
            
            output.Write(SoundSampleCount);
            output.Write(SoundData);
        }
    }
}