namespace Flazzy.ABC.AVM2.Instructions
{
    public class NextNameIns : ASInstruction
    {
        public NextNameIns()
            : base(OPCode.NextName)
        { }

        public override int GetPopCount() => 2;
        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            object index = machine.Values.Pop();
            object obj = machine.Values.Pop();
            machine.Values.Push(null);
        }
    }
}