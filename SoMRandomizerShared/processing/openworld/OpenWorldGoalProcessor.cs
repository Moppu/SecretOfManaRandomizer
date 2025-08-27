using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// Sets a couple convenience properties for open world goal, that are used by other hacks.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldGoalProcessor : RandoProcessor
    {
        public static string MANA_FORT_ACCESSIBLE_INDICATOR = "fastManaFort";
        public const string GOAL_SHORT_NAME = "openWorldGoalShortName";
        public const string GOAL_MANABEAST = "manabeast";
        public const string GOAL_MTR = "manatree";
        public const string GOAL_GIFTMODE = "xmas2020";
        public const string GOAL_REINDEER = "xmas2020b";

        protected override string getName()
        {
            return "Open world goal processing";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool fastManaFort = true;
            string goal = "";
            string goalType = settings.get(OpenWorldSettings.PROPERTYNAME_GOAL);
            if (goalType.Contains("short"))
            {
                fastManaFort = true;
                goal = GOAL_MANABEAST;
            }
            else if (goalType.Contains("long"))
            {
                fastManaFort = false;
                goal = GOAL_MANABEAST;
            }
            else if (goalType.Contains("mtr"))
            {
                // actually doesn't matter; we disable manafort
                fastManaFort = true;
                goal = GOAL_MTR;
            }
            else if (goalType.Contains("gift"))
            {
                fastManaFort = false;
                goal = GOAL_GIFTMODE;
            }
            else if (goalType.Contains("reindeer"))
            {
                fastManaFort = false;
                goal = GOAL_REINDEER;
            }

            // publish some convenience values for other hacks
            context.workingData.setBool(MANA_FORT_ACCESSIBLE_INDICATOR, fastManaFort);
            context.workingData.set(GOAL_SHORT_NAME, goal);
            return true;
        }
    }
}
