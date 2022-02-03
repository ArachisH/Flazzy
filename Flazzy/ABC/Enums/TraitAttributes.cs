namespace Flazzy.ABC
{
    [Flags]
    public enum TraitAttributes
    {
        None     = 0,
        Final    = 1 << 0,
        Override = 1 << 1,
        Metadata = 1 << 2
    }
}
