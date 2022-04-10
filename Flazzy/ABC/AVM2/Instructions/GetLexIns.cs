using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class GetLexIns : ASInstruction
{
    public int TypeNameIndex { get; set; }
    public ASMultiname TypeName => ABC.Pool.Multinames[TypeNameIndex];

    public GetLexIns(ABCFile abc)
        : base(OPCode.GetLex, abc)
    { }
    public GetLexIns(ABCFile abc, ref FlashReader input)
        : this(abc)
    {
        TypeNameIndex = input.ReadEncodedInt();
    }
    public GetLexIns(ABCFile abc, int typeNameIndex)
        : this(abc)
    {
        TypeNameIndex = typeNameIndex;
    }

    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        return FlashWriter.GetEncodedIntSize(TypeNameIndex);
    }
    protected override void WriteValuesTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(TypeNameIndex);
    }
}
