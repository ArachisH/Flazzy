using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class GetDescendantsIns : ASInstruction
{
    public int DescendantIndex { get; set; }
    public ASMultiname Descendant => ABC.Pool.Multinames[DescendantIndex];

    public GetDescendantsIns(ABCFile abc)
        : base(OPCode.GetDescendants, abc)
    { }
    public GetDescendantsIns(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        DescendantIndex = input.ReadEncodedInt();
    }
    public GetDescendantsIns(ABCFile abc, int descendantIndex)
        : this(abc)
    {
        DescendantIndex = descendantIndex;
    }

    public override int GetPopCount()
    {
        return ResolveMultinamePops(Descendant) + 1;
    }
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        ResolveMultiname(machine, Descendant);
        object obj = machine.Values.Pop();
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(DescendantIndex);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(DescendantIndex);
    }
}