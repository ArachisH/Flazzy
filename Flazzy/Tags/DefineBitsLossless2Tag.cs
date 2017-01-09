using System;
using System.Drawing;
using System.IO.Compression;
using System.Drawing.Imaging;

using Flazzy.IO;
using Flazzy.Records;
using Flazzy.Compression;

namespace Flazzy.Tags
{
    public class DefineBitsLossless2Tag : TagItem
    {
        private byte _format;
        private ushort _width;
        private ushort _height;
        private byte[] _pixelData;
        private byte _colorTableSize;
        private byte[] _compressedBitmapData;

        public ushort Id { get; set; }

        public DefineBitsLossless2Tag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            Id = input.ReadUInt16();
            _format = input.ReadByte();

            _width = input.ReadUInt16();
            _height = input.ReadUInt16();

            if (_format == 3)
            {
                _colorTableSize = input.ReadByte();
            }

            int partialLength = (7 + (_format == 3 ? 1 : 0));
            _compressedBitmapData = input.ReadBytes(header.Length - partialLength);
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
            size += _compressedBitmapData.Length;
            return size;
        }
        public Bitmap GenerateAsset()
        {
            if (_format == 3)
            {
                throw new NotSupportedException(
                    "8-bit asset generator not supported.");
            }

            Compressor(CompressionMode.Decompress);
            var asset = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
            for (int y = 0, i = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; i += 4, x++)
                {
                    byte a = _pixelData[i];
                    byte r = _pixelData[i + 1];
                    byte g = _pixelData[i + 2];
                    byte b = _pixelData[i + 3];
                    asset.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }

            _pixelData = null;
            return asset;
        }
        public void SetPixelData(Bitmap asset)
        {
            if (asset.PixelFormat !=
                PixelFormat.Format32bppArgb)
            {
                throw new NotSupportedException("Not valid 32-bit ARGB bitmap.");
            }

            _width = (ushort)asset.Width;
            _height = (ushort)asset.Height;

            _pixelData = new byte[(_width * _height) * 4];
            for (int y = 0, i = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; i += 4, x++)
                {
                    Color pixel = asset.GetPixel(x, y);
                    _pixelData[i] = pixel.A;
                    _pixelData[i + 1] = pixel.R;
                    _pixelData[i + 2] = pixel.G;
                    _pixelData[i + 3] = pixel.B;
                }
            }

            _compressedBitmapData = null;
            Compressor(CompressionMode.Compress);
            _pixelData = null;
        }

        protected void Compressor(CompressionMode mode)
        {
            switch (mode)
            {
                case CompressionMode.Compress:
                {
                    if (_compressedBitmapData == null)
                    {
                        _compressedBitmapData =
                            Zlib.Compress(_pixelData);
                    }
                    break;
                }
                case CompressionMode.Decompress:
                {
                    if (_pixelData == null)
                    {
                        _pixelData =
                            Zlib.Decompress(_compressedBitmapData);
                    }
                    break;
                }
            }
        }
        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write(Id);
            output.Write(_format);
            output.Write(_width);
            output.Write(_height);

            if (_format == 3)
            {
                output.Write(_colorTableSize);
            }

            output.Write(_compressedBitmapData);
        }
    }
}