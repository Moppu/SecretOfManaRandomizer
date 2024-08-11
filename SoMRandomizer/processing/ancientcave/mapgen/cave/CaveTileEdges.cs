using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.ancientcave.mapgen.cave
{
    /// <summary>
    /// Utility to add tile edges to a basic "cave" tileset map.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CaveTileEdges
    {
        public static void addTileEdges(byte[,] layer1, byte[,] layer2, Random random, ProcGenMapNode startNode)
        {
            int mapWidth = layer1.GetLength(0);
            int mapHeight = layer1.GetLength(1);
            // first copy the edgeless layer1
            byte[,] layer1Copy = new byte[mapWidth, mapHeight];
            byte[,] layer2Copy = new byte[mapWidth, mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    layer1Copy[x, y] = layer1[x, y];
                    layer2Copy[x, y] = layer2[x, y];
                }
            }

            // //////////////////////////////////////////////
            // remove artifacts of the radial node generation
            // //////////////////////////////////////////////
            for (int i = 0; i < 10; i++)
            {
                for (int y = 1; y < mapHeight - 1; y++)
                {
                    for (int x = 1; x < mapWidth - 1; x++)
                    {
                        // lava, blackness
                        byte[] valueChecks = new byte[] { 70, 5, 43 };
                        foreach (byte value in valueChecks)
                        {
                            if (layer1Copy[x, y] == value)
                            {
                                int numMatches = 0;
                                byte[] checkTiles1 = new byte[]
                                {
                                layer1Copy[x + 1, y],
                                layer1Copy[x - 1, y],
                                layer1Copy[x, y + 1],
                                layer1Copy[x, y - 1],
                                };
                                byte[] checkTiles2 = new byte[]
                                {
                                layer2Copy[x + 1, y],
                                layer2Copy[x - 1, y],
                                layer2Copy[x, y + 1],
                                layer2Copy[x, y - 1],
                                };

                                List<byte> altTiles1 = new List<byte>();
                                List<byte> altTiles2 = new List<byte>();
                                bool[] dirs = new bool[] { false, false, false, false };
                                for (int j = 0; j < checkTiles1.Length; j++)
                                {
                                    byte b = checkTiles1[j];
                                    byte b2 = checkTiles2[j];
                                    if (b == value)
                                    {
                                        numMatches++;
                                    }
                                    else
                                    {
                                        altTiles1.Add(b);
                                        altTiles2.Add(b2);
                                        dirs[j] = true;
                                    }
                                }

                                bool flat = (dirs[0] && dirs[1]) || (dirs[2] && dirs[3]);
                                if (numMatches <= 1 || flat)
                                {
                                    int id = random.Next() % altTiles1.Count;
                                    layer1[x, y] = altTiles1[id];
                                    layer2[x, y] = altTiles2[id];
                                }
                            }
                        }
                    }
                }

                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        layer1Copy[x, y] = layer1[x, y];
                        layer2Copy[x, y] = layer2[x, y];
                    }
                }
            }

            for (int x = 15; x < mapWidth - 15; x++)
            {
                int y = mapHeight - 2;
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
                        for (int _y = 0; _y <= num; _y++)
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

            String logString = "AFTER ARTIFACT REMOVAL:";
            for (int y = 0; y < mapHeight; y++)
            {
                logString += "\r\n";
                for (int x = 0; x < mapWidth; x++)
                {

                    if (layer1[x, y] == 43)
                    {
                        logString += "w";
                    }
                    else if (layer1[x, y] == 70)
                    {
                        logString += " ";
                    }
                    else
                    {
                        logString += "#";
                    }

                }
            }

            // ///////////////////////////////////////////////////////
            // add edge/transition tile
            // ///////////////////////////////////////////////////////

            // first copy the edgeless layer1
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    layer1Copy[x, y] = layer1[x, y];
                }
            }

            int xRight = mapWidth - 15;
            int yBottom = mapHeight - 10;

            TileReplacer.BoundsRectangle rect = null;

            ////////// lava -> water transitions
            // lava on top, water on bottom
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"!43", "70", "!43",
                 "*",   "43",  "*",
                 "*",   "43",  "*",},
                new string[]
                {"-", "68", "-",
                 "-", "17", "-",
                 "-", "20", "-"},
                new string[]
                {"-", "-", "-",
                 "-", "2", "-",
                 "-", "50", "-"},
                3), layer1Copy, layer1, layer2, rect);
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"!43", "70", "!43",
                 "*",   "43",  "*",},
                new string[]
                {"-", "68", "-",
                 "-", "17", "-"},
                new string[]
                {"-", "-", "-",
                 "-", "2", "-",},
                3), layer1Copy, layer1, layer2, rect);
            // same thing, left corner
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",  "70", "*",
                 "43", "70", "!43",
                 "*",   "43",  "*",
                 "*",   "43",  "*",},
                new string[]
                {"-", "111", "-",
                 "-", "67", "-",
                 "-", "16", "-",
                 "-", "123", "-"},
                new string[]
                {"-", "-", "-",
                 "-", "-", "-",
                 "-", "33", "-",
                 "-", "22", "-"},
                3), layer1Copy, layer1, layer2, rect);
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",  "70", "*",
                 "43", "70", "!43",
                 "*",   "43",  "*",},
                new string[]
                {"-", "111", "-",
                 "-", "67", "-",
                 "-", "16", "-",},
                new string[]
                {"-", "-", "-",
                 "-", "-", "-",
                 "-", "33", "-",},
                3), layer1Copy, layer1, layer2, rect);
            // same thing, right corner
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",  "70", "*",
                 "!43", "70", "43",
                 "*",   "43",  "*",
                 "*",   "43",  "*",},
                new string[]
                {"-", "110", "-",
                 "-", "69", "-",
                 "-", "18", "-",
                 "-", "124", "-"},
                new string[]
                {"-", "-", "-",
                 "-", "-", "-",
                 "-", "35", "-",
                 "-", "22", "-"},
                3), layer1Copy, layer1, layer2, rect);
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",  "70", "*",
                 "!43", "70", "43",
                 "*",   "43",  "*",},
                new string[]
                {"-", "110", "-",
                 "-", "69", "-",
                 "-", "18", "-",},
                new string[]
                {"-", "-", "-",
                 "-", "-", "-",
                 "-", "35", "-",},
                3), layer1Copy, layer1, layer2, rect);

            // lava on bottom, water on top
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",   "43",  "*",
                 "!43", "70", "!43",},
                new string[]
                {"-", "-",  "-",
                 "-", "36", "-",},
                3), layer1Copy, layer1, layer2, rect);
            // same thing, left corner
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",   "43",  "*",
                 "43", "70", "!43",},
                new string[]
                {"-", "-",  "-",
                 "-", "96", "-",},
                new string[]
                {"-", "-",  "-",
                 "-", "1", "-",},
                3), layer1Copy, layer1, layer2, rect);
            // same thing, right corner
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",   "43",  "*",
                 "!43", "70", "43",},
                new string[]
                {"-", "-",  "-",
                 "-", "112", "-",},
                new string[]
                {"-", "-",  "-",
                 "-", "3", "-",},
                3), layer1Copy, layer1, layer2, rect);

            // lava on right, water on left
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",   "!43",
                 "43",  "70",
                 "*",   "!43",},
                new string[]
                {"-", "-",
                 "-", "51",
                 "-", "-",},
                new string[]
                {"-", "-",
                 "16", "-",
                 "-", "-",},
                2), layer1Copy, layer1, layer2, rect);

            // lava on left, water on right
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"!43",  "*",
                 "70",  "43",
                 "!43",  "*",},
                new string[]
                {"-", "-",
                 "53","-",
                 "-", "-",},
                new string[]
                {"-", "-",
                 "-","20",
                 "-", "-",},
                2), layer1Copy, layer1, layer2, rect);




            ////////// lava -> wall transitions
            // lava on ...
            // left side
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*", "5",  "*",
                 "!5", "5",  "5",
                 "*",  "5", "*",},
                new string[]
                {"-",  "-", "-",
                 "-", "4","-",
                 "-",  "-", "-"},
                3), layer1Copy, layer1, null, rect);

            // right side
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*", "5",  "*",
                 "5", "5",  "!5",
                 "*",  "5", "*",},
                new string[]
                {"-",  "-", "-",
                 "-", "6","-",
                 "-",  "-", "-"},
                3), layer1Copy, layer1, null, rect);

            // bottom side
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",  "*",  "*",
                 "*",  "*",  "*",
                 "*",  "*",  "*",
                 "*",  "*",  "*",
                 "5",  "5",  "5",
                 "*",  "!5", "*",},
                new string[]
                {"-", "8", "-",
                 "-", "11", "-",
                 "-", "14", "-",
                 "-", "17", "-",
                 "-", "20","-",
                 "-",  "-", "-"},
                3), layer1Copy, layer1, null, rect);

            // bottom left side
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",  "*",  "*",
                 "*",  "*",  "*",
                 "*",  "*",  "*",
                 "*",  "*",  "*",
                 "!5", "5",  "5",
                 "*",  "!5", "*",},
                new string[]
                {"-", "7", "-",
                 "-", "10", "-",
                 "-", "13", "-",
                 "-", "16", "-",
                 "-", "123","-",
                 "-",  "-", "-"},
                3), layer1Copy, layer1, null, rect);

            // bottom right side
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*",  "*",  "*",
                 "*",  "*",  "*",
                 "*",  "*",  "*",
                 "*",  "*",  "*",
                 "5", "5",  "!5",
                 "*",  "!5", "*",},
                new string[]
                {"-", "9", "-",
                 "-", "12", "-",
                 "-", "15", "-",
                 "-", "18", "-",
                 "-", "21","-",
                 "-",  "-", "-"},
                3), layer1Copy, layer1, null, rect);

            // top side
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*", "!5",  "*",
                 "5", "5",  "5",
                 "*",  "5", "*",},
                new string[]
                {"-",  "-", "-",
                 "-", "2","-",
                 "-",  "-", "-"},
                3), layer1Copy, layer1, null, rect);

            // top right side
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*", "!5",  "*",
                 "5", "5",  "!5",
                 "*",  "5", "*",},
                new string[]
                {"-",  "-", "-",
                 "-", "73","-",
                 "-",  "-", "-"},
                3), layer1Copy, layer1, null, rect);

            // top left side
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"*", "!5",  "*",
                 "!5", "5",  "5",
                 "*",  "5", "*",},
                new string[]
                {"-",  "-", "-",
                 "-", "71","-",
                 "-",  "-", "-"},
                3), layer1Copy, layer1, null, rect);

            // second pass
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    layer1Copy[x, y] = layer1[x, y];
                }
            }


            // fix ground corners
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "{67,68}", "70",
                 "*", "51",},
                new string[]
                {"-",  "111",
                 "-",  "-",},
               2), layer1Copy, layer1, null, rect);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "70", "{68,69}",
                 "53", "*",},
                new string[]
                {"110","-",
                 "-",  "-",},
               2), layer1Copy, layer1, null, rect);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "53", "*",
                 "70", "36",},
                new string[]
                {"-","-",
                 "126","-",},
               2), layer1Copy, layer1, null, rect);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "*", "51",
                 "36", "70",},
                new string[]
                {"-","-",
                 "-","127",},
               2), layer1Copy, layer1, null, rect);

            // fix water corners
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "-", "16",
                 "43", "-"},
                new string[]
                {"-",  "-",
                 "-", "-"},
                new string[]
                {"-",  "33",
                 "-", "-"},
               2), layer1Copy, layer1, layer2, rect);
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "123",
                 "43", },
                new string[]
                {"-",
                 "-",},
                new string[]
                {"22",
                 "-",},
               1), layer1Copy, layer1, layer2, rect);
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "20",
                 "43", },
                new string[]
                {"-",
                 "-",},
                new string[]
                {"50",
                 "-",},
               1), layer1Copy, layer1, layer2, rect);
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "21",
                 "43", },
                new string[]
                {"-",
                 "-",},
                new string[]
                {"22",
                 "-",},
               1), layer1Copy, layer1, layer2, rect);
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "18", "-",
                 "-", "43"},
                new string[]
                {"-",  "-",
                 "-", "-"},
                new string[]
                {"35",  "-",
                 "-", "-"},
               2), layer1Copy, layer1, layer2, rect);


            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "17", "{51,67}",
                 "20", "{51,67}",},
                new string[]
                {"-",  "-",
                 "-",  "-",},
                new string[]
                {"34",  "-",
                 "78",  "-",},
               2), layer1Copy, layer1, layer2, rect);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "{53,69}", "17",
                 "{53,69}", "20",},
                new string[]
                {"-",  "-",
                 "-",  "-",},
                new string[]
                {"-", "34",
                 "-", "77",},
               2), layer1Copy, layer1, layer2, rect);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "53", "18"},
                new string[]
                {"-", "-"},
                new string[]
                {"-", "35"},
               2), layer1Copy, layer1, layer2, rect);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "43",
                 "71" },
                new string[]
                {"-",
                 "-" },
                new string[]
                {"-",
                 "1" },
               1), layer1Copy, layer1, layer2, rect);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                 "43",
                 "73" },
                new string[]
                {"-",
                 "-" },
                new string[]
                {"-",
                 "3" },
               1), layer1Copy, layer1, layer2, rect);

            // perlin stuff
            int perlinZ = (random.Next() % 2000);
            // layer 2 = 5 for bushes
            for (int y = 1; y < mapHeight - 1; y++)
            {
                for (int x = 1; x < mapWidth - 1; x++)
                {
                    int dist = 256;
                    if (startNode != null)
                    {
                        dist = (int)Math.Sqrt((x - startNode.x) * (x - startNode.x) +
                            (y - startNode.y) * (y - startNode.y));
                    }
                    if (dist > 6 && (layer1[x, y] == 43 || layer1[x, y] == 70) && PerlinNoise.noise(x * 40 / (double)mapWidth, y * 40 / (double)mapHeight, perlinZ) > 0.4)
                    {
                        // axe things
                        if (layer1[x - 1, y] == layer1[x, y] &&
                            layer1[x + 1, y] == layer1[x, y] &&
                            layer1[x, y + 1] == layer1[x, y] &&
                            layer1[x, y - 1] == layer1[x, y])
                        {
                            layer1[x, y] = 168;
                        }
                    }
                    else if ((layer1[x, y] == 43 || layer1[x, y] == 70) && PerlinNoise.noise(x * 20 / (double)mapWidth, y * 20 / (double)mapHeight, perlinZ) < -0.2 && (random.Next() % 4) != 0)
                    {
                        // we can't put these at borders because making them collision type 8
                        // makes it so i can step across the borders.
                        // note this works out to shape it in diagonals just by not using layer1copy and i sort of like it that way
                        if (layer1[x - 1, y] == layer1[x, y] &&
                            layer1[x + 1, y] == layer1[x, y] &&
                            layer1[x, y + 1] == layer1[x, y] &&
                            layer1[x, y - 1] == layer1[x, y])
                        {
                            layer1[x, y] = (byte)((random.Next() % 2) == 0 ? 29 : 45);
                        }
                    }
                }
            }
        }
    }
}
