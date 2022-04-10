namespace Flazzy.ABC.AVM2.Instructions;

public sealed class StrictEqualsIns : Computation
{
    public StrictEqualsIns()
        : base(OPCode.StrictEquals)
    { }

    protected override object Execute(object left, object right)
    {
        if (left is not IComparable cLeft ||
            right is not IComparable cRight) return null;

        return cLeft.CompareTo(cRight) == 0;
    }
}
