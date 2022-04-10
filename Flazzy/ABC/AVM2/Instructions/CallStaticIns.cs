using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class CallStaticIns : ASInstruction
{
    public int MethodIndex { get; set; }
    public ASMethod Method => ABC.Methods[MethodIndex];

    public int ArgCount { get; set; }

    public CallStaticIns(ABCFile abc)
        : base(OPCode.CallStatic, abc)
    { }
    public CallStaticIns(ABCFile abc, ref FlashReader input)
        : this(abc)
    {
        MethodIndex = input.ReadEncodedInt();
        ArgCount = input.ReadEncodedInt();
    }
    public CallStaticIns(ABCFile abc, int methodIndex)
        : this(abc)
    {
        MethodIndex = methodIndex;
    }
    public CallStaticIns(ABCFile abc, int methodIndex, int argCount)
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
        size += FlashWriter.GetEncodedIntSize(MethodIndex);
        size += FlashWriter.GetEncodedIntSize(ArgCount);
        return size;
    }
    protected override void WriteValuesTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(MethodIndex);
        output.WriteEncodedInt(ArgCount);
    }
}
