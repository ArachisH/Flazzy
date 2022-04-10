namespace Flazzy.Tags;

[Flags]
public enum FileAttributes : byte
{
    None = 0,
    UseNetwork = 1 << 0,

    NoCrossDomainCache = 1 << 2,
    ActionScript3 = 1 << 3,
    HasMetadata = 1 << 4,
    UseGPU = 1 << 5,
    UseDirectBlit = 1 << 6
}
