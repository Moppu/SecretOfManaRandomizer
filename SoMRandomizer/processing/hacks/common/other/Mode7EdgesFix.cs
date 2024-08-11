using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that fixes a minor graphical glitch with flammie flight causing the corners of the screen to look incorrect.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class Mode7EdgesFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Mode 7 minor graphical fix";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_MODE7_EDGES_FIX))
            {
                return false;
            }

            // minor adjustments to mode7 calculations 
            // this is a really old hack and i don't remember exactly what it did
            // but it fixed some sort of graphical issue at the bottom corners of flammie flight

            /*
                $00/90A3 A9 0F       LDA #$0F                A:0000 X:00A0 Y:0000 P:envMXdiZc **
                $00/90A5 8D 05 42    STA $4205  [$00:4205]   A:000F X:00A0 Y:0000 P:envMXdizc
                ..
                $00/90B3 38          SEC                     A:00D0 X:00A0 Y:0000 P:eNvMXdizc
                $00/90B4 E9 28       SBC #$28                A:00D0 X:00A0 Y:0000 P:eNvMXdizC **
                $00/90B6 85 07       STA $07    [$00:0007]   A:00A8 X:00A0 Y:0000 P:eNvMXdizC
             */
            outRom[0x90A4] = 0x08;
            outRom[0x90B5] = 0x00;
            return true;
        }
    }
}
