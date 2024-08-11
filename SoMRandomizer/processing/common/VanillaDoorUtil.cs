using SoMRandomizer.processing.common.structure;
using System;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Utilities for reading/writing vanilla doors.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaDoorUtil
    {
        public static Door getDoor(byte[] rom, int doorId)
        {
            byte[] doorData = new byte[4];
            Array.Copy(rom, VanillaRomOffsets.DOORS + doorId * 4, doorData, 0, 4);
            return new Door(doorData);
        }

        public static void putDoor(byte[] rom, Door door, int doorId)
        {
            door.put(rom, VanillaRomOffsets.DOORS + doorId * 4);
        }
    }
}
