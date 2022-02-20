using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public sealed class LookUpSwitchIns : ASInstruction
    {
        public List<uint> CaseOffsets { get; }
        public uint DefaultOffset { get; set; }

        public LookUpSwitchIns()
            : base(OPCode.LookUpSwitch)
        {
            CaseOffsets = new List<uint>();
        }
        public LookUpSwitchIns(ref FlashReader input)
            : this()
        {
            DefaultOffset = input.ReadUInt24();
            CaseOffsets.Capacity = input.ReadEncodedInt() + 1;
            for (int i = 0; i < CaseOffsets.Capacity; i++)
            {
                CaseOffsets.Add(input.ReadUInt24());
            }
        }
        public LookUpSwitchIns(uint defaultOffset, params uint[] caseOffsets)
            : this()
        {
            DefaultOffset = defaultOffset;

            CaseOffsets.AddRange(caseOffsets);
            CaseOffsets.Add(DefaultOffset);
        }

        public override int GetPopCount() => 1;
        public override void Execute(ASMachine machine)
        {
            machine.Values.Pop();
        }

        protected override int GetBodySize()
        {
            int size = 0;
            size += 3;
            size += FlashWriter.GetEncodedIntSize(CaseOffsets.Count - 1);
            size += CaseOffsets.Count * 3;
            return size;
        }
        protected override void WriteValuesTo(ref FlashWriter output)
        {
            output.WriteUInt24(DefaultOffset);
            output.WriteEncodedInt(CaseOffsets.Count - 1);
            for (int i = 0; i < CaseOffsets.Count; i++)
            {
                output.WriteUInt24(CaseOffsets[i]);
            }
        }
    }
}