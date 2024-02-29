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
    public ASMetadata(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        NameIndex = input.ReadEncodedInt();
        Items.Capacity = input.ReadEncodedInt();
        if (Items.Capacity > 0)
        {
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
        size += SpanFlashWriter.GetEncodedIntSize(NameIndex);
        size += SpanFlashWriter.GetEncodedIntSize(Items.Count);
        for (int i = 0; i < Items.Count; i++)
        {
            size += SpanFlashWriter.GetEncodedIntSize(Items[i].KeyIndex);
            size += SpanFlashWriter.GetEncodedIntSize(Items[i].ValueIndex);
        }
        return size;
    }
    public void WriteTo(ref SpanFlashWriter output)
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