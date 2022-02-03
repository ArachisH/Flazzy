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
            NameIndex = input.ReadInt30();
            Items.Capacity = input.ReadInt30();
            for (int i = 0; i < Items.Capacity; i++)
            {
                var itemInfo = new ASItemInfo(abc, ref input);
                Items.Add(itemInfo);
            }
        }

        public int GetSize()
        {
            throw new NotImplementedException();
        }
        public void WriteTo(FlashWriter output)
        {
            output.WriteEncodedInt(NameIndex);
            output.WriteEncodedInt(Items.Count);
            foreach (var item in Items)
            {
                item.WriteTo(output);
            }
        }
    }
}