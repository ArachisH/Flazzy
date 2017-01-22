namespace Flazzy.ABC.AVM2.Instructions
{
    public class EqualsIns : Computation
    {
        public EqualsIns()
            : base(OPCode.Equals)
        { }

        protected override object Execute(dynamic left, dynamic right)
        {
            return (left == right);
        }
    }
}