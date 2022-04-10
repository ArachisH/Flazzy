namespace Flazzy.ABC.AVM2.Instructions;

public sealed class ReturnValueIns : ASInstruction
{
    public ReturnValueIns()
        : base(OPCode.ReturnValue)
    { }

    public override int GetPopCount() => 1;
    public override void Execute(ASMachine machine)
    {
        machine.Values.Pop();
    }
}
