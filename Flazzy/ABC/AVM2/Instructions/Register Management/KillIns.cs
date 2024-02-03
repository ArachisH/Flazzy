using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class KillIns : Local
{
    public KillIns(int register)
        : base(OPCode.Kill, register)
    { }
    public KillIns(ref SpanFlashReader input)
        : base(OPCode.Kill, ref input)
    { }

    public override void Execute(ASMachine machine)
    {
        machine.Registers[Register] = null;
    }
}