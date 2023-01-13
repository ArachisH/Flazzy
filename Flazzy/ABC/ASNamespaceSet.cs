using Flazzy.IO;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Flazzy.ABC;

[DebuggerDisplay("Namespaces: {NamespaceIndices.Count:n0}")]
public class ASNamespaceSet : IEquatable<ASNamespaceSet>, IFlashItem, IPoolConstant
{
    public ASConstantPool Pool { get; init; }
    public List<int> NamespaceIndices { get; }

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
    public ASNamespaceSet(ASConstantPool pool, ref FlashReader input)
        : this(pool)
    {
        NamespaceIndices.Capacity = input.ReadEncodedInt();
        for (int i = 0; i < NamespaceIndices.Capacity; i++)
        {
            NamespaceIndices.Add(input.ReadEncodedInt());
        }
    }

    public IEnumerable<ASNamespace> GetNamespaces()
    {
        for (int i = 0; i < NamespaceIndices.Count; i++)
        {
            yield return Pool.Namespaces[NamespaceIndices[i]];
        }
    }

    public int GetSize()
    {
        int size = 0;
        size += FlashWriter.GetEncodedIntSize(NamespaceIndices.Count);
        for (int i = 0; i < NamespaceIndices.Count; i++)
        {
            size += FlashWriter.GetEncodedIntSize(NamespaceIndices.Count);
        }
        return size;
    }
    public void WriteTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(NamespaceIndices.Count);
        for (int i = 0; i < NamespaceIndices.Count; i++)
        {
            output.WriteEncodedInt(NamespaceIndices[i]);
        }
    }

    public bool Equals(ASNamespaceSet other)
    {
        if (ReferenceEquals(this, other)) return true;

        if (NamespaceIndices.Count != other.NamespaceIndices.Count) return false;
        for (int i = 0; i < NamespaceIndices.Count; i++)
        {
            if (Pool.Namespaces[NamespaceIndices[i]] != other.Pool.Namespaces[NamespaceIndices[i]]) 
                return false;
        }
        return true;
    }
    public override bool Equals(object obj)
        => obj is ASNamespaceSet other && Equals(other);
    
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (ASNamespace @namespace in GetNamespaces())
        {
            hash.Add(@namespace);
        }
        return hash.ToHashCode();
    }
} 