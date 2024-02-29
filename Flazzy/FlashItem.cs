using System.Diagnostics;

using Flazzy.IO;

namespace Flazzy;

// TODO: Remove abstract FlashItem and move to composition with IFlashItem
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

/// <summary>
/// Represents a serializable structure in the Shockwave Flash file.
/// </summary>
public interface IFlashItem
{
    int GetSize();
    void WriteTo(ref SpanFlashWriter output);
}