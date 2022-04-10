using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC;

public class ASMultiname : IEquatable<ASMultiname>, IFlashItem, IPoolConstant, IQName, IRTQName, IMultiname, IMultinameL
{
    public ASConstantPool Pool { get; init; }

    public MultinameKind Kind { get; set; }

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
    public ASMultiname(ASConstantPool pool, ref FlashReader input)
        : this(pool)
    {
        Kind = (MultinameKind)input.ReadByte();
        switch (Kind)
        {
            case MultinameKind.QName:
            case MultinameKind.QNameA:
                {
                    NamespaceIndex = input.ReadEncodedInt();
                    NameIndex = input.ReadEncodedInt();
                    break;
                }

            case MultinameKind.RTQName:
            case MultinameKind.RTQNameA:
                {
                    NameIndex = input.ReadEncodedInt();
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
                    NameIndex = input.ReadEncodedInt();
                    NamespaceSetIndex = input.ReadEncodedInt();
                    break;
                }

            case MultinameKind.MultinameL:
            case MultinameKind.MultinameLA:
                {
                    NamespaceSetIndex = input.ReadEncodedInt();
                    break;
                }

            case MultinameKind.TypeName:
                {
                    QNameIndex = input.ReadEncodedInt();
                    TypeIndices.Capacity = input.ReadEncodedInt();
                    for (int i = 0; i < TypeIndices.Capacity; i++)
                    {
                        TypeIndices.Add(input.ReadEncodedInt());
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

    public int GetSize()
    {
        int size = 0;
        size += sizeof(byte);
        switch (Kind)
        {
            case MultinameKind.QName:
            case MultinameKind.QNameA:
                {
                    size += FlashWriter.GetEncodedIntSize(NamespaceIndex);
                    size += FlashWriter.GetEncodedIntSize(NameIndex);
                    break;
                }

            case MultinameKind.RTQName:
            case MultinameKind.RTQNameA:
                {
                    size += FlashWriter.GetEncodedIntSize(NameIndex);
                    break;
                }

            case MultinameKind.Multiname:
            case MultinameKind.MultinameA:
                {
                    size += FlashWriter.GetEncodedIntSize(NameIndex);
                    size += FlashWriter.GetEncodedIntSize(NamespaceSetIndex);
                    break;
                }

            case MultinameKind.MultinameL:
            case MultinameKind.MultinameLA:
                {
                    size += FlashWriter.GetEncodedIntSize(NamespaceSetIndex);
                    break;
                }

            case MultinameKind.TypeName:
                {
                    size += FlashWriter.GetEncodedIntSize(QNameIndex);
                    size += FlashWriter.GetEncodedIntSize(TypeIndices.Count);
                    for (int i = 0; i < TypeIndices.Count; i++)
                    {
                        size += FlashWriter.GetEncodedIntSize(TypeIndices[i]);
                    }
                    break;
                }
        }
        return size;
    }
    public void WriteTo(ref FlashWriter output)
    {
        output.Write((byte)Kind);
        switch (Kind)
        {
            case MultinameKind.QName:
            case MultinameKind.QNameA:
                {
                    output.WriteEncodedInt(NamespaceIndex);
                    output.WriteEncodedInt(NameIndex);
                    break;
                }

            case MultinameKind.RTQName:
            case MultinameKind.RTQNameA:
                {
                    output.WriteEncodedInt(NameIndex);
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
                    output.WriteEncodedInt(NameIndex);
                    output.WriteEncodedInt(NamespaceSetIndex);
                    break;
                }

            case MultinameKind.MultinameL:
            case MultinameKind.MultinameLA:
                {
                    output.WriteEncodedInt(NamespaceSetIndex);
                    break;
                }

            case MultinameKind.TypeName:
                {
                    output.WriteEncodedInt(QNameIndex);
                    output.WriteEncodedInt(TypeIndices.Count);
                    for (int i = 0; i < TypeIndices.Count; i++)
                    {
                        output.WriteEncodedInt(TypeIndices[i]);
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

    public override string ToString() => $"{Kind}: \"{Namespace.Name}.{Name}\"";

    public static bool operator ==(ASMultiname left, ASMultiname right)
    {
        return EqualityComparer<ASMultiname>.Default.Equals(left, right);
    }
    public static bool operator !=(ASMultiname left, ASMultiname right)
    {
        return !(left == right);
    }
}
