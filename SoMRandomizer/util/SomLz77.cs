using System;
using System.Collections.Generic;

namespace SoMRandomizer.util
{
    /// <summary>
    /// Encode/decode utility for SoM-specific LZ77 compression.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SomLz77
    {
        // decode an LZ77 compressed block from the ROM
        public static List<byte> decodeSomLz77(byte[] rom, int location)
        {
            List<byte> ret = new List<byte>();
            int windowSize = rom[location];
            location += 2;
            // big endian for whatever reason
            int size = (rom[location] << 8) + rom[location + 1];
            location += 2;
            int pos = 0;
            while (pos < size)
            {
                byte b = rom[location++];
                if ((b & 0x80) > 0)
                {
                    // seek back and read ram data
                    byte mask1 = 0x1F;
                    for (int i = 0; i < windowSize; i++)
                    {
                        mask1 >>= 1;
                    }
                    byte distMsb = (byte)(b & mask1);
                    byte numBytes = (byte)(((b & 0x7F) >> (5 - windowSize)) + 3);
                    ushort dist = (ushort)(rom[location++] + 1 + (distMsb * 256));
                    for (int i = 0; pos < size && i < numBytes; i++)
                    {
                        ret.Add(ret[pos - dist]);
                        pos++;
                    }
                }
                else
                {
                    // read rom data
                    for (int i = 0; pos < size && i <= b; i++)
                    {
                        ret.Add(rom[location++]);
                        pos++;
                    }
                }
            }

            return ret;
        }

        // encode a block with LZ77
        public static List<byte> encodeSomLz77(byte[] decompressedBlock, int windowSize)
        {
            // TODO: add test that decompresses and recompresses the stuff from ROM and assert it's identical
            List<byte> compressedBlock = new List<byte>();
            compressedBlock.Add((byte)windowSize);
            compressedBlock.Add(0);
            // big endian for some reason
            compressedBlock.Add((byte)(decompressedBlock.Length >> 8));
            compressedBlock.Add((byte)decompressedBlock.Length);

            int decompIndex = 0;
            int romBytes = 0;
            int maxLenComp = (int)Math.Pow(2, windowSize + 2) + 2;
            int maxDistComp = (int)Math.Pow(2, 13 - windowSize);

            while (decompIndex < decompressedBlock.Length)
            {
                // find the longest match for compression
                int bestMatchLen = 0; // less than 3 means no usable match found
                int bestMatchDist = 0;
                // we can only seek back maxDist and copy at most maxLenComp bytes, but not less than 3
                int maxLen = Math.Min(maxLenComp, decompressedBlock.Length - decompIndex);
                int maxDist = Math.Min(maxDistComp, decompIndex);
                for (int dist = 1; dist <= maxDist; dist++)
                {
                    int pos = decompIndex - dist;
                    for (int i = 0; i < maxLen; i++)
                    {
                        if (decompressedBlock[pos + i] != decompressedBlock[decompIndex + i])
                        {
                            if (i > bestMatchLen)
                            {
                                bestMatchLen = i;
                                bestMatchDist = dist;
                            }
                            break;
                        }
                        if (i == maxLen - 1)
                        {
                            bestMatchLen = i + 1;
                            bestMatchDist = dist;
                        }
                    }

                    if (bestMatchLen == maxLen)
                        break;
                }

                // was a usable match found?
                if (bestMatchLen >= 3)
                {
                    int len = bestMatchLen;
                    int dist = bestMatchDist;

                    // write buffered uncompressed bytes, if any
                    if (romBytes > 0)
                    {
                        compressedBlock.Add((byte)(romBytes - 1));
                        for (int i = 0; i < romBytes; i++)
                        {
                            byte thisByte = decompressedBlock[(decompIndex) - (romBytes - i)];
                            compressedBlock.Add(thisByte);
                        }
                    }
                    romBytes = 0;

                    // write compression marker
                    byte ctrlByte = 0x80;
                    byte lengthField = (byte)((len - 3) << (5 - windowSize));
                    ctrlByte |= lengthField;
                    byte distanceMsbPortion = (byte)(((dist - 1)) >> 8);
                    ctrlByte |= distanceMsbPortion;

                    byte distanceLsbByte = (byte)(dist - 1);

                    compressedBlock.Add(ctrlByte);
                    compressedBlock.Add(distanceLsbByte);
                    decompIndex += len;
                }
                else
                {
                    // append to uncompressed buffer
                    romBytes++;
                    decompIndex++;
                }

                // hit the max uncompressed block size?
                if (romBytes == 128)
                {
                    // write out
                    compressedBlock.Add((byte)(romBytes - 1));
                    for (int i = 0; i < romBytes; i++)
                    {
                        compressedBlock.Add(decompressedBlock[(decompIndex) - (romBytes - i)]);
                    }

                    romBytes = 0;
                }
            }

            // write remaining uncompressed bytes, if any
            if (romBytes > 0)
            {
                compressedBlock.Add((byte)(romBytes - 1));
                for (int i = 0; i < romBytes; i++)
                {
                    compressedBlock.Add(decompressedBlock[(decompIndex) - (romBytes - i)]);
                }
            }

            return compressedBlock;
        }
    }
}
