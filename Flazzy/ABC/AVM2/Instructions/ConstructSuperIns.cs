using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class ConstructSuperIns : ASInstruction
    {
        public int ArgCount { get; set; }

        public ConstructSuperIns()
            : base(OPCode.ConstructSuper)
        { }
        public ConstructSuperIns(int argCount)
            : this()
        {
            ArgCount = argCount;
        }
        public ConstructSuperIns(ref FlashReader input)
            : this()
        {
            ArgCount = input.ReadEncodedInt();
        }

        public override int GetPopCount()
        {
            return (ArgCount + 1);
        }
        public override void Execute(ASMachine machine)
        {
            for (int i = 0; i < ArgCount; i++)
            {
                machine.Values.Pop();
            }
            object obj = machine.Values.Pop();
        }

        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(ArgCount);
        }
    }
}