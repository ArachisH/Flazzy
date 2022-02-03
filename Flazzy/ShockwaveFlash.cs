using Flazzy.IO;
using Flazzy.Tags;
using Flazzy.Records;
using Flazzy.IO.Compression;

namespace Flazzy
{
    public class ShockwaveFlash : IDisposable
    {
        public List<ITagItem> Tags { get; }
        public CompressionKind Compression { get; }
        public string Signature => (char)Compression + "WS";

        public byte Version { get; set; }
        public uint FileLength { get; set; }
        public FrameRecord Frame { get; set; }

        protected ShockwaveFlash(ref FlashReader input)
            : this(false)
        {
            Compression = (CompressionKind)input.ReadString(3)[0];
            Version = input.ReadByte();
            FileLength = input.ReadUInt32();

            if (Compression == CompressionKind.LZMA)
            {
                throw new NotSupportedException("LZMA compression is not supported.");
            }

            //_input = (Compression == CompressionKind.ZLib) ?
            //    ZLib.WrapDecompressor(input.BaseStream) : input;
            Frame = new FrameRecord(ref input);
        }
        protected ShockwaveFlash(bool isCreatingTemplate)
        {
            Tags = new List<ITagItem>();
            if (isCreatingTemplate)
            {
                Frame = new FrameRecord();
                Frame.Area = new RectangeRecord();
                Compression = CompressionKind.ZLib;
            }
        }

        public virtual void Disassemble(ref FlashReader input, Action<ITagItem> callback = null)
        {
            long position = 8 + Frame.Area.GetSize() + 4;
            while (position != FileLength)
            {
                var header = new TagHeader(ref input);
                position += header.GetSize();

                long expectedPosition = header.Length + position;

                ITagItem tag = ITagItem.ReadTag(ref input, in header);
                position += tag.GetBodySize();

                if (position != expectedPosition)
                {
                    throw new IOException($"Expected position value '{expectedPosition}', instead got '{position}'.");
                }
                callback?.Invoke(tag);
                Tags.Add(tag);

                if (tag.Kind == TagKind.End) break;
            }
        }

        //public void Assemble(FlashWriter output)
        //{
        //    Assemble(output, Compression, null);
        //}
        //public void Assemble(FlashWriter output, Action<ITagItem> callback)
        //{
        //    Assemble(output, Compression, callback);
        //}
        //
        //public void Assemble(FlashWriter output, CompressionKind compression)
        //{
        //    Assemble(output, compression, null);
        //}
        //public virtual void Assemble(FlashWriter output, CompressionKind compression, Action<ITagItem> callback)
        //{
        //    output.Write(((char)compression) + "WS", true);
        //    output.Write(Version);
        //    output.Write(uint.MinValue);
        //
        //    Frame.WriteTo(output);
        //    int fileLength = Frame.Area.GetSize() + 4;
        //    foreach (var tag in Tags)
        //    {
        //        callback?.Invoke(tag);
        //        tag.WriteTo(output);
        //
        //        //fileLength += tag.Header.Length;
        //        //fileLength += (tag.Header.IsLongTag ? 6 : 2);
        //    }
        //
        //    //output.Position = 4;
        //    //output.Write((uint)fileLength);
        //    //output.Position = output.Length;
        //}

        //public void CopyTo(Stream output)
        //{
        //    CopyTo(output, Compression, null);
        //}
        //public void CopyTo(Stream output, Action<TagItem> callback)
        //{
        //    CopyTo(output, Compression, callback);
        //}
        //
        //public void CopyTo(Stream output, CompressionKind compression)
        //{
        //    CopyTo(output, compression, null);
        //}
        //public void CopyTo(Stream output, CompressionKind compression, Action<TagItem> callback)
        //{
        //    var fOutput = new FlashWriter(output);
        //    Assemble(fOutput, compression, callback);
        //}

        //public byte[] ToArray()
        //{
        //    return ToArray(Compression);
        //}
        //public byte[] ToArray(CompressionKind compression)
        //{
        //    using var output = new MemoryStream((int)FileLength);
        //    CopyTo(output, compression, null);
        //    return output.ToArray();
        //}
        
        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tags.Clear();
            }
        }
    }
}