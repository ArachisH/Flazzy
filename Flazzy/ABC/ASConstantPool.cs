﻿using System;
using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    /// <summary>
    /// Represents a block of array-based entries that reflect the constants used by all the methods.
    /// </summary>
    public class ASConstantPool : FlashItem
    {
        private readonly ABCFile _abc;
        private readonly FlashReader _input;

        /// <summary>
        /// Gets a list of integer constants referenced by the bytecode.
        /// </summary>
        public List<int> Integers { get; }
        /// <summary>
        /// Gets a list of unsigned integer constants references by the bytecode.
        /// </summary>
        public List<uint> UIntegers { get; }
        /// <summary>
        /// Gets a list of IEEE double-precision floating point constants referenced by the bytecode.
        /// </summary>
        public List<double> Doubles { get; }
        /// <summary>
        /// Gets a list of UTF-8 encoded strings referenced by the compiled code, and by many other parts of the <see cref="ABCFile"/>.
        /// </summary>
        public List<string> Strings { get; }
        /// <summary>
        /// Gets a list of namespaces used by the bytecode.
        /// </summary>
        public List<ASNamespace> Namespaces { get; }
        /// <summary>
        /// Gets a of list of namespace sets used in the descriptions of multinames.
        /// </summary>
        public List<ASNamespaceSet> NamespaceSets { get; }
        /// <summary>
        /// Gets a list of names used by the bytecode.
        /// </summary>
        public List<ASMultiname> Multinames { get; }

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

        public ASConstantPool()
        {
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
            _abc = abc;
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
        }

        private ASMultiname ReadMultiname()
        {
            return new ASMultiname(this, _input);
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