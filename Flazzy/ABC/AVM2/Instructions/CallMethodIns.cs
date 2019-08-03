using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class CallMethodIns : ASInstruction
    {
        public int MethodIndex { get; set; }
        public int ArgCount { get; set; }

        public CallMethodIns(ABCFile abc)
            : base(OPCode.CallMethod, abc)
        { }
        public CallMethodIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            MethodIndex = input.ReadInt30();
            ArgCount = input.ReadInt30();
        }
        public CallMethodIns(ABCFile abc, int methodIndex)
            : this(abc)
        {
            MethodIndex = methodIndex;
        }
        public CallMethodIns(ABCFile abc, int methodIndex, int argCount)
            : this(abc)
        {
            MethodIndex = methodIndex;
            ArgCount = argCount;
        }

        public override int GetPopCount()
        {
            return ArgCount + 1;
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
            machine.Values.Pop(); // Receiver
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(MethodIndex);
            output.WriteInt30(ArgCount);
        }
    }
}