using Flazzy.IO;

namespace Flazzy.ABC;

public class ASInstance : ASContainer, IAS3Item
{
    public ClassFlags Flags { get; set; }
    public bool IsInterface => Flags.HasFlag(ClassFlags.Interface);

    public int SuperIndex { get; set; }
    public ASMultiname Super => ABC.Pool.Multinames[SuperIndex];

    public int ConstructorIndex { get; set; }
    public ASMethod Constructor => ABC.Methods[ConstructorIndex];

    public int QNameIndex { get; set; }
    public override ASMultiname QName => ABC.Pool.Multinames[QNameIndex];

    public int ProtectedNamespaceIndex { get; set; }
    public ASNamespace ProtectedNamespace => ABC.Pool.Namespaces[ProtectedNamespaceIndex];

    public List<int> InterfaceIndices { get; }

    public ASInstance(ABCFile abc)
        : base(abc)
    {
        InterfaceIndices = new List<int>();
    }
    public ASInstance(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        QNameIndex = input.ReadEncodedInt();
        SuperIndex = input.ReadEncodedInt();
        Flags = (ClassFlags)input.ReadByte();

        if (Flags.HasFlag(ClassFlags.ProtectedNamespace))
        {
            ProtectedNamespaceIndex = input.ReadEncodedInt();
        }

        InterfaceIndices.Capacity = input.ReadEncodedInt();
        for (int i = 0; i < InterfaceIndices.Capacity; i++)
        {
            InterfaceIndices.Add(input.ReadEncodedInt());
        }

        ConstructorIndex = input.ReadEncodedInt();
        Constructor.IsConstructor = true;
        Constructor.Container = this;

        PopulateTraits(ref input);
    }

    public IEnumerable<ASMultiname> GetInterfaces()
    {
        for (int i = 0; i < InterfaceIndices.Count; i++)
        {
            yield return ABC.Pool.Multinames[InterfaceIndices[i]];
        }
    }
    public bool ContainsInterface(string qualifiedName)
    {
        if (Super.Name != "Object")
        {
            ASInstance superInstance = ABC.GetInstance(Super);
            if (superInstance.ContainsInterface(qualifiedName))
            {
                return true;
            }
        }
        foreach (ASMultiname @interface in GetInterfaces())
        {
            if (@interface.Name != qualifiedName) continue;
            return true;
        }
        return false;
    }

    public string ToAS3()
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
        if (!isInterface && ((Super?.Name ?? "Object") != "Object"))
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

    public override int GetSize()
    {
        int size = 0;
        size += SpanFlashWriter.GetEncodedIntSize(QNameIndex);
        size += SpanFlashWriter.GetEncodedIntSize(SuperIndex);
        size += sizeof(byte);

        if (Flags.HasFlag(ClassFlags.ProtectedNamespace))
        {
            size += SpanFlashWriter.GetEncodedIntSize(ProtectedNamespaceIndex);
        }

        size += SpanFlashWriter.GetEncodedIntSize(InterfaceIndices.Count);
        for (int i = 0; i < InterfaceIndices.Count; i++)
        {
            size += SpanFlashWriter.GetEncodedIntSize(InterfaceIndices[i]);
        }

        size += SpanFlashWriter.GetEncodedIntSize(ConstructorIndex);
        return size + base.GetSize();
    }
    public override void WriteTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(QNameIndex);
        output.WriteEncodedInt(SuperIndex);
        output.Write((byte)Flags);

        if (Flags.HasFlag(ClassFlags.ProtectedNamespace))
        {
            output.WriteEncodedInt(ProtectedNamespaceIndex);
        }

        output.WriteEncodedInt(InterfaceIndices.Count);
        for (int i = 0; i < InterfaceIndices.Count; i++)
        {
            output.WriteEncodedInt(InterfaceIndices[i]);
        }

        output.WriteEncodedInt(ConstructorIndex);
        base.WriteTo(ref output);
    }

    public override string ToString() => ToAS3();
}