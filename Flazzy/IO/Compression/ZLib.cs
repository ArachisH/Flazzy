using System.IO.Compression;

namespace Flazzy.IO.Compression
{
    public static class ZLib
    {
        public static FlashWriter WrapCompressor(Stream stream, bool leaveOpen)
        {
            return new FlashWriter(new ZLibStream(stream, CompressionMode.Compress, leaveOpen));
        }
        public static FlashReader WrapDecompressor(Stream stream)
        {
            return new FlashReader(new ZLibStream(stream, CompressionMode.Decompress));
        }
    }
}
