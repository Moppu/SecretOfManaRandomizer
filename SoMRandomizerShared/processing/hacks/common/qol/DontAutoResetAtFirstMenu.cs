using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Remove a vanilla timer that makes the game reset if you sit at the new game menu for too long.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DontAutoResetAtFirstMenu : RandoProcessor
    {
        protected override string getName()
        {
            return "Remove title-screen menu auto-reset";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            /*
                C0/28EC:   C00807   CPY #$0708
                C0/28EF:   D00A     BNE $28FB - change to branch always, to never hit the code that auto-resets
             */
            outRom[0x28EF] = 0x80;
            return true;
        }
    }
}
