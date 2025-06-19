using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// Determines the number of mana seeds required for MTR mode, and sets a convenience property for it, which is used by other hacks.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldMtrSeedNumSelection : RandoProcessor
    {
        public static string MANA_SEEDS_REQUIRED = "manaSeedsRequired";

        protected override string getName()
        {
            return "MTR seed selection";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;
            // mana seeds required for MTR goal
            int manaSeedsRequired;
            try
            {
                manaSeedsRequired = Int32.Parse(settings.get(OpenWorldSettings.PROPERTYNAME_MANA_SEEDS_REQUIRED));
            }
            catch (Exception e)
            {
                // "random" if we can't parse as int
                int min = Int32.Parse(settings.get(OpenWorldSettings.PROPERTYNAME_MANA_SEEDS_REQUIRED_MIN));
                int max = Int32.Parse(settings.get(OpenWorldSettings.PROPERTYNAME_MANA_SEEDS_REQUIRED_MAX));
                if (min > max)
                {
                    min = max;
                }
                int diff = max - min;
                // Note that Next(min,max) treats max as exclusive.
                manaSeedsRequired = diff == 0 ? min : r.Next(min, max + 1);
                if (manaSeedsRequired < 1)
                {
                    manaSeedsRequired = 1;
                }
                if (manaSeedsRequired > 8)
                {
                    manaSeedsRequired = 8;
                }
            }
            
            Logging.log("Seeds required for mana tree = " + manaSeedsRequired, "spoiler");
            // set on context for other hacks to use
            context.workingData.setInt(MANA_SEEDS_REQUIRED, manaSeedsRequired);
            return true;
        }
    }
}
