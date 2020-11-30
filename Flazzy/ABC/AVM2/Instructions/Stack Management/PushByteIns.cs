using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushByteIns : Primitive
    {
        private sbyte _value;
        new public sbyte Value
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
        public PushByteIns(sbyte value)
            : this()
        {
            Value = value;
        }
        public PushByteIns(FlashReader input)
            : this()
        {
            Value = input.ReadSByte();
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.Write(Value);
        }
    }
}