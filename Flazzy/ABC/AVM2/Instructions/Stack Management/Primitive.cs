namespace Flazzy.ABC.AVM2.Instructions
{
    public abstract class Primitive : ASInstruction
    {
        public virtual object Value { get; set; }

        public Primitive(OPCode op)
            : base(op)
        { }
        public Primitive(OPCode op, ABCFile abc)
            : base(op, abc)
        { }

        public override int GetPopCount()
        {
            return 0;
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(Value);
        }

        public static bool IsValid(OPCode op)
        {
            switch (op)
            {
                case OPCode.PushByte:
                case OPCode.PushDouble:
                case OPCode.PushFalse:
                case OPCode.PushInt:
                case OPCode.PushNan:
                case OPCode.PushNull:
                case OPCode.PushShort:
                case OPCode.PushString:
                case OPCode.PushTrue:
                case OPCode.PushUInt:
                return true;

                default:
                return false;
            }
        }
        public static Primitive Create(ABCFile abc, object value)
        {
            var typeCode = Type.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.Int32:
                return new PushIntIns(abc, (int)value);

                case TypeCode.Int16:
                return new PushShortIns((int)value);

                case TypeCode.UInt32:
                return new PushUIntIns(abc, (uint)value);

                case TypeCode.Double:
                return new PushDoubleIns(abc, (double)value);

                case TypeCode.String:
                return new PushStringIns(abc, (string)value);

                case TypeCode.Boolean:
                {
                    var result = (bool)value;
                    if (result)
                    {
                        return new PushTrueIns();
                    }
                    else return new PushFalseIns();
                }

                case TypeCode.Empty:
                return new PushNullIns();

                default:
                return new PushNaNIns();
            }
        }
    }
}