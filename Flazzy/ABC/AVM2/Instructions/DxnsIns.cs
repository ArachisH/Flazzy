using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class DxnsIns : ASInstruction
    {
        public int UriIndex { get; set; }
        public string Uri => ABC.Pool.Strings[UriIndex];

        public DxnsIns(ABCFile abc)
            : base(OPCode.Dxns, abc)
        { }
        public DxnsIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            UriIndex = input.ReadInt30();
        }
    }
}