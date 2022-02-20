using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class IfFalseIns : Jumper
    {
        public IfFalseIns()
            : base(OPCode.IfFalse)
        { }
        public IfFalseIns(ref FlashReader input)
            : base(OPCode.IfFalse, ref input)
        { }

        public override bool? RunCondition(ASMachine machine)
        {
            var value = machine.Values.Pop() as bool?;
            if (value == null) return null;

            return value == false;
        }
    }
}