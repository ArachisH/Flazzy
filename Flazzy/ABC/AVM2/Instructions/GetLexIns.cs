using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class GetLexIns : ASInstruction
    {
        public ASMultiname TypeName
        {
            get { return ABC.Pool.Multinames[TypeNameIndex]; }
        }
        public int TypeNameIndex { get; set; }

        public GetLexIns(ABCFile abc)
            : base(OPCode.GetLex, abc)
        { }
        public GetLexIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            TypeNameIndex = input.ReadInt30();
        }
        public GetLexIns(ABCFile abc, int typeNameIndex)
            : this(abc)
        {
            TypeNameIndex = typeNameIndex;
        }

        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(TypeNameIndex);
        }
    }
}