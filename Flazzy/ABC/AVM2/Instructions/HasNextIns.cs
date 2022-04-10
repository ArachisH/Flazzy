namespace Flazzy.ABC.AVM2.Instructions;

public sealed class HasNextIns : ASInstruction
{
    public HasNextIns()
        : base(OPCode.HasNext)
    { }

    public override int GetPopCount() => 2;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object curIndex = machine.Values.Pop();
        object obj = machine.Values.Pop();
        machine.Values.Push(null);
    }
}
