namespace Flazzy.ABC.AVM2.Instructions
{
    public class GetLocal0Ins : Local
    {
        public override int Register
        {
            get => 0;
            set => throw new NotSupportedException();
        }

        public GetLocal0Ins()
            : base(OPCode.GetLocal_0)
        { }
    }
}