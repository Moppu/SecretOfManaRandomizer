using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.processing.hacks.common.util;
using SoMRandomizer.util;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.hacks.vanillarando
{
    public class WinterTheme : RandoProcessor
    {
        protected override string getName()
        {
            return "Winter theme for vanilla rando";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if (settings.get(VanillaRandoSettings.PROPERTYNAME_SPECIAL_MODE) != "xmas")
            {
                return false;
            }

            bool statusGlow = settings.getBool(CommonSettings.PROPERTYNAME_STATUSGLOW);

            if (!statusGlow)
            {
                // acid storm
                // replace with slow .. e190 x04
                // C8/EB44:	    BD B0 E1       LDA $E1B0,X
                // replace:
                // C8/EB47:     29 FC          AND #$FC
                // C8/EB49:     09 02          ORA #$02
                // C8/EB4B:     9D B0 E1       STA $E1B0,X
                outRom[0x8EB47] = 0x22;
                outRom[0x8EB48] = (byte)(context.workingOffset);
                outRom[0x8EB49] = (byte)(context.workingOffset >> 8);
                outRom[0x8EB4A] = (byte)((context.workingOffset >> 16) + 0xC0);
                outRom[0x8EB4B] = 0xEA;
                outRom[0x8EB4C] = 0xEA;
                outRom[0x8EB4D] = 0xEA;

                // (replaced code)
                // AND #FC
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFC;
                // ORA #02
                outRom[context.workingOffset++] = 0x09;
                outRom[context.workingOffset++] = 0x02;
                // STA $E1B0,X
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0xE1;
                // slow
                // LDA $E1B0,X
                outRom[context.workingOffset++] = 0xBD;
                outRom[context.workingOffset++] = 0x90;
                outRom[context.workingOffset++] = 0xE1;
                // ORA #04
                outRom[context.workingOffset++] = 0x09;
                outRom[context.workingOffset++] = 0x04;
                // STA $E1B0,X
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x90;
                outRom[context.workingOffset++] = 0xE1;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            context.namesOfThings.setName(NamesOfThings.INDEX_SPELLS_START + 7, "FREEZING RAIN");
            for (int i = 0; i < 15; i++)
            {
                SnesColor col = new SnesColor(outRom, 0x11FDB2 + i * 2);
                col.add(-128, -128, 128);
                col.put(outRom, 0x11FDB2 + i * 2);
            }

            for (int i = 0; i < 5; i++)
            {
                SnesColor col = new SnesColor(outRom, 0x12cf72 + i * 2);
                col.add(-128, -128, 128);
                col.put(outRom, 0x12cf72 + i * 2 + 1);
            }

            for (int i = 0; i < 15; i++)
            {
                SnesColor col = new SnesColor(outRom, 0x81FD2 + i * 2);
                col.add(-128, -128, 128);
                col.put(outRom, 0x81FD2 + i * 2);
            }


            Dictionary<int, Dictionary<int, SnesColor>> enemyColorChanges = new Dictionary<int, Dictionary<int, SnesColor>>();

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
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 1, "Jingle Bee");

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
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 4, "Poinsettia");

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
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 5, "Frozen Fish");

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
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x7, "Festive Eye Spy");

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
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x12, "Zombie Caroler");

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
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x16, "Pebbler Grinch");

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
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x18, "Christmas Crab");

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
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x19, "Gingerhorse");

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
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 0x2D, "Jolly Mimic");

            foreach (int enemyNum in enemyColorChanges.Keys)
            {
                Dictionary<int, SnesColor> enemyColors = enemyColorChanges[enemyNum];
                foreach (int palIndex in enemyColors.Keys)
                {
                    SnesColor c = enemyColors[palIndex];
                    int location = 0x80FFE + (enemyNum) * 0x1E + (palIndex) * 2;
                    c.put(outRom, location);

                }
            }

            int palStartOffset = 0xC7FE0;

            int kakkaraReplacementPal = 49;
            int kakkaraOriginalPal = 71;
            for (int palnum = 1; palnum < 8; palnum++)
            {
                for (int col = 1; col < 16; col++)
                {
                    byte p1 = outRom[palStartOffset + kakkaraReplacementPal * 30 * 7 + palnum * 30 + col * 2];
                    byte p2 = outRom[palStartOffset + kakkaraReplacementPal * 30 * 7 + palnum * 30 + col * 2 + 1];
                    SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });

                    if (palnum != 4 || col < 6)
                    {
                        thisCol.put(outRom, palStartOffset + kakkaraOriginalPal * 30 * 7 + palnum * 30 + col * 2);
                    }
                }
            }

            // make some maps look like icy versions of themselves just by swapping palettes
            for (int i = 16; i < 500; i++)
            {
                MapHeader header = VanillaMapUtil.getHeader(origRom, i);

                int mapTileSet = header.getTileset16();
                if (mapTileSet == 10)
                {
                    // change caves to use undine cave palette
                    header.setPaletteSet(101);
                    VanillaMapUtil.putHeader(outRom, header, i);
                }
                else if (mapTileSet == 14)
                {
                    // change castle exteriors to use ice castle exterior palette
                    header.setPaletteSet(84);
                    VanillaMapUtil.putHeader(outRom, header, i);
                }
                else if (mapTileSet == 15)
                {
                    // change castle/ruins interiors to use ice castle interior palette
                    header.setPaletteSet(85);
                    VanillaMapUtil.putHeader(outRom, header, i);
                }
                else if (mapTileSet == 30)
                {
                    // change pure lands to use the normal palette .. for some reason?  i don't remember why this one is here
                    header.setPaletteSet(54);
                    VanillaMapUtil.putHeader(outRom, header, i);
                }
            }

            int[] modPalSets = new int[] { 101, 54 };
            foreach (int cavePalSet in modPalSets)
            {
                for (int palnum = 1; palnum < 8; palnum++)
                {
                    for (int col = 1; col < 16; col++)
                    {
                        byte p1 = outRom[palStartOffset + cavePalSet * 30 * 7 + palnum * 30 + col * 2];
                        byte p2 = outRom[palStartOffset + cavePalSet * 30 * 7 + palnum * 30 + col * 2 + 1];
                        SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });
                        if (thisCol.getRed() != 0 || thisCol.getGreen() != 0 || thisCol.getBlue() != 0)
                        {
                            thisCol.scale(1.0, 1.0, 2.0);
                            thisCol.add(32, 32, 32);
                        }
                        thisCol.put(outRom, palStartOffset + cavePalSet * 30 * 7 + palnum * 30 + col * 2);
                    }
                }
            }

            // drop random gear and consumables
            new XmasDrops().process(outRom, ref context.workingOffset);

            int[] changePaletteSets1 = new int[] { 24, 25, 26, 29, 35, 36, 57, 59, 61, 62, 78, 81, 112 };

            byte[] giftBoxGraphics = DataUtil.readResource("SoMRandomizer.Resources.giftbox_tiles.bin");
            for (int i = 0; i < giftBoxGraphics.Length; i++)
            {
                outRom[i + 0x186540] = giftBoxGraphics[i];
            }

            int[] srcPal1DarkenIndexes = new int[] { 1, 2, 3, 4, 5, 6 };
            int[] srcPal3DarkenIndexes = new int[] { 1, 2, 3, 7, 11, 12, 13, 14 };
            int[] srcPal5DarkenIndexes = new int[] { 10, 11, 12, 13, 14, 15 };
            int srcPal = 66;
            List<List<int>> palCopyIndexes1 = new List<List<int>>();
            palCopyIndexes1.Add(new int[] { 1, 2, 3, 4, 5, 6 }.ToList());
            palCopyIndexes1.Add(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes1.Add(new int[] { 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes1.Add(new int[] { 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes1.Add(new int[] { 7, 8, 9, 10, 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes1.Add(new int[] { 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes1.Add(new int[] { 6, 7 }.ToList());
            foreach (int palSet in changePaletteSets1)
            {
                for (int palnum = 1; palnum < 8; palnum++)
                {
                    for (int col = 1; col < 16; col++)
                    {
                        if (palnum == 1 && srcPal1DarkenIndexes.Contains(col))
                        {
                            byte p1 = outRom[palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2];
                            byte p2 = outRom[palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2 + 1];
                            SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });
                            thisCol.add(-32, -32, -32);
                            thisCol.put(outRom, palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2);
                        }
                        if (palCopyIndexes1[palnum - 1].Contains(col))
                        {
                            byte p1 = outRom[palStartOffset + srcPal * 30 * 7 + palnum * 30 + col * 2];
                            byte p2 = outRom[palStartOffset + srcPal * 30 * 7 + palnum * 30 + col * 2 + 1];
                            if ((palnum == 1 && srcPal1DarkenIndexes.Contains(col))
                                || (palnum == 3 && srcPal3DarkenIndexes.Contains(col)))
                            {
                                SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });
                                thisCol.add(-32, -32, -32);
                                thisCol.put(outRom, palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2);
                            }
                            else if ((palnum == 5 && srcPal5DarkenIndexes.Contains(col)))
                            {
                                SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });
                                thisCol.add(-16, -16, -16);
                                thisCol.put(outRom, palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2);
                            }

                            outRom[palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2] = p1;
                            outRom[palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2 + 1] = p2;
                        }
                    }
                }
            }


            int[] changePaletteSets2 = new int[] { 98, 99, 55, };

            List<List<int>> palCopyIndexes2 = new List<List<int>>();
            palCopyIndexes2.Add(new int[] { 1, 2, 3, 4, 5, 6 }.ToList());
            palCopyIndexes2.Add(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes2.Add(new int[] { 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes2.Add(new int[] { 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes2.Add(new int[] { 10, 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes2.Add(new int[] { }.ToList());
            palCopyIndexes2.Add(new int[] { }.ToList());
            foreach (int palSet in changePaletteSets2)
            {
                for (int palnum = 1; palnum < 8; palnum++)
                {
                    for (int col = 1; col < 16; col++)
                    {
                        if (palCopyIndexes2[palnum - 1].Contains(col))
                        {
                            byte p1 = origRom[palStartOffset + srcPal * 30 * 7 + palnum * 30 + col * 2];
                            byte p2 = origRom[palStartOffset + srcPal * 30 * 7 + palnum * 30 + col * 2 + 1];
                            outRom[palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2] = p1;
                            outRom[palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2 + 1] = p2;
                        }
                    }
                }
            }

            int[] allOutdoorPaletteSets = new int[] { 24, 25, 26, 27, 29, 32, 31, 35, 36, 37, 38, 40, 41, 48, 49, 54, 55, 57, 59, 61, 62, 63, 64, 65, 66, 68, 71, 72, 74, 77, 78, 81, 82, 83, 84, 94, 98, 99, 104, 107, 109, 110, 112, };
            int[] cavePalSets = new int[] { 101 };

            List<int> combinedPalettes = new List<int>();
            combinedPalettes.AddRange(allOutdoorPaletteSets);
            combinedPalettes.AddRange(cavePalSets);

            int[] snowyPalettes = new int[] { 37, 49, 77, 84, 107 };

            int[] changeDisplaySets = new int[] { 1, 4, 9, 19, 21, };

            double scaleR = 0.75;
            double scaleG = 0.7;
            double scaleB = 0.85;

            foreach (int palSet in combinedPalettes)
            {
                if (allOutdoorPaletteSets.Contains(palSet))
                {
                    for (int palnum = 1; palnum < 8; palnum++)
                    {
                        Logging.log("Writing non-animated palette at " + (palStartOffset + palSet * 30 * 7 + palnum * 30).ToString("X6"), "debug");
                        for (int col = 1; col < 16; col++)
                        {
                            byte p1 = outRom[palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2];
                            byte p2 = outRom[palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2 + 1];
                            SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });
                            thisCol.scale(scaleR, scaleG, scaleB);
                            thisCol.put(outRom, palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2);
                        }
                    }
                }

                int animIndex = origRom[0x80802 + palSet] + 1;
                int animPalIndex = origRom[0x8087B + animIndex];
                int animPalOffset = animPalIndex * 0x1E + 0x808E4;

                for (int palnum = 0; palnum < 7; palnum++)
                {
                    Logging.log("Writing animated palette at " + (animPalOffset + palnum * 30).ToString("X6"), "debug");
                    for (int col = 0; col < 15; col++)
                    {
                        int off = animPalOffset + palnum * 30 + col * 2;
                        if (off < 0x80FFE || off > 0x819D6)
                        {
                            byte p1 = origRom[animPalOffset + palnum * 30 + col * 2];
                            byte p2 = origRom[animPalOffset + palnum * 30 + col * 2 + 1];
                            SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });
                            if (allOutdoorPaletteSets.Contains(palSet))
                            {
                                thisCol.scale(scaleR, scaleG, scaleB);
                            }
                            else
                            {
                                thisCol.scale(1.0, 1.0, 2.0);
                                thisCol.add(32, 32, 32);
                            }
                            thisCol.put(outRom, animPalOffset + palnum * 30 + col * 2);
                        }
                    }
                }
            }

            // color is [7,8] of 9 byte block, 0x80100
            foreach (int displaySet in changeDisplaySets)
            {
                byte p1 = origRom[0x80100 + displaySet * 9 + 7];
                byte p2 = origRom[0x80100 + displaySet * 9 + 8];
                SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });
                if (displaySet == 9)
                {
                    thisCol.scale(scaleR, scaleG, scaleB);
                    thisCol.put(outRom, 0x80100 + displaySet * 9 + 7);
                }
                else if (displaySet == 21)
                {
                    thisCol.scale(1.0, 1.0, 2.0);
                    thisCol.add(32, 32, 32);
                    thisCol.put(outRom, 0x80100 + displaySet * 9 + 7);
                }
                else
                {
                    // palette 6 index 0 from set 66
                    thisCol.setRed(112);
                    thisCol.setGreen(112);
                    thisCol.setBlue(128);
                    thisCol.scale(scaleR, scaleG, scaleB);
                    thisCol.put(outRom, 0x80100 + displaySet * 9 + 7);
                }

            }

            outRom[0x90A4] = 0x08;
            outRom[0x90B5] = 0x00;

            // world map
            // 13 palettes
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

            worldMapReplacements[10] = new Dictionary<int, SnesColor>();
            worldMapReplacements[10][6] = new SnesColor(224, 224, 248);
            worldMapReplacements[10][7] = new SnesColor(208, 208, 232);
            worldMapReplacements[10][8] = new SnesColor(216, 216, 240);

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

                        // just make em all the day palette
                        if (worldMapPalOffset == 0x67DFE)
                        {
                            thisCol.put(outRom, worldMapPalOffset + palnum * 30 + col * 2);
                        }
                        else
                        {
                            outRom[worldMapPalOffset + palnum * 30 + col * 2] = outRom[0x67DFE + palnum * 30 + col * 2];
                            outRom[worldMapPalOffset + palnum * 30 + col * 2 + 1] = outRom[0x67DFE + palnum * 30 + col * 2 + 1];
                        }
                    }
                }
            }
            return true;
        }
    }
}
