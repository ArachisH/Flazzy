using Flazzy.IO;

namespace Flazzy.ABC;

public sealed class ASMetadata : FlashItem
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
    public ASMetadata(ABCFile abc, FlashReader input)
        : this(abc)
    {
        NameIndex = input.ReadInt30();
        Items.Capacity = input.ReadInt30();
        if (Items.Capacity > 0)
        {
            Span<int> keys = stackalloc int[Items.Capacity];
            for (int i = 0; i < Items.Capacity; i++)
            {
                keys[i] = input.ReadInt30();
            }
            for (int i = 0; i < keys.Length; i++)
            {
                Items.Add(new ASItemInfo(abc, keys[i], input.ReadInt30()));
            }
        }
    }

    public override void WriteTo(FlashWriter output)
    {
        output.WriteInt30(NameIndex);
        output.WriteInt30(Items.Count);
        for (int i = 0; i < Items.Count; i++)
        {
            output.WriteInt30(Items[i].KeyIndex);
        }
        for (int i = 0; i < Items.Count; i++)
        {
            output.WriteInt30(Items[i].ValueIndex);
        }
    }
}