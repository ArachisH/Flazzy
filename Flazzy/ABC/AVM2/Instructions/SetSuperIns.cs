using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class SetSuperIns : ASInstruction
{
    public int PropertyNameIndex { get; set; }
    public ASMultiname PropertyName => ABC.Pool.Multinames[PropertyNameIndex];

    public SetSuperIns(ABCFile abc)
        : base(OPCode.SetSuper, abc)
    { }
    public SetSuperIns(ABCFile abc, ref FlashReader input)
        : this(abc)
    {
        PropertyNameIndex = input.ReadEncodedInt();
    }
    public SetSuperIns(ABCFile abc, int propertyNameIndex)
        : this(abc)
    {
        PropertyNameIndex = propertyNameIndex;
    }

    public override int GetPopCount()
    {
        return 2 + ResolveMultinamePops(PropertyName);
    }
    public override void Execute(ASMachine machine)
    {
        object value = machine.Values.Pop();
        ResolveMultiname(machine, PropertyName);
        object obj = machine.Values.Pop();
    }

    protected override int GetBodySize()
    {
        return FlashWriter.GetEncodedIntSize(PropertyNameIndex);
    }
    protected override void WriteValuesTo(ref FlashWriter output)
    {
        output.WriteEncodedInt(PropertyNameIndex);
    }
}
