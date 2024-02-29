namespace Flazzy.ABC;

[Flags]
public enum TraitAttributes : byte
{
    None = 0x00,
    Final = 0x01,
    Override = 0x02,
    Metadata = 0x04
}