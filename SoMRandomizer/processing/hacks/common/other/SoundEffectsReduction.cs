using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that makes a small change to vanilla SPC700 sound processing code to only allow sound effects on channels 6, 7, and 8.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SoundEffectsReduction : RandoProcessor
    {
        protected override string getName()
        {
            return "Sound effects channel reduction";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if (!settings.getBool(CommonSettings.PROPERTYNAME_FOOTSTEP_SOUND))
            {
                return false;
            }
            
            // note this is a patch to SPC700 code, which is an entirely different instruction set from normal SNES code
            // change 
            // 0E7D D0 ED     BNE $0E6C
            // to
            // 0E7D 2F 01     BRA $0E80
            // i don't remember exactly why this works, but it limits the number of channels that SPC code will choose
            // to allocate sound effects to.

            // this is part of an audio block in the ROM that gets loaded into SPC memory on game startup.
            outRom[0x313C7] = 0x2F;
            outRom[0x313C8] = 0x01;

            return true;
        }
    }
}
