using System.Text;

using Flazzy.IO;

namespace Flazzy.Tags
{
    public class ExportAssetsTag : ITagItem
    {
        public TagKind Kind => TagKind.ExportAssets;

        public Dictionary<ushort, string> Exports { get; set; }

        public ExportAssetsTag()
        {
            Exports = new Dictionary<ushort, string>();
        }
        public ExportAssetsTag(ref FlashReader input)
        {
            ushort assetCount = input.ReadUInt16();

            Exports = new Dictionary<ushort, string>(assetCount);
            for (int i = 0; i < assetCount; i++)
            {
                Exports.Add(input.ReadUInt16(), input.ReadNullString());
            }
        }

        public int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(ushort) * Exports.Count;
            foreach (string name in Exports.Values)
            {
                size += Encoding.UTF8.GetByteCount(name) + 1;
            }
            return size;
        }
        public void WriteBodyTo(FlashWriter output)
        {
            output.Write((ushort)Exports.Count);
            foreach ((ushort id, string name) in Exports)
            {
                output.Write(id);
                output.WriteNullString(name);
            }
        }
    }
}