namespace Flazzy.ABC
{
    public interface IAS3Item : IFlashItem
    {
        ABCFile ABC { get; }

        string ToAS3();
    }
}