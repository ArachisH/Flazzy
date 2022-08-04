using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class DecLocalIns : Local
    {
        public DecLocalIns(int register)
            : base(OPCode.DecLocal, register)
        { }
        public DecLocalIns(FlashReader input)
            : base(OPCode.DecLocal, input)
        { }

        public override void Execute(ASMachine machine)
        {
            object value = machine.Registers[Register];
            if (value != null)
            {
                value = (Convert.ToDouble(value) - 1);
            }
            machine.Registers[Register] = value;
        }
    }
}