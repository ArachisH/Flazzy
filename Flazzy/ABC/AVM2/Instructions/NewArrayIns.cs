using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class NewArrayIns : ASInstruction
{
    public int ArgCount { get; set; }

    public NewArrayIns()
        : base(OPCode.NewArray)
    { }
    public NewArrayIns(int argCount)
        : this()
    {
        ArgCount = argCount;
    }
    public NewArrayIns(ref SpanFlashReader input)
        : this()
    {
        ArgCount = input.ReadEncodedInt();
    }

    public override int GetPopCount() => ArgCount;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        var newarray = new object[ArgCount];
        for (int i = ArgCount - 1; i >= 0; i--)
        {
            newarray[i] = machine.Values.Pop();
        }
        machine.Values.Push(newarray);
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