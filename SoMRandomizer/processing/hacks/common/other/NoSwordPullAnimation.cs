using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that removes the rusty sword removal animation from the starting event, to save a little time.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NoSwordPullAnimation : RandoProcessor
    {
        protected override string getName()
        {
            return "Remove sword-pull animation";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            /*
                C1/EC78:  22 76C902   JSR $02C976
             */
            outRom[0x1EC78] = 0xEA;
            outRom[0x1EC79] = 0xEA;
            outRom[0x1EC7A] = 0xEA;
            outRom[0x1EC7B] = 0xEA;
            return true;
        }
    }
}
