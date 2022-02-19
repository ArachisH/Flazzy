using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushShortIns : Primitive
    {
        private int _value;
        new public int Value
        {
            get => _value;
            set
            {
                _value = value;
                base.Value = value;
            }
        }

        public PushShortIns()
            : base(OPCode.PushShort)
        { }
        public PushShortIns(int value)
            : this()
        {
            Value = value;
        }
        public PushShortIns(ref FlashReader input)
            : this()
        {
            Value = input.ReadEncodedInt();
        }

        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(Value);
        }
    }
}