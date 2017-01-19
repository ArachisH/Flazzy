namespace Flazzy.ABC.AVM2.Instructions
{
    public class URShiftIns : ASInstruction
    {
        public URShiftIns()
            : base(OPCode.URShift)
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
            machine.Values.Pop();
            machine.Values.Pop();
            machine.Values.Push(null);
        }
    }
}