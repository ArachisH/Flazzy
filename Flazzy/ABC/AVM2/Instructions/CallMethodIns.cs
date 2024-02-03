using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class CallMethodIns : ASInstruction
{
    public int MethodIndex { get; set; }
    public int ArgCount { get; set; }

    public CallMethodIns(ABCFile abc)
        : base(OPCode.CallMethod, abc)
    { }
    public CallMethodIns(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        MethodIndex = input.ReadEncodedInt();
        ArgCount = input.ReadEncodedInt();
    }
    public CallMethodIns(ABCFile abc, int methodIndex)
        : this(abc)
    {
        MethodIndex = methodIndex;
    }
    public CallMethodIns(ABCFile abc, int methodIndex, int argCount)
        : this(abc)
    {
        MethodIndex = methodIndex;
        ArgCount = argCount;
    }

    public override int GetPopCount() => ArgCount + 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        for (int i = 0; i < ArgCount; i++)
        {
            machine.Values.Pop();
        }
        machine.Values.Pop(); // Receiver
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        int size = 0;
        size += SpanFlashWriter.GetEncodedIntSize(MethodIndex);
        size += SpanFlashWriter.GetEncodedIntSize(ArgCount);
        return size;
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(MethodIndex);
        output.WriteEncodedInt(ArgCount);
    }
}