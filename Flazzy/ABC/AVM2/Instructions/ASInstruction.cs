using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions;

public abstract class ASInstruction : IFlashItem, ICloneable
{
    public OPCode OP { get; }
    protected ABCFile ABC { get; }

    public ASInstruction(OPCode op)
    {
        OP = op;
    }
    protected ASInstruction(OPCode op, ABCFile abc)
        : this(op)
    {
        ABC = abc;
    }

    public virtual int GetPopCount() => 0;
    public virtual int GetPushCount() => 0;
    public virtual void Execute(ASMachine machine)
    { }

    protected virtual int GetBodySize() => 0;
    protected virtual void WriteValuesTo(ref SpanFlashWriter output)
    { }

    public int GetSize() => sizeof(OPCode) + GetBodySize();
    public void WriteTo(ref SpanFlashWriter output)
    {
        output.Write((byte)OP);
        WriteValuesTo(ref output);
    }

    protected static int ResolveMultinamePops(ASMultiname multiname)
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
    protected static void ResolveMultiname(ASMachine machine, ASMultiname multiname)
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

    public static bool IsPropertyContainer(OPCode op) => op is OPCode.CallPropVoid or OPCode.CallProperty or OPCode.ConstructProp;
    public static ASInstruction Create(ABCFile abc, ref SpanFlashReader input)
    {
        OPCode op = (OPCode)input.ReadByte();
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
            OPCode.IfEq => new IfEqualIns(ref input),
            OPCode.IfGe => new IfGreaterEqualIns(ref input),
            OPCode.IfGt => new IfGreaterThanIns(ref input),
            OPCode.IfLe => new IfLessEqualIns(ref input),
            OPCode.IfLt => new IfLessThanIns(ref input),
            OPCode.IfNe => new IfNotEqualIns(ref input),
            OPCode.IfNGe => new IfNotGreaterEqualIns(ref input),
            OPCode.IfNGt => new IfNotGreaterThanIns(ref input),
            OPCode.IfNLe => new IfNotLessEqualIns(ref input),
            OPCode.IfNLt => new IfNotLessThanIns(ref input),
            OPCode.IfStrictEq => new IfStrictEqualIns(ref input),
            OPCode.IfStrictNE => new IfStrictNotEqualIns(ref input),
            OPCode.IfTrue => new IfTrueIns(ref input),
            OPCode.IfFalse => new IfFalseIns(ref input),
            OPCode.Jump => new JumpIns(ref input),

            OPCode.Label => new LabelIns(),
            OPCode.LookUpSwitch => new LookUpSwitchIns(ref input),

            // Register management
            OPCode.DecLocal_i => new DecLocalIIns(ref input),
            OPCode.DecLocal => new DecLocalIns(ref input),
            OPCode.GetLocal_0 => new GetLocal0Ins(),
            OPCode.GetLocal_1 => new GetLocal1Ins(),
            OPCode.GetLocal_2 => new GetLocal2Ins(),
            OPCode.GetLocal_3 => new GetLocal3Ins(),
            OPCode.GetLocal => new GetLocalIns(ref input),
            OPCode.IncLocal_i => new IncLocalIIns(ref input),
            OPCode.IncLocal => new IncLocalIns(ref input),
            OPCode.Kill => new KillIns(ref input),
            OPCode.SetLocal_0 => new SetLocal0Ins(),
            OPCode.SetLocal_1 => new SetLocal1Ins(),
            OPCode.SetLocal_2 => new SetLocal2Ins(),
            OPCode.SetLocal_3 => new SetLocal3Ins(),
            OPCode.SetLocal => new SetLocalIns(ref input),

            // Stack management
            OPCode.PushNull => new PushNullIns(),
            OPCode.PushUndefined => new PushUndefinedIns(),

            OPCode.PushByte => new PushByteIns(ref input),
            OPCode.PushShort => new PushShortIns(ref input),
            OPCode.PushTrue => new PushTrueIns(),
            OPCode.PushFalse => new PushFalseIns(),
            OPCode.PushString => new PushStringIns(abc, ref input),
            OPCode.PushNan => new PushNaNIns(),
            OPCode.PushInt => new PushIntIns(abc, ref input),
            OPCode.PushUInt => new PushUIntIns(abc, ref input),
            OPCode.PushDouble => new PushDoubleIns(abc, ref input),

            OPCode.Dup => new DupIns(),
            OPCode.Swap => new SwapIns(),
            OPCode.Not => new NotIns(),
            OPCode.Pop => new PopIns(),
            OPCode.PopScope => new PopScopeIns(),
            OPCode.PushWith => new PushWithIns(),
            OPCode.PushScope => new PushScopeIns(),

            // Type conversion
            OPCode.Coerce_a => new CoerceAIns(),
            OPCode.Coerce => new CoerceIns(abc, ref input),
            OPCode.Coerce_s => new CoerceSIns(),
            OPCode.Convert_b => new ConvertBIns(),
            OPCode.Convert_d => new ConvertDIns(),
            OPCode.Convert_i => new ConvertIIns(),
            OPCode.Convert_o => new ConvertOIns(),
            OPCode.Convert_s => new ConvertSIns(),
            OPCode.Convert_u => new ConvertUIns(),

            // Miscellaneous
            OPCode.Nop => new NopIns(),
            OPCode.ApplyType => new ApplyTypeIns(ref input),
            OPCode.AsType => new AsTypeIns(abc, ref input),
            OPCode.AsTypeLate => new AsTypeLateIns(),

            OPCode.Call => new CallIns(ref input),
            OPCode.CallMethod => new CallMethodIns(abc, ref input),
            OPCode.CallProperty => new CallPropertyIns(abc, ref input),
            OPCode.CallPropLex => new CallPropLexIns(abc, ref input),
            OPCode.CallPropVoid => new CallPropVoidIns(abc, ref input),
            OPCode.CallStatic => new CallStaticIns(abc, ref input),
            OPCode.CallSuper => new CallSuperIns(abc, ref input),
            OPCode.CallSuperVoid => new CallSuperVoidIns(abc, ref input),

            OPCode.CheckFilter => new CheckFilterIns(),

            OPCode.Construct => new ConstructIns(ref input),
            OPCode.ConstructProp => new ConstructPropIns(abc, ref input),
            OPCode.ConstructSuper => new ConstructSuperIns(ref input),

            OPCode.DebugFile => new DebugFileIns(abc, ref input),
            OPCode.Debug => new DebugIns(abc, ref input),
            OPCode.DebugLine => new DebugLineIns(ref input),

            OPCode.DeleteProperty => new DeletePropertyIns(abc, ref input),

            OPCode.Dxns => new DxnsIns(abc, ref input),
            OPCode.DxnsLate => new DxnsLateIns(),

            OPCode.Esc_XElem => new EscXElemIns(),
            OPCode.Esc_XAttr => new EscXAttrIns(),
            OPCode.FindProperty => new FindPropertyIns(abc, ref input),
            OPCode.FindPropStrict => new FindPropStrictIns(abc, ref input),
            OPCode.GetDescendants => new GetDescendantsIns(abc, ref input),
            OPCode.GetGlobalScope => new GetGlobalScopeIns(),
            OPCode.GetLex => new GetLexIns(abc, ref input),
            OPCode.GetProperty => new GetPropertyIns(abc, ref input),
            OPCode.GetScopeObject => new GetScopeObjectIns(ref input),
            OPCode.GetSlot => new GetSlotIns(ref input),
            OPCode.GetSuper => new GetSuperIns(abc, ref input),

            OPCode.HasNext => new HasNextIns(),
            OPCode.HasNext2 => new HasNext2Ins(ref input),

            OPCode.InitProperty => new InitPropertyIns(abc, ref input),
            OPCode.InstanceOf => new InstanceOfIns(),

            OPCode.NewActivation => new NewActivationIns(),
            OPCode.NewArray => new NewArrayIns(ref input),
            OPCode.NewCatch => new NewCatchIns(ref input),
            OPCode.NewClass => new NewClassIns(abc, ref input),
            OPCode.NewFunction => new NewFunctionIns(abc, ref input),
            OPCode.NewObject => new NewObjectIns(ref input),

            OPCode.NextName => new NextNameIns(),
            OPCode.NextValue => new NextValueIns(),
            OPCode.ReturnValue => new ReturnValueIns(),
            OPCode.ReturnVoid => new ReturnVoidIns(),
            OPCode.SetProperty => new SetPropertyIns(abc, ref input),
            OPCode.SetSlot => new SetSlotIns(ref input),
            OPCode.SetSuper => new SetSuperIns(abc, ref input),
            OPCode.Throw => new ThrowIns(),
            OPCode.TypeOf => new TypeOfIns(),

            _ => throw new Exception("Unhandled OPCode: " + op)
        };
    }

    object ICloneable.Clone() => Clone();
    public ASInstruction Clone() => (ASInstruction)MemberwiseClone();

    public override string ToString() => OP.ToString();
}