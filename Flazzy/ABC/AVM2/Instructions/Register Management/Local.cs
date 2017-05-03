using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public abstract class Local : ASInstruction
    {
        public virtual int Register { get; set; }

        public Local(OPCode op)
            : base(op)
        { }
        public Local(OPCode op, int register)
            : this(op)
        {
            Register = register;
        }
        public Local(OPCode op, FlashReader input)
            : this(op)
        {
            Register = input.ReadInt30();
        }

        public override int GetPopCount()
        {
            return (IsSetLocal(OP) ? 1 : 0);
        }
        public override int GetPushCount()
        {
            return (IsGetLocal(OP) ? 1 : 0);
        }
        public override void Execute(ASMachine machine)
        {
            if (IsSetLocal(OP))
            {
                machine.Registers[Register] = machine.Values.Pop();
            }
            else if (IsGetLocal(OP))
            {
                object value = machine.Registers[Register];
                machine.Values.Push(value);
            }
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            switch (OP)
            {
                case OPCode.SetLocal_0:
                case OPCode.SetLocal_1:
                case OPCode.SetLocal_2:
                case OPCode.SetLocal_3:

                case OPCode.GetLocal_0:
                case OPCode.GetLocal_1:
                case OPCode.GetLocal_2:
                case OPCode.GetLocal_3: return;

                default:
                output.WriteInt30(Register);
                break;
            }
        }

        public static bool IsValid(OPCode op)
        {
            switch (op)
            {
                case OPCode.Kill:

                case OPCode.DecLocal:
                case OPCode.DecLocal_i:

                case OPCode.IncLocal:
                case OPCode.IncLocal_i:

                case OPCode.GetLocal:
                case OPCode.GetLocal_0:
                case OPCode.GetLocal_1:
                case OPCode.GetLocal_2:
                case OPCode.GetLocal_3:

                case OPCode.SetLocal:
                case OPCode.SetLocal_0:
                case OPCode.SetLocal_1:
                case OPCode.SetLocal_2:
                case OPCode.SetLocal_3:
                return true;

                default: return false;
            }
        }
        public static bool IsGetLocal(OPCode op)
        {
            switch (op)
            {
                case OPCode.GetLocal:
                case OPCode.GetLocal_0:
                case OPCode.GetLocal_1:
                case OPCode.GetLocal_2:
                case OPCode.GetLocal_3:
                return true;

                default: return false;
            }
        }
        public static bool IsSetLocal(OPCode op)
        {
            switch (op)
            {
                case OPCode.SetLocal:
                case OPCode.SetLocal_0:
                case OPCode.SetLocal_1:
                case OPCode.SetLocal_2:
                case OPCode.SetLocal_3:
                return true;

                default: return false;
            }
        }

        public static Local CreateSet(int register)
        {
            switch (register)
            {
                case 0: return new SetLocal0Ins();
                case 1: return new SetLocal1Ins();
                case 2: return new SetLocal2Ins();
                case 3: return new SetLocal3Ins();

                default: return new SetLocalIns(register);
            }
        }
        public static Local CreateGet(int register)
        {
            switch (register)
            {
                case 0: return new GetLocal0Ins();
                case 1: return new GetLocal1Ins();
                case 2: return new GetLocal2Ins();
                case 3: return new GetLocal3Ins();

                default: return new GetLocalIns(register);
            }
        }
    }
}