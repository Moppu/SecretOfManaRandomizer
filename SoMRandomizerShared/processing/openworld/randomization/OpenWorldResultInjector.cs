using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.openworld.events;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// Injects location to prize pairings into open world events.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldResultInjector
    {
        public static void injectRandomization(byte[] outRom, Dictionary<PrizeLocation, PrizeItem> prizePlacements, RandoSettings settings, RandoContext context)
        {
            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            // starter weapons
            int boyStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.BOY_START_WEAPON_INDEX);
            int girlStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.GIRL_START_WEAPON_INDEX);
            int spriteStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.SPRITE_START_WEAPON_INDEX);
            outRom[0x57c0] = (byte)(boyStarterWeapon * 9);
            outRom[0x57c4] = (byte)(girlStarterWeapon * 9);
            outRom[0x57c8] = (byte)(spriteStarterWeapon * 9);

            // prizes in events
            foreach (PrizeLocation prizeLocation in prizePlacements.Keys)
            {
                int eventNum = prizeLocation.eventNum;
                int index = prizeLocation.eventReplacementIndex;
                PrizeItem thisPrize = prizePlacements[prizeLocation];

                List<byte> eventData = context.replacementEvents[eventNum];
                List<byte> injectionPattern = new List<byte>();
                foreach (byte b in PrizeEvents.OPENWORLD_EVENT_INJECTION_PATTERN)
                {
                    injectionPattern.Add(b);
                }
                injectionPattern.Add((byte)index);
                VanillaEventUtil.replaceEventData(injectionPattern, eventData, thisPrize.eventData.ToList());

                // set the visibility flag for chests so they disappear once giving their prize.
                // don't change the two whip chests since spikey death makes them appear; event flags D8, D9
                if(prizeLocation.mapNum != -1)
                {
                    // modify the visibility flags of the chest so it disappears after we get the thing
                    int mapNum = prizeLocation.mapNum;
                    int objNum = prizeLocation.objNum;
                    int evNum = prizeLocation.eventNum;
                    byte flag = thisPrize.gotItemEventFlag;
                    int mapObjOffset = 0x80000 + outRom[0x87000 + mapNum * 2] + (outRom[0x87000 + mapNum * 2 + 1] << 8);
                    mapObjOffset += 8; // skip header
                    outRom[mapObjOffset + 8 * objNum + 0] = flag;
                    outRom[mapObjOffset + 8 * objNum + 1] = 0x00;
                    outRom[mapObjOffset + 8 * objNum + 6] = (byte)evNum;
                    outRom[mapObjOffset + 8 * objNum + 7] &= 0xC0;
                    outRom[mapObjOffset + 8 * objNum + 7] |= (byte)(evNum >> 8);
                    Logging.log("Setting chest object " + objNum + " on map " + mapNum + " [" + prizeLocation.locationName + "] to use event flag " + flag.ToString("X"), "debug");
                }

                // fixes issues with close->open following each other?
                List<byte> filteredEvent = new List<byte>();
                for (int i = 0; i < eventData.Count; i++)
                {
                    if (i < eventData.Count - 1 && eventData[i] == EventCommandEnum.CLOSE_DIALOGUE.Value && eventData[i + 1] == EventCommandEnum.OPEN_DIALOGUE.Value)
                    {
                        filteredEvent.Add(EventCommandEnum.CLEAR_DIALOGUE.Value);
                        i++;
                    }
                    else
                    {
                        filteredEvent.Add(eventData[i]);
                    }
                }
                context.replacementEvents[eventNum] = filteredEvent;
            }

            if (goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                GiftModeProcessing.makeGiftModeEvents(settings, context);
            }
        }
    }
}
