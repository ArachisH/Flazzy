﻿using Flazzy.IO;
using Flazzy.Tags;
using System.Runtime.InteropServices;

namespace Flazzy
{
    internal static class FlashTools
    {
        public static ImageFormat GetImageFormat(ReadOnlySpan<byte> data)
        {
            if (MemoryMarshal.Read<int>(data) == -654321153 || MemoryMarshal.Read<short>(data) == -9985)
                return ImageFormat.JPEG;
            
            if (MemoryMarshal.Read<int>(data) == 944130375 && MemoryMarshal.Read<short>(data) == 24889) 
                return ImageFormat.GIF98a;
            
            if (MemoryMarshal.Read<long>(data) == 727905341920923785)
                return ImageFormat.PNG;
            
            throw new ArgumentException("Provided data contains an unknown image format.");
        }

        public static int[] GetMaxPaddedBitsNeeded(out int maxBits, params int[] values)
        {
            maxBits = 0;
            var fixedValues = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                int value = values[i];
                if (value > 0x3FFFFFFF)
                {
                    value = 0x3FFFFFFF;
                }
                else if (value < -0x3FFFFFFF)
                {
                    value = -0x3FFFFFFF;
                }

                fixedValues[i] = value;
                int neededBits = BitWriter.CountSBits(value);
                if (neededBits > maxBits)
                {
                    maxBits = neededBits;
                }
            }
            return fixedValues;
        }
    }
}