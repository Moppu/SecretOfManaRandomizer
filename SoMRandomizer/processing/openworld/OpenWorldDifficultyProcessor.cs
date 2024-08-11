using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using System.Collections.Generic;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// Define difficulty numbers needed for EnemiesAtYourLevel and other hacks to scale enemies for open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldDifficultyProcessor : RandoProcessor
    {
        public static string ENEMY_LEVEL_TYPE = "enemyLevelType";
        public static string ENEMY_LEVEL_TYPE_PLAYER = "player";
        public static string ENEMY_LEVEL_TYPE_TIMED = "timed";
        public static string ENEMY_LEVEL_TYPE_BOSSES = "bosses";
        public static string ENEMY_LEVEL_TYPE_NOFUTURE = "future";
        public static string ENEMY_LEVEL_TYPE_NO_SCALING = "none"; // for vanilla setting

        public static string ENEMY_DIFFICULTY_LEVEL = "enemyDifficultyLevel";

        public static string ENEMY_DIFFICULTY_STAT_SCALE = "enemyDifficultyStatScale";

        public static string BOSS_DIFFICULTY_INCREMENT_VALUE = "enemyBossDifficultyIncValue";

        public static string ENEMY_DIFFICULTY_TIMER_INC_PERIOD = "enemyDifficultyTimerIncPeriod";

        private static Dictionary<string, double> playerDifficultyValues = new Dictionary<string, double>();
        private static Dictionary<string, Dictionary<string, double>> difficultyValues = new Dictionary<string, Dictionary<string, double>>();
        private static Dictionary<string, double> timedDifficultyValues = new Dictionary<string, double>();
        private static Dictionary<string, double> bossesDifficultyValues = new Dictionary<string, double>();
        static OpenWorldDifficultyProcessor()
        {
            // these are used as scalars to the loaded enemy stats
            playerDifficultyValues["easy"] = 0.6;
            playerDifficultyValues["sortaeasy"] = 0.7;
            playerDifficultyValues["normal"] = 0.8;
            playerDifficultyValues["kindahard"] = 1.0;
            playerDifficultyValues["hard"] = 1.2;
            playerDifficultyValues["impossible"] = 2.0;
            difficultyValues["player"] = playerDifficultyValues;

            // these increments are in 0.5 levels to give a little better granularity of leveling, particularly
            // for boss kills difficulty mode

            // seconds per level
            timedDifficultyValues["easy"] = 320;
            timedDifficultyValues["sortaeasy"] = 256;
            timedDifficultyValues["normal"] = 200;
            timedDifficultyValues["kindahard"] = 160;
            timedDifficultyValues["hard"] = 128;
            timedDifficultyValues["impossible"] = 64;
            difficultyValues["timed"] = timedDifficultyValues;

            // number of enemy levels per boss killed
            bossesDifficultyValues["easy"] = 0.5; 
            bossesDifficultyValues["sortaeasy"] = 1;
            bossesDifficultyValues["normal"] = 1.5; 
            bossesDifficultyValues["kindahard"] = 2;
            bossesDifficultyValues["hard"] = 3; 
            bossesDifficultyValues["impossible"] = 4;
            difficultyValues["bosses"] = bossesDifficultyValues;
        }

        protected override string getName()
        {
            return "Open world difficulty value processor";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // difficulty settings for other hacks
            string enemyLevelType = ENEMY_LEVEL_TYPE_NO_SCALING;
            double difficultyLevel = 0.0;
            double difficultyStatScale = 1.0;

            string[] growthTypes = new string[] { ENEMY_LEVEL_TYPE_PLAYER, ENEMY_LEVEL_TYPE_TIMED, ENEMY_LEVEL_TYPE_BOSSES, ENEMY_LEVEL_TYPE_NOFUTURE };
            string difficulty = settings.get(OpenWorldSettings.PROPERTYNAME_ENEMY_STAT_GROWTH_DIFFICULTY);

            string statGrowthUiString = settings.get(OpenWorldSettings.PROPERTYNAME_ENEMY_STAT_GROWTH);
            foreach (string growthType in growthTypes)
            {
                if (statGrowthUiString.Contains(growthType))
                {
                    enemyLevelType = growthType;
                    if (difficultyValues.ContainsKey(enemyLevelType))
                    {
                        Dictionary<string, double> difficulties = difficultyValues[enemyLevelType];
                        difficultyLevel = difficulties[difficulty];
                    }
                    else
                    {
                        difficultyLevel = 1.0;
                    }

                }
            }
            difficultyStatScale = playerDifficultyValues[difficulty];

            context.workingData.set(ENEMY_LEVEL_TYPE, enemyLevelType);
            context.workingData.setDouble(ENEMY_DIFFICULTY_LEVEL, difficultyLevel);
            context.workingData.setDouble(ENEMY_DIFFICULTY_STAT_SCALE, difficultyStatScale);
            context.workingData.setDouble(BOSS_DIFFICULTY_INCREMENT_VALUE, bossesDifficultyValues[difficulty]);
            context.workingData.setDouble(ENEMY_DIFFICULTY_TIMER_INC_PERIOD, timedDifficultyValues[difficulty]);
            return true;
        }
    }
}
