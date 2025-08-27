using System;

namespace SoMRandomizer.processing.ancientcave.mapgen.manafortint.rooms
{
    /// <summary>
    /// Abstract manafort tileset room.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public abstract class ManafortRoomMaker
    {
        // 20x20
        public abstract byte[,] generateRoom(bool rightPath, bool leftPath, bool topPath, bool bottomPath, bool door, Random r);

        protected void injectData(byte[] roomData, byte[] newData, int x, int y, int width)
        {
            for(int yPos = 0; yPos < newData.Length / width; yPos++)
            {
                for(int xPos = 0; xPos < width; xPos++)
                {
                    roomData[(yPos + y) * 20 + (xPos + x)] = newData[yPos * width + xPos];
                }
            }
        }
    }
}
