using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class GetScopeObjectIns : Instruction
    {
        public byte ScopeIndex { get; set; }

        public GetScopeObjectIns()
            : base(OPCode.GetScopeObject)
        { }
        public GetScopeObjectIns(FlashReader input)
            : this()
        {
            ScopeIndex = input.ReadByte();
        }

        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            // TODO: Pop from Scope
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.Write(ScopeIndex);
        }
    }
}