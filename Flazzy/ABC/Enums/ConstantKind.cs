namespace Flazzy.ABC
{
    public enum ConstantKind
    {
        Null = 0x0C,
        Undefined = 0x00,

        String = 0x01,
        Double = 0x06,
        Integer = 0x03,
        UInteger = 0x04,

        True = 0x0B,
        False = 0x0A,

        Namespace = 0x08,
        Multiname = 0x09
    }
}