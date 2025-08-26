using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;

namespace SoMRandomizer.processing.ancientcave.mapgen.forest
{
    /// <summary>
    /// Utility to add tile edges to a basic "forest" tileset map.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ForestTileEdges
    {
        public static void addTileEdges(byte[,] layer1, byte[,] layer2, RandoContext context)
        {
            int width = layer1.GetLength(0);
            int height = layer1.GetLength(1);
            Random random = context.randomFunctional;
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
            byte[,] copy1 = new byte[width, height];
            byte[,] copy2 = new byte[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    copy1[x, y] = layer1[x, y];
                    copy2[x, y] = layer2[x, y];
                }
            }


            // 103 is also steps, 145 etc caves
            byte[] ledgeTileValues = new byte[] { 18, 19, 34, 35, 50, 51, 75, 76, 103, 1, 49, 33, 65, 4, 52, 36, 68, 145, 146, 161, 162, 6, 63, 79, 54, 9, 62, 78, 68 };

            // original pattern:
            // UR UL
            // MR ML
            // UL UR
            // ML MR

            byte[] fullTreeTiles = new byte[] {
                ForestConstants.L1_TREE_MIDLEFT,
                ForestConstants.L1_TREE_MIDRIGHT,
                ForestConstants.L1_TREE_UPPERLEFT,
                ForestConstants.L1_TREE_UPPERRIGHT,};

            byte[] allTreeTiles = new byte[] {
                ForestConstants.L1_TREE_MIDLEFT,
                ForestConstants.L1_TREE_MIDRIGHT,
                ForestConstants.L1_TREE_UPPERLEFT,
                ForestConstants.L1_TREE_UPPERRIGHT,
                ForestConstants.L1_TREE_LOWLEFT_EDGE,
                ForestConstants.L1_TREE_LOWRIGHT_EDGE,
                ForestConstants.L1_TREE_MIDLEFT_EDGE,
                ForestConstants.L1_TREE_MIDRIGHT_EDGE,
                ForestConstants.L1_TREE_UPPERLEFT_EDGE,
                ForestConstants.L1_TREE_UPPERRIGHT_EDGE};

            byte[] originalPattern = new byte[]
            {
                    ForestConstants.L1_TREE_UPPERRIGHT, ForestConstants.L1_TREE_UPPERLEFT,
                    ForestConstants.L1_TREE_MIDRIGHT, ForestConstants.L1_TREE_MIDLEFT,
                    ForestConstants.L1_TREE_UPPERLEFT, ForestConstants.L1_TREE_UPPERRIGHT,
                    ForestConstants.L1_TREE_MIDLEFT, ForestConstants.L1_TREE_MIDRIGHT,
            };

            // revert to blocks of original pattern for easier thingies
            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 2)
                {
                    int numLedges = 0;
                    int numTreeMatches = 0;
                    for (int _y = 0; _y < 4; _y++)
                    {
                        for (int _x = 0; _x < 2; _x++)
                        {
                            byte val = layer1[x + _x, y + _y];
                            if (val == originalPattern[_y * 2 + _x])
                            {
                                numTreeMatches++;
                            }
                            foreach (byte b in ledgeTileValues)
                            {
                                if (val == b)
                                {
                                    numLedges++;
                                }
                            }
                        }
                    }

                    if (numLedges > 0)
                    {
                        for (int _y = 0; _y < 4; _y++)
                        {
                            for (int _x = 0; _x < 2; _x++)
                            {
                                byte val = layer1[x + _x, y + _y];
                                bool ledge = false;
                                foreach (byte b in ledgeTileValues)
                                {
                                    if (val == b)
                                    {
                                        ledge = true;
                                    }
                                }
                                if (!ledge)
                                {
                                    layer1[x + _x, y + _y] = ForestConstants.L1_EMPTY;
                                }
                            }
                        }
                    }
                    else if (numTreeMatches < 8)
                    {
                        for (int _y = 0; _y < 4; _y++)
                        {
                            for (int _x = 0; _x < 2; _x++)
                            {
                                byte val = layer1[x + _x, y + _y];
                                if (numTreeMatches < 4)
                                {
                                    if (val == originalPattern[_y * 2 + _x])
                                    {
                                        layer1[x + _x, y + _y] = ForestConstants.L1_EMPTY;
                                    }
                                }
                                else
                                {
                                    if (val == ForestConstants.L1_EMPTY)
                                    {
                                        layer1[x + _x, y + _y] = originalPattern[_y * 2 + _x];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    copy1[x, y] = layer1[x, y];
                    copy2[x, y] = layer2[x, y];
                }
            }

            // fix bottoms of trees
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"" + ForestConstants.L1_TREE_UPPERLEFT,
                     "" + ForestConstants.L1_TREE_MIDLEFT,
                     "!" + ForestConstants.L1_TREE_UPPERRIGHT,},
                new string[]
                {"" + ForestConstants.L1_TREE_LOWRIGHT_EDGE,
                     "" + ForestConstants.L1_EMPTY_BLOCK,
                     "-",},
                new string[]
                {"-",
                     "" + ForestConstants.L2_STUMP_RIGHT,
                     "-",},
                1), copy1, layer1, layer2);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                     "" + ForestConstants.L1_TREE_UPPERRIGHT,
                     "" + ForestConstants.L1_TREE_MIDRIGHT,
                     "!" + ForestConstants.L1_TREE_UPPERLEFT,},
                new string[]
                {"" + ForestConstants.L1_TREE_LOWLEFT_EDGE,
                     "" + ForestConstants.L1_EMPTY_BLOCK,
                    "-",},
                new string[]
                {"-",
                     "" + ForestConstants.L2_STUMP_LEFT,
                    "-",},
                1), copy1, layer1, layer2);

            // fix right sides of trees
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                     "" + ForestConstants.L1_TREE_UPPERLEFT,
                     "!" + ForestConstants.L1_TREE_UPPERRIGHT,},
                new string[]
                {"" + ForestConstants.L1_TREE_LOWRIGHT_EDGE,
                    "-",},
                new string[]
                {"-",
                    "-",},
                2), copy1, layer1, layer2);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                     "" + ForestConstants.L1_TREE_MIDLEFT,
                     "!" + ForestConstants.L1_TREE_MIDRIGHT,},
                new string[]
                {"" + ForestConstants.L1_EMPTY_BLOCK,
                    "-",},
                new string[]
                {"" + ForestConstants.L2_STUMP_RIGHT,
                    "-",},
                2), copy1, layer1, layer2);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                     "" + ForestConstants.L1_TREE_UPPERRIGHT,
                     "!" + ForestConstants.L1_TREE_UPPERLEFT,},
                new string[]
                {"" + ForestConstants.L1_TREE_UPPERRIGHT_EDGE,
                    "-",},
                new string[]
                {"-",
                    "-",},
                2), copy1, layer1, layer2);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {
                     "" + ForestConstants.L1_TREE_MIDRIGHT,
                     "!" + ForestConstants.L1_TREE_MIDLEFT,},
                new string[]
                {"" + ForestConstants.L1_TREE_MIDRIGHT_EDGE,
                    "-",},
                new string[]
                {"-",
                    "-",},
                2), copy1, layer1, layer2);

            // fix tops of trees
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"!" + ForestConstants.L1_TREE_MIDRIGHT,
                     "" + ForestConstants.L1_TREE_UPPERLEFT,
                     "" + ForestConstants.L1_TREE_MIDLEFT,},
                new string[]
                {"-",
                     "" + ForestConstants.L1_TREE_UPPERLEFT_EDGE,
                     "" + ForestConstants.L1_TREE_MIDLEFT_EDGE,},
                1), copy1, layer1);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"!" + ForestConstants.L1_TREE_MIDLEFT,
                     "" + ForestConstants.L1_TREE_UPPERRIGHT,
                     "" + ForestConstants.L1_TREE_MIDRIGHT,},
                new string[]
                {"-",
                     "" + ForestConstants.L1_TREE_UPPERRIGHT_EDGE,
                     "" + ForestConstants.L1_TREE_MIDRIGHT_EDGE,},
                1), copy1, layer1);

            // fix left sides of trees
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"!" + ForestConstants.L1_TREE_UPPERLEFT,
                    "" + ForestConstants.L1_TREE_UPPERRIGHT,
                 },
                new string[]
                {"-",
                     "" + ForestConstants.L1_TREE_LOWLEFT_EDGE,
                },
                new string[]
                {"-",
                    "-",},
                2), copy1, layer1, layer2);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"!" + ForestConstants.L1_TREE_MIDLEFT,
                     "" + ForestConstants.L1_TREE_MIDRIGHT,
                 },
                new string[]
                {"-",
                     "" + ForestConstants.L1_EMPTY_BLOCK,
                },
                new string[]
                {"-",
                    "" + ForestConstants.L2_STUMP_LEFT,},
                2), copy1, layer1, layer2);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"!" + ForestConstants.L1_TREE_UPPERRIGHT,
                    "" + ForestConstants.L1_TREE_UPPERLEFT,
                 },
                new string[]
                {"-",
                     "" + ForestConstants.L1_TREE_UPPERLEFT_EDGE,
                },
                new string[]
                {"-",
                    "-",},
                2), copy1, layer1, layer2);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"!" + ForestConstants.L1_TREE_MIDRIGHT,
                     "" + ForestConstants.L1_TREE_MIDLEFT,
                 },
                new string[]
                {"-",
                     "" + ForestConstants.L1_TREE_MIDLEFT_EDGE,
                },
                2), copy1, layer1);


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    copy1[x, y] = layer1[x, y];
                    copy2[x, y] = layer2[x, y];
                }
            }

            string ledgeTilesPlusEmpty = "{18,19,34,35,50,51,75,76,103,1,49,33,65,4,52,36,68,145,146,161,162,140}";

            // corners leftover after the above stuff
            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"" + ForestConstants.L1_TREE_UPPERRIGHT_EDGE,
                     "" + ForestConstants.L1_TREE_MIDRIGHT_EDGE,
                     ledgeTilesPlusEmpty,
                 },
                new string[]
                { "" + ForestConstants.L1_EMPTY,
                    "" + ForestConstants.L1_EMPTY,
                     "-",
                },
                new string[]
                { "-",
                      "" + ForestConstants.L2_SHORT_GRASS,
                      "-",
                },
                1), copy1, layer1, layer2);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"" + ForestConstants.L1_TREE_UPPERLEFT_EDGE, ledgeTilesPlusEmpty,
                     "" + ForestConstants.L1_TREE_MIDLEFT_EDGE, ledgeTilesPlusEmpty,
                 },
                new string[]
                { "" + ForestConstants.L1_EMPTY, "-",
                    "" + ForestConstants.L1_EMPTY, "-",

                },
                new string[]
                { "-", "-",
                      "" + ForestConstants.L2_SHORT_GRASS, "-",

                },
                2), copy1, layer1, layer2);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {"" + ForestConstants.L1_TREE_UPPERLEFT_EDGE,
                     "" + ForestConstants.L1_TREE_MIDLEFT_EDGE,
                     ledgeTilesPlusEmpty,
                 },
                new string[]
                { "" + ForestConstants.L1_EMPTY,
                    "" + ForestConstants.L1_EMPTY,
                    "-",
                },
                new string[]
                { "-",
                      "" + ForestConstants.L2_SHORT_GRASS,
                      "-",
                },
                1), copy1, layer1, layer2);

            TileReplacer.applyPattern(new TileReplacer.ReplacementPattern(
                new string[]
                {ledgeTilesPlusEmpty,
                     "" + ForestConstants.L1_TREE_LOWLEFT_EDGE,
                     "" + ForestConstants.L1_EMPTY_BLOCK,
                 },
                new string[]
                { "-",
                    "" + ForestConstants.L1_EMPTY,
                     "" + ForestConstants.L1_EMPTY,
                },
                new string[]
                { "-",
                      "-",
                      "" + ForestConstants.L2_SHORT_GRASS,
                },
                1), copy1, layer1, layer2);

            // grass on layer 2
            // 64 grass in lower right
            // 65 grass in lower left
            // 66 grass in upper right
            // 67 grass in upper left
            for (int i = 0; i < 10; i++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool topMost = y == 0;
                    bool bottomMost = y == height - 1;

                    for (int x = 0; x < width; x++)
                    {
                        bool leftMost = x == 0;
                        bool rightMost = x == width - 1;

                        bool middle = !leftMost && !rightMost && !topMost && !bottomMost;
                        // lower right
                        if (middle &&
                            (layer2[x, y] == ForestConstants.L2_GREEN || layer2[x, y] == ForestConstants.L2_SHORT_GRASS) && // center
                            ForestConstants.isntGrass(layer2[x - 1, y]) &&  // left
                            ForestConstants.isntGrass(layer2[x, y - 1]) &&  // up
                            ForestConstants.isGrass(layer2[x + 1, y])  // right*
                            && ForestConstants.isGrass(layer2[x, y + 1])) // down*
                        {
                            layer2[x, y] = ForestConstants.L2_SHORT_GRASS_BR;
                        }
                        // upper right
                        if (middle &&
                            (layer2[x, y] == ForestConstants.L2_GREEN || layer2[x, y] == ForestConstants.L2_SHORT_GRASS) &&  // center
                            ForestConstants.isntGrass(layer2[x - 1, y]) &&  // left
                            ForestConstants.isGrass(layer2[x, y - 1]) &&  // up*
                            ForestConstants.isGrass(layer2[x + 1, y]) &&  // right*
                            ForestConstants.isntGrass(layer2[x, y + 1])) // down
                        {
                            layer2[x, y] = ForestConstants.L2_SHORT_GRASS_UR;
                        }
                        // lower left
                        if (middle &&
                            (layer2[x, y] == ForestConstants.L2_GREEN || layer2[x, y] == ForestConstants.L2_SHORT_GRASS) &&  // center
                            ForestConstants.isGrass(layer2[x - 1, y]) &&  // left*
                            ForestConstants.isntGrass(layer2[x, y - 1]) &&  // up
                            ForestConstants.isntGrass(layer2[x + 1, y]) &&  // right
                            ForestConstants.isGrass(layer2[x, y + 1])) // down*
                        {
                            layer2[x, y] = ForestConstants.L2_SHORT_GRASS_BL;
                        }
                        // upper left
                        if (middle &&
                            (layer2[x, y] == ForestConstants.L2_GREEN || layer2[x, y] == ForestConstants.L2_SHORT_GRASS) &&  // center
                            ForestConstants.isGrass(layer2[x - 1, y]) &&  // left*
                            ForestConstants.isGrass(layer2[x, y - 1]) &&  // up*
                            ForestConstants.isntGrass(layer2[x + 1, y]) &&  // right
                            ForestConstants.isntGrass(layer2[x, y + 1])) // down
                        {
                            layer2[x, y] = ForestConstants.L2_SHORT_GRASS_UL;
                        }
                    }
                }
            }

            // perlin noise for grass and bush placement
            int perlinZ = (random.Next() % 2000);
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if (layer1[x, y] == ForestConstants.L1_EMPTY && layer2[x, y] == ForestConstants.L2_SHORT_GRASS && PerlinNoise.noise(x * 20 / (double)width, y * 20 / (double)height, perlinZ) > 0.2)
                    {
                        if (layer1[x, y - 1] != ForestConstants.L1_CAVE_DOOR_LEFT && layer1[x, y - 1] != ForestConstants.L1_CAVE_DOOR_RIGHT)
                        {
                            layer1[x, y] = 5;
                        }
                    }
                    else if (layer1[x, y] == ForestConstants.L1_EMPTY && layer2[x, y] == ForestConstants.L2_SHORT_GRASS && PerlinNoise.noise(x * 20 / (double)width, y * 20 / (double)height, perlinZ) < -0.2)
                    {
                        // don't put it right next to other stuff
                        bool ul = ForestConstants.isGrassOrTallGrass(layer1[x - 1, y - 1], layer2[x - 1, y - 1]);
                        bool u = ForestConstants.isGrassOrTallGrass(layer1[x, y - 1], layer2[x, y - 1]);
                        bool ur = ForestConstants.isGrassOrTallGrass(layer1[x + 1, y - 1], layer2[x + 1, y - 1]);
                        bool l = ForestConstants.isGrassOrTallGrass(layer1[x - 1, y], layer2[x - 1, y]);
                        bool r = ForestConstants.isGrassOrTallGrass(layer1[x + 1, y], layer2[x + 1, y]);
                        bool bl = ForestConstants.isGrassOrTallGrass(layer1[x - 1, y + 1], layer2[x - 1, y + 1]);
                        bool b = ForestConstants.isGrassOrTallGrass(layer1[x, y + 1], layer2[x, y + 1]);
                        bool br = ForestConstants.isGrassOrTallGrass(layer1[x + 1, y + 1], layer2[x + 1, y + 1]);
                        if (u && l && r && b)
                        {
                            layer1[x, y] = ForestConstants.L1_TALL_GRASS;
                            layer2[x, y] = ForestConstants.L2_TALL_GRASS_DIRT;
                        }
                    }
                }
            }

            // now put the edges on the grass
            for (int i = 0; i < 10; i++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        if (ForestConstants.isTallGrass(layer1[x, y]))
                        {
                            bool ul = ForestConstants.isTallGrass(layer1[x - 1, y - 1]);
                            bool u = ForestConstants.isTallGrass(layer1[x, y - 1]);
                            bool ur = ForestConstants.isTallGrass(layer1[x + 1, y - 1]);
                            bool l = ForestConstants.isTallGrass(layer1[x - 1, y]);
                            bool r = ForestConstants.isTallGrass(layer1[x + 1, y]);
                            bool bl = ForestConstants.isTallGrass(layer1[x - 1, y + 1]);
                            bool b = ForestConstants.isTallGrass(layer1[x, y + 1]);
                            bool br = ForestConstants.isTallGrass(layer1[x + 1, y + 1]);

                            if (u && l && r && b)
                            {
                                // stay as is
                            }
                            else if (u && r && l && !b)
                            {
                                // bottom
                                layer1[x, y] = ForestConstants.L1_TALL_GRASS_B;
                                layer2[x, y] = ForestConstants.L2_TALL_GRASS_DIRT_B;
                            }
                            else if (!u && r && l && b)
                            {
                                // top
                                layer1[x, y] = ForestConstants.L1_TALL_GRASS_U;
                                layer2[x, y] = ForestConstants.L2_TALL_GRASS_DIRT_U;
                            }
                            else if (u && !r && l && b)
                            {
                                // right
                                layer1[x, y] = ForestConstants.L1_TALL_GRASS_R;
                                layer2[x, y] = ForestConstants.L2_TALL_GRASS_DIRT_R;
                            }
                            else if (u && r && !l && b)
                            {
                                // left
                                layer1[x, y] = ForestConstants.L1_TALL_GRASS_L;
                                layer2[x, y] = ForestConstants.L2_TALL_GRASS_DIRT_L;
                            }
                            else if (!u && r && !l && b)
                            {
                                // upper left
                                layer1[x, y] = ForestConstants.L1_TALL_GRASS_UL;
                                layer2[x, y] = ForestConstants.L2_TALL_GRASS_DIRT_UL;
                            }
                            else if (!u && !r && l && b)
                            {
                                // upper right
                                layer1[x, y] = ForestConstants.L1_TALL_GRASS_UR;
                                layer2[x, y] = ForestConstants.L2_TALL_GRASS_DIRT_UR;
                            }
                            else if (u && r && !l && !b)
                            {
                                // lower left
                                layer1[x, y] = ForestConstants.L1_TALL_GRASS_BL;
                                layer2[x, y] = ForestConstants.L2_TALL_GRASS_DIRT_BL;
                            }
                            else if (u && !r && l && !b)
                            {
                                // lower right
                                layer1[x, y] = ForestConstants.L1_TALL_GRASS_BR;
                                layer2[x, y] = ForestConstants.L2_TALL_GRASS_DIRT_BR;
                            }
                            else
                            {
                                // nothing
                                layer1[x, y] = ForestConstants.L1_EMPTY;
                                layer2[x, y] = 97;
                            }
                        }
                    }
                }
            }
        }


    }
}
