using System.Text;

using Flazzy.IO;

namespace Flazzy.Tags
{
    public class SymbolClassTag : ITagItem
    {
        public TagKind Kind => TagKind.SymbolClass;

        public Dictionary<ushort, string> Symbols { get; set; }

        public SymbolClassTag()
        {
            Symbols = new Dictionary<ushort, string>();
        }
        public SymbolClassTag(ref FlashReader input)
        {
            ushort symbolCount = input.ReadUInt16();

            Symbols = new Dictionary<ushort, string>(symbolCount);
            for (int i = 0; i < symbolCount; i++)
            {
                Symbols[input.ReadUInt16()] = input.ReadNullString();
            }
        }

        public int GetBodySize()
        {
            int size = 0;
            size += sizeof(ushort);
            size += sizeof(ushort) * Symbols.Count;
            foreach (string name in Symbols.Values)
            {
                size += Encoding.UTF8.GetByteCount(name) + 1;
            }
            return size;
        }

        public void WriteBodyTo(ref FlashWriter output)
        {
            output.Write((ushort)Symbols.Count);

            foreach ((ushort id, string name) in Symbols)
            {
                output.Write(id);
                output.WriteNullString(name);
            }
        }
    }
}