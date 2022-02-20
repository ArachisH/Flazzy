namespace Flazzy.ABC.AVM2.Instructions
{
    public class GetLocal3Ins : Local
    {
        public override int Register => 3;

        public GetLocal3Ins()
            : base(OPCode.GetLocal_3)
        { }
    }
}