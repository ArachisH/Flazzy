namespace Flazzy.ABC.AVM2.Instructions
{
    public class AsTypeLateIns : ASInstruction
    {
        public AsTypeLateIns()
            : base(OPCode.AsTypeLate)
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
            object @class = machine.Values.Pop();
            object value = machine.Values.Pop();
            machine.Values.Push(null);
        }
    }
}