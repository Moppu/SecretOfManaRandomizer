using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that scales experience received based on a configured multiplier.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ExperienceAdjust : RandoProcessor
    {
        protected override string getName()
        {
            return "Experience scale";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            double expMultiplier = 1.0;
            if(randoMode == VanillaRandoSettings.MODE_KEY)
            {
                string expm = settings.get(VanillaRandoSettings.PROPERTYNAME_EXP_MULTIPLIER);
                switch(expm)
                {
                    case "half":
                        expMultiplier = 0.5;
                        break;
                    case "double":
                        expMultiplier = 2.0;
                        break;
                    case "triple":
                        expMultiplier = 3.0;
                        break;
                }
            }
            else if(randoMode == OpenWorldSettings.MODE_KEY)
            {
                expMultiplier = settings.getDouble(OpenWorldSettings.PROPERTYNAME_OPEN_WORLD_EXP_MULTIPLIER);
            }
            else
            {
                Logging.log("Unsupported mode for exp multiplier");
            }

            if(expMultiplier == 1.0)
            {
                Logging.log("Skipping exp multiplier of 1.0");
                return false;
            }
            for (int i = 0; i <= 127; i++)
            {
                int expIndex = 16;
                // scale enemy exp and write it back
                int origVal = DataUtil.ushortFromBytes(outRom, 0x101C00 + i * 29 + expIndex);
                int newVal = DataUtil.clampToEndpoints((int)(Math.Ceiling(origVal * expMultiplier)), 0, 65535);
                DataUtil.ushortToBytes(outRom, 0x101C00 + i * 29 + expIndex, (ushort)newVal);
            }
            return true;
        }
    }
}
