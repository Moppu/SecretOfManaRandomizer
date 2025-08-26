using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that reduces player HP to 1 and armor defense to 0.
    /// Evade remains active.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OneHitKo : RandoProcessor
    {
        protected override string getName()
        {
            return "One-hit KO";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_OHKO))
            {
                return false;
            }
            // player hp = 1, def, mdef=0
            for(int i=0; i < 99; i++)
            {
                // boy
                outRom[0x104210 + i * 8] = 1; // hp lsb
                outRom[0x104210 + i * 8 + 1] = 0; // hp msb
                outRom[0x104210 + i * 8 + 5] = 0; // con

                // girl
                outRom[0x104528 + i * 8] = 1; // hp lsb
                outRom[0x104528 + i * 8 + 1] = 0; // hp msb
                outRom[0x104528 + i * 8 + 5] = 0; // con

                // sprite
                outRom[0x104840 + i * 8] = 1; // hp lsb
                outRom[0x104840 + i * 8 + 1] = 0; // hp msb
                outRom[0x104840 + i * 8 + 5] = 0; // con
            }
            // armor def,mdef=0
            for (int i=0; i <= 0x3E; i++)
            {
                outRom[0x103ED0 + i * 10 + 1] = 0; // def
                outRom[0x103ED0 + i * 10 + 3] = 0; // mdef
            }
            return true;
        }
    }
}
