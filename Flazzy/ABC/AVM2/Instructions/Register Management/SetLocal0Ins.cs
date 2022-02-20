namespace Flazzy.ABC.AVM2.Instructions
{
    public class SetLocal0Ins : Local
    {
        public override int Register => 0;

        public SetLocal0Ins()
            : base(OPCode.SetLocal_0)
        { }
    }
}