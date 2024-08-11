using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.openworld;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Randomize map palette colors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MapPaletteRandomizations : RandoProcessor
    {
        protected override string getName()
        {
            return "Map palette randomization";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random random = context.randomCosmetic;
            if(!settings.getBool(OpenWorldSettings.PROPERTYNAME_RANDOMIZE_COLORS))
            {
                return false;
            }

            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);

            if (goal == OpenWorldGoalProcessor.GOAL_GIFTMODE || goal == OpenWorldGoalProcessor.GOAL_REINDEER || settings.getBool(OpenWorldSettings.PROPERTYNAME_XMAS_DECO))
            {
                Logging.log("Ignoring map palette rando because we did christmas palettes");
                return false;
            }
            Dictionary<string, byte[]> groupedPalettes = new Dictionary<string, byte[]>();
            groupedPalettes.Add("cave", new byte[]
            {
                34, // gaia's navel
                101, // undine cave
            });

            // 54? purelands water
            groupedPalettes.Add("purelands", new byte[]
            {
                54, // purelands A
                104, // purelands B
            });

            Dictionary<byte, byte> palSetToDispSetting = new Dictionary<byte, byte>();
            palSetToDispSetting[25] = 4;
            palSetToDispSetting[26] = 1;
            palSetToDispSetting[36] = 9; // sky over jehk area
            palSetToDispSetting[103] = 18; // mana tree
            palSetToDispSetting[109] = 19; // ntc top sky
            palSetToDispSetting[108] = 20; // kakkara cannon guy
            palSetToDispSetting[118] = 23; // ending

            byte[] dontChangePalettes = new byte[]
            {
                37, // ice country
                97, // dryad palace
                89, // gnome palace (for orb)
                88, // undine
                47, // sylphid
                91, // salamando
                93, // lumina
                92, // shade
                95, // luna
            };

            Dictionary<byte, double> prevH = new Dictionary<byte, double>();
            Dictionary<byte, double> prevS = new Dictionary<byte, double>();
            Dictionary<byte, double> prevV = new Dictionary<byte, double>();

            int palStartOffset = 0xC7FE0;

            for (byte paletteSet = 24; paletteSet < 120; paletteSet++)
            {
                if (!dontChangePalettes.Contains(paletteSet))
                {
                    double hueMax = 250;

                    double hChange = ((random.Next() % (hueMax * 2)) / 1.0) - hueMax; // really funky hue
                    double sChange = -Math.Abs(hChange) / 1250.0; // hue-dependent; undersaturate if more change
                    double vChange = -Math.Abs(hChange) / 1250.0; // hue-dependent; darker if more change
                    if (palSetToDispSetting.ContainsKey(paletteSet))
                    {
                        byte dispSetNum = palSetToDispSetting[paletteSet];
                        // 80100
                        // 9 bytes each [7][8] = color
                        byte p1 = outRom[0x80100 + dispSetNum * 9 + 7];
                        byte p2 = outRom[0x80100 + dispSetNum * 9 + 8];
                        SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });
                        double h,s,v;
                        ColorUtil.rgbToHsv(thisCol.getRed(), thisCol.getGreen(), thisCol.getBlue(), out h, out s, out v);
                        h += hChange;
                        s += sChange;
                        if (s < 0.2)
                        {
                            s = 0.2;
                        }
                        v += vChange;

                        int r, g, b;
                        ColorUtil.hsvToRgb(h, s, v, out r, out g, out b);
                        thisCol.setRed((byte)r);
                        thisCol.setGreen((byte)g);
                        thisCol.setBlue((byte)b);
                        thisCol.put(outRom, 0x80100 + dispSetNum * 9 + 7);
                    }

                    bool doAnimation = false;
                    foreach (byte[] groupedPals in groupedPalettes.Values)
                    {
                        if (groupedPals.Contains(paletteSet))
                        {
                            if (groupedPals.ToList().IndexOf(paletteSet) == 0)
                            {
                                doAnimation = true;
                                prevH[paletteSet] = hChange;
                                prevS[paletteSet] = sChange;
                                prevV[paletteSet] = vChange;
                            }
                            else
                            {
                                hChange = prevH[groupedPals[0]];
                                sChange = prevS[groupedPals[0]];
                                vChange = prevV[groupedPals[0]];
                            }
                        }
                    }

                    for (int palnum = 1; palnum < 8; palnum++)
                    {
                        for (int col = 1; col < 16; col++)
                        {
                            byte p1 = outRom[palStartOffset + paletteSet * 30 * 7 + palnum * 30 + col * 2];
                            byte p2 = outRom[palStartOffset + paletteSet * 30 * 7 + palnum * 30 + col * 2 + 1];
                            SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });
                            double h, s, v;
                            ColorUtil.rgbToHsv(thisCol.getRed(), thisCol.getGreen(), thisCol.getBlue(), out h, out s, out v);
                            h += hChange;
                            s += sChange;
                            if (s < 0.2)
                            {
                                s = 0.2;
                            }
                            v += vChange;

                            int r, g, b;
                            ColorUtil.hsvToRgb(h, s, v, out r, out g, out b);
                            thisCol.setRed((byte)r);
                            thisCol.setGreen((byte)g);
                            thisCol.setBlue((byte)b);
                            thisCol.put(outRom, palStartOffset + paletteSet * 30 * 7 + palnum * 30 + col * 2);
                        }
                    }

                    if (doAnimation)
                    {
                        // 101 - cave - 80867 -> x56 (next one is x58) -> 808D1 -> D700?
                        // 34  - cave - 80824 -> x09 (next one is x0B) -> 80884 -> D700?
                        // 54 purelands - 80838 -> 2B (2D) -> 808A6 -> CF0E, also at 808D3 (58) <- 8086A - 104 - map 207
                        int animIndex = outRom[0x80802 + paletteSet] + 1;
                        int animIndexNext = outRom[0x80802 + paletteSet + 1] + 1;
                        int index = animIndex;
                        {
                            int animPalIndex = outRom[0x8087B + index];
                            if (animPalIndex >= 0x80)
                            {
                                animPalIndex = outRom[0x8087B + index + 1];
                            }
                            int animPalOffset = animPalIndex * 0x1E + 0x808E4;

                            for (int palnum = 0; palnum < 7; palnum++)
                            {
                                for (int col = 0; col < 15; col++)
                                {
                                    int off = animPalOffset + palnum * 30 + col * 2;
                                    if (off < 0x80FFE || off > 0x819D6)
                                    {
                                        byte p1 = outRom[animPalOffset + palnum * 30 + col * 2];
                                        byte p2 = outRom[animPalOffset + palnum * 30 + col * 2 + 1];
                                        SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });

                                        double h, s, v;
                                        ColorUtil.rgbToHsv(thisCol.getRed(), thisCol.getGreen(), thisCol.getBlue(), out h, out s, out v);
                                        h += hChange;
                                        s += sChange;
                                        if (s < 0.2)
                                        {
                                            s = 0.2;
                                        }
                                        v += vChange;

                                        int r, g, b;
                                        ColorUtil.hsvToRgb(h, s, v, out r, out g, out b);
                                        thisCol.setRed((byte)r);
                                        thisCol.setGreen((byte)g);
                                        thisCol.setBlue((byte)b);
                                        thisCol.put(outRom, animPalOffset + palnum * 30 + col * 2);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
