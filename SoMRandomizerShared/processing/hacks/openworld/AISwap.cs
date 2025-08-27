using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Hack that swaps AI of regular enemies.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AISwap : RandoProcessor
    {
        protected override string getName()
        {
            return "Enemy AI swap";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;
            if(settings.get(OpenWorldSettings.PROPERTYNAME_AI_RANDO) != "swap")
            {
                return false;
            }
            List<int> enemyIds = new List<int>();
            for(int i=0; i < 84; i++)
            {
                // shadow xx
                if(i != 32 && i != 46 && i != 63)
                {
                    enemyIds.Add(i);
                }
            }

            for(int i=0; i < 84; i++)
            {
                // shadow xx
                if (i != 32 && i != 46 && i != 63)
                {
                    // swap pointers
                    int sourceEnemy = enemyIds[r.Next() % enemyIds.Count];
                    enemyIds.Remove(sourceEnemy);
                    outRom[0x100000 + i * 16 + 9] = origRom[0x100000 + sourceEnemy * 16 + 9];
                    outRom[0x100000 + i * 16 + 10] = origRom[0x100000 + sourceEnemy * 16 + 10];
                    if(sourceEnemy == 0x08)
                    {
                        // green drop
                        outRom[0x1055ce] = (byte)i;
                    }
                    if (sourceEnemy == 0x26)
                    {
                        // emberman
                        outRom[0x10712e] = (byte)i;
                    }
                    if (sourceEnemy == 0x27)
                    {
                        // red drop
                        outRom[0x1071df] = (byte)i;
                    }
                    if (sourceEnemy == 0x38)
                    {
                        // blue drop
                        outRom[0x108275] = (byte)i;
                    }
                    if (sourceEnemy == 0x44)
                    {
                        // tsunami
                        outRom[0x108e94] = (byte)i;
                    }
                    Logging.log("Setting AI for " + context.namesOfThings.getOriginalName(NamesOfThings.INDEX_ENEMIES_START + i) + " to " + context.namesOfThings.getOriginalName(NamesOfThings.INDEX_ENEMIES_START + sourceEnemy), "spoiler");
                }
            }
            return true;
        }
    }
}
