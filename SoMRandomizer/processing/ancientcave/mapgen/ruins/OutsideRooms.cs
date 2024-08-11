using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System.Collections.Generic;

namespace SoMRandomizer.processing.ancientcave.mapgen.ruins
{
    /// <summary>
    /// Utility to make indoor rooms for "ruins" tileset floors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OutsideRooms
    {
        public static void makeRooms(byte[,] layer1, byte[,] layer2, AncientCaveMap outdoorMap, RandoContext context, AncientCaveGenerationContext generationContext)
        {
            Dictionary<string, ProcGenMapNode> allNodes = generationContext.getSharedNodesByName();
            // 16x9 balcony segment w/ door, pillars, surrounded by solid walls
            // 177, 181 are the door/trigger tiles
            byte[] layer1BaseRoom = new byte[]
            {
                127, 125, 127, 125, 127,  84,  85, 157, 159,  84,  85, 125, 127, 125, 127, 125,
                127, 125, 127, 125, 127,  84,  85, 176, 180,  84,  85, 125, 127, 125, 127, 125,
                127, 157, 159, 157, 159,  84,  85, 177, 181,  84,  85, 157, 159, 157, 159, 125,
                144, 170, 170, 170, 170, 116, 117, 170, 170, 116, 117, 170, 170, 170, 170, 145,
                144, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 145,
                144, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 145,
                144, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 171, 145,
                 87,  87,  87,  87,  87,  87,  87,  87,  87,  87,  87,  87,  87,  87,  87,  87,
                102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102,
            };

            ReplacementMapSegment stairsDownLeft = new ReplacementMapSegment();
            stairsDownLeft.x = 1;
            stairsDownLeft.y = 7;
            stairsDownLeft.w = 4;
            stairsDownLeft.h = 2;
            stairsDownLeft.data = new byte[] {
                73, 171, 171, 73,
                105, 88, 72, 105
            };

            ReplacementMapSegment stairsDownRight = new ReplacementMapSegment();
            stairsDownRight.x = 11;
            stairsDownRight.y = 7;
            stairsDownRight.w = 4;
            stairsDownRight.h = 2;
            stairsDownRight.data = new byte[] {
                73, 171, 171, 73,
                105, 88, 72, 105
            };

            ReplacementMapSegment stairsUpLeft = new ReplacementMapSegment();
            stairsUpLeft.x = 1;
            stairsUpLeft.y = 0;
            stairsUpLeft.w = 4;
            stairsUpLeft.h = 3;
            stairsUpLeft.data = new byte[] {
                105, 104, 72, 105,
                82, 104, 72, 82,
                98, 104, 72, 98,
            };

            ReplacementMapSegment stairsUpRight = new ReplacementMapSegment();
            stairsUpRight.x = 11;
            stairsUpRight.y = 0;
            stairsUpRight.w = 4;
            stairsUpRight.h = 3;
            stairsUpRight.data = new byte[] {
                105, 104, 72, 105,
                82, 104, 72, 82,
                98, 104, 72, 98,
            };

            ReplacementMapSegment noWallRight = new ReplacementMapSegment();
            noWallRight.x = 15;
            noWallRight.y = 2;
            noWallRight.w = 1;
            noWallRight.h = 7;
            noWallRight.data = new byte[] { 157, 170, 171, 171, 171, 87, 102 };

            ReplacementMapSegment noWallLeft = new ReplacementMapSegment();
            noWallLeft.x = 0;
            noWallLeft.y = 2;
            noWallLeft.w = 1;
            noWallLeft.h = 7;
            noWallLeft.data = new byte[] { 159, 170, 171, 171, 171, 87, 102 };

            ReplacementMapSegment bridge = new ReplacementMapSegment();
            bridge.x = 16;
            bridge.y = 2;
            bridge.w = 2;
            bridge.h = 7;
            bridge.data = new byte[] {
                159, 157,
                170, 170,
                171, 171,
                171, 171,
                171, 171,
                87, 87,
                102, 102,};

            foreach (ProcGenMapNode n in allNodes.Values)
            {
                if (n.name.StartsWith("o"))
                {
                    int ypos = n.values["ypos"];
                    int xpos = n.values["xpos"];
                    // same id as the door in InsideRooms
                    int insideDoorId = (ypos << 16) + xpos;
                    if (ypos <= 6)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                int mapPieceXPos = n.x + x;
                                int mapPieceYPos = n.y + y;
                                layer1[mapPieceXPos, mapPieceYPos] = layer1BaseRoom[y * 16 + x];
                                if (layer1[mapPieceXPos, mapPieceYPos] == 177 || layer1[mapPieceXPos, mapPieceYPos] == 181)
                                {
                                    // create door to indoor map
                                    outdoorMap.altMapExitLocations[new XyPos(mapPieceXPos, mapPieceYPos)] = insideDoorId;
                                    if (layer1[mapPieceXPos, mapPieceYPos] == 177)
                                    {
                                        // left side exit
                                        outdoorMap.altMapEntryLocations[insideDoorId] = new XyPos(mapPieceXPos, mapPieceYPos + 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (ProcGenMapNode n in allNodes.Values)
            {
                int ypos = n.values["ypos"];
                if (n.name.StartsWith("o") && ypos <= 6)
                {
                    int xpos = n.values["xpos"];
                    string leftNodeKey = "o_" + ypos + "_" + (xpos - 1);
                    if (allNodes.ContainsKey(leftNodeKey))
                    {
                        ProcGenMapNode leftNode = allNodes[leftNodeKey];
                        if (n.connections.Contains(leftNode))
                        {
                            ReplacementMapSegment.applyReplacement(layer1, n.x, n.y, noWallLeft);
                        }
                    }

                    string rightNodeKey = "o_" + ypos + "_" + (xpos + 1);
                    if (allNodes.ContainsKey(rightNodeKey))
                    {
                        ProcGenMapNode rightNode = allNodes[rightNodeKey];
                        if (n.connections.Contains(rightNode))
                        {
                            ReplacementMapSegment.applyReplacement(layer1, n.x, n.y, noWallRight);
                            for (int i = 0; i < 8; i += 2)
                            {
                                ReplacementMapSegment.applyReplacement(layer1, n.x + i, n.y, bridge);
                                // shadow under the bridge
                                for (int y = 0; y < 6; y++)
                                {
                                    if (layer1[n.x + i + bridge.x, n.y + y + bridge.h] == 122)
                                    {
                                        layer1[n.x + i + bridge.x, n.y + y + bridge.h] = 125;
                                    }
                                    if (layer1[n.x + i + bridge.x, n.y + y + bridge.h] == 124)
                                    {
                                        layer1[n.x + i + bridge.x, n.y + y + bridge.h] = 127;
                                    }
                                    if (layer1[n.x + i + bridge.x + 1, n.y + y + bridge.h] == 122)
                                    {
                                        layer1[n.x + i + bridge.x + 1, n.y + y + bridge.h] = 125;
                                    }
                                    if (layer1[n.x + i + bridge.x + 1, n.y + y + bridge.h] == 124)
                                    {
                                        layer1[n.x + i + bridge.x + 1, n.y + y + bridge.h] = 127;
                                    }

                                }
                            }
                        }
                    }
                }
            }

            foreach (ProcGenMapNode n in allNodes.Values)
            {
                if (n.name.StartsWith("o"))
                {
                    int xpos = n.values["xpos"];
                    int ypos = n.values["ypos"];
                    // xpos+ypos even for bottom node, use right steps, odd use left
                    bool rightSteps = ((xpos + ypos) % 2) == 0;
                    string topNodeKey = "o_" + (ypos + 1) + "_" + xpos;
                    if (allNodes.ContainsKey(topNodeKey) && (ypos < 6 || allNodes[topNodeKey].values["mainpath"] == 1))
                    {
                        ProcGenMapNode topNode = allNodes[topNodeKey];
                        if (n.connections.Contains(topNode))
                        {
                            ReplacementMapSegment.applyReplacement(layer1, n.x, n.y, rightSteps ? stairsUpRight : stairsUpLeft);
                            ReplacementMapSegment.applyReplacement(layer1, topNode.x, topNode.y, rightSteps ? stairsDownRight : stairsDownLeft);
                        }
                    }
                }
            }
        }
    }
}
