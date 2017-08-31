using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class AsTypeIns : ASInstruction
    {
        public int TypeNameIndex { get; set; }
        public ASMultiname TypeName => ABC.Pool.Multinames[TypeNameIndex];

        public AsTypeIns(ABCFile abc)
            : base(OPCode.AsType, abc)
        { }
        public AsTypeIns(ABCFile abc, int typeNameIndex)
            : this(abc)
        {
            TypeNameIndex = typeNameIndex;
        }
        public AsTypeIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            TypeNameIndex = input.ReadInt30();
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
            object value = machine.Values.Pop();
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(TypeNameIndex);
        }
    }
}