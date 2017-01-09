using System;
using System.IO;
using System.Collections.Generic;

using Flazzy.IO;
using Flazzy.Tags;
using Flazzy.Records;

using Ionic.Zlib;

namespace Flazzy
{
    public class ShockwaveFlash : IDisposable
    {
        private bool _disassembled;
        private FlashReader _input;
        private readonly CompressionKind _initialCompression;

        public byte Version { get; set; }
        public uint FileLength { get; set; }
        public FrameRecord Frame { get; set; }
        public CompressionKind Compression { get; set; }

        public List<TagItem> Tags { get; }
        public bool IsCompressed { get; private set; }

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
        {
            _input = input;

            Tags = new List<TagItem>();

            string signature = input.ReadString(3);
            Compression = (CompressionKind)signature[0];
            _initialCompression = Compression;

            Version = input.ReadByte();
            FileLength = input.ReadUInt32();

            IsCompressed = (Compression != CompressionKind.None);
            if (!IsCompressed)
            {
                Frame = new FrameRecord(input);
            }
        }

        public void Disassemble()
        {
            if (_disassembled)
            {
                throw new InvalidOperationException(
                    "Tags have already been read from the file.");
            }

            FlashReader input = _input;
            try
            {
                if (IsCompressed)
                {
                    if (_initialCompression == CompressionKind.LZMA)
                    {
                        throw new NotSupportedException("LZMA");
                    }
                    else if (_initialCompression == CompressionKind.ZLIB)
                    {
                        var decompressor = new ZlibStream(input.BaseStream, CompressionMode.Decompress);
                        input = new FlashReader(decompressor);
                    }
                    Frame = new FrameRecord(input);
                    IsCompressed = false;
                }
                Disassemble(input);
            }
            finally
            {
                _disassembled = true;
                input.Dispose();
            }
        }
        protected virtual void Disassemble(FlashReader input)
        {
            long relativePos = (8 + Frame.Area.GetByteSize() + 4);
            while (true)
            {
                var header = new HeaderRecord(input);
                relativePos += (header.IsLongTag ? 6 : 2);
                long expectedPos = (header.Length + relativePos);

                TagItem tag = ReadTag(header, input);
                relativePos += tag.GetBodySize();

                if (relativePos != expectedPos)
                {
                    throw new InvalidDataException("Unable to disassemble tag.");
                }
                else Tags.Add(tag);

                if (tag.Kind == TagKind.End)
                {
                    break;
                }
            }
        }

        public void Assemble(FlashWriter output)
        {
            Assemble(output, Compression);
        }
        public virtual void Assemble(FlashWriter output, CompressionKind compression)
        {
            FlashWriter bodyOutput = output;
            try
            {
                output.Write(((char)compression + "WS"), true);
                output.Write(Version);
                output.Write(uint.MinValue);

                int fileLength = 8;
                if (compression != CompressionKind.None)
                {
                    switch (compression)
                    {
                        case CompressionKind.LZMA:
                        {
                            throw new NotSupportedException("LZMA");
                        }
                        case CompressionKind.ZLIB:
                        {
                            bodyOutput = new FlashWriter(new ZlibStream(
                                output.BaseStream, CompressionMode.Compress, CompressionLevel.BestCompression, true));

                            break;
                        }
                    }
                }

                Frame.WriteTo(bodyOutput);
                fileLength += (Frame.Area.GetByteSize() + 4);
                for (int i = 0; i < Tags.Count; i++)
                {
                    TagItem tag = Tags[i];
                    WriteTag(tag, bodyOutput);

                    fileLength += tag.Header.Length;
                    fileLength += (tag.Header.IsLongTag ? 6 : 2);
                }

                if (bodyOutput != output)
                {
                    // Finalize compression.
                    bodyOutput.Dispose();
                }

                output.Position = 4;
                output.Write((uint)fileLength);
            }
            finally
            {
                if (bodyOutput != output)
                {
                    // Ensure compression stream has been disposed.
                    bodyOutput.Dispose();
                }
            }
        }

        protected virtual void WriteTag(TagItem tag, FlashWriter output)
        {
            tag.WriteTo(output);
        }
        protected virtual TagItem ReadTag(HeaderRecord header, FlashReader input)
        {
            TagItem tag = null;
            switch (header.Kind)
            {
                case TagKind.DefineBinaryData:
                tag = new DefineBinaryDataTag(header, input);
                break;

                case TagKind.DefineBitsLossless2:
                tag = new DefineBitsLossless2Tag(header, input);
                break;

                case TagKind.DefineFontName:
                tag = new DefineFontNameTag(header, input);
                break;

                case TagKind.DefineSound:
                tag = new DefineSoundTag(header, input);
                break;

                case TagKind.DoABC:
                tag = new DoABCTag(header, input);
                break;

                case TagKind.End:
                tag = new EndTag(header);
                break;

                case TagKind.ExportAssets:
                tag = new ExportAssetsTag(header, input);
                break;

                case TagKind.FileAttributes:
                tag = new FileAttributesTag(header, input);
                break;

                case TagKind.FrameLabel:
                tag = new FrameLabelTag(header, input);
                break;

                case TagKind.ProductInfo:
                tag = new ProductInfoTag(header, input);
                break;

                case TagKind.ScriptLimits:
                tag = new ScriptLimitsTag(header, input);
                break;

                case TagKind.SetBackgroundColor:
                tag = new SetBackgroundColorTag(header, input);
                break;

                case TagKind.ShowFrame:
                tag = new ShowFrameTag(header);
                break;

                case TagKind.SymbolClass:
                tag = new SymbolClassTag(header, input);
                break;

                default:
                case TagKind.Unknown:
                tag = new UnknownTag(header, input);
                break;
            }
            return tag;
        }

        public byte[] ToArray()
        {
            return ToArray(Compression);
        }
        public byte[] ToArray(CompressionKind compression)
        {
            using (var memOutput = new MemoryStream((int)FileLength))
            {
                using (var output = new FlashWriter(memOutput, true))
                {
                    Assemble(output, compression);
                }
                return memOutput.ToArray();
            }
        }

        public string GetSignature()
        {
            return ((char)Compression + "WS");
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