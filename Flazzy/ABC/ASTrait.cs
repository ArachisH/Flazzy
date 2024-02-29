using System.Text;

using Flazzy.IO;

namespace Flazzy.ABC;

public class ASTrait : IFlashItem, IAS3Item, IMethodGSTrait, ISlotConstantTrait, IClassTrait, IFunctionTrait
{
    public ABCFile ABC { get; }

    public int QNameIndex { get; set; }
    public ASMultiname QName => ABC.Pool.Multinames[QNameIndex];

    public int TypeIndex { get; set; }
    public ASMultiname Type => Kind switch
    {
        TraitKind.Slot or TraitKind.Constant => ABC.Pool.Multinames[TypeIndex],
        _ => null
    };

    public int MethodIndex { get; set; }
    public ASMethod Method => Kind switch
    {
        TraitKind.Method or TraitKind.Getter or TraitKind.Setter => ABC.Methods[MethodIndex],
        _ => null
    };

    public int FunctionIndex { get; set; }
    public ASMethod Function => Kind is TraitKind.Function ? ABC.Methods[FunctionIndex] : null;

    public int ClassIndex { get; set; }
    public ASClass Class => Kind is TraitKind.Class ? ABC.Classes[ClassIndex] : null;

    public int ValueIndex { get; set; }
    public object Value => ABC.Pool.GetConstant(ValueKind, ValueIndex);

    public ConstantKind ValueKind { get; set; }

    public int Id { get; set; }
    public List<int> MetadataIndices { get; }

    public bool IsStatic { get; internal set; }

    public TraitKind Kind { get; set; }
    public TraitAttributes Attributes { get; set; }

    public ASTrait(ABCFile abc)
    {
        ABC = abc;
        MetadataIndices = new List<int>();
    }
    public ASTrait(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        QNameIndex = input.ReadEncodedInt();

        byte bitContainer = input.ReadByte();
        Kind = (TraitKind)(bitContainer & 0x0F);
        Attributes = (TraitAttributes)(bitContainer >> 4);

        Id = input.ReadEncodedInt();
        switch (Kind)
        {
            case TraitKind.Slot:
            case TraitKind.Constant:
                {
                    TypeIndex = input.ReadEncodedInt();
                    ValueIndex = input.ReadEncodedInt();
                    if (ValueIndex != 0)
                    {
                        ValueKind = (ConstantKind)input.ReadByte();
                    }
                    break;
                }

            case TraitKind.Method:
            case TraitKind.Getter:
            case TraitKind.Setter:
                {
                    MethodIndex = input.ReadEncodedInt();
                    Method.Trait = this;
                    break;
                }

            case TraitKind.Class:
                {
                    ClassIndex = input.ReadEncodedInt();
                    break;
                }

            case TraitKind.Function:
                {
                    FunctionIndex = input.ReadEncodedInt();
                    break;
                }
        }

        if (Attributes.HasFlag(TraitAttributes.Metadata))
        {
            MetadataIndices.Capacity = input.ReadEncodedInt();
            for (int i = 0; i < MetadataIndices.Capacity; i++)
            {
                MetadataIndices.Add(input.ReadEncodedInt());
            }
        }
    }

    public IEnumerable<ASMetadata> GetMetadata()
    {
        for (int i = 0; i < MetadataIndices.Count; i++)
        {
            int metadatumIndex = MetadataIndices[i];
            ASMetadata metadatum = ABC.Metadata[metadatumIndex];
            yield return metadatum;
        }
    }

    public string ToAS3()
    {
        if (Kind is TraitKind.Constant or TraitKind.Slot)
        {
            StringBuilder builder = new();
            if (Attributes.HasFlag(TraitAttributes.Override))
            {
                builder.Append("override ");
            }
            var modifiers = QName.Namespace.GetAS3Modifiers();
            if (!string.IsNullOrEmpty(modifiers))
            {
                builder.Append(modifiers);
                builder.Append(' ');
            }
            if (IsStatic)
            {
                builder.Append("static ");
            }
            builder.Append(Kind == TraitKind.Constant ? "const " : "var ");
            builder.Append(QName.Name);
            if (Type != null)
            {
                builder.Append(':');
                builder.Append(Type.Name ?? Type.QName.Name);
                if (Type.Kind == MultinameKind.TypeName)
                {
                    builder.Append(".<");
                    builder.Append(string.Join(',', Type.TypeIndices.Select(i => ABC.Pool.Multinames[i].Name)));
                    builder.Append('>');
                }
            }
            if (!string.IsNullOrEmpty(Value?.ToString()))
            {
                builder.Append(" = ");
                if (ValueKind == ConstantKind.String)
                {
                    builder.Append('"');
                    builder.Append(Value.ToString());
                    builder.Append('"');
                }
                else builder.Append(Value.ToString().ToLower());
            }
            builder.Append(';');
            return builder.ToString();
        }
        else return string.Empty;
    }
    public int GetSize()
    {
        int size = 0;
        size += SpanFlashWriter.GetEncodedIntSize(QNameIndex);
        size += sizeof(byte);
        size += SpanFlashWriter.GetEncodedIntSize(Id);
        switch (Kind)
        {
            case TraitKind.Slot:
            case TraitKind.Constant:
                {
                    size += SpanFlashWriter.GetEncodedIntSize(TypeIndex);
                    size += SpanFlashWriter.GetEncodedIntSize(ValueIndex);
                    if (ValueIndex != 0)
                    {
                        size += sizeof(byte);
                    }
                    break;
                }

            case TraitKind.Method:
            case TraitKind.Getter:
            case TraitKind.Setter:
                {
                    size += SpanFlashWriter.GetEncodedIntSize(MethodIndex);
                    break;
                }

            case TraitKind.Class:
                {
                    size += SpanFlashWriter.GetEncodedIntSize(ClassIndex);
                    break;
                }

            case TraitKind.Function:
                {
                    size += SpanFlashWriter.GetEncodedIntSize(FunctionIndex);
                    break;
                }
        }

        if (Attributes.HasFlag(TraitAttributes.Metadata))
        {
            size += SpanFlashWriter.GetEncodedIntSize(MetadataIndices.Count);
            for (int i = 0; i < MetadataIndices.Count; i++)
            {
                size += SpanFlashWriter.GetEncodedIntSize(MetadataIndices[i]);
            }
        }
        return size;
    }
    public void WriteTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(QNameIndex);
        output.Write((byte)(((byte)Attributes << 4) + (byte)Kind));
        output.WriteEncodedInt(Id);
        switch (Kind)
        {
            case TraitKind.Slot:
            case TraitKind.Constant:
                {
                    output.WriteEncodedInt(TypeIndex);
                    output.WriteEncodedInt(ValueIndex);
                    if (ValueIndex != 0)
                    {
                        output.Write((byte)ValueKind);
                    }
                    break;
                }

            case TraitKind.Method:
            case TraitKind.Getter:
            case TraitKind.Setter:
                {
                    output.WriteEncodedInt(MethodIndex);
                    break;
                }

            case TraitKind.Class:
                {
                    output.WriteEncodedInt(ClassIndex);
                    break;
                }

            case TraitKind.Function:
                {
                    output.WriteEncodedInt(FunctionIndex);
                    break;
                }
        }

        if (Attributes.HasFlag(TraitAttributes.Metadata))
        {
            output.WriteEncodedInt(MetadataIndices.Count);
            for (int i = 0; i < MetadataIndices.Count; i++)
            {
                int metadatumIndex = MetadataIndices[i];
                output.WriteEncodedInt(metadatumIndex);
            }
        }
    }

    public override string ToString() => (Kind + ": " + QName.Name);
}