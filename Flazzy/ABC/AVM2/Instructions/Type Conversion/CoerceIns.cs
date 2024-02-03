using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class CoerceIns : ASInstruction
{
    public int TypeNameIndex { get; set; }
    public ASMultiname TypeName => ABC.Pool.Multinames[TypeNameIndex];

    public CoerceIns(ABCFile abc)
        : base(OPCode.Coerce, abc)
    { }
    public CoerceIns(ABCFile abc, int typeNameIndex)
        : this(abc)
    {
        TypeNameIndex = typeNameIndex;
    }
    public CoerceIns(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        TypeNameIndex = input.ReadEncodedInt();
    }

    public override int GetPopCount() => 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object value = machine.Values.Pop();
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(TypeNameIndex);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(TypeNameIndex);
    }
}