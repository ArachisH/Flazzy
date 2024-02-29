using Flazzy.ABC.AVM2;
using Flazzy.IO;

namespace Flazzy.ABC;

public class ASMethodBody : ASContainer
{
    public byte[] Code { get; set; }
    public int MaxStack { get; set; }
    public int LocalCount { get; set; }
    public int MaxScopeDepth { get; set; }
    public int InitialScopeDepth { get; set; }

    public int MethodIndex { get; set; }
    public ASMethod Method => ABC.Methods[MethodIndex];

    public List<ASException> Exceptions { get; }

    public override ASMultiname QName => Method.Trait?.QName;

    public ASMethodBody(ABCFile abc)
        : base(abc)
    {
        Exceptions = new List<ASException>();
    }
    public ASMethodBody(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        MethodIndex = input.ReadEncodedInt();
        Method.Body = this;

        MaxStack = input.ReadEncodedInt();
        LocalCount = input.ReadEncodedInt();
        InitialScopeDepth = input.ReadEncodedInt();
        MaxScopeDepth = input.ReadEncodedInt();

        int codeLength = input.ReadEncodedInt();
        Code = input.ReadBytes(codeLength).ToArray();

        Exceptions.Capacity = input.ReadEncodedInt();
        for (int i = 0; i < Exceptions.Capacity; i++)
        {
            Exceptions.Add(new ASException(abc, ref input));
        }
        PopulateTraits(ref input);
    }

    public ASCode ParseCode() => new(ABC, this);

    public override int GetSize()
    {
        int size = 0;
        size += SpanFlashWriter.GetEncodedIntSize(MethodIndex);
        size += SpanFlashWriter.GetEncodedIntSize(MaxStack);
        size += SpanFlashWriter.GetEncodedIntSize(LocalCount);
        size += SpanFlashWriter.GetEncodedIntSize(InitialScopeDepth);
        size += SpanFlashWriter.GetEncodedIntSize(MaxScopeDepth);

        size += SpanFlashWriter.GetEncodedIntSize(Code.Length);
        size += Code.Length;

        size += SpanFlashWriter.GetEncodedIntSize(Exceptions.Count);
        for (int i = 0; i < Exceptions.Count; i++)
        {
            size += Exceptions[i].GetSize();
        }

        size += base.GetSize();
        return size;
    }
    public override void WriteTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(MethodIndex);
        output.WriteEncodedInt(MaxStack);
        output.WriteEncodedInt(LocalCount);
        output.WriteEncodedInt(InitialScopeDepth);
        output.WriteEncodedInt(MaxScopeDepth);

        output.WriteEncodedInt(Code.Length);
        output.Write(Code);

        output.WriteEncodedInt(Exceptions.Count);
        for (int i = 0; i < Exceptions.Count; i++)
        {
            Exceptions[i].WriteTo(ref output);
        }
        base.WriteTo(ref output);
    }

    public override string ToString() => $"LocalCount: {LocalCount:n0}, MaxStack: {MaxStack:n0}";
}