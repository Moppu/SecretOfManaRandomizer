using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.ancientcave.mapgen.forest
{
    /// <summary>
    /// Ancient cave map generator for "forest" tileset.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ForestGenerator : AncientCaveMapGenerator
    {
        const int ledgeSize = 5;

        // space around edge of map where nothing will go, in 16x16 tiles
        const int nodeInsetAmountN = 10;
        const int nodeInsetAmountW = 10;
        const int nodeInsetAmountS = 10;
        const int nodeInsetAmountE = 10;

        // size of outdoor map, in 16x16 tiles
        const int outdoorMapWidth = 128;
        const int outdoorMapHeight = 128;
        // size of indoor map, in 16x16 tiles
        const int indoorMapWidth = 128;
        const int indoorMapHeight = 64;
        // size of boss map, in 16x16 tiles
        const int bossMapWidth = 32;
        const int bossMapHeight = 32;

        const string numSharedDoorsProperty = "SharedDoors";

        private PaletteSetSource[] outsideAndBossPalettes = new PaletteSetSource[]
        {
            new VanillaPaletteSetSource(26),
            new ResourcePaletteSetSource("acforest.fall.bin", 26),
            new ResourcePaletteSetSource("acforest.winter.bin", 26),
            new ResourcePaletteSetSource("acforest.spring.bin", 26),
            new ResourcePaletteSetSource("acforest.night.bin", 26),
            new ResourcePaletteSetSource("acforest.sunset.bin", 26),
        };

        private PaletteSetSource[] insidePalettes = new PaletteSetSource[]
        {
            new VanillaPaletteSetSource(69),
            new ResourcePaletteSetSource("accave.undinecave.bin", 69),
            new ResourcePaletteSetSource("accave.fierycave.bin", 69),
            new ResourcePaletteSetSource("accave.graycave.bin", 69),
        };

        protected override AncientCaveGenerationContext getGenerationContext(RandoContext context)
        {
            // no common nodes for forest tileset - outdoor map cave placement will determine the number of doors, which in turn determines
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

        private ProcGenMapNode makeRandomOutdoorNode(Random random, List<int> mountainLedgeYValues)
        {
            int totalUsableXArea = outdoorMapWidth - nodeInsetAmountW - nodeInsetAmountE;
            int totalUsableYArea = outdoorMapHeight - nodeInsetAmountN - nodeInsetAmountS;

            int ledgeCheckSize = ledgeSize + 2;
            int totalActualUsableYArea = totalUsableYArea - mountainLedgeYValues.Count * ledgeCheckSize * 2;
            ProcGenMapNode n = new ProcGenMapNode();
            n.x = (random.Next() % (totalUsableXArea)) + nodeInsetAmountW;
            n.y = (random.Next() % (totalActualUsableYArea)) + nodeInsetAmountN;
            n.centerX = n.x;
            foreach (int ledgeYValue in mountainLedgeYValues)
            {
                if (n.y >= ledgeYValue - ledgeCheckSize)
                {
                    n.y += ledgeCheckSize * 2;
                }
            }
            n.centerY = n.y;
            n.values["radius"] = (random.Next() % 4) + 4;
            return n;
        }


        protected override AncientCaveMap generateOutdoorMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            Random random = context.randomFunctional;
            int floorNumber = context.workingData.getInt(AncientCaveMapGenerator.PROPERTY_FLOOR_NUMBER);
            // trees, bushes, etc
            byte[,] layer1 = new byte[outdoorMapWidth, outdoorMapHeight];
            // background .. grass floor etc
            byte[,] layer2 = new byte[outdoorMapWidth, outdoorMapHeight];

            AncientCaveMap outsideMap = new AncientCaveMap();
            outsideMap.layer1Data = layer1;
            outsideMap.layer2Data = layer2;

            int numNodes = 20 + (random.Next() % 100);
            Logging.log("map " + floorNumber + " # nodes: " + numNodes, "debug");

            List<int> mountainLedgeYValues = new List<int>();
            int numLedges = (random.Next() % 4) + 1;
            for (int j = 0; j < numLedges; j++)
            {
                int yDiv = (outdoorMapHeight - nodeInsetAmountN - nodeInsetAmountS) / numLedges;
                int y = nodeInsetAmountN + (int)((j + 0.5) * yDiv) + (random.Next() % (yDiv / 2)) - yDiv / 4;
                mountainLedgeYValues.Add(y);
            }

            ProcGenMapNode startNode = makeRandomOutdoorNode(random, mountainLedgeYValues);
            startNode.name = "start";

            outsideMap.entryPos = new XyPos(startNode.x, startNode.y);
            // (layer1) tree pattern:
            // 117 116    \__/\__/\__/
            // 133 132     ||  ||  ||
            // 116 117    _/\__/\__/\_
            // 132 133    |  ||  ||  |
            // 117 116    \__/\__/\__/
            // 133 132     ||  ||  ||
            // 116 117    _/\__/\__/\_
            // 132 133    |  ||  ||  |

            // at edges:
            // 117 -> 113R, 114L
            // 132 -> 128L
            // 133 -> 129R (==53?)
            // 116 -> 115R

            // 70,71 on layer 2 for base of tree
            // below 115 (right), 114 (left)

            // 140 empty for layer1
            // 5 bush for layer1

            // fill L1 with trees initially, and L2 with grass
            for (int y = 0; y < outdoorMapHeight / 4; y++)
            {
                for (int x = 0; x < outdoorMapWidth / 2; x++)
                {
                    layer1[x * 2, y * 4] = ForestConstants.L1_TREE_UPPERRIGHT;
                    layer1[x * 2 + 1, y * 4] = ForestConstants.L1_TREE_UPPERLEFT;
                    layer1[x * 2, y * 4 + 1] = ForestConstants.L1_TREE_MIDRIGHT;
                    layer1[x * 2 + 1, y * 4 + 1] = ForestConstants.L1_TREE_MIDLEFT;
                    layer1[x * 2, y * 4 + 2] = ForestConstants.L1_TREE_UPPERLEFT;
                    layer1[x * 2 + 1, y * 4 + 2] = ForestConstants.L1_TREE_UPPERRIGHT;
                    layer1[x * 2, y * 4 + 3] = ForestConstants.L1_TREE_MIDLEFT;
                    layer1[x * 2 + 1, y * 4 + 3] = ForestConstants.L1_TREE_MIDRIGHT;
                }
            }

            // initial layer 2: short grass with random flowers
            for (int y = 0; y < outdoorMapHeight; y++)
            {
                for (int x = 0; x < outdoorMapWidth; x++)
                {
                    int r = (random.Next() % 100);
                    // flowers
                    if (r < 3)
                    {
                        layer2[x, y] = ForestConstants.L2_FLOWER_A;
                    }
                    else if (r < 6)
                    {
                        layer2[x, y] = ForestConstants.L2_FLOWER_B;
                    }
                    else
                    {
                        layer2[x, y] = ForestConstants.L2_SHORT_GRASS;
                    }
                }
            }

            // Now generate paths
            List<ProcGenMapNode> mapNodes = new List<ProcGenMapNode>();

            Logging.log("map " + floorNumber + " start node x=" + startNode.x + " y=" + startNode.y + " r=" + startNode.values["radius"], "debug");
            mapNodes.Add(startNode);
            generationContext.specialNodes["start"] = startNode;
            for (int i = 0; i < numNodes; i++)
            {
                ProcGenMapNode thisNode = makeRandomOutdoorNode(random, mountainLedgeYValues);
                thisNode.name = "n" + i;
                String nodeCharName;
                if (i < 10)
                {
                    nodeCharName = "" + i;
                }
                else
                {
                    nodeCharName = "" + (char)('A' + (i - 10));
                }

                Logging.log("map " + floorNumber + " node " + i + " (" + nodeCharName + ") x =" + thisNode.x + " y=" + thisNode.y + " r=" + thisNode.values["radius"], "debug");
                mapNodes.Add(thisNode);
            }

            ProcGenMapNode endNode = new ProcGenMapNode();
            bool northQuadrant = startNode.y < (outdoorMapHeight / 2);
            bool westQuadrant = startNode.x < (outdoorMapWidth / 2);
            // removed horizontal spawning for now to make ledges easier

            // exit along vertical
            if (northQuadrant)
            {
                // south exit
                endNode.y = outdoorMapHeight - 1 - nodeInsetAmountS;
            }
            else
            {
                // north exit
                endNode.y = nodeInsetAmountN;
            }
            endNode.x = random.Next() % (outdoorMapWidth / 2);
            if (westQuadrant)
            {
                // east exit
                endNode.x += (outdoorMapWidth / 2) - nodeInsetAmountE;
            }
            else
            {
                endNode.x += nodeInsetAmountW;
            }


            endNode.values["radius"] = (random.Next() % 4) + 4;
            endNode.name = "end";
            generationContext.specialNodes["end"] = endNode;
            Logging.log("map " + floorNumber + " end node x=" + endNode.x + " y=" + endNode.y + " r=" + endNode.values["radius"], "debug");
            mapNodes.Add(endNode);

            // now at each visited node, make a random-sized clearing, and connect it to the nearest unvisited node
            // first, process the startNode and mark it visited.

            for (int y = 0; y < outdoorMapHeight; y++)
            {
                for (int x = 0; x < outdoorMapWidth; x++)
                {
                    foreach (ProcGenMapNode n in mapNodes)
                    {
                        int xdiff = x - n.x;
                        int ydiff = y - n.y;
                        double diff = Math.Sqrt(xdiff * xdiff + ydiff * ydiff);
                        if (diff < n.values["radius"])
                        {
                            // empty
                            layer1[x, y] = ForestConstants.L1_EMPTY;
                        }
                        if (diff < n.values["radius"] - 2)
                        {
                            // empty
                            layer2[x, y] = ForestConstants.L2_GREEN;
                        }
                    }
                }
            }

            List<ProcGenMapNode> visitedNodes = new List<ProcGenMapNode>();
            List<ProcGenMapNode> unvisitedNodes = new List<ProcGenMapNode>();
            unvisitedNodes.AddRange(mapNodes);

            // entry node is auto visited, go from there
            visitedNodes.Add(startNode);
            unvisitedNodes.Remove(startNode);
            while (unvisitedNodes.Count > 0)
            {
                double minDist = 999;
                // must be visited
                ProcGenMapNode minDistSrcNode = null;
                // must be unvisited
                ProcGenMapNode minDistDstNode = null;
                foreach (ProcGenMapNode sourceNode in visitedNodes)
                {
                    foreach (ProcGenMapNode destNode in unvisitedNodes)
                    {
                        int xdiff = sourceNode.x - destNode.x;
                        int ydiff = sourceNode.y - destNode.y;
                        double diff = Math.Sqrt(xdiff * xdiff + ydiff * ydiff);
                        if (diff < minDist)
                        {
                            minDist = diff;
                            minDistSrcNode = sourceNode;
                            minDistDstNode = destNode;
                        }
                    }
                }

                int pathRadius = (random.Next() % 3) + 3;
                foreach (int ledgeYValue in mountainLedgeYValues)
                {
                    if ((minDistSrcNode.y <= ledgeYValue && minDistDstNode.y > ledgeYValue) ||
                        (minDistSrcNode.y > ledgeYValue && minDistDstNode.y <= ledgeYValue))
                    {
                        pathRadius += 4;
                        break;
                    }
                }

                int xd = minDistDstNode.x - minDistSrcNode.x;
                int yd = minDistDstNode.y - minDistSrcNode.y;
                double angle = Math.Atan2(yd, xd);
                double dx = Math.Cos(angle);
                double dy = Math.Sin(angle);
                double xPos = minDistSrcNode.x;
                double yPos = minDistSrcNode.y;
                int iters = 0;
                if (xd == 0)
                {
                    iters = (int)(Math.Abs((minDistSrcNode.y - minDistDstNode.y) / dy));
                }
                else
                {
                    iters = (int)(Math.Abs((minDistSrcNode.x - minDistDstNode.x) / dx));
                }
                for (int i = 0; i <= iters; i++)
                {
                    // clear radius, increment + dx/dy
                    for (int y = 0; y < outdoorMapHeight; y++)
                    {
                        for (int x = 0; x < outdoorMapWidth; x++)
                        {
                            double xdiff = x - xPos;
                            double ydiff = y - yPos;
                            double diff = Math.Sqrt(xdiff * xdiff + ydiff * ydiff);
                            if (diff < pathRadius)
                            {
                                // empty
                                layer1[x, y] = ForestConstants.L1_EMPTY;
                            }
                        }
                    }

                    xPos += dx;
                    yPos += dy;
                }

                Logging.log("map " + floorNumber + " connected " + minDistSrcNode.name + " to " + minDistDstNode.name, "debug");
                visitedNodes.Add(minDistDstNode);
                unvisitedNodes.Remove(minDistDstNode);
                minDistSrcNode.connections.Add(minDistDstNode);
                minDistDstNode.connections.Add(minDistSrcNode);
            }

            // make ledges, and caves to indoor map
            ForestMountainLedges.makeRandomLedgesWithCaves(outsideMap, context, mountainLedgeYValues, mapNodes, ledgeSize);

            // make the trees look like trees
            ForestTileEdges.addTileEdges(layer1, layer2, context);

            // 186 is invis layer1 w/ collision
            // 0 on layer 2?

            // mark the exit
            // layer 1 t 5 = bush
            layer1[endNode.x, endNode.y - 1] = ForestConstants.L1_BUSH;
            layer1[endNode.x, endNode.y + 1] = ForestConstants.L1_BUSH;
            layer1[endNode.x - 1, endNode.y] = ForestConstants.L1_BUSH;
            layer1[endNode.x + 1, endNode.y] = ForestConstants.L1_BUSH;
            // door
            layer1[endNode.x, endNode.y] = ForestConstants.L1_EMPTY_DOOR;
            // waterfall thing as a marker
            layer2[endNode.x, endNode.y] = ForestConstants.L2_WATERFALL;

            // create exit location
            outsideMap.exitLocations.Add(new XyPos((short)endNode.x, (short)endNode.y));

            // create map header
            outsideMap.mapData = new FullMap();
            outsideMap.mapData.mapHeader = new MapHeader();
            outsideMap.mapData.mapHeader.setWalkonEventEnabled(true);
            
            // set tileset; 2 for vanilla forest
            outsideMap.mapData.mapHeader.setTileset16(2);
            outsideMap.mapData.mapHeader.setTileset8(2);
            // make the grass blow around
            outsideMap.mapData.mapHeader.setAnimatedTiles(true);

            // display settings 1 for vanilla forest maps
            outsideMap.mapData.mapHeader.setDisplaySettings(1);

            // layer 1 is bg, layer 2 is fg
            outsideMap.mapData.mapHeader.setLayer1UsesForegroundTiles(false);
            outsideMap.mapData.mapHeader.setLayer2UsesForegroundTiles(true);

            // npc palette 0xFF for hostile map
            outsideMap.mapData.mapHeader.setNpcPalette(0xFF);

            // these have special uses for ancient cave, so we enable both everywhere
            outsideMap.mapData.mapHeader.setFlammieEnabled(true);
            outsideMap.mapData.mapHeader.setMagicRopeEnabled(true);

            // choose palette
            PaletteSetSource outsidePal = outsideAndBossPalettes[random.Next() % outsideAndBossPalettes.Length];
            applyPaletteSource(context, outsideMap, outsidePal);

            // so the exit door animates
            outsideMap.mapData.mapHeader.setPaletteAnimated(true);

            // create enemies
            outsideMap.mapData.mapObjects.AddRange(makeOutdoorEnemies(mapNodes, startNode, random, layer1));

            // tell the inside map how many rooms it should be making based on how many we were able to fit into the ledges
            generationContext.sharedSettings.setInt(numSharedDoorsProperty, outsideMap.altMapEntryLocations.Count);

            // tell the logging where the nodes ended up
            generationContext.sharedNodes.AddRange(mapNodes);

            return outsideMap;
        }

        private List<MapObject> makeOutdoorEnemies(List<ProcGenMapNode> mapNodes, ProcGenMapNode startNode, Random random, byte[,] layer1MapData)
        {
            int total = 0;
            int maxEnemies = 32; // game fucks up if any more than 48
            byte[] enemyIds = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // rabite
                0x01, // buzz bee
                0x02, // mushboom
                0x04, // lullabud
                0x14, // silktail
                0x29, // bomb bee
                0x2A, // mushgloom
                0x2B, // trap flower
            };
            List<MapObject> enemies = new List<MapObject>();
            foreach (ProcGenMapNode n in mapNodes)
            {
                int distX = Math.Abs(n.centerX - startNode.centerX);
                int distY = Math.Abs(n.centerY - startNode.centerY);
                if (distX > outdoorMapWidth / 2)
                {
                    distX = outdoorMapWidth - distX;
                }
                if (distY > outdoorMapHeight / 2)
                {
                    distY = outdoorMapHeight - distY;
                }

                if (n != startNode)
                {
                    int numEnemies = (random.Next() % 2);
                    if (distX > 16 || distY > 16)
                    {
                        for (int enemyNum = 0; enemyNum < numEnemies && total < maxEnemies; enemyNum++)
                        {
                            int enemyX = n.x + (random.Next() % (n.values["radius"] * 2 - 2)) - (n.values["radius"] - 1);
                            int enemyY = n.y + (random.Next() % (n.values["radius"] * 2 - 2)) - (n.values["radius"] - 1);
                            int iters = 0;
                            // find an empty spot
                            while (layer1MapData[enemyX, enemyY] != ForestConstants.L1_EMPTY && iters < 100)
                            {
                                enemyX = n.x + (random.Next() % (n.values["radius"] * 2 - 2)) - (n.values["radius"] - 1);
                                enemyY = n.y + (random.Next() % (n.values["radius"] * 2 - 2)) - (n.values["radius"] - 1);
                                iters++;
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
                            total++;
                        }
                        if (total >= maxEnemies)
                        {
                            break;
                        }
                    }
                }
            }
            return enemies;
        }

        protected override AncientCaveMap generateIndoorMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            // second map
            Random random = context.randomFunctional;
            AncientCaveMap insideMap = new AncientCaveMap();
            insideMap.mapData = new FullMap();
            byte[,] indoorLayer1 = new byte[indoorMapWidth, indoorMapHeight];
            byte[,] indoorLayer2 = new byte[indoorMapWidth, indoorMapHeight];
            int numSharedDoors = generationContext.sharedSettings.getInt(numSharedDoorsProperty);
            ForestCaveRooms.populateIndoorMapData(insideMap, numSharedDoors, context, indoorLayer1, indoorLayer2);
            List<int> indoorEntryX = new List<int>();
            List<int> indoorEntryY = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                indoorEntryX.Add(15 + (i % 4) * 32);
                indoorEntryY.Add(25 + (i / 4) * 32);
            }

            int numIndoorMaps = numSharedDoors;

            // populate with pointless dialogue npcs - these will be replaced later randomly for rewards, shops, etc
            int numDialogueEvents = context.workingData.getInt(AncientCaveEventMaker.NUM_DIALOGUE_EVENTS);
            for (int i = 0; i < numIndoorMaps; i++)
            {
                for (int n = 0; n < 3; n++)
                {
                    int npcX = indoorEntryX[i];
                    int npcY = indoorEntryY[i] - 3 - n;

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

            insideMap.layer1Data = indoorLayer1;
            insideMap.layer2Data = indoorLayer2;

            // create map header
            insideMap.mapData.mapHeader = new MapHeader();

            // set tileset; 10 for vanilla cave
            insideMap.mapData.mapHeader.setTileset16(10);
            insideMap.mapData.mapHeader.setTileset8(10);

            // display settings 1 for vanilla cave maps
            insideMap.mapData.mapHeader.setDisplaySettings(1);

            // layer 1 is bg, layer 2 is fg
            insideMap.mapData.mapHeader.setLayer1UsesForegroundTiles(false);
            insideMap.mapData.mapHeader.setLayer2UsesForegroundTiles(true);

            // npc palette 0xFF for hostile map
            insideMap.mapData.mapHeader.setNpcPalette(0xFF);

            // these have special uses for ancient cave, so we enable both everywhere
            insideMap.mapData.mapHeader.setFlammieEnabled(true);
            insideMap.mapData.mapHeader.setMagicRopeEnabled(true);

            // choose palette
            PaletteSetSource insidePal = insidePalettes[random.Next() % insidePalettes.Length];
            applyPaletteSource(context, insideMap, insidePal);

            return insideMap;
        }

        protected override AncientCaveMap generateBossMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            byte[,] bossLayer1 = new byte[bossMapWidth, bossMapHeight];
            byte[,] bossLayer2 = new byte[bossMapWidth, bossMapHeight];

            Random random = context.randomFunctional;
            // make a simple boss arena
            for (int y = 0; y < bossMapHeight; y++)
            {
                for (int x = 0; x < bossMapWidth; x++)
                {
                    bossLayer1[x, y] = ForestConstants.L1_EMPTY;
                    bossLayer2[x, y] = ForestConstants.L2_SHORT_GRASS;
                }
            }

            for (int y = 0; y < bossMapHeight / 4; y++)
            {
                for (int x = 0; x < bossMapWidth / 2; x++)
                {
                    if ((x * 2 < 4 || x * 2 >= bossMapWidth - 4) || (y * 4 < 4 || y * 4 >= bossMapHeight - 4))
                    {
                        bossLayer1[x * 2, y * 4] = ForestConstants.L1_TREE_UPPERRIGHT;
                        bossLayer1[x * 2 + 1, y * 4] = ForestConstants.L1_TREE_UPPERLEFT;
                        bossLayer1[x * 2, y * 4 + 1] = ForestConstants.L1_TREE_MIDRIGHT;
                        bossLayer1[x * 2 + 1, y * 4 + 1] = ForestConstants.L1_TREE_MIDLEFT;
                        bossLayer1[x * 2, y * 4 + 2] = ForestConstants.L1_TREE_UPPERLEFT;
                        bossLayer1[x * 2 + 1, y * 4 + 2] = ForestConstants.L1_TREE_UPPERRIGHT;
                        bossLayer1[x * 2, y * 4 + 3] = ForestConstants.L1_TREE_MIDLEFT;
                        bossLayer1[x * 2 + 1, y * 4 + 3] = ForestConstants.L1_TREE_MIDRIGHT;
                    }
                    else if (x * 2 == 4)
                    {
                        // left tree edges
                        bossLayer1[x * 2, y * 4] = ForestConstants.L1_TREE_UPPERRIGHT_EDGE;
                        bossLayer1[x * 2, y * 4 + 1] = ForestConstants.L1_TREE_MIDRIGHT_EDGE;
                        bossLayer1[x * 2, y * 4 + 2] = ForestConstants.L1_TREE_LOWRIGHT_EDGE;
                        bossLayer1[x * 2, y * 4 + 3] = ForestConstants.L1_EMPTY_BLOCK;
                        bossLayer2[x * 2, y * 4 + 3] = ForestConstants.L2_STUMP_RIGHT;
                    }
                    else if (x * 2 == bossMapWidth - 6)
                    {
                        // right tree edges
                        bossLayer1[x * 2 + 1, y * 4] = ForestConstants.L1_TREE_UPPERLEFT_EDGE;
                        bossLayer1[x * 2 + 1, y * 4 + 1] = ForestConstants.L1_TREE_MIDLEFT_EDGE;
                        bossLayer1[x * 2 + 1, y * 4 + 2] = ForestConstants.L1_TREE_LOWLEFT_EDGE;
                        bossLayer1[x * 2 + 1, y * 4 + 3] = ForestConstants.L1_EMPTY_BLOCK;
                        bossLayer2[x * 2 + 1, y * 4 + 3] = ForestConstants.L2_STUMP_LEFT;
                    }
                    else if (y * 4 == 4)
                    {
                        // upper tree edges
                        bossLayer1[x * 2, y * 4] = ForestConstants.L1_TREE_LOWLEFT_EDGE;
                        bossLayer1[x * 2 + 1, y * 4] = ForestConstants.L1_TREE_LOWRIGHT_EDGE;
                        bossLayer1[x * 2, y * 4 + 1] = ForestConstants.L1_EMPTY_BLOCK;
                        bossLayer1[x * 2 + 1, y * 4 + 1] = ForestConstants.L1_EMPTY_BLOCK;
                        bossLayer2[x * 2, y * 4 + 1] = ForestConstants.L2_STUMP_LEFT;
                        bossLayer2[x * 2 + 1, y * 4 + 1] = ForestConstants.L2_STUMP_RIGHT;
                    }
                    else if (y * 4 == bossMapHeight - 8)
                    {
                        // bottom tree edges
                        bossLayer1[x * 2, y * 4 + 2] = ForestConstants.L1_TREE_UPPERLEFT_EDGE;
                        bossLayer1[x * 2 + 1, y * 4 + 2] = ForestConstants.L1_TREE_UPPERRIGHT_EDGE;
                        bossLayer1[x * 2, y * 4 + 3] = ForestConstants.L1_TREE_MIDLEFT_EDGE;
                        bossLayer1[x * 2 + 1, y * 4 + 3] = ForestConstants.L1_TREE_MIDRIGHT_EDGE;
                    }
                }
            }

            // upper left corner
            bossLayer1[4, 4] = ForestConstants.L1_TREE_UPPERRIGHT;
            bossLayer1[5, 4] = ForestConstants.L1_TREE_LOWRIGHT_EDGE;
            bossLayer1[4, 5] = ForestConstants.L1_TREE_MIDRIGHT;
            bossLayer1[5, 5] = ForestConstants.L1_EMPTY_BLOCK;
            bossLayer2[5, 5] = ForestConstants.L2_STUMP_RIGHT;

            // upper right corner
            bossLayer1[bossMapWidth - 6, 4] = ForestConstants.L1_TREE_LOWLEFT_EDGE;
            bossLayer1[bossMapWidth - 5, 4] = ForestConstants.L1_TREE_UPPERLEFT;
            bossLayer1[bossMapWidth - 6, 5] = ForestConstants.L1_EMPTY_BLOCK;
            bossLayer2[bossMapWidth - 6, 5] = ForestConstants.L2_STUMP_LEFT;
            bossLayer1[bossMapWidth - 5, 5] = ForestConstants.L1_TREE_MIDLEFT;

            // lower left corner
            bossLayer1[4, bossMapHeight - 6] = ForestConstants.L1_TREE_UPPERLEFT;
            bossLayer1[5, bossMapHeight - 6] = ForestConstants.L1_TREE_UPPERRIGHT_EDGE;
            bossLayer1[4, bossMapHeight - 5] = ForestConstants.L1_TREE_MIDLEFT;
            bossLayer1[5, bossMapHeight - 5] = ForestConstants.L1_TREE_MIDRIGHT_EDGE;

            // lower right corner
            bossLayer1[bossMapWidth - 6, bossMapHeight - 6] = ForestConstants.L1_TREE_UPPERLEFT_EDGE;
            bossLayer1[bossMapWidth - 5, bossMapHeight - 6] = ForestConstants.L1_TREE_UPPERRIGHT;
            bossLayer1[bossMapWidth - 6, bossMapHeight - 5] = ForestConstants.L1_TREE_MIDLEFT_EDGE;
            bossLayer1[bossMapWidth - 5, bossMapHeight - 5] = ForestConstants.L1_TREE_MIDRIGHT;

            byte bossPlayerX = 16;
            byte bossPlayerY = 20;

            byte bossX = 16;
            byte bossY = 14;

            int numExtraTrees = 8 + (random.Next() % 8);
            List<int> treeXs = new List<int>();
            List<int> treeYs = new List<int>();
            for (int i = 0; i < numExtraTrees; i++)
            {
                int treeX = 6 + (random.Next() % (bossMapWidth - 12));
                int treeY = 8 + (random.Next() % (bossMapHeight - 16));
                bool overlap = false;
                for (int t = 0; t < treeXs.Count; t++)
                {
                    int tx = treeXs[t];
                    int ty = treeYs[t];
                    if (Math.Abs(treeX - tx) < 2 && Math.Abs(treeY - ty) < 4)
                    {
                        overlap = true;
                    }
                }
                // don't stick them near the center where the boss and player are
                if (!overlap && (Math.Abs(treeX - bossX) > 6 || Math.Abs(treeY - bossY) > 6)
                    && (Math.Abs(treeX - bossPlayerX) > 6 || Math.Abs(treeY - bossPlayerY) > 6))
                {
                    bossLayer1[treeX, treeY] = ForestConstants.L1_TREE_UPPERLEFT_EDGE;
                    bossLayer1[treeX + 1, treeY] = ForestConstants.L1_TREE_UPPERRIGHT_EDGE;
                    bossLayer1[treeX, treeY + 1] = ForestConstants.L1_TREE_MIDLEFT_EDGE;
                    bossLayer1[treeX + 1, treeY + 1] = ForestConstants.L1_TREE_MIDRIGHT_EDGE;
                    bossLayer1[treeX, treeY + 2] = ForestConstants.L1_TREE_LOWLEFT_EDGE;
                    bossLayer1[treeX + 1, treeY + 2] = ForestConstants.L1_TREE_LOWRIGHT_EDGE;
                    bossLayer1[treeX, treeY + 3] = ForestConstants.L1_EMPTY_BLOCK;
                    bossLayer1[treeX + 1, treeY + 3] = ForestConstants.L1_EMPTY_BLOCK;
                    bossLayer2[treeX, treeY + 3] = ForestConstants.L2_STUMP_LEFT;
                    bossLayer2[treeX + 1, treeY + 3] = ForestConstants.L2_STUMP_RIGHT;
                    treeXs.Add(treeX);
                    treeYs.Add(treeY);
                }
            }
            // flowers
            // perlin noise for grass and bush placement
            int perlinZ = (random.Next() % 2000);
            for (int y = 1; y < bossMapHeight; y++)
            {
                for (int x = 1; x < bossMapWidth; x++)
                {
                    double noise = PerlinNoise.noise(100 + x * 7 / (double)bossMapWidth, 100 + y * 7 / (double)bossMapHeight, perlinZ);
                    if (bossLayer1[x, y] == ForestConstants.L1_EMPTY && bossLayer2[x, y] == ForestConstants.L2_SHORT_GRASS && noise > 0.3)
                    {
                        bossLayer2[x, y] = ForestConstants.L2_FLOWER_A;
                    }
                    else if (bossLayer1[x, y] == ForestConstants.L1_EMPTY && bossLayer2[x, y] == ForestConstants.L2_SHORT_GRASS && noise > 0)
                    {
                        bossLayer2[x, y] = ForestConstants.L2_FLOWER_B;
                    }
                }
            }

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

            // set tileset; 2 for vanilla forest
            bossMap.mapData.mapHeader.setTileset16(2);
            bossMap.mapData.mapHeader.setTileset8(2);

            // display settings 1 for vanilla forest maps
            bossMap.mapData.mapHeader.setDisplaySettings(1);

            // layer 1 is bg, layer 2 is fg
            bossMap.mapData.mapHeader.setLayer1UsesForegroundTiles(false);
            bossMap.mapData.mapHeader.setLayer2UsesForegroundTiles(true);

            // npc palette 0xFF for hostile map
            bossMap.mapData.mapHeader.setNpcPalette(0xFF);

            // these have special uses for ancient cave, so we enable both everywhere
            bossMap.mapData.mapHeader.setFlammieEnabled(true);
            bossMap.mapData.mapHeader.setMagicRopeEnabled(true);

            // choose palette
            PaletteSetSource bossPal = outsideAndBossPalettes[random.Next() % outsideAndBossPalettes.Length];
            applyPaletteSource(context, bossMap, bossPal);

            return bossMap;
        }

        protected override byte[] getSongEventData()
        {
            // pre-pandora overworld theme
            return new byte[] { 0x40, 0x01, 0x0E, 0x1B, 0x8F };
        }

        protected override string getMapLogString(AncientCaveFloor floorData, AncientCaveGenerationContext generationContext)
        {
            String logString = "";
            logString += "[key]\n";
            logString += "! :   start\n";
            logString += "@ :   end\n";
            logString += "[]:   cave\n";
            logString += ". :   bush\n";
            logString += ", :   grass\n";
            logString += "= :   steps\n";
            logString += "& :   mountain\n";
            logString += "# :   trees\n";
            ProcGenMapNode startNode = generationContext.specialNodes["start"];
            ProcGenMapNode endNode = generationContext.specialNodes["end"];
            byte[,] layer1 = floorData.outsideMap.layer1Data;
            List<ProcGenMapNode> mapNodes = generationContext.sharedNodes;

            for (int y = 0; y < generationContext.outdoorMapHeight; y++)
            {
                logString += "\r\n";
                for (int x = 0; x < generationContext.outdoorMapWidth; x++)
                {
                    bool foundNode = false;
                    if (startNode.x == x && startNode.y == y)
                    {
                        logString += "!";
                        foundNode = true;
                    }
                    else if (endNode.x == x && endNode.y == y)
                    {
                        logString += "@";
                        foundNode = true;
                    }
                    else
                    {
                        for (int i = 0; i < mapNodes.Count; i++)
                        {
                            ProcGenMapNode n = mapNodes[i];
                            int index = i - 1;
                            if (n.x == x && n.y == y)
                            {
                                if (index < 10)
                                {
                                    logString += index;
                                }
                                else
                                {
                                    logString += (char)('A' + (index - 10));
                                }
                                foundNode = true;
                                break;
                            }
                        }
                    }
                    if (!foundNode)
                    {
                        if (layer1[x, y] == ForestConstants.L1_EMPTY)
                        {
                            logString += " ";
                        }
                        else if (layer1[x, y] == ForestConstants.L1_BUSH)
                        {
                            logString += ".";
                        }
                        else if (layer1[x, y] == ForestConstants.L1_CAVE_DOOR_LEFT)
                        {
                            logString += "[";
                        }
                        else if (layer1[x, y] == ForestConstants.L1_CAVE_DOOR_RIGHT)
                        {
                            logString += "]";
                        }
                        else if (layer1[x, y] == 103)
                        {
                            // steps
                            logString += "=";
                        }
                        else if (ForestConstants.isMountainTile(layer1[x, y]))
                        {
                            logString += "&";
                        }
                        else if (layer1[x, y] == ForestConstants.L1_TALL_GRASS ||
                            layer1[x, y] == ForestConstants.L1_TALL_GRASS_B ||
                            layer1[x, y] == ForestConstants.L1_TALL_GRASS_BL ||
                            layer1[x, y] == ForestConstants.L1_TALL_GRASS_BR ||
                            layer1[x, y] == ForestConstants.L1_TALL_GRASS_L ||
                            layer1[x, y] == ForestConstants.L1_TALL_GRASS_R ||
                            layer1[x, y] == ForestConstants.L1_TALL_GRASS_U ||
                            layer1[x, y] == ForestConstants.L1_TALL_GRASS_UL ||
                            layer1[x, y] == ForestConstants.L1_TALL_GRASS_UR)
                        {
                            logString += ",";
                        }
                        else
                        {
                            logString += "#";
                        }
                    }
                }
            }

            return logString;
        }

        protected override string getFloorType()
        {
            return "Forest";
        }
    }
}
