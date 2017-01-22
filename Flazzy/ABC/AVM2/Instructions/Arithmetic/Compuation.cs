namespace Flazzy.ABC.AVM2.Instructions
{
    public abstract class Computation : ASInstruction
    {
        public Computation(OPCode op)
            : base(op)
        { }

        public override int GetPopCount()
        {
            return 2;
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            object right = machine.Values.Pop();
            object left = machine.Values.Pop();

            object result = null;
            if (left != null && right != null)
            {
                result = Execute(left, right);
            }
            machine.Values.Push(result);
        }
        protected abstract object Execute(object left, object right);

        public static bool IsValid(OPCode op)
        {
            switch (op)
            {
                case OPCode.Add_i:
                case OPCode.Add:
                case OPCode.Divide:
                case OPCode.Equals:
                case OPCode.GreaterEquals:
                case OPCode.GreaterThan:
                case OPCode.LessEquals:
                case OPCode.LessThan:
                case OPCode.Modulo:
                case OPCode.Multiply_i:
                case OPCode.Multiply:
                case OPCode.StrictEquals:
                case OPCode.Subtract_i:
                case OPCode.Subtract:
                return true;

                default:
                return false;
            }
        }
    }
}
