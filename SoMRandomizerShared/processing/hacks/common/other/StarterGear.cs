using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that gives you basic starter gear in every slot, instead of just a chest piece.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class StarterGear : RandoProcessor
    {
        protected override string getName()
        {
            return "Starter gear in each slot";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_STARTER_GEAR))
            {
                return false;
            }

            // starting gear - note that weapons are also in here, we aren't setting them (here) though
            outRom[0x57BD] = 0x01; // bandana
            outRom[0x57BE] = 0x16; // overalls
            outRom[0x57BF] = 0x3E; // wristband

            outRom[0x57C1] = 0x02; // hair ribbon
            outRom[0x57C2] = 0x17; // kungfu suit
            outRom[0x57C3] = 0x3E; // wristband

            outRom[0x57C5] = 0x03; // rabite cap
            outRom[0x57C6] = 0x18; // midge robe
            outRom[0x57C7] = 0x2C; // elbow pad - wristband fucks up for sprite and overlaps with trash can value (hence not being equippable)
            return true;
        }
    }
}
