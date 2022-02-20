using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class DxnsIns : ASInstruction
    {
        public int UriIndex { get; set; }
        public string Uri => ABC.Pool.Strings[UriIndex];

        public DxnsIns(ABCFile abc)
            : base(OPCode.Dxns, abc)
        { }
        public DxnsIns(ABCFile abc, ref FlashReader input)
            : this(abc)
        {
            UriIndex = input.ReadEncodedInt();
        }

        protected override int GetBodySize()
        {
            return FlashWriter.GetEncodedIntSize(UriIndex);
        }
        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(UriIndex);
        }
    }
}