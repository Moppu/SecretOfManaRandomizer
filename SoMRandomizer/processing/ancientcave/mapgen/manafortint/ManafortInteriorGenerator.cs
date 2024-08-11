using System;
using System.Collections.Generic;
using System.Linq;
using SoMRandomizer.processing.ancientcave.mapgen.manafortint.rooms;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.ancientcave.mapgen.manafortint
{
    /// <summary>
    /// Ancient cave map generator for "manafort interior" tileset.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ManafortInteriorGenerator : AncientCaveMapGenerator
    {
        // size of outdoor map, in 16x16 tiles
        const int outdoorMapWidth = 128;
        const int outdoorMapHeight = 128;
        // size of indoor map, in 16x16 tiles
        const int indoorMapWidth = 128;
        const int indoorMapHeight = 64;
        // size of boss map, in 16x16 tiles
        const int bossMapWidth = 32;
        const int bossMapHeight = 32;

        // 4x4 rooms per outdoor map
        const int numRoomsX = 4;
        const int numRoomsY = 4;

        const string numSharedDoorsProperty = "SharedDoors";

        // layer 1 tile types that enemies can be placed on
        static List<byte> enemyValidPlacementTiles = new byte[] {
            48,1,2,17,18,5,6,21,22,10,11,12,26,28,42,43,44,
        }.ToList();

        private PaletteSetSource[] palettes = new PaletteSetSource[]
        {
            // vanilla manafort palette
            new VanillaPaletteSetSource(45),
            // MOPPLE: add more?
        };

        protected override AncientCaveGenerationContext getGenerationContext(RandoContext context)
        {
            // no common nodes for manafort/int tileset - outdoor map door placement will be done at random, which in turn determines
            // the number of indoor areas to create
            AncientCaveGenerationContext generationContext = new AncientCaveGenerationContext();
            generationContext.outdoorMapWidth = outdoorMapWidth;
            generationContext.outdoorMapHeight = outdoorMapHeight;
            generationContext.indoorMapWidth = indoorMapWidth;
            generationContext.indoorMapHeight = indoorMapHeight;
            generationContext.bossMapWidth = bossMapWidth;
            generationContext.bossMapHeight = bossMapHeight;
            return generationContext;
        }

        protected override AncientCaveMap generateOutdoorMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            Random random = context.randomFunctional;

            // basically everything
            byte[,] layer1 = new byte[outdoorMapWidth, outdoorMapHeight];
            // nothing at all? full of zeros
            byte[,] layer2 = new byte[outdoorMapWidth, outdoorMapHeight];

            AncientCaveMap outsideMap = new AncientCaveMap();
            outsideMap.layer1Data = layer1;
            outsideMap.layer2Data = layer2;

            for (int y = 0; y < outdoorMapHeight; y++)
            {
                for (int x = 0; x < outdoorMapWidth; x++)
                {
                    layer1[x, y] = 95;
                    layer2[x, y] = 0;
                }
            }

            Dictionary<ManafortRoomMaker, int> roomMakerWeights = new Dictionary<ManafortRoomMaker, int>();
            roomMakerWeights[new CrystalRoom()] = 10;
            roomMakerWeights[new BridgeRoom()] = 10;
            roomMakerWeights[new DiagonalRoom()] = 10;
            roomMakerWeights[new CircleLineRoom()] = 10;
            // MOPPLE: whip posts are currently not working in ancient cave.  how did these get broken?
            roomMakerWeights[new WhipRoom()] = 0;
            roomMakerWeights[new MultiDiagonalRoom()] = 10;

            int sum = 0;
            foreach (int weight in roomMakerWeights.Values)
            {
                sum += weight;
            }
            ManafortRoomMaker[] allRoomMakers = new ManafortRoomMaker[sum];
            int id = 0;
            foreach (ManafortRoomMaker key in roomMakerWeights.Keys)
            {
                int weight = roomMakerWeights[key];
                for (int i = 0; i < weight; i++)
                {
                    allRoomMakers[id] = key;
                    id++;
                }
            }

            ProcGenMapNode startNode = new ProcGenMapNode();
            int sourceNodeCorner = random.Next() % 4;
            int sourceX = (sourceNodeCorner % 2) * (numRoomsX - 1);
            int sourceY = (sourceNodeCorner / 2) * (numRoomsY - 1);

            startNode.x = 4 + sourceX * 20;
            startNode.y = 4 + sourceY * 20;
            startNode.centerX = startNode.x + 10;
            startNode.centerY = startNode.y + 10;

            outsideMap.entryPos = new XyPos(startNode.centerX, startNode.centerY);

            StartEndRoom startRoom = new StartEndRoom(false);
            StartEndRoom endRoom = new StartEndRoom(true);
            Dictionary<string, ProcGenMapNode> nodes = new Dictionary<string, ProcGenMapNode>();
            int sx = (startNode.x - 4) / 20;
            int sy = (startNode.y - 4) / 20;
            // needs coordinate name for logging
            startNode.name = "" + sx + "" + sy;
            nodes.Add("" + sx + "" + sy, startNode);

            int ex = sx == 0 ? (numRoomsX - 1) : 0;
            int ey = sy == 0 ? (numRoomsY - 1) : 0;

            for (int nodeY = 0; nodeY < numRoomsY; nodeY++)
            {
                for (int nodeX = 0; nodeX < numRoomsX; nodeX++)
                {
                    string nodeKey = "" + nodeX + "" + nodeY;
                    if (!nodes.ContainsKey(nodeKey))
                    {
                        ProcGenMapNode n = new ProcGenMapNode();
                        n.x = 4 + nodeX * 20;
                        n.y = 4 + nodeY * 20;
                        n.centerX = n.x + 10;
                        n.centerY = n.y + 10;
                        n.name = nodeKey;
                        nodes.Add(nodeKey, n);
                    }
                }
            }

            // go from start, random valid direction from random visited node hitting only unvisited nodes until visited every spot
            List<ProcGenMapNode> visitedNodes = new List<ProcGenMapNode>();
            visitedNodes.Add(startNode);


            while (visitedNodes.Count < numRoomsX * numRoomsY)
            {
                ProcGenMapNode sourceNode = visitedNodes[random.Next() % visitedNodes.Count];
                List<ProcGenMapNode> possibleDestNodes = new List<ProcGenMapNode>();
                int sourcex = (sourceNode.x - 4) / 20;
                int sourcey = (sourceNode.y - 4) / 20;
                // check left
                if (sourcex != 0)
                {
                    ProcGenMapNode leftNode = nodes["" + (sourcex - 1) + "" + sourcey];
                    if (!visitedNodes.Contains(leftNode))
                    {
                        possibleDestNodes.Add(leftNode);
                    }
                }
                // right
                if (sourcex != numRoomsX - 1)
                {
                    ProcGenMapNode rightNode = nodes["" + (sourcex + 1) + "" + sourcey];
                    if (!visitedNodes.Contains(rightNode))
                    {
                        possibleDestNodes.Add(rightNode);
                    }
                }
                // check top
                if (sourcey != 0)
                {
                    ProcGenMapNode topNode = nodes["" + sourcex + "" + (sourcey - 1)];
                    if (!visitedNodes.Contains(topNode))
                    {
                        possibleDestNodes.Add(topNode);
                    }
                }
                // bottom
                if (sourcey != numRoomsY - 1)
                {
                    ProcGenMapNode bottomNode = nodes["" + sourcex + "" + (sourcey + 1)];
                    if (!visitedNodes.Contains(bottomNode))
                    {
                        possibleDestNodes.Add(bottomNode);
                    }
                }

                if (possibleDestNodes.Count > 0)
                {
                    ProcGenMapNode connectionNode = possibleDestNodes[random.Next() % possibleDestNodes.Count];
                    visitedNodes.Add(connectionNode);
                    int destx = (connectionNode.x - 4) / 20;
                    int desty = (connectionNode.y - 4) / 20;
                    string dir = "";
                    string otherDir = "";
                    if (destx == sourcex + 1)
                    {
                        dir = "right";
                        otherDir = "left";
                    }
                    if (destx == sourcex - 1)
                    {
                        dir = "left";
                        otherDir = "right";
                    }
                    if (desty == sourcey + 1)
                    {
                        dir = "bottom";
                        otherDir = "top";
                    }
                    if (desty == sourcey - 1)
                    {
                        dir = "top";
                        otherDir = "bottom";
                    }
                    sourceNode.values[dir] = 1;
                    connectionNode.values[otherDir] = 1;
                }
            }

            // give me some other random left/right doors, just to make it less tedious
            int extraLeftRightDoors = 5;
            int attempts = 0;
            while (extraLeftRightDoors > 0 && attempts < 100)
            {
                int x = random.Next() % numRoomsX;
                int y = random.Next() % numRoomsY;
                ProcGenMapNode n = nodes["" + x + "" + y];
                if ((!n.values.ContainsKey("left") || n.values["left"] == 0) && x != 0)
                {
                    int x2 = x - 1;
                    n.values["left"] = 1;
                    nodes["" + x2 + "" + y].values["right"] = 1;
                    extraLeftRightDoors--;
                }
                if ((!n.values.ContainsKey("right") || n.values["right"] == 0) && x != numRoomsX - 1)
                {
                    int x2 = x + 1;
                    n.values["right"] = 1;
                    nodes["" + x2 + "" + y].values["left"] = 1;
                    extraLeftRightDoors--;
                }
                attempts++;
            }

            // find all the spots where a door could lay
            List<ProcGenMapNode> possibleDoors = new List<ProcGenMapNode>();
            foreach (ProcGenMapNode n in nodes.Values)
            {
                // x=9, y=1
                // 147 147
                // 163 164
                // 176 180
                // 177 181
                if (!n.values.ContainsKey("top"))
                {
                    possibleDoors.Add(n);
                }
            }

            // pick spots for doors, put em in
            int numDoors = 0;
            while (numDoors < 8 && possibleDoors.Count > 0)
            {
                int doorIndex = random.Next() % possibleDoors.Count;
                ProcGenMapNode n = possibleDoors[doorIndex];
                int doorX = n.x + 9;
                int doorY = n.y + 1;
                possibleDoors.RemoveAt(doorIndex);
                n.values.Add("door", 1);
                numDoors++;
            }

            // randomly pick rooms for each slot in the outside map grid
            int doorNum = 0;
            for (int nodeY = 0; nodeY < numRoomsY; nodeY++)
            {
                for (int nodeX = 0; nodeX < numRoomsX; nodeX++)
                {
                    ProcGenMapNode n = nodes["" + nodeX + "" + nodeY];
                    ManafortRoomMaker roomMaker;
                    if (nodeY == sy && nodeX == sx)
                    {
                        roomMaker = startRoom;
                        generationContext.specialNodes["start"] = n;
                    }
                    else if (nodeY == ey && nodeX == ex)
                    {
                        roomMaker = endRoom;
                        generationContext.specialNodes["end"] = n;
                    }
                    else
                    {
                        roomMaker = allRoomMakers[random.Next() % allRoomMakers.Length];
                    }

                    byte[,] roomData = roomMaker.generateRoom(
                        n.values.ContainsKey("right"),
                        n.values.ContainsKey("left"),
                        n.values.ContainsKey("top"),
                        n.values.ContainsKey("bottom"),
                        n.values.ContainsKey("door"), random);
                    int roomXPos = 4 + nodeX * 20;
                    int roomYPos = 4 + nodeY * 20;
                    for(int y=0; y < 20; y++)
                    {
                        for (int x = 0; x < 20; x++)
                        {
                            layer1[roomXPos + x, roomYPos + y] = roomData[x, y];
                            // 177, 181 are the door tiles that get used in rooms; create doors from them here
                            if (roomData[x, y] == 177 || roomData[x, y] == 181)
                            {
                                if (roomData[x, y] == 177)
                                {
                                    outsideMap.altMapEntryLocations[doorNum] = new XyPos(roomXPos + x, roomYPos + y + 1);
                                }
                                outsideMap.altMapExitLocations[new XyPos(roomXPos + x, roomYPos + y)] = doorNum;
                            }
                        }
                    }
                    if(n.values.ContainsKey("door"))
                    {
                        doorNum++;
                    }
                }
            }

            // 54 is the tile that the end room uses as a warper off the floor (or to the boss)
            XyPos exitPos = null;
            for (int y=0; y < outdoorMapHeight; y++)
            {
                for (int x = 0; x < outdoorMapWidth; x++)
                {
                    if(layer1[x, y] == 54)
                    {
                        exitPos = new XyPos(x, y);
                    }
                }
            }

            int floorNumber = context.workingData.getInt(AncientCaveMapGenerator.PROPERTY_FLOOR_NUMBER);
            // shouldn't happen, but check it anyway
            ErrorUtil.checkNotNull(exitPos, "Couldn't find exit room for manafort map on floor " + floorNumber);

            // create exit location
            outsideMap.exitLocations.Add(exitPos);

            // create map header
            outsideMap.mapData = new FullMap();
            outsideMap.mapData.mapHeader = new MapHeader();
            outsideMap.mapData.mapHeader.setWalkonEventEnabled(true);

            // set tileset; 21 for vanilla manafort
            outsideMap.mapData.mapHeader.setTileset16(21);
            outsideMap.mapData.mapHeader.setTileset8(21);
            // probably necessary
            outsideMap.mapData.mapHeader.setAnimatedTiles(true);

            // display settings 0 for vanilla manafort maps
            outsideMap.mapData.mapHeader.setDisplaySettings(0);

            // layer 1 is bg, layer 2 is fg
            outsideMap.mapData.mapHeader.setLayer1UsesForegroundTiles(false);
            outsideMap.mapData.mapHeader.setLayer2UsesForegroundTiles(true);

            // npc palette 0xFF for hostile map
            outsideMap.mapData.mapHeader.setNpcPalette(0xFF);

            // these have special uses for ancient cave, so we enable both everywhere
            outsideMap.mapData.mapHeader.setFlammieEnabled(true);
            outsideMap.mapData.mapHeader.setMagicRopeEnabled(true);

            // choose palette
            PaletteSetSource outsidePal = palettes[random.Next() % palettes.Length];
            applyPaletteSource(context, outsideMap, outsidePal);

            // basically the whole tileset constantly color cycles
            outsideMap.mapData.mapHeader.setPaletteAnimated(true);

            // create enemies
            outsideMap.mapData.mapObjects.AddRange(makeOutdoorEnemies(startNode, random, layer1));

            // tell the inside map how many doors we made on the outside
            generationContext.sharedSettings.setInt(numSharedDoorsProperty, outsideMap.altMapEntryLocations.Count);

            // tell the logging where the nodes ended up
            foreach(string nodeName in nodes.Keys)
            {
                generationContext.sharedNodes.Add(nodes[nodeName]);
            }

            return outsideMap;
        }

        private List<MapObject> makeOutdoorEnemies(ProcGenMapNode startNode, Random random, byte[,] layer1MapData)
        {
            int maxEnemies = 40; // game fucks up if any more than 48
            byte[] enemyIds = new byte[]
            {
                 0x00, 0x00, 0x00, 0x00, // a few rabites
                 68, // tsunami
                 69, // basilisk
                 73, // whimper
                 74, // heck hound
                 75, // fiend head
                 76, // national scar
                 77, // dark stalker
                 78, // dark knight
                 79, // shape shifter
                 80, // wolf lord
                 81, // doom sword
                 82, // terminator
                 83, // master ninja
            };
            List<MapObject> enemies = new List<MapObject>();
            for (int i = 0; i < maxEnemies; i++)
            {
                bool foundNonWall = false;
                int enemyX = random.Next() % 128;
                int enemyY = random.Next() % 128;
                int distX = Math.Abs(enemyX - startNode.centerX);
                int distY = Math.Abs(enemyY - startNode.centerY);
                if (distX > outdoorMapWidth / 2)
                {
                    distX = outdoorMapWidth - distX;
                }
                if (distY > outdoorMapHeight / 2)
                {
                    distY = outdoorMapHeight - distY;
                }

                while (!foundNonWall)
                {
                    if (enemyValidPlacementTiles.Contains(layer1MapData[enemyX, enemyY]) &&
                        (distX > 16 || distY > 16))
                    {
                        foundNonWall = true;
                    }
                    else
                    {
                        enemyX = random.Next() % 128;
                        enemyY = random.Next() % 128;
                        distX = Math.Abs(enemyX - startNode.centerX);
                        distY = Math.Abs(enemyY - startNode.centerY);
                        if (distX > outdoorMapWidth / 2)
                        {
                            distX = outdoorMapWidth - distX;
                        }
                        if (distY > outdoorMapHeight / 2)
                        {
                            distY = outdoorMapHeight - distY;
                        }
                    }
                }
                byte type = enemyIds[random.Next() % enemyIds.Length];
                MapObject enemy = new MapObject();
                enemy.setXpos((byte)enemyX);
                enemy.setYpos((byte)enemyY);
                enemy.setSpecies(type);
                enemy.setEventVisFlag(0x00);
                enemy.setEventVisMinimum(0x00);
                enemy.setEventVisMaximum(0x0F);
                enemy.setEvent(0x01);
                enemy.setDirection(MapObject.DIR_LEFT);
                enemy.setUnknownB7(0x18);
                enemies.Add(enemy);
            }
            return enemies;
        }

        protected override AncientCaveMap generateIndoorMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            byte[,] indoorLayer1 = new byte[indoorMapWidth, indoorMapHeight];
            byte[,] indoorLayer2 = new byte[indoorMapWidth, indoorMapHeight];

            AncientCaveMap insideMap = new AncientCaveMap();
            insideMap.mapData = new FullMap();

            insideMap.layer1Data = indoorLayer1;
            insideMap.layer2Data = indoorLayer2;

            int numSharedDoors = generationContext.sharedSettings.getInt(numSharedDoorsProperty);

            ManafortIntNpcRooms.populateIndoorMapData(insideMap, numSharedDoors, indoorLayer1);

            Random random = context.randomFunctional;
            int numDialogueEvents = context.workingData.getInt(AncientCaveEventMaker.NUM_DIALOGUE_EVENTS);

            for (int i = 0; i < numSharedDoors; i++)
            {
                for (int n = 0; n < 3; n++)
                {
                    int eid = i * 3 + n;
                    int npcX = (i % 4) * 32 + 16;
                    int npcY = (i / 4) * 32 + 24 - 3 - n;

                    int eventNum = dialogueEventStart + (random.Next() % numDialogueEvents);
                    MapObject npcObject = new MapObject();
                    npcObject.setSpecies(npcIds[random.Next() % npcIds.Length]);
                    npcObject.setEventVisFlag(0x00);
                    npcObject.setEventVisMinimum(0x00);
                    npcObject.setEventVisMaximum(0x0F);
                    npcObject.setXpos((byte)npcX);
                    npcObject.setYpos((byte)npcY);
                    npcObject.setEvent((ushort)eventNum);
                    npcObject.setUnknownB7(0x08);
                    npcObject.setDirection(MapObject.DIR_LEFT);
                    npcObject.setUnknown4A(true);
                    insideMap.mapData.mapObjects.Add(npcObject);
                }
            }

            // create map header
            insideMap.mapData.mapHeader = new MapHeader();

            // set tileset; 21 for vanilla manafort
            insideMap.mapData.mapHeader.setTileset16(21);
            insideMap.mapData.mapHeader.setTileset8(21);

            // probably necessary
            insideMap.mapData.mapHeader.setAnimatedTiles(true);

            // display settings 0 for vanilla manafort maps
            insideMap.mapData.mapHeader.setDisplaySettings(0);

            // layer 1 is bg, layer 2 is fg
            insideMap.mapData.mapHeader.setLayer1UsesForegroundTiles(false);
            insideMap.mapData.mapHeader.setLayer2UsesForegroundTiles(true);

            // npc palette 0xFF for hostile map
            insideMap.mapData.mapHeader.setNpcPalette(0xFF);

            // these have special uses for ancient cave, so we enable both everywhere
            insideMap.mapData.mapHeader.setFlammieEnabled(true);
            insideMap.mapData.mapHeader.setMagicRopeEnabled(true);

            // basically the whole tileset constantly color cycles
            insideMap.mapData.mapHeader.setPaletteAnimated(true);

            // choose palette
            PaletteSetSource insidePal = palettes[random.Next() % palettes.Length];
            applyPaletteSource(context, insideMap, insidePal);

            return insideMap;
        }

        protected override AncientCaveMap generateBossMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            byte[,] bossLayer1 = new byte[bossMapWidth, bossMapHeight];
            byte[,] bossLayer2 = new byte[bossMapWidth, bossMapHeight];

            // pre-built arena
            // 32x32 bytes
            byte[] bossRoomData = DataUtil.readResource("SoMRandomizer.Resources.customMaps.acManafortBossArenaL1.bin");

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    bossLayer1[x, y] = bossRoomData[y * 32 + x];
                }
            }

            byte bossPlayerX = 16;
            byte bossPlayerY = 20;

            byte bossX = 16;
            byte bossY = 14;

            AncientCaveMap bossMap = new AncientCaveMap();
            bossMap.layer1Data = bossLayer1;
            bossMap.layer2Data = bossLayer2;
            bossMap.entryPos = new XyPos(bossPlayerX, bossPlayerY);
            bossMap.mapData = new FullMap();

            byte[] supportedBossTypes = new byte[]
            {
               SomVanillaValues.BOSSID_MANTISANT,
               SomVanillaValues.BOSSID_MINOTAUR,
               SomVanillaValues.BOSSID_JABBER,
               SomVanillaValues.BOSSID_FROSTGIGAS,
               SomVanillaValues.BOSSID_SNAPDRAGON,
               SomVanillaValues.BOSSID_VAMPIRE,
               SomVanillaValues.BOSSID_METALMANTIS,
               SomVanillaValues.BOSSID_KILROY,
               SomVanillaValues.BOSSID_GORGON,
               SomVanillaValues.BOSSID_GREATVIPER,
               SomVanillaValues.BOSSID_HYDRA,
               SomVanillaValues.BOSSID_HEXAS,
               SomVanillaValues.BOSSID_KETTLEKIN,
               SomVanillaValues.BOSSID_TONPOLE,
               SomVanillaValues.BOSSID_FIREGIGAS,
               SomVanillaValues.BOSSID_BUFFY,
               SomVanillaValues.BOSSID_DRAGONWORM,
               SomVanillaValues.BOSSID_THUNDERGIGAS,
            };

            Random random = context.randomFunctional;

            byte bossId = supportedBossTypes[random.Next() % supportedBossTypes.Length];

            MapObject bossObject = new MapObject();
            bossObject.setSpecies(bossId);
            bossObject.setXpos(bossX);
            bossObject.setYpos(bossY);
            bossObject.setEventVisFlag(0x00);
            bossObject.setEventVisMinimum(0x00);
            bossObject.setEventVisMaximum(0x0F);
            bossMap.mapData.mapObjects.Add(bossObject);

            // create map header
            bossMap.mapData.mapHeader = new MapHeader();
            bossMap.mapData.mapHeader.setWalkonEventEnabled(false);

            // set tileset; 21 for vanilla manafort
            bossMap.mapData.mapHeader.setTileset16(21);
            bossMap.mapData.mapHeader.setTileset8(21);

            // probably necessary
            bossMap.mapData.mapHeader.setAnimatedTiles(true);

            // display settings 0 for vanilla manafort maps
            bossMap.mapData.mapHeader.setDisplaySettings(0);

            // layer 1 is bg, layer 2 is fg
            bossMap.mapData.mapHeader.setLayer1UsesForegroundTiles(false);
            bossMap.mapData.mapHeader.setLayer2UsesForegroundTiles(true);

            // npc palette 0xFF for hostile map
            bossMap.mapData.mapHeader.setNpcPalette(0xFF);

            // these have special uses for ancient cave, so we enable both everywhere
            bossMap.mapData.mapHeader.setFlammieEnabled(true);
            bossMap.mapData.mapHeader.setMagicRopeEnabled(true);

            // basically the whole tileset constantly color cycles
            bossMap.mapData.mapHeader.setPaletteAnimated(true);

            // choose palette
            PaletteSetSource bossPal = palettes[random.Next() % palettes.Length];
            applyPaletteSource(context, bossMap, bossPal);

            return bossMap;
        }

        protected override string getMapLogString(AncientCaveFloor floorData, AncientCaveGenerationContext generationContext)
        {
            string mapLog = "";
            mapLog += "[key]\n";
            mapLog += "! :   start\n";
            mapLog += "@ :   end\n";
            mapLog += "o :   non-start or end room\n";
            mapLog += "- :   horizontal connection\n";
            mapLog += "| :   vertical connection\n";

            Dictionary<String, ProcGenMapNode> nodes = generationContext.getSharedNodesByName();
            ProcGenMapNode startNode = generationContext.specialNodes["start"];
            ProcGenMapNode endNode = generationContext.specialNodes["end"];
            for (int nodeY = 0; nodeY < numRoomsY; nodeY++)
            {
                mapLog += "\r\n";
                for (int nodeX = 0; nodeX < numRoomsX; nodeX++)
                {
                    ProcGenMapNode n = nodes["" + nodeX + "" + nodeY];
                    if (n == startNode)
                    {
                        mapLog += "!";
                    }
                    else if (n == endNode)
                    {
                        mapLog += "@";
                    }
                    else
                    {
                        mapLog += "o";
                    }

                    if (n.values.ContainsKey("right"))
                    {
                        mapLog += "-";
                    }
                    else
                    {
                        mapLog += " ";
                    }
                }

                mapLog += "\r\n";

                for (int nodeX = 0; nodeX < numRoomsX; nodeX++)
                {
                    ProcGenMapNode n = nodes["" + nodeX + "" + nodeY];
                    // next line of log
                    if (n.values.ContainsKey("bottom"))
                    {
                        mapLog += "|";
                    }
                    else
                    {
                        mapLog += " ";
                    }
                    mapLog += " ";
                }
            }

            return mapLog;
        }

        protected override byte[] getSongEventData()
        {
            // vanilla manafort interior music
            return new byte[] { 0x40, 0x01, 0x31, 0x16, 0x8F };
        }

        protected override string getFloorType()
        {
            return "Manafort Interior";
        }
    }
}
