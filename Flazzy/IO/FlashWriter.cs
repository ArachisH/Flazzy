using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Flazzy.IO
{
    public ref struct FlashWriter
    {
        private readonly Span<byte> _data;

        public int Position { get; set; }

        public FlashWriter(Span<byte> data)
        {
            _data = data;
            Position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte value) => _data[Position++] = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsafeWrite(byte value) => Unsafe.Add(ref MemoryMarshal.GetReference(_data), Position++) = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(bool value) => UnsafeWrite(Unsafe.As<bool, byte>(ref value));
        public void Write(int value)
        {
            MemoryMarshal.Write(_data.Slice(Position), ref value);
            Position += sizeof(int);
        }
        public void Write(uint value)
        {
            MemoryMarshal.Write(_data.Slice(Position), ref value);
            Position += sizeof(uint);
        }
        public void Write(ulong value)
        {
            MemoryMarshal.Write(_data.Slice(Position), ref value);
            Position += sizeof(ulong);   
        }
        public void Write(double value)
        {
            MemoryMarshal.Write(_data.Slice(Position), ref value);
            Position += sizeof(double);
        }
        public void WriteUInt24(uint value)
        {
            var byteValue = (byte)(value & 0xff);
            UnsafeWrite(byteValue);

            value >>= 8;

            byteValue = (byte)(value & 0xff);
            UnsafeWrite(byteValue);

            value >>= 8;

            byteValue = (byte)(value & 0xff);
            UnsafeWrite(byteValue);
        }

        public void WriteEncodedInt(int value)
        {
            WriteEncodedUInt((uint)value);
        }
        public void WriteEncodedUInt(uint value)
        {
            while (value > 0x7Fu)
            {
                UnsafeWrite((byte)(value | ~0x7Fu));
                value >>= 7;
            }
            UnsafeWrite((byte)value);
        }

        public void Write(ReadOnlySpan<byte> value)
        {
            value.CopyTo(_data.Slice(Position));
            Position += value.Length;
        }

        public void WriteString(ReadOnlySpan<char> value)
        {
            WriteEncodedInt(Encoding.UTF8.GetByteCount(value));

            int len = Encoding.UTF8.GetBytes(value, _data.Slice(Position));
            Position += len;
        }
        public void WriteNullString(ReadOnlySpan<char> value)
        {
            int len = Encoding.UTF8.GetBytes(value, _data.Slice(Position));
            _data[Position + len] = 0;

            Position += len + 1;
        }

        public static int GetEncodedIntSize(int value)
        {
            return GetEncodedUIntSize((uint)value);
        }
        public static int GetEncodedUIntSize(uint value)
        {
            // TODO: Research if we can turn this branchless
            if (value < 0x80) return 1;
            if (value < 0x4000) return 2;
            if (value < 0x200000) return 3;
            if (value < 0x10000000) return 4;
            return 5;
        }
    }
}