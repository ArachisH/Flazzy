using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushByteIns : Primitive
    {
        private byte _value;
        new public byte Value
        {
            get => _value;
            set
            {
                _value = value;
                base.Value = value;
            }
        }

        public PushByteIns()
            : base(OPCode.PushByte)
        { }
        public PushByteIns(byte value)
            : this()
        {
            Value = value;
        }
        public PushByteIns(FlashReader input)
            : this()
        {
            Value = input.ReadByte();
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.Write(Value);
        }
    }
}