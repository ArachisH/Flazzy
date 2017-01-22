namespace Flazzy.ABC.AVM2.Instructions
{
    public class SubtractIns : Computation
    {
        public SubtractIns()
            : base(OPCode.Subtract)
        { }

        protected override object Execute(dynamic left, dynamic right)
        {
            return (left - right);
        }
    }
}