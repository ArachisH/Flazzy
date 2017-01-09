using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASMetadata : FlashItem
    {
        private readonly ABCFile _abc;

        public string Name
        {
            get { return _abc.Pool.Strings[NameIndex]; }
        }
        public int NameIndex { get; set; }

        public List<ASItemInfo> Items { get; }

        public ASMetadata(ABCFile abc)
        {
            _abc = abc;

            Items = new List<ASItemInfo>();
        }
        public ASMetadata(ABCFile abc, FlashReader input)
            : this(abc)
        {
            NameIndex = input.ReadInt30();
            Items.Capacity = input.ReadInt30();

            for (int i = 0; i < Items.Capacity; i++)
            {
                var itemInfo = new ASItemInfo(abc, input);
                Items.Add(itemInfo);
            }
        }

        public override void WriteTo(FlashWriter output)
        {
            output.WriteInt30(NameIndex);
            for (int i = 0; i < Items.Count; i++)
            {
                ASItemInfo item = Items[i];
                item.WriteTo(output);
            }
        }
    }
}