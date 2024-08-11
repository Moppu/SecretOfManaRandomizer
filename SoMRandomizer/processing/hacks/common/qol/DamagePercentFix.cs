using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Hack that removes vanilla math to divide outgoing damage by 2 if it wasn't a fully-charged (100% stamina) attack.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DamagePercentFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Accurate damage percentages";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_DAMAGE_PERCENT_FIX))
            {
                return false;
            }

            // this is from the old secret of mana editor
            // x5383, put EA EA
            /*
                C0/5383:   4689   LSR $89 - divide damage by two
             */
            outRom[0x5383] = 0xEA;
            outRom[0x5384] = 0xEA;
            return true;
        }
    }
}
