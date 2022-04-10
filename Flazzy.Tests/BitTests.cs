using Flazzy.IO;

using Xunit;

namespace Flazzy.Tests;

public class BitTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(0b_00111101_00000000, 14)]
    public void BitWriter_CountUnsignedBits(uint value, int expectedCount)
    {
        Assert.Equal(expectedCount, BitWriter.CountUBits(value));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(0b_00111101_00000000, 15)]
    [InlineData(-1, 1)]
    [InlineData(-2, 2)]
    [InlineData(-0b_00111101_00000000, 15)]
    public void BitWriter_CountSignedBits(int value, int expectedCount)
    {
        Assert.Equal(expectedCount, BitWriter.CountSBits(value));
    }
}
