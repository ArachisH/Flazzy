using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class KillIns : Local
    {
        public KillIns(int register)
            : base(OPCode.Kill, register)
        { }
        public KillIns(FlashReader input)
            : base(OPCode.Kill, input)
        { }

        public override void Execute(ASMachine machine)
        {
            machine.Registers[Register] = null;
        }
    }
}