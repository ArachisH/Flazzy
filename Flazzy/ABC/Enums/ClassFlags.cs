using System;

namespace Flazzy.ABC
{
    [Flags]
    public enum ClassFlags
    {
        None = 0x00,
        Sealed = 0x01,
        Final = 0x02,
        Interface = 0x04,
        ProtectedNamespace = 0x08
    }
}