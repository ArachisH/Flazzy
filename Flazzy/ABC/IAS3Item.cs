namespace Flazzy.ABC;

public interface IAS3Item
{
    ABCFile ABC { get; }

    string ToAS3();
}
