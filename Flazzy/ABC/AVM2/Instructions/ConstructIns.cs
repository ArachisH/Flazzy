using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class ConstructIns : ASInstruction
{
    public int ArgCount { get; set; }

    public ConstructIns()
        : base(OPCode.Construct)
    { }
    public ConstructIns(int argCount)
        : this()
    {
        ArgCount = argCount;
    }
    public ConstructIns(ref SpanFlashReader input)
        : this()
    {
        ArgCount = input.ReadEncodedInt();
    }

    public override int GetPopCount() => ArgCount + 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        for (int i = 0; i < ArgCount; i++)
        {
            machine.Values.Pop();
        }
        object obj = machine.Values.Pop();
        machine.Values.Push(null);
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