namespace Flazzy.ABC.AVM2.Instructions
{
    public class DxnsLateIns : Instruction
    {
        public DxnsLateIns()
            : base(OPCode.DxnsLate)
        { }

        public override int GetPopCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            object value = machine.Values.Pop();
        }
    }
}