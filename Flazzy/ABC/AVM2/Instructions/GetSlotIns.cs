using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class GetSlotIns : ASInstruction
    {
        public int SlotIndex { get; set; }

        public GetSlotIns()
            : base(OPCode.GetSlot)
        { }
        public GetSlotIns(int slotIndex)
            : this()
        {
            SlotIndex = slotIndex;
        }
        public GetSlotIns(ref FlashReader input)
            : this()
        {
            SlotIndex = input.ReadEncodedInt();
        }

        public override int GetPopCount() => 1;
        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            object obj = machine.Values.Pop();
            machine.Values.Push(null);
        }

        protected override int GetBodySize()
        {
            return FlashWriter.GetEncodedIntSize(SlotIndex);
        }
        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(SlotIndex);
        }
    }
}