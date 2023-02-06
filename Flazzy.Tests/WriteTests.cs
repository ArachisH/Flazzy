using Flazzy.IO;

using Xunit;

namespace Flazzy.Tests;

public class WriteTests
{
    [Theory]
    [InlineData(0, new byte[] { 0x00 })]
    [InlineData(2, new byte[] { 0x02 })]
    [InlineData(129, new byte[] { 0x81, 0x01 })]
    [InlineData(1687681, new byte[] { 0x81, 0x81, 0x67 })]
    [InlineData(4026531840, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x0F })]
    public void FlashWriter_WriteEncodedUInt(uint value, byte[] expected)
    {
        int size = FlashWriter.GetEncodedUIntSize(value);

        Span<byte> buffer = stackalloc byte[size];
        var writer = new FlashWriter(buffer);

        writer.WriteEncodedUInt(value);

        Assert.True(buffer.SequenceEqual(expected));
    }
}
