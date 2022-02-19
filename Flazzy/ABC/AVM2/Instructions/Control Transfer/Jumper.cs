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
        public Jumper(OPCode op, ref FlashReader input)
            : this(op)
        {
            Offset = input.ReadUInt24();
        }

        public override int GetPopCount() => OP switch
        {
            OPCode.Jump => 0,

            OPCode.IfTrue or
            OPCode.IfFalse => 1,
            
            _ => 2,
        };
        public override int GetPushCount() => 0;

        public override void Execute(ASMachine machine)
        {
            int popCount = GetPopCount();
            for (int i = 0; i < popCount; i++)
            {
                machine.Values.Pop();
            }
        }
        public abstract bool? RunCondition(ASMachine machine);

        protected override int GetBodySize() => 3;
        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteUInt24(Offset);
        }

        public static bool IsValid(OPCode op) => op switch
        {
            OPCode.IfEq or
            OPCode.IfGe or
            OPCode.IfGt or
            OPCode.IfLe or
            OPCode.IfLt or
            OPCode.Jump or
            OPCode.IfNe or
            OPCode.IfNGe or
            OPCode.IfNGt or
            OPCode.IfNLe or
            OPCode.IfNLt or
            OPCode.IfTrue or
            OPCode.IfFalse or
            OPCode.IfStrictEq or
            OPCode.IfStrictNE => true,

            _ => false,
        };
    }
}