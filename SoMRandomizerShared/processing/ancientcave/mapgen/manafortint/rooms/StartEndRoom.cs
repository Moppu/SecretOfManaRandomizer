using SoMRandomizer.util;
using System;

namespace SoMRandomizer.processing.ancientcave.mapgen.manafortint.rooms
{
    /// <summary>
    /// Start or end node rooms for manafort tileset floors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class StartEndRoom : ManafortRoomMaker
    {
        private bool endRoom;
        public StartEndRoom(bool endRoom)
        {
            this.endRoom = endRoom;
        }

        static byte[] baseRoom = new byte[] {
                 95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,
                 91, 135,  80,  80,  80,  80,  80,  80,  80,  80,  80,  80,  80,  80,  80,  80,  80,  80, 136,  91,
                 76, 159,  96,  96,  96,  96,  96,  96,  96,  96,  96,  96,  96,  96,  96,  96,  96,  96, 157,  76,
                 76, 159, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 157,  76,
                 76, 159, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 157,  76,
                 76, 159, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 144, 157,  76,
                 76, 159, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 157,  76,
                 76, 159,  35,  35,  35,  35,  35,  35,  35,  35,  35,  35,  35,  35,  35,  35,  35,  35, 157,  76,
                 76, 159,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48, 157,  76,
                 76, 159,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48, 157,  76,
                 76, 159,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48, 157,  76,
                 76, 159,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48, 157,  76,
                 76, 159,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48, 157,  76,
                 76, 159,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48, 157,  76,
                 76, 159,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48, 157,  76,
                 76, 159,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48, 157,  76,
                 76, 159,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48, 157,  76,
                 76, 159,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48, 157,  76,
                 76, 151, 142, 142, 142, 142, 142, 142, 142, 142, 142, 142, 142, 142, 142, 142, 142, 142, 152,  76,
                106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106,
            };

        // 0, 0
        static byte[] leftPassage = new byte[]
        {
                 95,  95,  95,
                 91, 135, 174,
                135,  99, 147,
                 99,  96, 116,
                 96, 112, 150,
                112, 128, 165,
                128, 144,  10,
                144, 161,  44,
                160,  44,  35,
                 35,  35,  48,
                 48,  48,  48,
                 48,  48,  48,
                 48,  48,  48,
                 48,  48,  48,
                 48,  48,  48,
                 48,  48,  48,
                 48,  48,  48,
                142, 143,  48,
                108, 151, 142,
                106, 106, 106,
        };

        // 17, 0
        static byte[] rightPassage = new byte[]
        {
                 95,  95,  95,
                174, 136,  91,
                147,  98, 136,
                116,  96,  98,
                150, 112,  96,
                165, 128, 112,
                 12, 144, 128,
                 42, 162, 144,
                 35,  42, 160,
                 48,  35,  35,
                 48,  48,  48,
                 48,  48,  48,
                 48,  48,  48,
                 48,  48,  48,
                 48,  48,  48,
                 48,  48,  48,
                 48,  48,  48,
                 48, 141, 142,
                142, 152, 108,
                106, 106, 106,
        };

        // 3, 17
        static byte[] bottomPassage = new byte[]
        {
                 48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,  48,
                142, 142, 142, 142, 142, 143,  48,  48, 141, 142, 142, 142, 142, 142,
                106, 106, 106, 106, 106, 151, 139, 139, 152, 106, 106, 106, 106, 106,
        };

        // 3, 0
        static byte[] topPassage = new byte[]
        {
                 95,  95,  95,  95, 135,  80, 171, 171,  80, 136,  95,  95,  95,  95,
                 80,  80,  80,  80,  99,  96, 147, 147,  96,  98,  80,  80,  80,  80,
                 96,  96,  96,  96,  96, 112,  46,  47, 112,  96,  96,  96,  96,  96,
                112, 112, 112, 112, 112, 128,  53,  34, 128, 112, 112, 112, 112, 112,
                128, 128, 128, 128, 128, 144,  69,  34, 144, 128, 128, 128, 128, 128,
                144, 144, 144, 144, 144, 161,   5,   6, 162, 144, 144, 144, 144, 144,
                160, 160, 160, 160, 160,  44,  21,  22,  42, 160, 160, 160, 160, 160,
                 35,  35,  35,  35,  35,  35, 105, 105,  35,  35,  35,  35,  35,  35,
        };

        // 9, 2 on top of topPassage
        static byte[] doorBlock = new byte[]
        {
                163, 164,
                176, 180,
                177, 181,
        };

        public override byte[,] generateRoom(bool rightPath, bool leftPath, bool topPath, bool bottomPath, bool door, Random r)
        {
            // it's just CrystalRoom without the crystals.
            byte[] room = new byte[400];
            for (int i = 0; i < 400; i++)
            {
                room[i] = baseRoom[i];
            }

            if (rightPath)
            {
                injectData(room, rightPassage, 17, 0, 3);
            }
            if (leftPath)
            {
                injectData(room, leftPassage, 0, 0, 3);
            }
            if (bottomPath)
            {
                injectData(room, bottomPassage, 3, 17, 14);
            }
            if (topPath)
            {
                injectData(room, topPassage, 3, 0, 14);
            }
            else if (door)
            {
                injectData(room, topPassage, 3, 0, 14);
                injectData(room, doorBlock, 9, 2, 2);
            }

            int perlinZ = (r.Next() % 2000);

            for (int yPos = 1; yPos < 19; yPos++)
            {
                for (int xPos = 1; xPos < 19; xPos++)
                {
                    if ((xPos < 9 || xPos > 11) && (yPos < 9 || yPos > 11))
                    {
                        if (room[yPos * 20 + xPos] == 48)
                        {
                            if (PerlinNoise.noise(xPos * 40 / (double)128, yPos * 40 / (double)128, perlinZ) > 0.3)
                            {
                                room[yPos * 20 + xPos] = 122;
                            }
                            else if (PerlinNoise.noise(xPos * 20 / (double)128, yPos * 20 / (double)128, perlinZ) > 0 &&
                                room[yPos * 20 + xPos + 1] == 48 &&
                                room[(yPos+1) * 20 + xPos + 1] == 48 &&
                                room[(yPos+1) * 20 + xPos] == 48)
                            {
                                room[yPos * 20 + xPos] = 1;
                                room[yPos * 20 + xPos + 1] = 2;
                                room[(yPos+1) * 20 + xPos] = 17;
                                room[(yPos+1) * 20 + xPos + 1] = 18;
                            }
                        }
                    }
                }
            }

            if(endRoom)
            {
                int ex = 10;
                int ey = 12;
                // warper
                room[ey * 20 + ex] = 54;
                // crystals
                room[ey * 20 + (ex+1)] = 122;
                room[ey * 20 + (ex-1)] = 122;
                room[(ey-1) * 20 + ex] = 122;
                room[(ey+1) * 20 + ex] = 122;
                room[ey * 20 + (ex + 2)] = 124;
                room[ey * 20 + (ex - 2)] = 124;
                room[(ey - 2) * 20 + ex] = 124;
                room[(ey + 2) * 20 + ex] = 124;

                // down arrow blocks
                room[(ey - 2) * 20 + (ex - 2)] = 5;
                room[(ey - 2) * 20 + (ex - 1)] = 6;
                room[(ey - 1) * 20 + (ex - 2)] = 21;
                room[(ey - 1) * 20 + (ex - 1)] = 22;

                room[(ey - 2) * 20 + (ex + 1)] = 5;
                room[(ey - 2) * 20 + (ex + 2)] = 6;
                room[(ey - 1) * 20 + (ex + 1)] = 21;
                room[(ey - 1) * 20 + (ex + 2)] = 22;

                // up arrow blocks
                room[(ey + 1) * 20 + (ex - 2)] = 3;
                room[(ey + 1) * 20 + (ex - 1)] = 4;
                room[(ey + 2) * 20 + (ex - 2)] = 19;
                room[(ey + 2) * 20 + (ex - 1)] = 20;

                room[(ey + 1) * 20 + (ex + 1)] = 3;
                room[(ey + 1) * 20 + (ex + 2)] = 4;
                room[(ey + 2) * 20 + (ex + 1)] = 19;
                room[(ey + 2) * 20 + (ex + 2)] = 20;
            }

            byte[,] layer1 = new byte[20, 20];

            for (int yPos = 0; yPos < 20; yPos++)
            {
                for (int xPos = 0; xPos < 20; xPos++)
                {
                    layer1[xPos, yPos] = room[yPos * 20 + xPos];
                }
            }

            return layer1;
        }
    }
}
