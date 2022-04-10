namespace Flazzy.ABC.AVM2.Instructions;

public sealed class ConvertSIns : ASInstruction
{
    public ConvertSIns()
        : base(OPCode.Convert_s)
    { }

    public override int GetPopCount() => 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object result = null;
        object value = machine.Values.Pop();
        if (value != null)
        {
            result = Convert.ToString(value);
        }
        else result = "null";
        machine.Values.Push(result);
    }
}
