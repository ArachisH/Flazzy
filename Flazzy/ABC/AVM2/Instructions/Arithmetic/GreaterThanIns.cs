namespace Flazzy.ABC.AVM2.Instructions;

public sealed class GreaterThanIns : Computation
{
    public GreaterThanIns()
        : base(OPCode.GreaterThan)
    { }

    protected override object Execute(dynamic left, dynamic right) => left > right;
}
