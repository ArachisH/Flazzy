namespace Flazzy.ABC.AVM2.Instructions;

public sealed class PopIns : ASInstruction
{
    public PopIns()
        : base(OPCode.Pop)
    { }

    public override int GetPopCount() => 1;
    public override void Execute(ASMachine machine)
    {
        machine.Values.Pop();
    }
}
