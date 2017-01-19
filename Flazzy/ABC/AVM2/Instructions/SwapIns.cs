namespace Flazzy.ABC.AVM2.Instructions
{
    public class SwapIns : ASInstruction
    {
        public SwapIns()
            : base(OPCode.Swap)
        { }

        public override int GetPopCount()
        {
            return 2;
        }
        public override int GetPushCount()
        {
            return 2;
        }
        public override void Execute(ASMachine machine)
        {
            object value2 = machine.Values.Pop();
            object value1 = machine.Values.Pop();

            machine.Values.Push(value2);
            machine.Values.Push(value1);
        }
    }
}