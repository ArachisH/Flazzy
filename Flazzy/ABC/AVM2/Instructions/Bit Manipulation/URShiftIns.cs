namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class URShiftIns : ASInstruction
    {
        public URShiftIns()
            : base(OPCode.URShift)
        { }

        public override int GetPopCount() => 2;
        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            machine.Values.Pop();
            machine.Values.Pop();
            machine.Values.Push(null);
        }
    }
}