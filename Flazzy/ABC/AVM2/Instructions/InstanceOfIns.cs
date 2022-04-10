namespace Flazzy.ABC.AVM2.Instructions;

public sealed class InstanceOfIns : ASInstruction
{
    public InstanceOfIns()
        : base(OPCode.InstanceOf)
    { }

    public override int GetPopCount() => 2;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object type = machine.Values.Pop();
        object value = machine.Values.Pop();
        machine.Values.Push(null);
    }
}
