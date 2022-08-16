﻿using Flazzy.IO;

using System.Text;

namespace Flazzy.ABC;

public class ASMethod : IFlashItem, IAS3Item
{
    public ABCFile ABC { get; }

    public int NameIndex { get; set; }
    public string Name => ABC.Pool.Strings[NameIndex];

    public int ReturnTypeIndex { get; set; }
    public ASMultiname ReturnType => ABC.Pool.Multinames[ReturnTypeIndex];

    public MethodFlags Flags { get; set; }
    public List<ASParameter> Parameters { get; }

    public ASTrait Trait { get; internal set; }
    public ASMethodBody Body { get; internal set; }
    public bool IsConstructor { get; internal set; }
    public ASContainer Container { get; internal set; }
    public bool IsAnonymous => Trait == null && !IsConstructor;

    public ASMethod(ABCFile abc)
    {
        ABC = abc;
        Parameters = new List<ASParameter>();
    }
    public ASMethod(ABCFile abc, ref FlashReader input)
        : this(abc)
    {
        Parameters.Capacity = input.ReadEncodedInt();
        ReturnTypeIndex = input.ReadEncodedInt();

        for (int i = 0; i < Parameters.Capacity; i++)
        {
            var parameter = new ASParameter(this);
            parameter.TypeIndex = input.ReadEncodedInt();
            Parameters.Add(parameter);
        }

        NameIndex = input.ReadEncodedInt();
        Flags = (MethodFlags)input.ReadByte();

        if (Flags.HasFlag(MethodFlags.HasOptional))
        {
            int optionalParamCount = input.ReadEncodedInt();
            for (int i = Parameters.Count - optionalParamCount;
                optionalParamCount > 0;
                i++, optionalParamCount--)
            {
                ASParameter parameter = Parameters[i];
                parameter.IsOptional = true;
                parameter.ValueIndex = input.ReadEncodedInt();
                parameter.ValueKind = (ConstantKind)input.ReadByte();
            }
        }

        if (Flags.HasFlag(MethodFlags.HasParamNames))
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                ASParameter parameter = Parameters[i];
                parameter.NameIndex = input.ReadEncodedInt();
            }
        }
    }

    public string ToAS3()
    {
        var builder = new StringBuilder();

        ASMultiname qName = Trait?.QName ?? Container?.QName;
        if (qName != null)
        {
            if(Trait.Attributes.HasFlag(TraitAttributes.Override))
            {
                builder.Append("override ");
            }
            builder.Append(qName.Namespace.GetAS3Modifiers());
            if (builder.Length > 0)
            {
                builder.Append(' ');
            }
            if (!IsConstructor && Trait.IsStatic)
            {
                builder.Append("static ");
            }
            builder.Append("function ");    
            if(Trait.Kind == TraitKind.Getter)
            {
                builder.Append("get ");
            }
            if(Trait.Kind == TraitKind.Setter)
            {
                builder.Append("set ");
            }
            builder.Append(qName.Name);
        }
        else if (IsAnonymous) builder.Append("function");

        builder.Append('('); // Parameters Start
        if (Parameters.Count > 0)
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                Parameters[i].Append(builder, i + 1);
                builder.Append(", ");
            }
            builder.Length -= 2;
        }

        if (Flags.HasFlag(MethodFlags.NeedRest))
        {
            builder.Append("... param");
            builder.Append(Parameters.Count + 1);
        }
        builder.Append(')'); // Parameters End
        if (ReturnType != null)
        {
            builder.Append(':');
            if(ReturnType.Kind == MultinameKind.TypeName)
            {
                builder.Append(ReturnType.QName.Name);
                builder.Append(".<");
                builder.Append(string.Join(',', ReturnType.TypeIndices.Select(i => ABC.Pool.Multinames[i].Name)));
                builder.Append('>');
            } else builder.Append(ReturnType.Name);
        }
        return builder.ToString();
    }
    public override string ToString() => ToAS3();

    public int GetSize()
    {
        int size = 0;
        size += FlashWriter.GetEncodedIntSize(Parameters.Count);
        size += FlashWriter.GetEncodedIntSize(ReturnTypeIndex);
        return size;
    }
    public void WriteTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(Parameters.Count);
        output.WriteEncodedInt(ReturnTypeIndex);

        int optionalParamCount = 0;
        int optionalParamStartIndex = Parameters.Count - 1;
        if (Parameters.Count > 0)
        {
            // This flag will be removed if at least a single parameter has no name assigned.
            Flags |= MethodFlags.HasParamNames;
            for (int i = 0; i < Parameters.Count; i++)
            {
                ASParameter parameter = Parameters[i];
                output.WriteEncodedInt(parameter.TypeIndex);

                // This flag should only be present when all parameters are assigned a Name.
                if (string.IsNullOrWhiteSpace(parameter.Name))
                {
                    Flags &= ~MethodFlags.HasParamNames;
                }

                // Just one optional parameter is enough to attain this flag.
                if (parameter.IsOptional)
                {
                    if (i < optionalParamStartIndex)
                    {
                        optionalParamStartIndex = i;
                    }
                    optionalParamCount++;
                    Flags |= MethodFlags.HasOptional;
                }
            }
        }

        output.WriteEncodedInt(NameIndex);
        output.Write((byte)Flags);
        if (Flags.HasFlag(MethodFlags.HasOptional))
        {
            output.WriteEncodedInt(optionalParamCount);
            for (int i = optionalParamStartIndex; i < Parameters.Count; i++)
            {
                ASParameter parameter = Parameters[i];
                output.WriteEncodedInt(parameter.ValueIndex);
                output.Write((byte)parameter.ValueKind);
            }
        }

        if (Flags.HasFlag(MethodFlags.HasParamNames))
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                output.WriteEncodedInt(Parameters[i].NameIndex);
            }
        }
    }
}