using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public sealed class NewClassIns : ASInstruction
{
    public int ClassIndex { get; set; }
    public ASClass Class => ABC.Classes[ClassIndex];

    public NewClassIns(ABCFile abc)
        : base(OPCode.NewClass, abc)
    { }
    public NewClassIns(ABCFile abc, int classIndex)
        : this(abc)
    {
        ClassIndex = classIndex;
    }
    public NewClassIns(ABCFile abc, ref SpanFlashReader input)
        : this(abc)
    {
        ClassIndex = input.ReadEncodedInt();
    }

    public override int GetPopCount() => 1;
    public override int GetPushCount() => 1;
    public override void Execute(ASMachine machine)
    {
        object baseType = machine.Values.Pop();
        machine.Values.Push(null);
    }

    protected override int GetBodySize()
    {
        return SpanFlashWriter.GetEncodedIntSize(ClassIndex);
    }
    protected override void WriteValuesTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(ClassIndex);
    }
}