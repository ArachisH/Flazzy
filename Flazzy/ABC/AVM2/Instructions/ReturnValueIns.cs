namespace Flazzy.ABC.AVM2.Instructions
{
    public class ReturnValueIns : Instruction
    {
        public ReturnValueIns()
            : base(OPCode.ReturnValue)
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