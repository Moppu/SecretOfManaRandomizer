using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.ancientcave.mapgen.forest
{
    /// <summary>
    /// Utility go generate ledges with caves for "forest" tileset floors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ForestMountainLedges
    {
        public static void makeRandomLedgesWithCaves(AncientCaveMap map, RandoContext context, List<int> mountainLedgeYValues, List<ProcGenMapNode> allNodes, int ledgeHeight)
        {
            byte[,] layer1 = map.layer1Data;
            int mapWidth = layer1.GetLength(0);
            Random random = context.randomFunctional;
            int numStraightPieces = 0;

            int numIndoorMaps = (random.Next() % 3) + 4;

            List<int> doorPossibleXValues = new List<int>();
            List<int> doorPossibleYValues = new List<int>();
            foreach (int yValue in mountainLedgeYValues)
            {
                List<int> stepsPositions = new List<int>();
                foreach (ProcGenMapNode n1 in allNodes)
                {
                    foreach (ProcGenMapNode n2 in n1.connections)
                    {
                        for (int x = 0; x < mapWidth - 1; x++)
                        {
                            // if x,yValue left of line, and x+1,yValue right of line, put steps here
                            if (n1.y != n2.y)
                            {
                                // straight line, special case
                                if (n1.x == n2.x)
                                {
                                    if (x == n1.x && ((n1.y < yValue + 5 && n2.y >= yValue - 5) || (n2.y < yValue + 5 && n1.y >= yValue - 5)))
                                    {
                                        stepsPositions.Add(x);
                                    }
                                }
                                else
                                {
                                    // line crosses this y boundary
                                    if ((n1.y < yValue + 5 && n2.y >= yValue - 5) || (n2.y < yValue + 5 && n1.y >= yValue - 5))
                                    {
                                        double m = (n2.y - n1.y) / (double)(n2.x - n1.x);
                                        // y=mx+b
                                        // b=y-mx
                                        double b = n2.y - (m * n2.x);
                                        // x=(y-b)/m
                                        double xAtThisY = (yValue - b) / m;
                                        if (x <= xAtThisY && x + 1 > xAtThisY)
                                        {
                                            stepsPositions.Add(x);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                int yPos = yValue;
                int prevYPos = yValue;
                bool steps = false;
                for (int x = 8; x < mapWidth - 8; x++)
                {
                    if (x == 8)
                    {
                        // left endcap
                        layer1[x, yPos - 2] = 6;
                        layer1[x, yPos - 1] = 63;
                        layer1[x, yPos - 0] = 79;
                        layer1[x, yPos + 1] = 54;
                    }
                    else if (x == mapWidth - 9)
                    {
                        // right endcap
                        // 9, 62, 78, 68
                        layer1[x, yPos - 2] = 9;
                        layer1[x, yPos - 1] = 62;
                        layer1[x, yPos - 0] = 78;
                        layer1[x, yPos + 1] = 68;
                    }
                    else if (yPos == prevYPos)
                    {
                        // straight piece
                        if ((numStraightPieces % 2) == 0)
                        {
                            steps = (stepsPositions.Contains(x) || stepsPositions.Contains(x - 1));
                            if (steps)
                            {
                                layer1[x, yPos - 2] = ForestConstants.L1_STEPS;
                                layer1[x, yPos - 1] = ForestConstants.L1_STEPS;
                                layer1[x, yPos - 0] = ForestConstants.L1_STEPS;
                                layer1[x, yPos + 1] = ForestConstants.L1_STEPS;
                            }
                            else
                            {
                                layer1[x, yPos - 2] = ForestConstants.L1_LEDGE_LEFT_TOPMOST;
                                layer1[x, yPos - 1] = ForestConstants.L1_LEDGE_LEFT_MIDTOP;
                                layer1[x, yPos - 0] = ForestConstants.L1_LEDGE_LEFT_MIDBOTTOM;
                                layer1[x, yPos + 1] = ForestConstants.L1_LEDGE_LEFT_BOTTOMMOST;
                            }
                        }
                        else
                        {
                            if (steps)
                            {
                                layer1[x, yPos - 2] = ForestConstants.L1_STEPS;
                                layer1[x, yPos - 1] = ForestConstants.L1_STEPS;
                                layer1[x, yPos - 0] = ForestConstants.L1_STEPS;
                                layer1[x, yPos + 1] = ForestConstants.L1_STEPS;
                            }
                            else
                            {
                                layer1[x, yPos - 2] = ForestConstants.L1_LEDGE_RIGHT_TOPMOST;
                                layer1[x, yPos - 1] = ForestConstants.L1_LEDGE_RIGHT_MIDTOP;
                                layer1[x, yPos - 0] = ForestConstants.L1_LEDGE_RIGHT_MIDBOTTOM;
                                layer1[x, yPos + 1] = ForestConstants.L1_LEDGE_RIGHT_BOTTOMMOST;
                            }
                        }
                        numStraightPieces++;

                        if ((numStraightPieces % 2) == 1 && !steps)
                        {
                            if (layer1[x, yPos + 2] == ForestConstants.L1_EMPTY)
                            {
                                doorPossibleXValues.Add(x);
                                doorPossibleYValues.Add(yPos);
                            }
                        }
                    }
                    else if (yPos > prevYPos)
                    {
                        // \ piece
                        layer1[x, yPos - 3] = 1;
                        layer1[x, yPos - 2] = 49;
                        layer1[x, yPos - 1] = 33;
                        layer1[x, yPos + 0] = 49;
                        layer1[x, yPos + 1] = 65;
                        numStraightPieces = 0;
                        steps = false;
                    }
                    else
                    {
                        // / piece
                        layer1[x, yPos - 2] = 4;
                        layer1[x, yPos - 1] = 52;
                        layer1[x, yPos - 0] = 36;
                        layer1[x, yPos + 1] = 52;
                        layer1[x, yPos + 2] = 68;
                        numStraightPieces = 0;
                        steps = false;
                    }

                    prevYPos = yPos;
                    if (numStraightPieces > 0 && (numStraightPieces % 2) == 0 && (random.Next() % 2) == 0 && !stepsPositions.Contains(x) && !stepsPositions.Contains(x + 1) && x < mapWidth - 10)
                    {
                        if (yPos <= yValue - ledgeHeight)
                        {
                            yPos++;
                        }
                        else if (yPos >= yValue + ledgeHeight)
                        {
                            yPos--;
                        }
                        else
                        {
                            yPos += (random.Next() % 2) == 0 ? -1 : 1;
                        }
                    }
                }
            }

            for (int i = 0; i < numIndoorMaps && doorPossibleXValues.Count > 0; i++)
            {
                int index = random.Next() % doorPossibleXValues.Count;
                int x = doorPossibleXValues[index];
                int y = doorPossibleYValues[index];
                doorPossibleXValues.RemoveAt(index);
                doorPossibleYValues.RemoveAt(index);
                layer1[x, y] = ForestConstants.L1_CAVE_TOP_LEFT;
                layer1[x + 1, y] = ForestConstants.L1_CAVE_TOP_RIGHT;
                layer1[x, y + 1] = ForestConstants.L1_CAVE_DOOR_LEFT;
                layer1[x + 1, y + 1] = ForestConstants.L1_CAVE_DOOR_RIGHT;
                // set door ids/locations for connection to the other map later
                map.altMapExitLocations[new XyPos((short)x, (short)(y + 1))] = i;
                map.altMapExitLocations[new XyPos((short)(x + 1), (short)(y + 1))] = i;
                map.altMapEntryLocations[i] = new XyPos((short)x, (short)(y + 2)); // one below
            }
        }
    }
}
