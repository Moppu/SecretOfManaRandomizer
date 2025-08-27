using System;
using System.Collections.Generic;
using SoMRandomizer.processing.ancientcave.mapgen.forest;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.ancientcave.mapgen.cave
{
    /// <summary>
    /// Ancient cave map generator for "cave" tileset.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CaveGenerator : AncientCaveMapGenerator
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

        const int numSolidNodes = 10;
        const int numNormalNodes = 30;

        const int solidNodeSize = 15;
        const int solidNodeSizeRandomness = 6;
        const int normalNodeSize = 4;
        const int normalNodeSizeRandomness = 4;

        const int pathSize = 3;
        const int pathSizeRandomness = 4;

        const string numSharedDoorsProperty = "SharedDoors";

        private PaletteSetSource[] palettes = new PaletteSetSource[]
        {
            // same palettes as ForestGenerator uses for interior areas
            new VanillaPaletteSetSource(69),
            new ResourcePaletteSetSource("accave.undinecave.bin", 69),
            new ResourcePaletteSetSource("accave.fierycave.bin", 69),
            new ResourcePaletteSetSource("accave.graycave.bin", 69),
        };

        protected override AncientCaveGenerationContext getGenerationContext(RandoContext context)
        {
            // no common nodes for cave tileset - outdoor map cave placement will determine the number of doors, which in turn determines
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

        private static ProcGenMapNode makeRandomOutdoorNode(Random random, Dictionary<string, int> nodeProperties)
        {
            ProcGenMapNode n = new ProcGenMapNode();
            n.values = nodeProperties;
            int nodeRadius = normalNodeSize + (random.Next() % normalNodeSizeRandomness);
            if (n.values.ContainsKey("solid"))
            {
                // make these bigger
                nodeRadius = solidNodeSize + (random.Next() % solidNodeSizeRandomness);
            }

            // up to 15 on each side + radius buffer around each
            int xRange = outdoorMapWidth - (20 + nodeRadius) * 2;
            // up to 20 on top, 5 on bottom for now + 5 buffer around each
            int yRange = outdoorMapHeight - 25 - 5 - (nodeRadius * 2);
            int yOffset = 25;

            if (n.values.ContainsKey("solid"))
            {
                // open space on top near the waterfalls
                yRange -= 20;
                yOffset += 20;
            }

            // center x,y of node
            n.x = 20 + nodeRadius + (random.Next() % xRange);
            n.y = yOffset + nodeRadius + (random.Next() % yRange);
            n.centerX = n.x;
            n.centerY = n.y;
            n.values.Add("radius", nodeRadius);
            return n;
        }

        protected override AncientCaveMap generateOutdoorMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            // most things
            byte[,] layer1 = new byte[outdoorMapWidth, outdoorMapHeight];
            // water
            byte[,] layer2 = new byte[outdoorMapWidth, outdoorMapHeight];

            AncientCaveMap outsideMap = new AncientCaveMap();
            outsideMap.layer1Data = layer1;
            outsideMap.layer2Data = layer2;

            // layer1
            // 5 = empty wall
            // 70 = normal floor
            // 82 = steps
            // ledge
            // 
            // [10][11][12]
            // [13][14][15]
            //  10  11  12
            //  13  14  15
            //  16? 17  18
            //  19  20  21
            //          ^ /
            //      ^ middle
            //  ^ \

            // ~ y == 10, waterfall ledge
            // ~ y == 30, border between wet floor & solid cave
            // ~ y >= 30, water nodes inside the wall
            // ~ y >= 10, floor nodes
            for (int y = 0; y < outdoorMapHeight; y++)
            {
                for (int x = 0; x < outdoorMapWidth; x++)
                {
                    // default: wet floor
                    layer1[x, y] = 43; // floor
                    layer2[x, y] = 22; // empty
                }
            }

            // nodes.  first make big empty nodes
            // then cut dry nodes into that + the water

            Random random = context.randomFunctional;

            for (int i = 0; i < numSolidNodes; i++)
            {
                ProcGenMapNode wallNode = makeRandomOutdoorNode(random, new Dictionary<string, int>{ { "solid", 1 } });
                // randomize radius and fill in spots between
                double radius = wallNode.values["radius"];
                double initialRadius = radius;
                for (int d = 0; d < 360; d++)
                {
                    double xPos = Math.Cos(d * Math.PI / 180.0);
                    double yPos = Math.Sin(d * Math.PI / 180.0);

                    for (double r = 0; r < radius; r += 0.1)
                    {
                        int xTile = (int)(wallNode.x + xPos * r);
                        int yTile = (int)(wallNode.y + yPos * r);
                        if (xTile >= 0 && xTile < outdoorMapWidth && yTile >= 0 && yTile < outdoorMapHeight)
                        {
                            layer1[xTile, yTile] = 5;
                            layer2[xTile, yTile] = 0;
                        }
                    }

                    double radiusChange = ((random.Next() % 10) - 10) / 20.0;
                    double newRadius = radius + radiusChange;
                    if (newRadius >= initialRadius - 3 && newRadius <= initialRadius + 3)
                    {
                        radius = newRadius;
                    }
                }
            }

            // now dry nodes
            // generate them and then connect them similar to how we do the forest ones
            // one of them is an end node, the startnode should be included, and they should be far away from each other
            List<ProcGenMapNode> allNodes = new List<ProcGenMapNode>();
            ProcGenMapNode startNode = makeRandomOutdoorNode(random, new Dictionary<string, int> { });
            startNode.name = "start";
            generationContext.specialNodes["start"] = startNode;
            outsideMap.entryPos = new XyPos(startNode.x, startNode.y);
            allNodes.Add(startNode);
            for (int i = 0; i < numNormalNodes; i++)
            {
                allNodes.Add(makeRandomOutdoorNode(random, new Dictionary<string, int> { }));
            }

            foreach (ProcGenMapNode node in allNodes)
            {
                // similar to what we do above for walls
                clearNodeArea(node, layer1, layer2, random, 3);
            }

            // connect nodes
            List<ProcGenMapNode> visitedNodes = new List<ProcGenMapNode>();
            List<ProcGenMapNode> unvisitedNodes = new List<ProcGenMapNode>();
            unvisitedNodes.AddRange(allNodes);

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

                // 5?
                int pathRadius = (random.Next() % pathSizeRandomness) + pathSize;
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
                                layer1[x, y] = 70;
                                layer2[x, y] = 0;
                            }
                        }
                    }

                    xPos += dx;
                    yPos += dy;
                }

                visitedNodes.Add(minDistDstNode);
                unvisitedNodes.Remove(minDistDstNode);
                minDistSrcNode.connections.Add(minDistDstNode);
                minDistDstNode.connections.Add(minDistSrcNode);
            }

            for (int x = 15; x < outdoorMapWidth - 15; x++)
            {
                int y = outdoorMapHeight - 2;
                while (y > 10)
                {
                    int num = 0;
                    int startY = y + 1;
                    while (y > 10 && layer1[x, y] == 5)
                    {
                        num++;
                        y--;
                    }
                    if (num > 0 && num < 7)
                    {
                        for (int _y = 0; _y < num; _y++)
                        {
                            layer1[x, _y + y] = layer1[x, startY];
                            layer2[x, _y + y] = layer2[x, startY];
                        }
                    }
                    if (num == 0)
                    {
                        y--;
                    }
                }
            }

            // add edges and shit to the basic cut-outs made above
            CaveTileEdges.addTileEdges(layer1, layer2, random, startNode);

            ////////////////////////////
            // upper border & waterfalls
            ////////////////////////////
            int waterfallLedgeY = 15;
            int wallDirection = 0;
            int leftWallStartY = 0;
            int rightWallStartY = 0;
            for (int x = 0; x < outdoorMapWidth; x++)
            {
                if (x == 10)
                {
                    leftWallStartY = waterfallLedgeY + 1;
                }
                if (x == outdoorMapWidth - 10)
                {
                    rightWallStartY = waterfallLedgeY + 1;
                }

                bool waterfall = (random.Next() % 4) > 0;
                int next = random.Next() % 4;
                // \ wall
                if (wallDirection == -1)
                {
                    layer1[x, waterfallLedgeY + 1] = 43;
                    layer1[x, waterfallLedgeY] = 19;
                    layer1[x, waterfallLedgeY - 1] = 16;
                    layer1[x, waterfallLedgeY - 2] = 13;
                    layer1[x, waterfallLedgeY - 3] = 10;
                    // waterfalls
                    layer2[x, waterfallLedgeY] = (byte)(waterfall ? 22 : 22);
                    layer2[x, waterfallLedgeY - 1] = (byte)(waterfall ? 12 : 33);
                    layer2[x, waterfallLedgeY - 2] = (byte)(waterfall ? 11 : 0);
                    layer2[x, waterfallLedgeY - 3] = (byte)(waterfall ? 11 : 0);

                    byte val = 13;
                    for (int y = waterfallLedgeY - 4; y >= 0; y--)
                    {
                        layer1[x, y] = val;
                        val = val == 13 ? (byte)10 : (byte)13;
                        layer2[x, y] = (byte)(waterfall ? 11 : 0);
                    }
                }

                // _ wall
                if (wallDirection == 0)
                {
                    layer1[x, waterfallLedgeY + 1] = 43;
                    layer1[x, waterfallLedgeY] = 20;
                    layer1[x, waterfallLedgeY - 1] = 17;
                    layer1[x, waterfallLedgeY - 2] = 14;
                    layer1[x, waterfallLedgeY - 3] = 11;
                    // waterfalls
                    layer2[x, waterfallLedgeY] = (byte)(waterfall ? 43 : 50);
                    layer2[x, waterfallLedgeY - 1] = (byte)(waterfall ? 27 : 0);
                    layer2[x, waterfallLedgeY - 2] = (byte)(waterfall ? 11 : 0);
                    layer2[x, waterfallLedgeY - 3] = (byte)(waterfall ? 11 : 0);
                    byte val = 14;
                    for (int y = waterfallLedgeY - 4; y >= 0; y--)
                    {
                        layer1[x, y] = val;
                        val = val == 14 ? (byte)11 : (byte)14;
                        layer2[x, y] = (byte)(waterfall ? 11 : 0);
                    }
                }

                // / wall
                if (wallDirection == 1)
                {
                    layer1[x, waterfallLedgeY + 2] = 43;
                    layer1[x, waterfallLedgeY + 1] = 21;
                    layer1[x, waterfallLedgeY] = 18;
                    layer1[x, waterfallLedgeY - 1] = 15;
                    layer1[x, waterfallLedgeY - 2] = 12;
                    // waterfalls
                    layer2[x, waterfallLedgeY + 1] = (byte)(waterfall ? 22 : 22);
                    layer2[x, waterfallLedgeY] = (byte)(waterfall ? 13 : 35);
                    layer2[x, waterfallLedgeY - 1] = (byte)(waterfall ? 11 : 0);
                    layer2[x, waterfallLedgeY - 2] = (byte)(waterfall ? 11 : 0);
                    byte val = 15;
                    for (int y = waterfallLedgeY - 3; y >= 0; y--)
                    {
                        layer1[x, y] = val;
                        val = val == 15 ? (byte)12 : (byte)15;
                        layer2[x, y] = (byte)(waterfall ? 11 : 0);
                    }
                }

                wallDirection = 0;
                if (next == 0 && waterfallLedgeY > 10)
                {
                    waterfallLedgeY--;
                    wallDirection = 1;
                }
                if (next == 1 && waterfallLedgeY < 20)
                {
                    waterfallLedgeY++;
                    wallDirection = -1;
                }

            }


            // left/right walls
            int wallX = 10;
            wallDirection = 0;
            for (int x = 0; x < wallX; x++)
            {
                layer1[x, leftWallStartY] = 2;
                layer2[x, leftWallStartY] = 0;
            }
            layer1[wallX, leftWallStartY] = 73;
            layer2[wallX, leftWallStartY] = 3;

            for (int y = leftWallStartY + 1; y < outdoorMapHeight; y++)
            {
                // top of wall: 2
                // corner off top wall: 73
                // right straight wall: 6
                // right / wall: 9, 12, 15, 18, 21
                // right \ wall: 3 if / within the last 4? or just if tile above is not 70, 73 otherwise
                // filler = 5
                int next = random.Next() % 4;

                for (int x = 0; x <= wallX; x++)
                {
                    layer1[x, y] = 5;
                    layer2[x, y] = 0;
                }

                if (wallDirection == -1)
                {
                    // away from edge of map
                    layer1[wallX, y] = 73;
                    layer2[wallX, y] = 3;
                }
                else if (wallDirection == 0)
                {
                    // straight wall
                    layer1[wallX, y] = 6;
                    layer2[wallX, y] = 0;
                }
                else if (wallDirection == 1)
                {
                    // toward edge of map
                    layer1[wallX + 1, y] = 9;
                    layer1[wallX, y] = 78;
                    layer2[wallX + 1, y] = 0;
                    layer2[wallX, y] = 0;
                    if (y < outdoorMapHeight - 5)
                    {
                        y++;
                        for (int x = 0; x <= wallX; x++)
                        {
                            layer1[x, y] = 5;
                            layer2[x, y] = 0;
                        }
                        layer1[wallX + 1, y] = 12;
                        layer1[wallX, y] = 6;
                        layer2[wallX + 1, y] = 0;
                        layer2[wallX, y] = 0;

                        y++;
                        for (int x = 0; x <= wallX; x++)
                        {
                            layer1[x, y] = 5;
                            layer2[x, y] = 0;
                        }
                        layer1[wallX + 1, y] = 15;
                        layer1[wallX, y] = 6;
                        layer2[wallX + 1, y] = 0;
                        layer2[wallX, y] = 0;

                        y++;
                        for (int x = 0; x <= wallX; x++)
                        {
                            layer1[x, y] = 5;
                            layer2[x, y] = 0;
                        }
                        layer1[wallX + 1, y] = 18;
                        layer1[wallX, y] = 6;
                        layer2[wallX + 1, y] = 35;
                        layer2[wallX, y] = 0;

                        y++;
                        for (int x = 0; x <= wallX; x++)
                        {
                            layer1[x, y] = 5;
                            layer2[x, y] = 0;
                        }
                        layer1[wallX + 1, y] = 21;
                        layer1[wallX, y] = 6;
                        layer2[wallX + 1, y] = 22;
                        layer2[wallX, y] = 0;
                    }
                }

                wallDirection = 0;
                if (next == 0 && wallX > 5)
                {
                    wallX--;
                    wallDirection = 1;
                }
                if (next == 1 && wallX < 15)
                {
                    wallX++;
                    wallDirection = -1;
                }
            }

            wallX = outdoorMapWidth - 10;
            wallDirection = 0;
            for (int x = wallX + 1; x < outdoorMapWidth; x++)
            {
                layer1[x, rightWallStartY] = 2;
                layer2[x, rightWallStartY] = 0;
            }
            layer1[wallX, rightWallStartY] = 71;
            layer2[wallX, rightWallStartY] = 1;

            for (int y = rightWallStartY + 1; y < outdoorMapHeight; y++)
            {
                // top of wall: 2
                // corner off top wall: 71
                // right straight wall: 4
                // right \ wall: 7, 10, 13, 16, 19
                // right / wall: 1 if / within the last 4? or just if tile above is not 70, 71 otherwise
                // filler = 5
                int next = random.Next() % 4;

                for (int x = wallX; x < outdoorMapWidth; x++)
                {
                    layer1[x, y] = 5;
                    layer2[x, y] = 0;
                }

                if (wallDirection == -1)
                {
                    // away from edge of map
                    layer1[wallX, y] = 71;
                    layer2[wallX, y] = 1;
                }
                else if (wallDirection == 0)
                {
                    // straight wall
                    layer1[wallX, y] = 4;
                    layer2[wallX, y] = 0;
                }
                else if (wallDirection == 1)
                {
                    // toward edge of map
                    layer1[wallX - 1, y] = 7;
                    layer1[wallX, y] = 79;
                    layer2[wallX - 1, y] = 0;
                    layer2[wallX, y] = 0;
                    if (y < outdoorMapHeight - 5)
                    {
                        y++;
                        for (int x = wallX; x < outdoorMapWidth; x++)
                        {
                            layer1[x, y] = 5;
                            layer2[x, y] = 0;
                        }
                        layer1[wallX - 1, y] = 10;
                        layer1[wallX, y] = 4;
                        layer2[wallX - 1, y] = 0;
                        layer2[wallX, y] = 0;

                        y++;
                        for (int x = wallX; x < outdoorMapWidth; x++)
                        {
                            layer1[x, y] = 5;
                            layer2[x, y] = 0;
                        }
                        layer1[wallX - 1, y] = 13;
                        layer1[wallX, y] = 4;
                        layer2[wallX - 1, y] = 0;
                        layer2[wallX, y] = 0;

                        y++;
                        for (int x = wallX; x < outdoorMapWidth; x++)
                        {
                            layer1[x, y] = 5;
                            layer2[x, y] = 0;
                        }
                        layer1[wallX - 1, y] = 16;
                        layer1[wallX, y] = 4;
                        layer2[wallX - 1, y] = 33;
                        layer2[wallX, y] = 0;

                        y++;
                        for (int x = wallX; x < outdoorMapWidth; x++)
                        {
                            layer1[x, y] = 5;
                            layer2[x, y] = 0;
                        }
                        layer1[wallX - 1, y] = 19;
                        layer1[wallX, y] = 4;
                        layer2[wallX - 1, y] = 22;
                        layer2[wallX, y] = 0;
                    }
                }

                wallDirection = 0;
                if (next == 0 && wallX > outdoorMapWidth - 15)
                {
                    wallX--;
                    wallDirection = -1;
                }
                if (next == 1 && wallX < outdoorMapWidth - 5)
                {
                    wallX++;
                    wallDirection = 1;
                }
            }

            // bottom wall
            for (int x = 0; x < outdoorMapWidth - 1; x++)
            {
                if (layer1[x, outdoorMapHeight - 1] == 43)
                {
                    layer1[x, outdoorMapHeight - 1] = 68;
                }
            }

            // -1 unreachable, 0 not checked, 1 reachable
            int[,] waterReachable = new int[outdoorMapWidth, outdoorMapHeight];
            List<int> xCheck = new List<int>();
            List<int> yCheck = new List<int>();
            xCheck.Add(outdoorMapWidth / 2);
            int yc = 0;
            while (yc < 40)
            {
                if (layer1[outdoorMapWidth / 2, yc] == 43)
                {
                    waterReachable[outdoorMapWidth / 2, yc] = 1;
                    break;
                }
                yc++;
            }
            if (yc < 40)
            {
                yCheck.Add(yc);
                while (xCheck.Count > 0)
                {
                    int xche = xCheck[0];
                    int yche = yCheck[0];
                    xCheck.RemoveAt(0);
                    yCheck.RemoveAt(0);
                    if (xche > 0)
                    {
                        // left
                        if (layer1[xche - 1, yche] == 43)
                        {
                            if (waterReachable[xche - 1, yche] == 0)
                            {
                                waterReachable[xche - 1, yche] = 1;
                                xCheck.Add(xche - 1);
                                yCheck.Add(yche);
                            }
                        }
                        else
                        {
                            waterReachable[xche - 1, yche] = -1;
                        }
                    }
                    if (xche < outdoorMapWidth - 1)
                    {
                        // right
                        if (layer1[xche + 1, yche] == 43)
                        {
                            if (waterReachable[xche + 1, yche] == 0)
                            {
                                waterReachable[xche + 1, yche] = 1;
                                xCheck.Add(xche + 1);
                                yCheck.Add(yche);
                            }
                        }
                        else
                        {
                            waterReachable[xche + 1, yche] = -1;
                        }
                    }
                    if (yche > 0)
                    {
                        // top
                        if (layer1[xche, yche - 1] == 43)
                        {
                            if (waterReachable[xche, yche - 1] == 0)
                            {
                                waterReachable[xche, yche - 1] = 1;
                                xCheck.Add(xche);
                                yCheck.Add(yche - 1);
                            }
                        }
                        else
                        {
                            waterReachable[xche, yche - 1] = -1;
                        }
                    }
                    if (yche < outdoorMapHeight - 1)
                    {
                        // top
                        if (layer1[xche, yche + 1] == 43)
                        {
                            if (waterReachable[xche, yche + 1] == 0)
                            {
                                waterReachable[xche, yche + 1] = 1;
                                xCheck.Add(xche);
                                yCheck.Add(yche + 1);
                            }
                        }
                        else
                        {
                            waterReachable[xche, yche + 1] = -1;
                        }
                    }
                }
            }

            List<int> xPossibleSteps = new List<int>();
            List<int> yPossibleSteps = new List<int>();
            for (int y = 1; y < outdoorMapHeight - 1; y++)
            {
                for (int x = 1; x < outdoorMapWidth - 1; x++)
                {
                    if (layer1[x, y] == 68 && layer1[x, y + 1] == 17 && layer1[x, y + 2] == 20 && layer1[x, y + 3] == 43)
                    {
                        if (waterReachable[x, y + 3] == 1)
                        {
                            xPossibleSteps.Add(x);
                            yPossibleSteps.Add(y);
                        }
                    }
                }
            }

            if (xPossibleSteps.Count > 0)
            {
                for (int i = 0; i < 10 && i < xPossibleSteps.Count; i++)
                {
                    int index = random.Next() % xPossibleSteps.Count;
                    int x = xPossibleSteps[index];
                    int y = yPossibleSteps[index];
                    layer1[x, y + 1] = 91;
                    layer1[x, y + 2] = 91;
                }
            }

            int startSearchY = 40;
            if (xPossibleSteps.Count > 0)
            {
                startSearchY = 2;
            }

            List<int> doorPossibleX = new List<int>();
            List<int> doorPossibleY = new List<int>();
            for (int y = startSearchY; y < outdoorMapHeight - 1; y++)
            {
                for (int x = 16; x < outdoorMapWidth - 16; x++)
                {
                    if (layer1[x, y] == 20 && layer1[x + 1, y] == 20 &&
                        layer1[x, y - 1] == 17 && layer1[x + 1, y - 1] == 17 &&
                        layer1[x, y - 2] == 14 && layer1[x + 1, y - 2] == 14)
                    {
                        bool tooClose = false;
                        for (int i = 0; i < doorPossibleX.Count; i++)
                        {
                            if (Math.Abs(x - doorPossibleX[i]) <= 2 && y == doorPossibleY[i])
                            {
                                tooClose = true;
                            }
                        }
                        if (!tooClose)
                        {
                            doorPossibleX.Add(x);
                            doorPossibleY.Add(y);
                        }
                    }
                }
            }

            int numDoors = 0;
            while (doorPossibleX.Count > 0 && numDoors < 8)
            {
                int index = random.Next() % doorPossibleX.Count;
                int doorX = doorPossibleX[index];
                int doorY = doorPossibleY[index];
                doorPossibleX.RemoveAt(index);
                doorPossibleY.RemoveAt(index);
                // 116 and 100 are both door tiles
                layer1[doorX, doorY] = 116;
                layer1[doorX + 1, doorY] = 100;
                layer1[doorX, doorY - 1] = 99;
                layer1[doorX + 1, doorY - 1] = 99;
                layer1[doorX, doorY - 2] = 107;
                layer1[doorX + 1, doorY - 2] = 107;
                // make doors
                outsideMap.altMapEntryLocations[numDoors] = new XyPos(doorX, doorY + 1);
                outsideMap.altMapExitLocations[new XyPos(doorX, doorY)] = numDoors;
                outsideMap.altMapExitLocations[new XyPos(doorX + 1, doorY)] = numDoors;
                numDoors++;
            }

            // exit at the farthest non-solid node away from start node
            int maxDist = 0;
            ProcGenMapNode furthestNode = null;
            foreach (ProcGenMapNode n in allNodes)
            {
                if (!n.values.ContainsKey("solid"))
                {
                    int xDiff = Math.Abs(n.x - startNode.x);
                    int yDiff = Math.Abs(n.y - startNode.y);
                    int totalDist = (int)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
                    if (totalDist > maxDist)
                    {
                        maxDist = totalDist;
                        furthestNode = n;
                    }
                }
            }

            generationContext.specialNodes["end"] = furthestNode;

            // 168 around, 72 as warper w/ 144 on L2
            layer1[furthestNode.x, furthestNode.y] = 97;
            layer2[furthestNode.x, furthestNode.y] = 62;

            layer1[furthestNode.x + 1, furthestNode.y] = 168;
            layer1[furthestNode.x - 1, furthestNode.y] = 168;
            layer1[furthestNode.x, furthestNode.y + 1] = 168;
            layer1[furthestNode.x, furthestNode.y - 1] = 168;

            // create exit location
            outsideMap.exitLocations.Add(new XyPos((short)furthestNode.x, (short)furthestNode.y));

            // create map header
            outsideMap.mapData = new FullMap();
            outsideMap.mapData.mapHeader = new MapHeader();
            outsideMap.mapData.mapHeader.setWalkonEventEnabled(true);

            // set tileset; 10 for vanilla cave
            outsideMap.mapData.mapHeader.setTileset16(10);
            outsideMap.mapData.mapHeader.setTileset8(10);
            // make the grass blow around
            outsideMap.mapData.mapHeader.setPaletteAnimated(true);

            // display settings 1 for vanilla cave maps
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
            PaletteSetSource outsidePal = palettes[random.Next() % palettes.Length];
            applyPaletteSource(context, outsideMap, outsidePal);

            // so the exit door animates
            outsideMap.mapData.mapHeader.setPaletteAnimated(true);

            // create enemies
            outsideMap.mapData.mapObjects.AddRange(makeOutdoorEnemies(startNode, random, layer1));

            // tell the inside map how many rooms it should be making based on how many we were able to fit into the ledges
            generationContext.sharedSettings.setInt(numSharedDoorsProperty, numDoors);

            // tell the logging where the nodes ended up
            generationContext.sharedNodes.AddRange(allNodes);

            return outsideMap;
        }

        private List<MapObject> makeOutdoorEnemies(ProcGenMapNode startNode, Random random, byte[,] layer1MapData)
        {
            List<MapObject> enemies = new List<MapObject>();
            byte[] waterEnemyIds = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, // a few rabites
                0x05, // iffish
                0x0C, // waterthug
                0x11, // icethug
                0x18, // crab
            };
            byte[] landEnemyIds = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, // a few rabites
                0x06, // goblin
                0x07, // eye spy
                0x08, // greendrop
                0x0A, // blat
                0x0B, // goblin
                0x0E, // ma goblin
            };
            // 16 in water, 16 on land
            int numWaterEnemies = 0;
            int numLandEnemies = 0;
            while (numWaterEnemies < 16 || numLandEnemies < 16)
            {
                int rx = random.Next() % outdoorMapWidth;
                int ry = random.Next() % outdoorMapHeight;
                int xdiff = Math.Abs(rx - startNode.x);
                int ydiff = Math.Abs(ry - startNode.y);
                if (xdiff > 10 && ydiff > 10)
                {
                    if (layer1MapData[rx, ry] == 43 && numWaterEnemies < 16)
                    {
                        byte type = waterEnemyIds[random.Next() % waterEnemyIds.Length];
                        MapObject enemy = new MapObject();
                        enemy.setXpos((byte)rx);
                        enemy.setYpos((byte)ry);
                        enemy.setSpecies(type);
                        enemy.setEventVisFlag(0x00);
                        enemy.setEventVisMinimum(0x00);
                        enemy.setEventVisMaximum(0x0F);
                        enemy.setEvent(0x01);
                        enemy.setDirection(MapObject.DIR_LEFT);
                        enemy.setUnknownB7(0x18);
                        enemies.Add(enemy);
                        numWaterEnemies++;
                    }
                    if (layer1MapData[rx, ry] == 70 && numLandEnemies < 16)
                    {
                        byte type = landEnemyIds[random.Next() % landEnemyIds.Length];
                        MapObject enemy = new MapObject();
                        enemy.setXpos((byte)rx);
                        enemy.setYpos((byte)ry);
                        enemy.setSpecies(type);
                        enemy.setEventVisFlag(0x00);
                        enemy.setEventVisMinimum(0x00);
                        enemy.setEventVisMaximum(0x0F);
                        enemy.setEvent(0x01);
                        enemy.setDirection(MapObject.DIR_LEFT);
                        enemy.setUnknownB7(0x18);
                        enemies.Add(enemy);
                        numLandEnemies++;
                    }
                }
            }
            return enemies;
        }

        private static void clearNodeArea(ProcGenMapNode node, byte[,] layer1, byte[,] layer2, Random random, int radialNoise)
        {
            double radius = node.values["radius"];
            double initialRadius = node.values["radius"];
            int noiseSeed1 = random.Next() % 5000;
            for (int d = 0; d < 360; d++)
            {
                double xPos = Math.Cos(d * Math.PI / 180.0);
                double yPos = Math.Sin(d * Math.PI / 180.0);

                radius = initialRadius + PerlinNoise.noise(noiseSeed1, xPos * 10, yPos * 10) * radialNoise;
                for (double r = 0; r < radius; r += 0.1)
                {
                    int xTile = (int)(node.x + xPos * r);
                    int yTile = (int)(node.y + yPos * r);
                    if (xTile >= 0 && xTile < outdoorMapWidth && yTile >= 0 && yTile < outdoorMapHeight)
                    {
                        layer1[xTile, yTile] = 70;
                        layer2[xTile, yTile] = 0;
                    }
                }
            }
        }

        protected override AncientCaveMap generateIndoorMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            // second map
            // identical to ForestGenerator.generateIndoorMap since they both use cave interiors
            Random random = context.randomFunctional;
            AncientCaveMap insideMap = new AncientCaveMap();
            insideMap.mapData = new FullMap();
            byte[,] indoorLayer1 = new byte[indoorMapWidth, indoorMapHeight];
            byte[,] indoorLayer2 = new byte[indoorMapWidth, indoorMapHeight];
            int numSharedDoors = generationContext.sharedSettings.getInt(numSharedDoorsProperty);
            // this makes doors too
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
            PaletteSetSource insidePal = palettes[random.Next() % palettes.Length];
            applyPaletteSource(context, insideMap, insidePal);

            return insideMap;
        }

        protected override AncientCaveMap generateBossMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            byte bossPlayerX = 16;
            byte bossPlayerY = 20;

            byte bossX = 16;
            byte bossY = 14;

            byte[,] bossLayer1 = new byte[bossMapWidth, bossMapHeight];
            byte[,] bossLayer2 = new byte[bossMapWidth, bossMapHeight];

            for (int y = 0; y < bossMapHeight; y++)
            {
                for (int x = 0; x < bossMapWidth; x++)
                {
                    // default: wet floor
                    bossLayer1[x, y] = 43; // floor
                    bossLayer2[x, y] = 22; // empty
                }
            }

            Random random = context.randomFunctional;
            ProcGenMapNode bossNode = new ProcGenMapNode();
            bossNode.centerX = 16;
            bossNode.x = 16;
            bossNode.centerY = 16;
            bossNode.y = 16;
            bossNode.values["radius"] = 10;
            clearNodeArea(bossNode, bossLayer1, bossLayer2, random, 6);

            // post-process edges the same way as above for the outdoor map
            CaveTileEdges.addTileEdges(bossLayer1, bossLayer2, random, bossNode);

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

            // set tileset; 10 for vanilla cave
            bossMap.mapData.mapHeader.setTileset16(10);
            bossMap.mapData.mapHeader.setTileset8(10);

            // display settings 1 for vanilla cave maps
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
            PaletteSetSource bossPal = palettes[random.Next() % palettes.Length];
            applyPaletteSource(context, bossMap, bossPal);

            return bossMap;
        }

        protected override string getMapLogString(AncientCaveFloor floorData, AncientCaveGenerationContext generationContext)
        {
            string logString = "";
            logString += "[key]\n";
            logString += "! :   start\n";
            logString += "@ :   end\n";
            logString += "[]:   cave\n";
            logString += ". :   rubble\n";
            logString += "^ :   rock\n";
            logString += "= :   steps\n";
            logString += "w :   water\n";
            logString += "# :   wall\n";
            ProcGenMapNode startNode = generationContext.specialNodes["start"];
            ProcGenMapNode endNode = generationContext.specialNodes["end"];
            byte[,] layer1 = floorData.outsideMap.layer1Data;
            List<ProcGenMapNode> allNodes = generationContext.sharedNodes;

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
                        for (int i = 0; i < allNodes.Count; i++)
                        {
                            ProcGenMapNode n = allNodes[i];
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
                        if (layer1[x, y] == 43)
                        {
                            logString += "w";
                        }
                        else if (layer1[x, y] == 70)
                        {
                            logString += " ";
                        }
                        else if (layer1[x, y] == 91)
                        {
                            logString += "=";
                        }
                        else if (layer1[x, y] == 168)
                        {
                            logString += "^";
                        }
                        else if (layer1[x, y] == 116)
                        {
                            logString += "[";
                        }
                        else if (layer1[x, y] == 100)
                        {
                            logString += "]";
                        }
                        else if (layer1[x, y] == 29 || layer1[x, y] == 45)
                        {
                            logString += ".";
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

        protected override byte[] getSongEventData()
        {
            // vanilla potos-area field/cave music
            return new byte[] { 0x40, 0x01, 0x0E, 0x1B, 0x8F };
        }

        protected override string getFloorType()
        {
            return "Cave";
        }
    }
}
