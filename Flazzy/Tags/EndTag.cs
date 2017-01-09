using Flazzy.Records;

namespace Flazzy.Tags
{
    public class EndTag : TagItem
    {
        public EndTag()
            : base(TagKind.End)
        { }
        public EndTag(HeaderRecord header)
            : base(header)
        { }

        public override int GetBodySize()
        {
            return 0;
        }
    }
}