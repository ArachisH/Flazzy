namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class EscXElemIns : ASInstruction
    {
        public EscXElemIns()
            : base(OPCode.Esc_XElem)
        { }

        public override int GetPopCount() => 1;
        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            object value = machine.Values.Pop();
            machine.Values.Push(value.ToString());
        }
    }
}