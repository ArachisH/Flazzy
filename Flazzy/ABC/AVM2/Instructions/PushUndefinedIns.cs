namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushUndefinedIns : Instruction
    {
        public PushUndefinedIns()
            : base(OPCode.PushUndefined)
        { }

        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(null);
        }
    }
}