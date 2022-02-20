using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class NewObjectIns : ASInstruction
    {
        public int ArgCount { get; set; }

        public NewObjectIns()
            : base(OPCode.NewObject)
        { }
        public NewObjectIns(int argCount)
            : this()
        {
            ArgCount = argCount;
        }
        public NewObjectIns(ref FlashReader input)
            : this()
        {
            ArgCount = input.ReadEncodedInt();
        }

        public override int GetPopCount() => ArgCount * 2;
        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            for (int i = 0; i < ArgCount; i++)
            {
                object value = machine.Values.Pop();
                object name = machine.Values.Pop();
            }
            machine.Values.Push(null);
        }

        protected override int GetBodySize()
        {
            return FlashWriter.GetEncodedIntSize(ArgCount);
        }
        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(ArgCount);
        }
    }
}