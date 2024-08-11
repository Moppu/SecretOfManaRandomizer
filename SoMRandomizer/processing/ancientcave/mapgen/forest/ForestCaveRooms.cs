using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;

namespace SoMRandomizer.processing.ancientcave.mapgen.forest
{
    /// <summary>
    /// Utility to generate indoor areas for "forest" tileset floors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ForestCaveRooms
    {
        // size in 16x16 tiles between rooms
        private static int buffer = 4;

        public static void populateIndoorMapData(AncientCaveMap indoorMap, int numRooms, RandoContext context, byte[,] layer1, byte[,] layer2)
        {
            int mapWidth = layer1.GetLength(0);
            int mapHeight = layer1.GetLength(1);

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    layer1[x, y] = 5;
                }
            }

            for (int i = 0; i < numRooms; i++)
            {
                // 32, 32?
                byte[] topWall = new byte[] { 8, 11, 14, 17, 20 };

                int centerX1 = 15;
                int centerX2 = 16;
                int xOffset = 32 * (i % 4);
                int yOffset = 32 * (i / 4);
                int xAtLeftWall = buffer;
                int xAtRightWall = 31 - buffer;
                int yAtEntryDoor = 31 - buffer;
                int yAtEntryDoorGlow = 31 - buffer - 1;
                // place doors and floor
                for (int y = buffer; y < 32 - buffer; y++)
                {
                    for (int x = buffer; x < 32 - buffer; x++)
                    {
                        if (x != xAtLeftWall && x != xAtRightWall)
                        {
                            if (y == yAtEntryDoor)
                            {
                                // place doors at the bottom-most y pos
                                if (x == centerX1 || x == centerX2)
                                {
                                    // doorway
                                    layer1[x + xOffset, y + yOffset] = 119;
                                    if (x == centerX1)
                                    {
                                        // position the outside map will warp in to
                                        indoorMap.altMapEntryLocations[i] = new XyPos((short)(x + xOffset), (short)(y + yOffset - 1)); // one above
                                    }
                                    // pairing of doors back to warp to outside map
                                    indoorMap.altMapExitLocations[new XyPos((short)(x + xOffset), (short)(y + yOffset))] = i;
                                }
                            }
                            else 
                            {
                                // place floors at every other y pos, including the door glow
                                if (y == 31 - buffer - 1 && (x == centerX1 || x == centerX2))
                                {
                                    // doorway glow
                                    if (x == centerX1)
                                    {
                                        // left
                                        layer1[x + xOffset, y + yOffset] = 173;
                                    }
                                    else
                                    {
                                        // right
                                        layer1[x + xOffset, y + yOffset] = 174;
                                    }
                                }
                                else
                                {
                                    // floor
                                    layer1[x + xOffset, y + yOffset] = 70;
                                }
                            }
                        }
                    }
                }
            }

            // randomize room shapes a bit
            randomizeRoomShapes(layer1, layer2, context.randomFunctional);
            placeWalls(layer1, layer2);
        }

        private static void randomizeRoomShapes(byte[,] layer1, byte[,] layer2, Random random)
        {
            byte[,] copy1 = new byte[128, 64];
            byte[,] copy2 = new byte[128, 64];

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    copy1[x, y] = layer1[x, y];
                    copy2[x, y] = layer2[x, y];
                }
            }

            for (int y = 2; y < 64 - 2; y += 2)
            {
                for (int x = 2; x < 126; x += 2)
                {
                    // floor
                    if (layer1[x, y] == 70)
                    {
                        if (layer1[x, y - 2] == 5 ||
                            layer1[x - 2, y] == 5 ||
                            layer1[x + 2, y] == 5 ||
                            layer1[x, y + 2] == 5)
                        {
                            // don't put it over doorways
                            if (layer1[x, y] != 173 && layer1[x, y] != 174 && layer1[x, y] != 180 &&
                                layer1[x + 1, y] != 173 && layer1[x + 1, y] != 174 && layer1[x + 1, y] != 180 &&
                                layer1[x + 1, y - 1] != 173 && layer1[x + 1, y - 1] != 174 && layer1[x + 1, y - 1] != 180 &&
                                layer1[x, y - 1] != 173 && layer1[x, y - 1] != 174 && layer1[x, y - 1] != 180)
                            {
                                if ((random.Next() % 2) == 0)
                                {
                                    copy1[x, y] = 5;
                                    copy1[x + 1, y] = 5;
                                    copy1[x + 1, y - 1] = 5;
                                    copy1[x, y - 1] = 5;
                                }
                            }
                        }
                    }
                }
            }

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    layer1[x, y] = copy1[x, y];
                    layer2[x, y] = copy2[x, y];
                }
            }
        }

        private static void placeWalls(byte[,] layer1, byte[,] layer2)
        {
            byte[] topWall = new byte[] { 8, 11, 14, 17, 20 };
            byte[] topWallDiag1 = new byte[] { 78, 9, 12, 15, 18, 21 }; // /
            byte[] topWallDiag2 = new byte[] { 79, 7, 10, 13, 16, 19 }; // \

            byte[,] copy1 = new byte[128, 64];
            byte[,] copy2 = new byte[128, 64];

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    copy1[x, y] = layer1[x, y];
                    copy2[x, y] = layer2[x, y];
                }
            }

            for (int y = 1; y < 64 - 1; y++)
            {
                for (int x = 1; x < 127; x++)
                {
                    bool corner = false;

                    // upper / corner
                    if (layer1[x, y] == 5 &&
                        layer1[x + 1, y] == 70 &&
                        layer1[x, y + 1] == 70)
                    {
                        for (int i = 0; i < topWallDiag1.Length; i++)
                        {
                            int _y = y - (topWallDiag1.Length - i) + 1;
                            if (_y >= 0)
                            {
                                copy1[x, _y] = topWallDiag1[i];
                            }
                        }
                        corner = true;
                    }

                    // upper \ corner
                    if (layer1[x, y] == 5 &&
                        layer1[x - 1, y] == 70 &&
                        layer1[x, y + 1] == 70)
                    {
                        for (int i = 0; i < topWallDiag2.Length; i++)
                        {
                            int _y = y - (topWallDiag2.Length - i) + 1;
                            if (_y >= 0)
                            {
                                copy1[x, _y] = topWallDiag2[i];
                            }
                        }
                        corner = true;
                    }

                    if (layer1[x, y] == 5 &&
                        layer1[x, y + 1] == 70 && !corner)
                    {
                        for (int i = 0; i < topWall.Length; i++)
                        {
                            int _y = y - (topWall.Length - i) + 1;
                            if (_y >= 0)
                            {
                                copy1[x, _y] = topWall[i];
                            }
                        }
                    }
                }
            }

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    layer1[x, y] = copy1[x, y];
                    layer2[x, y] = copy2[x, y];
                }
            }

            for (int y = 1; y < 64 - 1; y++)
            {
                for (int x = 1; x < 127; x++)
                {
                    bool corner = false;
                    bool ledgeAbove = false;
                    bool ledgeRight = false;
                    bool ledgeLeft = false;
                    foreach (byte v in topWall)
                    {
                        if (layer1[x, y - 1] == v)
                        {
                            ledgeAbove = true;
                        }
                        if (v != 8 && layer1[x - 1, y] == v)
                        {
                            ledgeLeft = true;
                        }
                        if (v != 8 && layer1[x + 1, y] == v)
                        {
                            ledgeRight = true;
                        }
                    }
                    foreach (byte v in topWallDiag1)
                    {
                        if (layer1[x, y - 1] == v)
                        {
                            ledgeAbove = true;
                        }
                        if (v != 78 && v != 9 && layer1[x - 1, y] == v)
                        {
                            ledgeLeft = true;
                        }
                        if (v != 78 && v != 9 && layer1[x + 1, y] == v)
                        {
                            ledgeRight = true;
                        }
                    }
                    foreach (byte v in topWallDiag2)
                    {
                        if (layer1[x, y - 1] == v)
                        {
                            ledgeAbove = true;
                        }
                        if (v != 79 && v != 7 && layer1[x - 1, y] == v)
                        {
                            ledgeLeft = true;
                        }
                        if (v != 79 && v != 7 && layer1[x + 1, y] == v)
                        {
                            ledgeRight = true;
                        }
                    }
                    // lower / corner
                    if ((layer1[x, y] == 5 || layer1[x, y] == 79) &&
                        (layer1[x - 1, y] == 70 || ledgeLeft) &&
                        (layer1[x, y - 1] == 70 || ledgeAbove))
                    {
                        if (layer1[x, y - 1] >= 19 && layer1[x, y - 1] <= 21)
                        {
                            copy1[x, y] = 71;
                        }
                        else if (ledgeAbove)
                        {
                            copy1[x, y] = 1;
                        }
                        else
                        {
                            copy1[x, y] = 71;
                        }
                        corner = true;
                    }

                    // lower \ corner
                    if ((layer1[x, y] == 5 || layer1[x, y] == 78) &&
                        (layer1[x + 1, y] == 70 || ledgeRight) &&
                        (layer1[x, y - 1] == 70 || ledgeAbove))
                    {
                        if (layer1[x, y - 1] >= 19 && layer1[x, y - 1] <= 21)
                        {
                            copy1[x, y] = 73;
                        }
                        else if (ledgeAbove)
                        {
                            copy1[x, y] = 3;
                        }
                        else
                        {
                            copy1[x, y] = 73;
                        }
                        corner = true;
                    }

                    if (layer1[x, y + 1] == 5 &&
                        layer1[x, y] == 70 && !corner)
                    {
                        // bottom wall
                        copy1[x, y + 1] = 2;
                    }

                    if (layer1[x, y] == 5 &&
                        (layer1[x - 1, y] == 70 || ledgeLeft) && !corner)
                    {
                        // right wall
                        copy1[x, y] = 4;
                    }
                    if (layer1[x, y] == 5 &&
                        (layer1[x + 1, y] == 70 || ledgeRight) && !corner)
                    {
                        // left wall
                        copy1[x, y] = 6;
                    }
                }
            }

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    layer1[x, y] = copy1[x, y];
                    layer2[x, y] = copy2[x, y];
                }
            }
        }
    }
}
