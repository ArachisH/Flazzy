namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class SetLocal0Ins : Local
    {
        public override int Register => 0;

        public SetLocal0Ins()
            : base(OPCode.SetLocal_0)
        { }
    }
}