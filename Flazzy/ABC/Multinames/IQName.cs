namespace Flazzy.ABC
{
    public interface IQName
    {
        string Name { get; }
        int NameIndex { get; set; }

        ASNamespace Namespace { get; }
        int NamespaceIndex { get; set; }
    }
}