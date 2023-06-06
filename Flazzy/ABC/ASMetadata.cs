using Flazzy.IO;

namespace Flazzy.ABC;

public sealed class ASMetadata : IFlashItem
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
        NameIndex = input.ReadEncodedInt();
        Items.Capacity = input.ReadEncodedInt();
        if (Items.Capacity > 0)
        {
            // TODO: Avoid potential stackoverflow.
            Span<int> keys = stackalloc int[Items.Capacity];
            for (int i = 0; i < Items.Capacity; i++)
            {
                keys[i] = input.ReadEncodedInt();
            }
            for (int i = 0; i < keys.Length; i++)
            {
                Items.Add(new ASItemInfo(abc, keys[i], input.ReadEncodedInt()));
            }
        }
    }
    
    public int GetSize()
    {
        int size = 0;
        size += FlashWriter.GetEncodedIntSize(NameIndex);
        size += FlashWriter.GetEncodedIntSize(Items.Count);
        foreach (var item in Items)
        {
            size += FlashWriter.GetEncodedIntSize(item.KeyIndex);
            size += FlashWriter.GetEncodedIntSize(item.ValueIndex);
        }
        return size;
    }
    public void WriteTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(NameIndex);
        output.WriteEncodedInt(Items.Count);
        for (int i = 0; i < Items.Count; i++)
        {
            output.WriteEncodedInt(Items[i].KeyIndex);
        }
        for (int i = 0; i < Items.Count; i++)
        {
            output.WriteEncodedInt(Items[i].ValueIndex);
        }
    }
}
