using Flazzy.IO;

namespace Flazzy
{
    // TODO: Would a TryWriteTo(Span<byte> dest, out int bytesWritten) + A "maximum" size allocation hint be better?
    // This size hint would return faster calculated size hint (no branches in encoded int size calculation)
    public interface IFlashItem
    {
        int GetSize();
        void WriteTo(ref FlashWriter output);
    }
}