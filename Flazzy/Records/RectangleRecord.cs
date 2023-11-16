using Flazzy.IO;

namespace Flazzy.Records;

public sealed class RectangleRecord : FlashItem
{
    public int X { get; set; }
    public int Y { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    public int TwipsWidth => Width * 20;
    public int TwipsHeight => Height * 20;

    public RectangleRecord()
    { }
    public RectangleRecord(FlashReader input)
    {
        int maxBitCount = input.ReadUB(5);

        X = input.ReadSB(maxBitCount);
        Width = (input.ReadSB(maxBitCount) / 20);

        Y = input.ReadSB(maxBitCount);
        Height = (input.ReadSB(maxBitCount) / 20);
    }

    public int GetByteSize()
    {
        using var rectMem = new MemoryStream();
        using var rectFlash = new FlashWriter(rectMem);
        WriteTo(rectFlash);

        rectFlash.Flush(); // Align the bits
        return (rectMem.ToArray().Length);
    }
    public override void WriteTo(FlashWriter output)
    {
        Span<long> paddedValues = stackalloc long[4];
        Span<long> values = stackalloc long[4] { X, TwipsWidth, Y, TwipsHeight };
        GetMaxPaddedBitsNeeded(out int maxBits, values, ref paddedValues);

        output.WriteBits(5, maxBits);
        for (int i = 0; i < paddedValues.Length; i++)
        {
            output.WriteBits(maxBits, paddedValues[i]);
        }
    }

    private static int GetNeededBits(long value)
    {
        int counter = 32;
        uint mask = 0x80000000;
        value = (value < 0 ? -value : value);
        while (((value & mask) == 0) && (counter > 0))
        {
            mask >>= 1;
            counter -= 1;
        }
        return (counter + 1);
    }
    private static void GetMaxPaddedBitsNeeded(out int maxBits, ReadOnlySpan<long> values, ref Span<long> paddedValues)
    {
        maxBits = 0;
        for (int i = 0; i < values.Length; i++)
        {
            long value = values[i];
            if (value > 0x3FFFFFFF)
            {
                value = 0x3FFFFFFF;
            }
            else if (value < -0x3FFFFFFF)
            {
                value = -0x3FFFFFFF;
            }

            paddedValues[i] = value;
            int neededBits = GetNeededBits(value);
            if (neededBits > maxBits)
            {
                maxBits = neededBits;
            }
        }
    }
}