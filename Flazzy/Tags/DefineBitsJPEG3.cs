using System.Drawing;

using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class DefineBitsJPEG3 : ImageTag
    {
        public ushort Id { get; set; }
        public byte[] Data { get; set; }
        public byte[] AlphaData { get; set; }

        public DefineBitsJPEG3()
            : base(TagKind.DefineBitsJPEG3)
        { }
        public DefineBitsJPEG3(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Id = input.ReadUInt16();

            int alphaDataOffset = input.ReadInt32();
            Data = input.ReadBytes(alphaDataOffset);

            Format = GetFormat(Data);
            if (Format == ImageFormat.JPEG)
            {
                int partialLength = (2 + 4 + alphaDataOffset);
                AlphaData = input.ReadBytes(Header.Length - partialLength);
            }
            else
            {
                // Minimum Compressed Empty Data Length
                AlphaData = input.ReadBytes(8);
            }
        }

        public override Color[,] GetARGBMap()
        {
            throw new NotSupportedException();
        }
        public override void SetARGBMap(Color[,] map)
        {
            throw new NotSupportedException();
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(uint);
            size += Data.Length;
            size += AlphaData.Length;
            return size;
        }
        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write(Id);
            output.Write((uint)Data.Length);
            output.Write(Data);
            output.Write(AlphaData);
        }
    }
}