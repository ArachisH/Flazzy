namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushNullIns : Primitive
    {
        public override object Value
        {
            get => null;
            set => throw new NotSupportedException();
        }

        public PushNullIns()
            : base(OPCode.PushNull)
        { }
    }
}