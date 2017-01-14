using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public abstract class Brancher : Jumper
    {
        public Brancher(OPCode op)
            : base(op)
        { }
        public Brancher(OPCode op, FlashReader input)
            : base(op, input)
        { }

        public abstract bool? RunCondition(ASMachine machine);

        public static new bool IsValid(OPCode op)
        {
            switch (op)
            {
                case OPCode.IfEq:
                case OPCode.IfGe:
                case OPCode.IfGt:
                case OPCode.IfLe:
                case OPCode.IfLt:
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