using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Hack that allows you to hit spring beak through the beak.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BeakShieldDisable : RandoProcessor
    {
        protected override string getName()
        {
            return "Beak disable for Spring/Axe Beak";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_BEAK_DISABLE))
            {
                return false;
            }

            // this is some sort of hitbox data, i currently don't have docs for the format of it

            // 07 -> 00
            outRom[0x7F7F0] = 0;
            // 0B -> 00
            outRom[0x7F7F1] = 0;
            return true;
        }
    }
}
