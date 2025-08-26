using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.processing.hacks.openworld;
using System;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// Process selection of open world christmas options.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldXmasHacks : RandoProcessor
    {
        public static string XMAS_PALETTES_INDICATOR = "didXmasPalettes";

        protected override string getName()
        {
            return "Open world Christmas changes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // christmas settings, including overriding the return door to get out of santa's house
            bool didChristmasPalettes = false;
            bool didChristmasDrops = false;
            // christmas settings
            bool xmasDeco = settings.getBool(OpenWorldSettings.PROPERTYNAME_XMAS_DECO);
            bool xmasDrops = settings.getBool(OpenWorldSettings.PROPERTYNAME_XMAS_DROPS);

            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            if (goal == OpenWorldGoalProcessor.GOAL_GIFTMODE || goal == OpenWorldGoalProcessor.GOAL_REINDEER || xmasDeco)
            {
                if (goal.Contains("xmas"))
                {
                    RemoveItemEventCommand removeItemEventCommand = new RemoveItemEventCommand();
                    removeItemEventCommand.process(outRom, ref context.workingOffset);
                }

                Random r_cosmetic = context.randomCosmetic;

                OpenWorldWinterTheme openWorldWinterTheme = new OpenWorldWinterTheme();
                openWorldWinterTheme.process(origRom, outRom, r_cosmetic, ref context.workingOffset, context.namesOfThings);
                XmasMapPalettes xmasMapPalettes = new XmasMapPalettes();
                xmasMapPalettes.process(outRom, r_cosmetic);
                didChristmasPalettes = true;

                // writing the return door; special case map 08 here to put us to ^
                // $01/CC83 8D 08 CD    STA $CD08  [$7E:CD08]   A:0008 X:0000 Y:0000 P:envmxdIzc
                // $01/CC86 E2 20       SEP #$20                A:0008 X:0000 Y:0000 P:envmxdIzc
                outRom[0x1CC83] = 0x22;
                outRom[0x1CC84] = (byte)context.workingOffset;
                outRom[0x1CC85] = (byte)(context.workingOffset >> 8);
                outRom[0x1CC86] = (byte)((context.workingOffset >> 16) + 0xC0);
                outRom[0x1CC87] = 0xEA;

                // removed code A
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x08;
                outRom[context.workingOffset++] = 0xCD;

                // CMP #0008
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = 0x08;
                outRom[context.workingOffset++] = 0x00;
                // BNE over [x18]
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x18;
                // LDA #014C [3]
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x4C;
                outRom[context.workingOffset++] = 0x01;
                // STA $CD08 [3]
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x08;
                outRom[context.workingOffset++] = 0xCD;
                // LDA #0050 [3]
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x50;
                outRom[context.workingOffset++] = 0x00;
                // STA $CD06 [3]
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x06;
                outRom[context.workingOffset++] = 0xCD;
                // STA $CD02 [3]
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x02;
                outRom[context.workingOffset++] = 0xCD;
                // LDA #0048 [3]
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x48;
                outRom[context.workingOffset++] = 0x00;
                // STA $CD04 [3]
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x04;
                outRom[context.workingOffset++] = 0xCD;
                // STA $CD00 [3]
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0xCD;
                // over:
                // removed code B
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;

                outRom[context.workingOffset++] = 0x6B;
            }

            // randomly get gear and consumables from chests
            if (goal == OpenWorldGoalProcessor.GOAL_GIFTMODE || goal == OpenWorldGoalProcessor.GOAL_REINDEER || xmasDrops)
            {
                XmasDrops xmasDropsHack = new XmasDrops();
                xmasDropsHack.process(outRom, ref context.workingOffset);
                didChristmasDrops = true;
            }

            if(goal == OpenWorldGoalProcessor.GOAL_GIFTMODE || goal == OpenWorldGoalProcessor.GOAL_REINDEER)
            {
                // turtle island maps normally use event 0x6f, which we repurpose for christmas mode, so make sure the gift guy here is visible.
                int turtleMapNum2 = 449;
                int mapObjOffset = 0x80000 + outRom[0x87000 + turtleMapNum2 * 2] + (outRom[0x87000 + turtleMapNum2 * 2 + 1] << 8);
                mapObjOffset += 8; // skip header
                outRom[mapObjOffset + 8 * 0 + 1] = 0x0F;
            }
            // publish this so map palette rando knows not to screw with the palettes
            context.workingData.setBool(XMAS_PALETTES_INDICATOR, didChristmasPalettes);
            return didChristmasPalettes || didChristmasDrops;
        }
    }
}
