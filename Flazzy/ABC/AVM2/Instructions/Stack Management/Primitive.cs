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

        public override int GetPopCount() => 0;
        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(Value);
        }

        public static bool IsValid(OPCode op)
        {
            return op switch
            {
                OPCode.PushNan or 
                OPCode.PushNull or 
                OPCode.PushByte or
                OPCode.PushShort or 
                OPCode.PushInt or
                OPCode.PushUInt or
                OPCode.PushDouble or 
                OPCode.PushString or 
                OPCode.PushTrue or 
                OPCode.PushFalse => true,

                _ => false
            };
        }
        public static Primitive Create(ABCFile abc, object value)
        {
            return value switch
            {
                byte @byte => new PushByteIns(@byte),
                short @short => new PushShortIns(@short),
                int @int => new PushIntIns(abc, @int),
                uint @uint => new PushUIntIns(abc, @uint),
                double @double => new PushDoubleIns(abc, @double),
                string @string => new PushStringIns(abc, @string),

                bool @bool when @bool => new PushTrueIns(),
                bool @bool when !@bool => new PushFalseIns(),

                null => new PushNullIns(),
                _ => new PushNaNIns()
            };
        }
    }
}