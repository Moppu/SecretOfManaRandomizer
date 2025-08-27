using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Hack that allows candy and other consumables to be used on characters who are at 0 HP, like the royal jam already can in vanilla.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CandyHealouts : RandoProcessor
    {
        protected override string getName()
        {
            return "Use candy and chocolate on 0 HP characters";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_CANDY_HEALOUTS))
            {
                return false;
            }

            /*
                C0/5539:   B982E1  LDA $E182,Y - load target character hp
                C0/553C:   F00E    BEQ $554C - branch if zero to code disallowing candy to be used - remove
             */
            outRom[0x553C] = 0xEA;
            outRom[0x553D] = 0xEA;

            return true;
        }
    }
}
