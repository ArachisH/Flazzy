using Flazzy.IO;

namespace Flazzy.ABC;

public class ASScript : ASContainer // TODO: Check QName usages
{
    public int InitializerIndex { get; set; }
    public ASMethod Initializer => ABC.Methods[InitializerIndex];

    public override ASMultiname QName => Traits[0].QName;

    public ASScript(ABCFile abc)
        : base(abc)
    { }
    public ASScript(ABCFile abc, ref SpanFlashReader input)
        : base(abc)
    {
        InitializerIndex = input.ReadEncodedInt();
        PopulateTraits(ref input);
    }

    public override int GetSize()
    {
        return SpanFlashWriter.GetEncodedIntSize(InitializerIndex) +  base.GetSize();
    }
    public override void WriteTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(InitializerIndex);
        base.WriteTo(ref output);
    }
}