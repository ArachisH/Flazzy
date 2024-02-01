namespace Flazzy.ABC.AVM2.Instructions;

public sealed class NopIns : ASInstruction
{
    public NopIns()
        : base(OPCode.Nop)
    { }
}