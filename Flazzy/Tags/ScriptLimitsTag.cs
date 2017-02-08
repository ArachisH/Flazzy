using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class ScriptLimitsTag : TagItem
    {
        public ushort MaxRecursionDepth { get; set; }
        public ushort ScriptTimeoutSeconds { get; set; }

        public ScriptLimitsTag()
            : base(TagKind.ScriptLimits)
        { }
        public ScriptLimitsTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            MaxRecursionDepth = input.ReadUInt16();
            ScriptTimeoutSeconds = input.ReadUInt16();
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(ushort);
            return size;
        }

        protected override void WriteBodyTo(FlashWriter output)
        {
            output.Write(MaxRecursionDepth);
            output.Write(ScriptTimeoutSeconds);
        }
    }
}