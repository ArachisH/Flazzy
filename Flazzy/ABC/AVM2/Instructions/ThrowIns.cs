namespace Flazzy.ABC.AVM2.Instructions;

public sealed class ThrowIns : ASInstruction
{
    public ThrowIns()
        : base(OPCode.Throw)
    { }

    public override int GetPopCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object value = machine.Values.Pop();
    }
}