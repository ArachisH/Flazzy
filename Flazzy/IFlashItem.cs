using System.Buffers;

using Flazzy.IO;

namespace Flazzy;

public interface IFlashItem
{
    int GetSize();
    void WriteTo(ref FlashWriter output);

    public void WriteTo(IBufferWriter<byte> output)
    {
        int size = GetSize();
        var writer = new FlashWriter(output.GetSpan(size));
        WriteTo(ref writer);

        output.Advance(size);
    }
}