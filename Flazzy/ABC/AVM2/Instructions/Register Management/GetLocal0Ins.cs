namespace Flazzy.ABC.AVM2.Instructions;

public sealed class GetLocal0Ins : Local
{
    public override int Register => 0;

    public GetLocal0Ins()
        : base(OPCode.GetLocal_0)
    { }
}