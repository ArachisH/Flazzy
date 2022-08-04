using Flazzy.IO;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class LookUpSwitchIns : ASInstruction
    {
        public List<uint> CaseOffsets { get; }
        public uint DefaultOffset { get; set; }

        public LookUpSwitchIns()
            : base(OPCode.LookUpSwitch)
        {
            CaseOffsets = new List<uint>();
        }
        public LookUpSwitchIns(FlashReader input)
            : this()
        {
            DefaultOffset = input.ReadUInt24();
            CaseOffsets.Capacity = (input.ReadInt30() + 1);
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

        public override int GetPopCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            machine.Values.Pop();
        }

        protected override void WriteValuesTo(FlashWriter output)
        {
            output.WriteUInt24(DefaultOffset);
            output.WriteInt30(CaseOffsets.Count - 1);
            for (int i = 0; i < CaseOffsets.Count; i++)
            {
                uint offset = CaseOffsets[i];
                output.WriteUInt24(offset);
            }
        }
    }
}