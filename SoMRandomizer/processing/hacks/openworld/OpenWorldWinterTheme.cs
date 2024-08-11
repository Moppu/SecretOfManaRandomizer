using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.util;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Palette and enemy changes for open world winter modes.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldWinterTheme
    {
        public void process(byte[] origRom, byte[] outRom, Random r, ref int newCodeOffset, NamesOfThings namesOfThings)
        {

            Dictionary<int, Dictionary<int, SnesColor>> enemyColorChanges = new Dictionary<int, Dictionary<int, SnesColor>>();

            // change enemy types, status effects on weapons, and spells

            // enemy palettes
            // [1] buzz bee
            // 6 -> 172,32,32
            // 7 -> 248,64,64
            // 8 -> 248,128,128
            // 9 -> 168,32,32
            // 10 -> 248,64,64
            enemyColorChanges[1] = new Dictionary<int, SnesColor>();
            enemyColorChanges[1][6] = new SnesColor(172, 32, 32);
            enemyColorChanges[1][7] = new SnesColor(248, 64, 64);
            enemyColorChanges[1][8] = new SnesColor(248, 128, 128);
            enemyColorChanges[1][9] = new SnesColor(168, 32, 32);
            enemyColorChanges[1][10] = new SnesColor(248, 64, 64);
            namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 1, "Jingle Bee");

            // [2] mushroom
            // 7 -> 128,240,128
            // 8 -> 160,224,160
            // 9 -> 200,248,200
            // 10-> 224,248,224
            // 11-> 160,160,248
            // 12-> 192,192,248
            // 13-> 208,208,248
            // 14-> 224,224,248
            enemyColorChanges[2] = new Dictionary<int, SnesColor>();
            enemyColorChanges[2][7] = new SnesColor(128, 240, 128);
            enemyColorChanges[2][8] = new SnesColor(160, 224, 160);
            enemyColorChanges[2][9] = new SnesColor(200, 248, 200);
            enemyColorChanges[2][10] = new SnesColor(224, 248, 224);
            enemyColorChanges[2][11] = new SnesColor(160, 160, 248);
            enemyColorChanges[2][12] = new SnesColor(192, 192, 248);
            enemyColorChanges[2][13] = new SnesColor(208, 208, 248);
            enemyColorChanges[2][14] = new SnesColor(224, 224, 248);

            // [3] chobin hood
            // 6 -> 128,0,0
            // 7 -> 200,32,32
            // 8 -> 248,64,64
            // 9 -> 248,128,128
            enemyColorChanges[3] = new Dictionary<int, SnesColor>();
            enemyColorChanges[3][6] = new SnesColor(128, 0, 0);
            enemyColorChanges[3][7] = new SnesColor(200, 32, 32);
            enemyColorChanges[3][8] = new SnesColor(248, 64, 64);
            enemyColorChanges[3][9] = new SnesColor(248, 128, 128);

            // [4] flower
            // 7 -> 128,8,8
            // 8 -> 192,48,48
            // 9 -> 248,96,96
            // 10-> 248,128,128
            // 11-> 248,160,160
            enemyColorChanges[4] = new Dictionary<int, SnesColor>();
            enemyColorChanges[4][7] = new SnesColor(128, 8, 8);
            enemyColorChanges[4][8] = new SnesColor(192, 48, 48);
            enemyColorChanges[4][9] = new SnesColor(248, 96, 96);
            enemyColorChanges[4][10] = new SnesColor(248, 128, 128);
            enemyColorChanges[4][11] = new SnesColor(248, 160, 160);
            namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 4, "Poinsettia");

            // [5] iffish
            // 2 -> 64,64,128
            // 4 -> 0,0,160
            // 5 -> 88,88,176
            // 6 -> 80,80,208
            // 7 -> 120,120,248
            // 8 -> 128,128,248
            // 9 -> 160,160,248
            // 10-> 192,192,248
            // 15-> 208,208,248
            enemyColorChanges[5] = new Dictionary<int, SnesColor>();
            enemyColorChanges[5][2] = new SnesColor(64, 64, 128);
            enemyColorChanges[5][4] = new SnesColor(0, 0, 160);
            enemyColorChanges[5][5] = new SnesColor(88, 88, 176);
            enemyColorChanges[5][6] = new SnesColor(80, 80, 208);
            enemyColorChanges[5][7] = new SnesColor(120, 120, 248);
            enemyColorChanges[5][8] = new SnesColor(128, 128, 248);
            enemyColorChanges[5][9] = new SnesColor(160, 160, 248);
            enemyColorChanges[5][10] = new SnesColor(192, 192, 248);
            enemyColorChanges[5][15] = new SnesColor(208, 208, 248);
            namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 5, "Frozen Fish");

            // [6] goblin
            // 2 -> 72,72,248
            // 3 -> 120,120,248
            // 4 -> 192,192,248
            // 5 -> 224,224,248
            // 10-> 216,40,40
            // 11-> 248,96,96
            enemyColorChanges[6] = new Dictionary<int, SnesColor>();
            enemyColorChanges[6][2] = new SnesColor(72, 72, 248);
            enemyColorChanges[6][3] = new SnesColor(120, 120, 248);
            enemyColorChanges[6][4] = new SnesColor(192, 192, 248);
            enemyColorChanges[6][5] = new SnesColor(224, 224, 248);
            enemyColorChanges[6][10] = new SnesColor(216, 40, 40);
            enemyColorChanges[6][11] = new SnesColor(248, 96, 96);

            // [7] eyespy
            // 4 -> 176,0,0
            // 5 -> 248,0,0
            // 6 -> 248,32,32
            // 7 -> 248,96,96
            // 8 -> 248,160,160
            enemyColorChanges[7] = new Dictionary<int, SnesColor>();
            enemyColorChanges[7][4] = new SnesColor(176, 0, 0);
            enemyColorChanges[7][5] = new SnesColor(248, 0, 0);
            enemyColorChanges[7][6] = new SnesColor(248, 32, 32);
            enemyColorChanges[7][7] = new SnesColor(248, 96, 96);
            enemyColorChanges[7][8] = new SnesColor(248, 160, 160);
            namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x7, "Festive Eye Spy");

            // [12] zombie
            // 4 -> 0,120,40
            // 5 -> 0,152,40
            // 6 -> 56,168,72
            // 7 -> 160,64,64
            // 8 -> 208,48,48
            // 9 -> 224,96,96
            // 10-> 232,168,168
            enemyColorChanges[0x12] = new Dictionary<int, SnesColor>();
            enemyColorChanges[0x12][4] = new SnesColor(0, 120, 40);
            enemyColorChanges[0x12][5] = new SnesColor(0, 152, 40);
            enemyColorChanges[0x12][6] = new SnesColor(56, 168, 72);
            enemyColorChanges[0x12][7] = new SnesColor(160, 64, 64);
            enemyColorChanges[0x12][8] = new SnesColor(208, 48, 48);
            enemyColorChanges[0x12][9] = new SnesColor(224, 96, 96);
            enemyColorChanges[0x12][10] = new SnesColor(232, 168, 168);
            namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x12, "Zombie Caroler");

            // [16] pebbler
            // 8 -> 248,64,64
            // 11-> 0,88,0
            // 12-> 96,176,32
            // 13-> 136,208,24
            // 14-> 160,216,32
            // 15-> 184,232,160
            enemyColorChanges[0x16] = new Dictionary<int, SnesColor>();
            enemyColorChanges[0x16][8] = new SnesColor(248, 64, 64);
            enemyColorChanges[0x16][11] = new SnesColor(0, 88, 0);
            enemyColorChanges[0x16][12] = new SnesColor(96, 176, 32);
            enemyColorChanges[0x16][13] = new SnesColor(136, 208, 24);
            enemyColorChanges[0x16][14] = new SnesColor(160, 216, 32);
            enemyColorChanges[0x16][15] = new SnesColor(184, 232, 160);
            namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x16, "Pebbler Grinch");

            // [18] crab
            // 5 -> 160,0,0
            // 6 -> 224,56,56
            // 7 -> 248,112,112
            // 8 -> 152,152,248
            // 9 -> 176,176,248
            // 10-> 200,200,248
            // 11-> 232,232,248
            // 12-> 96,176,96
            // 13-> 128,200,128
            // 14-> 192,248,192
            enemyColorChanges[0x18] = new Dictionary<int, SnesColor>();
            enemyColorChanges[0x18][5] = new SnesColor(160, 0, 0);
            enemyColorChanges[0x18][6] = new SnesColor(224, 56, 56);
            enemyColorChanges[0x18][7] = new SnesColor(248, 112, 112);
            enemyColorChanges[0x18][8] = new SnesColor(152, 152, 248);
            enemyColorChanges[0x18][9] = new SnesColor(176, 176, 248);
            enemyColorChanges[0x18][10] = new SnesColor(200, 200, 248);
            enemyColorChanges[0x18][11] = new SnesColor(232, 232, 248);
            enemyColorChanges[0x18][12] = new SnesColor(96, 176, 96);
            enemyColorChanges[0x18][13] = new SnesColor(128, 200, 128);
            enemyColorChanges[0x18][14] = new SnesColor(192, 248, 192);
            namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x18, "Christmas Crab");

            // [19] horse chess thing
            // 2 -> 96,48,8
            // 3 -> 32,120,32
            // 4 -> 112,56,0
            // 5 -> 128,64,0
            // 7 -> 176,88,16
            // 8 -> 176,104,48
            // 9 -> 176,120,72
            // 10-> 184,112,48
            // 11-> 88,176,88
            // 12-> 128,216,128
            // 13-> 168,248,168
            // 15-> 208,128,80
            enemyColorChanges[0x19] = new Dictionary<int, SnesColor>();
            enemyColorChanges[0x19][2] = new SnesColor(96, 48, 8);
            enemyColorChanges[0x19][3] = new SnesColor(32, 120, 32);
            enemyColorChanges[0x19][4] = new SnesColor(112, 56, 0);
            enemyColorChanges[0x19][5] = new SnesColor(128, 64, 0);
            enemyColorChanges[0x19][7] = new SnesColor(176, 88, 16);
            enemyColorChanges[0x19][8] = new SnesColor(176, 104, 48);
            enemyColorChanges[0x19][9] = new SnesColor(176, 120, 72);
            enemyColorChanges[0x19][10] = new SnesColor(184, 112, 48);
            enemyColorChanges[0x19][11] = new SnesColor(88, 176, 88);
            enemyColorChanges[0x19][12] = new SnesColor(128, 176, 128);
            enemyColorChanges[0x19][13] = new SnesColor(168, 248, 168);
            enemyColorChanges[0x19][15] = new SnesColor(208, 128, 80);
            namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x19, "Gingerhorse");

            // [2d] chest
            // 12-> 200,32,32
            // 13-> 248,72,72
            // 14-> 248,152,152
            // 15-> 248,192,192
            enemyColorChanges[0x2D] = new Dictionary<int, SnesColor>();
            enemyColorChanges[0x2D][12] = new SnesColor(200, 32, 32);
            enemyColorChanges[0x2D][13] = new SnesColor(248, 72, 72);
            enemyColorChanges[0x2D][14] = new SnesColor(248, 152, 152);
            enemyColorChanges[0x2D][15] = new SnesColor(248, 192, 192);
            namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x2D, "Jolly Mimic");

            foreach (int enemyNum in enemyColorChanges.Keys)
            {
                Dictionary<int, SnesColor> enemyColors = enemyColorChanges[enemyNum];
                foreach (int palIndex in enemyColors.Keys)
                {
                    enemyColors[palIndex].put(outRom, 0x80FFE + (enemyNum) * 0x1E + (palIndex) * 2);
                }
            }

            byte[] giftBoxGraphics = DataUtil.readResource("SoMRandomizer.Resources.giftbox_tiles.bin");
            for (int i = 0; i < giftBoxGraphics.Length; i++)
            {
                outRom[i + 0x186540] = giftBoxGraphics[i];
            }

            // npc palette 6A (object EA = palette EB)
            int chestPalA = 0xEA;
            int chestPalB = 0xEB;
            for(int i=0; i < 15; i++)
            {
                int locationA = 0x80FFE + (chestPalA) * 0x1E + (i) * 2;
                int locationB = 0x80FFE + (chestPalB) * 0x1E + (i) * 2;
                outRom[locationA] = outRom[locationB];
                outRom[locationA + 1] = outRom[locationB + 1];
            }


            // make world map x30 and xba/ca/da/ea landable in ice country region (or all) - x67600
            // these are 24 byte blocks, one for each region (16 total)
            for(int i=0; i < 16; i++)
            {
                // byte 6, bit 0 -> 0x30
                outRom[0x67600 + i * 0x18 + 6] &= 0xFE;
                // byte 23, bit 2 -> 0xBA
                outRom[0x67600 + i * 0x18 + 23] &= 0xFB;
                // byte 14, bit 0
                outRom[0x67600 + i * 0x18 + 14] &= 0xFE;
                // byte 13, bit 7
                outRom[0x67600 + i * 0x18 + 13] &= 0x7F;
                // byte 16, bit 1
                outRom[0x67600 + i * 0x18 + 16] &= 0xFD;
                // byte 14, bit 2
                outRom[0x67600 + i * 0x18 + 14] &= 0xFB;
            }

            outRom[0x90A4] = 0x08;
            outRom[0x90B5] = 0x00;
            int[] worldMapPalOffsets = new int[] { 0x67DFE, 0x6483E, };
            Dictionary<int, Dictionary<int, SnesColor>> worldMapReplacements = new Dictionary<int, Dictionary<int, SnesColor>>();
            worldMapReplacements[5] = new Dictionary<int, SnesColor>();
            worldMapReplacements[5][1] = new SnesColor(32, 88, 48);
            worldMapReplacements[5][2] = new SnesColor(40, 96, 56);
            worldMapReplacements[5][3] = new SnesColor(48, 104, 72);
            worldMapReplacements[5][4] = new SnesColor(96, 128, 120);
            worldMapReplacements[5][5] = new SnesColor(120, 136, 136);
            worldMapReplacements[5][6] = new SnesColor(160, 160, 168 + 16);
            worldMapReplacements[5][7] = new SnesColor(176, 176, 176 + 24);
            worldMapReplacements[5][8] = new SnesColor(192, 192, 192 + 24);
            worldMapReplacements[5][9] = new SnesColor(248, 248, 248);
            worldMapReplacements[5][10] = new SnesColor(208, 208, 208 + 24);
            worldMapReplacements[5][11] = new SnesColor(216, 216, 216 + 24);
            worldMapReplacements[5][12] = new SnesColor(224, 224, 224 + 24);
            worldMapReplacements[5][13] = new SnesColor(232, 232, 232 + 16);
            worldMapReplacements[5][14] = new SnesColor(200, 200, 200 + 24);
            worldMapReplacements[5][15] = new SnesColor(208, 208, 208 + 24);

            worldMapReplacements[6] = new Dictionary<int, SnesColor>();
            worldMapReplacements[6][0] = new SnesColor(208, 208, 208 + 24);
            worldMapReplacements[6][1] = new SnesColor(232, 232, 232);
            worldMapReplacements[6][5] = new SnesColor(224, 224, 224);
            worldMapReplacements[6][6] = new SnesColor(208, 208, 216);
            worldMapReplacements[6][12] = new SnesColor(192, 192, 192 + 24);
            worldMapReplacements[6][13] = new SnesColor(216, 216, 216 + 24);
            worldMapReplacements[6][14] = new SnesColor(168, 168, 168 + 24);

            worldMapReplacements[7] = new Dictionary<int, SnesColor>();
            worldMapReplacements[7][10] = new SnesColor(192, 192, 192 + 24);
            worldMapReplacements[7][11] = new SnesColor(200, 200, 200 + 24);
            worldMapReplacements[7][12] = new SnesColor(208, 208, 208 + 24);
            worldMapReplacements[7][13] = new SnesColor(216, 216, 216 + 24);
            worldMapReplacements[7][14] = new SnesColor(192, 192, 192 + 24);
            worldMapReplacements[7][15] = new SnesColor(176, 176, 176 + 24);

            worldMapReplacements[12] = new Dictionary<int, SnesColor>();
            worldMapReplacements[12][1] = new SnesColor(176, 176, 200);
            worldMapReplacements[12][2] = new SnesColor(192, 192, 216);
            worldMapReplacements[12][3] = new SnesColor(216, 216, 240);

            // ice country
            worldMapReplacements[11] = new Dictionary<int, SnesColor>();
            worldMapReplacements[11][7] = new SnesColor(176, 176, 192);
            worldMapReplacements[11][8] = new SnesColor(168, 168, 200);
            worldMapReplacements[11][9] = new SnesColor(168, 168, 208);
            worldMapReplacements[11][10] = new SnesColor(136, 136, 200);
            worldMapReplacements[11][11] = new SnesColor(120, 120, 168);

            worldMapReplacements[11][12] = new SnesColor(232, 232, 232);
            worldMapReplacements[11][13] = new SnesColor(224, 224, 224);
            worldMapReplacements[11][14] = new SnesColor(240, 240, 240);
            worldMapReplacements[11][15] = new SnesColor(184, 184, 184);

            worldMapReplacements[10] = new Dictionary<int, SnesColor>();
            worldMapReplacements[10][6] = new SnesColor(224, 224, 248);
            worldMapReplacements[10][7] = new SnesColor(208, 208, 232);
            worldMapReplacements[10][8] = new SnesColor(216, 216, 240);
            worldMapReplacements[10][9] = new SnesColor(232, 232, 232);
            worldMapReplacements[10][10] = new SnesColor(224, 224, 224);

            // 69784 - tile to change into santa house -> 72?
            outRom[0x69784] = 0x72;
            // change map 80 in landables to 332 outside santa house
            // 67780; block 1A, 1B
            int outsideMapNum = 332;
            int outsideX = 13;
            int outsideY = 16;
            outRom[0x677F2] = (byte)outsideMapNum;
            outRom[0x677F3] = (byte)((outsideMapNum >> 8) | (outsideX << 1));
            outRom[0x677F4] = (byte)(outsideY << 1);
            outRom[0x677F5] = (byte)outsideMapNum;
            outRom[0x677F6] = (byte)((outsideMapNum >> 8) | (outsideX << 1));
            outRom[0x677F7] = (byte)(outsideY << 1);

            // 67b18 - takeoff from santa house
            // 98 16 -> a8 1b
            outRom[0x67b18] = 0xA8;
            outRom[0x67b19] = 0x1B;
            foreach (int worldMapPalOffset in worldMapPalOffsets)
            {
                for (int palnum = 0; palnum < 13; palnum++)
                {
                    for (int col = 1; col < 16; col++)
                    {
                        byte p1 = origRom[worldMapPalOffset + palnum * 30 + col * 2];
                        byte p2 = origRom[worldMapPalOffset + palnum * 30 + col * 2 + 1];
                        SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });
                        if (worldMapReplacements.ContainsKey(palnum))
                        {
                            if (worldMapReplacements[palnum].ContainsKey(col))
                            {
                                thisCol = worldMapReplacements[palnum][col];
                            }
                        }


                        if (palnum == 1 || palnum == 2 || palnum == 3)
                        {
                            thisCol.add(-192, -192, -192);
                        }
                        else
                        {
                            thisCol.add(-64, -64, -64);
                        }

                        // just make em all the day palette
                        if (worldMapPalOffset == 0x67DFE)
                        {
                            // inject modified colors
                            thisCol.put(outRom, worldMapPalOffset + palnum * 30 + col * 2);
                        }
                        else
                        {
                            // copy the other palette
                            outRom[worldMapPalOffset + palnum * 30 + col * 2] = outRom[0x67DFE + palnum * 30 + col * 2];
                            outRom[worldMapPalOffset + palnum * 30 + col * 2 + 1] = outRom[0x67DFE + palnum * 30 + col * 2 + 1];
                        }
                    }
                }
            }
        }
    }
}
