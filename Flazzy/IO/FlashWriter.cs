using System.Text;
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

    public static void ThrowIndexOutOfRange() => throw new IndexOutOfRangeException();
}
