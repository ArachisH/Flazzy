namespace Flazzy.ABC.AVM2.Instructions
{
    public class HasNextIns : ASInstruction
    {
        public HasNextIns()
            : base(OPCode.HasNext)
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
            object curIndex = machine.Values.Pop();
            object obj = machine.Values.Pop();
            machine.Values.Push(null);
        }
    }
}