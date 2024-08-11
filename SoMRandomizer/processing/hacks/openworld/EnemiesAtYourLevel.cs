using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.processing.hacks.common.util;
using SoMRandomizer.processing.openworld;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Scale enemy levels by the selected method for open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EnemiesAtYourLevel : RandoProcessor
    {
        bool debugLogging = false;

        protected override string getName()
        {
            return "Enemy stats scaling/adjustment";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string difficulty = settings.get(OpenWorldSettings.PROPERTYNAME_ENEMY_STAT_GROWTH_DIFFICULTY);
            double expMultiplier = settings.getDouble(OpenWorldSettings.PROPERTYNAME_OPEN_WORLD_EXP_MULTIPLIER);
            double goldMultiplier = settings.getDouble(OpenWorldSettings.PROPERTYNAME_OPEN_WORLD_GOLD_MULTIPLIER);
            bool randomizeBossElements = settings.getBool(CommonSettings.PROPERTYNAME_BOSS_ELEMENT_RANDO);
            bool vanillaManaBeast = settings.get(OpenWorldSettings.PROPERTYNAME_MANA_BEAST_SCALING) == "vanilla";
            bool pauseTimerInMenu = settings.getBool(OpenWorldSettings.PROPERTYNAME_PAUSE_TIMER_IN_MENU);
            byte noFutureLevel = (byte)settings.getInt(OpenWorldSettings.PROPERTYNAME_NO_FUTURE_LEVEL);

            // see OpenWorldDifficultyProcessor, which determines these values based on selected difficulty
            double difficultyStatScale = context.workingData.getDouble(OpenWorldDifficultyProcessor.ENEMY_DIFFICULTY_STAT_SCALE);
            string enemyLevelSource = context.workingData.get(OpenWorldDifficultyProcessor.ENEMY_LEVEL_TYPE);
            // multiply by 2 since code below adds boss levels in increments of 0.5
            double bossIncrementValue = context.workingData.getDouble(OpenWorldDifficultyProcessor.BOSS_DIFFICULTY_INCREMENT_VALUE) * 2.0;
            // divide by 2 since code below adds 0.5 enemy levels every N seconds
            double timerIncrementValue = context.workingData.getDouble(OpenWorldDifficultyProcessor.ENEMY_DIFFICULTY_TIMER_INC_PERIOD) / 2.0;
            int screenTextSubrLocation = context.workingData.getInt(CustomTopOfScreenText.OFFSET_HIROM);
            int minLevelTableLoc = context.workingData.getInt(MinMaxLevel.MINMAXLEVEL_MIN_OFFSET_HIROM);
            int maxLevelTableLoc = context.workingData.getInt(MinMaxLevel.MINMAXLEVEL_MAX_OFFSET_HIROM);

            bool startSolo = context.workingData.getBool(OpenWorldCharacterSelection.START_SOLO);

            context.initialValues[CustomRamOffsets.OPENWORLD_CURRENT_ENEMY_LEVEL] = 0;
            context.initialValues[CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT] = 0;
            context.initialValues[CustomRamOffsets.LEVELMATCH_BOSSES_KILLED] = 1;

            if (enemyLevelSource == OpenWorldDifficultyProcessor.ENEMY_LEVEL_TYPE_NO_SCALING)
            {
                Logging.log("No enemy scaling; applying only exp/gold scale");
                new ExperienceAdjust().add(origRom, outRom, seed, settings, context);
                new GoldAdjust().add(origRom, outRom, seed, settings, context);
                return true;
            }

            // generate low/avg/high stat tables for each stat
            // vanilla enemy stats at 101c00; 29 bytes apiece
            // [0] level
            // [1,2] hp
            // [3] mp
            // [4] str
            // [5] agi
            // [6] int
            // [7] wis
            // [8] eva
            // [9,10] def
            // [11] mev
            // [12,13] mdef
            // [16,17] exp
            // [18,19] white/black magic power - we'll just use the same for both -> 1FD, 1FE in RAM
            // [26] weapon/magic level (nibbles)
            // [27,28] gold
            List<int> sourceOffsets = new List<int>();
            sourceOffsets.Add(0); // level
            sourceOffsets.Add(1); // hp
            sourceOffsets.Add(3); // mp
            sourceOffsets.Add(4); // str
            sourceOffsets.Add(99); // con
            sourceOffsets.Add(5); // agi
            sourceOffsets.Add(6); // int
            sourceOffsets.Add(7); // wis
            sourceOffsets.Add(8); // evade
            sourceOffsets.Add(9); // def
            sourceOffsets.Add(11); // mevade
            sourceOffsets.Add(12); // mdef
            sourceOffsets.Add(16); // exp
            sourceOffsets.Add(18); // wh/bl power
            sourceOffsets.Add(26); // wlev .. 1C0
            sourceOffsets.Add(260); // mlev .. 1C4
            sourceOffsets.Add(27); // money
            sourceOffsets.Add(23); // weapon1 damage
            sourceOffsets.Add(24); // weapon2 damage

            // which stats are 16-bit
            List<int> sixteenBitOffsets = new List<int>();
            sixteenBitOffsets.Add(1); // hp
            sixteenBitOffsets.Add(9); // def
            sixteenBitOffsets.Add(12); // mdef
            sixteenBitOffsets.Add(16); // exp
            sixteenBitOffsets.Add(27); // money

            // stats loaded by the loader that runs during fights sometimes when statuses change etc
            List<int> altLoaderStats = new List<int>();
            altLoaderStats.Add(4); // str
            altLoaderStats.Add(99); // con
            altLoaderStats.Add(5); // agi
            altLoaderStats.Add(6); // int
            altLoaderStats.Add(7); // wis
            altLoaderStats.Add(8); // eva
            altLoaderStats.Add(9); // def
            altLoaderStats.Add(11); // meva
            altLoaderStats.Add(12); // mdef
            altLoaderStats.Add(26); // wlev
            altLoaderStats.Add(260); // mlev

            // scaling for "high" versions of stats
            Dictionary<int, double> highMultipliers = new Dictionary<int, double>();
            highMultipliers[1] = 1.6; // hp
            highMultipliers[4] = 1.6; // str
            highMultipliers[9] = 3.5; // def (was 3.0)
            highMultipliers[18] = 1.2; // mgpwr
            highMultipliers[23] = 1.2; // weapons
            highMultipliers[24] = 1.2; // weapons
            highMultipliers[26] = 1.3; // weapon level
            highMultipliers[260] = 3.0; // magic level

            // scaling for "low" versions of stats
            Dictionary<int, double> lowMultipliers = new Dictionary<int, double>();
            lowMultipliers[1] = 0.9; // hp
            lowMultipliers[4] = 0.8; // str
            lowMultipliers[23] = 0.8; // weapons
            lowMultipliers[24] = 0.8; // weapons
            lowMultipliers[26] = 0.8; // weapon level
            lowMultipliers[18] = 0.8; // mgpwr

            // threshold to determine what's far enough from average to be considered high or low, per stat
            Dictionary<int, double> diffThresholds = new Dictionary<int, double>();
            diffThresholds[26] = 0.05; // wlev
            diffThresholds[260] = 0.05; // mlev
            // force everything to be avg exp/gold
            diffThresholds[16] = 10000; // exp
            diffThresholds[27] = 10000; // gold
            diffThresholds[8] = 0.5; // eva
            diffThresholds[9] = 0.5; // def
            diffThresholds[23] = 0.3; // weapon damage

            // override starting values for stats (at level 1)
            Dictionary<int, double> zeroValues = new Dictionary<int, double>();
            zeroValues[1] = 10; // hp
            zeroValues[99] = 1; // con
            zeroValues[4] = 3; // str
            zeroValues[5] = 1; // agi
            zeroValues[18] = 10; // mgpwr
            zeroValues[23] = 7; // rabite weapon power

            // override growth exponents for stats
            Dictionary<int, double> exponents = new Dictionary<int, double>();
            exponents[1] = 1.1; // hp

            // default ways to scale stats
            double defaultHighMultiplier = 2.0;
            double defaultLowMultiplier = 0.5;
            double defaultZeroValue = 0.0;
            double defaultExponent = 1.0;

            // values levels 1-99 for low,avg,high,low,avg,high,...
            Dictionary<int, int> valueTableLocations = new Dictionary<int, int>();

            // stat type id -> offset to find list of values with 0 for low, 1 for avg, 2 for high, one for each enemy (128 total, with a few gaps)
            Dictionary<int, int> classLocations = new Dictionary<int, int>();

            // E198 is weapon damage (including str); it's handled separately
            Dictionary<int, List<ushort>> ramLocations = new Dictionary<int, List<ushort>>();
            ramLocations[0] = new ushort[] { 0xE181 }.ToList(); // level
            ramLocations[1] = new ushort[] { 0xE182, 0xE184 }.ToList(); // current, max hp
            ramLocations[3] = new ushort[] { 0xE186, 0xE187 }.ToList(); // current, max mp
            ramLocations[4] = new ushort[] { 0xE188 }.ToList(); // str
            ramLocations[5] = new ushort[] { 0xE189 }.ToList(); // agi
            ramLocations[99] = new ushort[] { 0xE18A }.ToList(); // con - keep this always 1 like in vanilla
            ramLocations[6] = new ushort[] { 0xE18B }.ToList(); // int - is this actually 8B?
            ramLocations[7] = new ushort[] { 0xE18C }.ToList(); // wis - is this actually 8C?
            ramLocations[8] = new ushort[] { 0xE1A4 }.ToList(); // eva
            ramLocations[9] = new ushort[] { 0xE1A5 }.ToList(); // def
            ramLocations[11] = new ushort[] { 0xE1A7 }.ToList(); // meva
            ramLocations[12] = new ushort[] { 0xE1A8 }.ToList(); // mdef
            ramLocations[16] = new ushort[] { 0xE18D }.ToList(); // exp
            ramLocations[18] = new ushort[] { 0xE1FD, 0xE1FE }.ToList(); // bl/wh mgpwr
            ramLocations[26] = new ushort[] { 0xE1C0 }.ToList(); // wlev
            ramLocations[260] = new ushort[] { 0xE1C4 }.ToList(); // mlev
            ramLocations[27] = new ushort[] { 0xE1C8 }.ToList(); // money

            // normal enemies, then bosses; separate stat sets
            List<List<int>> enemySets = new List<List<int>>();
            List<int> normalEnemies = new List<int>();
            for (int i = 0; i < 84; i++)
            {
                normalEnemies.Add(i);
            }
            List<int> bosses = new List<int>();
            for (int i = 87; i < 128; i++)
            {
                bosses.Add(i);
            }
            // sprite doppleganger
            normalEnemies.Remove(32);
            bosses.Add(32);
            // boy doppleganger
            normalEnemies.Remove(46);
            bosses.Add(46);
            // girl doppleganger
            normalEnemies.Remove(63);
            bosses.Add(63);
            enemySets.Add(normalEnemies);
            enemySets.Add(bosses);

            // for logging
            Dictionary<int, string> statNames = new Dictionary<int, string>();
            statNames[0] = "Level";
            statNames[1] = "HP";
            statNames[3] = "MP";
            statNames[4] = "Str";
            statNames[99] = "Con";
            statNames[5] = "Agi";
            statNames[6] = "Int";
            statNames[7] = "Wis";
            statNames[8] = "Ev";
            statNames[9] = "Def";
            statNames[11] = "Mev";
            statNames[12] = "Mdef";
            statNames[16] = "Exp";
            statNames[18] = "MgPwr";
            statNames[26] = "WL";
            statNames[260] = "ML";
            statNames[27] = "Gold";
            statNames[23] = "W1 Dmg";
            statNames[24] = "W2 Dmg";

            // additional level for enemies for each difficulty
            Dictionary<string, int> difficultyAdditions = new Dictionary<string, int>();
            difficultyAdditions["easy"] = -2;
            difficultyAdditions["sorta easy"] = -1;
            difficultyAdditions["normal"] = -1;
            difficultyAdditions["kinda hard"] = -1;
            difficultyAdditions["hard"] = -1;
            difficultyAdditions["impossible"] = 5;

            // bosses are higher level than normal enemies
            Dictionary<string, int> bossAdditions = new Dictionary<string, int>();
            bossAdditions["easy"] = 3;
            bossAdditions["sorta easy"] = 3;
            bossAdditions["normal"] = 3;
            bossAdditions["kinda hard"] = 3;
            bossAdditions["hard"] = 3;
            bossAdditions["impossible"] = 5;

            // bosses can't be too low a level
            Dictionary<string, int> minimumBossLevels = new Dictionary<string, int>();
            minimumBossLevels["easy"] = 3;
            minimumBossLevels["sorta easy"] = 3;
            minimumBossLevels["normal"] = 4;
            minimumBossLevels["kinda hard"] = 4;
            minimumBossLevels["hard"] = 5;
            minimumBossLevels["impossible"] = 10;

            // stats won't go above the stats for this maximum level
            Dictionary<string, int> enemyLevelMaxes = new Dictionary<string, int>();
            enemyLevelMaxes["easy"] = 50;
            enemyLevelMaxes["sorta easy"] = 60;
            enemyLevelMaxes["normal"] = 70;
            enemyLevelMaxes["kinda hard"] = 80;
            enemyLevelMaxes["hard"] = 99;
            enemyLevelMaxes["impossible"] = 99;

            // evade for enemies won't go above this
            Dictionary<string, int> enemyEvadeMaxes = new Dictionary<string, int>();
            enemyEvadeMaxes["easy"] = 25;
            enemyEvadeMaxes["sorta easy"] = 40;
            enemyEvadeMaxes["normal"] = 50;
            enemyEvadeMaxes["kinda hard"] = 75;
            enemyEvadeMaxes["hard"] = 99;
            enemyEvadeMaxes["impossible"] = 99;

            // scale enemy magic defense
            Dictionary<string, double> extraMdefLevelMultipliers = new Dictionary<string, double>();
            extraMdefLevelMultipliers["easy"] = 0.0;
            extraMdefLevelMultipliers["sorta easy"] = 1.0;
            extraMdefLevelMultipliers["normal"] = 1.5;
            extraMdefLevelMultipliers["kinda hard"] = 2.0;
            extraMdefLevelMultipliers["hard"] = 2.5;
            extraMdefLevelMultipliers["impossible"] = 4.0;

            // enemy weapon/magic levels max at this amount
            Dictionary<string, int> enemyWeaponMagicLevelMaxes = new Dictionary<string, int>();
            enemyWeaponMagicLevelMaxes["easy"] = 4;
            enemyWeaponMagicLevelMaxes["sorta easy"] = 5;
            enemyWeaponMagicLevelMaxes["normal"] = 6;
            enemyWeaponMagicLevelMaxes["kinda hard"] = 7;
            enemyWeaponMagicLevelMaxes["hard"] = 8;
            enemyWeaponMagicLevelMaxes["impossible"] = 8;

            // for logging; stat num -> level -> class, value
            Dictionary<int, Dictionary<int, Dictionary<int, int>>> finalStatValues = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();
            // for logging; stat num -> enemy id -> class
            Dictionary<int, Dictionary<int, int>> finalStatClasses = new Dictionary<int, Dictionary<int, int>>();
            // for logging; stat num -> enemy id -> value
            Dictionary<int, Dictionary<int, int>> vanillaStatValues = new Dictionary<int, Dictionary<int, int>>();
            // for each stat - determine low/avg/high per enemy, and the values for those tables (separate for bosses)
            foreach (int offset in sourceOffsets)
            {
                finalStatValues[offset] = new Dictionary<int, Dictionary<int, int>>();
                finalStatClasses[offset] = new Dictionary<int, int>();
                vanillaStatValues[offset] = new Dictionary<int, int>();
                if (offset == 0)
                {
                    for (int i = 0; i < 128; i++)
                    {
                        vanillaStatValues[offset][i] = origRom[0x101C00 + i * 29 + 0];
                    }
                }

                int srcOffset = offset;
                // fix magic level offset; this and weapon level are on the same byte but in different nibbles
                if (srcOffset == 260)
                {
                    srcOffset = 26;
                }
                // normal enemies
                int _sum = 0;
                int _levelSum = 0;
                // determine average stat - one per enemy set
                List<double> avgStatValuePerLevel = new List<double>();
                // list of normal enemies, list of bosses
                foreach (List<int> enemyIds in enemySets)
                {
                    List<double> statValuePerLevel = new List<double>();
                    foreach (int i in enemyIds)
                    {
                        _levelSum += origRom[0x101C00 + i * 29 + 0];
                        int val = 0;
                        if(offset == 99)
                        {
                            // con
                            val = 1;
                        }
                        else if (offset == 23 || offset == 24)
                        {
                            // weapons
                            byte weaponId = origRom[0x101C00 + i * 29 + srcOffset];
                            // lookup in weapon table at 101000, 12 bytes each, [8] is weapon damage
                            byte weaponDamage = origRom[0x101000 + weaponId * 12 + 8];
                            _sum += weaponDamage;
                            val = weaponDamage;
                        }
                        else if (offset == 26 || offset == 260)
                        {
                            if (offset == 26)
                            {
                                // weapon level - lower bits
                                val = (origRom[0x101C00 + i * 29 + srcOffset] & 0x0F);
                            }
                            else
                            {
                                // magic level - upper bits
                                val = (origRom[0x101C00 + i * 29 + srcOffset] & 0xF0) >> 4;
                            }
                            _sum += val;
                        }
                        else
                        {
                            if (sixteenBitOffsets.Contains(offset))
                            {
                                val = DataUtil.ushortFromBytes(origRom, 0x101C00 + i * 29 + srcOffset);
                            }
                            else
                            {
                                val = origRom[0x101C00 + i * 29 + srcOffset];
                            }
                            _sum += val;
                        }

                        int lev = origRom[0x101C00 + i * 29 + 0];
                        if (lev == 0)
                        {
                            // shadowx are level 0 for some reason
                            lev = 50;
                        }
                        statValuePerLevel.Add(val / (double)lev);
                    }

                    double avg = 0;
                    foreach (double stat in statValuePerLevel)
                    {
                        avg += stat / (double)statValuePerLevel.Count;
                    }
                    avgStatValuePerLevel.Add(avg);
                }

                double highMultiplier = defaultHighMultiplier;
                if (highMultipliers.ContainsKey(offset))
                {
                    highMultiplier = highMultipliers[offset];
                }
                double lowMultiplier = defaultLowMultiplier;
                if (lowMultipliers.ContainsKey(offset))
                {
                    lowMultiplier = lowMultipliers[offset];
                }

                double difficultyStatMultiplier = 1.0;
                difficultyStatMultiplier = difficultyStatScale;
                int difficultyAddition = difficultyAdditions[difficulty];
                int bossAddition = bossAdditions[difficulty];

                if (offset != 0) // don't make these for level
                {
                    // don't allow a table to span banks, it doesn't load properly
                    int size = sixteenBitOffsets.Contains(offset) ? 6 : 3;
                    CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 98 * size);
                    valueTableLocations[offset] = context.workingOffset;
                    double zeroValue = defaultZeroValue;
                    if (zeroValues.ContainsKey(offset))
                    {
                        zeroValue = zeroValues[offset];
                    }
                    double exponent = defaultExponent;
                    if (exponents.ContainsKey(offset))
                    {
                        exponent = exponents[offset];
                    }

                    // enemy level
                    for (int i = 1; i <= 99; i++)
                    {
                        finalStatValues[offset][i] = new Dictionary<int, int>();
                        // enemy type
                        for (int e = 0; e < avgStatValuePerLevel.Count; e++)
                        {
                            // determine vanilla level we base this actual level's stat off of
                            int lev = i + difficultyAddition;
                            if (e == 1)
                            {
                                // bosses higher level
                                lev += bossAddition;

                                // don't allow super weak bosses
                                int minBossLevel = minimumBossLevels[difficulty];
                                lev = Math.Max(lev, minBossLevel);
                            }

                            // one extra level punishment for starting with 2-3 characters
                            if (!startSolo)
                            {
                                lev++;
                            }

                            int levelMax = enemyLevelMaxes[difficulty];
                            lev = Math.Max(lev, 1);
                            lev = Math.Min(lev, levelMax);
                            int valueHigh = (int)(zeroValue + (avgStatValuePerLevel[e] * Math.Pow(lev, exponent) * difficultyStatMultiplier * highMultiplier));
                            int valueAvg = (int)(zeroValue + (avgStatValuePerLevel[e] * Math.Pow(lev, exponent) * difficultyStatMultiplier));
                            int valueLow = (int)(zeroValue + (avgStatValuePerLevel[e] * Math.Pow(lev, exponent) * difficultyStatMultiplier * lowMultiplier));
                            if (offset == 16)
                            {
                                // exp - override with a formula.  this was based on vanilla values from https://mycurvefit.com/ w/ cubic regression
                                if (lev > 10)
                                {
                                    valueAvg = (int)(7.806507 - 2.172297 * lev + 0.2366238 * lev * lev + 0.01093192 * lev * lev * lev);
                                }
                                else
                                {
                                    // early levels don't make a lot of mathematical sense
                                    int[] lowExpValues = new int[] { 0, 1, 2, 3, 4, 7, 10, 12, 21, 30, 35 };
                                    valueAvg = lowExpValues[lev];
                                }
                                if (valueAvg < 1)
                                {
                                    valueAvg = 1;
                                }
                                if (e == 1)
                                {
                                    // bosses
                                    valueAvg *= 10;
                                }
                                valueAvg = (int)(valueAvg * expMultiplier);
                            }
                            if (offset == 27)
                            {
                                // money - override with a formula.  this was based on vanilla values from https://mycurvefit.com/ w/ cubic regression
                                if (lev > 10)
                                {
                                    valueAvg = (int)(-40.17712 + 0.2992243 * lev + 0.6395421 * lev * lev);
                                }
                                else
                                {
                                    // early levels don't make a lot of mathematical sense, so hardcode them
                                    int[] lowGoldValues = new int[] { 0, 2, 5, 8, 11, 14, 17, 21, 25, 31, 36, };
                                    valueAvg = lowGoldValues[lev];
                                }
                                if (valueAvg < 1)
                                {
                                    valueAvg = 1;
                                }
                                if (e == 1)
                                {
                                    // bosses
                                    valueAvg *= 4;
                                }
                                valueAvg = (int)(valueAvg * goldMultiplier);
                            }
                            if ((offset == 23 || offset == 24) && e == 1)
                            {
                                // boss weapons
                                // mantis ant is level 3 and has weapon power 12ish
                                // mana beast is level 73 and has weapon power 127ish
                                // this calculation is largely experimental
                                valueAvg = (int)(5 + lev * 1.4 * difficultyStatMultiplier);
                                valueAvg = Math.Max(valueAvg, 8); // don't allow it to go too low
                            }
                            if (offset == 8 || offset == 11)
                            {
                                // evade, m.evade
                                valueLow = 0;
                                valueAvg = 0;
                                valueHigh = enemyEvadeMaxes[difficulty];
                            }

                            // con - lock to 1
                            if(offset == 99)
                            {
                                valueLow = 1;
                                valueAvg = 1;
                                valueHigh = 1;
                            }

                            // mdef
                            if (offset == 12)
                            {
                                valueLow += (int)(lev * extraMdefLevelMultipliers[difficulty]);
                                valueAvg += (int)(lev * extraMdefLevelMultipliers[difficulty]);
                                valueHigh += (int)(lev * extraMdefLevelMultipliers[difficulty]);
                            }

                            if (sixteenBitOffsets.Contains(offset))
                            {
                                // 16 bit stats
                                valueHigh = Math.Min(valueHigh, 65535);
                                valueAvg = Math.Min(valueAvg, 65535);
                                valueLow = Math.Min(valueLow, 65535);
                                if (debugLogging)
                                {
                                    Logging.log("Level " + i + " stat " + offset + " low=" + valueLow + " avg=" + valueAvg + " high=" + valueHigh + " at offset " + context.workingOffset.ToString("X"), "debug");
                                }
                                outRom[context.workingOffset++] = (byte)valueLow;
                                outRom[context.workingOffset++] = (byte)(valueLow >> 8);
                                outRom[context.workingOffset++] = (byte)valueAvg;
                                outRom[context.workingOffset++] = (byte)(valueAvg >> 8);
                                outRom[context.workingOffset++] = (byte)valueHigh;
                                outRom[context.workingOffset++] = (byte)(valueHigh >> 8);
                                // 6 total values for [stat id][level] .. low/avg/high for normal enemies, low/avg/high for bosses
                                finalStatValues[offset][i][e * 3 + 0] = valueLow;
                                finalStatValues[offset][i][e * 3 + 1] = valueAvg;
                                finalStatValues[offset][i][e * 3 + 2] = valueHigh;
                            }
                            else
                            {
                                // 8 bit stats
                                int max = 255;
                                if (offset == 26 || offset == 260)
                                {
                                    max = enemyWeaponMagicLevelMaxes[difficulty]; // weapon/magic lev
                                }

                                if (offset == 3)
                                {
                                    max = 40; // mp
                                }

                                valueHigh = Math.Min(valueHigh, max);
                                valueAvg = Math.Min(valueAvg, max);
                                valueLow = Math.Min(valueLow, max);

                                if (offset == 3 && e == 1 && !randomizeBossElements)
                                {
                                    // tonpole mp
                                    valueLow = 6;
                                }

                                if (debugLogging)
                                {
                                    Logging.log("Level " + i + " stat " + offset + " low=" + valueLow + " avg=" + valueAvg + " high=" + valueHigh + " at offset " + context.workingOffset.ToString("X"), "debug");
                                }
                                outRom[context.workingOffset++] = (byte)valueLow;
                                outRom[context.workingOffset++] = (byte)valueAvg;
                                outRom[context.workingOffset++] = (byte)valueHigh;
                                // 6 total values for [stat id][level] .. low/avg/high for normal enemies, low/avg/high for bosses
                                finalStatValues[offset][i][e * 3 + 0] = valueLow;
                                finalStatValues[offset][i][e * 3 + 1] = valueAvg;
                                finalStatValues[offset][i][e * 3 + 2] = valueHigh;
                            }
                        }
                    }

                    // don't allow a table to span banks, it doesn't load properly
                    CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 128);

                    classLocations[offset] = context.workingOffset;

                    // now, for all enemies, determine if they're low/avg/high for this stat, and write that to an enemy "class" table
                    for (int i = 0; i < 128; i++)
                    {
                        // normal enemy, or boss
                        int typeIndex = 0;
                        for (int e = 0; e < enemySets.Count; e++)
                        {
                            if (enemySets[e].Contains(i))
                            {
                                typeIndex = e;
                            }
                        }
                        byte level = origRom[0x101C00 + i * 29 + 0];
                        int vanillaValue = 0;
                        if (sixteenBitOffsets.Contains(offset))
                        {
                            vanillaValue = DataUtil.ushortFromBytes(origRom, 0x101C00 + i * 29 + srcOffset);
                        }
                        else
                        {
                            if(offset == 99)
                            {
                                // con
                                vanillaValue = 1;
                            }
                            else if (offset == 23 || offset == 24)
                            {
                                // weapons
                                byte weaponId = origRom[0x101C00 + i * 29 + srcOffset];
                                // lookup in weapon table at 101000, 12 bytes each, [8] is weapon damage
                                byte weaponDamage = origRom[0x101000 + weaponId * 12 + 8];
                                vanillaValue = weaponDamage;
                                if (typeIndex == 1)
                                {
                                    vanillaValue = (int)(5 + level * 1.4 * difficultyStatMultiplier);
                                    if (vanillaValue < 8)
                                    {
                                        vanillaValue = 8;
                                    }
                                }
                            }
                            else if (offset == 26)
                            {
                                // wlev
                                vanillaValue = (origRom[0x101C00 + i * 29 + srcOffset] & 0x0F);
                            }
                            else if (offset == 260)
                            {
                                // mlev
                                vanillaValue = (origRom[0x101C00 + i * 29 + srcOffset] & 0xF0) >> 4;
                            }
                            else
                            {
                                vanillaValue = origRom[0x101C00 + i * 29 + srcOffset];
                            }
                        }

                        vanillaStatValues[offset][i] = vanillaValue;

                        if (offset == 9 && i == 9)
                        {
                            i = 9;
                        }
                        byte enemyClass = 0;
                        double diffThreshold = avgStatValuePerLevel[typeIndex] / 10.0;
                        if (diffThresholds.ContainsKey(offset))
                        {
                            diffThreshold = diffThresholds[offset];
                        }
                        if (debugLogging)
                        {
                            Logging.log("Enemy " + i + " v=" + vanillaValue + " z=" + zeroValue + " l=" + level + " (" + ((vanillaValue - zeroValue) / (double)level) + ") a=" + avgStatValuePerLevel[typeIndex] + " d=" + diffThreshold, "debug");
                        }
                        if ((vanillaValue - zeroValue) / (double)level < avgStatValuePerLevel[typeIndex] - diffThreshold)
                        {
                            enemyClass = 0; // low stat
                        }
                        else if ((vanillaValue - zeroValue) / (double)level > avgStatValuePerLevel[typeIndex] + diffThreshold)
                        {   
                            enemyClass = 2; // high stat
                        }
                        else
                        {   
                            enemyClass = 1; // avg stat
                        }

                        if (typeIndex == 1)
                        {
                            // boss
                            if (offset == 3)
                            {
                                // boss mp
                                if (vanillaValue == 99)
                                {
                                    // mp doesn't scale too well since it caps at 99, keep high mp if it was originally 99
                                    enemyClass = 2;
                                }
                                else
                                {
                                    // avg mp; further down we'll use the 6 for tonpoles
                                    enemyClass = 1;
                                }
                            }
                        }
                        if (offset == 8 || offset == 11)
                        {
                            // evades - 0 or 99 only; keep vanilla
                            if (vanillaValue == 0)
                            {
                                enemyClass = 0;
                            }
                            else
                            {
                                enemyClass = 2;
                            }
                        }

                        if (typeIndex == 1 && (offset == 23 || offset == 24))
                        {
                            // boss weapons, always use avg
                            enemyClass = 1;
                        }

                        // tonpole mp
                        if (!randomizeBossElements && typeIndex == 1 && offset == 3 && (i == 95 || i == 113 || i == 122))
                        {
                            // 6 mp
                            enemyClass = 0;
                        }

                        if (offset == 260)
                        {
                            // high magic level always
                            enemyClass = 2;
                        }

                        if ((offset == 9 || offset == 1) && difficulty == "impossible")
                        {
                            // low hp/def to give you some sort of shot
                            enemyClass = 0;
                        }

                        enemyClass = (byte)(enemyClass + typeIndex * 3);
                        finalStatClasses[offset][i] = enemyClass; // for logging
                        if (debugLogging)
                        {
                            Logging.log("Enemy " + i + " is class " + enemyClass + " for stat " + offset, "debug");
                        }
                        outRom[context.workingOffset++] = enemyClass;
                    }
                }
            }

            // logging for stat values
            Logging.log("Stat values for open world enemies:", "debug");
            for (int enemyId = 0; enemyId < 128; enemyId++)
            {
                Logging.log("ENEMY: " + context.namesOfThings.getOriginalName(NamesOfThings.INDEX_ENEMIES_START + enemyId), "debug");
                string vanillaStats = "[VANILLA] ";
                int vanillaLevel = 0;
                foreach (int statNum in sourceOffsets)
                {
                    Dictionary<int, int> vanillaValues = vanillaStatValues[statNum];
                    string statname = statNames[statNum];
                    if (vanillaValues.ContainsKey(enemyId))
                    {
                        int vanillaValue = vanillaValues[enemyId];
                        if (statNum == 0)
                        {
                            vanillaLevel = vanillaValue;
                        }
                        string addStr = statname + "=" + vanillaValue + " ";
                        while (addStr.Length < 16)
                        {
                            addStr += " ";
                        }

                        vanillaStats += addStr;
                    }
                }

                Logging.log(vanillaStats, "debug");

                string[] statClassIndicators = new string[] { "-", " ", "+", "-", " ", "+", };
                List<int> logLevels = new int[] { 1, 5, 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 99 }.ToList();
                for (int level = 1; level < 100; level++)
                {
                    String statsString = "          Level=" + level + "  ";
                    while (statsString.Length < 26)
                    {
                        statsString += " ";
                    }

                    foreach (int statNum in sourceOffsets)
                    {
                        Dictionary<int, Dictionary<int, int>> statValues = finalStatValues[statNum];
                        Dictionary<int, int> statClasses = finalStatClasses[statNum];
                        string statname = statNames[statNum];
                        if (statClasses.ContainsKey(enemyId))
                        {
                            int statClass = statClasses[enemyId];
                            Dictionary<int, int> levelStatValues = statValues[level];
                            int statValue = levelStatValues[statClass];
                            string addStr = statname + "=" + statValue + "[" + statClassIndicators[statClass] + "]  ";
                            while (addStr.Length < 16)
                            {
                                addStr += " ";
                            }
                            statsString += addStr;
                        }
                    }
                    if (debugLogging || logLevels.Contains(level))
                    {
                        Logging.log(statsString, "debug");
                    }

                    if (vanillaLevel == level)
                    {
                        statsString = "   DIFFS            ";
                        while (statsString.Length < 26)
                        {
                            statsString += " ";
                        }

                        foreach (int statNum in sourceOffsets)
                        {
                            Dictionary<int, int> vanillaValues = vanillaStatValues[statNum];
                            int vanillaValue = vanillaValues[enemyId];
                            Dictionary<int, Dictionary<int, int>> statValues = finalStatValues[statNum];
                            Dictionary<int, int> statClasses = finalStatClasses[statNum];
                            string statname = statNames[statNum];
                            if (statClasses.ContainsKey(enemyId))
                            {
                                int statClass = statClasses[enemyId];
                                Dictionary<int, int> levelStatValues = statValues[level];
                                int statValue = levelStatValues[statClass];
                                statValue = statValue - vanillaValue;
                                string statValueString = "" + statValue;
                                if (statValue >= 0)
                                {
                                    statValueString = "+" + statValueString;
                                }
                                string addStr = "d" + statname + "=" + statValueString;
                                while (addStr.Length < 16)
                                {
                                    addStr += " ";
                                }
                                statsString += addStr;
                            }
                        }
                        Logging.log(statsString, "debug");
                    }
                }
            }

            // more logging for stat values
            Logging.log("-------- STAT +/- TOTALS:", "debug");
            foreach (int statNum in sourceOffsets)
            {
                int[] num = new int[] { 0, 0, 0, 0, 0, 0 };
                string[] classNames = new string[] { "below avg", "avg", "above avg", "below avg (boss)", "avg (boss)", "above avg (boss)" };
                Dictionary<int, int> statData = finalStatClasses[statNum];
                foreach (int key in statData.Keys)
                {
                    int value = statData[key];
                    num[value]++;
                }

                string logString = statNames[statNum] + ": ";
                for (int i = 0; i < 6; i++)
                {
                    logString += classNames[i] + ": " + num[i] + "; ";
                }
                Logging.log(logString, "debug");
            }

            List<byte> enemyLevelupMessage = VanillaEventUtil.getBytes("Enemy level is now ");
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, enemyLevelupMessage.Count + 1);
            int messageLocation = context.workingOffset;
            Logging.log("enemy message location: " + messageLocation.ToString("X6"), "debug");
            foreach (byte b in enemyLevelupMessage)
            {
                outRom[context.workingOffset++] = b;
            }
            outRom[context.workingOffset++] = 0;


            // tonpole breaks with 99 agi andd just stands there unable to die or do anything with 99 agi
            /*
                $C2/3372 BD 89 01    LDA $0189,x[$7E:E789]   A:0000 X:E600 Y:0001 P:envmxdIZc
                $C2/3375 29 FF 00    AND #$00FF              A:0163 X:E600 Y:0001 P:envmxdIzc
                $C2/3378 85 95       STA $95    [$00:0295]   A:0063 X:E600 Y:0001 P:envmxdIzc
                $C2/337A A9 64 00    LDA #$0064              A:0063 X:E600 Y:0001 P:envmxdIzc
                replace:
                $C2/337D 38          SEC                     A:0064 X:E600 Y:0001 P:envmxdIzc **
                $C2/337E E5 95       SBC $95    [$00:0295]   A:0064 X:E600 Y:0001 P:envmxdIzC **
                $C2/3380 4A          LSR A                   A:0001 X:E600 Y:0001 P:envmxdIzC **
                $C2/3381 9D 2D 00    STA $002D,x[$7E:E62D]   A:0000 X:E600 Y:0001 P:envmxdIZC
             */
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 20);
            outRom[0x2337D] = 0x22;
            outRom[0x2337E] = (byte)(context.workingOffset);
            outRom[0x2337F] = (byte)(context.workingOffset >> 8);
            outRom[0x23380] = (byte)((context.workingOffset >> 16) + 0xC0);
            // OG code
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC $95
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x95;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x03;
            // LDA #0001 - don't let 0x0000 go into $2d
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // level thingy for bosses; store in LEVELMATCH_CURRENT_MAX_LEVEL_8BIT
            if (enemyLevelSource == "bosses")
            {
                // MOPPLE: i think this was supposed to make it say how many bosses you have killed in a text box like how timed mode does, but it's never worked
                CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 200);
                int messageGenSubrLocation = context.workingOffset;
                makeDisplayCurrentLevelSub(outRom, CustomRamOffsets.LEVELMATCH_BOSSES_KILLED, context, messageLocation);
                CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 50);
                int msgLocation = CustomRamOffsets.MESSAGE_TEMPORARY_BUFFER;
                int pushTextSubrLoc = context.workingOffset;

                // LDA #xx
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = (byte)msgLocation;
                // STA TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 0);
                outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 0) >> 8);
                outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 0) >> 16);

                // LDA #xx
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = (byte)(msgLocation >> 8);
                // STA TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT+1
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 1);
                outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 1) >> 8);
                outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 1) >> 16);
                // LDA #xx
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = (byte)((msgLocation >> 16));
                // STA TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT+2
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 2);
                outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 2) >> 8);
                outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 2) >> 16);
                // JSL topOfScreenText
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = (byte)screenTextSubrLocation;
                outRom[context.workingOffset++] = (byte)(screenTextSubrLocation >> 8);
                outRom[context.workingOffset++] = (byte)((screenTextSubrLocation >> 16) + 0xC0);

                outRom[context.workingOffset++] = 0x6B;

                byte incrementAmount = (byte)bossIncrementValue;
                CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 200);
                outRom[0x4205] = 0x22;
                outRom[0x4206] = (byte)(context.workingOffset);
                outRom[0x4207] = (byte)(context.workingOffset >> 8);
                outRom[0x4208] = (byte)((context.workingOffset >> 16) + 0xC0);
                outRom[0x4209] = 0xEA;
                outRom[0x420A] = 0xEA;

                // CPX #0600  E0 00 06
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x06;

                // BCS/BGE over [07] - skip RTL if enemy
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0x07;
                // STZ $E1B1,x
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0xB1;
                outRom[context.workingOffset++] = 0xE1;
                // STZ $E1B0,x
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0xE1;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
                // over:
                // LDA $E180,x
                outRom[context.workingOffset++] = 0xBD;
                outRom[context.workingOffset++] = 0x80;
                outRom[context.workingOffset++] = 0xE1;
                // CMP #57 - mantis ant
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = 0x57;
                // BCS greater [xx] - skip if greater or equal
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0x0B;
                // CMP #2E (shadow x1)
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = 0x2E;
                // BNE over 0F = 04 + 0B to skip to end plus 2 for some reason **** change to BEQ and have removed code + RTL here (7 bytes)
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x07;
                // STZ $E1B1,x
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0xB1;
                outRom[context.workingOffset++] = 0xE1;
                // STZ $E1B0,x
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0xE1;
                // RTL
                outRom[context.workingOffset++] = 0x6B;

                // greater:
                // CMP #80 - first npc
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = 0x80;
                // BCS/BGE skip [xx] **** **** change to BCC and have removed code + RTL here (7 bytes)
                outRom[context.workingOffset++] = 0x90;
                outRom[context.workingOffset++] = 0x07;
                // STZ $E1B1,x
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0xB1;
                outRom[context.workingOffset++] = 0xE1;
                // STZ $E1B0,x
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0xE1;
                // RTL
                outRom[context.workingOffset++] = 0x6B;

                // **** add checks for non-bosses in boss range (brambler, etc) - 103, 108, 126
                byte[] nonBossBosses = new byte[] { 103, 108, 126 };
                foreach (byte nonBossBoss in nonBossBosses)
                {
                    // CMP #xx
                    outRom[context.workingOffset++] = 0xC9;
                    outRom[context.workingOffset++] = nonBossBoss;
                    // BNE over (07)
                    outRom[context.workingOffset++] = 0xD0;
                    outRom[context.workingOffset++] = 0x07;
                    // (replaced code + rtl)
                    // STZ $E1B1,x
                    outRom[context.workingOffset++] = 0x9E;
                    outRom[context.workingOffset++] = 0xB1;
                    outRom[context.workingOffset++] = 0xE1;
                    // STZ $E1B0,x
                    outRom[context.workingOffset++] = 0x9E;
                    outRom[context.workingOffset++] = 0xB0;
                    outRom[context.workingOffset++] = 0xE1;
                    // RTL
                    outRom[context.workingOffset++] = 0x6B;
                }
                // here: boss handling
                // LDA LEVELMATCH_CURRENT_MAX_LEVEL_8BIT
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_BOSSES_KILLED);
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_BOSSES_KILLED >> 8);
                outRom[context.workingOffset++] = (byte)((CustomRamOffsets.LEVELMATCH_BOSSES_KILLED >> 16));
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC difficultyAmount
                outRom[context.workingOffset++] = 0x69;
                outRom[context.workingOffset++] = incrementAmount;
                // don't let it go over 99
                // CMP #63
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = 0x63 * 2; // since we div by 2 when we use it now
                // BLT/BCC over
                outRom[context.workingOffset++] = 0x90;
                outRom[context.workingOffset++] = 0x02;
                // LDA #63
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x63 * 2;

                // STA LEVELMATCH_CURRENT_MAX_LEVEL_8BIT
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_BOSSES_KILLED);
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_BOSSES_KILLED >> 8);
                outRom[context.workingOffset++] = (byte)((CustomRamOffsets.LEVELMATCH_BOSSES_KILLED >> 16));

                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = (byte)messageGenSubrLocation;
                outRom[context.workingOffset++] = (byte)(messageGenSubrLocation >> 8);
                outRom[context.workingOffset++] = (byte)((messageGenSubrLocation >> 16) + 0xC0);

                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = (byte)pushTextSubrLoc;
                outRom[context.workingOffset++] = (byte)(pushTextSubrLoc >> 8);
                outRom[context.workingOffset++] = (byte)((pushTextSubrLoc >> 16) + 0xC0);

                // skip:
                // removed code; rtl
                // STZ $E1B1,x
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0xB1;
                outRom[context.workingOffset++] = 0xE1;
                // STZ $E1B0,x
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0xE1;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            else if (enemyLevelSource == "timed")
            {
                CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 400);
                int messageGenSubrLocation = context.workingOffset;
                makeDisplayCurrentLevelSub(outRom, CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT, context, messageLocation);
                // vblank handlers.  same ones as in Layer3Changes for the ancient cave timer - increment timer value every frame
                outRom[0xC0AF] = 0x22;
                outRom[0xC0B0] = (byte)context.workingOffset;
                outRom[0xC0B1] = (byte)(context.workingOffset >> 8);
                outRom[0xC0B2] = (byte)((context.workingOffset >> 16) + 0xC0);

                generateIncrementCode(outRom, context, new byte[] { 0xC2, 0x20, 0xA9, 0x00, 0x00, 0x5B }, timerIncrementValue, messageGenSubrLocation, screenTextSubrLocation, pauseTimerInMenu);
                outRom[0x20B2] = 0x22;
                outRom[0x20B3] = (byte)context.workingOffset;
                outRom[0x20B4] = (byte)(context.workingOffset >> 8);
                outRom[0x20B5] = (byte)((context.workingOffset >> 16) + 0xC0);

                generateIncrementCode(outRom, context, new byte[] { 0xAF, 0x10, 0x42, 0x00 }, timerIncrementValue, messageGenSubrLocation, screenTextSubrLocation, pauseTimerInMenu);

            }


            // now replace the stat loaders
            // 5625 - full stats
            // 477c - partial stats
            // grab the current enemy level as determined by other code
            // then for each stat, load the enemy's class for that stat from the enemy classes table, 
            // and then load the necessary stat value for the level from the stat values table.

            // copy the original one for vanilla mana beast, if we selected that option
            // moved this forward 4 bytes since CrystalOrbColors hack uses 5625 and this was removing it
            List<byte> orig5629 = new List<byte>();
            for (int i = 0x5629; i <= 0x5696; i++)
            {
                orig5629.Add(origRom[i]);
            }
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, orig5629.Count + 1);
            int manaBeast5629Offset = context.workingOffset;
            foreach (byte b in orig5629)
            {
                outRom[context.workingOffset++] = b;
            }
            // rtl
            outRom[context.workingOffset++] = 0x6B;


            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 1000);

            outRom[0x5629] = 0x22;
            outRom[0x562A] = (byte)(context.workingOffset);
            outRom[0x562B] = (byte)(context.workingOffset >> 8);
            outRom[0x562C] = (byte)((context.workingOffset >> 16) + 0xC0);

            for (int i = 0x562D; i <= 0x5696; i++)
            {
                outRom[i] = 0xEA;
            }
            // CPX #0E63 for mana beast, or a value that we'll never encounter
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = (byte)(vanillaManaBeast ? 0x63 : 0x64); // lol
            outRom[context.workingOffset++] = 0x0E;
            // BNE over (5)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // JSL manaBeast5629Offset - use vanilla stat loader
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(manaBeast5629Offset);
            outRom[context.workingOffset++] = (byte)(manaBeast5629Offset >> 8);
            outRom[context.workingOffset++] = (byte)((manaBeast5629Offset >> 16) + 0xC0);
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // otherwise, use the new loader, starting with figuring out what enemy level should be
            determineEnemyLevel(outRom, context, enemyLevelSource, timerIncrementValue, messageLocation, screenTextSubrLocation, minLevelTableLoc, maxLevelTableLoc, noFutureLevel);

            // PHX
            outRom[context.workingOffset++] = 0xDA;

            /*
             * from vanilla
                C0/5644:	BF141CD0	LDA $D01C14,X - status immunity flags
                C0/5648:	99AAE1  	STA $E1AA,Y
                C0/564B:	BF171CD0	LDA $D01C17,X - enemy weapons
                C0/564F:	99E8E1  	STA $E1E8,Y
             */
            // LDA $D01C14,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x1C;
            outRom[context.workingOffset++] = 0xD0;
            // LDA $E1AA,Y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0xAA;
            outRom[context.workingOffset++] = 0xE1;
            // LDA $D01C17,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x17;
            outRom[context.workingOffset++] = 0x1C;
            outRom[context.workingOffset++] = 0xD0;
            // LDA $E1A8,Y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xE1;

            // from vanilla, set species byte:
            //** C0/5670:   A5 B2         LDA $B2
            //** C0/5672:   99 80 E1      STA $E180,Y
            //** C0/5675:   99 E7 E1      STA $E1E7,Y
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $B2
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xB2;
            // STA $E180,Y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xE1;
            // STA $E1E7,Y - species
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0xE7;
            outRom[context.workingOffset++] = 0xE1;
            foreach (int offset in sourceOffsets)
            {
                Logging.log("Writing stat " + offset.ToString("X6"), "debug");
                if (offset == 0)
                {
                    writeLevelLoader(outRom, context, ramLocations[offset]);
                }
                else if (offset != 23 && offset != 24) // don't load weapons here
                {
                    writeStatLoader(outRom, context, classLocations[offset], valueTableLocations[offset], sixteenBitOffsets.Contains(offset), ramLocations[offset]);
                }
            }
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // copy the original one for vanilla mana beast, if we selected that option
            /*
             *  replace stat loader
                $C0/477E BF 04 1C D0 LDA $D01C04,x[$D0:1C04] A:0000 X:0000 Y:0600 P:envmxdIzC - load and store a bunch of 16-bit enemy info blocks in a row
                $C0/4782 99 88 E1    STA $E188,y[$7E:E788]   A:0103 X:0000 Y:0600 P:envmxdIzC - str/agi
                $C0/4785 BF 06 1C D0 LDA $D01C06,x[$D0:1C06] A:0103 X:0000 Y:0600 P:envmxdIzC
                $C0/4789 99 8B E1    STA $E18B,y[$7E:E78B]   A:0101 X:0000 Y:0600 P:envmxdIzC - int/wis
                $C0/478C BF 08 1C D0 LDA $D01C08,x[$D0:1C08] A:0101 X:0000 Y:0600 P:envmxdIzC
                $C0/4790 99 A4 E1    STA $E1A4,y[$7E:E7A4]   A:0000 X:0000 Y:0600 P:envmxdIZC - evade/high byte of def
                $C0/4793 BF 0A 1C D0 LDA $D01C0A,x[$D0:1C0A] A:0000 X:0000 Y:0600 P:envmxdIZC
                $C0/4797 99 A6 E1    STA $E1A6,y[$7E:E7A6]   A:0000 X:0000 Y:0600 P:envmxdIZC - low byte of def, then m.evade
                $C0/479A BF 0C 1C D0 LDA $D01C0C,x[$D0:1C0C] A:0000 X:0000 Y:0600 P:envmxdIZC
                $C0/479E 99 A8 E1    STA $E1A8,y[$7E:E7A8]   A:0000 X:0000 Y:0600 P:envmxdIZC - mdef
                $C0/47A1 BF 0E 1C D0 LDA $D01C0E,x[$D0:1C0E] A:0000 X:0000 Y:0600 P:envmxdIZC
                $C0/47A5 99 92 E1    STA $E192,y[$7E:E792]   A:0008 X:0000 Y:0600 P:envmxdIzC - enemy type & element
                $C0/47A8 BF 09 1C D0 LDA $D01C09,x[$D0:1C09] A:0008 X:0000 Y:0600 P:envmxdIzC
                $C0/47AC 99 A5 E1    STA $E1A5,y[$7E:E7A5]   A:0000 X:0000 Y:0600 P:envmxdIZC - def (again?)
                $C0/47AF BF 0C 1C D0 LDA $D01C0C,x[$D0:1C0C] A:0000 X:0000 Y:0600 P:envmxdIZC
                $C0/47B3 99 A8 E1    STA $E1A8,y[$7E:E7A8]   A:0000 X:0000 Y:0600 P:envmxdIZC - mdef (again?)
             */
            List<byte> orig477e = new List<byte>();
            for (int i = 0x477e; i <= 0x47bc; i++)
            {
                orig477e.Add(origRom[i]);
            }
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, orig477e.Count + 1);
            int manaBeast477eOffset = context.workingOffset;
            foreach (byte b in orig477e)
            {
                outRom[context.workingOffset++] = b;
            }
            // rtl
            outRom[context.workingOffset++] = 0x6B;


            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 1000);
            outRom[0x477E] = 0x22;
            outRom[0x477F] = (byte)(context.workingOffset);
            outRom[0x4780] = (byte)(context.workingOffset >> 8);
            outRom[0x4781] = (byte)((context.workingOffset >> 16) + 0xC0);
            for (int i = 0x4782; i <= 0x47BC; i++)
            {
                outRom[i] = 0xEA;
            }
            // like above in the other stat loader, check for mana beast if we have it set to vanilla
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = (byte)(vanillaManaBeast ? 0x63 : 0x64); // lol
            outRom[context.workingOffset++] = 0x0E;
            // BNE over (5)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // run the vanilla 477e instead of the custom one
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(manaBeast477eOffset);
            outRom[context.workingOffset++] = (byte)(manaBeast477eOffset >> 8);
            outRom[context.workingOffset++] = (byte)((manaBeast477eOffset >> 16) + 0xC0);
            outRom[context.workingOffset++] = 0x6B;
            // get the enemy level
            determineEnemyLevel(outRom, context, enemyLevelSource, timerIncrementValue, messageLocation, screenTextSubrLocation, minLevelTableLoc, maxLevelTableLoc, noFutureLevel);
            // PHX
            outRom[context.workingOffset++] = 0xDA;

            // keep these from vanilla:
            // C0/47A1: BF0E1CD0    LDA $D01C0E,X  - load type and element
            // C0/47A5: 9992E1      STA $E192,Y     
            // LDA $D01C0E,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x0E;
            outRom[context.workingOffset++] = 0x1C;
            outRom[context.workingOffset++] = 0xD0;
            // STA $E192,Y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0x92;
            outRom[context.workingOffset++] = 0xE1;
            // TYX
            outRom[context.workingOffset++] = 0xBB;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $E1E7,X - species
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xE7;
            outRom[context.workingOffset++] = 0xE1;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            foreach (int offset in sourceOffsets)
            {
                if (altLoaderStats.Contains(offset))
                {
                    // load secondary stats based on the species in x
                    writeStatLoaderSmall(outRom, context, classLocations[offset], valueTableLocations[offset], sixteenBitOffsets.Contains(offset), ramLocations[offset]);
                }
            }
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 200);
            // weapon damage loader; 49CC-49CF
            // - $C0/49CC 7D 88 E1    ADC $E188,x[$7E:E988]   A:0007 X:0800 Y:0800 P:envMxdIzc  - add enemy str
            // - $C0/49CF 9D 98 E1    STA $E198,x[$7E:E798]   A:0035 X:0600 Y:0600 P:envMxdIzc	- store into atk
            outRom[0x49CC] = 0x22;
            outRom[0x49CD] = (byte)(context.workingOffset);
            outRom[0x49CE] = (byte)(context.workingOffset >> 8);
            outRom[0x49CF] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x49D0] = 0xEA;
            outRom[0x49D1] = 0xEA;
            int weaponLoaderOffset = context.workingOffset; // keep this for later, we'll need another of these
            // SEP 20, since this ^ REP 20s
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // here: 8 bit accumulator, 16 bit x/y; in X is the object index
            // CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BCS (BGE) over - skip returning early if enemy
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x07;
            // replaced code:
            // ADC $E188,X
            outRom[context.workingOffset++] = 0x7D;
            outRom[context.workingOffset++] = 0x88;
            outRom[context.workingOffset++] = 0xE1;
            // STA $E198,X
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x98;
            outRom[context.workingOffset++] = 0xE1;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // get enemy level
            determineEnemyLevel(outRom, context, enemyLevelSource, timerIncrementValue, messageLocation, screenTextSubrLocation, minLevelTableLoc, maxLevelTableLoc, noFutureLevel);
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $E1E7,X - enemy species
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xE7;
            outRom[context.workingOffset++] = 0xE1;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // here we're just gonna load weapon A's power for now; it should be mostly the same as B for every enemy
            int weaponClassOffset = classLocations[23];
            // LDA #classLocations[offset],X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)weaponClassOffset;
            outRom[context.workingOffset++] = (byte)(weaponClassOffset >> 8);
            outRom[context.workingOffset++] = (byte)((weaponClassOffset >> 16) + 0xC0);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 16);
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            int weaponStatValueOffset = valueTableLocations[23];
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA weaponStatValueOffset,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)weaponStatValueOffset;
            outRom[context.workingOffset++] = (byte)(weaponStatValueOffset >> 8);
            outRom[context.workingOffset++] = (byte)((weaponStatValueOffset >> 16) + 0xC0);
            // now use this value in the replaced code
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // - $C0/49CC 7D 88 E1    ADC $E188,x[$7E:E988]   A:0007 X:0800 Y:0800 P:envMxdIzc
            outRom[context.workingOffset++] = 0x7D;
            outRom[context.workingOffset++] = 0x88;
            outRom[context.workingOffset++] = 0xE1;
            // check for overflow (carry bit)
            // BCC over [02]
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;
            // LDA #FF - max if overflowed
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xFF;
            // - $C0/49CF 9D 98 E1    STA $E198,x[$7E:E798]   A:0035 X:0600 Y:0600 P:envMxdIzc
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x98;
            outRom[context.workingOffset++] = 0xE1;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            /*
             *  seemingly another copy of the above weapon loader?  reference the same sub here:
                C0/4631:    7988E1      ADC $E188,Y - add enemy str
                C0/4634:    9998E1      STA $E198,Y	- store into atk
             */
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 30);
            outRom[0x4631] = 0x22;
            outRom[0x4632] = (byte)(context.workingOffset);
            outRom[0x4633] = (byte)(context.workingOffset >> 8);
            outRom[0x4634] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x4635] = 0xEA;
            outRom[0x4636] = 0xEA;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // TYX
            outRom[context.workingOffset++] = 0xBB;
            // JSL weaponLoader - the 49CC bit we replaced above
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(weaponLoaderOffset);
            outRom[context.workingOffset++] = (byte)(weaponLoaderOffset >> 8);
            outRom[context.workingOffset++] = (byte)((weaponLoaderOffset >> 16) + 0xC0);
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            

            /*
             * a few changes to the loading of some of the 7ECFxx flags from saveram to allow them to be > 0x0F in RAM 
             * and represent time passed, bosses kills, stuff like that
             *  $C7/588E BF 00 60 30 LDA $306000,x[$30:68C4] A:3F00 X:08C4 Y:CF82 P:envMxdIZc
             *  replace:
                $C7/5893 4A          LSR A                   A:3F00 X: 08C4 Y:CF82 P:envMxdIZc
                $C7/5894 4A          LSR A                   A:3F00 X: 08C4 Y:CF82 P:envMxdIZc
                $C7/5895 4A          LSR A                   A:3F00 X: 08C4 Y:CF82 P:envMxdIZc
                $C7/5896 4A          LSR A                   A:3F00 X: 08C4 Y:CF82 P:envMxdIZc
                $C7/5897 99 00 00    STA $0000,y[$7E:CF82]   A:3F00 X:08C4 Y:CF82 P:envMxdIZc
             */
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 30);
            outRom[0x75893] = 0x22;
            outRom[0x75894] = (byte)(context.workingOffset);
            outRom[0x75895] = (byte)(context.workingOffset >> 8);
            outRom[0x75896] = (byte)((context.workingOffset >> 16) + 0xC0);
            // CPY #CF80 - event flag 0x80
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xCF;
            // BGE/BCS - check upper bounds
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x05;
            // (replaced code) LSR 4 times
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // CPY #CF88 - event flag 0x88
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x88;
            outRom[context.workingOffset++] = 0xCF;
            // BLT/BCC - check lower bounds
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x05;
            // (replaced code) LSR 4 times
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // here: custom loading for event flags 0x80 through 0x87
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA $4,s
            outRom[context.workingOffset++] = 0x83;
            outRom[context.workingOffset++] = 0x05;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // and then the save part of those same flags:
            // $C7/56FB B9 00 00    LDA $0000,y[$7E:CF80]   A:0000 X:08C3 Y:CF80 P:envMxdIzc
            // replace:
            // $C7/56FE 0A          ASL A                   A:0056 X:08C3 Y:CF80 P:envMxdIzc
            // $C7/56FF 0A          ASL A                   A:00AC X:08C3 Y:CF80 P:eNvMxdIzc
            // $C7/5700 0A          ASL A                   A:0058 X:08C3 Y:CF80 P:envMxdIzC
            // $C7/5701 0A          ASL A                   A:00B0 X:08C3 Y:CF80 P:eNvMxdIzc
            //
            // $C7/5702 8D CB A1    STA $A1CB[$7E:A1CB]     A:0060 X:08C3 Y:CF80 P:envMxdIzC
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 30);
            outRom[0x756FE] = 0x22;
            outRom[0x756FF] = (byte)(context.workingOffset);
            outRom[0x75700] = (byte)(context.workingOffset >> 8);
            outRom[0x75701] = (byte)((context.workingOffset >> 16) + 0xC0);
            // CPY #CF80 - event flag 0x80
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xCF;
            // BGE/BCS - lower bound
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x05;
            // (replaced code) ASL 4 times
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // CPY #CF88 - event flag 0x88
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x88;
            outRom[context.workingOffset++] = 0xCF;
            // BLT/BCC - upper bound - don't shift
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x04;
            // (replaced code) ASL 4 times
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }

        private void makeDisplayCurrentLevelSub(byte[] outRom, int addrOfValue, RandoContext context, int messageLocation)
        {
            // create subroutine to render the current enemy level to a text box, given the address of the level.
            int messageGenSubrLocation = context.workingOffset;
            // to show level - write message to RamOffsets.MESSAGE_TEMPORARY_BUFFER
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // PHY
            outRom[context.workingOffset++] = 0x5A;
            // PHB
            outRom[context.workingOffset++] = 0x8B;
            // LDA #7E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x7E;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // LDX #0000
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // LDY #0000
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // loop:
            // LDA messageLocation,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)messageLocation;
            outRom[context.workingOffset++] = (byte)(messageLocation >> 8);
            outRom[context.workingOffset++] = (byte)((messageLocation >> 16) + 0xC0);
            // STA RamOffsets.MESSAGE_TEMPORARY_BUFFER,y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.MESSAGE_TEMPORARY_BUFFER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MESSAGE_TEMPORARY_BUFFER >> 8);
            // BEQ out [02]
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x04;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // BRA loop
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xF3;
            // out:
            // -- load the 16-bit value from addrOfValue, add 1 to it, and print that.
            // divide by 10, if it's 0, skip. otherwise print; mod by 10, print (add 0xB5)
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA addrOfValue
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)addrOfValue;
            outRom[context.workingOffset++] = (byte)(addrOfValue >> 8);
            outRom[context.workingOffset++] = (byte)((addrOfValue >> 16));
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // INC
            outRom[context.workingOffset++] = 0x1A;
            // STA 004204
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #0A
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x0A;
            // STA 004206
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x06;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // 8? nops
            for (int i = 0; i < 8; i++)
            {
                outRom[context.workingOffset++] = 0xEA;
            }
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 004214 - first (ms) digit
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // BEQ xx [07] - skip if zero
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x07;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #B5
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;
            // STA RamOffsets.MESSAGE_TEMPORARY_BUFFER,y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.MESSAGE_TEMPORARY_BUFFER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MESSAGE_TEMPORARY_BUFFER >> 8);
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // xx:
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 004216 - second (ls) digit
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x16;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #B5
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;
            // STA RamOffsets.MESSAGE_TEMPORARY_BUFFER,y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.MESSAGE_TEMPORARY_BUFFER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MESSAGE_TEMPORARY_BUFFER >> 8);
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA RamOffsets.MESSAGE_TEMPORARY_BUFFER,y - terminate the string
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.MESSAGE_TEMPORARY_BUFFER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MESSAGE_TEMPORARY_BUFFER >> 8);
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
        }

        private void generateIncrementCode(byte[] outRom, RandoContext context, byte[] replacedCode, double difficultyLevel, int generateMsgSubrLocation, int screenTextSubrLocation, bool pauseInMenu)
        {
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7ECF73 - timer running
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = EventFlags.OPEN_WORLD_TIMED_MODE_ENABLE;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BEQ over if timer enabled
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = (byte)(replacedCode.Length + 1);
            foreach (byte b in replacedCode)
            {
                outRom[context.workingOffset++] = b;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // also check if we're in a menu
            if (pauseInMenu)
            {
                // zero if normal controls
                // LDA 7E002C
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x2C;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x7E;
                // BEQ over - continue
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = (byte)(replacedCode.Length + 1);
                foreach (byte b in replacedCode)
                {
                    outRom[context.workingOffset++] = b;
                }
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            // check for done leveling
            // LDA TIMED_LEVEL_VALUE_8BIT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT >> 16);
            // CMP #62 - max level
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x62;
            // BLT over - continue
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = (byte)(replacedCode.Length + 1);
            foreach (byte b in replacedCode)
            {
                outRom[context.workingOffset++] = b;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // 24-bit timer: frames, seconds low byte, seconds high byte.
            // increment and carry over to the next highest, similar to the more extensive timer for ancient cave in Layer3Changes.

            // LDA $LEVELMATCH_TIMER_FRAMES
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_FRAMES;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_FRAMES >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_FRAMES >> 16);
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // INC
            outRom[context.workingOffset++] = 0x1A;
            // STA $LEVELMATCH_TIMER_FRAMES
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_FRAMES;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_FRAMES >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_FRAMES >> 16);
            // CMP #3C - 60 frames have passed; need to increment the seconds
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x3C;
            // BEQ over to not rtl
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = (byte)(replacedCode.Length + 1);
            foreach (byte b in replacedCode)
            {
                outRom[context.workingOffset++] = b;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // now that the seconds have changed, see if the enemy level should be increasing.

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STZ $LEVELMATCH_TIMER_FRAMES
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_FRAMES;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_FRAMES >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_FRAMES >> 16);
            // LDA LEVELMATCH_TIMER_SECONDS_HIGH
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH >> 16);
            // XBA
            outRom[context.workingOffset++] = 0xEB;
            // LDA LEVELMATCH_TIMER_SECONDS_LOW
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW >> 16);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // STA 004204
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            byte divisor = (byte)difficultyLevel;
            // LDA #divisor based on difficulty level
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = divisor;
            // STA 004206
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x06;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // eight nops?
            for (int i = 0; i < 8; i++)
            {
                outRom[context.workingOffset++] = 0xEA;
            }
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 004214
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // shift right once so we top off at 127 instead of 255
            outRom[context.workingOffset++] = 0x4A;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // PHA
            outRom[context.workingOffset++] = 0x48;
            // CMP TIMED_LEVEL_VALUE_8BIT
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT >> 16);
            outRom[context.workingOffset++] = 0xF0; // BEQ past setting the enemy level (over)
            outRom[context.workingOffset++] = 4 + 6 * 3 + 4 + 2 + 4;
            // here: the enemy level increased
            // STA TIMED_LEVEL_VALUE_8BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TIMED_LEVEL_VALUE_8BIT >> 16);
            // JSL generateMsgSubrLocation
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)generateMsgSubrLocation;
            outRom[context.workingOffset++] = (byte)(generateMsgSubrLocation >> 8);
            outRom[context.workingOffset++] = (byte)((generateMsgSubrLocation >> 16) + 0xC0);
            // use MESSAGE_TEMPORARY_BUFFER from messageLocation and say what the new level is
            int messageLocation = CustomRamOffsets.MESSAGE_TEMPORARY_BUFFER;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #xx
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)messageLocation;
            // STA TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 0);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 0) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 0) >> 16);
            // LDA #messageLocation
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)(messageLocation >> 8);
            // STA TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT+1
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 1);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 1) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 1) >> 16);
            // LDA #messageLocation
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)((messageLocation >> 16));
            // STA TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 2) >> 16);
            // JSL topOfScreenText
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)screenTextSubrLocation;
            outRom[context.workingOffset++] = (byte)(screenTextSubrLocation >> 8);
            outRom[context.workingOffset++] = (byte)((screenTextSubrLocation >> 16) + 0xC0);
            // over: - the enemy level didn't increase yet (or we finished dealing with it)
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // now increment the seconds
            // LDA LEVELMATCH_TIMER_SECONDS_LOW
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW >> 16);
            // CLC/INC
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x1A;
            // STA
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW >> 16);
            // BEQ over to not rtl
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = (byte)(replacedCode.Length + 1);
            foreach (byte b in replacedCode)
            {
                outRom[context.workingOffset++] = b;
            }
            outRom[context.workingOffset++] = 0x6B;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // zero out the frames (is this necessary?)
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_FRAMES;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_FRAMES >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_FRAMES >> 16);

            // LDA LEVELMATCH_TIMER_SECONDS_HIGH
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH >> 16);
            // CLC/INC
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x1A;
            // STA LEVELMATCH_TIMER_SECONDS_HIGH
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH >> 16);
            // BEQ if we completely overflowed
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = (byte)(replacedCode.Length + 1);
            foreach (byte b in replacedCode)
            {
                outRom[context.workingOffset++] = b;
            }
            outRom[context.workingOffset++] = 0x6B;
            // LDA FF - at max timer - keep everything at 0xFF
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xFF;
            // STA LEVELMATCH_TIMER_SECONDS_LOW
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW >> 16);
            // STA LEVELMATCH_TIMER_SECONDS_HIGH
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH >> 16);
            foreach (byte b in replacedCode)
            {
                outRom[context.workingOffset++] = b;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
        }


        private void determineEnemyLevel(byte[] outRom, RandoContext context, string enemyLevelSource, double timedDifficultyValue, int messageLocation, int screenTextSubrLocation, int minLevelTableLoc, int maxLevelTableLoc, byte noFutureLevel)
        {
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            if (enemyLevelSource == "player")
            {
                // generate level based off player levels
                // (determine max of the three)
                // LDA 7EE181 - boy level
                outRom[context.workingOffset++] = 0xAD;
                outRom[context.workingOffset++] = 0x81;
                outRom[context.workingOffset++] = 0xE1;
                // STA LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 8);
                // LDA 7EE381 - girl level
                outRom[context.workingOffset++] = 0xAD;
                outRom[context.workingOffset++] = 0x81;
                outRom[context.workingOffset++] = 0xE3;
                // CMP LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT
                outRom[context.workingOffset++] = 0xCD;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 8);
                // BCC 3 - skip STA if less than
                outRom[context.workingOffset++] = 0x90;
                outRom[context.workingOffset++] = 0x03;
                // STA LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 8);
                // LDA 7EE581 - sprite level
                outRom[context.workingOffset++] = 0xAD;
                outRom[context.workingOffset++] = 0x81;
                outRom[context.workingOffset++] = 0xE5;
                // CMP LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT
                outRom[context.workingOffset++] = 0xCD;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 8);
                // BCC 3 - skip STA if less than
                outRom[context.workingOffset++] = 0x90;
                outRom[context.workingOffset++] = 0x03;
                // STA LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 8);
                // (use that as the level)
                // LDA LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT
                outRom[context.workingOffset++] = 0xAD;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 8);
                // STA LEVELMATCH_CURRENT_MAX_LEVEL_8BIT
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_8BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_8BIT >> 8);
            }
            else if (enemyLevelSource == "bosses")
            {
                // this is kept as the desired level * 2, so we have a little finer control over it
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_BOSSES_KILLED;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_BOSSES_KILLED >> 8);
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_BOSSES_KILLED >> 16);
                // LSR - div 2
                outRom[context.workingOffset++] = 0x4A;
            }
            else if (enemyLevelSource == "future")
            {
                // LDA #noFutureLevel - 1
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = (byte)(noFutureLevel - 1);
            }
            else if (enemyLevelSource == "timed")
            {
                // load value at LEVELMATCH_TIMER_SECONDS_LOW/LEVELMATCH_TIMER_SECONDS_HIGH
                // divide by some value based on difficulty
                // LDA LEVELMATCH_TIMER_SECONDS_HIGH
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH >> 8);
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_HIGH >> 16);
                // XBA
                outRom[context.workingOffset++] = 0xEB;
                // LDA LEVELMATCH_TIMER_SECONDS_LOW
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW >> 8);
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_TIMER_SECONDS_LOW >> 16);
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // STA 004204
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x04;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0x00;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                byte divisor = (byte)timedDifficultyValue;
                // LDA #divisor
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = divisor;
                // STA 004206
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x06;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0x00;
                // eight nops to wait for the divide
                for (int i = 0; i < 8; i++)
                {
                    outRom[context.workingOffset++] = 0xEA;
                }
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // LDA 004214
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x14;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0x00;
                // shift right once so we top off at 127 instead of 255
                outRom[context.workingOffset++] = 0x4A;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
            }

            // if x is 0xe63 (mana beast), add a few levels here
            // CPX #0E63
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x63;
            outRom[context.workingOffset++] = 0x0E;
            // BNE over [3]
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x03;
            // CLC 
            outRom[context.workingOffset++] = 0x18;
            // ADC #05
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x0A;
            // check against 99 to not allow it to go over the max supported level in stat tables
            // CMP #63
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x63;
            // BLT over [2]
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;
            // LDA #62
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x62;
            // check against min/max for map if applicable
            // see MinMaxLevel for where these can optionally be set
            // (if min or max defined - table offset != 0)
            if(minLevelTableLoc != 0 || maxLevelTableLoc != 0)
            {
                // PHX
                outRom[context.workingOffset++] = 0xDA;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // PHA
                outRom[context.workingOffset++] = 0x48;
                // LDA 7E00DC (map number)
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0xDC;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x7E;
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // PLA
                outRom[context.workingOffset++] = 0x68;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // if min defined
                if (minLevelTableLoc != 0)
                {
                    // CMP $minTable,x
                    outRom[context.workingOffset++] = 0xDF;
                    outRom[context.workingOffset++] = (byte)minLevelTableLoc;
                    outRom[context.workingOffset++] = (byte)(minLevelTableLoc >> 8);
                    outRom[context.workingOffset++] = (byte)(minLevelTableLoc >> 16);
                    // BGE over (4)
                    outRom[context.workingOffset++] = 0xB0;
                    outRom[context.workingOffset++] = 0x04;
                    // LDA $minTable,x
                    outRom[context.workingOffset++] = 0xBF;
                    outRom[context.workingOffset++] = (byte)minLevelTableLoc;
                    outRom[context.workingOffset++] = (byte)(minLevelTableLoc >> 8);
                    outRom[context.workingOffset++] = (byte)(minLevelTableLoc >> 16);
                }
                // if max defined
                if (maxLevelTableLoc != 0)
                {
                    // CMP $maxTable,x
                    outRom[context.workingOffset++] = 0xDF;
                    outRom[context.workingOffset++] = (byte)maxLevelTableLoc;
                    outRom[context.workingOffset++] = (byte)(maxLevelTableLoc >> 8);
                    outRom[context.workingOffset++] = (byte)(maxLevelTableLoc >> 16);
                    // BLT over (4)
                    outRom[context.workingOffset++] = 0x90;
                    outRom[context.workingOffset++] = 0x04;
                    // LDA $maxTable,x
                    outRom[context.workingOffset++] = 0xBF;
                    outRom[context.workingOffset++] = (byte)maxLevelTableLoc;
                    outRom[context.workingOffset++] = (byte)(maxLevelTableLoc >> 8);
                    outRom[context.workingOffset++] = (byte)(maxLevelTableLoc >> 16);
                }
                // PLX
                outRom[context.workingOffset++] = 0xFA;
            }

            // STA OPENWORLD_CURRENT_ENEMY_LEVEL
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.OPENWORLD_CURRENT_ENEMY_LEVEL;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.OPENWORLD_CURRENT_ENEMY_LEVEL >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.OPENWORLD_CURRENT_ENEMY_LEVEL >> 16);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND 00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC 01,s to effectively multiply by 3
            outRom[context.workingOffset++] = 0x63;
            outRom[context.workingOffset++] = 0x01;
            // ASL - added because we added boss stats
            outRom[context.workingOffset++] = 0x0A;
            // STA LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 16);
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // STA LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT >> 16);
            // PLA - even out the stack
            outRom[context.workingOffset++] = 0x68;
        }

        private void writeStatLoader(byte[] outRom, RandoContext context, int classOffset, int statValuesOffset, bool sixteenBit, List<ushort> ramLocations)
        {
            // write the full stat loader for when an enemy is loaded

            // TYX
            outRom[context.workingOffset++] = 0xBB;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $E1E7,X - enemy id
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xE7;
            outRom[context.workingOffset++] = 0xE1;
            // AND 00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #classLocations[offset],X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)classOffset;
            outRom[context.workingOffset++] = (byte)(classOffset >> 8);
            outRom[context.workingOffset++] = (byte)((classOffset >> 16) + 0xC0);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND 00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;

            // if 16 bit stat, ASL and add to LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT
            // if 8 bit stat, add to LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT
            if (sixteenBit)
            {
                // ASL
                outRom[context.workingOffset++] = 0x0A;
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT
                outRom[context.workingOffset++] = 0x6F;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT >> 8);
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT >> 16);
            }
            else
            {
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT
                outRom[context.workingOffset++] = 0x6F;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 8);
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 16);
            }
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // now LDA valueTableLocations[offset],X for our value; sep 20 first if 8bit stat
            if (!sixteenBit)
            {
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
            }
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)statValuesOffset;
            outRom[context.workingOffset++] = (byte)(statValuesOffset >> 8);
            outRom[context.workingOffset++] = (byte)((statValuesOffset >> 16) + 0xC0);
            foreach (ushort ramLocation in ramLocations)
            {
                // STA in original spot,y
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = (byte)ramLocation;
                outRom[context.workingOffset++] = (byte)(ramLocation >> 8);
            }
        }

        private void writeStatLoaderSmall(byte[] outRom, RandoContext context, int classOffset, int statValuesOffset, bool sixteenBit, List<ushort> ramLocations)
        {
            // write the partial stat loader for when an enemy's statuses change

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // BF xx xx xx LDA #classLocations[offset],X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)classOffset;
            outRom[context.workingOffset++] = (byte)(classOffset >> 8);
            outRom[context.workingOffset++] = (byte)((classOffset >> 16) + 0xC0);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND 00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // if 16 bit stat, ASL and add to LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT
            // if 8 bit stat, add to LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT
            if (sixteenBit)
            {
                // ASL
                outRom[context.workingOffset++] = 0x0A;
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT
                outRom[context.workingOffset++] = 0x6D;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT >> 8);
            }
            else
            {
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT
                outRom[context.workingOffset++] = 0x6D;
                outRom[context.workingOffset++] = (byte)CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT;
                outRom[context.workingOffset++] = (byte)(CustomRamOffsets.LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT >> 8);
            }
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // now LDA valueTableLocations[offset],X for our value; sep 20 first if 8bit stat
            if (!sixteenBit)
            {
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
            }
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)statValuesOffset;
            outRom[context.workingOffset++] = (byte)(statValuesOffset >> 8);
            outRom[context.workingOffset++] = (byte)((statValuesOffset >> 16) + 0xC0);
            foreach (ushort ramLocation in ramLocations)
            {
                // STA in original spot,y
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = (byte)ramLocation;
                outRom[context.workingOffset++] = (byte)(ramLocation >> 8);
            }
            // PLX
            outRom[context.workingOffset++] = 0xFA;
        }

        private void writeLevelLoader(byte[] outRom, RandoContext context, List<ushort> ramLocations)
        {
            // load the current enemy level from a common spot we dump it regardless of mode

            // TYX
            outRom[context.workingOffset++] = 0xBB;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA OPENWORLD_CURRENT_ENEMY_LEVEL
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.OPENWORLD_CURRENT_ENEMY_LEVEL;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.OPENWORLD_CURRENT_ENEMY_LEVEL >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.OPENWORLD_CURRENT_ENEMY_LEVEL >> 16);
            foreach (ushort ramLocation in ramLocations)
            {
                // STA in original spot,y
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = (byte)ramLocation;
                outRom[context.workingOffset++] = (byte)(ramLocation >> 8);
            }
        }
    }
}
