using Flazzy.IO;

namespace Flazzy.ABC
{
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
            //PopulateList(Integers, input.ReadInt30, 0);
            //PopulateList<uint>(UIntegers, input.ReadUInt30, 0);
            //PopulateList(Doubles, input.ReadDouble, double.NaN);
            //PopulateList(Strings, input.ReadString, null);
            //PopulateList(Namespaces, ReadNamespace, null);
            //PopulateList(NamespaceSets, ReadNamespaceSet, null);
            //PopulateList(Multinames, ReadMultiname, null);

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
        private ASNamespace ReadNamespace(ref FlashReader input)
        {
            return new ASNamespace(this, ref input);
        }
        private ASNamespaceSet ReadNamespaceSet(ref FlashReader input)
        {
            return new ASNamespaceSet(this, ref input);
        }

        //private void ReadList<T>(ref FlashReader input, List<T> list)
        //{
        //    list.Capacity = input.ReadInt30();
        //}
        //
        //private void PopulateList<T>(List<T> list, Func<T> reader, T defaultValue)
        //{
        //    list.Capacity = _input.ReadInt30();
        //    if (list.Equals(Multinames))
        //    {
        //        _multinamesByNameCache.EnsureCapacity(list.Capacity);
        //        _multinamesIndicesCache.EnsureCapacity(list.Capacity);
        //    }
        //    if (list.Capacity > 0)
        //    {
        //        list.Add(defaultValue);
        //        for (int i = 1; i < list.Capacity; i++)
        //        {
        //            T value = reader();
        //            list.Add(value);
        //        }
        //    }
        //}

        public void WriteTo(FlashWriter output)
        {
            //WriteTo(output, output.WriteInt30, Integers);
            //WriteTo(output, output.WriteUInt30, UIntegers);
            //WriteTo(output, output.Write, Doubles);
            //WriteTo(output, output.Write, Strings);
            //WriteTo(output, output.WriteItem, Namespaces);
            //WriteTo(output, output.WriteItem, NamespaceSets);
            //WriteTo(output, output.WriteItem, Multinames);
        }
        private static void WriteTo<T>(FlashWriter output, Action<T> writer, List<T> constants)
        {
            output.WriteEncodedInt(constants.Count);
            for (int i = 1; i < constants.Count; i++)
            {
                writer(constants[i]);
            }
        }

        public int GetSize()
        {
            throw new NotImplementedException();
        }
    }
}