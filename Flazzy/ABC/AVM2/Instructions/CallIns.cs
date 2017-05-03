using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class CallIns : ASInstruction
    {
        public int ArgCount { get; set; }

        public CallIns()
            : base(OPCode.Call)
        { }
        public CallIns(int argCount)
            : this()
        {
            ArgCount = argCount;
        }
        public CallIns(FlashReader input)
            : this()
        {
            ArgCount = input.ReadInt30();
        }

        public override int GetPopCount()
        {
            return (ArgCount + 2);
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
            object receiver = machine.Values.Pop();
            object function = machine.Values.Pop();
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(ArgCount);
        }
    }
}