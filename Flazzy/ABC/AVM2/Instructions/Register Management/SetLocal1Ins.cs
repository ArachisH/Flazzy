namespace Flazzy.ABC.AVM2.Instructions
{
    public class SetLocal1Ins : Local
    {
        public override int Register => 1;

        public SetLocal1Ins()
            : base(OPCode.SetLocal_1)
        { }
    }
}