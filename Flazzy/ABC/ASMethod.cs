using Flazzy.IO;

using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Flazzy.ABC;

/// <summary>
/// Represents a signature of a single ActionScript method.
/// </summary>
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
            Parameters.Add(new ASParameter(this)
            {
                TypeIndex = input.ReadEncodedInt()
            });
        }

        NameIndex = input.ReadEncodedInt();
        Flags = (MethodFlags)input.ReadByte();

        if (Flags.HasFlag(MethodFlags.HasOptional))
        {
            int optionalParamCount = input.ReadEncodedInt();
            foreach (var parameter in 
                CollectionsMarshal.AsSpan(Parameters).Slice(Parameters.Count - optionalParamCount))
            {
                parameter.IsOptional = true;
                parameter.ValueIndex = input.ReadEncodedInt();
                parameter.ValueKind = (ConstantKind)input.ReadByte();
            }
        }

        if (Flags.HasFlag(MethodFlags.HasParamNames))
        {
            foreach (var parameter in Parameters)
            {
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
            if (Trait.Attributes.HasFlag(TraitAttributes.Override))
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
            if (Trait.Kind == TraitKind.Getter)
            {
                builder.Append("get ");
            }
            if (Trait.Kind == TraitKind.Setter)
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
            if (ReturnType.Kind == MultinameKind.TypeName)
            {
                builder.Append(ReturnType.QName.Name);
                builder.Append(".<");
                builder.Append(string.Join(',', ReturnType.TypeIndices.Select(i => ABC.Pool.Multinames[i].Name)));
                builder.Append('>');
            }
            else builder.Append(ReturnType.Name);
        }
        return builder.ToString();
    }
    public override string ToString() => ToAS3();

    public int GetSize()
    {
        int size = 0;
        size += FlashWriter.GetEncodedIntSize(Parameters.Count);
        size += FlashWriter.GetEncodedIntSize(ReturnTypeIndex);

        int optionalParamCount = 0;
        if (Parameters.Count > 0)
        {
            foreach (var parameter in Parameters)
            {
                size += FlashWriter.GetEncodedIntSize(parameter.TypeIndex);

                // One named parameter is enough to attain this flag.
                if (!string.IsNullOrWhiteSpace(parameter.Name))
                {
                    Flags |= MethodFlags.HasParamNames;
                }

                // Just one optional parameter is enough to attain this flag.
                if (parameter.IsOptional)
                {
                    optionalParamCount++;
                    Flags |= MethodFlags.HasOptional;
                }
            }
        }

        size += FlashWriter.GetEncodedIntSize(NameIndex);
        size += sizeof(byte);
        if (Flags.HasFlag(MethodFlags.HasOptional))
        {
            size += FlashWriter.GetEncodedIntSize(optionalParamCount);
            foreach (var parameter in CollectionsMarshal.AsSpan(Parameters)
                .Slice(Parameters.Count - optionalParamCount))
            {
                size += FlashWriter.GetEncodedIntSize(parameter.ValueIndex);
            }
            size += optionalParamCount * sizeof(byte);
        }

        if (Flags.HasFlag(MethodFlags.HasParamNames))
        {
            foreach (var parameter in Parameters)
            {
                size += FlashWriter.GetEncodedIntSize(parameter.NameIndex);
            }
        }
        return size;
    }
    public void WriteTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(Parameters.Count);
        output.WriteEncodedInt(ReturnTypeIndex);

        // TODO: This logic is pretty fragile.
        // I think we should make Flags a getter-only and manage/validate the state some other way.
        int optionalParamCount = 0;
        if (Parameters.Count > 0)
        {
            foreach (var parameter in Parameters)
            {
                output.WriteEncodedInt(parameter.TypeIndex);
                
                // One named parameter is enough to attain this flag.
                if (!string.IsNullOrWhiteSpace(parameter.Name))
                {
                    Flags |= MethodFlags.HasParamNames;
                }

                // Just one optional parameter is enough to attain this flag.
                if (parameter.IsOptional)
                {
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
            foreach (var parameter in CollectionsMarshal.AsSpan(Parameters)
                .Slice(Parameters.Count - optionalParamCount))
            {
                output.WriteEncodedInt(parameter.ValueIndex);
                output.Write((byte)parameter.ValueKind);
            }
        }

        if (Flags.HasFlag(MethodFlags.HasParamNames))
        {
            foreach (var parameter in Parameters)
            {
                output.WriteEncodedInt(parameter.NameIndex);
            }
        }
    }
}