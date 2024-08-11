using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.hacks.openworld;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Swap enemies by species, or make them all the same enemy for "oops all owls"
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EnemySwaps : RandoProcessor
    {
        protected override string getName()
        {
            return "Non-boss enemy randomizer";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;
            string statusConditionType = "";
            int fixedEnemy = -1;
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            bool doEnemySwaps = false;

            if (randoMode == VanillaRandoSettings.MODE_KEY)
            {
                statusConditionType = settings.get(VanillaRandoSettings.PROPERTYNAME_STATUS_AILMENTS);
                doEnemySwaps = settings.getBool(VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_ENEMIES);
            }
            else if (randoMode == OpenWorldSettings.MODE_KEY)
            {
                statusConditionType = settings.get(OpenWorldSettings.PROPERTYNAME_STATUS_AILMENTS);
                string enemyRandoType = settings.get(OpenWorldSettings.PROPERTYNAME_RANDOMIZE_ENEMIES);
                if (enemyRandoType.Contains("oops"))
                {
                    fixedEnemy = settings.getInt(OpenWorldSettings.PROPERTYNAME_EVERY_ENEMY);
                }
                doEnemySwaps = enemyRandoType.Contains("swap") || fixedEnemy != -1;
                if(enemyRandoType.Contains("none"))
                {
                    Logging.log("Skipping enemy rando; no enemy setting selected");
                    new NoEnemies().add(origRom, outRom, seed, settings, context);
                    return false;
                }
                else if(enemyRandoType.Contains("spawns"))
                {
                    Logging.log("Skipping enemy rando; random spawns selected");
                    new EnemyRandomizer().add(origRom, outRom, seed, settings, context);
                    return false;
                }
            }
            else
            {
                Logging.log("Unsupported mode for enemy rando");
                return false;
            }

            if(!doEnemySwaps)
            {
                return false;
            }

            // note +4F15 for AI offsets

            Dictionary<int, int[]> summonedLocations = new Dictionary<int, int[]>();
            // map by summoner, and find something new lower level but near level for him to summon
            summonedLocations[34] = new int[] { 0x106D2F }; // zombie, summoned by tomato man
            summonedLocations[64] = new int[] { 0x108A58, 0x108A4E, 0x108A50 }; // needlion + rabites, summoned by eggplant man
            summonedLocations[19] = new int[] { 0x1060FC }; // pebbler, summoned by kimono bird
            summonedLocations[47] = new int[] { 0x1078F5 }; // metal crawler, summoned by kimono wizard
            summonedLocations[74] = new int[] { 0x1096A9, 0x1096A5, 0x1096B1, 0x1096B5, }; // wolf things, summoned by heck hound

            // eye spy
            // D0/5563:	ED 03				[Summon Monster: 03]
            summonedLocations[7] = new int[] { 0x105564 };

            // wizard eye
            // D0/6786:	ED331F  	SBC $1F33
            summonedLocations[26] = new int[] { 0x106787 };

            // goblin
            // D0/580E:	07ED    	ORA [$ED]
            // D0 / 5810:	0EE004 ASL $04E0
            // D0/58D2:	07ED    	ORA [$ED]
            // D0 / 58D4:	0EE004 ASL $04E0
            summonedLocations[11] = new int[] { 0x105810, 0x1058D4 };

            // basilisk
            // D0/8FAB:	ED281F  	SBC $1F28
            summonedLocations[69] = new int[] { 0x108FAC };

            // mad mallard
            // D0/7024:	ED 17				[Summon Monster: 17]
            // D0/705D:	85ED    	STA $ED
            // D0 / 705F:	17E3        ORA[$E3],Y
            // D0/706B:	85ED    	STA $ED
            // D0 / 706D:	17E6        ORA[$E6],Y
            summonedLocations[37] = new int[] { 0x107025, 0x10705F, 0x10706D };

            // fierce head
            // D0/8496:	04ED    	TSB $ED
            // D0 / 8498:	1E1F01      ASL $011F,X
            summonedLocations[58] = new int[] { 0x108498 };

            // nitro pumpkin transform
            // D0/8C2D:	01ED    	ORA ($ED,X)
            // D0 / 8C2F: 17EF        ORA[$EF],Y
            summonedLocations[66] = new int[] { 0x108C2F };

            // fiend head -> dark stalker
            // D0 / 9816:	04ED        TSB $ED
            // D0 / 9818:	4D1F01      EOR $011F
            summonedLocations[75] = new int[] { 0x109818 };

            // book enemies .. national scar [76] makes imps, mystic book [35] makes something too?  maybe it doesn't
            summonedLocations[76] = new int[] { 0x10999C };

            // shape shifter - 4D27 = 9C3C .. made red wolves? 0x35
            summonedLocations[79] = new int[] { 0x109CF3, 0x109CFB, 0x109D03, 0x109D0B, 0x109D10 };

            int[] unhittableEnemies = new int[] {
                9, // spectre
                15, // dark funk
                29, // LA funk
                48, // ghost
            };

            int[] noSwapEnemies = new int[]
            {
                32, // sprite boss
                46, // boy boss
                63, // girl boss
                50, // spider legs
            };

            int[] noSummonEnemies = new int[]
            {
                // map 419-426
                0x51, // doom sword
                // upper land orb
                0x10, // caterpillar
                // fire palace interior orb
                0x1C, // robin foot
                // luna palace orb
                0x39, // marmablue
                // matango cave orb
                0x16, // pebbler
                // jumpy thing on map 432
                0x47, // steelpion
            };
            Dictionary<int, int> enemyIndexes = new Dictionary<int, int>();
            Dictionary<int, int> enemyIndexesUnhittable = new Dictionary<int, int>();
            List<int> enemySourceIds = new List<int>();
            List<int> enemyDestIdsLeft = new List<int>();
            List<int> enemySourceIdsUnhittable = new List<int>();
            List<int> enemyDestIdsLeftUnhittable = new List<int>();

            List<int> allSummoners = new List<int>();
            foreach(int key in summonedLocations.Keys)
            {
                allSummoners.Add(key);
            }
            // stuff that dupes but doesn't summon
            allSummoners.Add(0x08); // green drop
            allSummoners.Add(0x27); // red drop
            allSummoners.Add(0x38); // blue drop
            allSummoners.Add(0x44); // tsunami
            allSummoners.Add(0x26); // emberman

            for (int i = 0; i < 84; i++)
            {
                if(!noSwapEnemies.Contains(i))
                {
                    if (!unhittableEnemies.Contains(i))
                    {
                        enemySourceIds.Add(i);
                        enemyDestIdsLeft.Add(i);
                    }
                    else
                    {
                        enemySourceIdsUnhittable.Add(i);
                        enemyDestIdsLeftUnhittable.Add(i);
                    }
                }
            }

            // split into two loops to prioritize randomizing the restricted guys first and make sure they choose a non-summoner
            // there was an issue where 0x51 (doom sword) had no choice left but to be a red drop
            foreach (int key in enemySourceIds)
            {
                if (noSummonEnemies.Contains(key))
                {
                    int destId = r.Next() % enemyDestIdsLeft.Count;
                    int iters = 0;
                    while (allSummoners.Contains(enemyDestIdsLeft[destId]) && iters < 100)
                    {
                        destId = r.Next() % enemyDestIdsLeft.Count;
                        iters++;
                    }
                    // key is original enemy, dest is new graphic
                    if (fixedEnemy == -1)
                    {
                        enemyIndexes[key] = enemyDestIdsLeft[destId];
                    }
                    else
                    {
                        enemyIndexes[key] = fixedEnemy;
                    }
                    string srcEnemyName = VanillaEventUtil.getVanillaThingName(origRom, 0x8CF + key);
                    string dstEnemyName = VanillaEventUtil.getVanillaThingName(origRom, 0x8CF + enemyDestIdsLeft[destId]);
                    Logging.log("enemy " + key.ToString("X") + " (" + srcEnemyName + ") -> " + enemyDestIdsLeft[destId].ToString("X") + " (" + dstEnemyName + ")", "spoiler");
                    enemyDestIdsLeft.RemoveAt(destId);
                }
            }

            foreach (int key in enemySourceIds)
            {
                if (!noSummonEnemies.Contains(key))
                {
                    int destId = r.Next() % enemyDestIdsLeft.Count;
                    // key is original enemy, dest is new graphic
                    if (fixedEnemy == -1)
                    {
                        enemyIndexes[key] = enemyDestIdsLeft[destId];
                    }
                    else
                    {
                        enemyIndexes[key] = fixedEnemy;
                    }
                    string srcEnemyName = VanillaEventUtil.getVanillaThingName(origRom, 0x8CF + key);
                    string dstEnemyName = VanillaEventUtil.getVanillaThingName(origRom, 0x8CF + enemyDestIdsLeft[destId]);
                    Logging.log("enemy " + key.ToString("X") + " (" + srcEnemyName + ") -> " + enemyDestIdsLeft[destId].ToString("X") + " (" + dstEnemyName + ")", "spoiler");
                    enemyDestIdsLeft.RemoveAt(destId);
                }
            }

            foreach (int key in enemySourceIdsUnhittable)
            {
                int destId = r.Next() % enemyDestIdsLeftUnhittable.Count;
                enemyIndexesUnhittable[key] = enemyDestIdsLeftUnhittable[destId];
                enemyDestIdsLeftUnhittable.RemoveAt(destId);
            }

            for (int mapNum = 16; mapNum < 500; mapNum++)
            {
                // [5] is enemy type
                int objsOffset1 = 0x80000 + outRom[0x87000 + mapNum * 2] + (outRom[0x87000 + mapNum * 2 + 1] << 8);
                int objsOffset2 = 0x80000 + outRom[0x87000 + mapNum * 2 + 2] + (outRom[0x87000 + mapNum * 2 + 3] << 8);
                objsOffset1 += 8; // skip map header
                while (objsOffset1 < objsOffset2)
                {
                    int objType = origRom[objsOffset1 + 5];
                    if (enemyIndexes.ContainsKey(objType))
                    {
                        outRom[objsOffset1 + 5] = (byte)enemyIndexes[objType];
                    }
                    if (enemyIndexesUnhittable.ContainsKey(objType))
                    {
                        outRom[objsOffset1 + 5] = (byte)enemyIndexesUnhittable[objType];
                    }
                    objsOffset1 += 8;
                }
            }

            // swap stats
            foreach (int oldEnemyNum in enemyIndexes.Keys)
            {
                int newEnemyNum = enemyIndexes[oldEnemyNum];
                int[] eightBitSwapStats = new int[] { 0, 11, 26, 3, 4, 5, 6, 7, 8, 18, 19, 25 };
                // 23,24 are weapon ids
                int[] sixteenBitSwapStats = new int[] { 1, 16, 27, 9, 12 };
                foreach (int eightSwapIndex in eightBitSwapStats)
                {
                    byte newValue = origRom[0x101C00 + oldEnemyNum * 29 + eightSwapIndex];
                    outRom[0x101C00 + newEnemyNum * 29 + eightSwapIndex] = newValue;
                }
                foreach (int sixteenSwapIndex in sixteenBitSwapStats)
                {
                    outRom[0x101C00 + newEnemyNum * 29 + sixteenSwapIndex] = origRom[0x101C00 + oldEnemyNum * 29 + sixteenSwapIndex];
                    outRom[0x101C00 + newEnemyNum * 29 + sixteenSwapIndex + 1] = origRom[0x101C00 + oldEnemyNum * 29 + sixteenSwapIndex + 1];
                }
            }
            foreach (int oldEnemyNum in enemyIndexesUnhittable.Keys)
            {
                int newEnemyNum = enemyIndexesUnhittable[oldEnemyNum];
                int[] eightBitSwapStats = new int[] { 0, 11, 26, 3, 4, 5, 6, 7, 8, 18, 19, 25 };
                // 23,24 are weapon ids
                int[] sixteenBitSwapStats = new int[] { 1, 16, 27, 9, 12 };
                foreach (int eightSwapIndex in eightBitSwapStats)
                {
                    byte newValue = origRom[0x101C00 + oldEnemyNum * 29 + eightSwapIndex];
                    outRom[0x101C00 + newEnemyNum * 29 + eightSwapIndex] = newValue;
                }
                foreach (int sixteenSwapIndex in sixteenBitSwapStats)
                {
                    outRom[0x101C00 + newEnemyNum * 29 + sixteenSwapIndex] = origRom[0x101C00 + oldEnemyNum * 29 + sixteenSwapIndex];
                    outRom[0x101C00 + newEnemyNum * 29 + sixteenSwapIndex + 1] = origRom[0x101C00 + oldEnemyNum * 29 + sixteenSwapIndex + 1];
                }
            }

            List<int> statusTypeList = new List<int>();

            if (statusConditionType.StartsWith("random"))
            {
                // 83 total enemies, we'll call it 90 to account for some overlap
                int totalEnemies = 90; // decrease this for harder mode
                int multiplier = 10;
                Dictionary<int, double> numOrigStatuses = new Dictionary<int, double>();
                numOrigStatuses[0x0080] = 0.5; // confuse
                numOrigStatuses[0x0040] = 2.0; // stone
                numOrigStatuses[0x0020] = 3.0; // snowman
                numOrigStatuses[0x0010] = 4.0; // sleep
                numOrigStatuses[0x4000] = 5.0; // engulf
                numOrigStatuses[0x2000] = 12.0; // poison
                numOrigStatuses[0x1000] = 4.0; // moogle
                numOrigStatuses[0x0200] = 4.0; // shrink
                numOrigStatuses[0x0100] = 0.5; // balloon
                if (statusConditionType.Contains("annoying"))
                {
                    totalEnemies = 45;
                }
                if (statusConditionType.Contains("awful"))
                {
                    Dictionary<int, double> numOrigStatusesCopy = new Dictionary<int, double>();
                    totalEnemies = 0;
                    // equal chance of everything
                    foreach(int key in numOrigStatuses.Keys)
                    {
                        numOrigStatusesCopy[key] = 1;
                    }
                    numOrigStatuses = numOrigStatusesCopy;
                }

                foreach (int key in numOrigStatuses.Keys)
                {
                    int scaledNum = (int)(multiplier * numOrigStatuses[key]);
                    for (int i = 0; i < scaledNum; i++)
                    {
                        statusTypeList.Add(key);
                    }
                }

                // fill the rest with no status
                for (int i = statusTypeList.Count; i < totalEnemies * multiplier; i++)
                {
                    statusTypeList.Add(0);
                }

                // now, for every enemy, just pick a random element out of statusTypeList
            }

            // enemy weapon stat swaps
            foreach (int oldEnemyNum in enemyIndexes.Keys)
            {
                int newEnemyNum = enemyIndexes[oldEnemyNum];

                // 23,24 are weapon ids
                byte oldW1 = origRom[0x101C00 + oldEnemyNum * 29 + 23];
                byte oldW2 = origRom[0x101C00 + oldEnemyNum * 29 + 24];

                byte newW1 = origRom[0x101C00 + newEnemyNum * 29 + 23];
                byte newW2 = origRom[0x101C00 + newEnemyNum * 29 + 24];

                // weapon power swap for new enemy in the old enemy's spot
                // note 9,10 are the status condition caused by the weapon
                // 09:
                // - 0x80 confuse            (0 enemies use)
                // - 0x40 petrify            (2 enemies use)
                // - 0x20 snowman            (3 enemies use)
                // - 0x10 stun (sleep)       (4 enemies use)
                // - 0x08 ?
                // - 0x04 slow               (0 enemies use)
                // - 0x02 ?
                // - 0x01 ?
                // 0a:
                // - 0x80 death (unusable)
                // - 0x40 engulf             (5 enemies use)
                // - 0x20 poison             (12 enemies use)
                // - 0x10 moogle             (4 enemies use)
                // - 0x08 transform?
                // - 0x04 barrel?
                // - 0x02 shrink             (4 enemies use)
                // - 0x01 balloon            (0 enemies use)

                byte[] indexesToSwap = statusConditionType == "location" ? new byte[] { 1, 2, 4, 5, 6, 7, 8, 9, 10, 11 } : new byte[] { 1, 2, 4, 5, 6, 7, 8, 11 };

                foreach (int i in indexesToSwap)
                {
                    outRom[0x101000 + newW1 * 12 + i] = origRom[0x101000 + oldW1 * 12 + i];
                    outRom[0x101000 + newW2 * 12 + i] = origRom[0x101000 + oldW2 * 12 + i];
                }

                if(statusConditionType.StartsWith("random"))
                {
                    // for every enemy, just pick a random element out of statusTypeList
                    int randomStatus = statusTypeList[(r.Next() % statusTypeList.Count)];
                    outRom[0x101000 + newW1 * 12 + 9] = (byte)(randomStatus);
                    outRom[0x101000 + newW2 * 12 + 9] = (byte)(randomStatus);
                    outRom[0x101000 + newW1 * 12 + 10] = (byte)(randomStatus>>8);
                    outRom[0x101000 + newW2 * 12 + 10] = (byte)(randomStatus>>8);
                }
            }
            foreach (int oldEnemyNum in enemyIndexesUnhittable.Keys)
            {
                int newEnemyNum = enemyIndexesUnhittable[oldEnemyNum];

                // 23,24 are weapon ids
                byte oldW1 = origRom[0x101C00 + oldEnemyNum * 29 + 23];
                byte oldW2 = origRom[0x101C00 + oldEnemyNum * 29 + 24];

                byte newW1 = origRom[0x101C00 + newEnemyNum * 29 + 23];
                byte newW2 = origRom[0x101C00 + newEnemyNum * 29 + 24];

                // weapon power swap for new enemy in the old enemy's spot
                byte[] indexesToSwap = statusConditionType == "location" ? new byte[] { 1, 2, 4, 5, 6, 7, 8, 9, 10, 11 } : new byte[] { 1, 2, 4, 5, 6, 7, 8, 11 };

                foreach (int i in indexesToSwap)
                {
                    outRom[0x101000 + newW1 * 12 + i] = origRom[0x101000 + oldW1 * 12 + i];
                    outRom[0x101000 + newW2 * 12 + i] = origRom[0x101000 + oldW2 * 12 + i];
                }

                if (statusConditionType.StartsWith("random"))
                {
                    // for every enemy, just pick a random element out of statusTypeList
                    int randomStatus = statusTypeList[(r.Next() % statusTypeList.Count)];
                    outRom[0x101000 + newW1 * 12 + 9] = (byte)(randomStatus);
                    outRom[0x101000 + newW2 * 12 + 9] = (byte)(randomStatus);
                    outRom[0x101000 + newW1 * 12 + 10] = (byte)(randomStatus >> 8);
                    outRom[0x101000 + newW2 * 12 + 10] = (byte)(randomStatus >> 8);
                }
            }

            foreach (int oldEnemyNum in enemyIndexes.Keys)
            {
                int newEnemyNum = enemyIndexes[oldEnemyNum];

                // 103A50, 5 bytes
                // drop swaps
                for (int i = 1; i < 5; i++)
                {
                    outRom[0x103A50 + newEnemyNum * 5 + i] = origRom[0x103A50 + oldEnemyNum * 5 + i];
                }
            }
            foreach (int oldEnemyNum in enemyIndexesUnhittable.Keys)
            {
                int newEnemyNum = enemyIndexesUnhittable[oldEnemyNum];

                // 103A50, 5 bytes
                // drop swaps
                for (int i = 1; i < 5; i++)
                {
                    outRom[0x103A50 + newEnemyNum * 5 + i] = origRom[0x103A50 + oldEnemyNum * 5 + i];
                }
            }

            foreach(int summoner in summonedLocations.Keys)
            {
                int[] locations = summonedLocations[summoner];
                // get its (new) level - [0]
                byte newLevel = outRom[0x101C00 + summoner * 29 + 0];
                List<int> enemiesUsed = new List<int>();
                for (int i = 0; i < locations.Length; i++)
                {
                    int location = locations[i];
                    int id = -1;
                    int lev = -1;
                    foreach (int enemyNum in enemyIndexes.Keys)
                    {
                        // find similar leveled stuff to summon
                        if (!enemiesUsed.Contains(enemyNum))
                        {
                            byte thisLevel = outRom[0x101C00 + enemyNum * 29 + 0];
                            if (thisLevel < newLevel)
                            {
                                if (id == -1 || (newLevel - thisLevel < newLevel - lev))
                                {
                                    lev = thisLevel;
                                    id = enemyNum;
                                }
                            }
                        }
                    }
                    if(id != -1)
                    {
                        outRom[location] = (byte)id;
                        enemiesUsed.Add(id);
                    }
                    else
                    {
                        // summon itself if we found nothing
                        outRom[location] = (byte)summoner;
                    }
                }
            }

            return true;
        }
    }
}
