namespace Flazzy.ABC.AVM2.Instructions;

public sealed class DxnsLateIns : ASInstruction
{
    public DxnsLateIns()
        : base(OPCode.DxnsLate)
    { }

    public override int GetPopCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object value = machine.Values.Pop();
    }
}
