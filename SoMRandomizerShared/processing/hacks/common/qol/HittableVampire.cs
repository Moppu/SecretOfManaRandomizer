using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Hack that allows the vampire to be hit with weapons when cloaked/flying.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class HittableVampire : RandoProcessor
    {
        protected override string getName()
        {
            return "Vampire hittable when cloaked";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_HITTABLE_VAMPIRE))
            {
                return false;
            }
            // the vampire closed-cape frame w/o hitbox can be found at 0xACAFC, as referenced at 0x17358 and other spots
            // i don't remember how this data works, but changing the last line of it to 01 01 01 01 00 00 00 00 gave him a hitbox
            // in this frame:
            // CAFC - CB46:
            // 10 00 
            // F0 C0 06 1A 00 C0 06 5A 
            // E0 D0 2A 1A 10 D0 2A 5A 
            // F0 D0 2C 1A 00 D0 2C 5A 
            // E0 E0 40 1A 10 E0 40 5A 
            // F0 E0 42 1A 00 E0 42 5A 
            // E0 F0 44 1A 10 F0 44 5A 
            // F0 F0 46 1A 00 F0 46 5A 
            // F0 00 2E 1A 00 00 2E 5A 
            // 00 00 00 00 11 17 00 DD
            // ^^^ change to:
            // 01 01 01 01 00 00 00 00
            // for hittable vampires. buffy shares the same data.
            outRom[0xACB3E] = 0x01;
            outRom[0xACB3F] = 0x01;
            outRom[0xACB40] = 0x01;
            outRom[0xACB41] = 0x01;
            outRom[0xACB42] = 0x00;
            outRom[0xACB43] = 0x00;
            outRom[0xACB44] = 0x00;
            outRom[0xACB45] = 0x00;
            return true;
        }
    }
}
