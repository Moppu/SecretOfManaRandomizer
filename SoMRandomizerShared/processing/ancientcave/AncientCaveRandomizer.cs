using SoMRandomizer.config.settings;
using SoMRandomizer.processing.ancientcave.mapgen;
using SoMRandomizer.processing.ancientcave.mapgen.cave;
using SoMRandomizer.processing.ancientcave.mapgen.forest;
using SoMRandomizer.processing.ancientcave.mapgen.island;
using SoMRandomizer.processing.ancientcave.mapgen.manafortint;
using SoMRandomizer.processing.ancientcave.mapgen.ruins;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.mapgen;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.procgen;
using SoMRandomizer.processing.hacks.common.util;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.ancientcave
{
    /// <summary>
    /// Main randomization for ancient cave mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AncientCaveRandomizer : RandoProcessor
    {
        private Dictionary<string, AncientCaveMapGenerator> mapGenerators = new Dictionary<string, AncientCaveMapGenerator>();
        // an event pattern to be replaced by a 5-byte music play pattern. if not replaced, this just sets and unsets event flag FB
        public static List<byte> MUSIC_REPLACEMENT_PATTERN = new byte[] { 0x29, 0xFB, 0x30, 0xFB, 0x00 }.ToList();
        private static Comparer<XyPos> locationComparison = Comparer<XyPos>.Create((pos1, pos2) =>
        {
            return pos1.ypos != pos2.ypos ? pos1.ypos.CompareTo(pos2.ypos) : pos1.xpos.CompareTo(pos2.xpos);
        });

        public AncientCaveRandomizer()
        {
            // enumeration of all floor types that can be generated, mapped by property string from AncientCaveSettings
            mapGenerators[AncientCaveSettings.PROPERTYVALUE_FORESTBIOME] = new ForestGenerator();
            mapGenerators[AncientCaveSettings.PROPERTYVALUE_RUINSBIOME] = new RuinsGenerator();
            mapGenerators[AncientCaveSettings.PROPERTYVALUE_ISLANDBIOME] = new IslandGenerator();
            mapGenerators[AncientCaveSettings.PROPERTYVALUE_CAVEBIOME] = new CaveGenerator();
            mapGenerators[AncientCaveSettings.PROPERTYVALUE_MANAFORTINTBIOME] = new ManafortInteriorGenerator();
        }

        protected override string getName()
        {
            return "Randomization for ancient cave mode";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            context.workingData.setInt(DoorReplacer.PROPERTY_DOOR_LOCATION, 0x68000);
            List<AncientCaveMapGenerator> enabledMapGenerators = new List<AncientCaveMapGenerator>();
            List<string> floorTypesEnabled = settings.getArray(AncientCaveSettings.PROPERTYNAME_BIOME_TYPES).ToList();
            foreach (string mapType in mapGenerators.Keys)
            {
                if (floorTypesEnabled.Contains(mapType))
                {
                    enabledMapGenerators.Add(mapGenerators[mapType]);
                }
            }

            int numFloors = AncientCaveGenerator.LENGTH_CONVERSIONS[settings.get(AncientCaveSettings.PROPERTYNAME_LENGTH)];

            // generate events
            Dictionary<int, List<NpcTaggedEvent>> npcEvents = new AncientCaveEventMaker().process(origRom, outRom, seed, settings, context);

            Dictionary<int, List<int>> bossEvents = new Dictionary<int, List<int>>();
            int numBossEvents = context.workingData.getInt(AncientCaveEventMaker.BOSS_EVENT_TOTAL);
            for (int i = 0; i < numBossEvents; i++)
            {
                int floorNum = context.workingData.getInt(AncientCaveEventMaker.BOSS_EVENT_FLOORNUM + i);
                int entryEventNum = context.workingData.getInt(AncientCaveEventMaker.BOSS_ENTRY_EVENT_EVENTNUM + i);
                int deathEventNum = context.workingData.getInt(AncientCaveEventMaker.BOSS_DEATH_EVENT_EVENTNUM + i);
                bossEvents[floorNum] = new int[] { entryEventNum, deathEventNum }.ToList();
            }

            Dictionary<int, int> walkOnEvents = new Dictionary<int, int>();
            int numWalkonEvents = context.workingData.getInt(AncientCaveEventMaker.WALKON_EVENT_TOTAL);
            for (int i = 0; i < numWalkonEvents; i++)
            {
                walkOnEvents[i] = context.workingData.getInt(AncientCaveEventMaker.WALKON_EVENT_EVENTNUM + i);
            }

            // replace vanilla doors starting here
            // see StartingDoor hack - ancient cave starting door is 0x0a, so we create that door here with that ID
            int generatedDoorId = 10;
            Door startingDoor = new Door();
            context.replacementDoors[generatedDoorId++] = startingDoor;
            // as we progress through floors, we use this as the transition to the start of each next floor
            List<Door> previousFloorExitDoors = new Door[] { startingDoor }.ToList();
            // running map palette set index generated for floors
            context.workingData.setInt(GeneratedMapUtil.PROPERTY_GENERATED_PALETTE_INDEX, 16);
            // generate maps
            for (int i = 0; i < numFloors; i++)
            {
                // write the "continue door" for DoorExpansion hack, which determines where we go when we die, and use magic rope
                int startDoorOffset = context.workingData.getInt(DoorExpansion.START_DOOR_OFFSET);
                // 16 bit offsets relative to 0x68000
                outRom[startDoorOffset + i * 2] = (byte)(generatedDoorId - 1);
                outRom[startDoorOffset + i * 2 + 1] = (byte)((generatedDoorId - 1) >> 8);

                // generate floor
                AncientCaveMapGenerator mapGen = enabledMapGenerators[context.randomFunctional.Next() % enabledMapGenerators.Count];
                context.workingData.setInt(AncientCaveMapGenerator.PROPERTY_FLOOR_NUMBER, i);
                AncientCaveFloor floorData = mapGen.generate(context);
                // set the progress value
                settings.setInt(CommonSettings.PROPERTYNAME_CURRENT_PROGRESS, ((100 * (i + 1)) / numFloors));

                // inject song play command into the dummy part of the event
                VanillaEventUtil.replaceEventData(MUSIC_REPLACEMENT_PATTERN, context.replacementEvents[walkOnEvents[i]], floorData.songEventData.ToList());

                // inject reward npcs etc for floor
                List<NpcTaggedEvent> indoorMapNpcEvents = npcEvents[i];
                int numReplacementNpcs = Math.Min(indoorMapNpcEvents.Count, floorData.insideMap.mapData.mapObjects.Count);
                List<int> replacedNpcIndexes = new List<int>();
                for (int n = 0; n < numReplacementNpcs; n++)
                {
                    NpcTaggedEvent replacementEvent = indoorMapNpcEvents[n];
                    List<MapObject> npcs = floorData.insideMap.mapData.mapObjects;
                    int replacementNpcIndex = context.randomFunctional.Next() % npcs.Count;
                    while (replacedNpcIndexes.Contains(replacementNpcIndex))
                    {
                        replacementNpcIndex = context.randomFunctional.Next() % npcs.Count;
                    }
                    replacedNpcIndexes.Add(replacementNpcIndex);
                    MapObject npc = npcs[replacementNpcIndex];
                    npc.setSpecies(replacementEvent.npcId);
                    npc.setEventVisFlag(replacementEvent.eventFlagId);
                    npc.setEventVisMinimum(replacementEvent.eventFlagMin);
                    npc.setEventVisMaximum(replacementEvent.eventFlagMax);
                    npc.setEvent((ushort)replacementEvent.eventId);
                    if (replacementEvent.npcId >= 0x90 && replacementEvent.npcId <= 0x97)
                    {
                        // set frozen for element NPCs, otherwise their appearance glitches out
                        npc.setFrozen(true);
                    }
                }

                // create all necessary doors
                // - each outdoor -> indoor (indoorMap.altMapEntryLocations total doors, for each spot in outdoorMap.altMapExitLocations)
                SortedDictionary<XyPos, int> outdoorMapTriggers = pairDoors(context, floorData.outsideMap, floorData.insideMap, ref generatedDoorId, floorData.insideMap.mapId);
                // connect map to itself where necessary
                SortedDictionary<XyPos, int> outdoorMapInternalTriggers = pairDoors(context, floorData.outsideMap, floorData.outsideMap, ref generatedDoorId, floorData.outsideMap.mapId);
                foreach (XyPos key in outdoorMapInternalTriggers.Keys)
                {
                    outdoorMapTriggers[key] = outdoorMapInternalTriggers[key];
                }

                // - each indoor -> outdoor (outdoorMap.altMapEntryLocations total doors, for each spot in indoorMap.altMapExitLocations)
                SortedDictionary<XyPos, int> indoorMapTriggers = pairDoors(context, floorData.insideMap, floorData.outsideMap, ref generatedDoorId, floorData.outsideMap.mapId);
                // connect map to itself where necessary
                SortedDictionary<XyPos, int> indoorMapInternalTriggers = pairDoors(context, floorData.insideMap, floorData.insideMap, ref generatedDoorId, floorData.insideMap.mapId);
                foreach(XyPos key in indoorMapInternalTriggers.Keys)
                {
                    indoorMapTriggers[key] = indoorMapInternalTriggers[key];
                }
                // finally, add the triggers, sorted by x/y, to the map data we'll generate
                foreach (XyPos pos in indoorMapTriggers.Keys)
                {
                    floorData.insideMap.mapData.mapTriggers.Add((ushort)(indoorMapTriggers[pos]));
                }

                // connect previous floor's exit door (or the start door, if this is floor 1) to this floor's entry pos
                foreach (Door previousFloorExitDoor in previousFloorExitDoors)
                {
                    previousFloorExitDoor.setTargetMap(floorData.outsideMap.mapId);
                    previousFloorExitDoor.setXpos((byte)floorData.outsideMap.entryPos.xpos);
                    previousFloorExitDoor.setYpos((byte)floorData.outsideMap.entryPos.ypos);
                }

                // process exit to next floor / boss map and set previousFloorExitDoor for the next floor to connect to once it decides on an entry position
                if (bossEvents.ContainsKey(i))
                {
                    // boss on this floor - warp from outside map to boss map
                    Door bossDoor = new Door();
                    int bossEntryEventId = bossEvents[i][0];
                    foreach (XyPos exitPos in floorData.outsideMap.exitLocations)
                    {
                        outdoorMapTriggers[exitPos] = bossEntryEventId;
                    }
                    bossDoor.setTargetMap((ushort)(i * 2 + 256));
                    bossDoor.setXpos((byte)floorData.bossMap.entryPos.xpos);
                    bossDoor.setYpos((byte)floorData.bossMap.entryPos.ypos);
                    bossDoor.setLayer2Collision(floorData.bossMap.layer2Collision);
                    context.replacementDoors[generatedDoorId] = bossDoor;
                    int eventDoorId = generatedDoorId + 0x800;
                    VanillaEventUtil.replaceEventData(AncientCaveEventMaker.UNKNOWN_DOOR_REPLACEMENT_PATTERN, context.replacementEvents[bossEntryEventId], new byte[] { 0x18, (byte)eventDoorId, (byte)(eventDoorId >> 8) }.ToList());
                    generatedDoorId++;

                    // create door from boss to next floor; later processing (next loop) will fill the mapnum/x/y in
                    previousFloorExitDoors.Clear();
                    Door previousFloorExitDoor = new Door();
                    context.replacementDoors[generatedDoorId] = previousFloorExitDoor;
                    previousFloorExitDoors.Add(previousFloorExitDoor);

                    eventDoorId = generatedDoorId + 0x800;
                    int bossDeathEventId = bossEvents[i][1];
                    VanillaEventUtil.replaceEventData(AncientCaveEventMaker.UNKNOWN_DOOR_REPLACEMENT_PATTERN, context.replacementEvents[bossDeathEventId], new byte[] { 0x18, (byte)eventDoorId, (byte)(eventDoorId >> 8) }.ToList());
                    generatedDoorId++;

                    // attach boss death event to the boss object
                    foreach(MapObject obj in floorData.bossMap.mapData.mapObjects)
                    {
                        // species in boss range
                        if(obj.getSpecies() >= 0x57 && obj.getSpecies() < 0x80)
                        {
                            obj.setEvent((ushort)bossDeathEventId);
                        }
                    }
                }
                else
                {
                    previousFloorExitDoors.Clear();
                    // no boss on this floor - warp from outside map right to the next floor
                    foreach (XyPos exitPos in floorData.outsideMap.exitLocations)
                    {
                        Door previousFloorExitDoor = new Door();
                        outdoorMapTriggers[exitPos] = 0x800 + generatedDoorId;
                        context.replacementDoors[generatedDoorId] = previousFloorExitDoor;
                        generatedDoorId++;
                        previousFloorExitDoors.Add(previousFloorExitDoor);
                    }
                }

                // add the walkon trigger, as the first map trigger
                floorData.outsideMap.mapData.mapTriggers.Add((ushort)(walkOnEvents[i]));
                // finally, add the triggers, sorted by x/y, to the map data we'll generate
                foreach (XyPos pos in outdoorMapTriggers.Keys)
                {
                    floorData.outsideMap.mapData.mapTriggers.Add((ushort)(outdoorMapTriggers[pos]));
                }
            }

            // last floor exit -> manabeast
            // this is a copy of vanilla door 0xBC, which leads to vanilla mana beast map 0xFD
            foreach (Door previousFloorExitDoor in previousFloorExitDoors)
            {
                previousFloorExitDoor.setTargetMap(0xFD);
                previousFloorExitDoor.setXpos(18);
                previousFloorExitDoor.setYpos(22);
                previousFloorExitDoor.setTransitionType(0x81);
            }

            // re-create mana beast map - 0xFD - and credits map - 0xB4 since ancient cave effectively wipes all vanilla maps
            ManaBeastMap.makeManaBeastMap(origRom, context);
            CreditsMap.makeCreditsMap(origRom, context);
            // create the door to the credits map, that the mana beast death event references
            Door creditsDoor = new Door();
            creditsDoor.setTargetMap(0xB4);
            creditsDoor.setTransitionType(0xA0);
            creditsDoor.setXpos(8);
            creditsDoor.setYpos(6);
            context.replacementDoors[0x3FF] = creditsDoor;

            return true;
        }

        // pair one map's door exit locations to another's entry locations by ids specified in ancient cave floor processing, and create triggers for each
        private static SortedDictionary<XyPos, int> pairDoors(RandoContext context, AncientCaveMap sourceMap, AncientCaveMap destMap, ref int generatedDoorId, ushort targetMapId)
        {
            SortedDictionary<XyPos, int> mapTriggers = new SortedDictionary<XyPos, int>(locationComparison);
            Dictionary<int, XyPos> entryLocations;
            Dictionary<XyPos, int> exitLocations;
            if(sourceMap == destMap)
            {
                entryLocations = sourceMap.sameMapEntryLocations;
                exitLocations = sourceMap.sameMapExitLocations;
            }
            else
            {
                entryLocations = destMap.altMapEntryLocations;
                exitLocations = sourceMap.altMapExitLocations;
            }

            foreach (int destinationTargetId in entryLocations.Keys)
            {
                XyPos destinationDoorPos = entryLocations[destinationTargetId];
                bool foundMatch = false;
                foreach (XyPos sourceExitLocation in exitLocations.Keys)
                {
                    int correspondingDestinationDoor = exitLocations[sourceExitLocation];
                    if (correspondingDestinationDoor == destinationTargetId)
                    {
                        Door door = new Door();
                        door.setXpos((byte)destinationDoorPos.xpos);
                        door.setYpos((byte)destinationDoorPos.ypos);
                        door.setTargetMap(targetMapId);
                        if(destMap.layer2Collision)
                        {
                            door.setLayer2Collision(true);
                        }
                        context.replacementDoors[generatedDoorId] = door;
                        mapTriggers[sourceExitLocation] = 0x800 + generatedDoorId;
                        generatedDoorId++;
                        foundMatch = true;
                    }
                }

                // door that nothing pairs to on the other map; error out of everything
                if(!foundMatch)
                {
                    if (!sourceMap.returnDoors.Contains(destinationTargetId))
                    {
                        throw new Exception("Unable to find door " + destinationTargetId + " in " + sourceMap.mapName + " as requested by " + destMap.mapName);
                    }
                }
            }
            return mapTriggers;
        }
    }
}
