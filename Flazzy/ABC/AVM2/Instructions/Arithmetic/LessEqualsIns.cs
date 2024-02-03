namespace Flazzy.ABC.AVM2.Instructions;

public sealed class LessEqualsIns : Computation
{
    public LessEqualsIns()
        : base(OPCode.LessEquals)
    { }

    protected override object Execute(dynamic left, dynamic right) => left <= right;
}