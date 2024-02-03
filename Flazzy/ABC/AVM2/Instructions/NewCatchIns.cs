using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class NewCatchIns : ASInstruction
{
    public int ExceptionIndex { get; set; }

    public NewCatchIns()
        : base(OPCode.NewCatch)
    { }
    public NewCatchIns(ref SpanFlashReader input)
        : this()
    {
        ExceptionIndex = input.ReadEncodedInt();
    }
    public NewCatchIns(int exceptionIndex)
        : this()
    {
        ExceptionIndex = exceptionIndex;
    }

    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(ExceptionIndex);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(ExceptionIndex);
    }
}