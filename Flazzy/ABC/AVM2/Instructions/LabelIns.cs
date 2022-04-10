namespace Flazzy.ABC.AVM2.Instructions;

public sealed class LabelIns : ASInstruction
{
    public LabelIns()
        : base(OPCode.Label)
    { }
}
