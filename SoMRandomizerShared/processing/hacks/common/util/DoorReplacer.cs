using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.util
{
    /// <summary>
    /// Inject a list of generated doors into the randomized ROM.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DoorReplacer
    {
        public const string PROPERTY_DOOR_LOCATION = "DoorLocation";

        public void process(byte[] rom, RandoContext context)
        {
            int doorsOffset = 0x83000; // vanilla location
            Dictionary<int, Door> replacementDoors = context.replacementDoors;
            if (context.workingData.hasSetting(PROPERTY_DOOR_LOCATION))
            {
                doorsOffset = context.workingData.getInt(PROPERTY_DOOR_LOCATION);
            }
            foreach(int doorId in replacementDoors.Keys)
            {
                Logging.log("Door replacement @" + (doorsOffset + doorId * 4).ToString("X6") + ": " + doorId.ToString("X4") + ": " + replacementDoors[doorId], "debug");
                replacementDoors[doorId].put(rom, doorsOffset + doorId * 4);
            }
        }
    }
}
