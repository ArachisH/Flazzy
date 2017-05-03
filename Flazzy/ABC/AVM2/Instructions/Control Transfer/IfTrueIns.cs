using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfTrueIns : Jumper
    {
        public IfTrueIns()
            : base(OPCode.IfTrue)
        { }
        public IfTrueIns(FlashReader input)
            : base(OPCode.IfTrue, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            return (machine.Values.Pop() as bool?);
        }
    }
}