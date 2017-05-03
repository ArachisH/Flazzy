using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public abstract class Jumper : ASInstruction
    {
        public uint Offset { get; set; }

        public Jumper(OPCode op)
            : base(op)
        { }
        public Jumper(OPCode op, uint offset)
            : this(op)
        {
            Offset = offset;
        }
        public Jumper(OPCode op, FlashReader input)
            : this(op)
        {
            Offset = input.ReadUInt24();
        }

        public override int GetPopCount()
        {
            switch (OP)
            {
                case OPCode.Jump:
                return 0;

                case OPCode.IfTrue:
                case OPCode.IfFalse:
                return 1;

                default: return 2;
            }
        }
        public override int GetPushCount()
        {
            return 0;
        }
        public override void Execute(ASMachine machine)
        {
            int popCount = GetPopCount();
            for (int i = 0; i < popCount; i++)
            {
                machine.Values.Pop();
            }
        }
        public abstract bool? RunCondition(ASMachine machine);

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteUInt24(Offset);
        }

        public static bool IsValid(OPCode op)
        {
            switch (op)
            {
                case OPCode.IfEq:
                case OPCode.IfGe:
                case OPCode.IfGt:
                case OPCode.IfLe:
                case OPCode.IfLt:
                case OPCode.Jump:
                case OPCode.IfNe:
                case OPCode.IfNGe:
                case OPCode.IfNGt:
                case OPCode.IfNLe:
                case OPCode.IfNLt:
                case OPCode.IfTrue:
                case OPCode.IfFalse:
                case OPCode.IfStrictEq:
                case OPCode.IfStrictNE:
                return true;

                default: return false;
            }
        }
    }
}