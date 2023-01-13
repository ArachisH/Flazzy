using System.Buffers;

using Flazzy.IO;
using Flazzy.Tags;
using Flazzy.Records;
using Flazzy.IO.Compression;

namespace Flazzy;

public class ShockwaveFlash : IDisposable
{
    public List<ITagItem> Tags { get; }

    public CompressionKind Compression { get; set; }
    public byte Version { get; set; }
    public FrameRecord Frame { get; set; }

    protected ShockwaveFlash()
    {
        Tags = new List<ITagItem>();
        Frame = new FrameRecord();
    }

    public void Assemble(IBufferWriter<byte> output)
    {
        Assemble(output, Compression);
    }
    public void Assemble(IBufferWriter<byte> output, CompressionKind compression)
    {
        if (Compression == CompressionKind.LZMA)
            throw new NotSupportedException($"{nameof(CompressionKind.LZMA)} is not supported!");

        // Write SWF body
        var bodyWriter = new ArrayBufferWriter<byte>();
        ((IFlashItem)Frame).WriteTo(bodyWriter);

        foreach (var tag in Tags)
        {
            tag.WriteTo(bodyWriter);
        }

        // TODO: Compare calculating entire body size manually when CompressionKind.None to the cost of ArrayBufferWriter allocs. I expect manual calc to be worth it.
        int bodyLength = bodyWriter.WrittenCount;

        // Write SWF header
        Span<byte> headerSpan = stackalloc byte[8];
        var headerWriter = new FlashWriter(headerSpan);

        headerWriter.Write((byte)compression);
        headerWriter.Write((short)0x5357); // "WS"
        headerWriter.Write(Version);
        headerWriter.Write(bodyLength + 8); // uncompressed body length + SWF header size (8)
        output.Write(headerSpan);

        if (compression == CompressionKind.ZLib)
        {
            byte[] compressedBuffer = ZLib.Compress(bodyWriter.WrittenSpan);
            output.Write(compressedBuffer);
        }
        else output.Write(bodyWriter.WrittenSpan);
    }

    public byte[] ToArray()
    {
        return ToArray(Compression);
    }
    public byte[] ToArray(CompressionKind compression)
    {
        var output = new ArrayBufferWriter<byte>();
        Assemble(output, compression);
        return output.WrittenSpan.ToArray();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Tags.Clear();
        }
    }

    public static ShockwaveFlash Read(string path)
    {
        return Read(File.ReadAllBytes(path));
    }
    public static ShockwaveFlash Read(ReadOnlySpan<byte> data)
    {
        var input = new FlashReader(data);

        var shockwaveFlash = new ShockwaveFlash
        {
            Compression = (CompressionKind)input.ReadBytes(3)[0],
            Version = input.ReadByte()
        };

        uint length = input.ReadUInt32() - 8; // TODO: is it was always -8
        Span<byte> bodySpan = new byte[length];

        if (shockwaveFlash.Compression == CompressionKind.None)
        {
            input.ReadBytes(bodySpan);
        }
        else if (shockwaveFlash.Compression == CompressionKind.ZLib)
        {
            ReadOnlySpan<byte> compressed = input.ReadBytes(input.Length - input.Position);
            int totalDecompressed = ZLib.Decompress(compressed, bodySpan);

            bodySpan = bodySpan.Slice(0, totalDecompressed);
        }
        else throw new NotSupportedException($"{nameof(CompressionKind.LZMA)} is not supported!");

        var bodyInput = new FlashReader(bodySpan);
        shockwaveFlash.Frame = new FrameRecord(ref bodyInput);

        while (bodyInput.IsDataAvailable)
        {
            if (!ITagItem.TryRead(ref bodyInput, out var tag))
                break;

            shockwaveFlash.Tags.Add(tag);

            if (tag.Kind == TagKind.End)
                break;
        }

        return shockwaveFlash;
    }
}
