namespace Flazzy.ABC.AVM2.Instructions
{
    public class BitAndIns : ASInstruction
    {
        public BitAndIns()
            : base(OPCode.BitAnd)
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
            object result = null;
            object right = machine.Values.Pop();
            object left = machine.Values.Pop();
            if (right != null && left != null)
            {
                var iLeft = (int)Convert.ToDouble(left);
                var iRight = (int)Convert.ToDouble(right);
                result = (iLeft & iRight);
            }
            machine.Values.Push(result);
        }
    }
}