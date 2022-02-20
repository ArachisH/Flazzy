namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class PushNullIns : Primitive
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