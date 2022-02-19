using Flazzy.IO;

namespace Flazzy.Records
{
    public class FrameRecord : IFlashItem
    {
        public ushort Rate { get; set; }
        public ushort Count { get; set; }
        public RectangeRecord Area { get; set; }

        public FrameRecord()
        {
            Area = new RectangeRecord();
        }
        public FrameRecord(ref FlashReader input)
        {
            Area = new RectangeRecord(ref input);
            Rate = (ushort)(input.ReadUInt16() >> 8);
            Count = input.ReadUInt16();
        }

        public int GetSize()
        {
            int size = 0;
            size += Area.GetSize();
            size += sizeof(ushort);
            size += sizeof(ushort);
            return size;
        }
        public void WriteTo(ref FlashWriter output)
        {
            Area.WriteTo(ref output);
            output.Write((ushort)(Rate << 8));
            output.Write(Count);
        }
    }
}