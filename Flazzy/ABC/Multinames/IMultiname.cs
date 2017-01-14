namespace Flazzy.ABC
{
    public interface IMultiname
    {
        string Name { get; }
        int NameIndex { get; set; }

        ASNamespaceSet NamespaceSet { get; }
        int NamespaceSetIndex { get; set; }
    }
}