using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.hacks.vanillarando
{
    /// <summary>
    /// Halloween enemy and map color changes for vanilla rando.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FallTheme : RandoProcessor
    {
        protected override string getName()
        {
            return "Fall theme for vanilla rando";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(settings.get(VanillaRandoSettings.PROPERTYNAME_SPECIAL_MODE) != "halloween")
            {
                return false;
            }
            Random r = context.randomFunctional;
            byte[] enemies = new byte[] {
                10,10,10,10, // blat
                13,13,13,13, // chair
                14, // ma goblin
                18,18,18,18, // zombie
                21,21,21,21, // owl
                23,23,23,23, // pumpkin
                30,30,30,30, // bat
                31,31,31,31, // werewolf
                33, // sword
                35, // book
                45, // mimicbox
                50, // spiderlegs?
                51, // weepy eye
                54,54,54,54, // ghoul
                55, // imp
                59, // griffin hand
                66,66,66,66, // pumpkin
                70, // gremlin
                73, // whimper
                80,80,80,80, // wolflord
                81, // doomsword
            };
            // swapping enemies for duplicates - swap the whole block in bank 10? 0x10 bytes at 100000 .. will weapons work though? should if i copy the weapon data
            int[] noSwapEnemies = new int[]
            {
                32, // sprite boss
                46, // boy boss
                63, // girl boss
                50, // spider legs
                9, // spectre
                15, // dark funk
                29, // LA funk
                48, // ghost
            };

            for(int i=0; i < 84; i++)
            {
                if(!noSwapEnemies.Contains(i) && !enemies.Contains((byte)i))
                {
                    // graphics, ai, etc
                    byte newEnemy = enemies[r.Next() % enemies.Length];
                    for(int j=0; j < 16; j++)
                    {
                        outRom[0x100000 + i * 16 + j] = origRom[0x100000 + newEnemy * 16 + j];
                    }

                    // weapon
                    byte sourceSrWeapon = origRom[0x101C00 + newEnemy * 29 + 23];
                    byte sourceLrWeapon = origRom[0x101C00 + newEnemy * 29 + 24];
                    byte destSrWeapon = origRom[0x101C00 + i * 29 + 23];
                    byte destLrWeapon = origRom[0x101C00 + i * 29 + 24];

                    // 101000, 12 bytes each
                    outRom[0x101000 + destSrWeapon * 12 + 0] = origRom[0x101000 + sourceSrWeapon * 12 + 0]; // type
                    outRom[0x101000 + destLrWeapon * 12 + 0] = origRom[0x101000 + sourceLrWeapon * 12 + 0]; // type

                    outRom[0x101000 + destSrWeapon * 12 + 1] = origRom[0x101000 + sourceSrWeapon * 12 + 1]; // stat A
                    outRom[0x101000 + destLrWeapon * 12 + 1] = origRom[0x101000 + sourceLrWeapon * 12 + 1]; // stat A

                    outRom[0x101000 + destSrWeapon * 12 + 2] = origRom[0x101000 + sourceSrWeapon * 12 + 2]; // stat B
                    outRom[0x101000 + destLrWeapon * 12 + 2] = origRom[0x101000 + sourceLrWeapon * 12 + 2]; // stat B

                    outRom[0x101000 + destSrWeapon * 12 + 3] = origRom[0x101000 + sourceSrWeapon * 12 + 3]; // proj
                    outRom[0x101000 + destLrWeapon * 12 + 3] = origRom[0x101000 + sourceLrWeapon * 12 + 3]; // proj

                    // palette?
                    // 80FFE, 30 bytes
                    for(int j=0; j < 30; j++)
                    {
                        outRom[0x80FFE + i * 30 + j] = origRom[0x80FFE + newEnemy * 30 + j];
                    }

                    // enemy names
                    context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + i, context.namesOfThings.getOriginalName(NamesOfThings.INDEX_ENEMIES_START + newEnemy));
                }
            }

            // drops // 103A50, 5 bytes .. change [3] to 0x40 or 0x41
            for (int i=0; i < 84; i++)
            {
                outRom[0x103A50 + i * 5 + 2] = 0xF8;
                outRom[0x103A50 + i * 5 + 3] = (byte)(0x40 + (r.Next() % 2));
            }

            // increase drop chance
            outRom[0x428E] = 0x0F;

            // candy and chocolate are free
            outRom[0x18FB9C] = 0;
            outRom[0x18FB9D] = 0;
            outRom[0x18FB9E] = 0;
            outRom[0x18FB9F] = 0;

            // change a bunch of map palettes
            int[] changePaletteSets1 = new int[] { 24, 25, 26, 29, 35, 57, 59, 61, 62, 78, 81, 112 };
            int palStartOffset = 0xC7FE0;

            int srcPal = 65;
            List<List<int>> palCopyIndexes1 = new List<List<int>>();
            palCopyIndexes1.Add(new int[] { 1, 2, 3, 4, 5, 6 }.ToList());
            palCopyIndexes1.Add(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes1.Add(new int[] { 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes1.Add(new int[] { 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes1.Add(new int[] { 10, 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes1.Add(new int[] { 11, 12, 13, 14, 15 }.ToList());
            palCopyIndexes1.Add(new int[] { 6, 7 }.ToList());
            foreach (int palSet in changePaletteSets1)
            {
                for(int palnum = 1; palnum < 8; palnum++)
                {
                    for (int col = 1; col < 16; col++)
                    {
                        if (palCopyIndexes1[palnum - 1].Contains(col))
                        {
                            byte p1 = origRom[palStartOffset + srcPal * 30 * 7 + palnum * 30 + col * 2];
                            byte p2 = origRom[palStartOffset + srcPal * 30 * 7 + palnum * 30 + col * 2 + 1];
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
            int[] changeDisplaySets = new int[] { 1, 4, 9, 19, 21 };
            foreach (int palSet in combinedPalettes)
            {
                if (allOutdoorPaletteSets.Contains(palSet))
                {
                    for (int palnum = 1; palnum < 8; palnum++)
                    {
                        for (int col = 1; col < 16; col++)
                        {
                            byte p1 = outRom[palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2];
                            byte p2 = outRom[palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2 + 1];
                            SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });

                            bool mostlyBlue = thisCol.getBlue() > thisCol.getRed() - 16 && thisCol.getBlue() > thisCol.getGreen() - 16;
                            if (snowyPalettes.Contains(palSet))
                            {
                                thisCol.scale(0.5, 0.4, 0.7);
                            }
                            else
                            {
                                if (!mostlyBlue || !(changePaletteSets1.Contains(palSet) || changePaletteSets2.Contains(palSet)))
                                {
                                    byte red = thisCol.getRed();
                                    thisCol.setGreen((byte)((thisCol.getRed() + thisCol.getGreen()) / 2));
                                    thisCol.scale(1.0, 0.65, 0.7);
                                    thisCol.add(-16, -16, -16);
                                }
                                else
                                {
                                    thisCol.add(-64, -64, -64);
                                    if (palnum == 6 || palnum == 7)
                                    {
                                        byte prevR = thisCol.getRed();
                                        thisCol.setRed(thisCol.getBlue());
                                        thisCol.setGreen(0);
                                        thisCol.setBlue(0);
                                    }
                                }
                            }
                            thisCol.put(outRom, palStartOffset + palSet * 30 * 7 + palnum * 30 + col * 2);
                        }
                    }
                }

                // animated palettes
                int animIndex = origRom[0x80802 + palSet] + 1;
                int animPalIndex = origRom[0x8087B + animIndex];
                int animPalOffset = animPalIndex * 0x1E + 0x808E4;

                for (int palnum = 0; palnum < 8; palnum++)
                {
                    for (int col = 0; col < 16; col++)
                    {
                        int off = animPalOffset + palnum * 30 + col * 2;
                        // why do some of these overwrite enemy palettes?  bounds check them here..
                        if ((off < 0x80FFE || off > 0x819D6) && animPalIndex > 0)
                        {
                            byte p1 = origRom[animPalOffset + palnum * 30 + col * 2];
                            byte p2 = origRom[animPalOffset + palnum * 30 + col * 2 + 1];
                            SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });

                            bool mostlyBlue = thisCol.getBlue() > thisCol.getRed() - 16 && thisCol.getBlue() > thisCol.getGreen() - 16;
                            if (snowyPalettes.Contains(palSet))
                            {
                                thisCol.scale(0.5, 0.4, 0.7);
                            }
                            else
                            {
                                if (!mostlyBlue || !(changePaletteSets1.Contains(palSet) || changePaletteSets2.Contains(palSet)))
                                {
                                    byte red = thisCol.getRed();
                                    thisCol.setGreen((byte)((thisCol.getRed() + thisCol.getGreen()) / 2));
                                    thisCol.scale(1.0, 0.65, 0.7);
                                    thisCol.add(-16, -16, -16);
                                }
                                else
                                {
                                    thisCol.add(-64, -64, -64);
                                    if (palnum == 6 || palnum == 7)
                                    {
                                        byte prevR = thisCol.getRed();
                                        thisCol.setRed(thisCol.getBlue());
                                        thisCol.setGreen(0);
                                        thisCol.setBlue(0);
                                    }
                                }
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
                byte red = thisCol.getRed();
                thisCol.setGreen((byte)((thisCol.getRed() + thisCol.getGreen()) / 2));
                thisCol.scale(1.0, 0.65, 0.7);
                thisCol.add(-16, -16, -16);
                thisCol.put(outRom, 0x80100 + displaySet * 9 + 7);
            }


            // world map
            // 13 palettes
            int[] worldMapPalOffsets = new int[] { 0x6483E, 0x67DFE, };
            foreach(int worldMapPalOffset in worldMapPalOffsets)
            {
                for (int palnum = 0; palnum < 13; palnum++)
                {
                    for (int col = 1; col < 16; col++)
                    {
                        byte p1 = origRom[worldMapPalOffset + palnum * 30 + col * 2];
                        byte p2 = origRom[worldMapPalOffset + palnum * 30 + col * 2 + 1];
                        SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });

                        bool mostlyBlue = thisCol.getBlue() > thisCol.getRed() - 16 && thisCol.getBlue() > thisCol.getGreen() - 16;
                        if (!mostlyBlue)
                        {
                            byte red = thisCol.getRed();
                            thisCol.setGreen((byte)((thisCol.getRed() + thisCol.getGreen()) / 2));
                            thisCol.scale(1.0, 0.5, 0.65);
                        }
                        else
                        {
                            thisCol.add(-64, -64, -64);
                        }

                        if(palnum == 1 || palnum == 2 || palnum == 3)
                        {
                            thisCol.add(-96, -96, -96);
                            thisCol.setRed((byte)(255 - thisCol.getRed()));
                            thisCol.add(-96, -96, -96);
                        }

                        // maybe just make em all the night palette
                        if (worldMapPalOffset == 0x6483E)
                        {
                            thisCol.put(outRom, worldMapPalOffset + palnum * 30 + col * 2);
                        }
                        else
                        {
                            outRom[worldMapPalOffset + palnum * 30 + col * 2] = outRom[0x6483E + palnum * 30 + col * 2];
                            outRom[worldMapPalOffset + palnum * 30 + col * 2 + 1] = outRom[0x6483E + palnum * 30 + col * 2 + 1];
                        }
                    }
                }
            }
            return true;
        }
    }
}
