using System.IO.Compression;

namespace Flazzy.IO.Compression
{
    public static class ZLib
    {
        public static unsafe int Decompress(ReadOnlySpan<byte> input, Span<byte> output)
        {
            fixed (byte* pBuffer = &input[0])
            {
                using var stream = new UnmanagedMemoryStream(pBuffer, input.Length);
                using var deflateStream = new ZLibStream(stream, CompressionMode.Decompress);

                return deflateStream.Read(output);
            }
        }
    }
}
