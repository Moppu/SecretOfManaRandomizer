using SoMRandomizer.util;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Some code taken from my Secret of Mana Editor to compress and decompress 16x16 tilesets.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaTilesetUtil
    {
        // context for encoding or decoding - location in the bit stream where we're currently reading or writing
        private class BitPosition
        {
            public int byteNum = 0;
            public int bitNum = 0;
        }

        private static ushort extractBits(byte[] compressed, int numBits, BitPosition bitPosition)
        {
            ushort result = 0;
            for (int i = 0; i < numBits; i++)
            {
                result <<= 1;
                byte mask = (byte)(1 << (7 - bitPosition.bitNum));
                if (bitPosition.byteNum < compressed.Length && (compressed[bitPosition.byteNum] & mask) > 0)
                    result |= 1;
                bitPosition.bitNum++;
                if (bitPosition.bitNum == 8)
                {
                    bitPosition.byteNum++;
                    bitPosition.bitNum = 0;
                }
            }
            return result;
        }

        private static ushort constructTileData(ushort tileNum, bool vflip, bool hflip, bool altbg, byte pal)
        {
            ushort result = tileNum;
            if (vflip)
                result |= 0x8000;
            if (hflip)
                result |= 0x4000;
            if (altbg)
                result |= 0x2000;
            result |= (ushort)(pal << 10);
            return result;
        }

        public static byte[] getCompressedVanillaTileset16(byte[] origRom, int tilesetNum)
        {
            int offset = DataUtil.int24FromBytes(origRom, VanillaRomOffsets.TILESET16_OFFSETS + tilesetNum * 3);
            int nextOffset = DataUtil.int24FromBytes(origRom, VanillaRomOffsets.TILESET16_OFFSETS + tilesetNum * 3 + 3);
            byte[] tilesetRaw = new byte[nextOffset - offset];
            for (int i = 0; i < tilesetRaw.Length; i++)
            {
                tilesetRaw[i] = origRom[offset + i];
            }
            return tilesetRaw;
        }

        public static List<short> DecodeTileset16(byte[] compressed)
        {
            BitPosition bitPosition = new BitPosition();
            ushort tileNum = 0;
            bool vflip = false;
            bool hflip = false;
            bool altbg = true;
            byte pal = 4;
            int counter = 0;
            short[] uncompressedBuffer = new short[384 * 4];
            while (counter < 384 * 4)
            {
                ushort control1 = extractBits(compressed, 2, bitPosition);
                switch (control1)
                {
                    case 0:
                        uncompressedBuffer[counter] = (short)constructTileData(tileNum, vflip, hflip, altbg, pal);
                        break;
                    case 1:
                        // 8bit add on 10bit value
                        if ((tileNum & 0xFF) == 0xFF)
                            tileNum -= 0xFF;
                        else
                            tileNum++;
                        uncompressedBuffer[counter] = (short)constructTileData(tileNum, vflip, hflip, altbg, pal);
                        break;
                    case 2:
                        // 8bit subtract on 10bit value
                        if ((tileNum & 0xFF) == 0)
                            tileNum += 0xFF;
                        else
                            tileNum--;
                        uncompressedBuffer[counter] = (short)constructTileData(tileNum, vflip, hflip, altbg, pal);
                        break;
                    case 3:
                        ushort nextBit = extractBits(compressed, 1, bitPosition);
                        if (nextBit == 1)
                        {
                            ushort fourthBit = extractBits(compressed, 1, bitPosition);
                            if (fourthBit == 1)
                            {
                                vflip = extractBits(compressed, 1, bitPosition) > 0;
                                hflip = extractBits(compressed, 1, bitPosition) > 0;
                                altbg = extractBits(compressed, 1, bitPosition) > 0;
                                pal = (byte)extractBits(compressed, 3, bitPosition);
                                tileNum = extractBits(compressed, 10, bitPosition);
                                uncompressedBuffer[counter] = (short)constructTileData(tileNum, vflip, hflip, altbg, pal);
                            }
                            else
                            {
                                ushort flagControlBit = extractBits(compressed, 1, bitPosition);
                                if (flagControlBit == 1)
                                {
                                    ushort flags = extractBits(compressed, 3, bitPosition);
                                    vflip = (flags & 0x04) > 0;
                                    hflip = (flags & 0x02) > 0;
                                    altbg = (flags & 0x01) > 0;
                                }
                                else
                                {
                                    ushort palette = extractBits(compressed, 3, bitPosition);
                                    pal = (byte)palette;
                                }

                                short add;
                                ushort sixBitSignedAdd = extractBits(compressed, 6, bitPosition);
                                // extend sign bits
                                if ((sixBitSignedAdd & 0x20) > 0)
                                {
                                    sixBitSignedAdd |= 0xFFC0;
                                    add = (short)sixBitSignedAdd;
                                }
                                else
                                {
                                    add = (short)sixBitSignedAdd;
                                }
                                tileNum = (ushort)(tileNum + add);
                                uncompressedBuffer[counter] = (short)constructTileData(tileNum, vflip, hflip, altbg, pal);
                            }
                        }
                        else
                        {
                            short add;
                            ushort sevenBitSignedAdd = extractBits(compressed, 7, bitPosition);
                            // extend sign bits
                            if ((sevenBitSignedAdd & 0x40) > 0)
                            {
                                sevenBitSignedAdd |= 0xFF80;
                                add = (short)sevenBitSignedAdd;
                            }
                            else
                            {
                                add = (short)sevenBitSignedAdd;
                            }
                            tileNum = (ushort)(tileNum + add);
                            uncompressedBuffer[counter] = (short)constructTileData(tileNum, vflip, hflip, altbg, pal);
                        }
                        break;
                }

                counter++;
            }
            return uncompressedBuffer.ToList();
        }

        private static void dropBits(byte[] compressed, int numBits, ushort data, BitPosition bitPosition)
        {
            for (int i = 0; i < numBits; i++)
            {
                byte mask = (byte)(1 << (7 - bitPosition.bitNum));
                ushort sourceMask = (ushort)(1 << (numBits - i - 1));
                if ((data & sourceMask) > 0)
                    compressed[bitPosition.byteNum] |= mask;
                else
                    compressed[bitPosition.byteNum] &= (byte)~mask;

                bitPosition.bitNum++;
                if (bitPosition.bitNum == 8)
                {
                    bitPosition.byteNum++;
                    bitPosition.bitNum = 0;
                }
            }
        }

        public static List<byte> EncodeTileset16(short[] uncompressed)
        {
            byte[] compressedBuffer = new byte[10000];
            BitPosition bitPosition = new BitPosition();
            int counter = 0;
            ushort tileNum = 0;
            bool vflip = false;
            bool hflip = false;
            bool altbg = true;
            byte pal = 4;
            while (counter < 384 * 4)
            {
                ushort uncompressedData = (ushort)uncompressed[counter];
                ushort thisTileNum = (ushort)(uncompressedData & 0x03FF);
                bool thisVflip = (uncompressedData & 0x8000) > 0;
                bool thisHflip = (uncompressedData & 0x4000) > 0;
                bool thisAltBg = (uncompressedData & 0x2000) > 0;
                byte thisPal = (byte)((uncompressedData & 0x1C00) >> 10);
                // first tile - always do a full one
                if (counter == 0)
                {
                    dropBits(compressedBuffer, 4, 0x0F, bitPosition);
                    dropBits(compressedBuffer, 16, uncompressedData, bitPosition);
                }
                else
                {
                    if (thisPal == pal && thisVflip == vflip && thisHflip == hflip && thisAltBg == altbg)
                    {
                        if (thisTileNum == tileNum)
                        {
                            // 00
                            dropBits(compressedBuffer, 2, 0, bitPosition);
                        }
                        else if ((thisTileNum) == ((tileNum + 1)))
                        {
                            // 01
                            dropBits(compressedBuffer, 2, 1, bitPosition);
                        }
                        else if ((thisTileNum % 256) == ((tileNum - 1) % 256))
                        {
                            // 10
                            dropBits(compressedBuffer, 2, 2, bitPosition);
                        }
                        // 7 bit range: -64 (1000000) to +63 (0111111)
                        else if (thisTileNum < tileNum && tileNum - thisTileNum <= 64)
                        {
                            // 110
                            dropBits(compressedBuffer, 3, 6, bitPosition);
                            int diff = thisTileNum - tileNum;
                            dropBits(compressedBuffer, 7, (ushort)diff, bitPosition);
                        }
                        else if (thisTileNum > tileNum && thisTileNum - tileNum <= 63)
                        {
                            // 110
                            dropBits(compressedBuffer, 3, 6, bitPosition);
                            int diff = thisTileNum - tileNum;
                            dropBits(compressedBuffer, 7, (ushort)diff, bitPosition);
                        }
                        else
                        {
                            // full drop
                            dropBits(compressedBuffer, 4, 0x0F, bitPosition);
                            dropBits(compressedBuffer, 16, uncompressedData, bitPosition);
                        }
                    }
                    else
                    if (thisPal == pal)
                    {
                        // 6 bit range: -32 (100000) to +31 (011111)
                        if (thisTileNum < tileNum && tileNum - thisTileNum <= 32)
                        {
                            // 1110
                            dropBits(compressedBuffer, 4, 14, bitPosition);
                            dropBits(compressedBuffer, 1, 1, bitPosition);
                            dropBits(compressedBuffer, 1, (ushort)(thisVflip ? 1 : 0), bitPosition);
                            dropBits(compressedBuffer, 1, (ushort)(thisHflip ? 1 : 0), bitPosition);
                            dropBits(compressedBuffer, 1, (ushort)(thisAltBg ? 1 : 0), bitPosition);
                            int diff = thisTileNum - tileNum;
                            dropBits(compressedBuffer, 6, (ushort)diff, bitPosition);
                        }
                        else if (thisTileNum > tileNum && thisTileNum - tileNum <= 31)
                        {
                            // 1110
                            dropBits(compressedBuffer, 4, 14, bitPosition);
                            dropBits(compressedBuffer, 1, 1, bitPosition);
                            dropBits(compressedBuffer, 1, (ushort)(thisVflip ? 1 : 0), bitPosition);
                            dropBits(compressedBuffer, 1, (ushort)(thisHflip ? 1 : 0), bitPosition);
                            dropBits(compressedBuffer, 1, (ushort)(thisAltBg ? 1 : 0), bitPosition);
                            int diff = thisTileNum - tileNum;
                            dropBits(compressedBuffer, 6, (ushort)diff, bitPosition);
                        }
                        else
                        {
                            // full drop
                            dropBits(compressedBuffer, 4, 0x0F, bitPosition);
                            dropBits(compressedBuffer, 16, uncompressedData, bitPosition);
                        }
                    }
                    else if (thisVflip == vflip && thisHflip == hflip && thisAltBg == altbg)
                    {
                        // 6 bit range: -32 (100000) to +31 (011111)
                        if (thisTileNum < tileNum && tileNum - thisTileNum <= 32)
                        {
                            // 1110
                            dropBits(compressedBuffer, 4, 14, bitPosition);
                            dropBits(compressedBuffer, 1, 0, bitPosition);
                            dropBits(compressedBuffer, 3, (ushort)thisPal, bitPosition);
                            int diff = thisTileNum - tileNum;
                            dropBits(compressedBuffer, 6, (ushort)diff, bitPosition);
                        }
                        else if (thisTileNum > tileNum && thisTileNum - tileNum <= 31)
                        {
                            // 1110
                            dropBits(compressedBuffer, 4, 14, bitPosition);
                            dropBits(compressedBuffer, 1, 0, bitPosition);
                            dropBits(compressedBuffer, 3, (ushort)thisPal, bitPosition);
                            int diff = thisTileNum - tileNum;
                            dropBits(compressedBuffer, 6, (ushort)diff, bitPosition);
                        }
                        else
                        {
                            // full drop
                            dropBits(compressedBuffer, 4, 0x0F, bitPosition);
                            dropBits(compressedBuffer, 16, uncompressedData, bitPosition);
                        }
                    }
                    else
                    {
                        // full drop
                        dropBits(compressedBuffer, 4, 0x0F, bitPosition);
                        dropBits(compressedBuffer, 16, uncompressedData, bitPosition);
                    }
                }

                tileNum = thisTileNum;
                pal = thisPal;
                vflip = thisVflip;
                hflip = thisHflip;
                altbg = thisAltBg;
                counter++;
            }

            List<byte> output = new List<byte>();
            for (int i=0; i < bitPosition.byteNum; i++)
            {
                output.Add(compressedBuffer[i]);
            }
            return output;
        }
    }
}
