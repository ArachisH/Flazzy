using Flazzy.IO;
using System.Runtime.CompilerServices;

namespace Flazzy.Tags;

public readonly ref struct TagHeader
{
    private const int MAX_SHORT_LENGTH = 62;

    public bool IsLongTag => Kind switch
    {
        /*
         * These tags are required to write their length amount in the long format,
         * despite if any of them do not meet the 63 length requirment.
         */
        TagKind.DefineBits or
        TagKind.DefineBitsJPEG2 or
        TagKind.DefineBitsJPEG3 or
        TagKind.DefineBitsJPEG4 or
        TagKind.DefineBitsLossless or
        TagKind.DefineBitsLossless2 or
        TagKind.SoundStreamBlock => true,

        _ => Length > MAX_SHORT_LENGTH
    };

    public TagKind Kind { get; }
    public int Length { get; }
    public TagHeader(TagKind kind, int length)
    {
        Kind = kind;
        Length = length;
    }

    public int GetSize() => IsLongTag ? 6 : 2;
    public void WriteTo(ref FlashWriter output)
    {
        uint header = (uint)Kind << 6;

        if (IsLongTag)
        {
            header |= 63;
            output.Write((ushort)header);
            output.Write(Length);
        }
        else
        {
            header |= (uint)Length;
            output.Write((ushort)header);
        }
    }

    public static bool TryRead(ref FlashReader input, out TagHeader header)
    {
        header = default;
        if (input.Length >= 2)
        {
            ushort value = input.ReadUInt16();
            TagKind kind = (TagKind)(value >> 6);

            int length = value & 63;
            if (length > MAX_SHORT_LENGTH)
            {
                if (input.Length >= 6)
                {
                    length = input.ReadInt32();
                }
                else return false;
            }

            header = new TagHeader(kind, length);
            return true;
        }
        return false;
    }
}
