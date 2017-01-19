namespace Flazzy.ABC.AVM2.Instructions
{
    public class DupIns : ASInstruction
    {
        public DupIns()
            : base(OPCode.Dup)
        { }

        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            object value = machine.Values.Peek();
            machine.Values.Push(value);
        }
    }
}