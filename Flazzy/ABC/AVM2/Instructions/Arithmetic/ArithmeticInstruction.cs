namespace Flazzy.ABC.AVM2.Instructions
{
    public abstract class ArithmeticInstruction : ASInstruction
    {
        public ArithmeticInstruction(OPCode op)
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
    }
}
