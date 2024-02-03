namespace Flazzy.ABC.AVM2.Instructions;

public sealed class NotIns : ASInstruction
{
    public NotIns()
        : base(OPCode.Not)
    { }

    public override int GetPopCount() => 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object value = machine.Values.Pop();
        if (value != null)
        {
            if (value is string @string)
            {
                value = !string.IsNullOrEmpty(@string);
            }
            else value = !Convert.ToBoolean(value);
        }
        machine.Values.Push(value);
    }
}