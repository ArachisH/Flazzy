using System.IO;

using Flazzy.IO;
using Flazzy.Compression.ComponentAce;

namespace Flazzy.Compression
{
    public static class ZLIB
    {
        /* The ZLIB compressor was not written by me, nor do I want the credit.
         * My goodness, have you seen the code: Flazzy.Compression.ComponentAce
         * Works fine though, so that's nice. */

        public static byte[] Compress(byte[] data)
        {
            using (var output = new MemoryStream())
            using (var compressor = new ZOutputStream(output, 9))
            {
                compressor.Write(data, 0, data.Length);
                return output.ToArray();
            }
        }
        public static byte[] Decompress(byte[] data)
        {
            using (var output = new MemoryStream())
            using (var input = new MemoryStream(data))
            using (var decompressor = new ZInputStream(input))
            {
                decompressor.CopyTo(output);
                return output.ToArray();
            }
        }

        public static FlashReader WrapDecompressor(Stream input)
        {
            return new FlashReader(new ZInputStream(input));
        }
        public static FlashWriter WrapCompressor(Stream output, bool leaveOpen = false)
        {
            var compressionStream = new ZOutputStream(output, 9, leaveOpen);
            return new FlashWriter(compressionStream);
        }
    }
}