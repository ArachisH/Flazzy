using Flazzy.IO;
using Flazzy.Tags;
using Flazzy.Records;
using Flazzy.IO.Compression;

namespace Flazzy
{
    public class ShockwaveFlash : IDisposable
    {
        private readonly FlashReader _input;

        public List<TagItem> Tags { get; }
        public CompressionKind Compression { get; }
        public string Signature => ((char)Compression + "WS");

        public byte Version { get; set; }
        public uint FileLength { get; set; }
        public FrameRecord Frame { get; set; }

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

            if (Compression == CompressionKind.LZMA)
            {
                throw new NotSupportedException("LZMA compression is not supported.");
            }

            _input = (Compression == CompressionKind.ZLIB) ?
                ZLib.WrapDecompressor(input.BaseStream) : input;
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
            if (_input.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(_input), "Input stream has already been disposed, or disassembly of the file has already occured.");
            }
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

                if (tag.Kind == TagKind.End) break;
            }
            _input.Dispose();
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

            if (compression == CompressionKind.LZMA)
            {
                throw new NotSupportedException("LZMA compression is not supported.");
            }
            int fileLength = 8;
            FlashWriter bodyWriter = compression == CompressionKind.ZLIB ?
                ZLib.WrapCompressor(output.BaseStream, true) : output;
            
            /* Body Start */
            Frame.WriteTo(bodyWriter);
            fileLength += (Frame.Area.GetByteSize() + 4);
            for (int i = 0; i < Tags.Count; i++)
            {
                TagItem tag = Tags[i];
                callback?.Invoke(tag);
                WriteTag(tag, bodyWriter);

                fileLength += tag.Header.Length;
                fileLength += (tag.Header.IsLongTag ? 6 : 2);
            }
            bodyWriter?.Dispose();
            /* Body End */

            output.Position = 4;
            output.Write((uint)fileLength);
            output.Position = output.Length;
        }

        public void CopyTo(Stream output)
        {
            CopyTo(output, Compression, null);
        }
        public void CopyTo(Stream output, Action<TagItem> callback)
        {
            CopyTo(output, Compression, callback);
        }

        public void CopyTo(Stream output, CompressionKind compression)
        {
            CopyTo(output, compression, null);
        }
        public void CopyTo(Stream output, CompressionKind compression, Action<TagItem> callback)
        {
            using var fOutput = new FlashWriter(output, true);
            Assemble(fOutput, compression, callback);
        }

        public byte[] ToArray()
        {
            return ToArray(Compression);
        }
        public byte[] ToArray(CompressionKind compression)
        {
            using var output = new MemoryStream((int)FileLength);
            CopyTo(output, compression, null);
            return output.ToArray();
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
                case TagKind.DefineBitsJPEG3: return new DefineBitsJPEG3(header, input);
                case TagKind.DefineBitsLossless2: return new DefineBitsLossless2Tag(header, input);
                case TagKind.DefineFontName: return new DefineFontNameTag(header, input);
                case TagKind.DefineSound: return new DefineSoundTag(header, input);
                case TagKind.DoABC: return new DoABCTag(header, input);
                case TagKind.End: return new EndTag(header);
                case TagKind.ExportAssets: return new ExportAssetsTag(header, input);
                case TagKind.FileAttributes: return new FileAttributesTag(header, input);
                case TagKind.FrameLabel: return new FrameLabelTag(header, input);
                case TagKind.Metadata: return new MetadataTag(header, input);
                case TagKind.ProductInfo: return new ProductInfoTag(header, input);
                case TagKind.ScriptLimits: return new ScriptLimitsTag(header, input);
                case TagKind.SetBackgroundColor: return new SetBackgroundColorTag(header, input);
                case TagKind.ShowFrame: return new ShowFrameTag(header);
                case TagKind.SymbolClass: return new SymbolClassTag(header, input);

                default:
                case TagKind.Unknown: return new UnknownTag(header, input);
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
                Tags.Clear();
                _input.Dispose();
            }
        }
    }
}