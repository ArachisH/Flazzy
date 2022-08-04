namespace Flazzy.ABC.AVM2.Instructions
{
    public class BitNotIns : ASInstruction
    {
        public BitNotIns()
            : base(OPCode.BitNot)
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
            object result = null;
            object value = machine.Values.Pop();
            if (value != null)
            {
                result = (~Convert.ToInt32(value));
            }
            machine.Values.Push(result);
        }
    }
}