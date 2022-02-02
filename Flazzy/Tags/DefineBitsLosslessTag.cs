using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class DefineBitsLosslessTag : TagItem
    {
        public ushort Id { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte ColorTableSize { get; }
        public BitmapFormat Format { get; set; }
        public byte[] CompressedData { get; set; }

        public int Version => Kind == TagKind.DefineBitsLossless ? 1 : 2;

        public DefineBitsLosslessTag(byte version)
            : base(version == 1 ? TagKind.DefineBitsLossless : TagKind.DefineBitsLossless2)
        {
            CompressedData = Array.Empty<byte>();
        }
        public DefineBitsLosslessTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Id = input.ReadUInt16();
            Format = input.ReadByte() switch
            {
                3 => BitmapFormat.ColorMap8,
                4 when Version == 1 => BitmapFormat.Rgb15,
                5 => BitmapFormat.Rgb32,

                _ => throw new InvalidDataException("Invalid bitmap format.")
            };

            Width = input.ReadUInt16();
            Height = input.ReadUInt16();

            if (Format == BitmapFormat.ColorMap8)
                ColorTableSize = input.ReadByte();

            CompressedData = input.ReadBytes(header.Length - GetHeaderSize());
        }

        private int GetHeaderSize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(byte);
            size += sizeof(ushort);
            size += sizeof(ushort);
            if (Format == BitmapFormat.ColorMap8)
            {
                size += sizeof(byte);
            }
            return size;
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += GetHeaderSize();
            size += CompressedData.Length;
            return size;
        }
        protected override void WriteBodyTo(FlashWriter output)
        {
            byte format = Format switch
            {
                BitmapFormat.ColorMap8 => 3,
                BitmapFormat.Rgb15 when Version == 1 => 4,
                BitmapFormat.Rgb32 => 5,

                BitmapFormat.Rgb15 when Version == 2 => throw new Exception($"{BitmapFormat.Rgb15} is only supported on {nameof(DefineBitsLosslessTag)} version 1."),
                _ => throw new InvalidDataException("Invalid bitmap format.")
            };

            output.Write(Id);
            output.Write(format);
            output.Write(Width);
            output.Write(Height);
            if (Format == BitmapFormat.ColorMap8)
            {
                output.Write(ColorTableSize);
            }
            output.Write(CompressedData);
        }
    }
}