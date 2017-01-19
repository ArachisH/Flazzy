using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class IfFalseIns : Jumper
    {
        public IfFalseIns()
            : base(OPCode.IfFalse)
        { }
        public IfFalseIns(FlashReader input)
            : base(OPCode.IfFalse, input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var value = (machine.Values.Pop() as bool?);
            if (value == null) return null;

            return (value == false);
        }
    }
}