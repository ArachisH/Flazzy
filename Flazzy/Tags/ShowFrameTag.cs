using Flazzy.Records;

namespace Flazzy.Tags
{
    public class ShowFrameTag : TagItem
    {
        public ShowFrameTag()
            : base(TagKind.ShowFrame)
        { }
        public ShowFrameTag(HeaderRecord header)
            : base(header)
        { }

        public override int GetBodySize()
        {
            return 0;
        }
    }
}