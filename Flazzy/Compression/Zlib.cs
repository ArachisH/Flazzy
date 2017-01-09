using System.IO;

using Ionic.Zlib;

namespace Flazzy.Compression
{
    public static class Zlib
    {
        public static byte[] Compress(byte[] data)
        {
            using (var output = new MemoryStream())
            using (var compressor = new ZlibStream(output,
                CompressionMode.Compress, CompressionLevel.BestCompression))
            {
                ZlibBaseStream.CompressBuffer(data, compressor);
                return output.ToArray();
            }
        }
        public static byte[] Decompress(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var decompressor = new ZlibStream(
                input, CompressionMode.Decompress))
            {
                return ZlibBaseStream.UncompressBuffer(decompressor);
            }
        }
    }
}