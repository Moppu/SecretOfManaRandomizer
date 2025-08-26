using SoMRandomizer.util;

namespace SoMRandomizer.processing.ancientcave.mapgen.manafortint
{
    /// <summary>
    /// Utility to generate interior areas for manafort interior tileset floors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ManafortIntNpcRooms
    {
        // 36 x 26 -> 24 x 26; buffer it to 32 x 32 with 0x0Fs
        const byte door = 0x3C;

        public static void populateIndoorMapData(AncientCaveMap indoorMap, int numDoors, byte[,] layer1)
        {
            int mapWidth = layer1.GetLength(0);
            int mapHeight = layer1.GetLength(1);
            byte[] layer1Data = DataUtil.readResource("SoMRandomizer.Resources.customMaps.acManafortNpcRoomL1.bin");
            int doorNum = 0;
            for (int y = 0; y < mapHeight; y += 32)
            {
                for (int x = 0; x < mapWidth; x += 32)
                {
                    if (doorNum < numDoors)
                    {
                        // 4 buffer on each side x
                        // 3 buffer on each side y
                        // using 0x0F
                        for (int _y = 0; _y < 32; _y++)
                        {
                            for (int _x = 0; _x < 32; _x++)
                            {
                                layer1[x + _x, y + _y] = 0x0F;
                            }
                        }
                        for (int _y = 0; _y < 26; _y++)
                        {
                            for (int _x = 0; _x < 24; _x++)
                            {
                                layer1[x + _x + 4, y + _y + 3] = layer1Data[_y * 24 + _x];
                                // from AcManafortTilesetChanges, 0x3C is a return door now, so create those here
                                if (layer1Data[_y * 24 + _x] == door)
                                {
                                    // one above
                                    indoorMap.altMapEntryLocations[doorNum] = new XyPos(x + _x + 4, y + _y + 2);
                                    indoorMap.returnDoors.Add(doorNum);
                                }
                            }
                        }
                        doorNum++;
                    }
                }
            }
        }
    }
}
