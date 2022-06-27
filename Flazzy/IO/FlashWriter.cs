using System.Text;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Flazzy.IO;

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

    public void Write(bool value) => Write(Unsafe.As<bool, byte>(ref value));
    public void Write(short value)
    {
        MemoryMarshal.Write(_data.Slice(Position), ref value);
        Position += sizeof(short);
    }
    public void Write(ushort value)
    {
        MemoryMarshal.Write(_data.Slice(Position), ref value);
        Position += sizeof(ushort);
    }
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
        Span<byte> slice = _data.Slice(Position, 3);
        slice[2] = (byte)(value >> 16 & 0xff);
        slice[1] = (byte)(value >> 8 & 0xff);
        slice[0] = (byte)(value & 0xff);

        Position += 3;
    }

    public void WriteEncodedInt(int value)
    {
        WriteEncodedUInt((uint)value);
    }
    public void WriteEncodedUInt(uint value)
    {
        while (value > 0x7Fu)
        {
            Write((byte)(value | ~0x7Fu));
            value >>= 7;
        }
        Write((byte)value);
    }

    public void WriteEncodedUInt2(uint value)
    {
        while (value > 0x7Fu)
        {
            Write((byte)(value | ~0x7Fu));
            value >>= 7;
        }
        Write((byte)value);
    }

    public void Write(ReadOnlySpan<byte> value)
    {
        value.CopyTo(_data.Slice(Position));
        Position += value.Length;
    }

    public ref uint ReserveUInt32()
    {
        // Reserve from memory and zero it
        ref uint result = ref Unsafe.As<byte, uint>(ref _data[0]);
        result = default;

        Position += sizeof(uint);
        return ref result;
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
        return (int)GetEncodedUIntSize((uint)value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetEncodedUIntSize(uint value)
    {
        // Avoid BitOperation software fallback
        if (Lzcnt.IsSupported || X86Base.IsSupported || ArmBase.IsSupported)
        {
            // bits_to_encode = (data != 0) ? 32 - CLZ(x) : 1  // 32 - CLZ(data | 1)
            // bytes = ceil(bits_to_encode / 7.0);             // (6 + bits_to_encode) / 7
            uint x = 6 + 32 - (uint)BitOperations.LeadingZeroCount(value | 1);
            // Division by 7 is done by (x * 37) >> 8 where 37 = ceil(256 / 7).
            // This works for 0 <= x < 256 / (7 * 37 - 256), i.e. 0 <= x <= 85.
            return (x * 37) >> 8;
        }
        else
        {
            if ((value & (~0U << 7)) == 0) return 1;
            if ((value & (~0U << 14)) == 0) return 2;
            if ((value & (~0U << 21)) == 0) return 3;
            if ((value & (~0U << 28)) == 0) return 4;
            return 5;
        }
    }

    public static void ThrowIndexOutOfRange() => throw new IndexOutOfRangeException();
}
