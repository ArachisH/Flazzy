namespace Flazzy.ABC.AVM2.Instructions
{
    public class ReturnValueIns : ASInstruction
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