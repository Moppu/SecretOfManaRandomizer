using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that removes the fade from map transitions to make them load a little faster.
    /// Sometimes introduces some minor graphical glitches.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FastTransitions : RandoProcessor
    {
        protected override string getName()
        {
            return "Faster door transitions";
        }

        // remove fade effect when walking between maps to make it a little faster
        // default off because it's less pretty
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_FAST_TRANSITIONS))
            {
                return false;
            }
            // $00/8B75 A5 E6       LDA $E6    [$00:00E6]   A:0001 X:0020 Y:00FE P:envMXdIzC
            // $00 / 8B77 10 02       BPL $02[$8B7B]      A: 0005 X: 0020 Y: 00FE P:envMXdIzC
            // $00 / 8B7B C5 E7 CMP $E7[$00:00E7]   A: 0005 X: 0020 Y: 00FE P:envMXdIzC
            // screen transition fades
            outRom[0x8B76] = 0xE7;
            return true;
        }
    }
}
