using System;
using System.IO;
using System.Collections.Generic;

using Flazzy.IO;
using Flazzy.Tags;
using Flazzy.Records;
using Flazzy.Compression;

namespace Flazzy
{
    public class ShockwaveFlash : IDisposable
    {
        private readonly FlashReader _input;

        public List<TagItem> Tags { get; }
        public string Signature => ((char)Compression + "WS");

        public byte Version { get; set; }
        public uint FileLength { get; set; }
        public FrameRecord Frame { get; set; }
        public CompressionKind Compression { get; set; }

        public ShockwaveFlash()
            : this(true)
        { }
        public ShockwaveFlash(string path)
            : this(File.OpenRead(path))
        { }
        public ShockwaveFlash(byte[] data)
            : this(new MemoryStream(data))
        { }
        public ShockwaveFlash(Stream input)
            : this(input, false)
        { }
        public ShockwaveFlash(Stream input, bool leaveOpen)
            : this(new FlashReader(input, leaveOpen))
        { }

        protected ShockwaveFlash(FlashReader input)
            : this(false)
        {
            Compression = (CompressionKind)input.ReadString(3)[0];
            Version = input.ReadByte();
            FileLength = input.ReadUInt32();

            switch (Compression)
            {
                case CompressionKind.LZMA:
                {
                    byte[] decompressed = LZMA.Decompress(input.BaseStream, ((int)FileLength - 8));
                    _input = new FlashReader(decompressed);
                    break;
                }

                case CompressionKind.ZLIB:
                _input = ZLIB.WrapDecompressor(input.BaseStream);
                break;

                case CompressionKind.None:
                _input = input;
                break;
            }
            Frame = new FrameRecord(_input);
        }
        protected ShockwaveFlash(bool isCreatingTemplate)
        {
            Tags = new List<TagItem>();
            if (isCreatingTemplate)
            {
                Frame = new FrameRecord();
                Frame.Area = new RectangeRecord();
                Compression = CompressionKind.ZLIB;
            }
        }

        public void Disassemble()
        {
            Disassemble(null);
        }
        public virtual void Disassemble(Action<TagItem> callback)
        {
            long position = (8 + Frame.Area.GetByteSize() + 4);
            while (position != FileLength)
            {
                var header = new HeaderRecord(_input);
                position += (header.IsLongTag ? 6 : 2);
                long offset = (header.Length + position);

                TagItem tag = ReadTag(header, _input);
                position += tag.GetBodySize();

                if (position != offset)
                {
                    throw new IOException($"Expected position value '{offset}', instead got '{position}'.");
                }
                callback?.Invoke(tag);
                Tags.Add(tag);

                if (tag.Kind == TagKind.End)
                {
                    FileLength = (uint)position;
                    break;
                }
            }
        }

        public void Assemble(FlashWriter output)
        {
            Assemble(output, Compression, null);
        }
        public void Assemble(FlashWriter output, Action<TagItem> callback)
        {
            Assemble(output, Compression, callback);
        }

        public void Assemble(FlashWriter output, CompressionKind compression)
        {
            Assemble(output, compression, null);
        }
        public virtual void Assemble(FlashWriter output, CompressionKind compression, Action<TagItem> callback)
        {
            output.Write(((char)compression) + "WS", true);
            output.Write(Version);
            output.Write(uint.MinValue);

            int fileLength = 8;
            FlashWriter compressor = null;
            switch (compression)
            {
                case CompressionKind.LZMA:
                {
                    compressor = new FlashWriter((int)FileLength);
                    break;
                }
                case CompressionKind.ZLIB:
                {
                    compressor = ZLIB.WrapCompressor(output.BaseStream, true);
                    break;
                }
            }

            /* Body Start */
            Frame.WriteTo(compressor ?? output);
            fileLength += (Frame.Area.GetByteSize() + 4);
            for (int i = 0; i < Tags.Count; i++)
            {
                TagItem tag = Tags[i];
                callback?.Invoke(tag);
                WriteTag(tag, compressor ?? output);

                fileLength += tag.Header.Length;
                fileLength += (tag.Header.IsLongTag ? 6 : 2);
            }
            if (compression == CompressionKind.LZMA)
            {
                byte[] uncompressedBody = ((MemoryStream)compressor.BaseStream).ToArray();
                byte[] compressedBody = LZMA.Compress(uncompressedBody);
                output.Write(compressedBody);
            }
            compressor?.Dispose();
            /* Body End */

            output.Position = 4;
            output.Write((uint)fileLength);
            output.Position = output.Length;
        }

        protected virtual void WriteTag(TagItem tag, FlashWriter output)
        {
            tag.WriteTo(output);
        }
        protected virtual TagItem ReadTag(HeaderRecord header, FlashReader input)
        {
            switch (header.Kind)
            {
                case TagKind.DefineBinaryData: return new DefineBinaryDataTag(header, input);
                case TagKind.DefineBitsLossless2: return new DefineBitsLossless2Tag(header, input);
                case TagKind.DefineFontName: return new DefineFontNameTag(header, input);
                case TagKind.DefineSound: return new DefineSoundTag(header, input);
                case TagKind.DoABC: return new DoABCTag(header, input);
                case TagKind.End: return new EndTag(header);
                case TagKind.ExportAssets: return new ExportAssetsTag(header, input);
                case TagKind.FileAttributes: return new FileAttributesTag(header, input);
                case TagKind.FrameLabel: return new FrameLabelTag(header, input);
                case TagKind.ProductInfo: return new ProductInfoTag(header, input);
                case TagKind.ScriptLimits: return new ScriptLimitsTag(header, input);
                case TagKind.SetBackgroundColor: return new SetBackgroundColorTag(header, input);
                case TagKind.ShowFrame: return new ShowFrameTag(header);
                case TagKind.SymbolClass: return new SymbolClassTag(header, input);

                default:
                case TagKind.Unknown: return new UnknownTag(header, input);
            }
        }

        public byte[] ToArray()
        {
            return ToArray(Compression);
        }
        public byte[] ToArray(CompressionKind compression)
        {
            using (var outputMem = new MemoryStream((int)FileLength))
            using (var output = new FlashWriter(outputMem))
            {
                Assemble(output, compression);
                return outputMem.ToArray();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _input.Dispose();
            }
        }
    }
}