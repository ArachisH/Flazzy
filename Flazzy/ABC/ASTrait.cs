using System.Text;


using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASTrait : IAS3Item, IMethodGSTrait, ISlotConstantTrait, IClassTrait, IFunctionTrait
    {
        public ABCFile ABC { get; }

        public int QNameIndex { get; set; }
        public ASMultiname QName => ABC.Pool.Multinames[QNameIndex];

        public ASMultiname Type
        {
            get
            {
                if (Kind == TraitKind.Slot ||
                    Kind == TraitKind.Constant)
                {
                    return ABC.Pool.Multinames[TypeIndex];
                }
                return null;
            }
        }
        public int TypeIndex { get; set; }

        public ASMethod Method
        {
            get
            {
                if (Kind == TraitKind.Method ||
                    Kind == TraitKind.Getter ||
                    Kind == TraitKind.Setter)
                {
                    return ABC.Methods[MethodIndex];
                }
                return null;
            }
        }
        public int MethodIndex { get; set; }

        public ASMethod Function
        {
            get
            {
                if (Kind != TraitKind.Function) return null;
                return ABC.Methods[FunctionIndex];
            }
        }
        public int FunctionIndex { get; set; }

        public ASClass Class
        {
            get
            {
                if (Kind != TraitKind.Class) return null;
                return ABC.Classes[ClassIndex];
            }
        }
        public int ClassIndex { get; set; }

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
        public ASTrait(ABCFile abc, ref FlashReader input)
            : this(abc)
        {
            QNameIndex = input.ReadEncodedInt();

            byte flags = input.ReadByte();
            Kind = (TraitKind)(flags & 0x0F);
            Attributes = (TraitAttributes)(flags >> 4);

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
                yield return ABC.Metadata[MetadataIndices[i]];
            }
        }

        public override string ToAS3()
        {
            switch(Kind)
            {
                case TraitKind.Constant:
                case TraitKind.Slot:
                    {
                        StringBuilder builder = new();
                        if(Attributes.HasFlag(TraitAttributes.Override))
                        {
                            builder.Append("override ");
                        }
                        var modifiers = QName.Namespace.GetAS3Modifiers();
                        if(!string.IsNullOrEmpty(modifiers))
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
                        if(Type != null)
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
            }
            return string.Empty;
        }

        public int GetSize()
        {
            int size = 0;
            size += FlashWriter.GetEncodedIntSize(QNameIndex);
            size += sizeof(byte);
            size += FlashWriter.GetEncodedIntSize(Id);
            switch (Kind)
            {
                case TraitKind.Slot:
                case TraitKind.Constant:
                {
                    size += FlashWriter.GetEncodedIntSize(TypeIndex);
                    size += FlashWriter.GetEncodedIntSize(ValueIndex);
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
                    size += FlashWriter.GetEncodedIntSize(MethodIndex);
                    break;
                }

                case TraitKind.Class:
                {
                    size += FlashWriter.GetEncodedIntSize(ClassIndex);
                    break;
                }

                case TraitKind.Function:
                {
                    size += FlashWriter.GetEncodedIntSize(FunctionIndex);
                    break;
                }
            }

            if (Attributes.HasFlag(TraitAttributes.Metadata))
            {
                size += FlashWriter.GetEncodedIntSize(MetadataIndices.Count);
                for (int i = 0; i < MetadataIndices.Count; i++)
                {
                    size += FlashWriter.GetEncodedIntSize(MetadataIndices[i]);
                }
            }
            return size;
        }
        public void WriteTo(ref FlashWriter output)
        {
            byte flags = (byte)(((byte)Attributes << 4) & (byte)Kind);

            output.WriteEncodedInt(QNameIndex);
            output.Write(flags);
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
                    output.WriteEncodedInt(MetadataIndices[i]);
                }
            }
        }
    }
}