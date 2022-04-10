namespace Flazzy.ABC.AVM2.Instructions;

public sealed class SubtractIns : Computation
{
    public SubtractIns()
        : base(OPCode.Subtract)
    { }

    protected override object Execute(dynamic left, dynamic right) => left - right;
}
