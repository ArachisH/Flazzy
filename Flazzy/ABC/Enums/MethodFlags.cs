namespace Flazzy.ABC
{
    [Flags]
    public enum MethodFlags
    {
        None           = 0,
        NeedArguments  = 1 << 0,
        NeedActivation = 1 << 1,
        NeedRest       = 1 << 2,
        HasOptional    = 1 << 3,
        IgnoreRest     = 1 << 4,
        Explicit       = 1 << 5,
        SetDxns        = 1 << 6,
        HasParamNames  = 1 << 7
    }
}