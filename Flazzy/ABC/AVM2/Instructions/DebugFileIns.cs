using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class DebugFileIns : ASInstruction
    {
        public int FileNameIndex { get; set; }
        public string FileName => ABC.Pool.Strings[FileNameIndex];

        public DebugFileIns(ABCFile abc)
            : base(OPCode.DebugFile, abc)
        { }
        public DebugFileIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            FileNameIndex = input.ReadInt30();
        }
        public DebugFileIns(ABCFile abc, int fileNameIndex)
            : this(abc)
        {
            FileNameIndex = fileNameIndex;
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(FileNameIndex);
        }
    }
}