namespace Flazzy.ABC.AVM2.Instructions
{
    public class IsTypeLateIns : ASInstruction
    {
        public IsTypeLateIns()
            : base(OPCode.IsTypeLate)
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
            object type = machine.Values.Pop();
            object value = machine.Values.Pop();
            machine.Values.Push(null);
        }
    }
}