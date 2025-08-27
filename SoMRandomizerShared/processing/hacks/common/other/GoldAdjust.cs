using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that scales gold received based on a configured multiplier.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class GoldAdjust : RandoProcessor
    {
        protected override string getName()
        {
            return "Gold scale";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            double goldMultiplier = 1.0;
            if (randoMode == VanillaRandoSettings.MODE_KEY)
            {
                string goldm = settings.get(VanillaRandoSettings.PROPERTYNAME_GOLD_MULTIPLIER);
                switch (goldm)
                {
                    case "half":
                        goldMultiplier = 0.5;
                        break;
                    case "double":
                        goldMultiplier = 2.0;
                        break;
                    case "triple":
                        goldMultiplier = 3.0;
                        break;
                }
            }
            else if (randoMode == OpenWorldSettings.MODE_KEY)
            {
                goldMultiplier = settings.getDouble(OpenWorldSettings.PROPERTYNAME_OPEN_WORLD_GOLD_MULTIPLIER);
            }
            else
            {
                Logging.log("Unsupported mode for gold multiplier");
            }

            if (goldMultiplier == 1.0)
            {
                Logging.log("Skipping gold multiplier of 1.0");
                return false;
            }
            for (int i = 0; i <= 127; i++)
            {
                int goldIndex = 0x1B;
                // scale enemy gold and write it back
                int origVal = DataUtil.ushortFromBytes(outRom, 0x101C00 + i * 29 + goldIndex);
                int newVal = DataUtil.clampToEndpoints((int)(Math.Ceiling(origVal * goldMultiplier)), 0, 65535);
                DataUtil.ushortToBytes(outRom, 0x101C00 + i * 29 + goldIndex, (ushort)newVal);
            }
            return true;
        }
    }
}
