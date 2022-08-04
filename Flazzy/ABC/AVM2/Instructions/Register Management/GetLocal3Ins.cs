namespace Flazzy.ABC.AVM2.Instructions
{
    public class GetLocal3Ins : Local
    {
        public override int Register
        {
            get => 3;
            set => throw new NotSupportedException();
        }

        public GetLocal3Ins()
            : base(OPCode.GetLocal_3)
        { }
    }
}