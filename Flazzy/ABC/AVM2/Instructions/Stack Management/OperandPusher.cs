namespace Flazzy.ABC.AVM2.Instructions
{
    public abstract class OperandPusher : Instruction
    {
        public virtual object Value { get; set; }

        public OperandPusher(OPCode op)
            : base(op)
        { }
        public OperandPusher(OPCode op, ABCFile abc)
            : base(op, abc)
        { }

        public override int GetPopCount()
        {
            return 0;
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(Value);
        }
    }
}