namespace Flazzy.ABC.AVM2.Instructions
{
    public class AddIns : Computation
    {
        public AddIns()
            : base(OPCode.Add)
        { }

        protected override object Execute(dynamic left, dynamic right)
        {
            return (left + right);
        }
    }
}