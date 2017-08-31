using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASNamespaceSet : ConstantItem
    {
        public List<int> NamespaceIndices { get; }

        public ASNamespaceSet(ASConstantPool pool)
            : base(pool)
        {
            NamespaceIndices = new List<int>();
        }
        public ASNamespaceSet(ASConstantPool pool, FlashReader input)
            : this(pool)
        {
            NamespaceIndices.Capacity = input.ReadInt30();
            for (int i = 0; i < NamespaceIndices.Capacity; i++)
            {
                int namespaceIndex = input.ReadInt30();
                NamespaceIndices.Add(namespaceIndex);
            }
        }

        protected override string DebuggerDisplay => $"Namespaces: {NamespaceIndices.Count:n0}";

        public IEnumerable<ASNamespace> GetNamespaces()
        {
            for (int i = 0; i < NamespaceIndices.Count; i++)
            {
                int namespaceIndex = NamespaceIndices[i];
                ASNamespace @namespace = Pool.Namespaces[namespaceIndex];
                yield return @namespace;
            }
        }

        public override void WriteTo(FlashWriter output)
        {
            output.WriteInt30(NamespaceIndices.Count);
            for (int i = 0; i < NamespaceIndices.Count; i++)
            {
                int namespaceIndex = NamespaceIndices[i];
                output.WriteInt30(namespaceIndex);
            }
        }
    }
}