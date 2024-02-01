namespace Flazzy.ABC.AVM2.Instructions;

public sealed class AddIIns : Computation
{
    public AddIIns()
        : base(OPCode.Add_i)
    { }

    protected override object Execute(dynamic left, dynamic right)
    {
        return (left + right);
    }
}