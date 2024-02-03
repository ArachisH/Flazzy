namespace Flazzy.ABC.AVM2.Instructions;

public sealed class GetGlobalScopeIns : ASInstruction
{
    public GetGlobalScopeIns()
        : base(OPCode.GetGlobalScope)
    { }

    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        machine.Values.Push(null);
    }
}