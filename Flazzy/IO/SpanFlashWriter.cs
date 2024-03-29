﻿using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Flazzy.IO;

public ref struct SpanFlashWriter
{
    private readonly Span<byte> _data;

    public int Position { get; set; }

    public SpanFlashWriter(Span<byte> data)
    {
        _data = data;
        Position = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte value) => _data[Position++] = value;

    public void Write(bool value) => Write((byte)(value ? 1 : 0));
    public void Write(short value)
    {
        BinaryPrimitives.WriteInt16LittleEndian(_data.Slice(Position), value);
        Position += sizeof(short);
    }
    public void Write(ushort value)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(_data.Slice(Position), value);
        Position += sizeof(ushort);
    }
    public void Write(int value)
    {
        BinaryPrimitives.WriteInt32LittleEndian(_data.Slice(Position), value);
        Position += sizeof(int);
    }
    public void Write(uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(_data.Slice(Position), value);
        Position += sizeof(uint);
    }
    public void Write(ulong value)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(_data.Slice(Position), value);
        Position += sizeof(ulong);
    }
    public void Write(double value)
    {
        BinaryPrimitives.WriteDoubleLittleEndian(_data.Slice(Position), value);
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

    public void Write(ReadOnlySpan<byte> value)
    {
        value.CopyTo(_data.Slice(Position));
        Position += value.Length;
    }

    internal void WriteDoubleArray(ReadOnlySpan<double> values)
    {
        // TODO: Investigate if this fails on platforms that don't support unaligned reads/writes.. ARM?
        if (BitConverter.IsLittleEndian)
        {
            // Because we are on little-endian platform, we can just blit the values.
            MemoryMarshal.AsBytes(values).CopyTo(_data.Slice(Position));
        }
        else
        {
            BinaryPrimitives.ReverseEndianness(
                source: MemoryMarshal.Cast<double, ulong>(values), 
                destination: MemoryMarshal.Cast<byte, ulong>(_data));
        }

        Position += values.Length * sizeof(double);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowOutOfRange() => throw new IndexOutOfRangeException();
    }

    public void WriteString(ReadOnlySpan<char> value)
    {
        // TODO: UTF8 "Encoding" 2x vs. UTF8.GetMaxByteCount -> stackalloc/rent -> encoding
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetEncodedIntSize(int value)
    {
        return GetEncodedUIntSize((uint)value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetEncodedUIntSize(uint value)
    {
        // bits = (value != 0) ? 32 - CLZ(value) : 1  <=>  32 - CLZ(value | 1) 
        // bytes = ceil(bits / 7.0);                  <=>  (6 + bits) / 7
        int x = 6 + 32 - BitOperations.LeadingZeroCount(value | 1);
        // Division by 7 is done by (x * 37) >> 8 where 37 = ceil(256 / 7).
        // This works for 0 <= x < 256 / (7 * 37 - 256), i.e. 0 <= x <= 85.
        return (x * 37) >> 8;
    }
}