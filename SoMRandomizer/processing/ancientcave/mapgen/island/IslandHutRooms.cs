using SoMRandomizer.util;

namespace SoMRandomizer.processing.ancientcave.mapgen.island
{
    /// <summary>
    /// Utility to make the indoor areas for "island" tileset floors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class IslandHutRooms
    {
        public static void populateIndoorMapData(AncientCaveMap insideMap, AncientCaveGenerationContext generationContext, byte[,] layer1, byte[,] layer2)
        {
            int buffer = 6;
            int mapWidth = generationContext.indoorMapWidth;
            int mapHeight = generationContext.indoorMapHeight;

            int numDoors = generationContext.sharedSettings.getInt(IslandGenerator.numSharedDoorsProperty);

            // MOPPLE: there's basically no detail on these maps at all; should add some random shit

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    layer1[x, y] = 0;
                    layer2[x, y] = 6;
                }
            }

            // from map 448

            // floor: 188 et al
            // most finer details on layer1
            for (int i = 0; i < numDoors; i++)
            {
                // 32, 32?
                byte[] topWall = new byte[] { 8, 11, 14, 17, 20 };

                int yStart = buffer;
                int yEnd = mapHeight - buffer;
                // total width 20, 4 buffer on each side
                int height = yEnd - yStart + 1;
                //int xBuffer = 4;
                int totalRoomWidth = 32 - buffer * 2;

                //             31-buffer*2
                //             v
                //       buffer*2
                //       v              
                //   buffer        31-buffer
                //   v             v
                //  
                //        _____
                //      /       \
                //    /           \
                //   |             |
                //   |             |
                //    \           /
                //      \ _____ /
                //
                //
                //
                int xOffset = 32 * (i % 4);
                int yOffset = 32 * (i / 4);
                int doorXPos = (buffer * 2 + (31 - buffer * 2)) / 2;
                for (int y = buffer; y < 32 - buffer; y++)
                {
                    for (int x = buffer; x < 32 - buffer; x++)
                    {
                        if (x == buffer)
                        {
                            // |##
                            if (y > buffer * 2 && y < 31 - buffer * 2)
                            {
                                if (y == buffer * 2 + 1)
                                {
                                    layer2[x + xOffset, y + yOffset - 1] = 1;
                                }
                                layer2[x + xOffset, y + yOffset] = 48;
                                layer2[x + xOffset, y + yOffset + 1] = 80;
                                layer1[x + xOffset, y + yOffset] = 0;
                                layer1[x + xOffset, y + yOffset + 1] = 1;
                            }
                        }
                        else if (x > buffer && x <= buffer * 2)
                        {
                            // -----______
                            // ----
                            // ---
                            // --
                            // -
                            // |
                            // /##
                            // or
                            // \##
                            if (x + y == buffer * 3)
                            {
                                layer2[x + xOffset, y + yOffset] = 1;
                                layer2[x + xOffset, y + yOffset + 1] = 17;
                                layer2[x + xOffset, y + yOffset + 2] = 33;
                                layer2[x + xOffset, y + yOffset + 3] = 49;
                            }
                            if (x + (30 - y) == buffer * 3)
                            {
                                layer2[x + xOffset, y + yOffset + 1] = 128;
                                // 2, 1, 37, 34, 50
                                layer1[x + xOffset, y + yOffset] = 2;
                                layer1[x + xOffset, y + yOffset + 1] = 1;
                                layer1[x + xOffset, y + yOffset + 2] = 37;
                                layer1[x + xOffset, y + yOffset + 3] = 34;
                                layer1[x + xOffset, y + yOffset + 4] = 50;
                            }
                        }
                        else if (x > buffer * 2 && x < 31 - buffer * 2)
                        {
                            // _________
                            // #########
                            // or
                            // #########
                            // _________
                            if (y == buffer)
                            {
                                layer2[x + xOffset, y + yOffset] = 3;
                                layer2[x + xOffset, y + yOffset + 1] = 19;
                                layer2[x + xOffset, y + yOffset + 2] = 35;
                            }
                            if (y == 31 - buffer)
                            {
                                layer2[x + xOffset, y + yOffset] = 130;
                                layer2[x + xOffset, y + yOffset + 1] = 130;
                                layer2[x + xOffset, y + yOffset + 2] = 176;
                                // return door 
                                if (x == doorXPos)
                                {
                                    insideMap.altMapEntryLocations[i] = new XyPos(x + xOffset, y + yOffset);
                                    insideMap.returnDoors.Add(i);
                                }
                                layer1[x + xOffset, y + yOffset] = 19;
                                layer1[x + xOffset, y + yOffset + 1] = 35;
                                layer1[x + xOffset, y + yOffset + 2] = 35;
                                layer1[x + xOffset, y + yOffset + 3] = 51;
                            }
                        }
                        else if (x >= 31 - buffer * 2 && x < 31 - buffer)
                        {
                            // ##\
                            // or
                            // ##/
                            if ((31 - x) + y == buffer * 3)
                            {
                                layer2[x + xOffset, y + yOffset] = 5;
                                layer2[x + xOffset, y + yOffset + 1] = 21;
                                layer2[x + xOffset, y + yOffset + 2] = 37;
                                layer2[x + xOffset, y + yOffset + 3] = 53;
                            }
                            if ((31 - x) + (30 - y) == buffer * 3)
                            {
                                layer2[x + xOffset, y + yOffset + 1] = 183;
                                // 2, 1, 37, 34, 50
                                layer1[x + xOffset, y + yOffset] = 6;
                                layer1[x + xOffset, y + yOffset + 1] = 22;
                                layer1[x + xOffset, y + yOffset + 2] = 37;
                                layer1[x + xOffset, y + yOffset + 3] = 38;
                                layer1[x + xOffset, y + yOffset + 4] = 54;
                            }
                        }
                        else if (x == 31 - buffer)
                        {
                            // ##|
                            if (y > buffer * 2 && y < 31 - buffer * 2)
                            {
                                if (y == buffer * 2 + 1)
                                {
                                    layer2[x + xOffset, y + yOffset - 1] = 5;
                                }
                                layer2[x + xOffset, y + yOffset] = 54;
                                layer2[x + xOffset, y + yOffset + 1] = 80;
                                layer1[x + xOffset, y + yOffset] = 0;
                                layer1[x + xOffset, y + yOffset + 1] = 7;
                            }
                        }
                    }
                }

                for (int y = buffer + 3; y < 32 - buffer - 1; y++)
                {
                    bool inside = false;
                    bool prevWall = false;
                    for (int x = buffer; x < 32 - buffer; x++)
                    {
                        if (layer2[x + xOffset, y + yOffset] != 6)
                        {
                            if (!prevWall)
                            {
                                inside = !inside;
                            }
                            prevWall = true;
                        }
                        else
                        {
                            prevWall = false;
                        }
                        if (inside && layer2[x + xOffset, y + yOffset] == 6)
                        {
                            layer2[x + xOffset, y + yOffset] = 184;
                        }
                    }
                }
            }
        }

    }
}
