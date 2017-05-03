using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class GetSlotIns : ASInstruction
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
        public GetSlotIns(FlashReader input)
            : this()
        {
            SlotIndex = input.ReadInt30();
        }

        public override int GetPopCount()
        {
            return 1;
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            object obj = machine.Values.Pop();
            machine.Values.Push(null);
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteInt30(SlotIndex);
        }
    }
}