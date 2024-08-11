using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Hack that adjusts magic defense on player armors for procedurally generated modes.
    /// I don't remember why this needed to be a thing.
    /// Switching out the enemy stats for open world ones will probably make this disappear.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MagDefAdjustment : RandoProcessor
    {
        protected override string getName()
        {
            return "Decreased armor magic defense for non-vanilla modes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // change magdef on armor
            // 103ed0, 10 byte block, 62 of them, [3] is mgdef
            double amount = 0.125; // this is the same for ac, bossrush, chaos currently. other modes do not use this hack

            for(int i=0; i < 62; i++)
            {
                int newMgdef = (byte)(amount * origRom[0x103ed0 + 10 * i + 3]);
                if(newMgdef > 255)
                {
                    newMgdef = 255;
                }
                outRom[0x103ed0 + 10 * i + 3] = (byte)newMgdef;
            }
            return true;
        }
    }
}
