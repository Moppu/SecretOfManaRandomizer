using SoMRandomizer.config.settings;
using SoMRandomizer.processing.bossrush;
using SoMRandomizer.processing.common;
using System.Linq;

namespace SoMRandomizer.processing.hacks.bossrush
{
    /// <summary>
    /// Hack that adjusts weapon power for the mana beast for boss rush mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ManaBeastDamageScaling : RandoProcessor
    {
        protected override string getName()
        {
            return "Reduction of mana beast power for boss rush";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            int numMaps = BossOrderRandomizer.allPossibleBosses.Count;
            double ratio = numMaps / 64.0;

            // mana beast weapon damages
            // this isn't handled by EnemyChanges changing weapon damage because boss weapons are stored separately
            outRom[0x10C0CD] = (byte)(0x59 * ratio); // flyby damage
            outRom[0x10C0DB] = (byte)(0x5C * ratio); // zoom-in damage
            outRom[0x10C0E2] = (byte)(0x78 * ratio); // fire attack damage

            return true;
        }
    }
}
