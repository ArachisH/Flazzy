using Flazzy.IO;

namespace Flazzy
{
    public interface IFlashItem
    {
        int GetSize();
        void WriteTo(FlashWriter output);
    }
}