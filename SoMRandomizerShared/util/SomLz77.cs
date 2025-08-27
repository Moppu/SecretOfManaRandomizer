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
            int seekback = rom[location];
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
                    for (int i = 0; i < seekback; i++)
                    {
                        mask1 >>= 1;
                    }
                    byte distMsb = (byte)(b & mask1);
                    byte numBytes = (byte)(((b & 0x7F) >> (5 - seekback)) + 3);
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
        public static List<byte> encodeSomLz77(byte[] decompressedBlock, int seekback)
        {
            List<byte> compressedBlock = new List<byte>();
            compressedBlock.Add((byte)seekback);
            compressedBlock.Add(0);
            // big endian for some reason
            compressedBlock.Add((byte)(decompressedBlock.Length >> 8));
            compressedBlock.Add((byte)decompressedBlock.Length);

            int decompIndex = 0;
            int romBytes = 0;
            int max_S = (int)Math.Pow(2, seekback + 2);
            int max_Dist = (int)Math.Pow(2, 13 - seekback);
            while (decompIndex < decompressedBlock.Length)
            {
                bool foundRamString = false;
                // search for longest RAM string we can generate
                int s = max_S + 2;
                int dist = 1;

                // maybe make dist the outer loop.
                for (s = max_S + 2; !foundRamString && s > 3; s--) // string length
                {
                    // seekback distance
                    for (dist = 1; !foundRamString && dist <= max_Dist; dist++)
                    {
                        if (decompIndex - dist >= 0)
                        {
                            bool ok = true;
                            int thisLoc = 0;
                            for (int pos = decompIndex - dist; ok && thisLoc < s; pos++)
                            {
                                if (decompIndex + thisLoc >= decompressedBlock.Length || decompressedBlock[pos] != decompressedBlock[decompIndex + thisLoc])
                                {
                                    ok = false;
                                    break;
                                }
                                thisLoc++;
                            }

                            if (ok)
                            {
                                // found it!
                                foundRamString = true;
                                break;
                            }
                        }
                    }
                }

                if (foundRamString)
                {
                    // write previous rom stuff if exists
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

                    // write ram block
                    byte ctrlByte = 0x80;
                    byte lengthField = (byte)((s - 3) << (5 - seekback));
                    ctrlByte |= lengthField;
                    byte distanceMsbPortion = (byte)(((dist - 1)) >> 8);
                    ctrlByte |= distanceMsbPortion;

                    byte distanceLsbByte = (byte)(dist - 1);

                    compressedBlock.Add(ctrlByte);
                    compressedBlock.Add(distanceLsbByte);
                    decompIndex += s;
                }
                else
                {
                    romBytes++;
                    decompIndex++;
                }
                // if found a ram string, write it; otherwise do the next byte
                // until we hit one that's ram-able.  then write the entire
                // previous ROM section, then the RAM.

                // hit the max rombytes string size
                if (romBytes == 128)
                {
                    // write rom things
                    compressedBlock.Add((byte)(romBytes - 1));
                    for (int i = 0; i < romBytes; i++)
                    {
                        compressedBlock.Add(decompressedBlock[(decompIndex) - (romBytes - i)]);
                    }

                    romBytes = 0;
                }
            } // compress all of first section of data

            // last bits if necessary
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
