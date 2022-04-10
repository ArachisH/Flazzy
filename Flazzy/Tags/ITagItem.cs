using System.Buffers;

using Flazzy.IO;

namespace Flazzy.Tags;

public interface ITagItem
{
    TagKind Kind { get; }

    int GetBodySize() => 0;
    void WriteBodyTo(ref FlashWriter output) { }

    public void WriteTo(IBufferWriter<byte> output)
    {
        int bodySize = GetBodySize();

        TagHeader header = new(Kind, bodySize);
        int headerSize = header.GetSize();

        // Write tag header
        FlashWriter headerWriter = new(output.GetSpan(headerSize));
        header.WriteTo(ref headerWriter);
        output.Advance(headerSize);

        // Write tag body
        FlashWriter bodyWriter = new(output.GetSpan(bodySize));
        WriteBodyTo(ref bodyWriter);
        output.Advance(bodySize);
    }

    public static ITagItem Read(ref FlashReader input, ref TagHeader header)
    {
        ReadOnlySpan<byte> tagSpan = input.ReadBytes(header.Length);
        FlashReader tagInput = new(tagSpan);

        return header.Kind switch
        {
            TagKind.DefineBinaryData => new DefineBinaryDataTag(ref tagInput),
            TagKind.DefineBitsJPEG3 => new DefineBitsJPEG3(ref tagInput),
            TagKind.DefineBitsLossless => new DefineBitsLosslessTag(ref tagInput, 1),
            TagKind.DefineBitsLossless2 => new DefineBitsLosslessTag(ref tagInput, 2),
            TagKind.DefineFontName => new DefineFontNameTag(ref tagInput),
            TagKind.DefineSound => new DefineSoundTag(ref tagInput),
            TagKind.DoABC => new DoABCTag(ref tagInput),
            TagKind.ExportAssets => new ExportAssetsTag(ref tagInput),
            TagKind.FileAttributes => new FileAttributesTag(ref tagInput),
            TagKind.FrameLabel => new FrameLabelTag(ref tagInput),
            TagKind.Metadata => new MetadataTag(ref tagInput),
            TagKind.ProductInfo => new ProductInfoTag(ref tagInput),
            TagKind.ScriptLimits => new ScriptLimitsTag(ref tagInput),
            TagKind.SetBackgroundColor => new SetBackgroundColorTag(ref tagInput),
            TagKind.ShowFrame => new ShowFrameTag(),
            TagKind.SymbolClass => new SymbolClassTag(ref tagInput),
            TagKind.End => new EndTag(),

            _ => new UnknownTag(ref tagInput, header.Kind)
        };
    }
    public static bool TryRead(ref FlashReader input, out ITagItem tag)
    {
        if (TagHeader.TryRead(ref input, out var header) &&
            input.Length >= input.Position + header.Length)
        {
            tag = Read(ref input, ref header);
            return true;
        }
        tag = default;
        return false;
    }
}
