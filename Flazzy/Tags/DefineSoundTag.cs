using Flazzy.IO;

namespace Flazzy.Tags;

public class DefineSoundTag : ITagItem
{
    public TagKind Kind => TagKind.DefineSound;

    public int SampleRate { get; set; }
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
        SampleRate = ((flags & 0b1100) >> 2) switch
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

        SoundData = new byte[input.Length - input.Position];
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

    public void WriteBodyTo(ref FlashWriter output)
    {
        output.Write(Id);

        var bits = new BitWriter();
        bits.WriteBits(ref output, 4, Format);
        bits.WriteBits(ref output, 2, (SampleRate switch
        {
            5512 => 0,
            11025 => 1,
            22050 => 2,
            44100 => 3,

            _ => throw new InvalidDataException("Invalid sample rate value.")
        }));
        bits.WriteBits(ref output, 1, Size);
        bits.WriteBits(ref output, 1, SoundType);
        bits.Flush(ref output);

        output.Write(SoundSampleCount);
        output.Write(SoundData);
    }
}
