namespace Flazzy.ABC.AVM2.Instructions
{
    public class RShiftIns : ASInstruction
    {
        public RShiftIns()
            : base(OPCode.RShift)
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
                int iLeft = Convert.ToInt32(left);
                int iRight = (Convert.ToInt32(right) & 0x1F);
                result = (iLeft >> iRight);
            }
            machine.Values.Push(result);
        }
    }
}