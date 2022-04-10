using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public abstract class Local : ASInstruction
{
    public virtual int Register { get; }

    public Local(OPCode op)
        : base(op)
    { }
    public Local(OPCode op, int register)
        : this(op)
    {
        Register = register;
    }
    public Local(OPCode op, ref FlashReader input)
        : this(op)
    {
        Register = input.ReadEncodedInt();
    }

    public override int GetPopCount()
    {
        return IsSetLocal(OP) ? 1 : 0;
    }
    public override int GetPushCount()
    {
        return IsGetLocal(OP) ? 1 : 0;
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

    protected override int GetBodySize()
    {
        return OP switch
        {
            OPCode.Kill or

            OPCode.SetLocal or
            OPCode.GetLocal or

            OPCode.IncLocal or
            OPCode.IncLocal_i or

            OPCode.DecLocal or
            OPCode.DecLocal_i => FlashWriter.GetEncodedIntSize(Register),

            _ => 0
        };
    }
    protected override void WriteValuesTo(ref FlashWriter output)
    {
        switch (OP)
        {
            case OPCode.Kill:

            case OPCode.SetLocal:
            case OPCode.GetLocal:

            case OPCode.IncLocal:
            case OPCode.IncLocal_i:

            case OPCode.DecLocal:
            case OPCode.DecLocal_i:
                output.WriteEncodedInt(Register);
                break;
        }
    }

    public static bool IsValid(OPCode op) => op switch
    {
        OPCode.Kill or

        OPCode.DecLocal or
        OPCode.DecLocal_i or

        OPCode.IncLocal or
        OPCode.IncLocal_i or

        OPCode.GetLocal or
        OPCode.GetLocal_0 or
        OPCode.GetLocal_1 or
        OPCode.GetLocal_2 or
        OPCode.GetLocal_3 or

        OPCode.SetLocal or
        OPCode.SetLocal_0 or
        OPCode.SetLocal_1 or
        OPCode.SetLocal_2 or
        OPCode.SetLocal_3 => true,

        _ => false
    };
    public static bool IsGetLocal(OPCode op) => op switch
    {
        OPCode.GetLocal or
        OPCode.GetLocal_0 or
        OPCode.GetLocal_1 or
        OPCode.GetLocal_2 or
        OPCode.GetLocal_3 => true,

        _ => false
    };
    public static bool IsSetLocal(OPCode op) => op switch
    {
        OPCode.SetLocal or
        OPCode.SetLocal_0 or
        OPCode.SetLocal_1 or
        OPCode.SetLocal_2 or
        OPCode.SetLocal_3 => true,

        _ => false
    };

    public static Local CreateSet(int register) => register switch
    {
        0 => new SetLocal0Ins(),
        1 => new SetLocal1Ins(),
        2 => new SetLocal2Ins(),
        3 => new SetLocal3Ins(),

        _ => new SetLocalIns(register),
    };
    public static Local CreateGet(int register) => register switch
    {
        0 => new GetLocal0Ins(),
        1 => new GetLocal1Ins(),
        2 => new GetLocal2Ins(),
        3 => new GetLocal3Ins(),

        _ => new GetLocalIns(register),
    };
}
