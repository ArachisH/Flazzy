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
            int size = 0;
            size += 0;
            return size;
        }
        public void WriteTo(ref FlashWriter output)
        {
            int maxBits = FlashTools.GetMaxPaddedBitsNeeded(stackalloc int[] { X, TwipsWidth, Y, TwipsHeight });

            var bits = new BitWriter();
            bits.WriteBits(ref output, 5, maxBits);
            bits.WriteBits(ref output, maxBits, X);
            bits.WriteBits(ref output, maxBits, TwipsWidth);
            bits.WriteBits(ref output, maxBits, Y);
            bits.WriteBits(ref output, maxBits, TwipsHeight);
            bits.Flush(ref output);
        }
    }
}