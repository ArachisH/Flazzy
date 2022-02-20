namespace Flazzy.ABC.AVM2.Instructions
{
    public class SetLocal3Ins : Local
    {
        public override int Register => 3;

        public SetLocal3Ins()
            : base(OPCode.SetLocal_3)
        { }
    }
}