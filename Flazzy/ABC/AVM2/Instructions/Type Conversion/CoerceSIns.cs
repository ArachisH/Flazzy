namespace Flazzy.ABC.AVM2.Instructions
{
    public class CoerceSIns : ASInstruction
    {
        public CoerceSIns()
            : base(OPCode.Coerce_s)
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
            string result = null;
            object value = machine.Values.Pop();
            if (value != null)
            {
                result = Convert.ToString(value);
            }
            machine.Values.Push(result);
        }
    }
}