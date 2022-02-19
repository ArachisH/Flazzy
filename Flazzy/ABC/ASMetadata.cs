using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASMetadata : IFlashItem
    {
        private readonly ABCFile _abc;

        public int NameIndex { get; set; }
        public string Name => _abc.Pool.Strings[NameIndex];

        public List<ASItemInfo> Items { get; }

        public ASMetadata(ABCFile abc)
        {
            _abc = abc;

            Items = new List<ASItemInfo>();
        }
        public ASMetadata(ABCFile abc, ref FlashReader input)
            : this(abc)
        {
            NameIndex = input.ReadEncodedInt();
            Items.Capacity = input.ReadEncodedInt();
            for (int i = 0; i < Items.Capacity; i++)
            {
                Items.Add(new ASItemInfo(abc, ref input));
            }
        }

        public int GetSize()
        {
            int size = 0;
            size += FlashWriter.GetEncodedIntSize(NameIndex);
            size += FlashWriter.GetEncodedIntSize(Items.Count);
            for (int i = 0; i < Items.Count; i++)
            {
                size += Items[i].GetSize();
            }
            return size;
        }
        public void WriteTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(NameIndex);
            output.WriteEncodedInt(Items.Count);
            foreach (var item in Items)
            {
                item.WriteTo(ref output);
            }
        }
    }
}