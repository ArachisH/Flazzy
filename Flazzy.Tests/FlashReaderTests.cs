using Flazzy.IO;

using Xunit;

namespace Flazzy.Tests
{
    public class FlashReaderTests
    {
        [Theory]
        [InlineData(new byte[] { 0x00 }, 0)]
        [InlineData(new byte[] { 0x02 }, 2)]
        [InlineData(new byte[] { 0x81, 0x1 }, 129)]
        [InlineData(new byte[] { 0x81, 0x81, 0x67}, 1687681)]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x0F }, 4026531840)]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0xFF }, 4026531840)]
        public void FlashReader_ReadEncodedUInt(byte[] input, uint expected)
        {
            var reader = new FlashReader(input);

            Assert.Equal(expected, reader.ReadUInt30());
        }
    }
}
