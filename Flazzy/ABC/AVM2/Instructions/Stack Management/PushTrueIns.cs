namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushTrueIns : Primitive
    {
        public override object Value
        {
            get => true;
            set => throw new NotSupportedException();
        }

        public PushTrueIns()
            : base(OPCode.PushTrue)
        { }
    }
}