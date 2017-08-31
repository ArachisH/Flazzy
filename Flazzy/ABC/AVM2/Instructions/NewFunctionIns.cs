using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class NewFunctionIns : ASInstruction
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
        public NewFunctionIns(ABCFile abc, FlashReader input)
            : this(abc)
        {
            MethodIndex = input.ReadInt30();
        }

        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(MethodIndex);
        }
    }
}