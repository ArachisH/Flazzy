namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushNaNIns : Primitive
    {
        public override object Value
        {
            get => double.NaN;
            set => throw new NotSupportedException();
        }

        public PushNaNIns()
            : base(OPCode.PushNan)
        { }
    }
}