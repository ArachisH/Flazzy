using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASItemInfo : IFlashItem
    {
        private readonly ABCFile _abc;

        public int KeyIndex { get; set; }
        public string Key => _abc.Pool.Strings[KeyIndex];

        public int ValueIndex { get; set; }
        public string Value => _abc.Pool.Strings[ValueIndex];

        public ASItemInfo(ABCFile abc)
        {
            _abc = abc;
        }
        public ASItemInfo(ABCFile abc, ref FlashReader input)
            : this(abc)
        {
            KeyIndex = input.ReadInt30();
            ValueIndex = input.ReadInt30();
        }

        public int GetSize()
        {
            throw new NotImplementedException();
        }
        public void WriteTo(FlashWriter output)
        {
            output.WriteEncodedInt(KeyIndex);
            output.WriteEncodedInt(ValueIndex);
        }
    }
}