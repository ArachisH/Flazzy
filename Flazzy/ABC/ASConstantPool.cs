using Flazzy.IO;

namespace Flazzy.ABC
{
    /// <summary>
    /// Represents a block of array-based entries that reflect the constants used by all the methods.
    /// </summary>
    public class ASConstantPool : FlashItem
    {
        private readonly Dictionary<ASMultiname, int> _multinamesIndicesCache;
        private readonly Dictionary<string, List<ASMultiname>> _multinamesByNameCache;

        private readonly FlashReader _input;

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
        public ASConstantPool(ABCFile abc, FlashReader input)
            : this(abc)
        {
            _input = input;

            PopulateList(Integers, input.ReadInt30, 0);
            PopulateList<uint>(UIntegers, input.ReadUInt30, 0);
            PopulateList(Doubles, input.ReadDouble, double.NaN);
            PopulateList(Strings, input.ReadString, null);
            PopulateList(Namespaces, ReadNamespace, null);
            PopulateList(NamespaceSets, ReadNamespaceSet, null);
            PopulateList(Multinames, ReadMultiname, null);

            _multinamesByNameCache.TrimExcess();
            _multinamesIndicesCache.TrimExcess();
        }

        public object GetConstant(ConstantKind type, int index)
        {
            switch (type)
            {
                case ConstantKind.True: return true;
                case ConstantKind.False: return false;

                case ConstantKind.Null:
                case ConstantKind.Undefined: return null;

                case ConstantKind.String: return Strings[index];
                case ConstantKind.Double: return Doubles[index];
                case ConstantKind.Integer: return Integers[index];
                case ConstantKind.UInteger: return UIntegers[index];

                case ConstantKind.Namespace: return Namespaces[index];

                default: return null;
            }
        }

        public int AddConstant(object value, bool recycle = true)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Int32: return AddConstant(Integers, (int)value, recycle);
                case TypeCode.UInt32: return AddConstant(UIntegers, (uint)value, recycle);
                case TypeCode.Double: return AddConstant(Doubles, (double)value, recycle);
                case TypeCode.String: return AddConstant(Strings, (string)value, recycle);
                default:
                {
                    return value switch
                    {
                        ASMultiname multiname => AddConstant(Multinames, multiname, recycle),
                        ASNamespace @namespace => AddConstant(Namespaces, @namespace, recycle),
                        ASNamespaceSet namespaceSet => AddConstant(NamespaceSets, namespaceSet, recycle),
                        _ => throw new ArgumentException("The provided value does not belone anywhere in the constant pool.", nameof(value)),
                    };
                }
            }
        }
        protected virtual int AddConstant<T>(List<T> constants, T value, bool recycle)
        {
            int index = (recycle ? constants.IndexOf(value, 1) : -1);
            if (index == -1)
            {
                constants.Add(value);
                index = (constants.Count - 1);
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

        private ASMultiname ReadMultiname()
        {
            ASMultiname multiname = new(this, _input);
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
        private ASNamespace ReadNamespace()
        {
            return new ASNamespace(this, _input);
        }
        private ASNamespaceSet ReadNamespaceSet()
        {
            return new ASNamespaceSet(this, _input);
        }

        private void PopulateList<T>(List<T> list, Func<T> reader, T defaultValue)
        {
            list.Capacity = _input.ReadInt30();
            if (list.Equals(Multinames))
            {
                _multinamesByNameCache.EnsureCapacity(list.Capacity);
                _multinamesIndicesCache.EnsureCapacity(list.Capacity);
            }
            if (list.Capacity > 0)
            {
                list.Add(defaultValue);
                for (int i = 1; i < list.Capacity; i++)
                {
                    T value = reader();
                    list.Add(value);
                }
            }
        }

        public override void WriteTo(FlashWriter output)
        {
            WriteTo(output, output.WriteInt30, Integers);
            WriteTo(output, output.WriteUInt30, UIntegers);
            WriteTo(output, output.Write, Doubles);
            WriteTo(output, output.Write, Strings);
            WriteTo(output, output.WriteItem, Namespaces);
            WriteTo(output, output.WriteItem, NamespaceSets);
            WriteTo(output, output.WriteItem, Multinames);
        }
        private void WriteTo<T>(FlashWriter output, Action<T> writer, List<T> constants)
        {
            output.WriteInt30(constants.Count);
            for (int i = 1; i < constants.Count; i++)
            {
                T value = constants[i];
                writer(value);
            }
        }
    }
}