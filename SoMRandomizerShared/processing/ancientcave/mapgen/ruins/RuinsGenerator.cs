using System;
using System.Collections.Generic;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.ancientcave.mapgen.ruins
{
    /// <summary>
    /// Ancient cave map generator for "ruins" tileset.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class RuinsGenerator : AncientCaveMapGenerator
    {
        // size of outdoor and indoor maps, in 16x16 tiles
        const int mapWidth = 128;
        const int mapHeight = 128;
        // size of boss map, in 16x16 tiles
        const int bossMapWidth = 32;
        const int bossMapHeight = 32;

        public const int numNodesX = 4;
        public const int numNodesY = 7;
        public const int nodeWidth = 20;
        public const int nodeHeightOutdoor = 9;
        public const int nodeHeightIndoor = 15;

        public const string roofNodeName = "roof";

        private PaletteSetSource[] outsidePalettes = new PaletteSetSource[]
        {
            new VanillaPaletteSetSource(78),
            new ResourcePaletteSetSource("acruins.nightruins.bin", 78),
            new VanillaPaletteSetSource(32),
            new ResourcePaletteSetSource("acruins.darkruins.bin", 32),
        };

        private PaletteSetSource[] insideAndBossPalettes = new PaletteSetSource[]
        {
            new VanillaPaletteSetSource(86),
            // MOPPLE: there should be more alternate palettes for this!
        };

        protected override AncientCaveGenerationContext getGenerationContext(RandoContext context)
        {
            AncientCaveGenerationContext generationContext = new AncientCaveGenerationContext();
            generationContext.outdoorMapWidth = mapWidth;
            generationContext.outdoorMapHeight = mapHeight;
            generationContext.indoorMapWidth = mapWidth;
            generationContext.indoorMapHeight = mapHeight;
            generationContext.bossMapWidth = bossMapWidth;
            generationContext.bossMapHeight = bossMapHeight;
            // create path through both indoor and outdoor maps
            RuinsPathGenerator.makePaths(context, generationContext);
            return generationContext;
        }

        protected override AncientCaveMap generateOutdoorMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            byte[,] layer1 = new byte[mapWidth, mapHeight];
            byte[,] layer2 = new byte[mapWidth, mapHeight];

            AncientCaveMap outsideMap = new AncientCaveMap();
            outsideMap.layer1Data = layer1;
            outsideMap.layer2Data = layer2;

            int effectiveHeight = mapHeight - 10;
            byte layer1dirt = 27;
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    // collision on layer 1 here
                    if (y < effectiveHeight)
                    {
                        layer1[x, y] = layer1dirt; // dirt
                    }
                    else
                    {
                        layer1[x, y] = 0; // empty wall on the bottom of the map
                    }
                    layer2[x, y] = 0; // empty
                }
            }

            // make the fixed part at the bottom of the map
            RuinsCourtyard.makeCourtyard(layer1, layer2, outsideMap);
            // generate a path through both maps to reach the ending
            ProcGenMapNode roofNode = generationContext.specialNodes[roofNodeName];

            int roomMinY = mapHeight - (70 + 4 * nodeHeightOutdoor);
            int roomMaxY = mapHeight - (70 - nodeHeightOutdoor - 2);
            // roof
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = roomMinY; y < roomMaxY; y++)
                {
                    layer1[x, y] = ((x % 2) == 0) ? (byte)124 : (byte)122;
                }

                // roof pieces
                layer1[x, roomMinY - 1] = 102;
                layer1[x, roomMinY - 2] = 87;
                layer1[x, roomMinY - 3] = 162;
                layer1[x, roomMinY - 4] = 162;
                layer1[x, roomMinY - 5] = 162;
                layer1[x, roomMinY - 6] = 162;
                layer1[x, roomMinY - 7] = 132;
            }

            // nodes are x=16, y=9 apart (mapHeight - 70, 79, etc)
            // so now we need a 16x9 block for every type of node:
            // node/no node in each of 4 dirs (16 total types * indoor/outdoor = 32 total)
            // entryway and roof will be special

            // create outdoor "rooms" and the doors to the corresponding inside rooms
            OutsideRooms.makeRooms(layer1, layer2, outsideMap, context, generationContext);

            // inside - like map 37; palset 86 for witch castle (map 306), 56 for castle (map 37)
            // these nodes will need to be taller
            byte[,] indoorLayer1 = new byte[mapWidth, mapHeight];
            byte[,] indoorLayer2 = new byte[mapWidth, mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    // collision on layer 1 here
                    indoorLayer1[x, y] = 18; // empty
                    indoorLayer2[x, y] = 0; // empty
                }
            }

            for (int y = 0; y < roofNode.y + 2; y++)
            {
                int _y = y - 8;
                if (_y >= 3)
                {
                    _y = 3;
                }

                for (int x = 0; x < mapWidth; x++)
                {
                    if (_y < 0)
                    {
                        layer1[x, y] = 0;
                        layer2[x, y] = 0;
                    }
                    else if (_y == 3)
                    {
                        layer1[x, y] = 0;
                        layer2[x, y] = 65;
                    }
                    else
                    {
                        int _x = x % 7;
                        layer1[x, y] = 0;
                        layer2[x, y] = (byte)(16 + _y * 16 + _x);
                    }
                }
            }

            // 144 left wall
            // 145 right wall
            // 119, 97 top wall
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    if (y == 0)
                    {
                        if (x == 0)
                        {
                            layer1[roofNode.x + x, roofNode.y + y] = 131;
                            layer2[roofNode.x + x, roofNode.y + y] = 0;
                        }
                        else if (x == 15)
                        {
                            layer1[roofNode.x + x, roofNode.y + y] = 133;
                            layer2[roofNode.x + x, roofNode.y + y] = 0;
                        }
                        else
                        {
                            layer1[roofNode.x + x, roofNode.y + y] = 31;
                            layer2[roofNode.x + x, roofNode.y + y] = 0;
                        }
                    }
                    else
                    {
                        if (x == 0)
                        {
                            layer1[roofNode.x + x, roofNode.y + y] = 144;
                            layer2[roofNode.x + x, roofNode.y + y] = 0;
                        }
                        else if (x == 15)
                        {
                            layer1[roofNode.x + x, roofNode.y + y] = 145;
                            layer2[roofNode.x + x, roofNode.y + y] = 0;
                        }
                        else
                        {
                            layer1[roofNode.x + x, roofNode.y + y] = 171;
                            layer2[roofNode.x + x, roofNode.y + y] = 0;
                        }
                    }
                }
            }

            ProcGenMapNode startNode = new ProcGenMapNode();
            // start at the bottom in the courtyard
            startNode.x = mapWidth / 2;
            startNode.y = mapHeight - 15;
            startNode.centerX = startNode.x;
            startNode.centerY = startNode.y;

            if ((context.randomFunctional.Next() % 2) == 0)
            {
                // start at gate - create exit point on the roof, and surround it with these thingies
                layer1[roofNode.x + 7, roofNode.y + 3] = 165;
                layer1[roofNode.x + 6, roofNode.y + 4] = 165;
                layer1[roofNode.x + 8, roofNode.y + 4] = 165;
                layer1[roofNode.x + 7, roofNode.y + 5] = 165;
                // exit tile
                layer1[roofNode.x + 7, roofNode.y + 4] = 186;
                outsideMap.exitLocations.Add(new XyPos(roofNode.x + 7, roofNode.y + 4));
            }
            else
            {
                // start on roof - create gate exit door at the bottom of the courtyard - two tiles next to each other
                layer1[mapWidth / 2 - 1, mapHeight - 10] = 185;
                layer1[mapWidth / 2, mapHeight - 10] = 185;
                outsideMap.exitLocations.Add(new XyPos(mapWidth / 2 - 1, mapHeight - 10));
                outsideMap.exitLocations.Add(new XyPos(mapWidth / 2, mapHeight - 10));
                // modify startnode to indicate roof start
                startNode.x = roofNode.x + 7;
                startNode.y = roofNode.y + 4;
                startNode.centerX = startNode.x;
                startNode.centerY = startNode.y;
            }

            // create start location
            outsideMap.entryPos = new XyPos(startNode.x, startNode.y);

            // create map header
            outsideMap.mapData = new FullMap();
            outsideMap.mapData.mapHeader = new MapHeader();
            outsideMap.mapData.mapHeader.setWalkonEventEnabled(true);

            // set tileset; 8 for ruins outside
            outsideMap.mapData.mapHeader.setTileset16(8);
            outsideMap.mapData.mapHeader.setTileset8(8);

            // create enemies
            outsideMap.mapData.mapObjects.AddRange(makeOutdoorEnemies(startNode, context.randomFunctional, layer1));

            // display settings 0 for vanilla ruins maps
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
            PaletteSetSource outsidePal = outsidePalettes[context.randomFunctional.Next() % outsidePalettes.Length];
            applyPaletteSource(context, outsideMap, outsidePal);

            return outsideMap;
        }

        private static List<MapObject> makeOutdoorEnemies(ProcGenMapNode startNode, Random random, byte[,] layer1MapData)
        {
            int maxEnemies = 40; // game fucks up if any more than 48
            byte[] enemyIds = new byte[]
            {
                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // a few rabites
                 0x07, // eye spy
                 0x09, // spectre
                 0x0a, // blat
                 0x0d, // polter chair
                 0x12, // zombie
                 0x17, // pumpkin bomb
                 0x19, // chess knight
                 0x1a, // wizard eye
                 0x1e, // grave bat
                 0x1f, // werewolf
                 0x22, // tomato man
                 0x23, // mystic book
                 0x2d, // mimic box
                 0x30, // ghost
                 0x33, // weepy eye
                 0x36, // ghoul
                 0x37, // imp
                 0x40, // eggplant man
                 0x42, // nitro pumpkin
                 0x46, // gremlin
                 0x49, // whimper
                 0x4c, // national scar
                 0x50, // wolf lord
            };

            List<MapObject> enemies = new List<MapObject>();

            for (int i = 0; i < maxEnemies; i++)
            {
                bool foundNonWall = false;
                int enemyX = random.Next() % 128;
                int enemyY = random.Next() % 128;
                int distX = Math.Abs(enemyX - startNode.centerX);
                int distY = Math.Abs(enemyY - startNode.centerY);
                if (distX > mapWidth / 2)
                {
                    distX = mapWidth - distX;
                }
                if (distY > mapHeight / 2)
                {
                    distY = mapHeight - distY;
                }

                while (!foundNonWall)
                {
                    if ((layer1MapData[enemyX, enemyY] == 28 ||
                        layer1MapData[enemyX, enemyY] == 168 ||
                        layer1MapData[enemyX, enemyY] == 63 ||
                        layer1MapData[enemyX, enemyY] == 170 ||
                        layer1MapData[enemyX, enemyY] == 171) &&
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
                        if (distX > mapWidth / 2)
                        {
                            distX = mapWidth - distX;
                        }
                        if (distY > mapHeight / 2)
                        {
                            distY = mapHeight - distY;
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
            byte[,] indoorLayer1 = new byte[mapWidth, mapHeight];
            byte[,] indoorLayer2 = new byte[mapWidth, mapHeight];
            AncientCaveMap insideMap = new AncientCaveMap();
            insideMap.mapData = new FullMap();
            insideMap.layer1Data = indoorLayer1;
            insideMap.layer2Data = indoorLayer2;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    // collision on layer 1 here
                    indoorLayer1[x, y] = 18; // empty
                    indoorLayer2[x, y] = 0; // empty
                }
            }

            // for indoor, nodes are 16x15 where outdoor are 16x9
            // create hallway, rooms, and all doors except the front door at the bottom
            InsideRooms.makeRooms(indoorLayer1, indoorLayer2, insideMap, context, generationContext);
            Dictionary<string, ProcGenMapNode> allNodes = generationContext.getSharedNodesByName();
            // indoor y values: innerNode.y = mapHeight - ((floorNum) * nodeHeightIndoor);
            // with height = 15, bottom floor = 2
            // so steps from y = mapHeight - 15 down toward bottom (-10?)
            // indoor warp spot = mapwidth/2, mapHeight - 11
            int stepsStartX = allNodes["i_2_" + (numNodesX / 2)].x - 4;
            for (int x = stepsStartX; x < stepsStartX + 4; x++)
            {
                for (int y = mapHeight - 18; y < mapHeight - 10; y++)
                {
                    indoorLayer1[x, y] = 108; // steps
                    if (y == mapHeight - 18)
                    {
                        if (x == stepsStartX)
                        {
                            indoorLayer1[x, y] = 120;
                            indoorLayer2[x, y] = 154;
                        }
                        else if (x == stepsStartX + 3)
                        {
                            indoorLayer1[x, y] = 120;
                            indoorLayer2[x, y] = 155;
                        }
                    }
                    else
                    {
                        if (x == stepsStartX)
                        {
                            indoorLayer1[x, y] = 120;
                            indoorLayer2[x, y] = 170;
                        }
                        else if (x == stepsStartX + 3)
                        {
                            indoorLayer1[x, y] = 120;
                            indoorLayer2[x, y] = 171;
                        }
                    }
                }
            }

            // front door; use -1 for the id; makeRooms() above is using 0+
            indoorLayer1[stepsStartX + 1, mapHeight - 10] = 65; // door
            indoorLayer1[stepsStartX + 2, mapHeight - 10] = 65; // door
            indoorLayer2[stepsStartX + 1, mapHeight - 10] = 164; // door
            indoorLayer2[stepsStartX + 2, mapHeight - 10] = 164; // door
            // courtyard -> main stairway exit/entry position
            insideMap.altMapExitLocations[new XyPos((short)(stepsStartX + 1), (short)(mapHeight - 10))] = -1;
            insideMap.altMapExitLocations[new XyPos((short)(stepsStartX + 2), (short)(mapHeight - 10))] = -1;
            insideMap.altMapEntryLocations[-1] = new XyPos((short)(stepsStartX + 1), (short)(mapHeight - 11)); 

            Random random = context.randomFunctional;

            // add npcs
            int numDialogueEvents = context.workingData.getInt(AncientCaveEventMaker.NUM_DIALOGUE_EVENTS);
            for (int i = 0; i < 15; i++)
            {
                int eid = i;
                int npcX = 0;
                int npcY = 0;
                if (insideMap.sameMapEntryLocations.ContainsKey(i))
                {
                    // indoor npcs
                    npcX = insideMap.sameMapEntryLocations[i].xpos;
                    npcY = insideMap.sameMapEntryLocations[i].ypos - 2;
                }
                else
                {
                    // hallway npcs
                    int floorNum = (i - 10) + 2;
                    bool foundEmptySpot = false;
                    int iters = 0;
                    while (!foundEmptySpot && iters < 100)
                    {
                        int xRoom = random.Next() % 4;
                        ProcGenMapNode n = allNodes["i_" + floorNum + "_" + xRoom];
                        int xpos = n.x + (random.Next() % 16);
                        int ypos = n.y + (random.Next() % 16);
                        if (xpos >= 0 && ypos >= 0 && xpos < mapWidth && ypos < mapHeight)
                        {
                            if (indoorLayer1[xpos, ypos] == 103 || indoorLayer1[xpos, ypos] == 100)
                            {
                                foundEmptySpot = true;
                                npcX = xpos;
                                npcY = ypos;
                            }
                        }
                        iters++;
                    }
                }
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

            insideMap.layer1Data = indoorLayer1;
            insideMap.layer2Data = indoorLayer2;

            // create map header
            insideMap.mapData.mapHeader = new MapHeader();

            // set tileset; 15 for vanilla castle interior
            insideMap.mapData.mapHeader.setTileset16(15);
            insideMap.mapData.mapHeader.setTileset8(15);

            // display settings 2 for vanilla ruins interior
            insideMap.mapData.mapHeader.setDisplaySettings(2);

            // layer 1 is bg, layer 2 is fg
            insideMap.mapData.mapHeader.setLayer1UsesForegroundTiles(false);
            insideMap.mapData.mapHeader.setLayer2UsesForegroundTiles(true);

            // npc palette 0xFF for hostile map
            insideMap.mapData.mapHeader.setNpcPalette(0xFF);

            // these have special uses for ancient cave, so we enable both everywhere
            insideMap.mapData.mapHeader.setFlammieEnabled(true);
            insideMap.mapData.mapHeader.setMagicRopeEnabled(true);

            // choose palette
            PaletteSetSource insidePal = insideAndBossPalettes[random.Next() % insideAndBossPalettes.Length];
            applyPaletteSource(context, insideMap, insidePal);

            return insideMap;
        }

        protected override AncientCaveMap generateBossMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            byte[,] bossLayer1 = new byte[bossMapWidth, bossMapHeight];
            byte[,] bossLayer2 = new byte[bossMapWidth, bossMapHeight];

            // for this one, don't use layer 2 .. we can have more shit that way
            for (int y = 0; y < bossMapHeight; y++)
            {
                for (int x = 0; x < bossMapWidth; x++)
                {
                    // black collidy wall
                    bossLayer1[x, y] = 18;
                    // nothin'
                    bossLayer2[x, y] = 0;
                }
            }

            int bossBoundary = 6;
            for (int y = bossBoundary; y < bossMapHeight - bossBoundary; y++)
            {
                for (int x = bossBoundary; x < bossMapWidth - bossBoundary; x++)
                {
                    if (y != bossBoundary && y < bossMapHeight - (bossBoundary + 1))
                    {
                        if (x == bossBoundary)
                        {
                            // left wall
                            bossLayer1[x, y] = 17;
                        }
                        else if (x == bossMapWidth - (bossBoundary + 1))
                        {
                            // right wall
                            bossLayer1[x, y] = 19;
                        }
                    }

                    if (x != bossBoundary && x != bossMapWidth - (bossBoundary + 1))
                    {
                        if (y == bossBoundary)
                        {
                            // upper wall
                            bossLayer1[x, y] = 2;
                            bossLayer1[x, y + 1] = 139;
                            bossLayer1[x, y + 2] = 56;
                            bossLayer1[x, y + 3] = 56;
                            bossLayer1[x, y + 4] = 56;
                            bossLayer1[x, y + 5] = 56;
                            bossLayer1[x, y + 6] = 136;
                            // floor
                            bossLayer1[x, y + 7] = 103;
                            // carpet north border
                            if (x == bossBoundary + 1)
                            {
                                bossLayer1[x, y + 8] = 103;
                            }
                            else if (x == bossBoundary + 2)
                            {
                                bossLayer1[x, y + 8] = 48;
                            }
                            else if (x == bossMapWidth - (bossBoundary + 3))
                            {
                                bossLayer1[x, y + 8] = 50;
                            }
                            else if (x == bossMapWidth - (bossBoundary + 2))
                            {
                                bossLayer1[x, y + 8] = 103;
                            }
                            else
                            {
                                bossLayer1[x, y + 8] = 49;
                            }

                            // floor
                            for (int _y = y + 9; _y < bossMapHeight - (bossBoundary + 3); _y++)
                            {
                                if (x == bossBoundary + 1)
                                {
                                    bossLayer1[x, _y] = 103;
                                }
                                else if (x == bossBoundary + 2)
                                {
                                    bossLayer1[x, _y] = 64;
                                }
                                else if (x == bossMapWidth - (bossBoundary + 3))
                                {
                                    bossLayer1[x, _y] = 66;
                                }
                                else if (x == bossMapWidth - (bossBoundary + 2))
                                {
                                    bossLayer1[x, _y] = 103;
                                }
                                else
                                {
                                    bossLayer1[x, _y] = 184;
                                }
                            }

                            if (x == bossBoundary + 1)
                            {
                                bossLayer1[x, bossMapHeight - (bossBoundary + 3)] = 103;
                            }
                            else if (x == bossBoundary + 2)
                            {
                                bossLayer1[x, bossMapHeight - (bossBoundary + 3)] = 80;
                            }
                            else if (x == bossMapWidth - (bossBoundary + 3))
                            {
                                bossLayer1[x, bossMapHeight - (bossBoundary + 3)] = 82;
                            }
                            else if (x == bossMapWidth - (bossBoundary + 2))
                            {
                                bossLayer1[x, bossMapHeight - (bossBoundary + 3)] = 103;
                            }
                            else
                            {
                                bossLayer1[x, bossMapHeight - (bossBoundary + 3)] = 81;
                            }

                            bossLayer1[x, bossMapHeight - (bossBoundary + 2)] = 103;
                        }
                        else if (y == bossMapHeight - (bossBoundary + 1))
                        {
                            // bottom wall
                            bossLayer1[x, y] = 34;
                        }
                    }

                }
            }

            // bookshelf
            bossLayer1[bossBoundary + 5, bossBoundary + 7] = 40;
            bossLayer1[bossBoundary + 6, bossBoundary + 7] = 41;
            bossLayer1[bossBoundary + 5, bossBoundary + 6] = 24;
            bossLayer1[bossBoundary + 6, bossBoundary + 6] = 25;
            bossLayer1[bossBoundary + 5, bossBoundary + 5] = 8;
            bossLayer1[bossBoundary + 6, bossBoundary + 5] = 9;

            // fireplace
            bossLayer1[bossBoundary + 10, bossBoundary + 6] = 170;
            bossLayer1[bossBoundary + 11, bossBoundary + 6] = 171;
            bossLayer1[bossBoundary + 12, bossBoundary + 6] = 172;
            bossLayer1[bossBoundary + 10, bossBoundary + 5] = 154;
            bossLayer1[bossBoundary + 11, bossBoundary + 5] = 155;
            bossLayer1[bossBoundary + 12, bossBoundary + 5] = 156;
            bossLayer1[bossBoundary + 10, bossBoundary + 4] = 46;
            bossLayer1[bossBoundary + 11, bossBoundary + 4] = 92;
            bossLayer1[bossBoundary + 12, bossBoundary + 4] = 47;

            AncientCaveMap bossMap = new AncientCaveMap();
            bossMap.layer1Data = bossLayer1;
            bossMap.layer2Data = bossLayer2;
            bossMap.entryPos = new XyPos(16, 24);
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
            bossObject.setXpos(16);
            bossObject.setYpos(16);
            bossObject.setEventVisFlag(0x00);
            bossObject.setEventVisMinimum(0x00);
            bossObject.setEventVisMaximum(0x0F);
            bossMap.mapData.mapObjects.Add(bossObject);

            // create map header
            bossMap.mapData.mapHeader = new MapHeader();
            bossMap.mapData.mapHeader.setWalkonEventEnabled(false);

            // set tileset; 15 for vanilla ruins interior
            bossMap.mapData.mapHeader.setTileset16(15);
            bossMap.mapData.mapHeader.setTileset8(15);

            // display settings 2 for vanilla ruins interior
            bossMap.mapData.mapHeader.setDisplaySettings(2);

            // layer 1 is bg, layer 2 is fg
            bossMap.mapData.mapHeader.setLayer1UsesForegroundTiles(false);
            bossMap.mapData.mapHeader.setLayer2UsesForegroundTiles(true);

            // npc palette 0xFF for hostile map
            bossMap.mapData.mapHeader.setNpcPalette(0xFF);

            // these have special uses for ancient cave, so we enable both everywhere
            bossMap.mapData.mapHeader.setFlammieEnabled(true);
            bossMap.mapData.mapHeader.setMagicRopeEnabled(true);

            // choose palette
            PaletteSetSource bossPal = insideAndBossPalettes[random.Next() % insideAndBossPalettes.Length];
            applyPaletteSource(context, bossMap, bossPal);

            return bossMap;
        }

        protected override byte[] getSongEventData()
        {
            // pandora music
            return new byte[] { 0x40, 0x01, 0x0F, 0x18, 0x8F };
        }

        protected override string getMapLogString(AncientCaveFloor floorData, AncientCaveGenerationContext generationContext)
        {
            Dictionary<String, ProcGenMapNode> allNodes = generationContext.getSharedNodesByName();
            String outsideLoggingString = "(outside)\n";
            for (int y = numNodesY; y >= 2; y--)
            {
                // node row
                for (int x = 0; x < numNodesX; x++)
                {
                    ProcGenMapNode n = allNodes["o_" + y + "_" + x];
                    if (n.values["mainpath"] == 1)
                    {
                        outsideLoggingString += "*";
                    }
                    else
                    {
                        if (n.values["ypos"] == 7)
                        {
                            outsideLoggingString += " ";
                        }
                        else
                        {
                            outsideLoggingString += "o";
                        }
                    }

                    if (x != numNodesX - 1)
                    {
                        ProcGenMapNode rn = allNodes["o_" + y + "_" + (x + 1)];
                        if (n.connections.Contains(rn) && n.values["ypos"] != 7)
                        {
                            outsideLoggingString += "-";
                        }
                        else
                        {
                            outsideLoggingString += " ";
                        }
                    }
                }
                outsideLoggingString += " " + y + "\r\n";
                // v conn row
                if (y != 2)
                {
                    for (int x = 0; x < numNodesX; x++)
                    {
                        ProcGenMapNode n = allNodes["o_" + y + "_" + x];
                        ProcGenMapNode bn = allNodes["o_" + (y - 1) + "_" + x];
                        if (n.connections.Contains(bn) && (n.values["ypos"] != 7 || n.values["mainpath"] == 1))
                        {
                            outsideLoggingString += "|";
                        }
                        else
                        {
                            outsideLoggingString += " ";
                        }
                        outsideLoggingString += " ";
                    }
                }
                outsideLoggingString += "\r\n";
            }

            String insideLoggingString = "(inside)\n";
            for (int y = numNodesY; y >= 2; y--)
            {
                // node row
                for (int x = 0; x < numNodesX; x++)
                {
                    ProcGenMapNode n = allNodes["i_" + y + "_" + x];
                    if (n.values["ypos"] != 7)
                    {
                        if (n.values["mainpath"] == 1)
                        {
                            insideLoggingString += "*";
                        }
                        else
                        {
                            insideLoggingString += "o";
                        }
                    }
                    else
                    {
                        insideLoggingString += " ";
                    }
                    if (x != numNodesX - 1)
                    {
                        ProcGenMapNode rn = allNodes["i_" + y + "_" + (x + 1)];
                        if (n.connections.Contains(rn) && n.values["ypos"] != 7)
                        {
                            insideLoggingString += "-";
                        }
                        else
                        {
                            insideLoggingString += " ";
                        }
                    }
                }
                insideLoggingString += " " + y + "\r\n";
                // v conn row
                if (y != 2)
                {
                    for (int x = 0; x < numNodesX; x++)
                    {
                        ProcGenMapNode n = allNodes["i_" + y + "_" + x];
                        ProcGenMapNode bn = allNodes["i_" + (y - 1) + "_" + x];
                        if (n.connections.Contains(bn) && n.values["ypos"] != 7)
                        {
                            insideLoggingString += "|";
                        }
                        else
                        {
                            insideLoggingString += " ";
                        }
                        insideLoggingString += " ";
                    }
                }
                insideLoggingString += "\r\n";
            }

            string logString = "";
            logString += "[key]\n";
            logString += "* :   main-path room\n";
            logString += "o :   non-main-path room\n";
            logString += "- :   horizontal connection\n";
            logString += "| :   vertical connection\n";
            logString += outsideLoggingString + "\n\n" + insideLoggingString;

            return logString;
        }

        protected override string getFloorType()
        {
            return "Ruins";
        }
    }
}
