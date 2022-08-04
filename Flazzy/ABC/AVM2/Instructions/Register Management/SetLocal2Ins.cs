namespace Flazzy.ABC.AVM2.Instructions
{
    public class SetLocal2Ins : Local
    {
        public override int Register
        {
            get => 2;
            set => throw new NotSupportedException();
        }

        public SetLocal2Ins()
            : base(OPCode.SetLocal_2)
        { }
    }
}