using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASMultiname : FlashItem, IEquatable<ASMultiname>, IPoolConstant, IQName, IRTQName, IMultiname, IMultinameL
    {
        public MultinameKind Kind { get; set; }
        public ASConstantPool Pool { get; init; }

        public bool IsRuntime => Kind switch
        {
            MultinameKind.RTQName or
            MultinameKind.RTQNameA or
            MultinameKind.MultinameL or
            MultinameKind.MultinameLA => true,
            _ => false,
        };
        public bool IsAttribute => Kind switch
        {
            MultinameKind.QNameA or
            MultinameKind.RTQNameA or
            MultinameKind.RTQNameLA or
            MultinameKind.MultinameA or
            MultinameKind.MultinameLA => true,
            _ => false,
        };
        public bool IsNameNeeded => Kind switch
        {
            MultinameKind.RTQNameL or
            MultinameKind.RTQNameLA or
            MultinameKind.MultinameL or
            MultinameKind.MultinameLA => true,
            _ => false,
        };
        public bool IsNamespaceNeeded => Kind switch
        {
            MultinameKind.RTQName or
            MultinameKind.RTQNameA or
            MultinameKind.RTQNameL or
            MultinameKind.RTQNameLA => true,
            _ => false,
        };

        public int NameIndex { get; set; }
        public string Name => Pool.Strings[NameIndex];

        public int QNameIndex { get; set; }
        public ASMultiname QName => Pool.Multinames[QNameIndex];

        public int NamespaceIndex { get; set; }
        public ASNamespace Namespace => Pool.Namespaces[NamespaceIndex];

        public int NamespaceSetIndex { get; set; }
        public ASNamespaceSet NamespaceSet => Pool.NamespaceSets[NamespaceSetIndex];

        public List<int> TypeIndices { get; }
        protected override string DebuggerDisplay => $"{Kind}: \"{Namespace.Name}.{Name}\"";

        public static bool operator ==(ASMultiname left, ASMultiname right)
        {
            return EqualityComparer<ASMultiname>.Default.Equals(left, right);
        }
        public static bool operator !=(ASMultiname left, ASMultiname right)
        {
            return !(left == right);
        }

        public ASMultiname(ASConstantPool pool)
        {
            Pool = pool;
            TypeIndices = new List<int>();
        }
        public ASMultiname(ASConstantPool pool, FlashReader input)
            : this(pool)
        {
            Kind = (MultinameKind)input.ReadByte();
            switch (Kind)
            {
                case MultinameKind.QName:
                case MultinameKind.QNameA:
                {
                    NamespaceIndex = input.ReadInt30();
                    NameIndex = input.ReadInt30();
                    break;
                }

                case MultinameKind.RTQName:
                case MultinameKind.RTQNameA:
                {
                    NameIndex = input.ReadInt30();
                    break;
                }

                case MultinameKind.RTQNameL:
                case MultinameKind.RTQNameLA:
                {
                    /* No data. */
                    break;
                }

                case MultinameKind.Multiname:
                case MultinameKind.MultinameA:
                {
                    NameIndex = input.ReadInt30();
                    NamespaceSetIndex = input.ReadInt30();
                    break;
                }

                case MultinameKind.MultinameL:
                case MultinameKind.MultinameLA:
                {
                    NamespaceSetIndex = input.ReadInt30();
                    break;
                }

                case MultinameKind.TypeName:
                {
                    QNameIndex = input.ReadInt30();
                    TypeIndices.Capacity = input.ReadInt30();
                    for (int i = 0; i < TypeIndices.Capacity; i++)
                    {
                        int typeIndex = input.ReadInt30();
                        TypeIndices.Add(typeIndex);
                    }
                    break;
                }
            }
        }

        public IEnumerable<ASMultiname> GetTypes()
        {
            for (int i = 0; i < TypeIndices.Count; i++)
            {
                yield return Pool.Multinames[TypeIndices[i]];
            }
        }
        public override void WriteTo(FlashWriter output)
        {
            output.Write((byte)Kind);
            switch (Kind)
            {
                case MultinameKind.QName:
                case MultinameKind.QNameA:
                {
                    output.WriteInt30(NamespaceIndex);
                    output.WriteInt30(NameIndex);
                    break;
                }

                case MultinameKind.RTQName:
                case MultinameKind.RTQNameA:
                {
                    output.WriteInt30(NameIndex);
                    break;
                }

                case MultinameKind.RTQNameL:
                case MultinameKind.RTQNameLA:
                {
                    /* No data. */
                    break;
                }

                case MultinameKind.Multiname:
                case MultinameKind.MultinameA:
                {
                    output.WriteInt30(NameIndex);
                    output.WriteInt30(NamespaceSetIndex);
                    break;
                }

                case MultinameKind.MultinameL:
                case MultinameKind.MultinameLA:
                {
                    output.WriteInt30(NamespaceSetIndex);
                    break;
                }

                case MultinameKind.TypeName:
                {
                    output.WriteInt30(QNameIndex);
                    output.WriteInt30(TypeIndices.Count);
                    for (int i = 0; i < TypeIndices.Count; i++)
                    {
                        int typeIndex = TypeIndices[i];
                        output.WriteInt30(typeIndex);
                    }
                    break;
                }
            }
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Kind);
            hash.Add(IsRuntime);
            hash.Add(IsAttribute);
            hash.Add(IsNameNeeded);
            hash.Add(IsNamespaceNeeded);
            hash.Add(Name);
            hash.Add(QName);
            hash.Add(Namespace);
            hash.Add(NamespaceSet);
            return hash.ToHashCode();
        }
        public bool Equals(ASMultiname other)
        {
            if (other == null) return false;
            if (!ReferenceEquals(this, other) && Pool == other.Pool) return false;

            // Since both of these names exists within the same pool of constants, then there is no need to do a 'deep' compare.
            if (Pool != other.Pool)
            {
                // This condition is useful in cases where two name objects exists within different constant pool instances, yet refer to the same 'name'.
                if (Kind != other.Kind) return false;
                if (IsRuntime != other.IsRuntime) return false;
                if (IsAttribute != other.IsAttribute) return false;
                if (IsNameNeeded != other.IsNameNeeded) return false;
                if (IsNamespaceNeeded != other.IsNamespaceNeeded) return false;
                if (Name != other.Name) return false;
                if (QName != other.QName) return false;
                if (Namespace != other.Namespace) return false;
                if (NamespaceSet != other.NamespaceSet) return false;
            }

            return true;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as ASMultiname);
        }
    }
}