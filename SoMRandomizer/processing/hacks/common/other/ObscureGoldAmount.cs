using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that hides how much gold you currently have, so you need to guess at what you can actually afford.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ObscureGoldAmount : RandoProcessor
    {
        protected override string getName()
        {
            return "Obscure own gold amount";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_OBSCURE_GOLD))
            {
                return false;
            }

            // status menu:
            // $C7/69A7 BD 6A CC    LDA $CC6A,x[$7E:CC6A]   A:0400 X:0000 Y:0000 P:envMxdIZc
            // LDA #00 instead
            outRom[0x769A7] = 0xA9;
            outRom[0x769A8] = 0x00;
            outRom[0x769A9] = 0xEA;

            // events:
            // $C0/19A2 BD 6A CC    LDA $CC6A,x[$7E:CC6A]   A:04CC X:0000 Y:0000 P:eNvMxdIzc
            // LDA #00 instead
            outRom[0x19A2] = 0xA9;
            outRom[0x19A3] = 0x00;
            outRom[0x19A4] = 0xEA;
            return true;
        }
    }
}
