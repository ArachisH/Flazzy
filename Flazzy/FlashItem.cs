using System.Diagnostics;

using Flazzy.IO;

namespace Flazzy
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class FlashItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual string DebuggerDisplay => "{" + ToString() + "}";

        public byte[] ToArray()
        {
            using (var outputMem = new MemoryStream())
            using (var output = new FlashWriter(outputMem))
            {
                WriteTo(output);
                return outputMem.ToArray();
            }
        }
        public abstract void WriteTo(FlashWriter output);
    }
}