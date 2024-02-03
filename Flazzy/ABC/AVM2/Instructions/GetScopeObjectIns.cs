using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class GetScopeObjectIns : ASInstruction
{
    public byte ScopeIndex { get; set; }

    public GetScopeObjectIns()
        : base(OPCode.GetScopeObject)
    { }
    public GetScopeObjectIns(byte scopeIndex)
        : this()
    {
        ScopeIndex = scopeIndex;
    }
    public GetScopeObjectIns(ref SpanFlashReader input)
        : this()
    {
        ScopeIndex = input.ReadByte();
    }

    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(ScopeIndex);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.Write(ScopeIndex);
    }
}