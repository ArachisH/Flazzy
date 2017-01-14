namespace Flazzy.ABC.AVM2.Instructions
{
    public class ThrowIns : Instruction
    {
        public ThrowIns()
            : base(OPCode.Throw)
        { }

        public override int GetPopCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            object value = machine.Values.Pop();
        }
    }
}