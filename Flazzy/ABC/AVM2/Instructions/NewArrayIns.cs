using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class NewArrayIns : ASInstruction
    {
        public int ArgCount { get; set; }

        public NewArrayIns()
            : base(OPCode.NewArray)
        { }
        public NewArrayIns(int argCount)
            : this()
        {
            ArgCount = argCount;
        }
        public NewArrayIns(FlashReader input)
            : this()
        {
            ArgCount = input.ReadInt30();
        }

        public override int GetPopCount()
        {
            return ArgCount;
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            var newarray = new object[ArgCount];
            for (int i = ArgCount - 1; i >= 0; i--)
            {
                newarray[i] = machine.Values.Pop();
            }
            machine.Values.Push(newarray);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(ArgCount);
        }
    }
}