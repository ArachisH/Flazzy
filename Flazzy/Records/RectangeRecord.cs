using Flazzy.IO;

namespace Flazzy.Records
{
    public class RectangeRecord : FlashItem
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int TwipsWidth => (Width * 20);
        public int TwipsHeight => (Height * 20);

        public RectangeRecord()
        { }
        public RectangeRecord(FlashReader input)
        {
            int maxBitCount = input.ReadUB(5);

            X = input.ReadSB(maxBitCount);
            Width = (input.ReadSB(maxBitCount) / 20);

            Y = input.ReadSB(maxBitCount);
            Height = (input.ReadSB(maxBitCount) / 20);
        }

        public int GetByteSize()
        {
            using (var rectMem = new MemoryStream())
            using (var rectFlash = new FlashWriter(rectMem))
            {
                WriteTo(rectFlash);

                rectFlash.Flush(); // Align the bits
                return (rectMem.ToArray().Length);
            }
        }

        public override void WriteTo(FlashWriter output)
        {
            int maxBits = 0;
            long[] paddedValues = FlashTools.GetMaxPaddedBitsNeeded(
                out maxBits, X, TwipsWidth, Y, TwipsHeight);

            output.WriteBits(5, maxBits);
            for (int i = 0; i < paddedValues.Length; i++)
            {
                output.WriteBits(maxBits, paddedValues[i]);
            }
        }
    }
}