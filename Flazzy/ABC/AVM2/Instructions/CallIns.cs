using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class CallIns : ASInstruction
{
    public int ArgCount { get; set; }

    public CallIns()
        : base(OPCode.Call)
    { }
    public CallIns(int argCount)
        : this()
    {
        ArgCount = argCount;
    }
    public CallIns(ref FlashReader input)
        : this()
    {
        ArgCount = input.ReadEncodedInt();
    }

    public override int GetPopCount() => ArgCount + 2;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        for (int i = 0; i < ArgCount; i++)
        {
            machine.Values.Pop();
        }
        object receiver = machine.Values.Pop();
        object function = machine.Values.Pop();
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        return FlashWriter.GetEncodedIntSize(ArgCount);
    }
    protected override void WriteValuesTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(ArgCount);
    }
}
