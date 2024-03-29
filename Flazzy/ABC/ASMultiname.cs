﻿using Flazzy.IO;

namespace Flazzy.ABC;

public sealed class ASMultiname : IFlashItem, IEquatable<ASMultiname>, IPoolConstant, IQName, IRTQName, IMultiname, IMultinameL
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

    public ASMultiname(ASConstantPool pool)
    {
        Pool = pool;
        TypeIndices = new List<int>();
    }
    public ASMultiname(ASConstantPool pool, ref SpanFlashReader input)
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
                        int typeIndex = input.ReadEncodedInt();
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

    public int GetSize()
    {
        int size = 0;
        size += sizeof(byte);
        switch (Kind)
        {
            case MultinameKind.QName:
            case MultinameKind.QNameA:
                {
                    size += SpanFlashWriter.GetEncodedIntSize(NamespaceIndex);
                    size += SpanFlashWriter.GetEncodedIntSize(NameIndex);
                    break;
                }

            case MultinameKind.RTQName:
            case MultinameKind.RTQNameA:
                {
                    size += SpanFlashWriter.GetEncodedIntSize(NameIndex);
                    break;
                }

            case MultinameKind.Multiname:
            case MultinameKind.MultinameA:
                {
                    size += SpanFlashWriter.GetEncodedIntSize(NameIndex);
                    size += SpanFlashWriter.GetEncodedIntSize(NamespaceSetIndex);
                    break;
                }

            case MultinameKind.MultinameL:
            case MultinameKind.MultinameLA:
                {
                    size += SpanFlashWriter.GetEncodedIntSize(NamespaceSetIndex);
                    break;
                }

            case MultinameKind.TypeName:
                {
                    size += SpanFlashWriter.GetEncodedIntSize(QNameIndex);
                    size += SpanFlashWriter.GetEncodedIntSize(TypeIndices.Count);
                    for (int i = 0; i < TypeIndices.Count; i++)
                    {
                        size += SpanFlashWriter.GetEncodedIntSize(TypeIndices[i]);
                    }
                    break;
                }
        }
        return size;
    }
    public void WriteTo(ref SpanFlashWriter output)
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
                        int typeIndex = TypeIndices[i];
                        output.WriteEncodedInt(typeIndex);
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
        hash.Add(QNameIndex);
        hash.Add(NamespaceIndex);
        hash.Add(NamespaceSetIndex);
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
        => obj is ASMultiname multiname && Equals(multiname);
    
    public static bool operator ==(ASMultiname left, ASMultiname right)
    {
        return EqualityComparer<ASMultiname>.Default.Equals(left, right);
    }
    public static bool operator !=(ASMultiname left, ASMultiname right)
    {
        return !(left == right);
    }

    public override string ToString() => $"{Kind}: \"{Namespace.Name}.{Name}\"";
}