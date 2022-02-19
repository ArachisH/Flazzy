using Flazzy.IO;

namespace Flazzy.Tags
{
    // TODO: OperationsStatus TryRead(...) - allows efficient parsing from non-contiguous memory using modern buffer APIs such as S.IO.Pipelines :)
    // Once we have TryRead, we can safely force read code-path to eliminate most of the bounds checks :))
    public interface ITagItem
    {
        TagKind Kind { get; }

        public static ITagItem ReadTag(ref FlashReader input)
        {
            var header = new TagHeader(ref input);
            return ReadTag(ref input, in header);
        }
        public static ITagItem ReadTag(ref FlashReader input, in TagHeader header)
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

                _ => new UnknownTag(ref input, header.Kind)
            };
        }

        int GetBodySize() => 0;
        void WriteBodyTo(ref FlashWriter output) { }
    }
}