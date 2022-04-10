using Flazzy.IO;

namespace Flazzy.ABC;

public class ASItemInfo : IFlashItem
{
    private readonly ABCFile _abc;

    public int KeyIndex { get; set; }
    public string Key => _abc.Pool.Strings[KeyIndex];

    public int ValueIndex { get; set; }
    public string Value => _abc.Pool.Strings[ValueIndex];

    public ASItemInfo(ABCFile abc)
    {
        _abc = abc;
    }
    public ASItemInfo(ABCFile abc, ref FlashReader input)
        : this(abc)
    {
        KeyIndex = input.ReadEncodedInt();
        ValueIndex = input.ReadEncodedInt();
    }

    public int GetSize()
    {
        int size = 0;
        size += FlashWriter.GetEncodedIntSize(KeyIndex);
        size += FlashWriter.GetEncodedIntSize(ValueIndex);
        return size;
    }
    public void WriteTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(KeyIndex);
        output.WriteEncodedInt(ValueIndex);
    }
}
