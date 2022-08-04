namespace Flazzy.ABC
{
    [Flags]
    public enum MethodFlags
    {
        None = 0,
        NeedArguments = 1,
        NeedActivation = 2,
        NeedRest = 4,
        HasOptional = 8,
        IgnoreRest = 16,
        Explicit = 32,
        SetDxns = 64,
        HasParamNames = 128
    }
}