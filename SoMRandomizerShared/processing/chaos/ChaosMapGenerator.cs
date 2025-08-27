using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.chaos
{
    /// <summary>
    /// Creates chaos mode maps based on given ChaosGenerationState and ChaosMapGenerationInfo.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosMapGenerator
    {
        // undine, gnome, sylphid, salamando, lumina, shade, luna, dryad
        private static byte[] elementMapPalettes = new byte[] { 88, 89, 47, 91, 93, 92, 95, 97 };
        // Spell NPCs = 90 gnome, 91 undine, 92 salamando, 93 sylphid, 94 lumina, 95 shade, 96 luna, 97 dryad
        public static byte[] elementSprites = new byte[] { 0x91, 0x90, 0x93, 0x92, 0x94, 0x95, 0x96, 0x97 };
        // vanilla flags repurposed for chaos mode elements obtained; 10 gnome, 11 undine, 12 salamando, 13 sylphid, 14 lumina, 15 shade, 16 luna, 17 dryad
        private static byte[] elementFlags = new byte[] { 0x11, 0x10, 0x13, 0x12, 0x14, 0x15, 0x16, 0x17 };

        public static void generateFloor(RandoContext context, RandoSettings settings, ChaosMapGenerationInfo mapInfo, ChaosGenerationState chaosState)
        {
            ChaosVanillaMap map = mapInfo.map;
            byte[] origRom = context.originalRom;
            byte[] outRom = context.outputRom;
            MapHeader ogMapHeader = VanillaMapUtil.getHeader(origRom, map.mapId);
            FullMap generatedMap = new FullMap();
            generatedMap.mapHeader = ogMapHeader;
            int mainMapIndex = chaosState.fullMapIndex++;
            Logging.log("  Main map id = " + mainMapIndex.ToString("X4") + " from original map " + map.mapId.ToString("X4"), "debug");
            context.generatedMaps[mainMapIndex] = generatedMap;

            // order of maps for floor:
            // - main map
            // - element map, if applicable
            // - seed map, if applicable
            // - boss map, if applicable

            // transfer triggers
            List<ushort> ogTriggers = VanillaMapUtil.getTriggers(origRom, map.mapId);
            int numTriggers = ogTriggers.Count;
            int triggerOffset = 0;

            if (ogMapHeader.getWalkonEventEnabled())
            {
                // first one is a walk-on; ignore
                numTriggers--;
                triggerOffset++;
            }

            if (map.removeWalkonEvent)
            {
                if (mapInfo.walkonEventId == null)
                {
                    ogMapHeader.setWalkonEventEnabled(false);
                }
            }
            else
            {
                if (mapInfo.walkonEventId != null)
                {
                    // turn on 0x04
                    ogMapHeader.setWalkonEventEnabled(true);
                }
            }

            // fill in placeholder triggers that we will populate later
            for (int i = 0; i < numTriggers; i++)
            {
                generatedMap.mapTriggers.Add(ogTriggers[i + triggerOffset]);
            }

            // door to next floor
            int doorIndexToNextFloor = (int)mapInfo.doorIndexToNextFloor;
            // populate triggers for "next floor" door
            int[] triggerIds = map.doors[(int)mapInfo.doorIndexToNextFloor].triggerIds;
            foreach (int id in triggerIds)
            {
                generatedMap.mapTriggers[id] = (ushort)(chaosState.doorIndex + 0x800);
            }

            // make room for other maps on this floor, if applicable
            int elementMapNum = 0;
            int manaSeedMapNum = 0;
            int bossMapNum = 0;
            if (mapInfo.doorIndexToElementRoom != null)
            {
                elementMapNum = chaosState.fullMapIndex++;
                Logging.log("  Element map id = " + elementMapNum.ToString("X"), "debug");
            }
            if (mapInfo.doorIndexToManaSeedRoom != null)
            {
                manaSeedMapNum = chaosState.fullMapIndex++;
                Logging.log("  Mana seed map id = " + manaSeedMapNum.ToString("X"), "debug");
            }
            if (mapInfo.bossEventId != null)
            {
                bossMapNum = chaosState.fullMapIndex++;
                Logging.log("  Boss map id = " + bossMapNum.ToString("X"), "debug");
            }
            int nextFloorMapNum = chaosState.fullMapIndex;
            Logging.log("  Next floor's main map id = " + nextFloorMapNum.ToString("X"), "debug");

            // now make the door itself - these values will be set when we pick the next map
            Door nextFloorDoor = new Door();
            nextFloorDoor.setTargetMap((byte)nextFloorMapNum);
            nextFloorDoor.setXpos(0); // placeholder
            nextFloorDoor.setYpos(0); // placeholder
            nextFloorDoor.setTransitionType(0);
            int nextFloorDoorId = chaosState.doorIndex;
            Logging.log("  Door to next floor = " + nextFloorDoorId.ToString("X"), "debug");
            // set it on the map info so the next floor can link to it
            mapInfo.nextFloorDoorId = nextFloorDoorId;
            context.replacementDoors[chaosState.doorIndex++] = nextFloorDoor;

            if (mapInfo.bossEventId != null)
            {
                // in the next event, replace placeholder FFFFFF (from ChaosEventMaker) with door to the next actual floor (the one we just made)
                int skipToNextFloorEvent = (int)mapInfo.bossEventId + 1;
                int bossDeathEvent = skipToNextFloorEvent + 1;
                Logging.log("  Injecting next-floor door = " + nextFloorDoorId.ToString("X") + " into next floor skip event " + skipToNextFloorEvent.ToString("X"), "debug");
                VanillaEventUtil.replaceEventData(ChaosEventMaker.NEXT_FLOOR_DOOR_INJECTION_PATTERN.ToList(), context.replacementEvents[skipToNextFloorEvent], new byte[] { 0x18, (byte)nextFloorDoorId, (byte)((nextFloorDoorId >> 8) + 8) }.ToList());
                // replace triggers with the event that either goes to the boss room or triggers the next-floor door
                foreach (int id in triggerIds)
                {
                    generatedMap.mapTriggers[id] = (ushort)(mapInfo.bossEventId);
                }
            }


            // door back to prev floor
            if (mapInfo.doorIndexToLastFloor != null)
            {
                ChaosMapGenerationInfo prevFloorInfo = chaosState.previousFloorData;
                ChaosVanillaMap prevFloorMap = prevFloorInfo.map;
                int backFloorExitNum = (int)mapInfo.doorIndexToLastFloor;
                // Set the appropriate current-map triggers to point to the door to the previous floor
                int[] lastMapTriggerIds = map.doors[backFloorExitNum].triggerIds;
                foreach (int id in lastMapTriggerIds)
                {
                    generatedMap.mapTriggers[id] = (ushort)(chaosState.doorIndex + 0x800);
                }

                // Create the door back to the previous floor
                Door prevFloorDoor = new Door();
                prevFloorDoor.setTargetMap((ushort)prevFloorInfo.mainRoomMapId);
                prevFloorDoor.setXpos((byte)prevFloorMap.doors[(int)prevFloorInfo.doorIndexToNextFloor].warpBackX);
                prevFloorDoor.setYpos((byte)prevFloorMap.doors[(int)prevFloorInfo.doorIndexToNextFloor].warpBackY);
                prevFloorDoor.setTransitionType(getEnterExitByte(prevFloorMap.doors[(int)prevFloorInfo.doorIndexToNextFloor]));
                Logging.log("  Door to previous floor = " + chaosState.doorIndex.ToString("X"), "debug");
                context.replacementDoors[chaosState.doorIndex++] = prevFloorDoor;

                // Modify the previous floor's "next map" door to point to the right spot, and have the right collision.
                Logging.log("  Modifying previous floor's door to this floor = " + prevFloorInfo.nextFloorDoorId.ToString("X") + " to have correct x/y", "debug");
                Door prevFloorDoorToThisMap = context.replacementDoors[prevFloorInfo.nextFloorDoorId];
                prevFloorDoorToThisMap.setLayer2Collision(map.layer2Collision);
                prevFloorDoorToThisMap.setXpos((byte)map.doors[backFloorExitNum].warpBackX);
                prevFloorDoorToThisMap.setYpos((byte)map.doors[backFloorExitNum].warpBackY);
            }

            // copy objects from original map
            generatedMap.mapObjects.AddRange(VanillaMapUtil.getObjects(origRom, map.mapId));

            // enable flammie drum
            ogMapHeader.setFlammieEnabled(true);

            Random r = context.randomFunctional;

            // half the time for now - maybe later i make it do this more as the floors increase?
            if (settings.get(ChaosSettings.PROPERTYNAME_PALETTE_SWAP_TYPE) == "reasonable")
            {
                if ((r.Next() % 2) == 0)
                {
                    if (map.altPaletteSets.Length > 0)
                    {
                        ogMapHeader.setPaletteSet(map.altPaletteSets[r.Next() % map.altPaletteSets.Length]);
                        Logging.log("  Setting (reasonable) map palette to " + ogMapHeader.getPaletteSet().ToString("X2"), "debug");
                    }
                }
            }
            else if (settings.get(ChaosSettings.PROPERTYNAME_PALETTE_SWAP_TYPE) == "ridiculous")
            {
                // 115, 119
                byte[] pals = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 120, 121, 122, 123, 124, 125, 126, 127 };
                ogMapHeader.setPaletteSet(pals[r.Next() % pals.Length]);
                Logging.log("  Setting (ridiculous) map palette to " + ogMapHeader.getPaletteSet().ToString("X2"), "debug");
            }

            List<int> enemyIds = new List<int>();
            for (int i = 0; i < generatedMap.mapObjects.Count; i++)
            {
                if (map.objectsToRemove.Contains(i))
                {
                    // set vis flag to 0x00 F-F
                    generatedMap.mapObjects[i].setNeverVisible();
                }
                else
                {
                    byte objectType = generatedMap.mapObjects[i].getSpecies();
                    if (objectType >= 0x80 || objectType == 0x55)
                    {
                        // npcs and chests - always invisible
                        // also, for now, orbs
                        generatedMap.mapObjects[i].setNeverVisible();
                    }
                    else
                    {
                        // enemies - always visible
                        generatedMap.mapObjects[i].setAlwaysVisible();
                        enemyIds.Add(i);
                    }
                }
            }

            if (mapInfo.townMap && enemyIds.Count >= 2)
            {
                int wattsEventNum = context.workingData.getInt(ChaosEventMaker.WATTS_EVENT);
                int nekoEventNum = mapInfo.nekoEventNum;
                int wattsIndex = r.Next() % enemyIds.Count;
                int wattsId = enemyIds[wattsIndex];
                enemyIds.RemoveAt(wattsIndex);
                int nekoIndex = r.Next() % enemyIds.Count;
                int nekoId = enemyIds[nekoIndex];

                for (int i = 0; i < generatedMap.mapObjects.Count; i++)
                {
                    if (i == wattsId)
                    {
                        Logging.log("  Replacing object " + i + " with watts", "debug");
                        // visible
                        generatedMap.mapObjects[i].setAlwaysVisible();
                        // watts
                        generatedMap.mapObjects[i].setSpecies(0x9C);
                        // event
                        generatedMap.mapObjects[i].setEvent((ushort)wattsEventNum);
                        generatedMap.mapObjects[i].setUnknownB7(0x08);
                    }
                    else if (i == nekoId)
                    {
                        Logging.log("  Replacing object " + i + " with neko", "debug");
                        // visible
                        generatedMap.mapObjects[i].setAlwaysVisible();
                        // neko
                        generatedMap.mapObjects[i].setSpecies(0x99);
                        // event
                        generatedMap.mapObjects[i].setEvent((ushort)nekoEventNum);
                        generatedMap.mapObjects[i].setUnknownB7(0x08);
                    }
                    else
                    {
                        // invisible
                        generatedMap.mapObjects[i].setNeverVisible();
                    }
                }
            }

            // transfer map pieces
            FullMapMapPieces ogPieces = VanillaMapUtil.getMapPieceReference(origRom, map.mapId);
            generatedMap.mapPieces = ogPieces;

            // mana seed map
            if (mapInfo.doorIndexToManaSeedRoom != null)
            {
                FullMap manaSeedMap = new FullMap();
                context.generatedMaps[manaSeedMapNum] = manaSeedMap;
                int seedOgMap = SomVanillaValues.MAPNUM_FIRESEED; // fire palace seed map

                // set up header
                // note: no triggers on OG map; it uses a special event instead to provide a door back out
                MapHeader manaSeedMapHeader = VanillaMapUtil.getHeader(origRom, seedOgMap);
                manaSeedMap.mapHeader = manaSeedMapHeader;
                // remove walkon, but keep the special event
                manaSeedMapHeader.setWalkonEventEnabled(false);
                manaSeedMapHeader.setFlammieEnabled(true);

                manaSeedMapHeader.setPaletteSet(elementMapPalettes[(int)mapInfo.seedIndex]);


                // make both doors for this transition
                ChaosDoor seedDoor = new ChaosDoor(new int[] { }, 2, 14, 29);

                // door to seedmap
                Door doorToManaSeedMap = new Door();
                doorToManaSeedMap.setTargetMap((ushort)manaSeedMapNum);
                doorToManaSeedMap.setXpos((byte)seedDoor.warpBackX);
                doorToManaSeedMap.setYpos((byte)seedDoor.warpBackY);
                doorToManaSeedMap.setTransitionType(getEnterExitByte(seedDoor));
                int manaSeedExitNum = (int)mapInfo.doorIndexToManaSeedRoom;
                int[] lastMapTriggerIds = map.doors[(int)mapInfo.doorIndexToManaSeedRoom].triggerIds;
                Logging.log("  Door to mana seed map = " + chaosState.doorIndex.ToString("X"), "debug");
                foreach (int id in lastMapTriggerIds)
                {
                    generatedMap.mapTriggers[id] = (ushort)(chaosState.doorIndex + 0x800);
                }
                context.replacementDoors[chaosState.doorIndex++] = doorToManaSeedMap;


                // door back to floor main map
                Door doorBackFromManaSeedMap = new Door();
                doorBackFromManaSeedMap.setTargetMap((ushort)mainMapIndex);
                doorBackFromManaSeedMap.setXpos((byte)map.doors[manaSeedExitNum].warpBackX);
                doorBackFromManaSeedMap.setYpos((byte)map.doors[manaSeedExitNum].warpBackY);
                doorBackFromManaSeedMap.setTransitionType(getEnterExitByte(map.doors[manaSeedExitNum]));
                Logging.log("  Door back from mana seed map = " + chaosState.doorIndex.ToString("X"), "debug");
                // set the special event trigger to be the door back to the main map
                manaSeedMap.mapTriggers.Add((ushort)(chaosState.doorIndex + 0x800));
                context.replacementDoors[chaosState.doorIndex++] = doorBackFromManaSeedMap;


                // transfer map objects: two mana seeds, for some reason
                manaSeedMap.mapObjects.AddRange(VanillaMapUtil.getObjects(origRom, seedOgMap));

                foreach (MapObject mapObj in manaSeedMap.mapObjects)
                {
                    mapObj.setEvent((ushort)mapInfo.seedEventIndex);
                }

                // transfer pieces - don't modify
                manaSeedMap.mapPieces = VanillaMapUtil.getMapPieceReference(origRom, seedOgMap);
            }

            // element map
            if (mapInfo.doorIndexToElementRoom != null)
            {
                FullMap elementMap = new FullMap();
                context.generatedMaps[elementMapNum] = elementMap;

                int elementOgMap = 420;

                // set up header
                // note: no triggers on OG map; it uses return tiles to provide a door back out, so it isn't necessary to create any doors/triggers here
                MapHeader elementMapHeader = VanillaMapUtil.getHeader(origRom, elementOgMap);
                elementMap.mapHeader = elementMapHeader;
                // remove walkon
                elementMapHeader.setWalkonEventEnabled(false);
                elementMapHeader.setFlammieEnabled(true);

                elementMapHeader.setPaletteSet(elementMapPalettes[(int)mapInfo.elementIndex]);

                // make door for this transition
                ChaosDoor elementDoor = new ChaosDoor(new int[] { }, 2, 15, 25);

                // door to element map
                Door doorToElementMap = new Door();
                doorToElementMap.setTargetMap((ushort)elementMapNum);
                doorToElementMap.setXpos((byte)elementDoor.warpBackX);
                doorToElementMap.setYpos((byte)elementDoor.warpBackY);
                doorToElementMap.setTransitionType(getEnterExitByte(elementDoor));
                int[] lastMapTriggerIds = map.doors[(int)mapInfo.doorIndexToElementRoom].triggerIds;
                Logging.log("  Door to element map = " + chaosState.doorIndex.ToString("X"), "debug");
                foreach (int id in lastMapTriggerIds)
                {
                    generatedMap.mapTriggers[id] = (ushort)(chaosState.doorIndex + 0x800);
                }
                context.replacementDoors[chaosState.doorIndex++] = doorToElementMap;

                // transfer map objects
                // obj 0 needs to be removed (crystal orb)
                // objs 1 and 2 are enemies and can stay that way
                // obj 3 needs to become an element npc and have the event/vis flag from the thingy
                elementMap.mapObjects.AddRange(VanillaMapUtil.getObjects(origRom, elementOgMap));

                int elementIndex = (int)mapInfo.elementIndex;
                // spell orb
                elementMap.mapObjects[0].setNeverVisible();
                // book: change to element. visible only when flag hasn't been set yet
                elementMap.mapObjects[3].setEventVisFlag(elementFlags[elementIndex]);
                elementMap.mapObjects[3].setEventVisMinimum(0);
                elementMap.mapObjects[3].setEventVisMaximum(0);
                elementMap.mapObjects[3].setSpecies(elementSprites[elementIndex]);
                elementMap.mapObjects[3].setEvent((ushort)mapInfo.elementEventIndex);
                elementMap.mapObjects[3].setFrozen(true);

                // transfer pieces
                elementMap.mapPieces = VanillaMapUtil.getMapPieceReference(origRom, elementOgMap);

                // center the piece with the door outlet
                elementMap.mapPieces.bgPieces[1].xPos = 13;
            }

            // make boss map
            if (mapInfo.bossEventId != null)
            {
                Door doorToBossMap = new Door();
                ChaosBossMap bossMap = mapInfo.bossMap;
                doorToBossMap.setTargetMap((ushort)bossMapNum);
                doorToBossMap.setXpos((byte)bossMap.playerx);
                doorToBossMap.setYpos((byte)bossMap.playery);
                doorToBossMap.setTransitionType(getEnterExitByte(new ChaosDoor(new int[] { }, 4, 0, 0)));
                int bossDoorNum = chaosState.doorIndex;
                Logging.log("  Door to boss map = " + chaosState.doorIndex.ToString("X"), "debug");
                context.replacementDoors[chaosState.doorIndex++] = doorToBossMap;

                ushort deathEvent = (ushort)mapInfo.bossDeathEventId;

                Logging.log("  Importing map for boss " + mapInfo.bossId, "debug");
                int bossOgMapNum;
                if (mapInfo.bossId == SomVanillaValues.BOSSID_MANABEAST)
                {
                    // mana beast map settings
                    bossOgMapNum = SomVanillaValues.MAPNUM_MANABEAST_ARENA;
                    doorToBossMap.setTransitionType(0x81);
                }
                else
                {
                    bossOgMapNum = VanillaBossMaps.BY_VANILLA_BOSS_ID[(byte)mapInfo.bossId].originalMapNum;
                }
                FullMap generatedBossMap = new FullMap();
                generatedBossMap.mapHeader = VanillaMapUtil.getHeader(origRom, bossOgMapNum);
                // objects
                List<MapObject> mapObjects = VanillaMapUtil.getObjects(origRom, bossOgMapNum);
                generatedBossMap.mapObjects.AddRange(mapObjects);
                for (int i = 0; i < mapObjects.Count; i++)
                {
                    if (bossMap.objsToHide.Contains((byte)i))
                    {
                        mapObjects[i].setNeverVisible();
                    }
                    else if (bossMap.objsToShow.Contains((byte)i))
                    {
                        mapObjects[i].setAlwaysVisible();
                        mapObjects[i].setEvent(deathEvent);
                    }
                }
                context.generatedMaps[bossMapNum] = generatedBossMap;
                // pieces
                generatedBossMap.mapPieces = VanillaMapUtil.getMapPieceReference(origRom, bossOgMapNum);
                // triggers - set all to 0
                int triggerNum = VanillaMapUtil.getTriggers(origRom, bossOgMapNum).Count;
                if (generatedBossMap.mapHeader.getWalkonEventEnabled())
                {
                    // don't need this since it's not enabled for chaos boss maps
                    triggerNum--;
                }
                for (int i = 0; i < triggerNum; i++)
                {
                    generatedBossMap.mapTriggers.Add(0);
                }

                generatedBossMap.mapHeader.setFlammieEnabled(true);
                generatedBossMap.mapHeader.setWalkonEventEnabled(false);

                // in the boss entry event, replace placeholder EEEEEE (from ChaosEventMaker) with door to boss room - see bottom where i make the map (and have the door id)
                VanillaEventUtil.replaceEventData(ChaosEventMaker.BOSS_DOOR_INJECTION_PATTERN.ToList(), context.replacementEvents[(int)mapInfo.bossEventId], new byte[] { 0x18, (byte)bossDoorNum, (byte)((bossDoorNum >> 8) + 8) }.ToList());

                if (mapInfo.bossId == SomVanillaValues.BOSSID_DARKLICH)
                {
                    // lich music instead of boss music
                    VanillaEventUtil.replaceEventData(new byte[] { 0x40, 0x01, 0x04, 0x04, 0xFF }.ToList(), context.replacementEvents[(int)mapInfo.bossEventId], new byte[] { 0x40, 0x01, 0x0C, 0x06, 0xFF }.ToList());
                }
            }

            // add walkon event once we've set up all the real triggers, since it goes first and throws off all the indexing
            if (mapInfo.walkonEventId != null)
            {
                generatedMap.mapTriggers.Insert(0, (ushort)mapInfo.walkonEventId);
            }

            // we made a map!
        }

        public static byte getEnterExitByte(ChaosDoor destDoor)
        {
            byte dirByte = 0;
            // [3] eeexxxxx - exit, entry style
            // eee = 00 facing up (door=down), 05 facing down (door=up), 06 facing right (door=left), 07 facing left (door=right)
            // ^ 04 for teleport doors
            // xxxxx = 01 to walk up, 02 to walk down, 03 to walk right, 04 to walk left
            // ^ 00 for teleport doors

            // 0 up 1 right 2 down 3 left
            switch (destDoor.dir)
            {
                // entrance
                case 0:
                    dirByte |= (5 << 5);
                    break;
                case 1:
                    dirByte |= (7 << 5);
                    break;
                case 2:
                    // 0
                    break;
                case 3:
                    dirByte |= (6 << 5);
                    break;
                case 4:
                    // 0
                    break;
            }
            return dirByte;
        }

        public void replaceEventPattern(int startOffset, int nextOffset, byte[] outRom, byte[] searchPattern, byte[] replacePattern)
        {
            for (int e = startOffset; e < nextOffset; e++)
            {
                bool found = true;
                for (int j = 0; j < searchPattern.Length; j++)
                {
                    // from EventUtil,
                    // pointlessly screwing with event flag FB indicates where we should dump the song
                    if (outRom[e + j] != searchPattern[j])
                    {
                        found = false;
                    }
                }
                if (found)
                {
                    for (int j = 0; j < replacePattern.Length; j++)
                    {
                        outRom[e + j] = replacePattern[j];
                    }
                    break;
                }
            }
        }
    }
}
