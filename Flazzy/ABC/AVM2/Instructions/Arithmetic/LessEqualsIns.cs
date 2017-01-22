namespace Flazzy.ABC.AVM2.Instructions
{
    public class LessEqualsIns : Computation
    {
        public LessEqualsIns()
            : base(OPCode.LessEquals)
        { }

        protected override object Execute(dynamic left, dynamic right)
        {
            return (left <= right);
        }
    }
}