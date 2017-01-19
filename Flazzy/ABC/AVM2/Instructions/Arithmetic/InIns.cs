namespace Flazzy.ABC.AVM2.Instructions
{
    public class InIns : ASInstruction
    {
        public InIns()
            : base(OPCode.In)
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
            object obj = machine.Values.Pop();
            object name = machine.Values.Pop();
            machine.Values.Push(null);
        }
    }
}