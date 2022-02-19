namespace Flazzy.IO
{
    public ref struct BitReader
    {
        private int _bitPosition;
        private byte _currentBit;

        public BitReader()
        {
            _bitPosition = 0;
            _currentBit = 0;
        }

        public int ReadUBits(ref FlashReader reader, int bitCount)
        {
            int result = 0;
            if (bitCount > 0)
            {
                if (_bitPosition == 0)
                {
                    _currentBit = reader.ReadByte();
                }

                for (int i = 0; i < bitCount; i++)
                {
                    int bit = (_currentBit >> (7 - _bitPosition)) & 1;
                    result += bit << (bitCount - 1 - i);

                    if (++_bitPosition == 8)
                    {
                        _bitPosition = 0;
                        if (i != (bitCount - 1))
                        {
                            _currentBit = reader.ReadByte();
                        }
                    }
                }
            }
            return result;
        }
        public int ReadSBits(ref FlashReader reader, int bitCount)
        {
            int result = ReadUBits(ref reader, bitCount);
            int shift = 32 - bitCount;

            return (result << shift) >> shift;
        }
    }
}
