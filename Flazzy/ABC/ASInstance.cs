using System;
using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASInstance : ASContainer
    {
        public ClassFlags Flags { get; set; }

        public ASMultiname Super
        {
            get { return ABC.Pool.Multinames[SuperIndex]; }
        }
        public int SuperIndex { get; set; }

        public ASMethod Constructor
        {
            get { return ABC.Methods[ConstructorIndex]; }
        }
        public int ConstructorIndex { get; set; }

        public ASMultiname QName
        {
            get { return ABC.Pool.Multinames[QNameIndex]; }
        }
        public int QNameIndex { get; set; }

        public ASNamespace ProtectedNamespace
        {
            get { return ABC.Pool.Namespaces[ProtectedNamespaceIndex]; }
        }
        public int ProtectedNamespaceIndex { get; set; }

        public List<int> InterfaceIndices { get; }

        protected override string DebuggerDisplay
        {
            get
            {
                return ToAS3();
            }
        }

        public ASInstance(ABCFile abc)
            : base(abc)
        {
            InterfaceIndices = new List<int>();
        }
        public ASInstance(ABCFile abc, FlashReader input)
            : this(abc)
        {
            QNameIndex = input.ReadInt30();
            SuperIndex = input.ReadInt30();
            Flags = (ClassFlags)input.ReadByte();

            if (Flags.HasFlag(ClassFlags.ProtectedNamespace))
            {
                ProtectedNamespaceIndex = input.ReadInt30();
            }

            InterfaceIndices.Capacity = input.ReadInt30();
            for (int i = 0; i < InterfaceIndices.Capacity; i++)
            {
                int interfaceIndex = input.ReadInt30();
                InterfaceIndices.Add(interfaceIndex);
            }

            ConstructorIndex = input.ReadInt30();
            PopulateTraits(input);
        }

        public IEnumerable<ASMultiname> GetInterfaces()
        {
            for (int i = 0; i < InterfaceIndices.Count; i++)
            {
                int interfaceIndex = InterfaceIndices[i];
                ASMultiname @interface = ABC.Pool.Multinames[interfaceIndex];
                yield return @interface;
            }
        }

        public override string ToAS3()
        {
            string modifiers = QName.Namespace.GetAS3Modifiers();
            if (Flags.HasFlag(ClassFlags.Final))
            {
                modifiers += "final";
            }

            string type = "class";
            if (Flags.HasFlag(ClassFlags.Interface))
            {
                type = "interface";
            }

            //TODO: Add Super & Interfaces.
            return $"{modifiers} {type} {QName.Name}";
        }
        public override void WriteTo(FlashWriter output)
        {
            throw new NotImplementedException();
        }
    }
}