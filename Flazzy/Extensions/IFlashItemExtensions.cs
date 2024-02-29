using System.Buffers;
using System.Runtime.CompilerServices;

using Flazzy.IO;

namespace Flazzy;

public static class IFlashItemExtensions
{
    [SkipLocalsInit]
    public static void WriteTo(this IFlashItem item, Stream output)
    {
        const int StackallocThreshold = 512;

        int size = item.GetSize();

        byte[] rentedBuffer = null;
        Span<byte> buffer = size <= StackallocThreshold 
            ? stackalloc byte[StackallocThreshold] : 
            (rentedBuffer = ArrayPool<byte>.Shared.Rent(size));

        buffer = buffer.Slice(0, size);
        buffer.Clear();

        var writer = new SpanFlashWriter(buffer);
        item.WriteTo(ref writer);

        try
        {
            output.Write(buffer);
        }
        finally
        {
            if (rentedBuffer is not null) 
                ArrayPool<byte>.Shared.Return(rentedBuffer);
        }
    }
    public static void WriteTo(this IFlashItem item, IBufferWriter<byte> output)
    {
        int size = item.GetSize();
        var writer = new SpanFlashWriter(output.GetSpan(size).Slice(0, size));
        item.WriteTo(ref writer);

        output.Advance(size);
    }

    public static byte[] ToArray(this IFlashItem item)
    {
        byte[] buffer = new byte[item.GetSize()];
        var writer = new SpanFlashWriter(buffer);
        item.WriteTo(ref writer);
        
        return buffer;
    }
}
