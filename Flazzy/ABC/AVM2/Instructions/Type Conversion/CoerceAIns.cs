namespace Flazzy.ABC.AVM2.Instructions;

public sealed class CoerceAIns : ASInstruction
{
    public CoerceAIns()
        : base(OPCode.Coerce_a)
    { }

    public override int GetPopCount() => 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object value = machine.Values.Pop();
        machine.Values.Push(value);
    }
}