using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class PushByteIns : Primitive
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
        public PushByteIns(ref FlashReader input)
            : this()
        {
            Value = input.ReadByte();
        }

        protected override int GetBodySize() => sizeof(byte);
        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.Write(Value);
        }
    }
}