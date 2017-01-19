namespace Flazzy.ABC.AVM2.Instructions
{
    public class CoerceAIns : ASInstruction
    {
        public CoerceAIns()
            : base(OPCode.Coerce_a)
        { }

        public override int GetPopCount()
        {
            return 1;
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            object value = machine.Values.Pop();
            machine.Values.Push(value);
        }
    }
}