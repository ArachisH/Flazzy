namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class NewActivationIns : ASInstruction
    {
        public NewActivationIns()
            : base(OPCode.NewActivation)
        { }

        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(null);
        }
    }
}