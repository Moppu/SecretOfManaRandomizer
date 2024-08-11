using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Display a hash of the seed on the file select screen.
    /// This should always be the last hack applied for any mode it's used on, since it uses the same rng as the
    /// rest of the rando generation, and it's intended to be a representation of the state of rando generation
    /// at completion of the processing.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class TitleMenuSeedHashDisplay : RandoProcessor
    {
        protected override string getName()
        {
            return "Seed/hash display";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;
            long hash = 0;
            for (int i = 0; i < 8; i++)
            {
                hash <<= 4;
                hash |= (r.Next() % 16);
            }
            string hashString = hash.ToString("X").ToUpper();
            string seedShortened = seed;
            if (seedShortened.Length > 20)
            {
                seedShortened = seedShortened.Substring(0, 20) + "...";
            }
            List<byte> introMenuReplacement = VanillaEventUtil.getBytes("Secret of Mana Open World v" + RomGenerator.VERSION_NUMBER + "\n\n" + "Seed: " + seedShortened + "\n\n" + "Hash check: " + hashString + "\n\nEnjoy", -1);
            // 0x33F0 is the "WELCOME TO SECRET OF MANA" text
            // through 348B
            // MOPPLE: this sometimes leaves some trailing garbage on the right side, and i'm not sure why
            int introMenuSize = 0x348B - 0x33F0 + 1;
            int srcI = 0;
            for (int i = 0; i < introMenuSize; i++)
            {
                if (srcI < introMenuReplacement.Count)
                {
                    if (introMenuReplacement[srcI] == 0x28)
                    {
                        srcI += 2;
                    }
                    outRom[0x33F0 + i] = introMenuReplacement[srcI];
                }
                else
                {
                    outRom[0x33F0 + i] = 0;
                }
                srcI++;
            }

            Logging.log("Hash check value: " + hashString);
            return true;
        }
    }
}
