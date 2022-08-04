using System.Text;

namespace Flazzy.IO
{
    public class FlashWriter : BinaryWriter
    {
        private readonly bool _leaveOpen;

        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }
        public long Length => BaseStream.Length;

        protected int BitPosition { get; set; }
        protected int BitContainer { get; set; }

        public FlashWriter()
            : this(0)
        { }
        public FlashWriter(byte[] data)
            : this(data.Length)
        {
            Write(data, 0, data.Length);
        }
        public FlashWriter(int capacity)
            : this(new MemoryStream(capacity))
        { }

        public FlashWriter(Stream output)
            : this(output, new UTF8Encoding(false, true), false)
        { }
        public FlashWriter(Stream output, bool leaveOpen)
            : this(output, new UTF8Encoding(false, true), leaveOpen)
        { }
        public FlashWriter(Stream output, Encoding encoding)
            : this(output, encoding, false)
        { }
        public FlashWriter(Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding)
        {
            _leaveOpen = leaveOpen;
        }

        public void WriteItem(FlashItem item)
        {
            item.WriteTo(this);
        }

        public void WriteInt30(int value)
        {
            Align();
            Write7BitEncodedInt(value);
        }
        public void WriteUInt24(uint value)
        {
            Align();

            var byteValue = (byte)(value & 0xff);
            Write(byteValue);

            value >>= 8;

            byteValue = (byte)(value & 0xff);
            Write(byteValue);

            value >>= 8;

            byteValue = (byte)(value & 0xff);
            Write(byteValue);
        }
        public void WriteUInt30(uint value)
        {
            Align();
            Write7BitEncodedInt((int)value);
        }
        public void WriteBits(int maxBits, long value)
        {
            for (int i = 0; i < maxBits; i++)
            {
                int bit = (int)((value >> ((maxBits - 1) - i)) & 1);

                BitContainer += (bit * (1 << (7 - BitPosition)));
                if (++BitPosition == 8)
                {
                    base.Write((byte)BitContainer);

                    BitPosition = 0;
                    BitContainer = 0;
                }
            }
        }

        public void WriteNullString(string value)
        {
            Write(value.ToCharArray());
            Write('\0');
        }
        public void Write(string value, bool excludeLength)
        {
            if (excludeLength)
            {
                Write(value.ToCharArray());
            }
            else
            {
                Write(value);
            }
        }

        #region Write Overrides
        public override void Write(bool value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(byte value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(byte[] buffer)
        {
            Align();
            base.Write(buffer);
        }
        public override void Write(byte[] buffer, int index, int count)
        {
            Align();
            base.Write(buffer, index, count);
        }
        public override void Write(char ch)
        {
            Align();
            base.Write(ch);
        }
        public override void Write(char[] chars)
        {
            Align();
            base.Write(chars);
        }
        public override void Write(char[] chars, int index, int count)
        {
            Align();
            base.Write(chars, index, count);
        }
        public override void Write(decimal value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(double value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(float value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(int value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(long value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(sbyte value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(short value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(string value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(uint value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(ulong value)
        {
            Align();
            base.Write(value);
        }
        public override void Write(ushort value)
        {
            Align();
            base.Write(value);
        }
        #endregion

        protected void Align()
        {
            if (BitPosition > 0)
            {
                base.Write((byte)BitContainer);

                BitPosition = 0;
                BitContainer = 0;
            }
        }
        public override void Flush()
        {
            Align();
            base.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_leaveOpen)
                {
                    OutStream.Flush();
                }
                else
                {
                    OutStream.Dispose();
                }
            }
        }
    }
}