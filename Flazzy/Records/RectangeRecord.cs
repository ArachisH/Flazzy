using Flazzy.IO;

namespace Flazzy.Records
{
    public class RectangeRecord : IFlashItem
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int TwipsWidth => Width * 20;
        public int TwipsHeight => Height * 20;

        public RectangeRecord()
        { }
        public RectangeRecord(ref FlashReader input)
        {
            var bits = new BitReader();
            int maxBitCount = bits.ReadUBits(ref input, 5);

            X = bits.ReadSBits(ref input, maxBitCount);
            Width = bits.ReadSBits(ref input, maxBitCount) / 20;

            Y = bits.ReadSBits(ref input, maxBitCount);
            Height = bits.ReadSBits(ref input, maxBitCount) / 20;
        }

        public int GetSize()
        {
            throw new NotImplementedException();
        }
        public void WriteTo(FlashWriter output)
        {
            int[] paddedValues = FlashTools.GetMaxPaddedBitsNeeded(
                out int maxBits, X, TwipsWidth, Y, TwipsHeight);

            var bits = new BitWriter();
            bits.WriteBits(output, 5, maxBits);
            for (int i = 0; i < paddedValues.Length; i++)
            {
                bits.WriteBits(output, maxBits, paddedValues[i]);
            }
            //TODO: Align
        }
    }
}