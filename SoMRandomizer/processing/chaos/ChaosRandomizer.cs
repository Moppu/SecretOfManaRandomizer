using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.mapgen;
using SoMRandomizer.processing.common.structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.chaos
{
    /// <summary>
    /// Main randomization for chaos mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosRandomizer : RandoProcessor
    {
        public static Dictionary<string, int> BOSS_SETTING_VALUES = new Dictionary<string, int> { { "vfew", 1 }, { "few", 2 }, { "avg", 3 }, { "many", 4 }, { "vmany", 5 }, };
        public static Dictionary<string, int> MAPNUM_SETTING_VALUES = new Dictionary<string, int> { { "vfew", 20 }, { "few", 40 }, { "avg", 60 }, { "many", 80 }, { "vmany", 100 }, };
        public const string FLOOR_TABLE_LOCATION = "floorTableLocation";
        public const string DROP_TABLE_LOCATION = "dropTableLocation";
        protected override string getName()
        {
            return "Chaos mode map generation & randomization";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            int doorNum = 10;
            int newMapNum = 16;

            int floorNum = 2;
            List<int> bossFloors = new List<int>();
            int bossId = -1;

            Random r = context.randomFunctional;

            // populate stuff from the event generator
            List<int> shopEventIds = new List<int>();
            for(int i=0; i < ChaosEventMaker.NUM_SHOPS; i++)
            {
                shopEventIds.Add(context.workingData.getInt(ChaosEventMaker.SHOP_EVENT_ID_PREFIX + i));
            }

            List<int> bossFloorEvents = new List<int>();
            int numFloorEvents = context.workingData.getInt(ChaosEventMaker.BOSS_ENTRY_EVENT_NUM);
            for (int i = 0; i < numFloorEvents; i++)
            {
                bossFloorEvents.Add(context.workingData.getInt(ChaosEventMaker.BOSS_ENTRY_EVENT_ID_PREFIX + i));
            }
            List<int> bossDeathEvents = new List<int>();
            int numDeathEvents = context.workingData.getInt(ChaosEventMaker.BOSS_DEATH_EVENT_NUM);
            for (int i = 0; i < numDeathEvents; i++)
            {
                bossDeathEvents.Add(context.workingData.getInt(ChaosEventMaker.BOSS_DEATH_EVENT_ID_PREFIX + i));
            }
            Dictionary<int, int> elementEvents = new Dictionary<int, int>();
            for (int i=0; i < 8; i++)
            {
                elementEvents[ChaosMapGenerator.elementSprites[i]] = context.workingData.getInt(ChaosEventMaker.ELEMENT_EVENT_ID_PREFIX + i);
            }

            Dictionary<int, int> walkonEvents = new Dictionary<int, int>();
            walkonEvents[1] = context.workingData.getInt(ChaosEventMaker.INITIAL_WALKON_EVENT);

            int bossAmount = BOSS_SETTING_VALUES[settings.get(ChaosSettings.PROPERTYNAME_NUM_BOSSES)];
            int numFloors = MAPNUM_SETTING_VALUES[settings.get(ChaosSettings.PROPERTYNAME_NUM_FLOORS)];

            int manaBeastFloorEvent = context.workingData.getInt(ChaosEventMaker.MANABEAST_WALKON_EVENT);
            int manaBeastDeathEvent = context.workingData.getInt(ChaosEventMaker.MANABEAST_DEATH_EVENT);
            bossFloorEvents.Add(manaBeastFloorEvent);
            bossDeathEvents.Add(manaBeastDeathEvent);
            List<int> seedEvents = new List<int>();
            for(int i=0; i < 8; i++)
            {
                seedEvents.Add(context.workingData.getInt(ChaosEventMaker.SEED_EVENT_ID_PREFIX + i));
            }
            
            // first door .. 31,58 on map 128
            Door startDoor = new Door();
            startDoor.setTargetMap((ushort)newMapNum);
            startDoor.setXpos(31);
            startDoor.setYpos(58);
            startDoor.setTransitionType(0);
            context.replacementDoors[doorNum++] = startDoor;

            Dictionary<int, int> floorMapping = new Dictionary<int, int>();
            floorMapping[newMapNum] = 0;

            // generated ids of things
            ChaosGenerationState chaosState = new ChaosGenerationState();
            chaosState.fullMapIndex = newMapNum;
            chaosState.doorIndex = doorNum;
            // info for next floor
            ChaosMapGenerationInfo initialMapGenInfo = new ChaosMapGenerationInfo();
            initialMapGenInfo.walkonEventId = walkonEvents[1];
            initialMapGenInfo.doorIndexToNextFloor = 0; // only one way out of this map
            initialMapGenInfo.map = ChaosVanillaMaps.startMap;
            initialMapGenInfo.mainRoomMapId = chaosState.fullMapIndex;
            initialMapGenInfo.nextFloorDoorId = chaosState.doorIndex;

            // first floor - standard waterfall map with the sword and such
            Logging.log("Making start floor", "debug");
            ChaosMapGenerator.generateFloor(context, settings, initialMapGenInfo, chaosState);

            chaosState.previousFloorData = initialMapGenInfo;
            ChaosVanillaMap prevMap = ChaosVanillaMaps.startMap;

            List<int> townFloors = new List<int>();
            List<int> elementFloors = new List<int>();
            List<int> elements = new List<int>();
            List<int> elementsLeft = new List<int>();

            // und gno syl sal lum sha lun dry
            int[] elementWeights = new int[] { 10, 1, 1, 1, 1, 1, 1, 2 };
            if (!settings.getBool(ChaosSettings.PROPERTYNAME_PRIORITIZE_HEAL_SPELLS))
            {
                elementWeights = new int[] { 1, 1, 1, 1, 1, 1, 1, 1 };
            }
            for (int i = 0; i < 8; i++)
            {
                elementsLeft.Add(i);
            }

            List<int> manaSeedFloors = new List<int>();
            List<int> seeds = new List<int>();
            List<int> seedsLeft = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                seedsLeft.Add(i);
            }

            int numBosses = (int)(bossAmount * 5 * numFloors / 100.0);
            if (numBosses > 25)
            {
                numBosses = 25;
            }
            int attempts = 0;
            int bossFrequency = 7 - bossAmount;
            int bossStartMultiplier = 1;
            if (settings.getBool(ChaosSettings.PROPERTYNAME_SAFER_EARLY_FLOORS))
            {
                bossStartMultiplier = 2;
            }

            // determine which floors have bosses
            if (bossAmount > 1)
            {
                for (int i = 0; i < numBosses; i++)
                {
                    int floor = bossFrequency * bossStartMultiplier + (r.Next() % (numFloors - 2));
                    bool floorAvail = true;
                    foreach (int fl in bossFloors)
                    {
                        if (Math.Abs(fl - floor) < bossFrequency)
                        {
                            floorAvail = false;
                            break;
                        }
                    }

                    attempts++;
                    if (floorAvail)
                    {
                        bossFloors.Add(floor);
                        Logging.log("Boss on floor " + floor, "debug");
                    }
                    else
                    {
                        i--;
                    }

                    if (attempts >= 1000)
                    {
                        break;
                    }
                }
            }
            else
            {
                Logging.log("No bosses.", "debug");
            }

            // determine which floors are towns
            int townMapNum = floorNum;
            while (townMapNum < numFloors)
            {
                townMapNum += (r.Next() % 4) + 4;
                townFloors.Add(townMapNum);
            }

            // determine which floors have elements
            attempts = 0;
            while (elementFloors.Count < 8 && attempts < 100)
            {
                // don't stick em on first or last floor
                int floor = 2 + r.Next() % (numFloors - 3);
                if (!elementFloors.Contains(floor))
                {
                    elementFloors.Add(floor);
                    List<int> weightedElements = new List<int>();
                    foreach (int eleLeft in elementsLeft)
                    {
                        for (int i = 0; i < elementWeights[eleLeft]; i++)
                        {
                            weightedElements.Add(eleLeft);
                        }
                    }
                    int id = weightedElements[r.Next() % weightedElements.Count];
                    elements.Add(id);
                    elementsLeft.Remove(id);
                }
                attempts++;
            }

            // determine which floors have mana seeds
            List<int> seedEventsInOrder = new List<int>();
            attempts = 0;
            while (manaSeedFloors.Count < 8 && attempts < 100)
            {
                // don't stick em on first or last floor
                int floor = 2 + r.Next() % (numFloors - 3);
                if (!manaSeedFloors.Contains(floor))
                {
                    manaSeedFloors.Add(floor);
                    int id = r.Next() % seedsLeft.Count;
                    seeds.Add(seedsLeft[id]);
                    seedEventsInOrder.Add(seedEvents[seedsLeft[id]]);
                    seedsLeft.RemoveAt(id);
                }
                attempts++;
            }

            List<ChaosVanillaMap> usedMaps = new List<ChaosVanillaMap>();
            int elementId = 0;
            int seedId = 0;
            bossId = 0;

            List<int> possibleBosses = new List<int>();
            possibleBosses.AddRange(ChaosVanillaMaps.bossMaps.Keys);

            // generate all floors after the start floor
            while (floorNum < numFloors)
            {
                Logging.log("Generating floor " + floorNum, "debug");
                // info for next floor
                ChaosMapGenerationInfo mapGenerationInfo = new ChaosMapGenerationInfo();

                // determine number of exits needed on the chosen vanilla map 
                int exitNum = 2; // one to go back, one to go forward
                if (elementFloors.Contains(floorNum))
                {
                    // plus one for a side room with an element
                    exitNum++;
                }
                if (manaSeedFloors.Contains(floorNum))
                {
                    // plus one for a side room with a mana seed
                    exitNum++;
                }
                
                if (bossFloors.Contains(floorNum) || floorNum == numFloors - 1)
                {
                    if(floorNum == numFloors - 1)
                    {
                        mapGenerationInfo.bossEventId = manaBeastFloorEvent;
                        mapGenerationInfo.bossDeathEventId = manaBeastDeathEvent;
                        mapGenerationInfo.bossMap = ChaosVanillaMaps.finalBossMap;
                        mapGenerationInfo.bossId = 0x7F;
                    }
                    else
                    {
                        mapGenerationInfo.bossEventId = bossFloorEvents[bossId];
                        mapGenerationInfo.bossDeathEventId = bossDeathEvents[bossId];
                        int bossIndex = r.Next() % possibleBosses.Count;
                        int bossNum = possibleBosses[bossIndex];
                        possibleBosses.RemoveAt(bossIndex);
                        if (possibleBosses.Count == 0)
                        {
                            // reset boss possibilities
                            possibleBosses.AddRange(ChaosVanillaMaps.bossMaps.Keys);
                        }
                        mapGenerationInfo.bossMap = ChaosVanillaMaps.bossMaps[bossNum];
                        mapGenerationInfo.bossId = (byte)bossNum;
                        bossId++;
                    }
                    Logging.log("Boss on floor " + floorNum, "spoiler");
                }

                // pick a base map for this floor, based on the number of exits. 2 base exits (forward and back) plus one for mana seed or elemental if it appears on this floor.
                List<ChaosVanillaMap> eligibleMaps = townFloors.Contains(floorNum) ? ChaosVanillaMaps.townMapsByExitNum[exitNum] : ChaosVanillaMaps.chaosMapsByExitNum[exitNum];
                ChaosVanillaMap map = eligibleMaps[r.Next() % eligibleMaps.Count];
                if (usedMaps.Contains(map))
                {
                    // reroll once
                    map = eligibleMaps[r.Next() % eligibleMaps.Count];
                }

                // reroll forever to prevent the same one twice in a row, because that's confusing as fuck
                while (map == prevMap)
                {
                    map = eligibleMaps[r.Next() % eligibleMaps.Count];
                }

                usedMaps.Add(map);

                List<int> newMapDoorIndexes = new List<int>();
                for (int i = 0; i < exitNum; i++)
                {
                    newMapDoorIndexes.Add(i);
                }

                // determine which doors on the chosen map will go where

                // pointing back to prev floor
                int backFloorExitNum = newMapDoorIndexes[r.Next() % newMapDoorIndexes.Count];
                newMapDoorIndexes.Remove(backFloorExitNum);

                // next floor (or boss)
                int nextFloorExitNum = newMapDoorIndexes[r.Next() % newMapDoorIndexes.Count];
                newMapDoorIndexes.Remove(nextFloorExitNum);

                mapGenerationInfo.doorIndexToNextFloor = nextFloorExitNum;
                mapGenerationInfo.doorIndexToLastFloor = backFloorExitNum;

                // door to element room, if applicable
                if (elementFloors.Contains(floorNum))
                {
                    mapGenerationInfo.doorIndexToElementRoom = newMapDoorIndexes[r.Next() % newMapDoorIndexes.Count];
                    newMapDoorIndexes.Remove((int)mapGenerationInfo.doorIndexToElementRoom);
                    mapGenerationInfo.elementIndex = elements[elementId];
                    mapGenerationInfo.elementEventIndex = elementEvents[elementId + 0x90];
                    elementId++;
                    Logging.log("Element on floor " + floorNum, "spoiler");
                }

                // door to mana seed room, if applicable
                if (manaSeedFloors.Contains(floorNum))
                {
                    mapGenerationInfo.doorIndexToManaSeedRoom = newMapDoorIndexes[r.Next() % newMapDoorIndexes.Count];
                    newMapDoorIndexes.Remove((int)mapGenerationInfo.doorIndexToManaSeedRoom);
                    mapGenerationInfo.seedIndex = seeds[seedId];
                    mapGenerationInfo.seedEventIndex = seedEventsInOrder[seedId];
                    seedId++;
                    Logging.log("Seed on floor " + floorNum, "spoiler");
                }

                // set walkon event, if applicable
                if (walkonEvents.ContainsKey(floorNum))
                {
                    mapGenerationInfo.walkonEventId = walkonEvents[floorNum];
                }

                mapGenerationInfo.townMap = townFloors.Contains(floorNum);

                int shopId = (floorNum * 16) / numFloors;
                if (shopId >= 16)
                {
                    shopId = 15;
                }
                mapGenerationInfo.nekoEventNum = shopEventIds[shopId];

                mapGenerationInfo.map = map;
                mapGenerationInfo.mainRoomMapId = chaosState.fullMapIndex;
                mapGenerationInfo.nextFloorDoorId = chaosState.doorIndex;
                int floorFirstMapIndex = chaosState.fullMapIndex;

                // generate the floor's maps
                Logging.log("Making floor " + floorNum, "debug");
                ChaosMapGenerator.generateFloor(context, settings, mapGenerationInfo, chaosState);

                int floorLastMapIndex = chaosState.fullMapIndex - 1;

                // set the floor number for each map that was generated
                for (int i = floorFirstMapIndex; i <= floorLastMapIndex; i++)
                {
                    floorMapping[i] = floorNum;
                }

                chaosState.previousFloorData = mapGenerationInfo;
                floorNum++;
            } // end floor loop

            // make the credits map
            int creditsNewMapNum = chaosState.fullMapIndex++;
            CreditsMap.makeCreditsMap(origRom, context, creditsNewMapNum);
            Door creditsMapDoor = new Door();
            creditsMapDoor.setTargetMap((ushort)creditsNewMapNum);
            creditsMapDoor.setXpos((byte)ChaosVanillaMaps.creditsMap.doors[0].warpBackX);
            creditsMapDoor.setYpos((byte)ChaosVanillaMaps.creditsMap.doors[0].warpBackY);
            creditsMapDoor.setTransitionType(ChaosMapGenerator.getEnterExitByte(ChaosVanillaMaps.creditsMap.doors[0]));
            int creditsMapDoorId = chaosState.doorIndex;
            Logging.log("  Door to credits map = " + creditsMapDoorId.ToString("X"), "debug");
            context.replacementDoors[chaosState.doorIndex++] = creditsMapDoor;

            // inject credits door into mana beast death event
            Logging.log("  Injecting credits door into mana beast death event = " + manaBeastDeathEvent.ToString("X4"), "debug");
            VanillaEventUtil.replaceEventData(ChaosEventMaker.NEXT_FLOOR_DOOR_INJECTION_PATTERN.ToList(), context.replacementEvents[manaBeastDeathEvent], new byte[] { 0x18, (byte)creditsMapDoorId, (byte)((creditsMapDoorId >> 8) + 8) }.ToList());
            
            int floorTableLocation = context.workingOffset;
            context.workingData.setInt(FLOOR_TABLE_LOCATION, floorTableLocation);
            // write floor mapping to rom
            for (int i = 0; i < 512; i++)
            {
                if (floorMapping.ContainsKey(i))
                {
                    outRom[context.workingOffset++] = (byte)floorMapping[i];
                }
                else
                {
                    outRom[context.workingOffset++] = 0;
                }
            }

            int dropTableLocation = context.workingOffset;
            context.workingData.setInt(DROP_TABLE_LOCATION, dropTableLocation);
            // write floor mapping scaled down for drop index to rom
            for (int i = 0; i < 512; i++)
            {
                if (floorMapping.ContainsKey(i))
                {
                    int num = (int)((floorMapping[i] * 32.0) / numFloors);
                    if (num >= 32)
                    {
                        num = 31;
                    }
                    outRom[context.workingOffset++] = (byte)num;
                }
                else
                {
                    outRom[context.workingOffset++] = 0;
                }
            }
            return true;
        }
    }
}
