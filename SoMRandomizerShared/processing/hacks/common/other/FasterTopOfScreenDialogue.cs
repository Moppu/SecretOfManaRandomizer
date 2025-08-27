using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that speeds up top-of-screen messages.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FasterTopOfScreenDialogue : RandoProcessor
    {
        protected override string getName()
        {
            return "Faster top of screen dialogue";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_FASTER_TOP_SCREEN_DIALOGUE))
            {
                return false;
            }
            /*
            $C0/5CF3 A9 60       LDA #$60                A:0000 X:61AE Y:005A P:envMxdIzc
            $C0/5CF5 99 00 FE    STA $FE00,y[$7E:FE5A]   A:0060 X:61AE Y:005A P:envMxdIzc
             */
            // faster top-screen dialogues (was 0x60)
            outRom[0x5CF4] = 0x30;
            // MOPPLE: there may be more spots? this doesn't seem to speed up everything
            return true;
        }
    }
}
