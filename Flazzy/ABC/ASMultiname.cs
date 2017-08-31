using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASMultiname : ConstantItem, IQName, IRTQName, IMultiname, IMultinameL
    {
        public MultinameKind Kind { get; set; }

        public bool IsRuntime
        {
            get
            {
                switch (Kind)
                {
                    case MultinameKind.RTQName:
                    case MultinameKind.RTQNameA:

                    case MultinameKind.MultinameL:
                    case MultinameKind.MultinameLA:
                    return true;

                    default: return false;
                }
            }
        }
        public bool IsAttribute
        {
            get
            {
                switch (Kind)
                {
                    case MultinameKind.QNameA:

                    case MultinameKind.RTQNameA:
                    case MultinameKind.RTQNameLA:

                    case MultinameKind.MultinameA:
                    case MultinameKind.MultinameLA:
                    return true;

                    default: return false;
                }
            }
        }
        public bool IsNameNeeded
        {
            get
            {
                switch (Kind)
                {
                    case MultinameKind.RTQNameL:
                    case MultinameKind.RTQNameLA:

                    case MultinameKind.MultinameL:
                    case MultinameKind.MultinameLA:
                    return true;

                    default: return false;
                }
            }
        }
        public bool IsNamespaceNeeded
        {
            get
            {
                switch (Kind)
                {
                    case MultinameKind.RTQName:
                    case MultinameKind.RTQNameA:

                    case MultinameKind.RTQNameL:
                    case MultinameKind.RTQNameLA:
                    return true;

                    default: return false;
                }
            }
        }

        public int NameIndex { get; set; }
        public string Name => Pool.Strings[NameIndex];

        public int QNameIndex { get; set; }
        public ASMultiname QName => Pool.Multinames[QNameIndex];

        public int NamespaceIndex { get; set; }
        public ASNamespace Namespace => Pool.Namespaces[NamespaceIndex];

        public int NamespaceSetIndex { get; set; }
        public ASNamespaceSet NamespaceSet => Pool.NamespaceSets[NamespaceSetIndex];

        public List<int> TypeIndices { get; }
        protected override string DebuggerDisplay => $"{Kind}: \"{Name}\"";

        public ASMultiname(ASConstantPool pool)
            : base(pool)
        {
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
                int typeIndex = TypeIndices[i];
                ASMultiname type = Pool.Multinames[typeIndex];
                yield return type;
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
    }
}