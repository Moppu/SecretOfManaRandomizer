using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Hack that displays a mini-map on the sprite layer when flying on flammie.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class Ff6StyleMiniMap : RandoProcessor
    {
        protected override string getName()
        {
            return "Minimap when flying on flammie";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_MINIMAP))
            {
                return false;
            }

            // 2560 bytes for tiles + some buffer for code
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 3000);

            // we need to decode the RLE map in bank 6, and translate each tile type to a color value for our map thing, then dump it in rom somewhere in the 4bpp composite format
            // this will be 64 8x8 tiles, which will correspond to 16 16x16 sprites
            // the map is 512x512? tiles, so we need to prioritize within 8x8 blocks

            // 68000 - line offsets (512 x 16 bit into bank C6) - each one expects 512 tiles
            byte[,] mapData = new byte[512, 512];
            int bank = VanillaRomOffsets.getBankStart(VanillaRomOffsets.WORLDMAP_ROWS_OFFSETS);
            int lastPos = 0;
            for(int i=0; i < 512; i++)
            {
                int pos16 = DataUtil.ushortFromBytes(outRom, VanillaRomOffsets.WORLDMAP_ROWS_OFFSETS + i * 2);
                // roll to the next bank
                if(pos16 < lastPos && bank == VanillaRomOffsets.getBankStart(VanillaRomOffsets.WORLDMAP_ROWS_OFFSETS))
                {
                    bank += 0x10000;
                }
                lastPos = pos16;
                int romPosition = pos16 + bank;
                int rowPosition = 0;
                while(rowPosition < 512)
                {
                    byte romByte = outRom[romPosition++];
                    if(romByte >= 0xc0)
                    {
                        int num = romByte - 0xBF;
                        byte romByte2 = outRom[romPosition++];
                        for(int j=0; j < num && rowPosition < 512; j++)
                        {
                            mapData[i, rowPosition++] = romByte2;
                        }
                    }
                    else
                    {
                        mapData[i, rowPosition++] = romByte;
                    }
                }
            }

            Dictionary<byte, int> mapTileToColorConversion = new Dictionary<byte, int>();
            List<byte> mountainTiles = new byte[] {
                0,1,2,3,4,5,12,13,14,15,
                16,17,18,19,20,21,28,30,31,
                32,33,34,35,36,37,44,45,46,47,
                49,50,63,
                66,78,79,
                // luna stuff
                54,55,56,70,71,72,86,87,88,141,
            }.ToList();
            List<byte> desertTiles = new byte[] {
                6,7,8,
                22,24,
                38,39,40,
                89, 90, 106, 107, 108, 111,
                122, 123, 124,
            }.ToList();
            List<byte> desertEdgeTiles = new byte[] {
                144,145,146,147,148,149,150,151,
            }.ToList();
            List<byte> forestTiles = new byte[] {
                9, 10, 11,
                25, 26, 27,
                41, 42, 43,
                // grass on the edge of water
                51,52,53,
                67, 68, 69,
                83, 84, 85, 
            }.ToList();
            List<byte> grassTiles = new byte[] {
                23, 29,
                60, 61, 62,
                104, 105, 
                120, 121, 127,
                136, 137, 138, 139, 140, 142, 143,
                179, 180, 181, 182, 183, 185,
            }.ToList();
            List<byte> snowTiles = new byte[] {
                48,
                64,65,
                80,81,
                96,97,98,109,110,
                112,113,114,125,126,
                128,129,130,
                175,
                186,187,
            }.ToList();
            List<byte> purpleTiles = new byte[] {
                160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174
            }.ToList();

            // colors:
            // 0 = transparent
            // 1 = dark gray
            // 2 = light lavender
            // 3 = darker lavender
            // 4 = purple
            // 5 = yellow
            // 6 = darker yellow
            // 7 = orange
            // 8 = light green
            // 9 = green
            // 10 = dark green
            // 11 = basically white, really light yellow
            // 12 = same but a little darker
            // 13 = same but darker
            // 14 = very light blue, almost white
            // 15 = same but a little darker
            foreach (byte mountainTile in mountainTiles)
            {
                mapTileToColorConversion[mountainTile] = 1;
            }
            foreach (byte desertTile in desertTiles)
            {
                mapTileToColorConversion[desertTile] = 5;
            }
            foreach (byte desertEdgeTile in desertEdgeTiles)
            {
                mapTileToColorConversion[desertEdgeTile] = 7;
            }
            foreach (byte forestTile in forestTiles)
            {
                mapTileToColorConversion[forestTile] = 10;
            }
            foreach (byte grassTile in grassTiles)
            {
                mapTileToColorConversion[grassTile] = 9;
            }
            foreach (byte snowTile in snowTiles)
            {
                mapTileToColorConversion[snowTile] = 2;
            }
            foreach (byte purpleTile in purpleTiles)
            {
                mapTileToColorConversion[purpleTile] = 4;
            }

            // 64x64 = 8x8 tiles
            List<byte[]> allTileData = new List<byte[]>();
            for(int tileIndexY = 0; tileIndexY < 8; tileIndexY++)
            {
                for (int tileIndexX = 0; tileIndexX < 8; tileIndexX++)
                {
                    byte[] tileData = new byte[64];
                    allTileData.Add(tileData);
                    for (int pixelY = 0; pixelY < 8; pixelY++)
                    {
                        int sourceTileYStart = tileIndexY * 64 + pixelY * 8;
                        for (int pixelX = 0; pixelX < 8; pixelX++)
                        {
                            int sourceTileXStart = tileIndexX * 64 + pixelX * 8;
                            // this minimap pixel represents an 8x8 block of tiles in the map (512/64)
                            int[] paletteIndexNums = new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, };
                            for (int tileY = 0; tileY < 8; tileY++)
                            {
                                for (int tileX = 0; tileX < 8; tileX++)
                                {
                                    int paletteIndex = 0;
                                    byte mapTile = mapData[sourceTileYStart + tileY, sourceTileXStart + tileX];
                                    if(mapTileToColorConversion.ContainsKey(mapTile))
                                    {
                                        paletteIndex = mapTileToColorConversion[mapTile];
                                    }
                                    paletteIndexNums[paletteIndex]++;
                                }
                            }
                            // now find the max one used in this block, and we'll use that
                            int maxIndex = 0;
                            int maxValue = 0;
                            for(int i=1; i < 16; i++)
                            {
                                if(paletteIndexNums[i] > maxValue)
                                {
                                    maxIndex = i;
                                    maxValue = paletteIndexNums[i];
                                }
                            }
                            tileData[pixelY * 8 + pixelX] = (byte)maxIndex;
                        }
                    }
                }
            }

            int tileDataStartOffset = context.workingOffset + 0xC00000;
            int[] vramTileOrder = new int[] 
            {
                00, 01, 02, 03, 04, 05, 06, 07,
                16, 17, 18, 19, 20, 21, 22, 23,
                08, 09, 10, 11, 12, 13, 14, 15,
                24, 25, 26, 27, 28, 29, 30, 31,
                32, 33, 34, 35, 36, 37, 38, 39,
                48, 49, 50, 51, 52, 53, 54, 55,
                40, 41, 42, 43, 44, 45, 46, 47,
                56, 57, 58, 59, 60, 61, 62, 63
            };
            for (int tileIndex = 0; tileIndex < 64; tileIndex++)
            {
                SnesTile tile = new SnesTile();
                tile.data = allTileData[vramTileOrder[tileIndex]];
                SnesTile.WriteTile4bpp(tile, outRom, context.workingOffset);
                context.workingOffset += 32;
            }

            byte y = 14;
            byte m = 1;
            byte n = 0;
            // 16 of these?  for different angles
            // north is 0, west is positive
            List<byte[]> pointerTiles = new byte[][] {
                new byte[] { // north
                    n,n,n,m,y,m,n,n,
                    n,n,n,m,y,m,n,n,
                    n,n,n,m,y,m,n,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,n,m,m,m,n,n,
                    n,n,n,n,n,n,n,n,
                },
                new byte[] {
                    n,n,y,n,n,n,n,n,
                    n,m,y,m,n,n,n,n,
                    n,n,m,y,m,m,n,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,n,m,m,m,n,n,
                    n,n,n,n,n,n,n,n,
                },
                new byte[] {
                    n,m,n,n,n,n,n,n,
                    m,y,m,n,n,n,n,n,
                    n,m,y,m,m,m,n,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,n,m,m,m,n,n,
                    n,n,n,n,n,n,n,n,
                },
                new byte[] {
                    n,n,n,n,n,n,n,n,
                    n,m,n,n,n,n,n,n,
                    y,y,m,m,m,m,n,n,
                    n,m,y,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,n,m,m,m,n,n,
                    n,n,n,n,n,n,n,n,
                },
                new byte[] { // west
                    n,n,n,n,n,n,n,n,
                    n,n,n,n,n,n,n,n,
                    n,n,n,m,m,m,n,n,
                    m,m,m,y,y,y,m,n,
                    y,y,y,y,y,y,m,n,
                    m,m,m,y,y,y,m,n,
                    n,n,n,m,m,m,n,n,
                    n,n,n,n,n,n,n,n,
                },
                new byte[] {
                    n,n,n,n,n,n,n,n,
                    n,n,n,m,m,m,n,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,m,y,y,y,y,m,n,
                    y,y,m,m,m,m,n,n,
                    n,m,n,n,n,n,n,n,
                    n,n,n,n,n,n,n,n,
                },
                new byte[] {
                    n,n,n,n,n,n,n,n,
                    n,n,n,m,m,m,n,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,m,y,m,m,m,n,n,
                    m,y,m,n,n,n,n,n,
                    n,m,n,n,n,n,n,n,
                },
                new byte[] {
                    n,n,n,n,n,n,n,n,
                    n,n,n,m,m,m,n,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,m,m,n,n,
                    n,m,y,m,n,n,n,n,
                    n,n,y,n,n,n,n,n,
                },
                new byte[] { // south
                    n,n,n,n,n,n,n,n,
                    n,n,n,m,m,m,n,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,m,y,y,y,m,n,
                    n,n,n,m,y,m,n,n,
                    n,n,n,m,y,m,n,n,
                    n,n,n,m,y,m,n,n,
                },
                new byte[] {
                    n,n,n,n,n,n,n,n,
                    n,n,m,m,m,n,n,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,n,m,m,y,m,n,n,
                    n,n,n,n,m,y,m,n,
                    n,n,n,n,n,y,n,n,
                },
                new byte[] {
                    n,n,n,n,n,n,n,n,
                    n,n,m,m,m,n,n,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,n,m,m,m,y,m,n,
                    n,n,n,n,n,m,y,m,
                    n,n,n,n,n,n,m,n,
                },
                new byte[] {
                    n,n,n,n,n,n,n,n,
                    n,n,m,m,m,n,n,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,y,m,n,
                    n,n,m,m,m,m,y,y,
                    n,n,n,n,n,n,m,n,
                    n,n,n,n,n,n,n,n,
                },
                new byte[] { // east
                    n,n,n,n,n,n,n,n,
                    n,n,n,n,n,n,n,n,
                    n,n,m,m,m,n,n,n,
                    n,m,y,y,y,m,m,m,
                    n,m,y,y,y,y,y,y,
                    n,m,y,y,y,m,m,m,
                    n,n,m,m,m,n,n,n,
                    n,n,n,n,n,n,n,n,
                },
                new byte[] {
                    n,n,n,n,n,n,n,n,
                    n,n,n,n,n,n,m,n,
                    n,n,m,m,m,m,y,y,
                    n,m,y,y,y,y,m,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,n,m,m,m,n,n,n,
                    n,n,n,n,n,n,n,n,
                },
                new byte[] {
                    n,n,n,n,n,n,m,n,
                    n,n,n,n,n,m,y,m,
                    n,n,m,m,m,y,m,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,n,m,m,m,n,n,n,
                    n,n,n,n,n,n,n,n,
                },
                new byte[] {
                    n,n,n,n,n,y,n,n,
                    n,n,n,n,m,y,m,n,
                    n,n,m,m,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,m,y,y,y,m,n,n,
                    n,n,m,m,m,n,n,n,
                    n,n,n,n,n,n,n,n,
                },
            }.ToList();

            for(int p=0; p < 16; p++)
            {
                SnesTile tile = new SnesTile();
                tile.data = pointerTiles[p];
                SnesTile.WriteTile4bpp(tile, outRom, context.workingOffset);
                context.workingOffset += 32;
            }

            // now when we take off and upload flammie tiles, make sure we upload this crap too
            outRom[0x1F8D8] = 0x22;
            outRom[0x1F8D9] = (byte)(context.workingOffset);
            outRom[0x1F8DA] = (byte)(context.workingOffset >> 8);
            outRom[0x1F8DB] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1F8DC] = 0xEA;

            // removed code first - drop shit to DMA
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x80;

            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0x42;

            // now do it again for our new tiles .. can we use the same channel?  nfi how many cycles it takes to actually transfer
            // A9 01       LDA #01
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;
            // 8D 70 43    STA $4370
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x70;
            outRom[context.workingOffset++] = 0x43;
            // A9 18       LDA #18
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x18;
            // 8D 71 43    STA $4371
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x71;
            outRom[context.workingOffset++] = 0x43;
            // A9 xx       LDA (tiles LSB)
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)(tileDataStartOffset);
            // 8D 72 43    STA $4372
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x72;
            outRom[context.workingOffset++] = 0x43;
            // A9 xx       LDA (tiles mid)
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)(tileDataStartOffset>>8);
            // 8D 73 43    STA $4373
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x73;
            outRom[context.workingOffset++] = 0x43;
            // A9 xx       LDA (tiles MSB)
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)(tileDataStartOffset >> 16);
            // 8D 74 43    STA $4374
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x74;
            outRom[context.workingOffset++] = 0x43;
            // A9 00       LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // 8D 75 43    STA $4375
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x75;
            outRom[context.workingOffset++] = 0x43;
            // A9 08       LDA #08 -> 0x0A to include pointer tiles
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x0A;
            // 8D 76 43    STA $4376
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x76;
            outRom[context.workingOffset++] = 0x43;

            // now trigger transfer again, same way it does above
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x80;

            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0x42;

            // don't forget to RTL
            outRom[context.workingOffset++] = 0x6B;

            int frameProcessSubroutine = context.workingOffset;

            int mapScreenX = 0xB0;
            int mapScreenY = 0x90;
            // start at 7e0960?
            int ramAddr = 0x0970;
            for (int i = 0; i < 16; i++)
            {
                int xpos = i % 4;
                int ypos = i / 4;
                int thisX = mapScreenX + xpos * 16;
                int thisY = mapScreenY + ypos * 16;
                // LDA #yyxx
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = (byte)thisX;
                outRom[context.workingOffset++] = (byte)thisY;

                // STA ramaddr
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = (byte)ramAddr;
                outRom[context.workingOffset++] = (byte)(ramAddr >> 8);

                ramAddr += 2;

                // 0x2000 = priority 2; we'll want the position indicator to be 3
                int tileIndex = i * 2;
                if (i >= 8)
                {
                    tileIndex += 0x10;
                }
                int controlValue = 0x80 + tileIndex + 0x2000;
                // tile indexes start at 0x80, msb off, pal 0
                // tile index 0x80 uses tile 0, 1, 16, and 17 (below); we want to use 0x82 next, and so on
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = (byte)controlValue;
                outRom[context.workingOffset++] = (byte)(controlValue >> 8);

                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = (byte)ramAddr;
                outRom[context.workingOffset++] = (byte)(ramAddr >> 8);
                ramAddr += 2;
            }

            ramAddr = 0x0960;
            // 16x16 sprite settings - start at 7e0a16, write 0xAA, i think we want four of them total
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xAA;
            outRom[context.workingOffset++] = 0xAA;

            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x17;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x19;
            outRom[context.workingOffset++] = 0x0A;

            // here: pointer sprite .. $FA, $FC - 0x1000 -> 0x40 pixels - shift right 6x
            // LDA $FA
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xFA;
            // AND #0FFF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x0F;
            // LSR x6
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #mapScreenX
            int mapScreenXOffset = mapScreenX - 3;
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = (byte)mapScreenXOffset;
            outRom[context.workingOffset++] = (byte)(mapScreenXOffset >> 8);
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA $FC
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xFC;
            // AND #0FFF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x0F;
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // AND #FF00
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xFF;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC mapScreenY at MSB
            int mapScreenYOffset = mapScreenY - 3;
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = (byte)(mapScreenYOffset);
            // ADC $1,s
            outRom[context.workingOffset++] = 0x63;
            outRom[context.workingOffset++] = 0x01;
            // STA ramaddr
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = (byte)ramAddr;
            outRom[context.workingOffset++] = (byte)(ramAddr >> 8);

            // PLA to even out stack
            outRom[context.workingOffset++] = 0x68;

            ramAddr += 2;

            // LDA $F6, not f8
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xF6;
            // -------------
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #0020 - add half the size of each rotational chunk, so we're choosing for the center, not the left edge - this makes the sprite match the direction a little better
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0x00;
            // AND #3FF - handle overflow on the add
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x03;
            // -------------
            // LSR 6x
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #00A0 // base tile for flammie icon
            // ADC #3000 // priority + palette
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x30;
            // STA ramaddr
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = (byte)ramAddr;
            outRom[context.workingOffset++] = (byte)(ramAddr >> 8);

            // rtl at the end
            outRom[context.workingOffset++] = 0x6B;

            // write to the sprites block every frame so we show the map
            outRom[0x9BF7] = 0x22;
            outRom[0x9BF8] = (byte)(context.workingOffset);
            outRom[0x9BF9] = (byte)(context.workingOffset >> 8);
            outRom[0x9BFA] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x9BFB] = 0xEA;
            outRom[0x9BFC] = 0xEA;
            // note: 16bit a, 8bit x/y

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(frameProcessSubroutine);
            outRom[context.workingOffset++] = (byte)(frameProcessSubroutine >> 8);
            outRom[context.workingOffset++] = (byte)((frameProcessSubroutine >> 16) + 0xC0);

            // at the end - the removed code
            outRom[context.workingOffset++] = 0xAE;
            outRom[context.workingOffset++] = 0x45;
            outRom[context.workingOffset++] = 0x0A;

            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x1B;
            outRom[context.workingOffset++] = 0x9C;

            // rtl
            outRom[context.workingOffset++] = 0x6B;

            // overhead view, too
            outRom[0x9FA8] = 0x22;
            outRom[0x9FA9] = (byte)(context.workingOffset);
            outRom[0x9FAA] = (byte)(context.workingOffset >> 8);
            outRom[0x9FAB] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x9FAC] = 0xEA;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(frameProcessSubroutine);
            outRom[context.workingOffset++] = (byte)(frameProcessSubroutine >> 8);
            outRom[context.workingOffset++] = (byte)((frameProcessSubroutine >> 16) + 0xC0);

            // at the end - the removed code
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x00;

            outRom[context.workingOffset++] = 0xA6;
            outRom[context.workingOffset++] = 0x5F;

            // rtl
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
