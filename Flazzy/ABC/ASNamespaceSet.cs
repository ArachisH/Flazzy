using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASNamespaceSet : FlashItem, IEquatable<ASNamespaceSet>, IPoolConstant
    {
        public ASConstantPool Pool { get; init; }
        public List<int> NamespaceIndices { get; }

        protected override string DebuggerDisplay => $"Namespaces: {NamespaceIndices.Count:n0}";

        public static bool operator ==(ASNamespaceSet left, ASNamespaceSet right)
        {
            return EqualityComparer<ASNamespaceSet>.Default.Equals(left, right);
        }
        public static bool operator !=(ASNamespaceSet left, ASNamespaceSet right)
        {
            return !(left == right);
        }

        public ASNamespaceSet(ASConstantPool pool)
        {
            Pool = pool;
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

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (ASNamespace @namespace in GetNamespaces())
            {
                hash.Add(@namespace);
            }
            return hash.ToHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as ASNamespaceSet);
        }
        public bool Equals(ASNamespaceSet other)
        {
            if (other == null) return false;
            if (!ReferenceEquals(this, other))
            {
                if (NamespaceIndices.Count != other.NamespaceIndices.Count) return false;
                for (int i = 0; i < NamespaceIndices.Count; i++)
                {
                    if (Pool.Namespaces[NamespaceIndices[i]] != other.Pool.Namespaces[NamespaceIndices[i]]) return false;
                }
            }
            return true;
        }

        public IEnumerable<ASNamespace> GetNamespaces()
        {
            for (int i = 0; i < NamespaceIndices.Count; i++)
            {
                yield return Pool.Namespaces[NamespaceIndices[i]];
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