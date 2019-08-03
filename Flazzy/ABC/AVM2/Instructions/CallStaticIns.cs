using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class CallStaticIns : ASInstruction
    {
        public int MethodIndex { get; set; }
        public ASMethod Method => ABC.Methods[MethodIndex];

        public int ArgCount { get; set; }

        public CallStaticIns(ABCFile abc)
            : base(OPCode.CallStatic, abc)
        { }
        public CallStaticIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            MethodIndex = input.ReadInt30();
            ArgCount = input.ReadInt30();
        }
        public CallStaticIns(ABCFile abc, int methodIndex)
            : this(abc)
        {
            MethodIndex = methodIndex;
        }
        public CallStaticIns(ABCFile abc, int methodIndex, int argCount)
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