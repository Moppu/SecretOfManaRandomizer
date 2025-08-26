using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.util;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Randomizes the flavor and associated color of the Lime Slime boss.  No other effect.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class LimeSlimeFlavors : RandoProcessor
    {
        protected override string getName()
        {
            return "Random flavor for lime slime";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random random = context.randomCosmetic;
            // // index into 10bb24 (16 bit) -> bank 2 (0c -> 2f1dc for mantis ant for example)
            byte[] palIndexes = new byte[] {
                0x5b, // blue one (crystals?
                0x5c, // green one
                0x5d, // red one
                0x5e, // another blue one (slime?)
            };

            Dictionary<string, List<double>> flavors = new Dictionary<string, List<double>>()
            {
                { "Strawberry",
                    new double[] {
                        350.0, 1.2, 0.8, 0.0,
                        110.0, 0.7, 0.7, 0.0,
                    }.ToList() },
                    
                { "Watermelon",
                    new double[] {
                        110.0, 0.9, 0.7, 0.0,
                        340.0, 0.8, 0.8, 0.0,
                    }.ToList() },

                { "Grape",
                    new double[] {
                        (179 / 256.0) * 360.0, 1.2, 0.8, 0.0,
                        (189 / 256.0) * 360.0, 0.6, 0.7, 0.0,
                    }.ToList() },
                    
                { "Lemon",
                    new double[] {
                        (41 / 256.0) * 360.0, 1.2, 0.8, 0.0,
                        (59 / 256.0) * 360.0, 0.6, 0.7, 0.0,
                    }.ToList() },

                { "Blueberry",
                    new double[] {
                        (169 / 256.0) * 360.0, 1.2, 0.8, 0.0,
                        (179 / 256.0) * 360.0, 0.6, 0.7, 0.0,
                    }.ToList() },

                { "Cherry",
                    new double[] {
                        350.0, 0.9, 0.8, 0.0,
                        330.0, 0.8, 0.7, 0.0,
                    }.ToList() },

                { "Lime",
                    new double[] {
                        (68 / 256.0) * 360.0, 1.2, 0.8, 0.0,
                        (72 / 256.0) * 360.0, 0.6, 0.7, 0.0,
                    }.ToList() },

                { "Mint",
                    new double[] {
                        (124 / 256.0) * 360.0, 1.5, 1.5, 0.0,
                        (109 / 256.0) * 360.0, 0.6, 0.7, 0.0,
                    }.ToList() },

                { "Ketchup",
                    new double[] {
                        5.0, 2.5, 0.7, 0.2,
                        355.0, 0.8, 0.5, 0.0,
                    }.ToList() },

                { "Mustard",
                    new double[] {
                        (33 / 256.0) * 360.0, 1.5, 0.6, 0.3,
                        (29 / 256.0) * 360.0, 0.8, 0.2, 0.0,
                    }.ToList() },

                { "Egg",
                    new double[] {
                        350.0, 0.0, 2.0, 0.2,
                        (29 / 256.0) * 360.0, 0.7, 0.9, 0.0,
                    }.ToList() },

                { "Chocolate",
                    new double[] {
                        (16 / 256.0) * 360.0, 0.6, 0.9, -0.3,
                        (16 / 256.0) * 360.0, 0.7, 0.8, -0.2,
                    }.ToList() },

                { "Vanilla",
                    new double[] {
                        (166 / 256.0) * 360.0, 0.1, 0.6, 0.3,
                        (166 / 256.0) * 360.0, 0.1, 0.6, 0.3,
                    }.ToList() },

                { "Orange",
                    new double[] {
                        (16 / 256.0) * 360.0, 1.2, 0.6, 0.3,
                        (20 / 256.0) * 360.0, 0.6, 0.6, 0.3,
                    }.ToList() },

                { "Pineapple",
                    new double[] {
                        (41 / 256.0) * 360.0, 0.6, 0.6, 0.3,
                        (59 / 256.0) * 360.0, 0.6, 0.7, 0.2,
                    }.ToList() },

                { "Melon",
                    new double[] {
                        (59 / 256.0) * 360.0, 0.6, 0.6, 0.3,
                        (69 / 256.0) * 360.0, 0.6, 0.7, 0.2,
                    }.ToList() },

                { "Rainbow",
                    new double[] {
                        (0 / 256.0) * 360.0, 0.9, 0.6, 0.3,
                        (0 / 256.0) * 360.0, 0.9, 0.6, 0.3,
                    }.ToList() },
            };

            string flavor = flavors.Keys.ToList()[random.Next() % flavors.Count];
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 106, flavor + " Slime");

            Logging.log("Lime slime flavor: " + flavor, "spoiler");

            // slime part + middle part
            int[] palPartStarts = new int[] { 1, 12 };
            int[] palPartEnds = new int[] { 9, 15 };

            double[] origHues = new double[] {
                (160 / 256.0) * 360,
                (70 / 256.0) * 360,
                (0 / 256.0) * 360,
                (165 / 256.0) * 360,
            };


            for (int palPart = 0; palPart < 2; palPart++)
            {
                double targetH = flavors[flavor][0 + palPart * 4];
                double targetS = flavors[flavor][1 + palPart * 4];
                double targetV = flavors[flavor][2 + palPart * 4];
                double targetVAdd = flavors[flavor][3 + palPart * 4];

                foreach (byte paletteId in palIndexes)
                {
                    int paletteOffset = 0x20000 + DataUtil.ushortFromBytes(outRom, 0x10bb24 + paletteId * 2);
                    int paletteReadOffset = paletteOffset;
                    if(paletteId == 0x5e)
                    {
                        paletteReadOffset = 0x20000 + DataUtil.ushortFromBytes(outRom, 0x10bb24 + 0x5b * 2);
                    }

                    // fuck with the colors; leave index 0 and 14 out of it though
                    double avgHue;
                    
                    if(palPart == 1)
                    {
                        // middle part thing
                        avgHue = 10;
                    }
                    else
                    {
                        avgHue = origHues[paletteId - 0x5b];
                    }

                    double targetSValue = targetS;
                    double tH = targetH < 0 ? -1 : targetH;
                    double tS = targetSValue;
                    double tV = targetV;
                    double tA = targetVAdd;

                    double hueShift = tH < 0 ? -1 : avgHue - tH; // subtract this later

                    for (int i = palPartStarts[palPart]; i <= palPartEnds[palPart]; i++)
                    {
                        SnesColor color = new SnesColor(origRom, paletteReadOffset + i * 2);
                        ColorUtil.rgbToHsv(color.getRed(), color.getGreen(), color.getBlue(), out double h, out double s, out double v);

                        if(paletteId != 0x5e && palPart == 0)
                        {
                            v += 0.2;
                        }
                        else if(palPart == 0)
                        {
                            v += 0.1;
                        }
                        double vDiff = (1 - v) * tV;
                        v -= vDiff;
                        v += tA;
                        if (v < 0.1)
                        {
                            v = 0.1;
                        }
                        if (v > 1)
                        {
                            v = 1;
                        }

                        if (hueShift != -1)
                        {
                            h -= hueShift;
                        }
                        s *= tS;

                        if(palPart == 1)
                        {
                            // hue flatten
                            double diffFromTh = h - tH;
                            h -= diffFromTh * 0.7;
                        }

                        if(flavor == "Rainbow")
                        {
                            if (palPart == 1)
                            {
                                double ir = (i - palPartStarts[palPart]) / (double)(palPartEnds[palPart] - palPartStarts[palPart]);
                                h = ir * 360.0;
                            }
                            else
                            {
                                if (i == palPartStarts[palPart])
                                {
                                    v = 0;
                                }
                                else
                                {
                                    double ir = ((i - 1) - palPartStarts[palPart]) / (double)(palPartEnds[palPart] - palPartStarts[palPart]);
                                    h = ir * 360.0;
                                }
                            }
                        }
                        ColorUtil.hsvToRgb(h, s, v, out int r, out int g, out int b);
                        SnesColor colorNew = new SnesColor(r, g, b);
                        colorNew.put(outRom, paletteOffset + i * 2);
                    }

                }
            }

            return true;
        }
    }
}
