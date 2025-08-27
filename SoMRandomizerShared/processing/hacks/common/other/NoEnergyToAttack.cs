using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that makes players always attack at 100% power.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NoEnergyToAttack : RandoProcessor
    {
        protected override string getName()
        {
            return "Attacking doesn't cost stamina";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_NO_WEAPON_STAMINA_COST))
            {
                return false;
            }
            // store zero into stamina remaining instead of the usual amount
            // STA $E1ED,X -> STZ $E1ED,X
            outRom[0x2C24A] = 0x9E;
            // STA $E1ED,X -> STZ $E1ED,X
            // note that FasterEnemies overwrites this later if it's enabled
            outRom[0xF944] = 0x9E;
            return true;
        }
    }
}
