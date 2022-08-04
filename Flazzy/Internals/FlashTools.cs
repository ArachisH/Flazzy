namespace Flazzy
{
    internal static class FlashTools
    {
        public static DateTime Epoch { get; }

        static FlashTools()
        {
            Epoch = new DateTime(1970, 1, 1);
        }

        public static long[] GetMaxPaddedBitsNeeded(out int maxBits, params long[] values)
        {
            maxBits = 0;
            var fixedValues = new long[values.Length];
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

                fixedValues[i] = value;
                int neededBits = GetNeededBits(value);
                if (neededBits > maxBits)
                {
                    maxBits = neededBits;
                }
            }
            return fixedValues;
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
    }
}