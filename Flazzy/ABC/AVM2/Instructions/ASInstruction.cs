using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public abstract class ASInstruction : FlashItem, ICloneable
{
    public OPCode OP { get; }
    protected ABCFile ABC { get; }
    protected override string DebuggerDisplay => OP.ToString();

    public ASInstruction(OPCode op)
    {
        OP = op;
    }
    protected ASInstruction(OPCode op, ABCFile abc)
        : this(op)
    {
        ABC = abc;
    }

    public override void WriteTo(FlashWriter output)
    {
        output.Write((byte)OP);
        WriteValuesTo(output);
    }
    protected virtual void WriteValuesTo(FlashWriter output)
    { }

    public virtual int GetPopCount()
    {
        return 0;
    }
    public virtual int GetPushCount()
    {
        return 0;
    }
    public virtual void Execute(ASMachine machine)
    { }

    protected int ResolveMultinamePops(ASMultiname multiname)
    {
        int popCount = 0;
        if (multiname.IsNameNeeded)
        {
            popCount++;
        }
        if (multiname.IsNamespaceNeeded)
        {
            popCount++;
        }
        return popCount;
    }
    protected void ResolveMultiname(ASMachine machine, ASMultiname multiname)
    {
        if (multiname.IsNameNeeded)
        {
            object name = machine.Values.Pop();
        }
        if (multiname.IsNamespaceNeeded)
        {
            object @namespace = machine.Values.Pop();
        }
    }

    // TODO: Use source generator to add new items at compile time.
    public static bool IsPropertyContainer(OPCode op) => op is OPCode.CallPropVoid or OPCode.CallProperty or OPCode.ConstructProp;

    public static ASInstruction Create(ABCFile abc, FlashReader input)
    {
        var op = (OPCode)input.ReadByte();
        return op switch
        {
            // Arithmetic
            OPCode.Add_i => new AddIIns(),
            OPCode.Add => new AddIns(),
            OPCode.Decrement_i => new DecrementIIns(),
            OPCode.Decrement => new DecrementIns(),
            OPCode.Divide => new DivideIns(),
            OPCode.Equals => new EqualsIns(),
            OPCode.GreaterEquals => new GreaterEqualsIns(),
            OPCode.GreaterThan => new GreaterThanIns(),
            OPCode.Increment_i => new IncrementIIns(),
            OPCode.Increment => new IncrementIns(),
            OPCode.In => new InIns(),
            OPCode.IsTypeLate => new IsTypeLateIns(),
            OPCode.LessEquals => new LessEqualsIns(),
            OPCode.LessThan => new LessThanIns(),
            OPCode.Modulo => new ModuloIns(),
            OPCode.Multiply_i => new MultiplyIIns(),
            OPCode.Multiply => new MultiplyIns(),
            OPCode.Negate_i => new NegateIIns(),
            OPCode.Negate => new NegateIns(),
            OPCode.StrictEquals => new StrictEqualsIns(),
            OPCode.Subtract_i => new SubtractIIns(),
            OPCode.Subtract => new SubtractIns(),

            // Bit manipulation
            OPCode.BitAnd => new BitAndIns(),
            OPCode.BitNot => new BitNotIns(),
            OPCode.BitOr => new BitOrIns(),
            OPCode.BitXor => new BitXorIns(),
            OPCode.LShift => new LShiftIns(),
            OPCode.RShift => new RShiftIns(),
            OPCode.URShift => new URShiftIns(),

            // Control transfer
            OPCode.IfEq => new IfEqualIns(input),
            OPCode.IfFalse => new IfFalseIns(input),
            OPCode.IfGe => new IfGreaterEqualIns(input),
            OPCode.IfGt => new IfGreaterThanIns(input),
            OPCode.IfLe => new IfLessEqualIns(input),
            OPCode.IfLt => new IfLessThanIns(input),
            OPCode.IfNe => new IfNotEqualIns(input),
            OPCode.IfNGe => new IfNotGreaterEqualIns(input),
            OPCode.IfNGt => new IfNotGreaterThanIns(input),
            OPCode.IfNLe => new IfNotLessEqualIns(input),
            OPCode.IfNLt => new IfNotLessThanIns(input),
            OPCode.IfStrictEq => new IfStrictEqualIns(input),
            OPCode.IfStrictNE => new IfStrictNotEqualIns(input),
            OPCode.IfTrue => new IfTrueIns(input),
            OPCode.Jump => new JumpIns(input),

            // Register management
            OPCode.DecLocal_i => new DecLocalIIns(input),
            OPCode.DecLocal => new DecLocalIns(input),
            OPCode.GetLocal_0 => new GetLocal0Ins(),
            OPCode.GetLocal_1 => new GetLocal1Ins(),
            OPCode.GetLocal_2 => new GetLocal2Ins(),
            OPCode.GetLocal_3 => new GetLocal3Ins(),
            OPCode.GetLocal => new GetLocalIns(input),
            OPCode.IncLocal_i => new IncLocalIIns(input),
            OPCode.IncLocal => new IncLocalIns(input),
            OPCode.Kill => new KillIns(input),
            OPCode.SetLocal_0 => new SetLocal0Ins(),
            OPCode.SetLocal_1 => new SetLocal1Ins(),
            OPCode.SetLocal_2 => new SetLocal2Ins(),
            OPCode.SetLocal_3 => new SetLocal3Ins(),
            OPCode.SetLocal => new SetLocalIns(input),

            // Stack management
            OPCode.PushByte => new PushByteIns(input),
            OPCode.PushDouble => new PushDoubleIns(abc, input),
            OPCode.PushFalse => new PushFalseIns(),
            OPCode.PushInt => new PushIntIns(abc, input),
            OPCode.PushNan => new PushNaNIns(),
            OPCode.PushNull => new PushNullIns(),
            OPCode.PushShort => new PushShortIns(input),
            OPCode.PushString => new PushStringIns(abc, input),
            OPCode.PushTrue => new PushTrueIns(),
            OPCode.PushUInt => new PushUIntIns(abc, input),

            // Type conversion
            OPCode.Coerce_a => new CoerceAIns(),
            OPCode.Coerce => new CoerceIns(abc, input),
            OPCode.Coerce_s => new CoerceSIns(),
            OPCode.Convert_b => new ConvertBIns(),
            OPCode.Convert_d => new ConvertDIns(),
            OPCode.Convert_i => new ConvertIIns(),
            OPCode.Convert_o => new ConvertOIns(),
            OPCode.Convert_s => new ConvertSIns(),
            OPCode.Convert_u => new ConvertUIns(),

            // Miscellaneous
            OPCode.ApplyType => new ApplyTypeIns(input),
            OPCode.AsType => new AsTypeIns(abc, input),
            OPCode.AsTypeLate => new AsTypeLateIns(),
            OPCode.Call => new CallIns(input),
            OPCode.CallMethod => new CallMethodIns(abc, input),
            OPCode.CallProperty => new CallPropertyIns(abc, input),
            OPCode.CallPropLex => new CallPropLexIns(abc, input),
            OPCode.CallPropVoid => new CallPropVoidIns(abc, input),
            OPCode.CallStatic => new CallStaticIns(abc, input),
            OPCode.CallSuper => new CallSuperIns(abc, input),
            OPCode.CallSuperVoid => new CallSuperVoidIns(abc, input),
            OPCode.CheckFilter => new CheckFilterIns(),
            OPCode.Construct => new ConstructIns(input),
            OPCode.ConstructProp => new ConstructPropIns(abc, input),
            OPCode.ConstructSuper => new ConstructSuperIns(input),
            OPCode.DebugFile => new DebugFileIns(abc, input),
            OPCode.Debug => new DebugIns(abc, input),
            OPCode.DebugLine => new DebugLineIns(input),
            OPCode.DeleteProperty => new DeletePropertyIns(abc, input),
            OPCode.Dup => new DupIns(),
            OPCode.Dxns => new DxnsIns(abc, input),
            OPCode.DxnsLate => new DxnsLateIns(),
            OPCode.Esc_XElem => new EscXElemIns(),
            OPCode.Esc_XAttr => new EscXAttrIns(),
            OPCode.FindProperty => new FindPropertyIns(abc, input),
            OPCode.FindPropStrict => new FindPropStrictIns(abc, input),
            OPCode.GetDescendants => new GetDescendantsIns(abc, input),
            OPCode.GetGlobalScope => new GetGlobalScopeIns(),
            OPCode.GetLex => new GetLexIns(abc, input),
            OPCode.GetProperty => new GetPropertyIns(abc, input),
            OPCode.GetScopeObject => new GetScopeObjectIns(input),
            OPCode.GetSlot => new GetSlotIns(input),
            OPCode.GetSuper => new GetSuperIns(abc, input),
            OPCode.HasNext2 => new HasNext2Ins(input),
            OPCode.HasNext => new HasNextIns(),
            OPCode.InitProperty => new InitPropertyIns(abc, input),
            OPCode.InstanceOf => new InstanceOfIns(),
            OPCode.Label => new LabelIns(),
            OPCode.LookUpSwitch => new LookUpSwitchIns(input),
            OPCode.NewActivation => new NewActivationIns(),
            OPCode.NewArray => new NewArrayIns(input),
            OPCode.NewCatch => new NewCatchIns(input),
            OPCode.NewClass => new NewClassIns(abc, input),
            OPCode.NewFunction => new NewFunctionIns(abc, input),
            OPCode.NewObject => new NewObjectIns(input),
            OPCode.NextName => new NextNameIns(),
            OPCode.NextValue => new NextValueIns(),
            OPCode.Nop => new NopIns(),
            OPCode.Not => new NotIns(),
            OPCode.Pop => new PopIns(),
            OPCode.PopScope => new PopScopeIns(),
            OPCode.PushScope => new PushScopeIns(),
            OPCode.PushUndefined => new PushUndefinedIns(),
            OPCode.PushWith => new PushWithIns(),
            OPCode.ReturnValue => new ReturnValueIns(),
            OPCode.ReturnVoid => new ReturnVoidIns(),
            OPCode.SetProperty => new SetPropertyIns(abc, input),
            OPCode.SetSlot => new SetSlotIns(input),
            OPCode.SetSuper => new SetSuperIns(abc, input),
            OPCode.Swap => new SwapIns(),
            OPCode.Throw => new ThrowIns(),
            OPCode.TypeOf => new TypeOfIns(),

            _ => throw new Exception("Unhandled OPCode: " + op),
        };
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
    public ASInstruction Clone()
    {
        return (ASInstruction)MemberwiseClone();
    }
}