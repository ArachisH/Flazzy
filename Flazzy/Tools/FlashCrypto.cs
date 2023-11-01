using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Flazzy.Tools;

public static class FlashCrypto
{
    private static ReadOnlySpan<byte> SecondaryKey => " EncryptSWF "u8;
    private static ReadOnlySpan<byte> GlobalKey => "Adobe AIR SDK (c) 2021 HARMAN Internation Industries Incorporated"u8;

    public static int Decrypt(ref Span<byte> buffer, out int writtenOffset)
    {
        buffer[0] -= 32; // To Upper 'c' > 'C'

        Span<byte> aesIV = stackalloc byte[16];
        buffer.Slice(0, 12).CopyTo(aesIV); // Header Field + Encrypted Length Field

        uint key = GetKey(aesIV.Slice(0, 8));
        int encryptedLength = (int)(MemoryMarshal.Read<uint>(aesIV.Slice(8, 4)) ^ key);

        encryptedLength += 31;
        encryptedLength &= -32; // Padding Adjustment

        MemoryMarshal.Write(aesIV.Slice(12), ref key); // Place key into the last four bytes of the AES IV.
        for (int i = 0; i < 16; i++)
        {
            aesIV[i] ^= GlobalKey[i];
        }

        Span<byte> aesKey = stackalloc byte[32];
        buffer.Slice(12 + encryptedLength).CopyTo(aesKey); // Skip Header Field & Encrypted Length Field & Encrypted Body

        Span<uint> aesKeyChunks = MemoryMarshal.Cast<byte, uint>(aesKey);
        for (int i = 0; i < aesKeyChunks.Length; i++)
        {
            if ((i & 1) == 1)
            {
                aesKeyChunks[i] -= key;
            }
            else aesKeyChunks[i] += key;
        }

        using var aes = Aes.Create();
        aes.Key = aesKey.ToArray();

        // Moving the header up by four bytes, so we can avoid having to deal with the gap that a trimmed 'Encrypted Length' field would bring.
        writtenOffset = 4;
        buffer.Slice(0, 8).CopyTo(buffer.Slice(4));
        buffer = buffer.Slice(4, 8 + encryptedLength);

        // Ensure the slices do not overlap, as it would cause the internal Aes implementation to rent(which may allocate) a buffer to place the decrypted data into.
        return aes.DecryptCbc(buffer.Slice(8, encryptedLength), aesIV, buffer.Slice(8), PaddingMode.None) + 8;
    }

    private static uint GetKey(ReadOnlySpan<byte> headerBytes)
    {
        int dSum = 0;
        for (int i = 0; i < headerBytes.Length; i++)
        {
            dSum += headerBytes[i];
        }

        int dMod = dSum % GlobalKey.Length;
        Span<byte> buffer = stackalloc byte[GlobalKey.Length];

        GlobalKey[dMod..].CopyTo(buffer);
        GlobalKey[..dMod].CopyTo(buffer.Slice(GlobalKey.Length - dMod, dMod));

        uint key = 0;
        for (int i = 0; i < buffer.Length; i++)
        {
            key *= 31;
            key += buffer[i];
        }

        Span<char> dSumChars = stackalloc char[4]; // Max Digits (255 * 8 = 2,040)
        dSum.TryFormat(dSumChars, out int charsWritten);

        SecondaryKey.CopyTo(buffer);
        int encoded = SecondaryKey.Length;

        encoded += Encoding.Default.GetBytes(dSumChars.Slice(0, charsWritten), buffer.Slice(encoded));
        for (int i = 0; i < encoded; i++)
        {
            key *= 31;
            key += buffer[i];
        }

        return key & uint.MaxValue;
    }
}