using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.ancientcave.mapgen.ruins
{
    /// <summary>
    /// Utility to make indoor rooms for "ruins" tileset floors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class InsideRooms
    {
        public static void makeRooms(byte[,] layer1, byte[,] layer2, AncientCaveMap indoorMap, RandoContext context, AncientCaveGenerationContext generationContext)
        {
            Random random = context.randomFunctional;
            Dictionary<string, ProcGenMapNode> allNodes = generationContext.getSharedNodesByName();
            // 16x15
            // purple floor
            // 181 is the door tile here; create doors (to this same map!) for each
            // 65 is a door to the outside map; create doors for each to the outside map
            byte[] layer1BaseRoom1Door = new byte[]
                {
                  1,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   3,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  89,  89,  89,  89,  89, 105, 180, 152,  89,  89,  89,  89,  89,  89,  19,
                 17, 136, 136, 136, 136, 136, 137, 181, 135, 136, 136, 136, 136, 136, 136,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 33,  34,  34,  34,  34,  34,   5, 103,  4,   34,  34,  34,  34,  34,  34,  35,
                 18,  18,  18,  18,  18,  18,  33,  34,  35,  18,  18,  18,  18,  18,  18,  18,
                 18,  18,  18,  18,  18,  18,  18,  65,  18,  18,  18,  18,  18,  18,  18,  18,
                };

            byte[] layer1BaseRoom1NoDoor = new byte[]
                {
                  1,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   3,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  89,  89,  89,  89,  89,  89,  89,  89,  89,  89,  89,  89,  89,  89,  19,
                 17, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 17, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103, 103,  19,
                 33,  34,  34,  34,  34,  34,   5, 103,  4,   34,  34,  34,  34,  34,  34,  35,
                 18,  18,  18,  18,  18,  18,  33,  34,  35,  18,  18,  18,  18,  18,  18,  18,
                 18,  18,  18,  18,  18,  18,  18,  65,  18,  18,  18,  18,  18,  18,  18,  18,
                };

            // green floor
            byte[] layer1BaseRoom2Door = new byte[]
                {
                  1,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   3,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  89,  89,  89,  89,  89, 105, 180, 152,  89,  89,  89,  89,  89,  89,  19,
                 17, 136, 136, 136, 136, 136, 137, 181, 135, 136, 136, 136, 136, 136, 136,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 33,  34,  34,  34,  34,  34,   5, 100,   4,  34,  34,  34,  34,  34,  34,  35,
                 18,  18,  18,  18,  18,  18,  33,  34,  35,  18,  18,  18,  18,  18,  18,  18,
                 18,  18,  18,  18,  18,  18,  18,  65,  18,  18,  18,  18,  18,  18,  18,  18,
                };

            byte[] layer1BaseRoom2NoDoor = new byte[]
                {
                  1,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   3,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  89,  89,  89,  89,  89,  89,  89,  89,  89,  89,  89,  89,  89,  89,  19,
                 17, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136, 136,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 17, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,  19,
                 33,  34,  34,  34,  34,  34,   5, 100,   4,  34,  34,  34,  34,  34,  34,  35,
                 18,  18,  18,  18,  18,  18,  33,  34,  35,  18,  18,  18,  18,  18,  18,  18,
                 18,  18,  18,  18,  18,  18,  18,  65,  18,  18,  18,  18,  18,  18,  18,  18,
                };

            ReplacementMapSegment stairsDownLeft = new ReplacementMapSegment();
            stairsDownLeft.x = 2;
            stairsDownLeft.y = 12;
            stairsDownLeft.w = 4;
            stairsDownLeft.h = 3;
            stairsDownLeft.data = new byte[] {
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
            };

            ReplacementMapSegment stairsDownRight = new ReplacementMapSegment();
            stairsDownRight.x = 10;
            stairsDownRight.y = 12;
            stairsDownRight.w = 4;
            stairsDownRight.h = 3;
            stairsDownRight.data = new byte[] {
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
            };

            ReplacementMapSegment l2stairsDownLeft = new ReplacementMapSegment();
            l2stairsDownLeft.x = 2;
            l2stairsDownLeft.y = 12;
            l2stairsDownLeft.w = 4;
            l2stairsDownLeft.h = 3;
            l2stairsDownLeft.data = new byte[] {
                154,   0,   0, 155,
                170,   0,   0, 171,
                170,   0,   0, 171,
            };

            ReplacementMapSegment l2stairsDownRight = new ReplacementMapSegment();
            l2stairsDownRight.x = 10;
            l2stairsDownRight.y = 12;
            l2stairsDownRight.w = 4;
            l2stairsDownRight.h = 3;
            l2stairsDownRight.data = new byte[] {
                154,   0,   0, 155,
                170,   0,   0, 171,
                170,   0,   0, 171,
            };

            ReplacementMapSegment stairsUpLeft = new ReplacementMapSegment();
            stairsUpLeft.x = 2;
            stairsUpLeft.y = 0;
            stairsUpLeft.w = 4;
            stairsUpLeft.h = 7;
            stairsUpLeft.data = new byte[] {
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
            };

            ReplacementMapSegment stairsUpRight = new ReplacementMapSegment();
            stairsUpRight.x = 10;
            stairsUpRight.y = 0;
            stairsUpRight.w = 4;
            stairsUpRight.h = 7;
            stairsUpRight.data = new byte[] {
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
                120, 108, 108, 120,
            };

            ReplacementMapSegment l2stairsUpLeft = new ReplacementMapSegment();
            l2stairsUpLeft.x = 2;
            l2stairsUpLeft.y = 0;
            l2stairsUpLeft.w = 4;
            l2stairsUpLeft.h = 7;
            l2stairsUpLeft.data = new byte[] {
                170,   0,   0, 171,
                170,   0,   0, 171,
                170,   0,   0, 171,
                170,   0,   0, 171,
                170,   0,   0, 171,
                170,   0,   0, 171,
                  0,   0,   0,   0,
            };

            ReplacementMapSegment l2stairsUpRight = new ReplacementMapSegment();
            l2stairsUpRight.x = 10;
            l2stairsUpRight.y = 0;
            l2stairsUpRight.w = 4;
            l2stairsUpRight.h = 7;
            l2stairsUpRight.data = new byte[] {
                170,   0,   0, 171,
                170,   0,   0, 171,
                170,   0,   0, 171,
                170,   0,   0, 171,
                170,   0,   0, 171,
                170,   0,   0, 171,
                  0,   0,   0,   0,
            };

            ReplacementMapSegment noWallRightRoom1 = new ReplacementMapSegment();
            noWallRightRoom1.x = 15;
            noWallRightRoom1.y = 0;
            noWallRightRoom1.w = 1;
            noWallRightRoom1.h = 15;
            noWallRightRoom1.data = new byte[] { 2, 56, 56, 56, 89, 136, 103, 103, 103, 103, 103, 103, 34, 18, 18 };

            ReplacementMapSegment noWallLeftRoom1 = new ReplacementMapSegment();
            noWallLeftRoom1.x = 0;
            noWallLeftRoom1.y = 0;
            noWallLeftRoom1.w = 1;
            noWallLeftRoom1.h = 15;
            noWallLeftRoom1.data = new byte[] { 2, 56, 56, 56, 89, 136, 103, 103, 103, 103, 103, 103, 34, 18, 18 };

            ReplacementMapSegment noWallRightRoom2 = new ReplacementMapSegment();
            noWallRightRoom2.x = 15;
            noWallRightRoom2.y = 0;
            noWallRightRoom2.w = 1;
            noWallRightRoom2.h = 15;
            noWallRightRoom2.data = new byte[] { 2, 56, 56, 56, 89, 136, 100, 100, 100, 100, 100, 100, 34, 18, 18 };

            ReplacementMapSegment noWallLeftRoom2 = new ReplacementMapSegment();
            noWallLeftRoom2.x = 0;
            noWallLeftRoom2.y = 0;
            noWallLeftRoom2.w = 1;
            noWallLeftRoom2.h = 15;
            noWallLeftRoom2.data = new byte[] { 2, 56, 56, 56, 89, 136, 100, 100, 100, 100, 100, 100, 34, 18, 18 };

            int roomXOffset = 0;
            int roomYOffset = 0;

            int hallwayDoorNum = 0;
            foreach (ProcGenMapNode n in allNodes.Values)
            {
                if (n.name.StartsWith("i"))
                {
                    int floorNum = n.values["ypos"];
                    int xPos = n.values["xpos"];
                    // same id as the door in OutsideRooms
                    int outsideDoorId = (floorNum << 16) + xPos;
                    // place indoor rooms on the edges
                    bool indoorRoom = xPos == 0 || xPos == 3;
                    if (floorNum <= 6)
                    {
                        bool room1 = (floorNum % 2) == 0;
                        for (int y = 0; y < 15; y++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                int mapPieceXPos = n.x + x + roomXOffset;
                                int mapPieceYPos = n.y + y + roomYOffset;
                                layer1[mapPieceXPos, mapPieceYPos] =
                                    indoorRoom ? (room1 ? layer1BaseRoom1Door[y * 16 + x] : layer1BaseRoom2Door[y * 16 + x]) :
                                    (room1 ? layer1BaseRoom1NoDoor[y * 16 + x] : layer1BaseRoom2NoDoor[y * 16 + x]);
                                if (layer1[mapPieceXPos, mapPieceYPos] == 65)
                                {
                                    layer2[mapPieceXPos, mapPieceYPos] = 164;
                                }
                                if (layer1[mapPieceXPos, mapPieceYPos] == 65)
                                {
                                    // create door to outdoor map
                                    indoorMap.altMapExitLocations[new XyPos(mapPieceXPos, mapPieceYPos)] = outsideDoorId;
                                    indoorMap.altMapEntryLocations[outsideDoorId] = new XyPos(mapPieceXPos, mapPieceYPos - 1);
                                }
                                if (layer1[mapPieceXPos, mapPieceYPos] == 181)
                                {
                                    // create door to other part of the indoor map, with the room interior
                                    indoorMap.sameMapExitLocations[new XyPos((short)mapPieceXPos, (short)mapPieceYPos)] = hallwayDoorNum++;
                                    // on the other side, the room will have return doors, which don't need an actual door defined, and just go
                                    // back to where you came from.
                                }
                            }
                        }
                    }
                }
            }


            // ///////////
            // solid stuff
            // ///////////
            ReplacementMapSegment l1Table = new ReplacementMapSegment();
            // ignore x/y for these
            l1Table.x = 0;
            l1Table.y = 0;
            l1Table.w = 3;
            l1Table.h = 4;
            l1Table.data = new byte[] {
                10, 11, 12,
                26, 27, 28,
                42, 43, 44,
                58, 59, 60
            };
            int l1TableResizeColumn = 1;
            int l1TableResizeRow = 1;

            ReplacementMapSegment l1Cabinet = new ReplacementMapSegment();
            l1Cabinet.x = 0;
            l1Cabinet.y = 0;
            l1Cabinet.w = 2;
            l1Cabinet.h = 3;
            l1Cabinet.data = new byte[] {
                8, 9,
                24, 25,
                40, 41,
            };

            ReplacementMapSegment l1Bookshelf = new ReplacementMapSegment();
            l1Bookshelf.x = 0;
            l1Bookshelf.y = 0;
            l1Bookshelf.w = 2;
            l1Bookshelf.h = 3;
            l1Bookshelf.data = new byte[] {
                140, 140,
                121, 122,
                123, 124,
            };

            ReplacementMapSegment l1Fireplace = new ReplacementMapSegment();
            l1Fireplace.x = 0;
            l1Fireplace.y = 0;
            l1Fireplace.w = 3;
            l1Fireplace.h = 3;
            l1Fireplace.data = new byte[] {
                138, 139, 15,
                154, 155, 156,
                170, 171, 172
            };

            // R chair 85 l2 on y-1, 38 on l1 at y
            ReplacementMapSegment l1Window1 = new ReplacementMapSegment();
            l1Window1.x = 0;
            l1Window1.y = 0;
            l1Window1.w = 2;
            l1Window1.h = 3;
            l1Window1.data = new byte[] {
                0, 0,
                0, 0,
                0, 0,
            };
            ReplacementMapSegment l2Window1 = new ReplacementMapSegment();
            l2Window1.x = 0;
            l2Window1.y = 0;
            l2Window1.w = 2;
            l2Window1.h = 3;
            l2Window1.data = new byte[] {
                1, 2,
                17, 18,
                33, 34,
            };


            ReplacementMapSegment l1Window2 = new ReplacementMapSegment();
            l1Window2.x = 0;
            l1Window2.y = 0;
            l1Window2.w = 1;
            l1Window2.h = 3;
            l1Window2.data = new byte[] {
                0,
                0,
                0,
            };
            ReplacementMapSegment l2Window2 = new ReplacementMapSegment();
            l2Window2.x = 0;
            l2Window2.y = 0;
            l2Window2.w = 1;
            l2Window2.h = 3;
            l2Window2.data = new byte[] {
                3,
                19,
                35,
            };

            ReplacementMapSegment l2Couch = new ReplacementMapSegment();
            l2Couch.x = 0;
            l2Couch.y = 0;
            l2Couch.w = 5;
            l2Couch.h = 2;
            l2Couch.data = new byte[] {
                103,104,105,106,107,
                119,120,121,122,123,
            };

            List<ReplacementMapSegment> l2TableDecorations = new List<ReplacementMapSegment>();
            ReplacementMapSegment l2PottedPlant = new ReplacementMapSegment();
            l2PottedPlant.x = 0;
            l2PottedPlant.y = 0;
            l2PottedPlant.w = 1;
            l2PottedPlant.h = 2;
            l2PottedPlant.data = new byte[] {
                108,
                124,
            };

            ReplacementMapSegment l2Jar = new ReplacementMapSegment();
            l2Jar.x = 0;
            l2Jar.y = 0;
            l2Jar.w = 1;
            l2Jar.h = 1;
            l2Jar.data = new byte[] {
                88,
            };

            ReplacementMapSegment l2Clock = new ReplacementMapSegment();
            l2Clock.x = 0;
            l2Clock.y = 0;
            l2Clock.w = 1;
            l2Clock.h = 2;
            l2Clock.data = new byte[] {
                128,
                144
            };

            ReplacementMapSegment l2Swords = new ReplacementMapSegment();
            l2Clock.x = 0;
            l2Clock.y = 0;
            l2Clock.w = 2;
            l2Clock.h = 1;
            l2Clock.data = new byte[] {
                56,
                57
            };

            ReplacementMapSegment l2Bottle = new ReplacementMapSegment();
            l2Bottle.x = 0;
            l2Bottle.y = 0;
            l2Bottle.w = 1;
            l2Bottle.h = 1;
            l2Bottle.data = new byte[] {
                150,
            };

            ReplacementMapSegment l2Ball = new ReplacementMapSegment();
            l2Ball.x = 0;
            l2Ball.y = 0;
            l2Ball.w = 1;
            l2Ball.h = 1;
            l2Ball.data = new byte[] {
                151,
            };

            ReplacementMapSegment l2Book = new ReplacementMapSegment();
            l2Book.x = 0;
            l2Book.y = 0;
            l2Book.w = 1;
            l2Book.h = 1;
            l2Book.data = new byte[] {
                152,
            };

            l2TableDecorations.Add(l2PottedPlant);
            l2TableDecorations.Add(l2Jar);
            l2TableDecorations.Add(l2Clock);
            l2TableDecorations.Add(l2Swords);
            l2TableDecorations.Add(l2Bottle);
            l2TableDecorations.Add(l2Ball);
            l2TableDecorations.Add(l2Book);

            foreach (ProcGenMapNode n in allNodes.Values)
            {
                int ypos = n.values["ypos"];
                if (n.name.StartsWith("i") && ypos <= 6)
                {
                    int xpos = n.values["xpos"];
                    bool room1 = (ypos % 2) == 0;

                    string leftNodeKey = "i_" + ypos + "_" + (xpos - 1);
                    if (allNodes.ContainsKey(leftNodeKey))
                    {
                        ProcGenMapNode leftNode = allNodes[leftNodeKey];
                        if (n.connections.Contains(leftNode))
                        {
                            ReplacementMapSegment.applyReplacement(layer1, n.x + roomXOffset, n.y + roomYOffset, room1 ? noWallLeftRoom1 : noWallLeftRoom2);
                        }
                    }

                    string rightNodeKey = "i_" + ypos + "_" + (xpos + 1);
                    if (allNodes.ContainsKey(rightNodeKey))
                    {
                        ProcGenMapNode rightNode = allNodes[rightNodeKey];
                        if (n.connections.Contains(rightNode))
                        {
                            ReplacementMapSegment.applyReplacement(layer1, n.x + roomXOffset, n.y + roomYOffset, room1 ? noWallRightRoom1 : noWallRightRoom2);
                            for (int i = 0; i < 8; i++)
                            {
                                ReplacementMapSegment.applyReplacement(layer1, n.x + roomXOffset + i, n.y + roomYOffset, room1 ? noWallRightRoom1 : noWallRightRoom2);
                            }
                        }
                    }
                }
            }

            // 16x15
            // 4 spaces inbetween rooms on x (nodeWidth = 20)
            //                 v tables start here currently
            // ##ssss####ssss##....##ssss####ssss##
            // ##ssss####ssss##....##ssss####ssss##
            // ##ssss####ssss##....##ssss####ssss##
            // ##ssss####ssss##....##ssss####ssss##
            // ##ssss####ssss##....##ssss####ssss##
            // ##ssss####ssss##....##ssss####ssss##
            // # ssss    ssss #....# ssss    ssss #
            // #              #....#              # <
            // #              #....#              #
            // #              #....#              #
            // #              #....#              # <
            // #              #....#              #
            // # ssss    ssss #....# ssss    ssss #
            // ##ssss#dd#ssss##....##ssss#dd#ssss##
            // ##ssss####ssss##....##ssss####ssss##
            //  ^                 ^
            // total width up to 19 (let's say 10 for now?)
            // total height up to 4
            foreach (ProcGenMapNode n in allNodes.Values)
            {
                int ypos = n.values["ypos"];
                if (n.name.StartsWith("i") && ypos <= 6)
                {
                    int xpos = n.values["xpos"];

                    string rightNodeKey = "i_" + ypos + "_" + (xpos + 1);
                    int width = 16;
                    bool rightNodePresent = false;
                    if (allNodes.ContainsKey(rightNodeKey))
                    {
                        ProcGenMapNode rightNode = allNodes[rightNodeKey];
                        if (n.connections.Contains(rightNode))
                        {
                            width += 4;
                            rightNodePresent = true;
                        }
                    }


                    // window
                    if (rightNodePresent)
                    {
                        if ((ypos % 2) == 0)
                        {
                            ReplacementMapSegment.applyReplacement(layer1, n.x + 16 + roomXOffset + 1, n.y + roomYOffset + 1, l1Window1);
                            ReplacementMapSegment.applyReplacement(layer2, n.x + 16 + roomXOffset + 1, n.y + roomYOffset + 1, l2Window1);
                        }
                        else
                        {
                            if ((random.Next() % 2) == 0)
                            {
                                ReplacementMapSegment.applyReplacement(layer1, n.x + 16 + roomXOffset + 2, n.y + roomYOffset + 1, l1Window2);
                                ReplacementMapSegment.applyReplacement(layer2, n.x + 16 + roomXOffset + 2, n.y + roomYOffset + 1, l2Window2);
                            }
                            else
                            {
                                ReplacementMapSegment.applyReplacement(layer1, n.x + 16 + roomXOffset + 1, n.y + roomYOffset + 1, l1Window2);
                                ReplacementMapSegment.applyReplacement(layer2, n.x + 16 + roomXOffset + 1, n.y + roomYOffset + 1, l2Window2);
                                ReplacementMapSegment.applyReplacement(layer1, n.x + 16 + roomXOffset + 3, n.y + roomYOffset + 1, l1Window2);
                                ReplacementMapSegment.applyReplacement(layer2, n.x + 16 + roomXOffset + 3, n.y + roomYOffset + 1, l2Window2);
                            }
                        }
                    }
                    ReplacementMapSegment table = null;

                    bool stepsUp = false;
                    bool stepsDown = false;
                    foreach (ProcGenMapNode otherNode in n.connections)
                    {
                        if (ypos <= 5 && otherNode.name.Contains("i_" + (ypos + 1)))
                        {
                            stepsUp = true;
                        }
                        if (ypos >= 2 && otherNode.name.Contains("i_" + (ypos - 1)))
                        {
                            stepsDown = true;
                        }
                    }


                    // don't stick it on the main entry stairway, or other steps
                    if ((random.Next() % 4) != 0 && !(ypos == 2 && xpos == 1))
                    {
                        // table
                        // interior width - actual width + 2
                        // 0->7 or 0->3
                        int tWidth = random.Next() % (width - 12);
                        // interior height - actual height + 3
                        int tHeight = random.Next() % 3;
                        // old: y=5 -> 10

                        int tPosx = 1 + random.Next() % (width - 1 - (tWidth + 2));

                        // y is either gonna be 7 or 8 now
                        int tPosy = 7;
                        if ((random.Next() % 2) > 0 && tHeight == 0)
                        {
                            tPosy++;
                        }

                        if (stepsUp && tPosy == 7)
                        {
                            tPosy++;
                            if (tHeight > 0)
                            {
                                tHeight--;
                            }
                        }

                        if (stepsDown)
                        {
                            while (tPosy + tHeight > 8)
                            {
                                if (tHeight > 0)
                                {
                                    tHeight--;
                                }
                                else
                                {
                                    tPosy--;
                                }
                            }
                        }

                        table = new ReplacementMapSegment();
                        table.x = tPosx;
                        table.y = tPosy;
                        table.w = tWidth + l1Table.w - 1;
                        table.h = tHeight + l1Table.h - 1;
                        table.data = new byte[table.w * table.h];
                        bool[] tableSpotsOccupied = new bool[table.w * table.h];
                        int sourceX = 0;
                        int sourceY = 0;
                        for (int y = 0; y < table.h; y++)
                        {
                            sourceX = 0;
                            for (int x = 0; x < table.w; x++)
                            {
                                table.data[y * table.w + x] = l1Table.data[sourceY * l1Table.w + sourceX];
                                tableSpotsOccupied[y * table.w + x] = y == table.h - 1;
                                if (x < l1TableResizeColumn)
                                {
                                    sourceX++;
                                }
                                if (x >= l1TableResizeColumn + tWidth - 1)
                                {
                                    sourceX++;
                                }
                            }

                            if (y < l1TableResizeRow)
                            {
                                sourceY++;
                            }
                            if (y >= l1TableResizeRow + tHeight - 1)
                            {
                                sourceY++;
                            }
                        }
                        ReplacementMapSegment.applyReplacement(layer1, n.x + roomXOffset, n.y + roomYOffset, table);

                        // stick shit on the table
                        for (int i = 0; i < 10; i++)
                        {
                            ReplacementMapSegment decor = l2TableDecorations[random.Next() % l2TableDecorations.Count];
                            int xPos = random.Next() % (table.w - decor.w + 1);
                            int yPos = random.Next() % (table.h - decor.h);
                            bool ok = true;
                            for (int y = 0; y < decor.h; y++)
                            {
                                for (int x = 0; x < decor.w; x++)
                                {
                                    int index = (yPos + y) * table.w + (x + xPos);
                                    if (index >= tableSpotsOccupied.Length || tableSpotsOccupied[index])
                                    {
                                        ok = false;
                                    }
                                }
                            }
                            if (ok)
                            {
                                for (int y = 0; y < decor.h; y++)
                                {
                                    for (int x = 0; x < decor.w; x++)
                                    {
                                        int index = (yPos + y) * table.w + (x + xPos);
                                        tableSpotsOccupied[index] = true;
                                    }
                                }

                                ReplacementMapSegment.applyReplacement(layer2, n.x + roomXOffset + table.x + xPos, n.y + roomYOffset + table.y + yPos, decor);
                            }
                        }
                    }
                    if ((random.Next() % 3) > 0 && !stepsUp && rightNodePresent)
                    {
                        // bookshelf thing, if we don't overlap a table
                        int tPosx = random.Next() % 6;
                        int tPosy = 4;
                        bool canUse = true;
                        if (canUse)
                        {
                            ReplacementMapSegment.applyReplacement(layer1, n.x + 16 + roomXOffset + tPosx, n.y + roomYOffset + tPosy, l1Bookshelf);
                        }
                    }


                }
            }

            foreach (ProcGenMapNode n in allNodes.Values)
            {
                int ypos = n.values["ypos"];
                if (n.name.StartsWith("i") && ypos <= 6)
                {
                    int xpos = n.values["xpos"];
                    // xpos+ypos even for bottom node, use right steps, odd use left
                    bool rightSteps = ((xpos + ypos) % 2) == 0;
                    string topNodeKey = "i_" + (ypos + 1) + "_" + xpos;
                    if (ypos <= 5 && allNodes.ContainsKey(topNodeKey))
                    {
                        ProcGenMapNode topNode = allNodes[topNodeKey];
                        if (n.connections.Contains(topNode))
                        {
                            ReplacementMapSegment.applyReplacement(layer1, n.x + roomXOffset, n.y + roomYOffset, rightSteps ? stairsUpRight : stairsUpLeft);
                            ReplacementMapSegment.applyReplacement(layer1, topNode.x + roomXOffset, topNode.y + roomYOffset, rightSteps ? stairsDownRight : stairsDownLeft);
                            ReplacementMapSegment.applyReplacement(layer2, n.x + roomXOffset, n.y + roomYOffset, rightSteps ? l2stairsUpRight : l2stairsUpLeft);
                            ReplacementMapSegment.applyReplacement(layer2, topNode.x + roomXOffset, topNode.y + roomYOffset, rightSteps ? l2stairsDownRight : l2stairsDownLeft);
                        }
                    }

                }
            }

            byte[] layer1BaseInnerRoom = new byte[]
                {
                  1,   2,   2,   2,   2,   2,   2,   2,   2,   2,   2,   3,
                 17, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139,  19,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  56,  56,  56,  56,  56,  56,  56,  56,  56,  56,  19,
                 17,  89,  89,  89,  89,  89,  89,   8,   9,  89,  89,  19,
                 17, 136,  13,  14, 136, 136, 136,  24,  25, 136, 136,  19,
                 17,  99,  29,  30,  99,  99,  99,  40,  41,  99,  99,  19,
                 17,  99,  74,  75,  99,  99,  99,  99,  99,  99,  99,  19,
                 17,  99,  99,  99,  99,  99,  99,  99,  99,  99,  99,  19,
                 17,  99,  99,  99,  99,  99,  99,  99,  99,  99,  99,  19,
                 17,  99,  99,  99,  99,  99,  99,  99,  99,  99,  99,  19,
                 17,  99,  99,  99,  99,  99,  99,  99,  99,  99,  99,  19,
                 17,  99,  99,  99,  99,  99,  99,  99,  99,  99,  99,  19,
                 17,  99,  99,  99,  99,  99,  99,  99,  99,  99,  99,  19,
                 17,  99,  99,  99,  99,  99,  99,  99,  99,  99,  99,  19,
                 33,  34,  34,  34,   5,  99,  99,   4,  34,  34,  34,  35,
                 18,  18,  18,  18,  33,  34,  34,  35,  18,  18,  18,  18,
                 18,  18,  18,  18,  18, 190, 190,  18,  18,  18,  18,  18, // 190 - return doors
                };
            // 16 rooms on top
            int roomNum = 0;
            for (int _y = 0; _y < 2; _y++)
            {
                for (int _x = 0; _x < 8; _x++)
                {
                    // position that the hallway door should stick you in
                    int xEntryPos = 0;
                    int yEntryPos = 0;
                    for (int y = 0; y < 18; y++)
                    {
                        for (int x = 0; x < 12; x++)
                        {
                            layer1[1 + _x * 16 + x, 1 + _y * 19 + y] = layer1BaseInnerRoom[y * 12 + x];
                            if (layer1BaseInnerRoom[y * 12 + x] == 190)
                            {
                                layer2[1 + _x * 16 + x, 1 + _y * 19 + y] = 164;
                                if (xEntryPos == 0)
                                {
                                    // found return door; entry position is above it
                                    xEntryPos = x;
                                    yEntryPos = y - 1;
                                }
                            }
                        }
                    }
                    // only if there's actually a door that should lead to it
                    if (roomNum < hallwayDoorNum)
                    {
                        // set the position to enter from the hallway segments
                        indoorMap.sameMapEntryLocations[roomNum++] = new XyPos(xEntryPos, yEntryPos - 1);
                    }
                }
            }

            // 4 down the right side
            for (int _y = 2; _y < 6; _y++)
            {
                for (int y = 0; y < 18; y++)
                {
                    for (int x = 0; x < 12; x++)
                    {
                        layer1[1 + 7 * 16 + x, 1 + _y * 19 + y] = layer1BaseInnerRoom[y * 12 + x];
                        if (layer1BaseInnerRoom[y * 12 + x] == 190)
                        {
                            layer2[1 + 7 * 16 + x, 1 + _y * 19 + y] = 164;
                        }
                    }
                }
            }
        }
    }
}
