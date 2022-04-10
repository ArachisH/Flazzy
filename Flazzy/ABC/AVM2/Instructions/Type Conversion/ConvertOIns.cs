namespace Flazzy.ABC.AVM2.Instructions;

public sealed class ConvertOIns : ASInstruction
{
    public ConvertOIns()
        : base(OPCode.Convert_o)
    { }

    public override int GetPopCount() => 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object value = machine.Values.Pop();
        machine.Values.Push(value);
    }
}
