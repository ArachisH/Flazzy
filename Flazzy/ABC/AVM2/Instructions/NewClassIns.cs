using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class NewClassIns : ASInstruction
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
        public NewClassIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            ClassIndex = input.ReadInt30();
        }

        public override int GetPopCount()
        {
            return 1;
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            object baseType = machine.Values.Pop();
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(ClassIndex);
        }
    }
}