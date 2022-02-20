using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class NewFunctionIns : ASInstruction
    {
        public int MethodIndex { get; set; }
        public ASMethod Method => ABC.Methods[MethodIndex];

        public NewFunctionIns(ABCFile abc)
            : base(OPCode.NewFunction, abc)
        { }
        public NewFunctionIns(ABCFile abc, int methodIndex)
            : this(abc)
        {
            MethodIndex = methodIndex;
        }
        public NewFunctionIns(ABCFile abc, ref FlashReader input)
            : this(abc)
        {
            MethodIndex = input.ReadEncodedInt();
        }

        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(null);
        }

        protected override int GetBodySize()
        {
            return FlashWriter.GetEncodedIntSize(MethodIndex);
        }
        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(MethodIndex);
        }
    }
}