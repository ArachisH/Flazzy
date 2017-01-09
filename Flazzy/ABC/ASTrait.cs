using System;
using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASTrait : AS3Item
    {
        public ASMultiname QName
        {
            get { return ABC.Pool.Multinames[QNameIndex]; }
        }
        public int QNameIndex { get; set; }

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
                return (Kind == TraitKind.Function ?
                  ABC.Methods[FunctionIndex] : null);
            }
        }
        public int FunctionIndex { get; set; }

        public ASClass Class
        {
            get
            {
                return (Kind == TraitKind.Class ?
                    ABC.Classes[ClassIndex] : null);
            }
        }
        public int ClassIndex { get; set; }

        public int Id { get; set; }

        public object Value
        {
            get { return ABC.Pool.GetConstant(ValueKind, ValueIndex); }
        }
        public int ValueIndex { get; set; }
        public ConstantKind ValueKind { get; set; }

        public List<int> MetadataIndices { get; }

        public TraitKind Kind { get; set; }
        public TraitAttributes Attributes { get; set; }

        public ASTrait(ABCFile abc)
            : base(abc)
        {
            MetadataIndices = new List<int>();
        }
        public ASTrait(ABCFile abc, FlashReader input)
            : this(abc)
        {
            QNameIndex = input.ReadInt30();

            byte bitContainer = input.ReadByte();
            Kind = (TraitKind)(bitContainer & 0x0F);
            Attributes = (TraitAttributes)(bitContainer >> 4);

            Id = input.ReadInt30();
            switch (Kind)
            {
                case TraitKind.Slot:
                case TraitKind.Constant:
                {
                    TypeIndex = input.ReadInt30();
                    ValueIndex = input.ReadInt30();
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
                    MethodIndex = input.ReadInt30();
                    break;
                }

                case TraitKind.Class:
                {
                    ClassIndex = input.ReadInt30();
                    break;
                }

                case TraitKind.Function:
                {
                    FunctionIndex = input.ReadInt30();
                    break;
                }
            }

            if (Attributes.HasFlag(TraitAttributes.Metadata))
            {
                MetadataIndices.Capacity = input.ReadInt30();
                for (int i = 0; i < MetadataIndices.Capacity; i++)
                {
                    int metadatumIndex = input.ReadInt30();
                    MetadataIndices.Add(metadatumIndex);
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

        public override string ToAS3()
        {
            throw new NotImplementedException();
        }
        public override void WriteTo(FlashWriter output)
        {
            throw new NotImplementedException();
        }
    }
}