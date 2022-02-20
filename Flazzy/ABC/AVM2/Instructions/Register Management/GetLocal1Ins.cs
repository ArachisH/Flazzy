namespace Flazzy.ABC.AVM2.Instructions
{
    public class GetLocal1Ins : Local
    {
        public override int Register => 1;

        public GetLocal1Ins()
            : base(OPCode.GetLocal_1)
        { }
    }
}