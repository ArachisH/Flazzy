using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class IfTrueIns : Jumper
{
    public IfTrueIns()
        : base(OPCode.IfTrue)
    { }
    public IfTrueIns(ref FlashReader input)
        : base(OPCode.IfTrue, ref input)
    { }

    public override bool? RunCondition(ASMachine machine)
    {
        return machine.Values.Pop() as bool?;
    }
}
