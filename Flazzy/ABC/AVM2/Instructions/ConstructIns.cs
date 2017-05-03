using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class ConstructIns : ASInstruction
    {
        public int ArgCount { get; set; }

        public ConstructIns()
            : base(OPCode.Construct)
        { }
        public ConstructIns(int argCount)
            : this()
        {
            ArgCount = argCount;
        }
        public ConstructIns(FlashReader input)
            : this()
        {
            ArgCount = input.ReadInt30();
        }

        public override int GetPopCount()
        {
            return (ArgCount + 1);
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
            object obj = machine.Values.Pop();
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(ArgCount);
        }
    }
}