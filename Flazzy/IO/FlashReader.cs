using System.Text;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Flazzy.IO;

public unsafe ref struct FlashReader
{
    private readonly ReadOnlySpan<byte> _data;

    public int Position { get; set; }

    public readonly int Length => _data.Length;
    public readonly bool IsDataAvailable => Position < _data.Length;

    public FlashReader(ReadOnlySpan<byte> data)
    {
        _data = data;
        Position = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte() => _data[Position++];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref byte UnsafeReadByte() => ref Unsafe.Add(ref MemoryMarshal.GetReference(_data), Position++);

    public ReadOnlySpan<byte> ReadBytes(int count)
    {
        ReadOnlySpan<byte> data = _data.Slice(Position, count);
        Position += count;
        return data;
    }

    public void ReadBytes(Span<byte> buffer)
    {
        _data.Slice(Position, buffer.Length).CopyTo(buffer);
        Position += buffer.Length;
    }

    public short ReadInt16()
    {
        short value = BinaryPrimitives.ReadInt16LittleEndian(_data.Slice(Position));
        Position += sizeof(short);
        return value;
    }
    public ushort ReadUInt16()
    {
        ushort value = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(Position));
        Position += sizeof(ushort);
        return value;
    }

    public uint ReadUInt24()
    {
        uint value = (uint)((_data[Position + 2] << 16) + (_data[Position + 1] << 8) + _data[Position]);
        Position += 3;

        if ((value >> 23) == 1)
            value |= 0xff000000;

        return value;
    }

    public int ReadInt32()
    {
        int value = BinaryPrimitives.ReadInt32LittleEndian(_data.Slice(Position));
        Position += sizeof(int);
        return value;
    }
    public uint ReadUInt32()
    {
        uint value = BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(Position));
        Position += sizeof(uint);
        return value;
    }

    public ulong ReadUInt64()
    {
        ulong value = BinaryPrimitives.ReadUInt64LittleEndian(_data.Slice(Position));
        Position += sizeof(ulong);
        return value;
    }
    public double ReadDouble()
    {
        double value = BinaryPrimitives.ReadDoubleLittleEndian(_data.Slice(Position));
        Position += sizeof(double);
        return value;
    }

    public int ReadEncodedInt()
    {
        int result = UnsafeReadByte();
        if ((result & 0x00000080) == 0) return result;

        result = (result & 0x0000007f) | (UnsafeReadByte()) << 7;
        if ((result & 0x00004000) == 0) return result;

        result = (result & 0x00003fff) | (UnsafeReadByte()) << 14;
        if ((result & 0x00200000) == 0) return result;

        result = (result & 0x001fffff) | (UnsafeReadByte()) << 21;
        if ((result & 0x10000000) == 0) return result;

        return (result & 0x0fffffff) | (UnsafeReadByte()) << 28;
    }
    public uint ReadEncodedUInt()
    {
        return (uint)ReadEncodedInt(); //TODO: isn't this cast backwards
    }

    public string ReadString()
    {
        int length = ReadEncodedInt();
        return Encoding.UTF8.GetString(ReadBytes(length));
    }
    public string ReadString(int length)
    {
        return Encoding.UTF8.GetString(ReadBytes(length));
    }
    public string ReadNullString()
    {
        int length = _data.Slice(Position).IndexOf((byte)0);
        string value = Encoding.UTF8.GetString(_data.Slice(Position, length));

        Position += length + 1;
        return value;
    }
}
