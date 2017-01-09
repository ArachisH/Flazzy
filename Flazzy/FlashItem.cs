using Flazzy.IO;

using System.Diagnostics;

namespace Flazzy
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class FlashItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual string DebuggerDisplay
        {
            get { return "{" + ToString() + "}"; }
        }

        public abstract void WriteTo(FlashWriter output);
    }
}