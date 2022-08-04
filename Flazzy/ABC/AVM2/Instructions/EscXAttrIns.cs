namespace Flazzy.ABC.AVM2.Instructions
{
    public class EscXAttrIns : ASInstruction
    {
        public EscXAttrIns()
            : base(OPCode.Esc_XAttr)
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
                result = Convert.ToString(value);
            }
            machine.Values.Push(result);
        }
    }
}