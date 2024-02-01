namespace Flazzy.ABC.AVM2.Instructions;

public sealed class SubtractIIns : Computation
{
    public SubtractIIns()
        : base(OPCode.Subtract_i)
    { }

    protected override object Execute(dynamic left, dynamic right)
    {
        return (left - right);
    }
}