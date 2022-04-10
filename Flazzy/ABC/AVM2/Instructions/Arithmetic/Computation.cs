namespace Flazzy.ABC.AVM2.Instructions;

public abstract class Computation : ASInstruction
{
    public Computation(OPCode op)
        : base(op)
    { }

    public override int GetPopCount() => 2;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object right = machine.Values.Pop();
        object left = machine.Values.Pop();

        object result = null;
        if (left != null && right != null)
        {
            result = Execute(left, right);
        }
        machine.Values.Push(result);
    }
    protected abstract object Execute(object left, object right);

    public static bool IsValid(OPCode op) => op switch
    {
        OPCode.Add_i or
        OPCode.Add or
        OPCode.Divide or
        OPCode.Equals or
        OPCode.GreaterEquals or
        OPCode.GreaterThan or
        OPCode.LessEquals or
        OPCode.LessThan or
        OPCode.Modulo or
        OPCode.Multiply_i or
        OPCode.Multiply or
        OPCode.StrictEquals or
        OPCode.Subtract_i or
        OPCode.Subtract => true,
        _ => false,
    };
}
