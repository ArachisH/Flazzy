using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushStringIns : Primitive
    {
        private string _value;
        new public string Value
        {
            get => _value;
            set
            {
                _value = value;
                _valueIndex = ABC.Pool.AddConstant(value);

                base.Value = value;
            }
        }

        private int _valueIndex;
        public int ValueIndex
        {
            get => _valueIndex;
            set
            {
                _valueIndex = value;
                _value = ABC.Pool.Strings[value];

                base.Value = _value;
            }
        }

        public PushStringIns(ABCFile abc)
            : base(OPCode.PushString, abc)
        { }
        public PushStringIns(ABCFile abc, string value)
            : this(abc)
        {
            Value = value;
        }
        public PushStringIns(ABCFile abc, int valueIndex)
            : this(abc)
        {
            ValueIndex = valueIndex;
        }
        public PushStringIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            ValueIndex = input.ReadInt30();
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(ValueIndex);
        }
    }
}