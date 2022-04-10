namespace Flazzy.ABC.AVM2.Instructions;

public sealed class PushWithIns : ASInstruction
{
    public PushWithIns()
        : base(OPCode.PushWith)
    { }

    public override int GetPopCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object scopeObj = machine.Values.Pop();
        machine.Scopes.Push(scopeObj);
    }
}
