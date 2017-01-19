using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class DebugFileIns : ASInstruction
    {
        public string FileName
        {
            get { return ABC.Pool.Strings[FileNameIndex]; }
        }
        public int FileNameIndex { get; set; }

        public DebugFileIns(ABCFile abc)
            : base(OPCode.DebugFile, abc)
        { }
        public DebugFileIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            FileNameIndex = input.ReadInt30();
        }
        
        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(FileNameIndex);
        }
    }
}