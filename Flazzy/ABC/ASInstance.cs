using System.Linq;
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

        public override ASMultiname QName
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
            string as3 = QName.Namespace.GetAS3Modifiers();
            bool isInterface = Flags.HasFlag(ClassFlags.Interface);

            if (!string.IsNullOrWhiteSpace(as3)) as3 += " ";
            if (Flags.HasFlag(ClassFlags.Final))
            {
                as3 += "final ";
            }

            if (Flags.HasFlag(ClassFlags.Interface))
            {
                as3 += "interface ";
            }
            else as3 += "class ";

            as3 += QName.Name;

            if (!isInterface && Super.Name != "Object")
            {
                as3 += $" extends {Super.Name}";
            }

            if (InterfaceIndices.Count > 0)
            {
                string interfacesAS3 = string.Join(
                    ", ", GetInterfaces().Select(i => i.Name));

                as3 += (" implements " + interfacesAS3);
            }

            return as3;
        }
        public override void WriteTo(FlashWriter output)
        {
            output.WriteInt30(QNameIndex);
            output.WriteInt30(SuperIndex);
            output.Write((byte)Flags);

            if (Flags.HasFlag(ClassFlags.ProtectedNamespace))
            {
                output.WriteInt30(ProtectedNamespaceIndex);
            }

            output.WriteInt30(InterfaceIndices.Count);
            for (int i = 0; i < InterfaceIndices.Count; i++)
            {
                int interfaceIndex = InterfaceIndices[i];
                output.WriteInt30(interfaceIndex);
            }

            output.WriteInt30(ConstructorIndex);
            base.WriteTo(output);
        }
    }
}