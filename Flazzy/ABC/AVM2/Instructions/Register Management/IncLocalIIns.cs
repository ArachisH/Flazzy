using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IncLocalIIns : Local
    {
        public IncLocalIIns(int register)
            : base(OPCode.IncLocal_i, register)
        { }
        public IncLocalIIns(FlashReader input)
            : base(OPCode.IncLocal_i, input)
        { }

        public override void Execute(ASMachine machine)
        {
            object value = machine.Registers[Register];
            if (value != null)
            {
                value = (Convert.ToInt32(value) + 1);
            }
            machine.Registers[Register] = value;
        }
    }
}