using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.fix
{
    /// <summary>
    /// Hack that fixes a couple issues with whip post targetting.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class WhipFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Whip coordinate fix";
        }
        
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_WHIP_COORDINATE_CORRUPTION_FIX))
            {
                return false;
            }

            // limit a loop writing palettes that overwrites some other data
            outRom[0x586A] = 0x1D;
            // 14D8C7 - length of whip? downward only.  normally 3C, change to 4C
            outRom[0x14D8C7] = 0x4C;

            return true;
        }

    }
}
