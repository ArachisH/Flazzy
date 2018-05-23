using System;
using System.Drawing;
using System.IO.Compression;

using Flazzy.IO;
using Flazzy.Records;
using Flazzy.Compression;

namespace Flazzy.Tags
{
    public class DefineBitsLossless2Tag : TagItem
    {
        private byte _format;
        private Color[,] _argbMap;
        private readonly byte _colorTableSize;

        private byte[] _compressedData;
        private byte[] _uncompressedData;

        public ushort Id { get; set; }

        public DefineBitsLossless2Tag()
            : base(TagKind.DefineBitsLossless2)
        {
            _argbMap = new Color[0, 0];
            _compressedData = new byte[0];
        }
        public DefineBitsLossless2Tag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Id = input.ReadUInt16();
            _format = input.ReadByte();

            ushort width = input.ReadUInt16();
            ushort height = input.ReadUInt16();
            _argbMap = new Color[width, height];

            if (_format == 3)
            {
                _colorTableSize = input.ReadByte();
            }

            int partialLength = (7 + (_format == 3 ? 1 : 0));
            _compressedData = input.ReadBytes(header.Length - partialLength);
        }

        public Color[,] GetARGBMap()
        {
            if (_format == 3)
            {
                throw new NotSupportedException(
                    "8-bit asset generator not supported.");
            }

            Compressor(CompressionMode.Decompress);
            for (int y = 0, i = 0; y < _argbMap.GetLength(1); y++)
            {
                for (int x = 0; x < _argbMap.GetLength(0); i += 4, x++)
                {
                    byte a = _uncompressedData[i];
                    byte r = _uncompressedData[i + 1];
                    byte g = _uncompressedData[i + 2];
                    byte b = _uncompressedData[i + 3];
                    _argbMap[x, y] = Color.FromArgb(a, r, g, b);
                }
            }

            _uncompressedData = null;
            return _argbMap;
        }
        public void SetARGBMap(Color[,] map)
        {
            _argbMap = map;
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            _uncompressedData = new byte[width * height * 4];
            for (int y = 0, i = 0; y < height; y++)
            {
                for (int x = 0; x < width; i += 4, x++)
                {
                    Color pixel = map[x, y];
                    _uncompressedData[i + 0] = pixel.A;
                    _uncompressedData[i + 1] = pixel.R;
                    _uncompressedData[i + 2] = pixel.G;
                    _uncompressedData[i + 3] = pixel.B;
                }
            }
            _compressedData = null;
            Compressor(CompressionMode.Compress);
            _uncompressedData = null;
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(byte);
            size += sizeof(ushort);
            size += sizeof(ushort);
            if (_format == 3)
            {
                size += sizeof(byte);
            }
            size += _compressedData.Length;
            return size;
        }
        protected void Compressor(CompressionMode mode)
        {
            switch (mode)
            {
                case CompressionMode.Compress:
                {
                    _compressedData =
                        ZLIB.Compress(_uncompressedData);
                    break;
                }
                case CompressionMode.Decompress:
                {
                    _uncompressedData =
                        ZLIB.Decompress(_compressedData);
                    break;
                }
            }
        }
        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write(Id);
            output.Write(_format);
            output.Write((ushort)_argbMap.GetLength(0));
            output.Write((ushort)_argbMap.GetLength(1));
            if (_format == 3)
            {
                output.Write(_colorTableSize);
            }
            output.Write(_compressedData);
        }
    }
}