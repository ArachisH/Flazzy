using System.IO.Compression;

namespace Flazzy.IO.Compression;

internal unsafe static class ZLib
{
    /// <summary>
    /// Decompresses provided input data into output buffer.<para/>
    /// SAFETY: The length of decompressed <paramref name="output"/> buffer MUST be known and allocated beforehand.
    /// </summary>
    internal static unsafe int Decompress(ReadOnlySpan<byte> input, Span<byte> output)
    {
        fixed (byte* inputPtr = &input[0])
        {
            using var stream = new UnmanagedMemoryStream(inputPtr, input.Length);
            using var zlibStream = new ZLibStream(stream, CompressionMode.Decompress);

            int totalRead = 0;
            while (totalRead < output.Length)
            {
                int bytesRead = zlibStream.Read(output.Slice(totalRead));
                if (bytesRead == 0) break;
                totalRead += bytesRead;
            }
            return totalRead;
        }
    }

    internal static byte[] Compress(ReadOnlySpan<byte> input)
    {
        using var stream = new MemoryStream(input.Length);
        using var zlibStream = new ZLibStream(stream, CompressionMode.Compress);
        zlibStream.Write(input);
        zlibStream.Flush();

        return stream.ToArray();
    }
}
