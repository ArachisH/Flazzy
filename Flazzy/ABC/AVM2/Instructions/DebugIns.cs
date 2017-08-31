using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class DebugIns : ASInstruction
    {
        public int NameIndex { get; set; }
        public string Name => ABC.Pool.Strings[NameIndex];

        public int Extra { get; set; }
        public byte DebugType { get; set; }
        public byte RegisterIndex { get; set; }

        public DebugIns(ABCFile abc)
            : base(OPCode.Debug, abc)
        { }
        public DebugIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            DebugType = input.ReadByte();
            NameIndex = input.ReadInt30();
            RegisterIndex = input.ReadByte();
            Extra = input.ReadInt30();
        }
        public DebugIns(ABCFile abc, int nameIndex, byte debugType, byte registerIndex)
            : this(abc)
        {
            NameIndex = nameIndex;
            DebugType = debugType;
            RegisterIndex = registerIndex;
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.Write(DebugType);
            output.WriteInt30(NameIndex);
            output.Write(RegisterIndex);
            output.WriteInt30(Extra);
        }
    }
}