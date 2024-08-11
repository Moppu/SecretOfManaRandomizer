using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.bossrush
{
    /// <summary>
    /// Randomize the order of bosses for boss rush mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BossOrderRandomizer : RandoProcessor
    {
        public static List<int> allPossibleBosses = new List<int>();
        static BossOrderRandomizer()
        {
            foreach (byte bossId in VanillaBossMaps.BY_VANILLA_BOSS_ID.Keys)
            {
                allPossibleBosses.Add(bossId);
            }
        }

        public const string BOSS_INDEX_PREFIX = "bossNum";
        protected override string getName()
        {
            return "Randomize boss order for boss rush";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            int numMaps = allPossibleBosses.Count;
            List<int> allBosses = new List<int>(allPossibleBosses);

            Random r = context.randomFunctional;
            for (int i = 0; i < numMaps; i++)
            {
                int mapNum = i;
                int bossIndex = r.Next() % allBosses.Count;
                int bossNum = allBosses[bossIndex];
                allBosses.RemoveAt(bossIndex);
                context.workingData.setInt(BOSS_INDEX_PREFIX + i, bossNum);
            }
            // end on mana beast
            context.workingData.setInt(BOSS_INDEX_PREFIX + numMaps, 0x7F);
            return true;
        }
    }
}
