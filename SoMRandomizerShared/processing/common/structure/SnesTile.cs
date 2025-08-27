
using SoMRandomizer.util;

namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// SNES graphic encoding and decoding utility.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SnesTile
    {
        // 8x8, row major, paletted
        public byte[] data = new byte[64];

        public static SnesTile LoadTile4bpp(byte[] rom, int offset)
        {
            SnesTile result = new SnesTile();
            byte[] row = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                row[i] = rom[offset + i];
            }
            for (int y = 0; y < 32; y++)
            { 
                // interleaved bpp's
                // y value in this tile
                int actualy = (y % 16) / 2;

                // for each bit in this byte
                for (int i = 0; i < 8; i++)
                {
                    if (y < 16 && y % 2 == 0 && DataUtil.bitTest(row[y], i))
                    { 
                        // bpp 1 on
                        result.data[actualy * 8 + i] |= 0x01;
                    }
                    if (y < 16 && y % 2 == 1 && DataUtil.bitTest(row[y], i))
                    { 
                        // bpp 2 on
                        result.data[actualy * 8 + i] |= 0x02;
                    }
                    if (y >= 16 && y % 2 == 0 && DataUtil.bitTest(row[y], i))
                    { 
                        // bpp 3 on
                        result.data[actualy * 8 + i] |= 0x04;
                    }
                    if (y >= 16 && y % 2 == 1 && DataUtil.bitTest(row[y], i))
                    { 
                        // bpp 4 on
                        result.data[actualy * 8 + i] |= 0x08;
                    }

                }
            }
            return result;
        }

        public static void WriteTile4bpp(SnesTile tile, byte[] rom, int address)
        {
            byte[] encodedTile = new byte[32];
            for (int b = 0; b < 32; b++)
            {
                encodedTile[b] = 0;
                for (int bit = 0; bit < 8; bit++)
                {
                    // x location = bit number
                    int xLoc = bit;
                    // y location = (byte number / 2) % 8
                    int yLoc = (b / 2) % 8;
                    // bitplane number = (byte number / 16) * 2 + byte number % 2
                    int bitNum = (b / 16) * 2 + (b % 2);
                    int bitMask = 1 << bitNum;
                    bool d = (tile.data[yLoc * 8 + xLoc] & bitMask) > 0;
                    if (d)
                    {
                        int destBitMask = 1 << (7 - bit);
                        encodedTile[b] |= (byte)destBitMask;
                    }
                }
            }
            for(int i=0; i < 32; i++)
            {
                rom[address + i] = encodedTile[i];
            }
        }
    }
}
