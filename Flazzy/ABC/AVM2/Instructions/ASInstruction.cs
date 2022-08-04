using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
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
        public static bool IsPropertyContainer(OPCode op)
        {
            switch (op)
            {
                case OPCode.CallPropVoid:
                case OPCode.CallProperty:
                case OPCode.ConstructProp: return true;
                default: return false;
            }
        }
        public static ASInstruction Create(ABCFile abc, FlashReader input)
        {
            var op = (OPCode)input.ReadByte();
            switch (op)
            {
                #region Arithmetic
                case OPCode.Add_i:
                return new AddIIns();

                case OPCode.Add:
                return new AddIns();

                case OPCode.Decrement_i:
                return new DecrementIIns();

                case OPCode.Decrement:
                return new DecrementIns();

                case OPCode.Divide:
                return new DivideIns();

                case OPCode.Equals:
                return new EqualsIns();

                case OPCode.GreaterEquals:
                return new GreaterEqualsIns();

                case OPCode.GreaterThan:
                return new GreaterThanIns();

                case OPCode.Increment_i:
                return new IncrementIIns();

                case OPCode.Increment:
                return new IncrementIns();

                case OPCode.In:
                return new InIns();

                case OPCode.IsTypeLate:
                return new IsTypeLateIns();

                case OPCode.LessEquals:
                return new LessEqualsIns();

                case OPCode.LessThan:
                return new LessThanIns();

                case OPCode.Modulo:
                return new ModuloIns();

                case OPCode.Multiply_i:
                return new MultiplyIIns();

                case OPCode.Multiply:
                return new MultiplyIns();

                case OPCode.Negate_i:
                return new NegateIIns();

                case OPCode.Negate:
                return new NegateIns();

                case OPCode.StrictEquals:
                return new StrictEqualsIns();

                case OPCode.Subtract_i:
                return new SubtractIIns();

                case OPCode.Subtract:
                return new SubtractIns();
                #endregion

                #region Bit Manipulation
                case OPCode.BitAnd:
                return new BitAndIns();

                case OPCode.BitNot:
                return new BitNotIns();

                case OPCode.BitOr:
                return new BitOrIns();

                case OPCode.BitXor:
                return new BitXorIns();

                case OPCode.LShift:
                return new LShiftIns();

                case OPCode.RShift:
                return new RShiftIns();

                case OPCode.URShift:
                return new URShiftIns();
                #endregion

                #region Control Transfer
                case OPCode.IfEq:
                return new IfEqualIns(input);

                case OPCode.IfFalse:
                return new IfFalseIns(input);

                case OPCode.IfGe:
                return new IfGreaterEqualIns(input);

                case OPCode.IfGt:
                return new IfGreaterThanIns(input);

                case OPCode.IfLe:
                return new IfLessEqualIns(input);

                case OPCode.IfLt:
                return new IfLessThanIns(input);

                case OPCode.IfNe:
                return new IfNotEqualIns(input);

                case OPCode.IfNGe:
                return new IfNotGreaterEqualIns(input);

                case OPCode.IfNGt:
                return new IfNotGreaterThanIns(input);

                case OPCode.IfNLe:
                return new IfNotLessEqualIns(input);

                case OPCode.IfNLt:
                return new IfNotLessThanIns(input);

                case OPCode.IfStrictEq:
                return new IfStrictEqualIns(input);

                case OPCode.IfStrictNE:
                return new IfStrictNotEqualIns(input);

                case OPCode.IfTrue:
                return new IfTrueIns(input);

                case OPCode.Jump:
                return new JumpIns(input);
                #endregion

                #region Register Management
                case OPCode.DecLocal_i:
                return new DecLocalIIns(input);

                case OPCode.DecLocal:
                return new DecLocalIns(input);

                case OPCode.GetLocal_0:
                return new GetLocal0Ins();

                case OPCode.GetLocal_1:
                return new GetLocal1Ins();

                case OPCode.GetLocal_2:
                return new GetLocal2Ins();

                case OPCode.GetLocal_3:
                return new GetLocal3Ins();

                case OPCode.GetLocal:
                return new GetLocalIns(input);

                case OPCode.IncLocal_i:
                return new IncLocalIIns(input);

                case OPCode.IncLocal:
                return new IncLocalIns(input);

                case OPCode.Kill:
                return new KillIns(input);

                case OPCode.SetLocal_0:
                return new SetLocal0Ins();

                case OPCode.SetLocal_1:
                return new SetLocal1Ins();

                case OPCode.SetLocal_2:
                return new SetLocal2Ins();

                case OPCode.SetLocal_3:
                return new SetLocal3Ins();

                case OPCode.SetLocal:
                return new SetLocalIns(input);
                #endregion

                #region Stack Management
                case OPCode.PushByte:
                return new PushByteIns(input);

                case OPCode.PushDouble:
                return new PushDoubleIns(abc, input);

                case OPCode.PushFalse:
                return new PushFalseIns();

                case OPCode.PushInt:
                return new PushIntIns(abc, input);

                case OPCode.PushNan:
                return new PushNaNIns();

                case OPCode.PushNull:
                return new PushNullIns();

                case OPCode.PushShort:
                return new PushShortIns(input);

                case OPCode.PushString:
                return new PushStringIns(abc, input);

                case OPCode.PushTrue:
                return new PushTrueIns();

                case OPCode.PushUInt:
                return new PushUIntIns(abc, input);
                #endregion

                #region Type Conversion
                case OPCode.Coerce_a:
                return new CoerceAIns();

                case OPCode.Coerce:
                return new CoerceIns(abc, input);

                case OPCode.Coerce_s:
                return new CoerceSIns();

                case OPCode.Convert_b:
                return new ConvertBIns();

                case OPCode.Convert_d:
                return new ConvertDIns();

                case OPCode.Convert_i:
                return new ConvertIIns();

                case OPCode.Convert_o:
                return new ConvertOIns();

                case OPCode.Convert_s:
                return new ConvertSIns();

                case OPCode.Convert_u:
                return new ConvertUIns();
                #endregion

                #region Miscellaneous
                case OPCode.ApplyType:
                return new ApplyTypeIns(input);

                case OPCode.AsType:
                return new AsTypeIns(abc, input);

                case OPCode.AsTypeLate:
                return new AsTypeLateIns();

                case OPCode.Call:
                return new CallIns(input);

                case OPCode.CallMethod:
                return new CallMethodIns(abc, input);

                case OPCode.CallProperty:
                return new CallPropertyIns(abc, input);

                case OPCode.CallPropLex:
                return new CallPropLexIns(abc, input);

                case OPCode.CallPropVoid:
                return new CallPropVoidIns(abc, input);

                case OPCode.CallStatic:
                return new CallStaticIns(abc, input);

                case OPCode.CallSuper:
                return new CallSuperIns(abc, input);

                case OPCode.CallSuperVoid:
                return new CallSuperVoidIns(abc, input);

                case OPCode.CheckFilter:
                return new CheckFilterIns();

                case OPCode.Construct:
                return new ConstructIns(input);

                case OPCode.ConstructProp:
                return new ConstructPropIns(abc, input);

                case OPCode.ConstructSuper:
                return new ConstructSuperIns(input);

                case OPCode.DebugFile:
                return new DebugFileIns(abc, input);

                case OPCode.Debug:
                return new DebugIns(abc, input);

                case OPCode.DebugLine:
                return new DebugLineIns(input);

                case OPCode.DeleteProperty:
                return new DeletePropertyIns(abc, input);

                case OPCode.Dup:
                return new DupIns();

                case OPCode.Dxns:
                return new DxnsIns(abc, input);

                case OPCode.DxnsLate:
                return new DxnsLateIns();

                case OPCode.Esc_XElem:
                return new EscXElemIns();

                case OPCode.Esc_XAttr:
                return new EscXAttrIns();

                case OPCode.FindProperty:
                return new FindPropertyIns(abc, input);

                case OPCode.FindPropStrict:
                return new FindPropStrictIns(abc, input);

                case OPCode.GetDescendants:
                return new GetDescendantsIns(abc, input);

                case OPCode.GetGlobalScope:
                return new GetGlobalScopeIns();

                case OPCode.GetLex:
                return new GetLexIns(abc, input);

                case OPCode.GetProperty:
                return new GetPropertyIns(abc, input);

                case OPCode.GetScopeObject:
                return new GetScopeObjectIns(input);

                case OPCode.GetSlot:
                return new GetSlotIns(input);

                case OPCode.GetSuper:
                return new GetSuperIns(abc, input);

                case OPCode.HasNext2:
                return new HasNext2Ins(input);

                case OPCode.HasNext:
                return new HasNextIns();

                case OPCode.InitProperty:
                return new InitPropertyIns(abc, input);

                case OPCode.InstanceOf:
                return new InstanceOfIns();

                case OPCode.Label:
                return new LabelIns();

                case OPCode.LookUpSwitch:
                return new LookUpSwitchIns(input);

                case OPCode.NewActivation:
                return new NewActivationIns();

                case OPCode.NewArray:
                return new NewArrayIns(input);

                case OPCode.NewCatch:
                return new NewCatchIns(input);

                case OPCode.NewClass:
                return new NewClassIns(abc, input);

                case OPCode.NewFunction:
                return new NewFunctionIns(abc, input);

                case OPCode.NewObject:
                return new NewObjectIns(input);

                case OPCode.NextName:
                return new NextNameIns();

                case OPCode.NextValue:
                return new NextValueIns();

                case OPCode.Nop:
                return new NopIns();

                case OPCode.Not:
                return new NotIns();

                case OPCode.Pop:
                return new PopIns();

                case OPCode.PopScope:
                return new PopScopeIns();

                case OPCode.PushScope:
                return new PushScopeIns();

                case OPCode.PushUndefined:
                return new PushUndefinedIns();

                case OPCode.PushWith:
                return new PushWithIns();

                case OPCode.ReturnValue:
                return new ReturnValueIns();

                case OPCode.ReturnVoid:
                return new ReturnVoidIns();

                case OPCode.SetProperty:
                return new SetPropertyIns(abc, input);

                case OPCode.SetSlot:
                return new SetSlotIns(input);

                case OPCode.SetSuper:
                return new SetSuperIns(abc, input);

                case OPCode.Swap:
                return new SwapIns();

                case OPCode.Throw:
                return new ThrowIns();

                case OPCode.TypeOf:
                return new TypeOfIns();
                #endregion
            }
            throw new Exception("Unhandled OPCode: " + op);
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
}