using Flazzy.IO;

namespace Flazzy.Tags
{
    public ref struct TagHeader
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

        public int Length { get; set; }
        public TagKind Kind { get; set; }

        public TagHeader(TagKind kind)
        {
            Length = 0;
            Kind = kind;
        }
        public TagHeader(ref FlashReader input)
        {
            ushort header = input.ReadUInt16();
            Kind = (TagKind)(header >> 6);

            Length = header & 63;
            if (Length > MAX_SHORT_LENGTH)
            {
                Length = input.ReadInt32();
            }
        }

        public int GetSize() => IsLongTag ? 6 : 2;
        public void WriteTo(FlashWriter output)
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
    }
}