namespace Flazzy.ABC.AVM2.Instructions;

public sealed class GetLocal2Ins : Local
{
    public override int Register => 2;

    public GetLocal2Ins()
        : base(OPCode.GetLocal_2)
    { }
}