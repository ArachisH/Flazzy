namespace Flazzy
{
    public enum CompressionKind
    {
        /// <summary>
        /// Represents no compression.
        /// </summary>
        None = 0x46,
        /// <summary>
        /// Represents ZLib compression. (SWF +6)
        /// </summary>
        ZLib = 0x43,
        /// <summary>
        /// Represents LZMA compression. (SWF +13)
        /// </summary>
        LZMA = 0x5A
    }
}