using Flazzy.IO;
using Flazzy.Tags;

namespace Flazzy.Records
{
    public class HeaderRecord : FlashItem
    {
        private const int MAX_SHORT_LENGTH = 62;

        private bool _isLongTag;
        public bool IsLongTag
        {
            get
            {
                switch (Kind)
                {
                    case TagKind.DefineBits:

                    case TagKind.DefineBitsJPEG2:
                    case TagKind.DefineBitsJPEG3:
                    case TagKind.DefineBitsJPEG4:

                    case TagKind.DefineBitsLossless:
                    case TagKind.DefineBitsLossless2:

                    case TagKind.SoundStreamBlock:
                    {
                        /*
                         * These tags are required to write their length amount in the long format,
                         * despite if any of them do not meet the 63 length requirment.
                         */
                        return true;
                    }

                    default:
                    {
                        return (_isLongTag ||
                            (Length > MAX_SHORT_LENGTH));
                    }
                }
            }
        }

        public int Length { get; set; }
        public TagKind Kind { get; set; }

        public HeaderRecord(TagKind kind)
        {
            Kind = kind;
        }
        public HeaderRecord(FlashReader input)
        {
            ushort header = header = input.ReadUInt16();
            Kind = (TagKind)(header >> 6);

            Length = (header & 63);
            if (Length > MAX_SHORT_LENGTH)
            {
                Length = input.ReadInt32();
                _isLongTag = (Length <= MAX_SHORT_LENGTH);
            }
        }

        public override void WriteTo(FlashWriter output)
        {
            var header = ((uint)Kind << 6);
            header |= (IsLongTag ? 63 : (uint)Length);

            output.Write((ushort)header);
            if (IsLongTag)
            {
                output.Write(Length);
            }
        }
    }
}