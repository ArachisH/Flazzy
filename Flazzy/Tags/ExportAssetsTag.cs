using System.Text;

using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class ExportAssetsTag : TagItem
    {
        public List<ushort> Ids { get; }
        public List<string> Names { get; }

        public ExportAssetsTag()
            : base(TagKind.ExportAssets)
        {
            Ids = new List<ushort>();
            Names = new List<string>();
        }
        public ExportAssetsTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            ushort assetCount = input.ReadUInt16();

            Ids = new List<ushort>(assetCount);
            Names = new List<string>(assetCount);
            for (int i = 0; i < assetCount; i++)
            {
                Ids.Add(input.ReadUInt16());
                Names.Add(input.ReadNullString());
            }
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += (sizeof(ushort) * Ids.Count);
            foreach (string name in Names)
            {
                size += (Encoding.UTF8.GetByteCount(name) + 1);
            }
            return size;
        }

        protected override void WriteBodyTo(FlashWriter output)
        {
            int assetCount = Math.Min(Ids.Count, Names.Count);
            output.Write((ushort)assetCount);

            for (int i = 0; i < assetCount; i++)
            {
                output.Write(Ids[i]);
                output.WriteNullString(Names[i]);
            }
        }
    }
}