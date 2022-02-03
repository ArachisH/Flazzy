using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IncLocalIns : Local
    {
        public IncLocalIns(int register)
            : base(OPCode.IncLocal, register)
        { }
        public IncLocalIns(ref FlashReader input)
            : base(OPCode.IncLocal, ref input)
        { }

        public override void Execute(ASMachine machine)
        {
            object value = machine.Registers[Register];
            if (value != null)
            {
                value = (Convert.ToDouble(value) + 1);
            }
            machine.Registers[Register] = value;
        }
    }
}