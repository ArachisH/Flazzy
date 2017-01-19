namespace Flazzy.ABC.AVM2.Instructions
{
    public class NewActivationIns : ASInstruction
    {
        public NewActivationIns()
            : base(OPCode.NewActivation)
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