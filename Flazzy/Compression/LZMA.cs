using System;
using System.IO;

using SevenZip;
using SevenZip.Compression.LZMA;

namespace Flazzy.Compression
{
    public static class LZMA
    {
        private static readonly CoderPropID[] _defaultIds;
        private static readonly object[] _defaultProperties;

        static LZMA()
        {
            _defaultIds = new CoderPropID[]
            {
                CoderPropID.DictionarySize,
                CoderPropID.PosStateBits,
                CoderPropID.LitContextBits,
                CoderPropID.LitPosBits,
                CoderPropID.Algorithm,
                CoderPropID.NumFastBytes,
                CoderPropID.MatchFinder,
                CoderPropID.EndMarker
            };
            _defaultProperties = new object[]
            {
                (1 << 23),
                2,
                3,
                0,
                2,
                32,
                "bt4",
                false
            };
        }

        public static byte[] Compress(byte[] data)
        {
            var encoder = new LZMAEncoder();
            encoder.SetCoderProperties(_defaultIds, _defaultProperties);
            using (var input = new MemoryStream(data))
            using (var output = new MemoryStream())
            {
                output.Position += 4;
                encoder.WriteCoderProperties(output);
                encoder.Code(input, output, -1, -1, null);

                byte[] compressed = output.ToArray();
                byte[] compressedLengthData = BitConverter.GetBytes(compressed.Length);

                Buffer.BlockCopy(compressedLengthData, 0, compressed, 0, 4);
                return compressed;
            }
        }
        public static byte[] Decompress(byte[] data, int outputLength)
        {
            using (var input = new MemoryStream(data))
            {
                return Decompress(input, outputLength);
            }
        }
        public static byte[] Decompress(Stream input, int outputLength)
        {
            var decoder = new LZMADecoder();
            using (var output = new MemoryStream(outputLength))
            {
                var ignoredData = new byte[4];
                input.Read(ignoredData, 0, ignoredData.Length);

                var lzmaProperties = new byte[5];
                input.Read(lzmaProperties, 0, 5);

                decoder.SetDecoderProperties(lzmaProperties);
                decoder.Code(input, output, input.Length, outputLength, null);
                return output.ToArray();
            }
        }
    }
}