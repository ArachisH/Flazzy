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
        public PushShortIns(FlashReader input)
            : this()
        {
            Value = input.ReadInt30();
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(Value);
        }
    }
}