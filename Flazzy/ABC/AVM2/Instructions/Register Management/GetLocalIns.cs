using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class GetLocalIns : Local
{
    public GetLocalIns(int register)
        : base(OPCode.GetLocal, register)
    { }
    public GetLocalIns(FlashReader input)
        : base(OPCode.GetLocal, input)
    { }
}