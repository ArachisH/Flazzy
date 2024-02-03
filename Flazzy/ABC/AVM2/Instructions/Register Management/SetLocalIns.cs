using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class SetLocalIns : Local
{
    public SetLocalIns(int register)
        : base(OPCode.SetLocal, register)
    { }
    public SetLocalIns(ref SpanFlashReader input)
        : base(OPCode.SetLocal, ref input)
    { }
}