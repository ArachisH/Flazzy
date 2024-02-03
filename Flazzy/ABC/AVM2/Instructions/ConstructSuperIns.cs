using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class ConstructSuperIns : ASInstruction
{
    public int ArgCount { get; set; }

    public ConstructSuperIns()
        : base(OPCode.ConstructSuper)
    { }
    public ConstructSuperIns(int argCount)
        : this()
    {
        ArgCount = argCount;
    }
    public ConstructSuperIns(ref SpanFlashReader input)
        : this()
    {
        ArgCount = input.ReadEncodedInt();
    }

    public override int GetPopCount() => ArgCount + 1;
    public override void Execute(ASMachine machine)
    {
        for (int i = 0; i < ArgCount; i++)
        {
            machine.Values.Pop();
        }
        object obj = machine.Values.Pop();
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(ArgCount);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(ArgCount);
    }
}