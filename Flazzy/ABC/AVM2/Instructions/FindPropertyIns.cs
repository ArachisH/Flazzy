using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class FindPropertyIns : ASInstruction
{
    public int PropertyNameIndex { get; set; }
    public ASMultiname PropertyName => ABC.Pool.Multinames[PropertyNameIndex];

    public FindPropertyIns(ABCFile abc)
        : base(OPCode.FindProperty, abc)
    { }
    public FindPropertyIns(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        PropertyNameIndex = input.ReadEncodedInt();
    }
    public FindPropertyIns(ABCFile abc, int propertyNameIndex)
        : this(abc)
    {
        PropertyNameIndex = propertyNameIndex;
    }

    public override int GetPopCount()
    {
        return ResolveMultinamePops(PropertyName);
    }
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        ResolveMultiname(machine, PropertyName);
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(PropertyNameIndex);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(PropertyNameIndex);
    }
}