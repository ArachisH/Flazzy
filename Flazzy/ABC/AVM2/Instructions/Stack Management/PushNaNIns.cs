namespace Flazzy.ABC.AVM2.Instructions;

public sealed class PushNaNIns : Primitive
{
    public override object Value
    {
        get => double.NaN;
        set => throw new NotSupportedException();
    }

    public PushNaNIns()
        : base(OPCode.PushNan)
    { }
}
