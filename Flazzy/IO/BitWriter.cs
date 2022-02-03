namespace Flazzy.IO
{
    // TODO: Align
    public ref struct BitWriter
    {
        private int _bitPosition;
        private byte _currentBit;

        public BitWriter()
        {
            _bitPosition = 0;
            _currentBit = 0;
        }

        public void WriteBits(FlashWriter output, int maxBits, int value)
        {
            for (int i = 0; i < maxBits; i++)
            {
                int bit = (value >> (maxBits - 1 - i)) & 1;

                _currentBit += (byte)(bit * (1 << (7 - _bitPosition)));
                if (++_bitPosition == 8)
                {
                    output.Write(_currentBit);

                    _bitPosition = 0;
                    _currentBit = 0;
                }
            }
        }

        public static int CountUBits(uint value)
        {
            int count = 0;
            while (value > 0)
            {
                value >>= 1;
                count++;
            }
            return count;
        }
        public static int CountSBits(int value)
        {
            if (value == 0) return 0;
            if (value == -1) return 1;

            if (value < 0) value = ~value;
            return CountUBits((uint)value) + 1;
        }
    }
}
