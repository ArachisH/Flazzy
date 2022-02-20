namespace Flazzy.ABC.AVM2.Instructions
{
    public class SetLocal2Ins : Local
    {
        public override int Register => 2;

        public SetLocal2Ins()
            : base(OPCode.SetLocal_2)
        { }
    }
}