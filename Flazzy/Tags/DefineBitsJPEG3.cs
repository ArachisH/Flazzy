using Flazzy.IO;

namespace Flazzy.Tags
{
    public class DefineBitsJPEG3 : ITagItem
    {
        public TagKind Kind => TagKind.DefineBitsJPEG3;

        public ushort Id { get; set; }
        public byte[] Data { get; set; }
        public byte[] AlphaData { get; set; }
        public ImageFormat Format { get; set; }

        public DefineBitsJPEG3()
        { }
        public DefineBitsJPEG3(ref FlashReader input)
        {
            Id = input.ReadUInt16();

            int alphaDataOffset = input.ReadInt32();

            Data = new byte[alphaDataOffset];
            input.ReadBytes(Data);

            Format = FlashTools.GetImageFormat(Data);
            if (Format == ImageFormat.JPEG)
            {
                int partialLength = (2 + 4 + alphaDataOffset);
                AlphaData = new byte[input.Length - partialLength];
            }
            else
            {
                // Minimum Compressed Empty Data Length
                AlphaData = new byte[8];
            }
            input.ReadBytes(AlphaData);
        }

        public int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(uint);
            size += Data.Length;
            size += AlphaData.Length;
            return size;
        }
        public void WriteBodyTo(FlashWriter output)
        {
            output.Write(Id);
            output.Write((uint)Data.Length);
            output.Write(Data);
            output.Write(AlphaData);
        }
    }
}