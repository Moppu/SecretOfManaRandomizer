using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SoMRandomizer.config
{
    /// <summary>
    /// Difficulty settings constants for earlier modes like ancient cave
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DifficultySettings
    {
        public static string DIFFICULTY_PROPERTY_PREFIX = "difficulty_";

        public static string ENEMY_LEVEL = "enemyLevel";
        public static string ENEMY_STR = "enemyStr";
        public static string ENEMY_AGI = "enemyAgi";
        public static string ENEMY_INT = "enemyInt";
        public static string ENEMY_WIS = "enemyWis";
        public static string ENEMY_EVADE = "enemyEvade";
        public static string ENEMY_MEVADE = "enemyMevade";
        public static string ENEMY_HP = "enemyHp";
        public static string ENEMY_MP = "enemyMp";
        public static string TONPOLE_MP = "tonpoleMp";
        public static string ENEMY_DEF = "enemyDef";
        public static string ENEMY_MDEF = "enemyMdef";
        public static string ENEMY_EXP = "enemyExp";
        public static string ENEMY_GOLD = "enemyGold";
        public static string ENEMY_WEAPON_LEV = "enemyWeaponLev";
        public static string ENEMY_MAGIC_LEV = "enemyMagicLev";
        public static string ENEMY_WEAPON_DAMAGE = "enemyWeaponDamage";
        public static string MANA_POWER = "manaPower";
        public static string BOSS_STAT_MULTIPLIER = "bossStatMultiplier";
        public static string BOSS_EXP_MULTIPLIER = "bossExpMultiplier";
        public static string BOSS_GOLD_MULTIPLIER = "bossGoldMultiplier";
        public static string BOSS_HP_MULTIPLIER = "bossHpMultiplier";
        public static string STARTING_GOLD = "startingGold";
        public static string STARTING_CONSUMABLES = "startingConsumables";
        public static string RESTORE_COST = "restoreCost";
        public static string NUM_PHANNAS = "numPhannas";
        public static string NUM_ELEMENTS = "numElements";
        public static string NUM_WATTS = "numWatts";
        public static string CONTINUE_COST = "continueCost";
        public static string STARTING_LEVEL = "startingLevel";
        public static string RESTORES_LIMIT = "restoresLimit";

        public static Dictionary<string, string> displayStrings = new Dictionary<string, string>();
        static DifficultySettings()
        {
            displayStrings.Add("Enemy Level", ENEMY_LEVEL);
            displayStrings.Add("Enemy Str", ENEMY_STR);
            displayStrings.Add("Enemy Agi", ENEMY_AGI);
            displayStrings.Add("Enemy Int", ENEMY_INT);
            displayStrings.Add("Enemy Wis", ENEMY_WIS);
            displayStrings.Add("Enemy Evade", ENEMY_EVADE);
            displayStrings.Add("Enemy Mevade", ENEMY_MEVADE);
            displayStrings.Add("Enemy HP", ENEMY_HP);
            displayStrings.Add("Enemy MP", ENEMY_MP);
            displayStrings.Add("Enemy Def", ENEMY_DEF);
            displayStrings.Add("Enemy Mdef", ENEMY_MDEF);
            displayStrings.Add("Enemy Exp", ENEMY_EXP);
            displayStrings.Add("Enemy Gold", ENEMY_GOLD);
            displayStrings.Add("Enemy Weapon Level", ENEMY_WEAPON_LEV);
            displayStrings.Add("Enemy Magic Level", ENEMY_MAGIC_LEV);
            displayStrings.Add("Enemy Weapon Power", ENEMY_WEAPON_DAMAGE);
            displayStrings.Add("Mana Power", MANA_POWER);
            displayStrings.Add("Tonpole MP", TONPOLE_MP);
            displayStrings.Add("Boss Stat Multiplier", BOSS_STAT_MULTIPLIER);
            displayStrings.Add("Boss Exp Multiplier", BOSS_EXP_MULTIPLIER);
            displayStrings.Add("Boss Gold Multiplier", BOSS_GOLD_MULTIPLIER);
            displayStrings.Add("Boss HP Multiplier", BOSS_HP_MULTIPLIER);
            displayStrings.Add("Starting Gold", STARTING_GOLD);
            displayStrings.Add("Starting Consumables", STARTING_CONSUMABLES);
            displayStrings.Add("Restore Cost", RESTORE_COST);
            displayStrings.Add("# Phannas / Floor", NUM_PHANNAS);
            displayStrings.Add("# Elements / Floor", NUM_ELEMENTS);
            displayStrings.Add("# Watts / Floor", NUM_WATTS);
            displayStrings.Add("Continue Cost", CONTINUE_COST);
            displayStrings.Add("Starting Level", STARTING_LEVEL);
            displayStrings.Add("Max Restores", RESTORES_LIMIT);
        }
        private static string[] intGrowthValues = new string[] {
        ENEMY_LEVEL,
        ENEMY_STR,
        ENEMY_AGI,
        ENEMY_INT,
        ENEMY_WIS,
        ENEMY_EVADE,
        ENEMY_MEVADE,
        ENEMY_HP,
        ENEMY_MP,
        ENEMY_DEF,
        ENEMY_MDEF,
        ENEMY_EXP,
        ENEMY_GOLD,
        ENEMY_WEAPON_LEV,
        ENEMY_MAGIC_LEV,
        ENEMY_WEAPON_DAMAGE,
        MANA_POWER
        };

        private static string[] doubleValues = new string[]
        {
            TONPOLE_MP,
            BOSS_STAT_MULTIPLIER,
            BOSS_EXP_MULTIPLIER,
            BOSS_GOLD_MULTIPLIER,
            BOSS_HP_MULTIPLIER,
            STARTING_GOLD,
            STARTING_CONSUMABLES,
            RESTORE_COST,
            NUM_PHANNAS,
            NUM_ELEMENTS,
            NUM_WATTS,
            CONTINUE_COST,
            STARTING_LEVEL,
            RESTORES_LIMIT,
        };

        public static bool isGrowthValue(string displayString)
        {
            string key = displayStrings[displayString];
            return intGrowthValues.Contains(key);
        }

        public DifficultySettings()
        {

        }

        public DifficultySettings(DifficultySettings src)
        {
            foreach(string key in src.values.Keys)
            {
                values[key] = src.values[key];
            }
        }

        private Dictionary<string, object> values = new Dictionary<string, object>();

        public void readFromFile(string path, DifficultySettings defaultValues)
        {
            Dictionary<string, string> properties = new PropertyFileReader().readFile(path);
            if(properties.Count == 0)
            {
                throw new Exception("Failed to read file");
            }

            // these will throw no such key if anything is missing
            foreach (string key in intGrowthValues)
            {
                IntGrowthValue igv = new IntGrowthValue();
                try
                {
                    igv.baseValue = Double.Parse(properties[key + "_base"], CultureInfo.InvariantCulture.NumberFormat);
                    igv.growthValue = Double.Parse(properties[key + "_growth"], CultureInfo.InvariantCulture.NumberFormat);
                    igv.exponent = Double.Parse(properties[key + "_exp"], CultureInfo.InvariantCulture.NumberFormat);
                    values[key] = igv;
                }
                catch (Exception e)
                {
                    if (defaultValues != null)
                    {
                        values[key] = defaultValues.values[key];
                    }
                    else
                    {
                        igv = new IntGrowthValue();
                        igv.baseValue = 1.0;
                        igv.growthValue = 1.0;
                        igv.exponent = 1.0;
                        values[key] = igv;
                    }
                }
            }

            foreach (string key in doubleValues)
            {
                try
                {
                    values[key] = Double.Parse(properties[key], CultureInfo.InvariantCulture.NumberFormat);
                }
                catch(Exception e)
                {
                    if (defaultValues != null)
                    {
                        values[key] = defaultValues.values[key];
                    }
                    else
                    {
                        values[key] = 1.0;
                    }
                }
            }
        }
        public void readFromResourceFile(string path, DifficultySettings defaultValues)
        {
            Dictionary<string, string> properties = new PropertyFileReader().readPropertyResourceFile(path);
            foreach (string key in intGrowthValues)
            {
                IntGrowthValue igv = new IntGrowthValue();
                try
                {
                    igv.baseValue = Double.Parse(properties[key + "_base"], CultureInfo.InvariantCulture.NumberFormat);
                    igv.growthValue = Double.Parse(properties[key + "_growth"], CultureInfo.InvariantCulture.NumberFormat);
                    igv.exponent = Double.Parse(properties[key + "_exp"], CultureInfo.InvariantCulture.NumberFormat);
                    values[key] = igv;
                }
                catch(Exception e)
                {
                    if (defaultValues != null)
                    {
                        values[key] = defaultValues.values[key];
                    }
                    else
                    {
                        igv = new IntGrowthValue();
                        igv.baseValue = 1.0;
                        igv.growthValue = 1.0;
                        igv.exponent = 1.0;
                        values[key] = igv;
                    }
                }
            }

            foreach (string key in doubleValues)
            {
                try
                {
                    values[key] = Double.Parse(properties[key], CultureInfo.InvariantCulture.NumberFormat);
                }
                catch(Exception e)
                {
                    if (defaultValues != null)
                    {
                        values[key] = defaultValues.values[key];
                    }
                    else
                    {
                        values[key] = 1.0;
                    }
                }
            }
        }

        public IntGrowthValue getGrowthValue(string displayString)
        {
            string key;
            if (displayStrings.ContainsKey(displayString))
            {
                key = displayStrings[displayString];
            }
            else
            {
                key = displayString;
            }
            return (IntGrowthValue)values[key];
        }

        public void setGrowthValue(string displayString, IntGrowthValue newValue)
        {
            string key;
            if (displayStrings.ContainsKey(displayString))
            {
                key = displayStrings[displayString];
            }
            else
            {
                key = displayString;
            }
            values[key] = newValue;
        }

        public double getDoubleValue(string displayString)
        {
            string key;
            if (displayStrings.ContainsKey(displayString))
            {
                key = displayStrings[displayString];
            }
            else
            {
                key = displayString;
            }
            return (double)values[key];
        }

        public void setDoubleValue(string displayString, double newValue)
        {
            string key;
            if (displayStrings.ContainsKey(displayString))
            {
                key = displayStrings[displayString];
            }
            else
            {
                key = displayString;
            }
            values[key] = newValue;
        }
    }

    public class IntGrowthValue
    {
        public double baseValue;
        public double growthValue;
        public double exponent;
    }

}
