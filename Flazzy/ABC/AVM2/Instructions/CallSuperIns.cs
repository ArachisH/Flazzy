using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class CallSuperIns : ASInstruction
    {
        public int MethodNameIndex { get; set; }
        public ASMultiname MethodName => ABC.Pool.Multinames[MethodNameIndex];

        public int ArgCount { get; set; }

        public CallSuperIns(ABCFile abc)
            : base(OPCode.CallSuper, abc)
        { }
        public CallSuperIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            MethodNameIndex = input.ReadInt30();
            ArgCount = input.ReadInt30();
        }
        public CallSuperIns(ABCFile abc, int methodNameIndex)
            : this(abc)
        {
            MethodNameIndex = methodNameIndex;
        }
        public CallSuperIns(ABCFile abc, int methodNameIndex, int argCount)
            : this(abc)
        {
            MethodNameIndex = methodNameIndex;
            ArgCount = argCount;
        }

        public override int GetPopCount()
        {
            return (ArgCount + ResolveMultinamePops(MethodName) + 1);
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            for (int i = 0; i < ArgCount; i++)
            {
                machine.Values.Pop();
            }
            ResolveMultiname(machine, MethodName);
            object receiver = machine.Values.Pop();
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(MethodNameIndex);
            output.WriteInt30(ArgCount);
        }
    }
}