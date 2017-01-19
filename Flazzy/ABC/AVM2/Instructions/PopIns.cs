namespace Flazzy.ABC.AVM2.Instructions
{
    public class PopIns : ASInstruction
    {
        public PopIns()
            : base(OPCode.Pop)
        { }

        public override int GetPopCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            machine.Values.Pop();
        }
    }
}