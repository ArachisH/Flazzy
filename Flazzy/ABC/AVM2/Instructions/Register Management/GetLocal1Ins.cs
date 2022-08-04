namespace Flazzy.ABC.AVM2.Instructions
{
    public class GetLocal1Ins : Local
    {
        public override int Register
        {
            get => 1;
            set => throw new NotSupportedException();
        }

        public GetLocal1Ins()
            : base(OPCode.GetLocal_1)
        { }
    }
}