using System.Collections.Generic;

namespace SoMRandomizer.config.settings
{
    /// <summary>
    /// Enumeration of settings and defaults for open world mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldSettings : RandoSettings
    {
        public const string MODE_KEY = "open";
        public const string MODE_NAME = "Open";

        public const string PROPERTYNAME_PLANDO = "plandoSettings";
        public const string PROPERTYNAME_RANDOMIZE_ENEMIES = "opEnemies";
        public const string PROPERTYNAME_RANDOMIZE_BOSSES = "opBosses";
        public const string PROPERTYNAME_RANDOMIZE_WEAPONS = "opWeapons";
        public const string PROPERTYNAME_RANDOMIZE_MUSIC = "opMusic";
        public const string PROPERTYNAME_RANDOMIZE_SHOPS = "opShop";
        public const string PROPERTYNAME_RANDOMIZE_COLORS = "opMapColors";
        public const string PROPERTYNAME_AUTOSAVE = "opAutosave";
        public const string PROPERTYNAME_STATUS_AILMENTS = "opStatusAilments";
        public const string PROPERTYNAME_ENEMY_STAT_GROWTH = "opStatGrowth";
        public const string PROPERTYNAME_ENEMY_STAT_GROWTH_DIFFICULTY = "opDifficulty";
        public const string PROPERTYNAME_START_WITH_GIRL_AND_SPRITE = "opCharacters";
        public const string PROPERTYNAME_STARTING_CHAR = "opStartChar";
        public const string PROPERTYNAME_OPEN_WORLD_EXP_MULTIPLIER = "opExp";
        public const string PROPERTYNAME_OPEN_WORLD_GOLD_MULTIPLIER = "opGold";
        public const string PROPERTYNAME_MANA_BEAST_SCALING = "opManaBeastScale";
        public const string PROPERTYNAME_COMPLEXITY = "opComplexity";
        public const string PROPERTYNAME_SHOW_ENEMY_LEVEL = "opShowLevels";
        public const string PROPERTYNAME_REFACTOR_MAGIC_ROPE = "opMagicRopeRefactor";
        public const string PROPERTYNAME_LOGIC_MODE = "opLogic";
        public const string PROPERTYNAME_GOAL = "opGoal";
        public const string PROPERTYNAME_EVERY_ENEMY = "oopsAllThis";
        public const string PROPERTYNAME_FORCE_START_WEAPON = "opStartWeapon";
        public const string PROPERTYNAME_MANA_SEEDS_REQUIRED = "opNumSeeds";
        public const string PROPERTYNAME_MANA_SEEDS_REQUIRED_MIN = "opMinSeeds";
        public const string PROPERTYNAME_MANA_SEEDS_REQUIRED_MAX = "opMaxSeeds";
        public const string PROPERTYNAME_NO_WALKING_CHESTS = "opChestsDontRun";
        public const string PROPERTYNAME_CHEST_TRAPS = "opTraps";
        public const string PROPERTYNAME_CHEST_FREQUENCY = "opChestFreq";
        public const string PROPERTYNAME_AI_RANDO = "opAiRando";
        public const string PROPERTYNAME_PAUSE_TIMER_IN_MENU = "opPauseInMenu";
        public const string PROPERTYNAME_XMAS_DECO = "opXmasMaps";
        public const string PROPERTYNAME_XMAS_DROPS = "opXmasItems";
        public const string PROPERTYNAME_RANDOMIZE_GRANDPALACE_ELEMENTS = "opGPEleRando";
        public const string PROPERTYNAME_OBSCURE_MAP_DATA = "opPlatinumMode";
        public const string PROPERTYNAME_MIN_ENEMY_LEVELS = "opMinLevel";
        public const string PROPERTYNAME_MAX_ENEMY_LEVELS = "opMaxLevel";
        public const string PROPERTYNAME_ALLOW_MISSED_ITEMS = "opAllowLockedItems";
        public const string PROPERTYNAME_NO_HINTS = "opDisableHints";
        public const string PROPERTYNAME_NO_FUTURE_LEVEL = "opNoFutureLevel";
        public const string PROPERTYNAME_BOY_CLASS = "opBoyRole";
        public const string PROPERTYNAME_GIRL_CLASS = "opGirlRole";
        public const string PROPERTYNAME_SPRITE_CLASS = "opSpriteRole";
        public const string PROPERTYNAME_START_WITH_ALL_WEAPON_ORBS = "opAllWeaponOrbs";
        public const string PROPERTYNAME_FAST_WHIP_POSTS = "opFastWhipPosts";
        public const string PROPERTYNAME_NUMERIC_GOLD_CHECK_MULTIPLIER = "opGoldCheckMul";
        public const string PROPERTYNAME_NUMERIC_GOLD_DROP_MULTIPLIER = "opGoldDropMul";
        public const string PROPERTYNAME_NUMERIC_START_GOLD = "opStartGold";
        public const string PROPERTYNAME_NUM_XMAS_GIFTS = "opXmasGifts";
        public const string PROPERTYNAME_STARTING_LEVEL = "opStartLevel";
        public const string PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC = "opFlammieDrumInLogic";
        
        public const string PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_EASY = "easy";
        public const string PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_SORTA_EASY = "sortaeasy";
        public const string PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_NORMAL = "normal";
        public const string PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_KINDA_HARD = "kindahard";
        public const string PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_HARD = "hard";
        public const string PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_IMPOSSIBLE = "impossible";

        public OpenWorldSettings(CommonSettings commonSettings) : base(commonSettings)
        {
            // default open world settings

            // boolean settings
            setInitial(PROPERTYNAME_RANDOMIZE_WEAPONS, true); // weapon rando vs vanilla weapons
            setInitial(PROPERTYNAME_RANDOMIZE_MUSIC, true); // import & randomize music
            setInitial(PROPERTYNAME_RANDOMIZE_SHOPS, true); // shop rando
            setInitial(PROPERTYNAME_RANDOMIZE_COLORS, false); // map color rando
            setInitial(PROPERTYNAME_AUTOSAVE, true); // autosave in 4th slot
            setInitial(PROPERTYNAME_NO_WALKING_CHESTS, true); // chests don't run away
            setInitial(PROPERTYNAME_SHOW_ENEMY_LEVEL, true); // show enemy level as part of its name
            setInitial(PROPERTYNAME_REFACTOR_MAGIC_ROPE, true); // magic rope works in more places
            setInitial(PROPERTYNAME_PAUSE_TIMER_IN_MENU, true); // timed mode pauses when in menu or on flammie
            setInitial(PROPERTYNAME_XMAS_DECO, false); // xmas decorations - snowy palettes etc
            setInitial(PROPERTYNAME_XMAS_DROPS, false); // xmas drops - randomly get gear and stuff
            setInitial(PROPERTYNAME_RANDOMIZE_GRANDPALACE_ELEMENTS, true); // randomize orbs in grand palace
            setInitial(PROPERTYNAME_OBSCURE_MAP_DATA, false); // all maps are blacked out
            setInitial(PROPERTYNAME_MIN_ENEMY_LEVELS, false); // apply minimum enemy level based on area
            setInitial(PROPERTYNAME_MAX_ENEMY_LEVELS, false); // apply maximum enemy level based on area
            setInitial(PROPERTYNAME_ALLOW_MISSED_ITEMS, false); // logic can create spots you can never reach but you can still meet the goal
            setInitial(PROPERTYNAME_NO_HINTS, false); // only trash hints
            setInitial(PROPERTYNAME_START_WITH_ALL_WEAPON_ORBS, false); // start with all weapon orbs
            setInitial(PROPERTYNAME_FAST_WHIP_POSTS, true); // step on whip spot with whip to jump automatically
            setInitial(PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC, false); // have to find flammie drum

            // enumerations
            setInitial(PROPERTYNAME_RANDOMIZE_ENEMIES, new string[] { "vanilla", "swap", "random", "oops", "none" }, new string[] { "Vanilla", "Swap", "Random spawns", "Oops! All owls", "None" }, "swap");
            setInitial(PROPERTYNAME_RANDOMIZE_BOSSES, new string[] { "vanilla", "swap", "random" }, new string[] { "Vanilla", "Swap", "Random" }, "random");
            setInitial(PROPERTYNAME_STATUS_AILMENTS, new string[] { "location", "type", "easy", "annoying", "awful" }, new string[] { "Location", "Enemy type", "Random (easy)", "Random (annoying)", "Random (awful)" }, "location");
            setInitial(PROPERTYNAME_ENEMY_STAT_GROWTH, new string[] { "player", "bosses", "timed", "nofuture", "vanilla" }, new string[] { "Match player", "Increase after bosses", "Timed", "No Future", "Vanilla" }, "player");
            setInitial(PROPERTYNAME_ENEMY_STAT_GROWTH_DIFFICULTY, new string[]
            {
                PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_EASY,
                PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_SORTA_EASY,
                PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_NORMAL, 
                PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_KINDA_HARD,
                PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_HARD,
                PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_IMPOSSIBLE
            }, new string[] { "Easy", "Sorta easy", "Normal", "Kinda hard", "Hard", "Impossible" }, PROPERTYVALUE_STAT_GROWTH_DIFFICULTY_NORMAL);
            setInitial(PROPERTYNAME_START_WITH_GIRL_AND_SPRITE, new string[] { "startboth", "findbothL1", "findbothCL", "start1find1", "start1only", "find1L1", "find1CL", "none" }, 
                new string[] { "Start with both", "Find both at level 1", "Find both at current level", "Start with one, find the other", "Start with one, other doesn't exist", "Find one at level 1", "Find one at current level", "They don't exist" }, "findbothL1");
            setInitial(PROPERTYNAME_STARTING_CHAR, new string[] { "random", "boy", "girl", "sprite" }, new string[] { "Random", "Boy", "Girl", "Sprite" }, "boy");
            setInitial(PROPERTYNAME_MANA_BEAST_SCALING, new string[] { "vanilla", "scaled" }, new string[] { "Vanilla", "Scaled" }, "scaled");
            setInitial(PROPERTYNAME_COMPLEXITY, new string[] { "dontcare", "easy", "hard" }, new string[] { "Don't care", "Easy", "Hard" }, "dontcare");
            setInitial(PROPERTYNAME_LOGIC_MODE, new string[] { "basic", "restrictive" }, new string[] { "Basic", "Restrictive" }, "basic");
            setInitial(PROPERTYNAME_GOAL, new string[] { "vshort", "vlong", "mtr", "gift", "reindeer" }, new string[] { "Vanilla short", "Vanilla long", "Mana tree revival", "Gift exchange", "Reindeer hunt" }, "vshort");
            setInitial(PROPERTYNAME_CHEST_TRAPS, new string[] { "normal", "none", "many" }, new string[] { "Normal", "None", "Many" }, "normal");
            setInitial(PROPERTYNAME_CHEST_FREQUENCY, new string[] { "none", "low", "normal", "high", "highest" }, new string[] { "None", "Low", "Normal", "High", "Highest" }, "normal");
            setInitial(PROPERTYNAME_AI_RANDO, new string[] { "vanilla", "swap" }, new string[] { "Vanilla", "Swap" }, "vanilla");

            // numeric
            setInitial(PROPERTYNAME_OPEN_WORLD_EXP_MULTIPLIER, 2.0, 0, 1000, 1); // exp multiplier
            setInitial(PROPERTYNAME_OPEN_WORLD_GOLD_MULTIPLIER, 2.0, 0, 1000, 1); // gold multiplier (for enemy kills)
            setInitial(PROPERTYNAME_NUMERIC_GOLD_CHECK_MULTIPLIER, 2.0, 0, 50, 0); // multiplier for values of gold checks
            setInitial(PROPERTYNAME_NUMERIC_GOLD_DROP_MULTIPLIER, 2.0, 0, 255, 0); // multiplier for values of gold chest drops
            setInitial(PROPERTYNAME_NUMERIC_START_GOLD, 0.0, 0, 65535, 0); // starting gold
            setInitial(PROPERTYNAME_NUM_XMAS_GIFTS, 8.0, 4, 8, 0); // number of gifts for christmas mode
            setInitial(PROPERTYNAME_STARTING_LEVEL, 1.0, 1, 99, 0); // starting level of first character

            // /////////////
            // oops all owls
            // /////////////
            List<string> enemyTypeOptions = new List<string>();
            enemyTypeOptions.Add("Random");
            enemyTypeOptions.Add("0 - Rabite");
            enemyTypeOptions.Add("1 - Buzz Bee");
            enemyTypeOptions.Add("2 - Mushboom");
            enemyTypeOptions.Add("3 - Chobin Hood");
            enemyTypeOptions.Add("4 - Lullabud");
            enemyTypeOptions.Add("5 - Iffish");
            enemyTypeOptions.Add("6 - Kid Goblin");
            enemyTypeOptions.Add("7 - Eye Spy");
            enemyTypeOptions.Add("8 - Green Drop");
            enemyTypeOptions.Add("9 - Specter");
            enemyTypeOptions.Add("10 - Blat");
            enemyTypeOptions.Add("11 - Goblin");
            enemyTypeOptions.Add("12 - Water Thug");
            enemyTypeOptions.Add("13 - Polter Chair");
            enemyTypeOptions.Add("14 - Ma Goblin");
            enemyTypeOptions.Add("15 - Dark Funk");
            enemyTypeOptions.Add("16 - Crawler");
            enemyTypeOptions.Add("17 - Ice Thug");
            enemyTypeOptions.Add("18 - Zombie");
            enemyTypeOptions.Add("19 - Kimono Bird");
            enemyTypeOptions.Add("20 - Silktail");
            enemyTypeOptions.Add("21 - Nemesis Owl");
            enemyTypeOptions.Add("22 - Pebbler");
            enemyTypeOptions.Add("23 - Pumpkin Bomb");
            enemyTypeOptions.Add("24 - Steamed Crab");
            enemyTypeOptions.Add("25 - Chess Knight");
            enemyTypeOptions.Add("26 - Wizard Eye");
            enemyTypeOptions.Add("27 - Howler");
            enemyTypeOptions.Add("28 - Robin Foot");
            enemyTypeOptions.Add("29 - LA Funk");
            enemyTypeOptions.Add("30 - Grave Bat");
            enemyTypeOptions.Add("31 - Werewolf");
            enemyTypeOptions.Add("32 - Shadow X3");
            enemyTypeOptions.Add("33 - Evil Sword");
            enemyTypeOptions.Add("34 - Tomato Man");
            enemyTypeOptions.Add("35 - Mystic Book");
            enemyTypeOptions.Add("36 - Sand Stinger");
            enemyTypeOptions.Add("37 - Mad Mallard");
            enemyTypeOptions.Add("38 - Emberman");
            enemyTypeOptions.Add("39 - Red Drop");
            enemyTypeOptions.Add("40 - Eggatrice");
            enemyTypeOptions.Add("41 - Bomb Bee");
            enemyTypeOptions.Add("42 - Mushgloom");
            enemyTypeOptions.Add("43 - Trap Flower");
            enemyTypeOptions.Add("44 - Dinofish");
            enemyTypeOptions.Add("45 - Mimic Box");
            enemyTypeOptions.Add("46 - Shadow X1");
            enemyTypeOptions.Add("47 - Kimono Wizard");
            enemyTypeOptions.Add("48 - Ghost");
            enemyTypeOptions.Add("49 - Metal Crawler");
            enemyTypeOptions.Add("50 - Spider Legs");
            enemyTypeOptions.Add("51 - Weepy Eye");
            enemyTypeOptions.Add("52 - Shellblast");
            enemyTypeOptions.Add("53 - Beast Zombie");
            enemyTypeOptions.Add("54 - Ghoul");
            enemyTypeOptions.Add("55 - Imp");
            enemyTypeOptions.Add("56 - Blue Drop");
            enemyTypeOptions.Add("57 - Marmablue");
            enemyTypeOptions.Add("58 - Fierce Head");
            enemyTypeOptions.Add("59 - Griffin Hand");
            enemyTypeOptions.Add("60 - Needlion");
            enemyTypeOptions.Add("61 - Metal Crab");
            enemyTypeOptions.Add("62 - Armored Man");
            enemyTypeOptions.Add("63 - Shadow X2");
            enemyTypeOptions.Add("64 - Eggplant Man");
            enemyTypeOptions.Add("65 - Captain Duck");
            enemyTypeOptions.Add("66 - Nitro Pumpkin");
            enemyTypeOptions.Add("67 - Turtlance");
            enemyTypeOptions.Add("68 - Tsunami");
            enemyTypeOptions.Add("69 - Basilisk");
            enemyTypeOptions.Add("70 - Gremlin");
            enemyTypeOptions.Add("71 - Steelpion");
            enemyTypeOptions.Add("72 - Dark Ninja");
            enemyTypeOptions.Add("73 - Whimper");
            enemyTypeOptions.Add("74 - Heck Hound");
            enemyTypeOptions.Add("75 - Fiend Head");
            enemyTypeOptions.Add("76 - National Scar");
            enemyTypeOptions.Add("77 - Dark Stalker");
            enemyTypeOptions.Add("78 - Dark Knight");
            enemyTypeOptions.Add("79 - Shape Shifter");
            enemyTypeOptions.Add("80 - Wolf Lord");
            enemyTypeOptions.Add("81 - Doom Sword");
            enemyTypeOptions.Add("82 - Terminator");
            enemyTypeOptions.Add("83 - Master Ninja");
            string[] oopsAllFlagsValues = new string[85];
            oopsAllFlagsValues[0] = "random";
            for (int i = 0; i <= 83; i++)
            {
                oopsAllFlagsValues[i + 1] = "" + i;
            }
            setInitial(PROPERTYNAME_EVERY_ENEMY, oopsAllFlagsValues, enemyTypeOptions.ToArray(), "21");

            // ///////////////
            // starting weapon
            // ///////////////
            List<string> forceStartWeaponOptions = new List<string>();
            forceStartWeaponOptions.Add("Random");
            forceStartWeaponOptions.Add("0 - Glove");
            forceStartWeaponOptions.Add("1 - Sword");
            forceStartWeaponOptions.Add("2 - Axe");
            forceStartWeaponOptions.Add("3 - Spear");
            forceStartWeaponOptions.Add("4 - Whip");
            forceStartWeaponOptions.Add("5 - Bow");
            forceStartWeaponOptions.Add("6 - Boomerang");
            forceStartWeaponOptions.Add("7 - Javelin");
            string[] startWeaponFlagsValues = new string[9];
            startWeaponFlagsValues[0] = "-1";
            for (int i = 0; i < 8; i++)
            {
                startWeaponFlagsValues[i + 1] = "" + i;
            }
            setInitial(PROPERTYNAME_FORCE_START_WEAPON, startWeaponFlagsValues, forceStartWeaponOptions.ToArray(), "-1");

            // //////////////
            // mtr mana seeds
            // //////////////
            List<string> numberSeedsRequiredOptions = new List<string>();
            numberSeedsRequiredOptions.Add("Random");
            numberSeedsRequiredOptions.Add("1");
            numberSeedsRequiredOptions.Add("2");
            numberSeedsRequiredOptions.Add("3");
            numberSeedsRequiredOptions.Add("4");
            numberSeedsRequiredOptions.Add("5");
            numberSeedsRequiredOptions.Add("6");
            numberSeedsRequiredOptions.Add("7");
            numberSeedsRequiredOptions.Add("8");
            string[] numSeedsFlagsValues = new string[9];
            numSeedsFlagsValues[0] = "random";
            for (int i = 0; i < 8; i++)
            {
                numSeedsFlagsValues[i + 1] = "" + (i + 1);
            }
            setInitial(PROPERTYNAME_MANA_SEEDS_REQUIRED, numSeedsFlagsValues, numberSeedsRequiredOptions.ToArray(), "8");

            List<string> numberSeedsMinMaxOptions = new List<string>();
            numberSeedsMinMaxOptions.Add("1");
            numberSeedsMinMaxOptions.Add("2");
            numberSeedsMinMaxOptions.Add("3");
            numberSeedsMinMaxOptions.Add("4");
            numberSeedsMinMaxOptions.Add("5");
            numberSeedsMinMaxOptions.Add("6");
            numberSeedsMinMaxOptions.Add("7");
            numberSeedsMinMaxOptions.Add("8");
            string[] numSeedsMinMaxFlagsValues = new string[8];
            for (int i = 0; i < 8; i++)
            {
                numSeedsMinMaxFlagsValues[i] = "" + (i + 1);
            }
            setInitial(PROPERTYNAME_MANA_SEEDS_REQUIRED_MIN, numSeedsMinMaxFlagsValues, numberSeedsMinMaxOptions.ToArray(), "1");
            setInitial(PROPERTYNAME_MANA_SEEDS_REQUIRED_MAX, numSeedsMinMaxFlagsValues, numberSeedsMinMaxOptions.ToArray(), "8");

            // //////////////////////////
            // no future mode enemy level
            // //////////////////////////
            List<string> noFutureLevels = new List<string>();
            string[] noFutureLevelsFlags = new string[99];
            for (int i = 1; i <= 99; i++)
            {
                noFutureLevels.Add("" + i);
                noFutureLevelsFlags[i - 1] = "" + i;
            }
            setInitial(PROPERTYNAME_NO_FUTURE_LEVEL, noFutureLevelsFlags, noFutureLevels.ToArray(), "99");

            // ///////////////
            // character roles
            // ///////////////
            List<string> characterOptions = new List<string>();
            characterOptions.Add("Random");
            characterOptions.Add("Random (unique)");
            characterOptions.Add("Boy");
            characterOptions.Add("Girl");
            characterOptions.Add("Sprite");
            List<string> characterOptionValues = new List<string>();
            characterOptionValues.Add("random");
            characterOptionValues.Add("randomunique");
            characterOptionValues.Add("OGboy");
            characterOptionValues.Add("OGgirl");
            characterOptionValues.Add("OGsprite");
            setInitial(PROPERTYNAME_BOY_CLASS, characterOptionValues.ToArray(), characterOptions.ToArray(), "OGboy");
            setInitial(PROPERTYNAME_GIRL_CLASS, characterOptionValues.ToArray(), characterOptions.ToArray(), "OGgirl");
            setInitial(PROPERTYNAME_SPRITE_CLASS, characterOptionValues.ToArray(), characterOptions.ToArray(), "OGsprite");

            setInitial(PROPERTYNAME_PLANDO, "");
        }
    }
}
