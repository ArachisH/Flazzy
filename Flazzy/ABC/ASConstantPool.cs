using System.Text;

using Flazzy.IO;

namespace Flazzy.ABC;

/// <summary>
/// Represents a block of array-based entries that reflect the constants used by all the methods.
/// </summary>
public class ASConstantPool : IFlashItem
{
    private readonly Dictionary<ASMultiname, int> _multinamesIndicesCache;
    private readonly Dictionary<string, List<ASMultiname>> _multinamesByNameCache;

    public ABCFile ABC { get; }

    public List<int> Integers { get; }
    public List<uint> UIntegers { get; }
    public List<double> Doubles { get; }
    public List<string> Strings { get; }
    public List<ASNamespace> Namespaces { get; }
    public List<ASNamespaceSet> NamespaceSets { get; }
    public List<ASMultiname> Multinames { get; }

    public ASConstantPool()
    {
        _multinamesIndicesCache = new Dictionary<ASMultiname, int>();
        _multinamesByNameCache = new Dictionary<string, List<ASMultiname>>();

        Integers = new List<int>();
        UIntegers = new List<uint>();
        Doubles = new List<double>();
        Strings = new List<string>();
        Namespaces = new List<ASNamespace>();
        NamespaceSets = new List<ASNamespaceSet>();
        Multinames = new List<ASMultiname>();
    }
    public ASConstantPool(ABCFile abc)
        : this()
    {
        ABC = abc;
    }
    public ASConstantPool(ABCFile abc, ref FlashReader input)
        : this(abc)
    {
        Integers.Capacity = input.ReadEncodedInt();
        if (Integers.Capacity > 0) Integers.Add(0);
        for (int i = 1; i < Integers.Capacity; i++)
        {
            Integers.Add(input.ReadEncodedInt());
        }

        UIntegers.Capacity = input.ReadEncodedInt();
        if (UIntegers.Capacity > 0) UIntegers.Add(0);
        for (int i = 1; i < UIntegers.Capacity; i++)
        {
            UIntegers.Add(input.ReadEncodedUInt());
        }

        // TODO: MemoryMarshal?
        Doubles.Capacity = input.ReadEncodedInt();
        if (Doubles.Capacity > 0) Doubles.Add(double.NaN);
        for (int i = 1; i < Doubles.Capacity; i++)
        {
            Doubles.Add(input.ReadDouble());
        }

        Strings.Capacity = input.ReadEncodedInt();
        if (Strings.Capacity > 0) Strings.Add(default);
        for (int i = 1; i < Strings.Capacity; i++)
        {
            Strings.Add(input.ReadNullString());
        }

        Namespaces.Capacity = input.ReadEncodedInt();
        for (int i = 1; i < Namespaces.Capacity; i++)
        {
            Namespaces.Add(new ASNamespace(this, ref input));
        }

        NamespaceSets.Capacity = input.ReadEncodedInt();
        for (int i = 1; i < NamespaceSets.Capacity; i++)
        {
            NamespaceSets.Add(new ASNamespaceSet(this, ref input));
        }

        Multinames.Capacity = input.ReadEncodedInt();
        for (int i = 1; i < Multinames.Capacity; i++)
        {
            Multinames.Add(ReadMultiname(ref input));
        }

        _multinamesByNameCache.TrimExcess();
        _multinamesIndicesCache.TrimExcess();
    }

    public object GetConstant(ConstantKind type, int index)
    {
        return type switch
        {
            ConstantKind.True => true,
            ConstantKind.False => false,

            ConstantKind.String => Strings[index],
            ConstantKind.Double => Doubles[index],
            ConstantKind.Integer => Integers[index],
            ConstantKind.UInteger => UIntegers[index],
            ConstantKind.Namespace => Namespaces[index],

            ConstantKind.Null or ConstantKind.Undefined or _ => null,
        };
    }

    public int AddConstant(object value, bool recycle = true)
    {
        return value switch
        {
            int @int => AddConstant(Integers, @int, recycle),
            uint @uint => AddConstant(UIntegers, @uint, recycle),
            double @double => AddConstant(Doubles, @double, recycle),
            string @string => AddConstant(Strings, @string, recycle),

            ASMultiname multiname => AddConstant(Multinames, multiname, recycle),
            ASNamespace @namespace => AddConstant(Namespaces, @namespace, recycle),
            ASNamespaceSet namespaceSet => AddConstant(NamespaceSets, namespaceSet, recycle),

            _ => throw new ArgumentException("The provided value does not belone anywhere in the constant pool.", nameof(value)),
        };
    }
    protected virtual int AddConstant<T>(List<T> constants, T value, bool recycle)
    {
        int index = recycle ? constants.IndexOf(value, 1) : -1;
        if (index == -1)
        {
            constants.Add(value);
            index = constants.Count - 1;
        }
        return index;
    }

    public int GetMultinameIndex(string name)
    {
        return GetMultinameIndices(name).FirstOrDefault();
    }
    public ASMultiname GetMultiname(string name)
    {
        return GetMultinames(name).FirstOrDefault();
    }

    public IEnumerable<int> GetMultinameIndices(string name)
    {
        foreach (ASMultiname multiname in GetMultinames(name))
        {
            yield return _multinamesIndicesCache[multiname];
        }
    }
    public IEnumerable<ASMultiname> GetMultinames(string name)
    {
        return _multinamesByNameCache.GetValueOrDefault(name) ?? Enumerable.Empty<ASMultiname>();
    }

    private ASMultiname ReadMultiname(ref FlashReader input)
    {
        ASMultiname multiname = new(this, ref input);
        if (!string.IsNullOrWhiteSpace(multiname.Name))
        {
            if (!_multinamesByNameCache.TryGetValue(multiname.Name, out List<ASMultiname> multinames))
            {
                multinames = new List<ASMultiname>();
                _multinamesByNameCache.Add(multiname.Name, multinames);
            }
            multinames.Add(multiname);
        }
        _multinamesIndicesCache.Add(multiname, Multinames.Count);
        return multiname;
    }

    public int GetSize()
    {
        int size = 0;
        size += FlashWriter.GetEncodedIntSize(Integers.Count);
        for (int i = 1; i < Integers.Count; i++)
        {
            size += FlashWriter.GetEncodedIntSize(Integers[i]);
        }

        size += FlashWriter.GetEncodedIntSize(UIntegers.Count);
        for (int i = 1; i < UIntegers.Count; i++)
        {
            size += FlashWriter.GetEncodedUIntSize(UIntegers[i]);
        }

        size += FlashWriter.GetEncodedIntSize(Doubles.Count);
        size += Doubles.Count * sizeof(double);

        size += FlashWriter.GetEncodedIntSize(Strings.Count);
        for (int i = 1; i < Strings.Count; i++)
        {
            int length = Encoding.UTF8.GetByteCount(Strings[i]);
            size += FlashWriter.GetEncodedIntSize(length);
            size += length;
        }

        size += FlashWriter.GetEncodedIntSize(Namespaces.Count);
        for (int i = 1; i < Namespaces.Count; i++)
        {
            size += Namespaces[i].GetSize();
        }

        size += FlashWriter.GetEncodedIntSize(NamespaceSets.Count);
        for (int i = 1; i < NamespaceSets.Count; i++)
        {
            size += NamespaceSets[i].GetSize();
        }

        size += FlashWriter.GetEncodedIntSize(Multinames.Count);
        for (int i = 1; i < Multinames.Count; i++)
        {
            size += Multinames[i].GetSize();
        }
        return size;
    }
    public void WriteTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(Integers.Count);
        for (int i = 1; i < Integers.Count; i++)
        {
            output.WriteEncodedInt(Integers[i]);
        }

        output.WriteEncodedInt(UIntegers.Count);
        for (int i = 1; i < UIntegers.Count; i++)
        {
            output.WriteEncodedUInt(UIntegers[i]);
        }

        output.WriteEncodedInt(Doubles.Count);
        for (int i = 1; i < Doubles.Count; i++)
        {
            output.Write(Doubles[i]);
        }

        output.WriteEncodedInt(Doubles.Count);
        for (int i = 1; i < Doubles.Count; i++)
        {
            output.Write(Doubles[i]);
        }

        output.WriteEncodedInt(Doubles.Count);
        for (int i = 1; i < Doubles.Count; i++)
        {
            output.Write(Doubles[i]);
        }

        output.WriteEncodedInt(Strings.Count);
        for (int i = 1; i < Strings.Count; i++)
        {
            output.WriteString(Strings[i]);
        }

        WriteItems(ref output, Namespaces);
        WriteItems(ref output, NamespaceSets);
        WriteItems(ref output, Multinames);
    }

    private static void WriteItems<T>(ref FlashWriter output, List<T> constants) where T : IFlashItem
    {
        output.WriteEncodedInt(constants.Count);
        for (int i = 1; i < constants.Count; i++)
        {
            constants[i].WriteTo(ref output);
        }
    }
}
