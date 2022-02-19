using Flazzy.IO;
using Flazzy.Records;

using Xunit;

namespace Flazzy.Tests
{
    public class ReadTests
    {
        [Theory]
        [InlineData(new byte[] { 0x00 }, 0)]
        [InlineData(new byte[] { 0x02 }, 2)]
        [InlineData(new byte[] { 0x81, 0x1 }, 129)]
        [InlineData(new byte[] { 0x81, 0x81, 0x67 }, 1687681)]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x0F }, 4026531840)]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0xFF }, 4026531840)]
        public void FlashReader_ReadEncodedUInt(byte[] input, uint expected)
        {
            var reader = new FlashReader(input);

            Assert.Equal(expected, reader.ReadEncodedUInt());
        }

        [Fact]
        public void Rectangle_Read_Empty()
        {
            Span<byte> input = stackalloc byte[] { 0b00000000 };
            var reader = new FlashReader(input);

            var rectangle = new RectangeRecord(ref reader);

            Assert.Equal(0, rectangle.X);
            Assert.Equal(0, rectangle.Y);
            Assert.Equal(0, rectangle.Width);
            Assert.Equal(0, rectangle.Height);
        }

        [Fact]
        public void Rectangle_Read_Signed()
        {
            Span<byte> input = stackalloc byte[] { 0b00110101, 0b10001010, 0b01011000, 0b10100000 };
            var reader = new FlashReader(input);

            var rectangle = new RectangeRecord(ref reader);

            Assert.Equal(-20, rectangle.X);
            Assert.Equal(-20, rectangle.Y);
            Assert.Equal(1, rectangle.Width);
            Assert.Equal(1, rectangle.Height);
        }
    }
}
