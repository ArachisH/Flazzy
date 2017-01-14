using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class JumpIns : Jumper
    {
        public JumpIns()
            : base(OPCode.Jump)
        { }
        public JumpIns(FlashReader input)
            : base(OPCode.Jump, input)
        { }
    }
}