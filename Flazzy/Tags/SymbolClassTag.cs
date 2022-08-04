using System.Text;

using Flazzy.IO;
using Flazzy.Records;

namespace Flazzy.Tags
{
    public class SymbolClassTag : TagItem
    {
        public List<ushort> Ids { get; }
        public List<string> Names { get; }

        public SymbolClassTag()
            : base(TagKind.SymbolClass)
        {
            Ids = new List<ushort>();
            Names = new List<string>();
        }
        public SymbolClassTag(HeaderRecord header, FlashReader input)
            : base(header)
        {
            ushort symbolCount = input.ReadUInt16();

            Ids = new List<ushort>(symbolCount);
            Names = new List<string>(symbolCount);

            for (int i = 0; i < symbolCount; i++)
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
            int symbolCount = Math.Min(Ids.Count, Names.Count);
            output.Write((ushort)symbolCount);

            for (int i = 0; i < symbolCount; i++)
            {
                output.Write(Ids[i]);
                output.WriteNullString(Names[i]);
            }
        }
    }
}