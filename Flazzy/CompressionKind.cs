namespace Flazzy
{
    public enum CompressionKind
    {
        /// <summary>
        /// Represents no compression.
        /// </summary>
        None = 0x46,
        /// <summary>
        /// Represents ZLIB compression. (SWF +6)
        /// </summary>
        ZLIB = 0x43,
        /// <summary>
        /// Represents LZMA compression. (SWF +13)
        /// </summary>
        LZMA = 0x5A
    }
}