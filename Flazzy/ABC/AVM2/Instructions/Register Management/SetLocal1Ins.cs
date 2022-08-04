namespace Flazzy.ABC.AVM2.Instructions
{
    public class SetLocal1Ins : Local
    {
        public override int Register
        {
            get => 1;
            set => throw new NotSupportedException();
        }

        public SetLocal1Ins()
            : base(OPCode.SetLocal_1)
        { }
    }
}