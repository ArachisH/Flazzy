namespace Flazzy.ABC;

public sealed class ASItemInfo
{
    private readonly ABCFile _abc;

    public int KeyIndex { get; set; }
    public string Key => _abc.Pool.Strings[KeyIndex];

    public int ValueIndex { get; set; }
    public string Value => _abc.Pool.Strings[ValueIndex];

    public ASItemInfo(ABCFile abc, int keyIndex, int valueIndex)
    {
        _abc = abc;

        KeyIndex = keyIndex;
        ValueIndex = valueIndex;
    }

    public override string ToString() => $"{Key}=\"{Value}\"";
}