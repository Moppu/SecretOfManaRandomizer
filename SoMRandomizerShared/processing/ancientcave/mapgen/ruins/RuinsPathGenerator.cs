using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.ancientcave.mapgen.ruins
{
    /// <summary>
    /// Utility to make a solvable path through rooms for "ruins" tileset floors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class RuinsPathGenerator
    {
        public static void makePaths(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            int numNodesX = RuinsGenerator.numNodesX;
            int numNodesY = RuinsGenerator.numNodesY;
            int nodeWidth = RuinsGenerator.nodeWidth;
            int mapHeight = generationContext.outdoorMapHeight;
            int nodeHeightIndoor = RuinsGenerator.nodeHeightIndoor;
            int nodeHeightOutdoor = RuinsGenerator.nodeHeightOutdoor;
            Random random = context.randomFunctional;

            // y levels .. every .. 9? starting at y=70
            Dictionary<string, ProcGenMapNode> allNodes = new Dictionary<string, ProcGenMapNode>();
            List<ProcGenMapNode> visitedNodes = new List<ProcGenMapNode>();
            ProcGenMapNode roofNode = null;
            for (int floorNum = 2; floorNum <= numNodesY; floorNum++)
            {
                // 8 nodes per floor
                for (int x = 0; x < numNodesX; x++)
                {
                    // inside nodes
                    ProcGenMapNode innerNode = new ProcGenMapNode();
                    innerNode.name = "i" + "_" + floorNum + "_" + x;
                    innerNode.x = nodeWidth / 2 + x * nodeWidth;
                    innerNode.centerX = innerNode.x;
                    innerNode.y = mapHeight - ((floorNum) * nodeHeightIndoor);
                    innerNode.centerY = innerNode.y;
                    innerNode.values["xpos"] = x;
                    innerNode.values["ypos"] = floorNum;
                    innerNode.values["mainpath"] = 0;
                    allNodes.Add(innerNode.name, innerNode);

                    // outside nodes
                    ProcGenMapNode outerNode = new ProcGenMapNode();
                    outerNode.name = "o" + "_" + floorNum + "_" + x;
                    outerNode.x = nodeWidth / 2 + x * nodeWidth;
                    outerNode.centerX = outerNode.x;
                    outerNode.y = mapHeight - (70 + (floorNum - 2) * nodeHeightOutdoor);
                    outerNode.centerY = outerNode.y;
                    outerNode.values["xpos"] = x;
                    outerNode.values["ypos"] = floorNum;
                    outerNode.values["mainpath"] = 0;
                    allNodes.Add(outerNode.name, outerNode);

                    // first floor is autovisit
                    if (floorNum == 2)
                    {
                        visitedNodes.Add(innerNode);
                        visitedNodes.Add(outerNode);
                    }
                }
            }

            int lowerNodeNum = random.Next() % numNodesX;
            bool lowerNodeOuter = true;

            for (int floorNum = 2; floorNum < numNodesY; floorNum++)
            {
                // create main path
                if (floorNum == 2)
                {
                    // connect all indoor nodes
                    for (int x = 0; x < numNodesX - 1; x++)
                    {
                        ProcGenMapNode n1 = allNodes["i_2_" + x];
                        ProcGenMapNode n2 = allNodes["i_2_" + (x + 1)];
                        n1.connections.Add(n2);
                        n2.connections.Add(n1);
                    }
                }

                // now find a random path from lowerNodeNum to upperNodeNum
                int upperNodeNum = random.Next() % numNodesX;
                int hDist = Math.Abs(lowerNodeNum - upperNodeNum);
                Logging.log("Making path from floor " + floorNum + " node " + lowerNodeNum + " to floor " + (floorNum + 1) + " node " + upperNodeNum + (lowerNodeOuter ? " outside" : " inside"), "debug");

                if (hDist == 0)
                {
                    // same h pos, connect straight up
                    ProcGenMapNode n1 = allNodes[(lowerNodeOuter ? "o" : "i") + "_" + floorNum + "_" + lowerNodeNum];
                    ProcGenMapNode n2 = allNodes[(lowerNodeOuter ? "o" : "i") + "_" + (floorNum + 1) + "_" + upperNodeNum];
                    Logging.log("  Node " + n1.name + " connects to " + n2.name, "debug");
                    ProcGenMapNode n2_opposite = allNodes[(lowerNodeOuter ? "i" : "o") + "_" + (floorNum + 1) + "_" + upperNodeNum];
                    n1.connections.Add(n2);
                    n2.connections.Add(n1);
                    n1.values["mainpath"] = 1;
                    n2.values["mainpath"] = 1;
                    n2_opposite.values["mainpath"] = 1;
                    if (floorNum == 6)
                    {
                        roofNode = n2;
                    }
                    if (!visitedNodes.Contains(n2))
                    {
                        visitedNodes.Add(n2);
                    }
                    if (!visitedNodes.Contains(n2_opposite))
                    {
                        visitedNodes.Add(n2_opposite);
                    }
                }
                else
                {
                    int vChangePos = random.Next() % hDist;
                    int xPos = lowerNodeNum;
                    int xChange = 1;
                    if (upperNodeNum < lowerNodeNum)
                    {
                        xChange = -1;
                    }
                    bool upper = false;
                    for (int x = 0; x < hDist; x++)
                    {
                        if (x == vChangePos)
                        {
                            ProcGenMapNode n1 = allNodes[(lowerNodeOuter ? "o" : "i") + "_" + floorNum + "_" + xPos];
                            ProcGenMapNode n2 = allNodes[(lowerNodeOuter ? "o" : "i") + "_" + (floorNum + 1) + "_" + xPos];
                            Logging.log("  Node " + n1.name + " connects to " + n2.name, "debug");
                            n1.connections.Add(n2);
                            n2.connections.Add(n1);
                            n1.values["mainpath"] = 1;
                            n2.values["mainpath"] = 1;
                            if (floorNum == 6)
                            {
                                roofNode = n2;
                            }
                            if (!visitedNodes.Contains(n2))
                            {
                                visitedNodes.Add(n2);
                            }
                            upper = true;
                        }

                        {
                            int f = floorNum;
                            if (upper)
                            {
                                f++;
                            }
                            ProcGenMapNode n1 = allNodes[(lowerNodeOuter ? "o" : "i") + "_" + f + "_" + xPos];
                            ProcGenMapNode n2 = allNodes[(lowerNodeOuter ? "o" : "i") + "_" + f + "_" + (xPos + xChange)];
                            Logging.log("  Node " + n1.name + " connects to " + n2.name, "debug");
                            n1.connections.Add(n2);
                            n2.connections.Add(n1);
                            n1.values["mainpath"] = 1;
                            n2.values["mainpath"] = 1;
                            if (!visitedNodes.Contains(n2))
                            {
                                visitedNodes.Add(n2);
                            }
                        }
                        xPos += xChange;
                    }

                    ProcGenMapNode final_opposite = allNodes[(lowerNodeOuter ? "i" : "o") + "_" + (floorNum + 1) + "_" + upperNodeNum];
                    visitedNodes.Add(final_opposite);
                    final_opposite.values["mainpath"] = 1;
                }
                lowerNodeNum = upperNodeNum;
                lowerNodeOuter = !lowerNodeOuter;
            }

            Logging.log("Main path created.", "debug");
            foreach (ProcGenMapNode n in visitedNodes)
            {
                Logging.log("Visited node: " + n.name, "debug");
            }

            // now connect all the unvisited nodes
            while (allNodes.Count != visitedNodes.Count)
            {
                foreach (ProcGenMapNode n in allNodes.Values)
                {
                    bool outside = n.name.StartsWith("o");
                    if (!visitedNodes.Contains(n) || n.values["mainpath"] == 0)
                    {
                        List<ProcGenMapNode> possibleConnections = new List<ProcGenMapNode>();
                        if (n.values["xpos"] != 0)
                        {
                            ProcGenMapNode leftNode = allNodes[(outside ? "o" : "i") + "_" + n.values["ypos"] + "_" + (n.values["xpos"] - 1)];
                            if (visitedNodes.Contains(leftNode) || leftNode.values["mainpath"] == 0)
                            {
                                possibleConnections.Add(leftNode);
                            }
                        }
                        if (n.values["xpos"] != numNodesX - 1)
                        {
                            ProcGenMapNode rightNode = allNodes[(outside ? "o" : "i") + "_" + n.values["ypos"] + "_" + (n.values["xpos"] + 1)];
                            if (visitedNodes.Contains(rightNode) || rightNode.values["mainpath"] == 0)
                            {
                                possibleConnections.Add(rightNode);
                            }
                        }
                        if (n.values["ypos"] < numNodesY - 1)
                        {
                            ProcGenMapNode topNode = allNodes[(outside ? "o" : "i") + "_" + (n.values["ypos"] + 1) + "_" + n.values["xpos"]];
                            if (visitedNodes.Contains(topNode) || topNode.values["mainpath"] == 0)
                            {
                                possibleConnections.Add(topNode);
                            }
                        }
                        if (n.values["ypos"] != 2)
                        {
                            ProcGenMapNode bottomNode = allNodes[(outside ? "o" : "i") + "_" + (n.values["ypos"] - 1) + "_" + n.values["xpos"]];
                            if (visitedNodes.Contains(bottomNode) || bottomNode.values["mainpath"] == 0)
                            {
                                possibleConnections.Add(bottomNode);
                            }
                        }

                        if (possibleConnections.Count > 0)
                        {
                            int index = random.Next() % possibleConnections.Count;
                            ProcGenMapNode otherNode = possibleConnections[index];
                            n.connections.Add(otherNode);
                            otherNode.connections.Add(n);
                            if (!visitedNodes.Contains(n))
                            {
                                visitedNodes.Add(n);
                            }
                            ProcGenMapNode oppositeNode = allNodes[outside ? n.name.Replace("o", "i") : n.name.Replace("i", "o")];
                            if (!visitedNodes.Contains(oppositeNode))
                            {
                                visitedNodes.Add(oppositeNode);
                            }
                        }
                    }
                }
            }

            generationContext.sharedNodes.AddRange(allNodes.Values);
            generationContext.specialNodes[RuinsGenerator.roofNodeName] = roofNode;
        }
    }
}
