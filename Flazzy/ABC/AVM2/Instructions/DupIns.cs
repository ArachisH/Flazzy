namespace Flazzy.ABC.AVM2.Instructions
{
    public class DupIns : Instruction
    {
        public DupIns()
            : base(OPCode.Dup)
        { }

        public override int GetPopCount()
        {
            return 1;
        }
        public override int GetPushCount()
        {
            return 2;
        }
        public override void Execute(ASMachine machine)
        {
            object value = machine.Values.Pop();
            machine.Values.Push(value);
            machine.Values.Push(value);
        }
    }
}