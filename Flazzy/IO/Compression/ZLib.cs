using System.IO.Compression;

namespace Flazzy.IO.Compression
{
    public static class ZLib
    {
        public static FlashReader WrapDecompressor(Stream stream)
        {
            stream.Seek(2, SeekOrigin.Current); // Skip Zlib header
            return new FlashReader(new DeflateStream(stream, CompressionMode.Decompress));
        }

        public static FlashWriter WrapCompressor(Stream stream, bool leaveOpen)
        {
            stream.Write(stackalloc byte[2] { 0x78, 0x9C }); // Default ZLib compression
            return new FlashWriter(new DeflateStream(stream, CompressionMode.Compress, leaveOpen));
        }
    }
}
