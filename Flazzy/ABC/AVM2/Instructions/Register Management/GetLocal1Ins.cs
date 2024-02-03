namespace Flazzy.ABC.AVM2.Instructions;

public sealed class GetLocal1Ins : Local
{
    public override int Register => 1;

    public GetLocal1Ins()
        : base(OPCode.GetLocal_1)
    { }
}