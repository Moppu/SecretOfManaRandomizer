using System;
using System.Collections.Generic;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.ancientcave.mapgen.island
{
    // MOPPLE: these kind of take fucking forever to generate.  might want to look into improving them
    /// <summary>
    /// Ancient cave map generator for "island" tileset.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class IslandGenerator : AncientCaveMapGenerator
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

        public const string numSharedDoorsProperty = "SharedDoors";

        private PaletteSetSource[] outsideAndBossPalettes = new PaletteSetSource[]
        {
            new VanillaPaletteSetSource(61),
            new ResourcePaletteSetSource("acisland.greenwater.bin", 61),
            new ResourcePaletteSetSource("acisland.night.bin", 61),
            new ResourcePaletteSetSource("acisland.alt.bin", 61),
        };

        private PaletteSetSource[] insidePalettes = new PaletteSetSource[]
        {
            new VanillaPaletteSetSource(30),
            // MOPPLE: should probably add some more palette variations here.
        };

        protected override AncientCaveGenerationContext getGenerationContext(RandoContext context)
        {
            AncientCaveGenerationContext generationContext = new AncientCaveGenerationContext();
            generationContext.outdoorMapWidth = outdoorMapWidth;
            generationContext.outdoorMapHeight = outdoorMapHeight;
            generationContext.indoorMapWidth = indoorMapWidth;
            generationContext.indoorMapHeight = indoorMapHeight;
            generationContext.bossMapWidth = bossMapWidth;
            generationContext.bossMapHeight = bossMapHeight;
            return generationContext;
        }

        private static ProcGenMapNode makeRandomOutdoorNode(Random random)
        {
            ProcGenMapNode n = new ProcGenMapNode();
            n.x = (random.Next() % (outdoorMapWidth));
            n.y = (random.Next() % (outdoorMapHeight));
            n.values["width"] = (random.Next() % 10) + 8;
            n.values["height"] = (random.Next() % 10) + 8;
            n.centerX = DataUtil.clampToPositiveRange(n.x + n.values["width"] / 2, outdoorMapWidth);
            n.centerY = DataUtil.clampToPositiveRange(n.y + n.values["height"] / 2, outdoorMapHeight);
            n.values["cornersize"] = (random.Next() % 3) + 2;
            if (n.values["cornersize"] > n.values["width"] / 2 - 3)
            {
                n.values["cornersize"] = n.values["width"] / 2 - 3;
            }
            if (n.values["cornersize"] > n.values["height"] / 2 - 3)
            {
                n.values["cornersize"] = n.values["height"] / 2 - 3;
            }
            return n;
        }

        protected override AncientCaveMap generateOutdoorMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            // MOPPLE: it would probably be nice to have an equivalent of ForestConstants for all the other tilesets too,
            // so there aren't just all these tile numbers thrown around.
            byte[,] layer1 = new byte[outdoorMapWidth, outdoorMapHeight];
            byte[,] layer2 = new byte[outdoorMapWidth, outdoorMapHeight];

            for (int y = 0; y < outdoorMapHeight; y++)
            {
                for (int x = 0; x < outdoorMapWidth; x++)
                {
                    layer1[x, y] = 171;
                    layer2[x, y] = 139;
                }
            }

            AncientCaveMap outsideMap = new AncientCaveMap();
            outsideMap.layer1Data = layer1;
            outsideMap.layer2Data = layer2;

            Random random = context.randomFunctional;

            // make start node
            ProcGenMapNode startNode = makeRandomOutdoorNode(random);
            startNode.name = "start";
            generationContext.specialNodes["start"] = startNode;
            List<ProcGenMapNode> mapNodes = new List<ProcGenMapNode>();
            mapNodes.Add(startNode);
            outsideMap.entryPos = new XyPos(startNode.centerX, startNode.centerY);

            // make end node, away from the start node (noting that the map loops)
            ProcGenMapNode endNode = makeRandomOutdoorNode(random);
            endNode.name = "end";
            generationContext.specialNodes["end"] = endNode;
            endNode.x = DataUtil.clampToPositiveRange(startNode.x + outdoorMapWidth / 2 + (random.Next() % 20) - 10, outdoorMapWidth);
            endNode.y = DataUtil.clampToPositiveRange(startNode.y + outdoorMapHeight / 2 + (random.Next() % 20) - 10, outdoorMapHeight);
            endNode.centerX = DataUtil.clampToPositiveRange(endNode.x + endNode.values["width"] / 2, outdoorMapWidth);
            endNode.centerY = DataUtil.clampToPositiveRange(endNode.y + endNode.values["height"] / 2, outdoorMapHeight);

            mapNodes.Add(endNode);
            bool done = false;
            int nodeNum = 0;
            bool[,] overlapCheck = new bool[outdoorMapWidth, outdoorMapHeight];
            int overlapBuffer = 1;
            int numArrows = 0;
            while (!done)
            {
                bool foundPos = false;
                ProcGenMapNode thisNode = null;
                int iters = 0;
                while (!foundPos)
                {
                    foundPos = true;
                    thisNode = makeRandomOutdoorNode(random);
                    thisNode.values.Remove("arrow");
                    // if near directly one direction from end node, sometimes make it an arrow node.  these will ignore the random size and be small
                    int xdiff = Math.Abs(thisNode.centerX - endNode.centerX);
                    if (xdiff > outdoorMapWidth / 2)
                    {
                        xdiff = outdoorMapWidth - xdiff;
                    }
                    int ydiff = Math.Abs(thisNode.centerY - endNode.centerY);
                    if (ydiff > outdoorMapHeight / 2)
                    {
                        ydiff = outdoorMapHeight - ydiff;
                    }

                    if (xdiff < 5 && ydiff > 20)
                    {
                        if (numArrows == 0 || (random.Next() % numArrows) == 0)
                        {
                            // vertical arrow
                            bool downArrow = true;
                            int yd = DataUtil.clampToPositiveRange(endNode.centerY - thisNode.centerY, outdoorMapHeight);
                            if (yd >= outdoorMapHeight / 2)
                            {
                                downArrow = false;
                            }
                            thisNode.values["width"] = 7;
                            thisNode.values["height"] = 7;
                            thisNode.x = thisNode.centerX - 3;
                            thisNode.y = thisNode.centerY - 3;
                            thisNode.values["arrow"] = downArrow ? 0 : 1;
                        }
                    }
                    else if (ydiff < 5 && xdiff > 20)
                    {
                        if (numArrows == 0 || (random.Next() % numArrows) == 0)
                        {
                            // horizontal arrow
                            bool rightArrow = true;
                            int xd = DataUtil.clampToPositiveRange(endNode.centerX - thisNode.centerX, outdoorMapWidth);
                            // ----------------
                            //  t e            -->
                            // t            e  <--
                            //      t  e       -->
                            //
                            if (xd >= outdoorMapWidth / 2)
                            {
                                rightArrow = false;
                            }
                            thisNode.values["width"] = 7;
                            thisNode.values["height"] = 7;
                            thisNode.x = thisNode.centerX - 3;
                            thisNode.y = thisNode.centerY - 3;
                            thisNode.values["arrow"] = rightArrow ? 2 : 3;
                        }
                    }
                    thisNode.name = "n" + nodeNum;
                    String nodeCharName;
                    if (nodeNum < 10)
                    {
                        nodeCharName = "" + nodeNum;
                    }
                    else
                    {
                        nodeCharName = "" + (char)('A' + (nodeNum - 10));
                    }

                    for (int y = 0; y < outdoorMapHeight; y++)
                    {
                        for (int x = 0; x < outdoorMapWidth; x++)
                        {
                            overlapCheck[x, y] = false;
                        }
                    }

                    for (int y = thisNode.y - overlapBuffer; y < thisNode.y + thisNode.values["height"] + overlapBuffer; y++)
                    {
                        int _y = DataUtil.clampToPositiveRange(y, outdoorMapWidth);
                        for (int x = thisNode.x - overlapBuffer; x < thisNode.x + thisNode.values["width"] + overlapBuffer; x++)
                        {
                            int _x = DataUtil.clampToPositiveRange(x, outdoorMapWidth);
                            overlapCheck[_x, _y] = true;
                        }
                    }

                    foreach (ProcGenMapNode n in mapNodes)
                    {
                        for (int y = n.y - overlapBuffer; foundPos && y < n.y + n.values["height"] + overlapBuffer; y++)
                        {
                            int _y = DataUtil.clampToPositiveRange(y, outdoorMapWidth);
                            for (int x = n.x - overlapBuffer; foundPos && x < n.x + n.values["width"] + overlapBuffer; x++)
                            {
                                int _x = DataUtil.clampToPositiveRange(x, outdoorMapWidth);
                                if (overlapCheck[_x, _y])
                                {
                                    foundPos = false;
                                }
                            }

                        }

                        if (!foundPos)
                        {
                            break;
                        }
                    }
                    iters++;
                    if (iters > 1000)
                    {
                        done = true;
                        break;
                    }
                }

                if (!done)
                {
                    mapNodes.Add(thisNode);
                    if (thisNode.values.ContainsKey("arrow"))
                    {
                        numArrows++;
                    }
                    String ls = "";
                    if (nodeNum < 10)
                    {
                        ls = "" + nodeNum;
                    }
                    else
                    {
                        ls = "" + (char)('A' + (nodeNum - 10));
                    }
                    nodeNum++;
                }
            }

            // add rounded corners to islands
            foreach (ProcGenMapNode n in mapNodes)
            {
                if (!n.values.ContainsKey("arrow"))
                {
                    List<int> corners = new List<int>();
                    for (int i = 0; i < 4; i++)
                    {
                        if ((random.Next() % 4) != 0)
                        {
                            corners.Add(i);
                        }
                    }
                    int height = n.values["height"];
                    int width = n.values["width"];
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int xDist = x > width / 2 ? width - x - 1 : x;
                            int yDist = y > height / 2 ? height - y - 1 : y;
                            int cornerIndex = 0;
                            if (x > width / 2)
                            {
                                cornerIndex += 1;
                            }
                            if (y > height / 2)
                            {
                                cornerIndex += 2;
                            }
                            int mapX = DataUtil.clampToPositiveRange(n.x + x, outdoorMapWidth);
                            int mapY = DataUtil.clampToPositiveRange(n.y + y, outdoorMapHeight);
                            int cornerSize = n.values["cornersize"];
                            if (!corners.Contains(cornerIndex))
                            {
                                cornerSize -= 2;
                            }
                            if (xDist + yDist >= cornerSize)
                            {
                                layer1[mapX, mapY] = 148;
                                layer2[mapX, mapY] = 0;
                            }
                        }
                    }
                }
            }

            // add corner/edge graphic tiles, based on position of existing grass tiles
            byte[,] copy1 = new byte[outdoorMapWidth, outdoorMapHeight];
            byte[,] copy2 = new byte[outdoorMapWidth, outdoorMapHeight];
            for (int y = 0; y < outdoorMapHeight; y++)
            {
                for (int x = 0; x < outdoorMapWidth; x++)
                {
                    copy1[x, y] = layer1[x, y];
                    copy2[x, y] = layer2[x, y];
                }
            }

            for (int y = 0; y < outdoorMapHeight; y++)
            {
                for (int x = 0; x < outdoorMapWidth; x++)
                {
                    int leftX = DataUtil.clampToPositiveRange(x - 1, outdoorMapWidth);
                    int rightX = DataUtil.clampToPositiveRange(x + 1, outdoorMapWidth);
                    int upY = DataUtil.clampToPositiveRange(y - 1, outdoorMapHeight);
                    int downY = DataUtil.clampToPositiveRange(y + 1, outdoorMapHeight);
                    if (copy1[x, y] == 148 && copy1[leftX, y] == 171 && copy1[x, upY] == 171)
                    {
                        // upper left corner
                        layer1[x, y] = 85;
                        layer2[x, y] = 136;
                    }
                    else if (copy1[x, y] == 148 && copy1[rightX, y] == 171 && copy1[x, upY] == 171)
                    {
                        // upper right corner
                        layer1[x, y] = 88;
                        layer2[x, y] = 135;
                    }
                    else if (copy1[x, y] == 148 && copy1[rightX, y] == 171 && copy1[x, downY] == 171)
                    {
                        // lower right corner
                        layer1[x, y] = 100;
                        layer2[x, y] = 122;
                    }
                    else if (copy1[x, y] == 148 && copy1[leftX, y] == 171 && copy1[x, downY] == 171)
                    {
                        // lower left corner
                        layer1[x, y] = 96;
                        layer2[x, y] = 123;
                    }
                    else if (copy1[x, y] == 148 && copy1[leftX, y] == 171)
                    {
                        // left edge
                        layer1[x, y] = 89;
                        layer2[x, y] = 141;
                    }
                    else if (copy1[x, y] == 148 && copy1[rightX, y] == 171)
                    {
                        // right edge
                        layer1[x, y] = 90;
                        layer2[x, y] = 140;
                    }
                    else if (copy1[x, y] == 148 && copy1[x, upY] == 171)
                    {
                        // up edge
                        layer1[x, y] = 82;
                        layer2[x, y] = 143;
                    }
                    else if (copy1[x, y] == 148 && copy1[x, downY] == 171)
                    {
                        // down edge
                        layer1[x, upY] = 98;
                        layer2[x, y] = 142;
                    }
                }
            }

            for (int y = 0; y < outdoorMapHeight; y++)
            {
                for (int x = 0; x < outdoorMapWidth; x++)
                {
                    copy1[x, y] = layer1[x, y];
                    copy2[x, y] = layer2[x, y];
                }
            }

            for (int y = 0; y < outdoorMapHeight; y++)
            {
                for (int x = 0; x < outdoorMapWidth; x++)
                {
                    int leftX = x == 0 ? outdoorMapWidth - 1 : x - 1;
                    int rightX = x == outdoorMapWidth - 1 ? 0 : x + 1;
                    int upY = y == 0 ? outdoorMapHeight - 1 : y - 1;
                    int downY = y == outdoorMapHeight - 1 ? 0 : y + 1;
                    if (copy1[x, y] == 98)
                    {
                        layer1[x, downY] = 102;
                    }
                    if (copy1[rightX, y] == 100 && copy1[x, upY] != 98)
                    {
                        if (copy1[leftX, y] != 98)
                        {
                            layer1[leftX, y] = 93;
                        }
                        layer1[x, y] = 99;
                        layer2[x, y] = 121;
                    }
                    if (copy1[leftX, y] == 96 && copy1[x, upY] != 98)
                    {
                        if (copy1[rightX, y] != 98)
                        {
                            layer1[rightX, y] = 92;
                        }
                        layer1[x, y] = 97;
                        layer2[x, y] = 124;
                    }

                    if (copy1[x, y] == 85 && copy1[rightX, y] != 82)
                    {
                        layer1[rightX, y] = 86;
                        layer2[rightX, y] = 137;
                    }
                    if (copy1[x, y] == 88 && copy1[leftX, y] != 82)
                    {
                        layer1[leftX, y] = 87;
                        layer2[leftX, y] = 127;
                    }
                    if (copy1[x, y] == 90 && copy1[x, downY] == 100)
                    {
                        layer1[x, y] = 94;
                        layer2[x, y] = 190;
                        layer1[leftX, y] = 93;
                    }
                    if (copy1[x, y] == 89 && copy1[x, downY] == 96)
                    {
                        layer1[x, y] = 91;
                        layer2[x, y] = 189;
                        layer1[rightX, y] = 92;
                    }
                }
            }

            // add arrows pointing to the goal
            foreach (ProcGenMapNode n in mapNodes)
            {
                if (n.values.ContainsKey("arrow"))
                {
                    int arrowDir = n.values["arrow"];
                    // 144
                    // 7x7
                    // .......
                    // ...#...
                    // ....#..
                    // .#####.
                    // ....#..
                    // ...#...
                    // .......
                    // down up right left
                    // 1, 2, 4, 8
                    int bitMask = (1 << arrowDir);
                    int[] arrowShape = new int[]
                    {
                            0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 15,0, 0, 0,
                            0, 0, 10,3, 6, 0, 0,
                            0, 15,12,15,12,15,0,
                            0, 0, 9, 3, 5, 0, 0,
                            0, 0, 0, 15,0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0,
                    };

                    for (int y = 0; y < 7; y++)
                    {
                        int ty = DataUtil.clampToPositiveRange(y + n.centerY, outdoorMapHeight);
                        for (int x = 0; x < 7; x++)
                        {
                            int tx = DataUtil.clampToPositiveRange(x + n.centerX, outdoorMapWidth);
                            if ((arrowShape[y * 7 + x] & bitMask) > 0)
                            {
                                layer1[tx, ty] = 144;
                            }
                        }
                    }
                }
            }

            // add up to 8 huts on islands that are large enough
            int maxDoors = 8;
            int numDoors = 0;
            byte[] hutLayer1 = new byte[] { 0, 119, 120, 121, 0, 134, 135, 136, 137, 138, 118, 1, 2, 3, 122, 0, 17, 18, 19, 0, 0, 33, 34, 35, 0 };
            int hutWidth = 5;
            int hutHeight = 5;
            int hutOffsetX = -2;
            int hutOffsetY = -2;
            foreach (ProcGenMapNode n in mapNodes)
            {
                if (!n.values.ContainsKey("arrow"))
                {
                    if (n.name.StartsWith("n"))
                    {
                        if ((random.Next() % 2) == 0 && numDoors < maxDoors && n.values["height"] - n.values["cornersize"] >= 9 && n.values["width"] - n.values["cornersize"] >= 9)
                        {
                            // only set layer2 if it's 0
                            for (int y = 0; y < hutHeight; y++)
                            {
                                for (int x = 0; x < hutWidth; x++)
                                {
                                    int thisX = DataUtil.clampToPositiveRange(n.x + x + hutOffsetX + n.values["width"] / 2, outdoorMapWidth);
                                    int thisY = DataUtil.clampToPositiveRange(n.y + y + hutOffsetY + n.values["height"] / 2, outdoorMapHeight);
                                    layer1[thisX, thisY] = hutLayer1[y * hutWidth + x];
                                    if (layer2[thisX, thisY] == 0)
                                    {
                                        layer2[thisX, thisY] = 77;
                                    }
                                    if(layer1[thisX, thisY] == 34)
                                    {
                                        // create door (numDoors)
                                        outsideMap.altMapEntryLocations[numDoors] = new XyPos(thisX, thisY + 2);
                                        outsideMap.altMapExitLocations[new XyPos(thisX, thisY)] = numDoors;
                                    }
                                }
                            }

                            // stick a number of little decorative jars around the hut to indicate which one it is
                            int numThingies = numDoors + 1;

                            int numThingiesPlaced = 0;

                            while (numThingiesPlaced < numThingies)
                            {
                                int x = DataUtil.clampToPositiveRange(n.x + random.Next() % n.values["width"], outdoorMapWidth);
                                int y = DataUtil.clampToPositiveRange(n.y + random.Next() % n.values["height"], outdoorMapHeight);
                                if (layer1[x, y] == 148)
                                {
                                    layer1[x, y] = 39;
                                    numThingiesPlaced++;
                                }
                            }

                            numDoors++;
                        }
                    }
                }
            }

            // add flowers or water tiles, more often if you are near a hut, based on perlin noise
            int perlinZ = (random.Next() % 2000);

            for (int y = 1; y < outdoorMapHeight - 1; y++)
            {
                for (int x = 1; x < outdoorMapWidth - 1; x++)
                {
                    int nearestHutDist = 500;
                    for (int _y = 1; _y < outdoorMapHeight - 1; _y++)
                    {
                        for (int _x = 1; _x < outdoorMapWidth - 1; _x++)
                        {
                            if (layer1[_x, _y] == 34)
                            {
                                int xd = Math.Abs(_x - x);
                                if (xd > outdoorMapWidth / 2)
                                {
                                    xd = outdoorMapWidth - xd;
                                }
                                int yd = Math.Abs(_y - y);
                                if (yd > outdoorMapHeight / 2)
                                {
                                    yd = outdoorMapHeight - yd;
                                }
                                int dist = (int)Math.Sqrt(xd * xd + yd * yd);
                                if (dist < nearestHutDist)
                                {
                                    nearestHutDist = dist;
                                }
                            }
                        }
                    }

                    double threshold = 0.0;
                    threshold += nearestHutDist / 50.0;
                    if (threshold > .5)
                    {
                        threshold = .5;
                    }
                    if (PerlinNoise.noise(x * 20 / (double)outdoorMapWidth, y * 20 / (double)outdoorMapHeight, perlinZ) > threshold)
                    {
                        if (layer1[x, y] == 171)
                        {
                            layer1[x, y] = 144;
                        }
                        if (layer1[x, y] == 148)
                        {
                            layer1[x, y] = 173;
                        }
                    }
                }
            }

            // place steps so you can walk up on each island
            foreach (ProcGenMapNode n in mapNodes)
            {
                if (!n.values.ContainsKey("arrow"))
                {

                    int numSteps = (random.Next() % 3) + 1;
                    List<int> steps = new List<int>();
                    for (int i = 0; i < numSteps; i++)
                    {
                        int stepsDirection;
                        bool ok = false;
                        while (!ok)
                        {
                            stepsDirection = (random.Next() % 4);
                            if (!steps.Contains(stepsDirection))
                            {
                                steps.Add(stepsDirection);
                                ok = true;
                            }
                        }
                    }

                    foreach (int stepsDirection in steps)
                    {
                        switch (stepsDirection)
                        {
                            case 0:
                                // top
                                {
                                    for (int x = n.x + n.values["width"] / 2 - 1; x <= n.x + n.values["width"] / 2 + 1; x++)
                                    {
                                        int mapx = DataUtil.clampToPositiveRange(x, outdoorMapWidth);
                                        layer1[mapx, n.y] = 156;
                                    }
                                }
                                break;
                            case 1:
                                // left
                                {
                                    for (int y = n.y + n.values["height"] / 2 - 1; y <= n.y + n.values["height"] / 2 + 1; y++)
                                    {
                                        int mapy = DataUtil.clampToPositiveRange(y, outdoorMapHeight);
                                        if (y == n.y + n.values["height"] / 2 - 1)
                                        {
                                            layer1[n.x, mapy] = 13;
                                        }
                                        else if (y == n.y + n.values["height"] / 2 + 1)
                                        {
                                            layer1[n.x, mapy] = 45;
                                        }
                                        else
                                        {
                                            layer1[n.x, mapy] = 29;
                                        }
                                    }
                                }
                                break;
                            case 2:
                                // bottom
                                {
                                    for (int x = n.x + n.values["width"] / 2 - 1; x <= n.x + n.values["width"] / 2 + 1; x++)
                                    {
                                        int mapx = DataUtil.clampToPositiveRange(x, outdoorMapWidth);
                                        int mapy1 = DataUtil.clampToPositiveRange(n.y + n.values["height"] - 1, outdoorMapHeight);
                                        int mapy2 = DataUtil.clampToPositiveRange(n.y + n.values["height"] - 2, outdoorMapHeight);
                                        layer1[mapx, mapy1] = 153;
                                        if (x == n.x + n.values["width"] / 2 - 1)
                                        {
                                            layer1[mapx, mapy2] = 92;
                                        }
                                        else if (x == n.x + n.values["width"] / 2 + 1)
                                        {
                                            layer1[mapx, mapy2] = 93;
                                        }
                                        else
                                        {
                                            layer1[mapx, mapy2] = 185;
                                        }
                                    }
                                }
                                break;
                            case 3:
                                // right
                                {
                                    for (int y = n.y + n.values["height"] / 2 - 1; y <= n.y + n.values["height"] / 2 + 1; y++)
                                    {
                                        int mapy = DataUtil.clampToPositiveRange(y, outdoorMapHeight);
                                        int mapx = DataUtil.clampToPositiveRange(n.x + n.values["width"] - 1, outdoorMapWidth);
                                        if (y == n.y + n.values["height"] / 2 - 1)
                                        {
                                            layer1[mapx, mapy] = 14;
                                        }
                                        else if (y == n.y + n.values["height"] / 2 + 1)
                                        {
                                            layer1[mapx, mapy] = 46;
                                        }
                                        else
                                        {
                                            layer1[mapx, mapy] = 30;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            // exit:
            // four 143 (fire) on layer1, 77 (grass) on layer2 on same spots
            // middle: 102 on layer2?, 79 (grass door) on layer1
            int ex0 = DataUtil.clampToPositiveRange(endNode.x + endNode.values["width"] / 2 - 1, outdoorMapWidth);
            int ex1 = DataUtil.clampToPositiveRange(endNode.x + endNode.values["width"] / 2, outdoorMapWidth);
            int ex2 = DataUtil.clampToPositiveRange(endNode.x + endNode.values["width"] / 2 + 1, outdoorMapWidth);
            int ey0 = DataUtil.clampToPositiveRange(endNode.y + endNode.values["height"] / 2 - 1, outdoorMapHeight);
            int ey1 = DataUtil.clampToPositiveRange(endNode.y + endNode.values["height"] / 2, outdoorMapHeight);
            int ey2 = DataUtil.clampToPositiveRange(endNode.y + endNode.values["height"] / 2 + 1, outdoorMapHeight);
            layer1[ex0, ey1] = 143;
            layer1[ex1, ey0] = 143;
            layer1[ex1, ey2] = 143;
            layer1[ex2, ey1] = 143;
            layer2[ex0, ey1] = 77;
            layer2[ex1, ey0] = 77;
            layer2[ex1, ey2] = 77;
            layer2[ex2, ey1] = 77;
            // exit position, tile 79
            outsideMap.exitLocations.Add(new XyPos(ex1, ey1));
            layer1[ex1, ey1] = 79;
            layer2[ex1, ey1] = 102;

            // create map header
            outsideMap.mapData = new FullMap();
            outsideMap.mapData.mapHeader = new MapHeader();
            outsideMap.mapData.mapHeader.setWalkonEventEnabled(true);

            // set tileset; 5 for vanilla island
            outsideMap.mapData.mapHeader.setTileset16(5);
            outsideMap.mapData.mapHeader.setTileset8(5);
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
            generationContext.sharedSettings.setInt(numSharedDoorsProperty, numDoors);

            // tell the logging where the nodes ended up
            generationContext.sharedNodes.AddRange(mapNodes);

            return outsideMap;
        }

        private List<MapObject> makeOutdoorEnemies(List<ProcGenMapNode> mapNodes, ProcGenMapNode startNode, Random random, byte[,] layer1MapData)
        {
            int maxEnemies = 32; // game fucks up if any more than 48
            byte[] enemyIds = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, // a few rabites
                0x05, // iffish
                0x0C, // water thug
                0x11, // ice thug
                0x18, // steamed crab
                0x2C, // dinofish
                0x34, // shellblast
                0x39, // marmablue
                0x3D, // metal crab
                0x43, // turtlance
            };
            List<MapObject> enemies = new List<MapObject>();
            for (int i = 0; i < maxEnemies; i++)
            {
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
                // don't stick them at the map edges
                while (distX < 16 && distY < 16)
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
            Random random = context.randomFunctional;
            AncientCaveMap insideMap = new AncientCaveMap();
            insideMap.mapData = new FullMap();
            byte[,] indoorLayer1 = new byte[indoorMapWidth, indoorMapHeight];
            byte[,] indoorLayer2 = new byte[indoorMapWidth, indoorMapHeight];
            IslandHutRooms.populateIndoorMapData(insideMap, generationContext, indoorLayer1, indoorLayer2);

            List<int> indoorEntryX = new List<int>();
            List<int> indoorEntryY = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                indoorEntryX.Add(15 + (i % 4) * 32);
                indoorEntryY.Add(25 + (i / 4) * 32);
            }

            // as determined by the outdoor map generation
            int numDoors = generationContext.sharedSettings.getInt(numSharedDoorsProperty);

            int numDialogueEvents = context.workingData.getInt(AncientCaveEventMaker.NUM_DIALOGUE_EVENTS);
            
            for (int i = 0; i < numDoors; i++)
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
                    npcObject.setLayer2Collision(true);
                    insideMap.mapData.mapObjects.Add(npcObject);
                }
            }

            insideMap.layer1Data = indoorLayer1;
            insideMap.layer2Data = indoorLayer2;

            // create map header
            insideMap.mapData.mapHeader = new MapHeader();

            // set tileset; 6 for vanilla huts
            insideMap.mapData.mapHeader.setTileset16(6);
            insideMap.mapData.mapHeader.setTileset8(6);

            // display settings 0 for vanilla hut maps
            insideMap.mapData.mapHeader.setDisplaySettings(0);

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

            // this tileset uses layer 2 collision; flag the doors we create to this map to have that flag enabled
            insideMap.layer2Collision = true;

            return insideMap;
        }

        protected override AncientCaveMap generateBossMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            byte[,] bossLayer1 = new byte[bossMapWidth, bossMapHeight];
            byte[,] bossLayer2 = new byte[bossMapWidth, bossMapHeight];

            for (int y = 0; y < bossMapHeight; y++)
            {
                for (int x = 0; x < bossMapWidth; x++)
                {
                    bossLayer1[x, y] = 171;
                    bossLayer2[x, y] = 139;
                }
            }

            Random random = context.randomFunctional;

            // 4 water on each side
            // corner size 4
            int bossNodeWidth = bossMapWidth - 4 * 2;
            int bossNodeHeight = bossMapHeight - 4 * 2;
            for (int y = 0; y < bossNodeHeight; y++)
            {
                for (int x = 0; x < bossNodeWidth; x++)
                {
                    int xDist = x > bossNodeWidth / 2 ? bossNodeWidth - x - 1 : x;
                    int yDist = y > bossNodeHeight / 2 ? bossNodeHeight - y - 1 : y;
                    int cornerIndex = 0;
                    if (x > bossNodeWidth / 2)
                    {
                        cornerIndex += 1;
                    }
                    if (y > bossNodeHeight / 2)
                    {
                        cornerIndex += 2;
                    }
                    int mapX = 4 + x;
                    int mapY = 4 + y;
                    // loop around
                    if (mapX >= bossMapWidth)
                    {
                        mapX -= bossMapWidth;
                    }
                    if (mapY >= bossMapHeight)
                    {
                        mapY -= bossMapHeight;
                    }
                    int cornerSize = 4;
                    if (xDist + yDist >= cornerSize)
                    {
                        bossLayer1[mapX, mapY] = 148;
                        bossLayer2[mapX, mapY] = 0;
                    }
                }
            }

            byte[,] bosscopy1 = new byte[bossMapWidth, bossMapHeight];
            byte[,] bosscopy2 = new byte[bossMapWidth, bossMapHeight];
            for (int y = 0; y < bossMapHeight; y++)
            {
                for (int x = 0; x < bossMapWidth; x++)
                {
                    bosscopy1[x, y] = bossLayer1[x, y];
                    bosscopy2[x, y] = bossLayer2[x, y];
                }
            }

            for (int y = 0; y < bossMapHeight; y++)
            {
                for (int x = 0; x < bossMapWidth; x++)
                {
                    int leftX = x == 0 ? bossMapWidth - 1 : x - 1;
                    int rightX = x == bossMapWidth - 1 ? 0 : x + 1;
                    int upY = y == 0 ? bossMapHeight - 1 : y - 1;
                    int downY = y == bossMapHeight - 1 ? 0 : y + 1;
                    if (bosscopy1[x, y] == 148 && bosscopy1[leftX, y] == 171 && bosscopy1[x, upY] == 171)
                    {
                        // upper left corner
                        bossLayer1[x, y] = 85;
                        bossLayer2[x, y] = 136;
                    }
                    else if (bosscopy1[x, y] == 148 && bosscopy1[rightX, y] == 171 && bosscopy1[x, upY] == 171)
                    {
                        // upper right corner
                        bossLayer1[x, y] = 88;
                        bossLayer2[x, y] = 135;
                    }
                    else if (bosscopy1[x, y] == 148 && bosscopy1[rightX, y] == 171 && bosscopy1[x, downY] == 171)
                    {
                        // lower right corner
                        bossLayer1[x, y] = 100;
                        bossLayer2[x, y] = 122;
                    }
                    else if (bosscopy1[x, y] == 148 && bosscopy1[leftX, y] == 171 && bosscopy1[x, downY] == 171)
                    {
                        // lower left corner
                        bossLayer1[x, y] = 96;
                        bossLayer2[x, y] = 123;
                    }
                    else if (bosscopy1[x, y] == 148 && bosscopy1[leftX, y] == 171)
                    {
                        // left edge
                        bossLayer1[x, y] = 89;
                        bossLayer2[x, y] = 141;
                    }
                    else if (bosscopy1[x, y] == 148 && bosscopy1[rightX, y] == 171)
                    {
                        // right edge
                        bossLayer1[x, y] = 90;
                        bossLayer2[x, y] = 140;
                    }
                    else if (bosscopy1[x, y] == 148 && bosscopy1[x, upY] == 171)
                    {
                        // up edge
                        bossLayer1[x, y] = 82;
                        bossLayer2[x, y] = 143;
                    }
                    else if (bosscopy1[x, y] == 148 && bosscopy1[x, downY] == 171)
                    {
                        // down edge
                        bossLayer1[x, upY] = 98;
                        bossLayer2[x, y] = 142;
                    }
                }
            }

            for (int y = 0; y < bossMapHeight; y++)
            {
                for (int x = 0; x < bossMapWidth; x++)
                {
                    bosscopy1[x, y] = bossLayer1[x, y];
                    bosscopy2[x, y] = bossLayer2[x, y];
                }
            }

            for (int y = 0; y < bossMapHeight; y++)
            {
                for (int x = 0; x < bossMapWidth; x++)
                {
                    int leftX = x == 0 ? bossMapWidth - 1 : x - 1;
                    int rightX = x == bossMapWidth - 1 ? 0 : x + 1;
                    int upY = y == 0 ? bossMapHeight - 1 : y - 1;
                    int downY = y == bossMapHeight - 1 ? 0 : y + 1;
                    if (bosscopy1[x, y] == 98)
                    {
                        bossLayer1[x, downY] = 102;
                    }
                    if (bosscopy1[rightX, y] == 100 && bosscopy1[x, upY] != 98)
                    {
                        if (bosscopy1[leftX, y] != 98)
                        {
                            bossLayer1[leftX, y] = 93;
                        }
                        bossLayer1[x, y] = 99;
                        bossLayer2[x, y] = 121;
                    }
                    if (bosscopy1[leftX, y] == 96 && bosscopy1[x, upY] != 98)
                    {
                        if (bosscopy1[rightX, y] != 98)
                        {
                            bossLayer1[rightX, y] = 92;
                        }
                        bossLayer1[x, y] = 97;
                        bossLayer2[x, y] = 124;
                    }

                    if (bosscopy1[x, y] == 85 && bosscopy1[rightX, y] != 82)
                    {
                        bossLayer1[rightX, y] = 86;
                        bossLayer2[rightX, y] = 137;
                    }
                    if (bosscopy1[x, y] == 88 && bosscopy1[leftX, y] != 82)
                    {
                        bossLayer1[leftX, y] = 87;
                        bossLayer2[leftX, y] = 127;
                    }
                    if (bosscopy1[x, y] == 90 && bosscopy1[x, downY] == 100)
                    {
                        bossLayer1[x, y] = 94;
                        bossLayer2[x, y] = 190;
                        bossLayer1[leftX, y] = 93;
                    }
                    if (bosscopy1[x, y] == 89 && bosscopy1[x, downY] == 96)
                    {
                        bossLayer1[x, y] = 91;
                        bossLayer2[x, y] = 189;
                        bossLayer1[rightX, y] = 92;
                    }
                }
            }

            byte bossX = 16;
            byte bossY = 14;
            byte bossPlayerX = 16;
            byte bossPlayerY = 20;
            int perlinZ = (random.Next() % 2000);

            // decoration
            for (int y = 1; y < bossMapHeight - 1; y++)
            {
                for (int x = 1; x < bossMapWidth - 1; x++)
                {
                    double noise = PerlinNoise.noise(x * 5 / (double)bossMapWidth + 100, y * 5 / (double)bossMapHeight + 100, perlinZ);
                    if (noise > -0.2)
                    {
                        if (noise > 0.1 && bossLayer1[x, y] == 171)
                        {
                            bossLayer1[x, y] = 144;
                        }
                        if (bossLayer1[x, y] == 148)
                        {
                            if (noise > .5)
                            {
                                // solid flower
                                if ((Math.Abs(x - bossX) > 5 || Math.Abs(y - bossY) > 5)
                                    && (Math.Abs(x - bossPlayerX) > 5 || Math.Abs(y - bossPlayerY) > 5))
                                {
                                    // don't stick these on top of the objects
                                    bossLayer1[x, y] = 127;
                                }
                                else
                                {
                                    bossLayer1[x, y] = 173;
                                }
                            }
                            else if (noise > .3)
                            {
                                if ((random.Next() % 2) == 0)
                                {
                                    bossLayer1[x, y] = 173;
                                }
                                else
                                {
                                    bossLayer1[x, y] = 174;
                                }
                            }
                            else if (noise > .1)
                            {
                                if ((random.Next() % 4) == 0)
                                {
                                    if ((random.Next() % 2) == 0)
                                    {
                                        bossLayer1[x, y] = 173;
                                    }
                                    else
                                    {
                                        bossLayer1[x, y] = 174;
                                    }
                                }
                            }
                            else
                            {
                                if ((random.Next() % 16) == 0)
                                {
                                    if ((random.Next() % 2) == 0)
                                    {
                                        bossLayer1[x, y] = 173;
                                    }
                                    else
                                    {
                                        bossLayer1[x, y] = 174;
                                    }
                                }
                            }
                        }
                    }
                    if (noise < -0.4 && bossLayer1[x, y] == 148 && bossLayer1[x + 1, y] == 148 && (random.Next() % 4) == 0)
                    {
                        // rocks
                        bossLayer1[x, y] = 60;
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

            // set tileset; 5 for vanilla island
            bossMap.mapData.mapHeader.setTileset16(5);
            bossMap.mapData.mapHeader.setTileset8(5);

            // display settings 1 for vanilla island maps with bit 0x40 set; MOPPLE: unsure what this is for
            bossMap.mapData.mapHeader.setDisplaySettings(0x41);

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

        protected override string getMapLogString(AncientCaveFloor floorData, AncientCaveGenerationContext generationContext)
        {
            String logString = "";
            logString += "[key]\n";
            logString += "! :   start\n";
            logString += "@ :   end\n";
            logString += ". :   hut\n";
            logString += ", :   decoration\n";
            logString += "= :   steps\n";
            logString += "# :   solid wall\n";

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
                    if (startNode.centerX == x && startNode.centerY == y)
                    {
                        logString += "!";
                        foundNode = true;
                    }
                    else if (endNode.centerX == x && endNode.centerY == y)
                    {
                        logString += "@";
                        foundNode = true;
                    }
                    else
                    {
                        for (int i = 0; i < mapNodes.Count; i++)
                        {
                            ProcGenMapNode n = mapNodes[i];
                            int index = i - 2;
                            if (n.centerX == x && n.centerY == y)
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
                        if (layer1[x, y] == 171 || layer1[x, y] == 148)
                        {
                            // empty space
                            logString += " ";
                        }
                        else if (layer1[x, y] == 34)
                        {
                            // hut door
                            logString += ".";
                        }
                        else if (layer1[x, y] == 144 || layer1[x, y] == 173)
                        {
                            // decor
                            logString += ",";
                        }
                        else if (layer1[x, y] == 156 ||
                            layer1[x, y] == 153 ||
                            layer1[x, y] == 13 ||
                            layer1[x, y] == 45 ||
                            layer1[x, y] == 29 ||
                            layer1[x, y] == 92 ||
                            layer1[x, y] == 93 ||
                            layer1[x, y] == 185 ||
                            layer1[x, y] == 14 ||
                            layer1[x, y] == 46 ||
                            layer1[x, y] == 30)
                        {
                            // steps
                            logString += "=";
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
            // gaia's navel area overworld music
            return new byte[] { 0x40, 0x01, 0x05, 0x10, 0x8F };
        }

        protected override string getFloorType()
        {
            return "Island";
        }
    }
}
