﻿using System;

namespace SoMRandomizer.processing.ancientcave.mapgen.manafortint.rooms
{
    /// <summary>
    /// Currently-unused whip-post room for manafort tileset.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class WhipRoom : ManafortRoomMaker
    {
        static byte[] baseRoom = new byte[] {
                111,  95,  95,  95,  95, 135, 174,  81,  81, 174, 174,  81,  81, 174, 136,  95,  95,  95,  95, 107,
                 76,  91,  91, 135, 174,  99, 147,  96,  96, 147, 147,  97,  96, 147,  98, 174, 136,  91,  91,  77,
                107, 135, 174,  99, 147,  96, 114, 112, 112, 114, 116, 113, 112, 116,  96, 147,  98, 174, 136, 108,
                 77, 159, 147,  96, 114, 112, 166, 128, 128, 130, 132, 129, 128, 134, 112, 116,  96, 147, 157, 106,
                108, 159, 114, 112, 166, 128, 165, 144, 144, 146, 146, 145, 144, 165, 128, 150, 112, 116, 157,  95,
                106, 159, 150, 128, 165, 144,  10, 160, 161,  43,  43, 162, 160,  12, 144, 165, 128, 134, 157,  91,
                111, 159, 165, 144,  10, 160,  44,  11,  11, 126, 191,  43,  11,  42, 160,  12, 144, 165, 157, 111,
                 76, 159,  10, 160,  35,  35,  35,  35,  35,  35,  35,  35,  35,  35,  35,  35, 160,  12, 157,  76,
                107, 159,  26,  28,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  28,  28, 157, 107,
                 77, 159,  26,  28,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  28,  28, 157,  77,
                108, 159,  26,  28,  15,  15,  15,  10,  11, 191, 126,  11,  12,  15,  15,  15,  28,  28, 157, 108,
                106, 159,  26, 191,  15,  15,  15, 126,   7,   8,   1,   2, 191,  15,  15,  15, 126,  28, 157, 106,
                 95, 159,  26, 126,  15,  15,  15, 191,  23,  24,  17,  18, 126,  15,  15,  15, 191,  28, 157,  95,
                 91, 159,  26,  28,  15,  15,  15,  42,  43, 126, 191,  43,  44,  15,  15,  15,  28,  28, 157,  91,
                 92, 159,  26,  28,  15,  15,  15,  35,  35,  35,  35,  35,  35,  15,  15,  15,  28,  28, 157,  92,
                 78, 159,  26,  28,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  28,  28, 157,  78,
                111, 159,  42,  44,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  15,  42,  44, 157, 111,
                 77, 151, 140,  35,  10,  11,  11,  11,  11, 191, 126,  11,  11,  11,  11,  12,  35, 138, 152,  77,
                106, 106, 151, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 139, 152, 106, 106,
                 95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,  95,
            };

        // 0, 3
        static byte[] leftPassage = new byte[]
        {
                174,  99,
                147,  96,
                114, 112,
                130, 129,
                146, 145,
                 43, 161,
                 35,  35,
                 10,  44,
                 10,  12,
                 10,  44,
                 10,  12,
                 10,  44,
                 10,  12,
                 10,  44,
                142, 142,
        };

        // 18, 3
        static byte[] rightPassage = new byte[]
        {
                 98, 174,
                 96, 147,
                112, 144,
                128, 130,
                144, 146,
                160,  43,
                 35,  35,
                 10,  12,
                 10,  44,
                 10,  12,
                 10,  44,
                 10,  12,
                 10,  44,
                 10,  12,
                142, 142,
        };

        // 8, 17
        static byte[] bottomPassage = new byte[]
        {
                44, 191, 126, 42,
                140, 21, 22, 138,
                151, 139, 139, 152,
        };

        // 9, 0
        static byte[] topPassage = new byte[]
        {
                 171, 171,
                 147, 147,
                 46, 47,
                 53, 34,
                 69, 34,
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
            byte[] room = new byte[400];
            for (int i = 0; i < 400; i++)
            {
                room[i] = baseRoom[i];
            }

            if (rightPath)
            {
                injectData(room, rightPassage, 18, 3, 2);
            }
            if (leftPath)
            {
                injectData(room, leftPassage, 0, 3, 2);
            }
            if (bottomPath)
            {
                injectData(room, bottomPassage, 8, 17, 4);
            }
            if (topPath)
            {
                injectData(room, topPassage, 9, 0, 2);
            }
            else if (door)
            {
                injectData(room, topPassage, 9, 0, 2);
                injectData(room, doorBlock, 9, 2, 2);
            }

            // 3, 11; 12, 11
            byte[] horizontalBypass = new byte[]
            {
                42, 11, 11, 11, 44,
                10, 11, 11, 11, 12,
            };
            // +1 x and +2 y
            byte[] horizontalBypassBottom = new byte[]
            {
                35, 35, 35,
            };

            // 9, 6; 9, 13
            byte[] verticalBypass = new byte[]
            {
                12, 10,
                26, 26,
                26, 26,
                26, 26,
                44, 42,
            };

            // sometimes no whip thingies
            if((r.Next() % 3) == 0)
            {
                injectData(room, horizontalBypass, 3, 11, 5);
                injectData(room, horizontalBypassBottom, 4, 13, 3);
            }
            if ((r.Next() % 3) == 0)
            {
                injectData(room, horizontalBypass, 12, 11, 5);
                injectData(room, horizontalBypassBottom, 13, 13, 3);
            }
            if ((r.Next() % 3) == 0)
            {
                injectData(room, verticalBypass, 9, 6, 2);
            }
            if ((r.Next() % 3) == 0)
            {
                injectData(room, verticalBypass, 9, 13, 2);
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
