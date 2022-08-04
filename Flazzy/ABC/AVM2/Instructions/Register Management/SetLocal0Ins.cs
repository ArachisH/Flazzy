namespace Flazzy.ABC.AVM2.Instructions
{
    public class SetLocal0Ins : Local
    {
        public override int Register
        {
            get => 0;
            set => throw new NotSupportedException();
        }

        public SetLocal0Ins()
            : base(OPCode.SetLocal_0)
        { }
    }
}