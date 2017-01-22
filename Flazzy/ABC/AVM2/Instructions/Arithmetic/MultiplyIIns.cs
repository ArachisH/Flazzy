namespace Flazzy.ABC.AVM2.Instructions
{
    public class MultiplyIIns : Computation
    {
        public MultiplyIIns()
            : base(OPCode.Multiply_i)
        { }

        protected override object Execute(dynamic left, dynamic right)
        {
            return (left * right);
        }
    }
}