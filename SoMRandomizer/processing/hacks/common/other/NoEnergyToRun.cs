using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that makes running not cost stamina.
    /// </summary>
    public class NoEnergyToRun : RandoProcessor
    {
        protected override string getName()
        {
            return "Remove energy cost for running";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_NO_ENERGY_TO_RUN))
            {
                return false;
            }
            // this is from the old secret of mana editor
            // MOPPLE: this removes the spending of stamina, but not the requirement to have 100 first
            // STA $E1ED,X -> remove
            outRom[0xB78A] = 0xEA;
            outRom[0xB78B] = 0xEA;
            outRom[0xB78C] = 0xEA;
            return true;
        }
    }
}
