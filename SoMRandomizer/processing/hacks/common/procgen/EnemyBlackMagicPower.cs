using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// Hack to adjust enemy damage spell power for procgen modes.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EnemyBlackMagicPower : RandoProcessor
    {
        protected override string getName()
        {
            return "Fixed magic power for enemies in non-vanilla modes";
        }

        // int scales; instead of scaling this we're just going to fix it
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            byte value = 0x38; // currently same for ac/bossrush/chaos. other modes do not use this hack
            // [18] value in enemy stats table, for every enemy
            for(int i=0; i < 128; i++)
            {
                outRom[0x101C00 + 29 * i + 18] = value;
            }
            return true;
        }
    }
}
