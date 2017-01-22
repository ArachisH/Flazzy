namespace Flazzy.ABC.AVM2.Instructions
{
    public class MultiplyIns : Computation
    {
        public MultiplyIns()
            : base(OPCode.Multiply)
        { }

        protected override object Execute(dynamic left, dynamic right)
        {
            return (left * right);
        }
    }
}