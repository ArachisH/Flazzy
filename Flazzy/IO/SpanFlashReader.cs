using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flazzy.IO;

public ref struct SpanFlashReader
{
    private readonly ReadOnlySpan<byte> _data;

    public int Position { get; set; }

    public readonly int Length => _data.Length;
    public readonly bool IsDataAvailable => Position < _data.Length;

    public SpanFlashReader(ReadOnlySpan<byte> data)
    {
        _data = data;
        Position = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte() => _data[Position++];

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
        return (int)ReadEncodedUInt();
    }
    public uint ReadEncodedUInt()
    {
        uint result = ReadByte();
        if ((result & 0x80) == 0) return result;

        result = (result & 0x7F) | (uint)ReadByte() << 7;
        if ((result & 0x4000) == 0) return result;

        result = (result & 0x3FFF) | (uint)ReadByte() << 14;
        if ((result & 0x200000) == 0) return result;

        result = (result & 0x1FFFFF) | (uint)ReadByte() << 21;
        if ((result & 0x10000000) == 0) return result;

        return (result & 0xFFFFFFF) | (uint)ReadByte() << 28;
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