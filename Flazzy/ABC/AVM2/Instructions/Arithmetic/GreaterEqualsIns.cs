namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class GreaterEqualsIns : Computation
    {
        public GreaterEqualsIns()
            : base(OPCode.GreaterEquals)
        { }

        protected override object Execute(dynamic left, dynamic right) => left >= right;
    }
}