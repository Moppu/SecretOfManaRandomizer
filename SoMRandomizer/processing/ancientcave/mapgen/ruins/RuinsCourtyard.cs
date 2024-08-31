using SoMRandomizer.util;

namespace SoMRandomizer.processing.ancientcave.mapgen.ruins
{
    /// <summary>
    /// Utility to generate the courtyard portion of "ruins" floors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class RuinsCourtyard
    {
        public static void makeCourtyard(byte[,] layer1, byte[,] layer2, AncientCaveMap outsideMap)
        {
            // ////////////////////////////////////////////////////////
            // static stuff
            // ////////////////////////////////////////////////////////
            // front
            // layer1
            // 168 = road
            // 189 = solid dirt, for behind layer2 stuff
            // 158 = solid stone wall thing to use under layer2 fences
            // 28, 44, 47 = shadow on right of small statue things
            // 44 = full shadow dirt
            byte[] l1LargeWall1 = new byte[] { 162, 87, 80, 96, 106, 122, 138, 154 };
            byte[] l1LargeWall2 = new byte[] { 162, 87, 80, 96, 108, 124, 140, 156 };

            byte[] layer2WallTop = new byte[] { 112, 162, 164 };
            byte[] layer2WallTopAlt1Left = new byte[] { 156, 162, 164 };
            byte[] layer2WallTopAlt1Right = new byte[] { 172, 162, 164 };
            byte[] layer2WallTopAlt2 = new byte[] { 44, 162, 164 };

            byte layer1dirt = 27;

            int mapWidth = layer1.GetLength(0);
            int mapHeight = layer1.GetLength(1);

            int wallYValue = mapHeight - 15 - l1LargeWall1.Length;

            int middle = mapWidth / 2;
            // entryway
            int leftBound = middle - 4;
            int rightBound = middle + 3;
            int leftBoundCourtyard = leftBound - 12;
            int rightBoundCourtyard = rightBound + 12;
            for (int y = 0; y < l1LargeWall1.Length; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (x < leftBound || x > rightBound)
                    {
                        if ((x % 2) == 0)
                        {
                            layer1[x, wallYValue + y] = l1LargeWall1[y];
                        }
                        else
                        {
                            layer1[x, wallYValue + y] = l1LargeWall2[y];
                        }
                    }
                }
            }

            for (int y = 0; y < layer2WallTop.Length; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (x < leftBound || x > rightBound)
                    {
                        if (x > leftBoundCourtyard && x < rightBoundCourtyard)
                        {
                            layer2[x, wallYValue + y - 2] = layer2WallTop[y];
                        }
                        else if (x == leftBoundCourtyard)
                        {
                            layer2[x, wallYValue + y - 2] = layer2WallTopAlt1Left[y];
                        }
                        else if (x == rightBoundCourtyard)
                        {
                            layer2[x, wallYValue + y - 2] = layer2WallTopAlt1Right[y];
                        }
                        else
                        {
                            layer2[x, wallYValue + y - 2] = layer2WallTopAlt2[y];
                        }
                    }
                }
            }

            for (int y = 0; y < 4; y++)
            {
                layer1[leftBound, wallYValue + 3 + y] = 12;
                layer1[rightBound, wallYValue + 3 + y] = 12;
            }

            byte[] l2DoubleFencePillar = new byte[] { 112, 128, 164, 164, 180 };
            for (int y = 0; y < 5; y++)
            {
                layer2[leftBound, wallYValue + y - 2] = l2DoubleFencePillar[y];
                layer2[rightBound, wallYValue + y - 2] = l2DoubleFencePillar[y];
            }

            for (int y = mapHeight - 35; y < mapHeight - 10; y++)
            {
                for (int x = leftBound + 1; x <= rightBound - 1; x++)
                {
                    layer1[x, y] = 168;
                }
            }

            // layer 2 statue
            // 4x5
            byte[] layer2Statue = new byte[] {
                73, 74, 75, 76,
                89, 90, 91, 92,
                0, 106, 107, 0,
                0, 122, 123, 0,
                0, 138, 139, 0
            };

            for (int sy = 0; sy < 5; sy++)
            {
                for (int sx = 0; sx < 4; sx++)
                {
                    layer2[leftBound - 5 + sx, mapHeight - 18 + sy] = layer2Statue[sy * 4 + sx];
                    if (layer1[leftBound - 5 + sx, mapHeight - 18 + sy] == layer1dirt && layer2[leftBound - 5 + sx, mapHeight - 18 + sy] != 0)
                    {
                        layer1[leftBound - 5 + sx, mapHeight - 18 + sy] = 189;
                    }

                    layer2[rightBound + 2 + sx, mapHeight - 18 + sy] = layer2Statue[sy * 4 + sx];
                    if (layer1[rightBound + 2 + sx, mapHeight - 18 + sy] == layer1dirt && layer2[rightBound + 2 + sx, mapHeight - 18 + sy] != 0)
                    {
                        layer1[rightBound + 2 + sx, mapHeight - 18 + sy] = 189;
                    }
                }
            }


            byte[] layer2GateFence = new byte[] {
                114, 115,
                130, 131,
                144, 103,
                178, 103,
                178, 103,
                178, 103,
                146, 147,
                180, 180
            };

            for (int sy = 0; sy < 8; sy++)
            {
                for (int sx = 0; sx < 2; sx++)
                {
                    layer2[leftBound - 8 + sx, mapHeight - 18 + sy] = layer2GateFence[sy * 2 + sx];
                    if (layer1[leftBound - 8 + sx, mapHeight - 18 + sy] == layer1dirt)
                    {
                        layer1[leftBound - 8 + sx, mapHeight - 18 + sy] = 189;
                    }

                    layer2[rightBound + 7 + sx, mapHeight - 18 + sy] = layer2GateFence[sy * 2 + sx];
                    if (layer1[rightBound + 7 + sx, mapHeight - 18 + sy] == layer1dirt)
                    {
                        layer1[rightBound + 7 + sx, mapHeight - 18 + sy] = 189;
                    }
                }
            }

            //
            // inner courtyard thing
            //

            // purple floor on the porch
            for (int y = mapHeight - 50; y <= mapHeight - 37; y++)
            {
                for (int x = leftBoundCourtyard; x <= rightBoundCourtyard; x++)
                {
                    if (y < mapHeight - 44)
                    {
                        // shadowed
                        layer1[x, y] = 170;
                    }
                    else
                    {
                        // not shadowed
                        layer1[x, y] = 171;
                    }
                }
            }

            // front porch, steps
            byte[] l1PorchFrontWall = new byte[]
            {
                87, 87,
                106, 108,
                154, 156
            };

            // porch front walls
            for (int y = 0; y < 3; y++)
            {
                for (int x = leftBoundCourtyard; x < rightBoundCourtyard; x += 2)
                {
                    layer1[x, mapHeight - 35 + y - 4] = l1PorchFrontWall[(x % 2) + y * 2];
                    layer1[x + 1, mapHeight - 35 + y - 4] = l1PorchFrontWall[(x % 2) + 1 + y * 2];
                }
            }

            // left/right walls
            for (int y = mapHeight - 52; y <= mapHeight - 24; y++)
            {
                layer1[leftBoundCourtyard, y] = 149;
                layer1[rightBoundCourtyard, y] = 95;
                for (int x = 1; x <= 8; x++)
                {
                    if (y == mapHeight - 25)
                    {
                        layer1[leftBoundCourtyard - x, y] = 1;
                        layer1[rightBoundCourtyard + x, y] = 1;
                    }
                    else
                    {
                        layer1[leftBoundCourtyard - x, y] = 162;
                        layer1[rightBoundCourtyard + x, y] = 162;
                    }
                }
            }


            layer1[leftBoundCourtyard, mapHeight - 25] = 1;
            layer1[rightBoundCourtyard, mapHeight - 25] = 1;
            layer1[leftBoundCourtyard, mapHeight - 24] = 162;
            layer1[rightBoundCourtyard, mapHeight - 24] = 162;

            byte[] l1steps = new byte[]
            {
                73, 171, 171, 73,
                105, 88, 72, 105,
                105, 104, 72, 105,
                82, 104, 72, 82,
                98, 104, 72, 98,
            };


            int stepsWidth = 6;
            int stepsRepeatColumn = 2;
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < stepsWidth; x++)
                {
                    int sourceX = x;
                    if (x > stepsRepeatColumn)
                    {
                        sourceX -= stepsRepeatColumn;
                        if (sourceX < stepsRepeatColumn)
                        {
                            sourceX = stepsRepeatColumn;
                        }
                    }
                    layer1[leftBound + 1 + x, y + mapHeight - 40] = l1steps[y * 4 + sourceX];
                }
            }

            byte[] l2ArchLeft = new byte[] { 116, 132, 148, 0, 0 };
            byte[] l2ArchRight = new byte[] { 117, 133, 149, 0, 0 };
            byte[] l2ArchMiddle = new byte[] { 118, 134, 150, 166, 182 };

            byte[] l1DoorLeft = new byte[] {
                176,
                177, // trigger tile
            };
            byte[] l1DoorRight = new byte[] {
                180,
                181, // trigger tile
            };

            byte[] l1NonDoorLeft = new byte[] {
                125,
                157,
            };
            byte[] l1NonDoorRight = new byte[] {
                127,
                159,
            };

            int doorYPos = mapHeight - 47;

            // left side
            for (int x = mapWidth / 2 - 1; x >= leftBoundCourtyard + 2; x -= 3)
            {
                for (int y = 0; y < 5; y++)
                {
                    layer2[x, mapHeight - 48 + y] = l2ArchLeft[y];
                    layer2[x - 1, mapHeight - 48 + y] = l2ArchMiddle[y];
                    layer2[x - 2, mapHeight - 48 + y] = l2ArchRight[y];
                }
                // solid block at bottom of pillar thing
                layer1[x - 1, mapHeight - 44] = 188;
                if (x == mapWidth / 2 - 1)
                {
                    layer1[x, doorYPos] = l1DoorLeft[0];
                    layer1[x, doorYPos + 1] = l1DoorLeft[1];
                    // match with the insidemap's -1 door created in the main generator
                    outsideMap.altMapExitLocations[new XyPos(x, doorYPos + 1)] = -1;
                }
                else
                {
                    layer1[x, doorYPos] = l1NonDoorLeft[0];
                    layer1[x, doorYPos + 1] = l1NonDoorLeft[1];
                }
                layer1[x - 1, doorYPos] = 188;
                layer1[x - 1, doorYPos + 1] = 188;
                layer1[x - 2, doorYPos] = l1NonDoorRight[0];
                layer1[x - 2, doorYPos + 1] = l1NonDoorRight[1];
            }

            // right side
            for (int x = mapWidth / 2; x <= rightBoundCourtyard - 2; x += 3)
            {
                for (int y = 0; y < 5; y++)
                {
                    layer2[x, mapHeight - 48 + y] = l2ArchRight[y];
                    layer2[x + 1, mapHeight - 48 + y] = l2ArchMiddle[y];
                    layer2[x + 2, mapHeight - 48 + y] = l2ArchLeft[y];
                }
                // solid block at bottom of pillar thing
                layer1[x + 1, mapHeight - 44] = 188;
                if (x == mapWidth / 2)
                {
                    layer1[x, doorYPos] = l1DoorRight[0];
                    layer1[x, doorYPos + 1] = l1DoorRight[1];
                    // match with the insidemap's -1 door created in the main generator
                    outsideMap.altMapEntryLocations[-1] = new XyPos(x, doorYPos + 2);
                    outsideMap.altMapExitLocations[new XyPos(x, doorYPos + 1)] = -1;
                }
                else
                {
                    layer1[x, doorYPos] = l1NonDoorRight[0];
                    layer1[x, doorYPos + 1] = l1NonDoorRight[1];
                }
                layer1[x + 1, doorYPos] = 188;
                layer1[x + 1, doorYPos + 1] = 188;
                layer1[x + 2, doorYPos] = l1NonDoorLeft[0];
                layer1[x + 2, doorYPos + 1] = l1NonDoorLeft[1];
            }

            // up above on layer 1
            for (int x = leftBoundCourtyard + 1; x < rightBoundCourtyard; x++)
            {
                layer1[x, mapHeight - 50] = 87;
                layer1[x, mapHeight - 49] = 97;
            }

            layer1[mapWidth / 2 - 2, mapHeight - 50] = 75;
            layer1[mapWidth / 2 - 2, mapHeight - 49] = 91;
            layer1[mapWidth / 2 + 1, mapHeight - 50] = 75;
            layer1[mapWidth / 2 + 1, mapHeight - 49] = 91;
        }
    }
}
