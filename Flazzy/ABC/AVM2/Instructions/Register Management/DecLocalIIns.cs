using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class DecLocalIIns : Local
    {
        public DecLocalIIns(int register)
            : base(OPCode.DecLocal_i, register)
        { }
        public DecLocalIIns(FlashReader input)
            : base(OPCode.DecLocal_i, input)
        { }

        public override void Execute(ASMachine machine)
        {
            object value = machine.Registers[Register];
            if (value != null)
            {
                value = (Convert.ToInt32(value) - 1);
            }
            machine.Registers[Register] = value;
        }
    }
}