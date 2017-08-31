using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public abstract class TagItem : FlashItem
    {
        public TagKind Kind => Header.Kind;
        public HeaderRecord Header { get; }

        protected override string DebuggerDisplay => Kind.ToString();

        public TagItem(TagKind kind)
            : this(new HeaderRecord(kind))
        { }
        public TagItem(HeaderRecord header)
        {
            Header = header;
        }

        public abstract int GetBodySize();

        public override void WriteTo(FlashWriter output)
        {
            Header.Length = GetBodySize();
            Header.WriteTo(output);
            WriteBodyTo(output);
        }
        protected virtual void WriteBodyTo(FlashWriter output)
        { }
    }
}