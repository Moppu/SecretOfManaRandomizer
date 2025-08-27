using System;
using System.Collections.Generic;
using System.Linq;
using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that handles changes to weapon orb reward events for various randomized modes.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OrbRewardEventGenerator : RandoProcessor
    {
        protected override string getName()
        {
            return "Changes to boss/vanilla chest weapon orb reward events";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool orbRewardFix = settings.getBool(CommonSettings.PROPERTYNAME_ORB_REWARD_FIX);
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            Dictionary<int, int> nonSwappedOrbs = new Dictionary<int, int>();
            for (int i = 0; i < 8; i++)
            {
                nonSwappedOrbs[i] = i;
            }
            if (randoMode == VanillaRandoSettings.MODE_KEY)
            {
                Dictionary<int, int> orbSwaps = nonSwappedOrbs;
                if (settings.getBool(VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_WEAPON_ORBS))
                {
                    orbSwaps = processSwaps(context.randomFunctional);
                    Logging.log("Weapon orb swaps:", "spoiler");
                    for(int i=0; i < 8; i++)
                    {
                        Logging.log("  " + SomVanillaValues.weaponByteToName(i) + " => " + SomVanillaValues.weaponByteToName(orbSwaps[i]), "spoiler");
                    }
                    // for vanilla rando, also swap which event flags control visibility of which chests with orbs in them
                    processChestSwaps(origRom, outRom, orbSwaps);
                }
                processEventChanges(context, orbRewardFix, orbSwaps);
            }
            else if (randoMode == OpenWorldSettings.MODE_KEY)
            {
                // always swap for open world.
                Dictionary<int, int> orbSwaps = processSwaps(context.randomFunctional);
                orbSwaps = processSwaps(context.randomFunctional);
                Logging.log("Weapon orb swaps (for boss rewards only):", "spoiler");
                for (int i = 0; i < 8; i++)
                {
                    Logging.log("  " + SomVanillaValues.weaponByteToName(i) + " => " + SomVanillaValues.weaponByteToName(orbSwaps[i]), "spoiler");
                }
                // nope that for open world, this impacts only boss orb rewards.
                // orb prize events, which are generated in OpenWorldPrizes, re-write these events and don't use 0x500 - 0x507 directly.
                processEventChanges(context, orbRewardFix, orbSwaps);
                // for open world, the logic overrides all the event flags for chests, so no need to swap them here
            }
            else
            {
                if (orbRewardFix)
                {
                    // process without swaps, but with the reward fix
                    processEventChanges(context, true, nonSwappedOrbs);
                }
                else
                {
                    Logging.log("No changes to be done for weapon orb reward events.");
                    return false;
                }
            }

            return true;
        }

        private Dictionary<int, int> processSwaps(Random r)
        {
            List<int> orbNums = new int[] { 0, 2, 3, 4, 5, 6, 7 }.ToList();

            Dictionary<int, int> randoOrbs = new Dictionary<int, int>();

            // skip 1 - sword
            randoOrbs[1] = 1;

            int oldId = 0;
            while (orbNums.Count > 0)
            {
                int newId = r.Next() % orbNums.Count;
                randoOrbs[oldId] = orbNums[newId];
                oldId++;
                if (oldId == 1)
                {
                    oldId++;
                }
                orbNums.RemoveAt(newId);
            }

            return randoOrbs;
        }

        private void processChestSwaps(byte[] origRom, byte[] outRom, Dictionary<int, int> orbSwaps)
        {
            // swap visibility flags for chests
            for (int mapNum = 16; mapNum < 500; mapNum++)
            {
                List<MapObject> mapObjs = VanillaMapUtil.getObjects(origRom, mapNum);
                for (int objNum = 0; objNum < mapObjs.Count; objNum++)
                {
                    MapObject obj = mapObjs[objNum];
                    int objVisFlag = obj.getEventVisFlag();
                    if (objVisFlag >= 0xB8 && objVisFlag <= 0xBF)
                    {
                        obj.setEventVisFlag((byte)(orbSwaps[objVisFlag - 0xB8] + 0xB8));
                        VanillaMapUtil.putObject(outRom, obj, mapNum, objNum);
                    }
                }
            }
        }

        private void processEventChanges(RandoContext context, bool orbRewardFix, Dictionary<int, int> orbSwaps)
        {
            // replace events 0x500 through 0x507 with swapped versions of themselves
            for (int i = 0; i < 8; i++)
            {
                byte flag1 = (byte)(0xb8 + orbSwaps[i]);
                byte flag2 = (byte)(0xc0 + orbSwaps[i]);
                EventScript newEvent = new EventScript();
                context.replacementEvents[0x500 + i] = newEvent;
                if (orbRewardFix)
                {
                    // just ignore getting more than 8
                    newEvent.Logic(flag1, 0x0, 0x8, EventScript.GetIncrCmd(flag1));
                }
                else
                {
                    // vanilla behavior - causes problems if you're over 8 orbs
                    newEvent.Logic(flag1, 0x9, 0xF, EventScript.GetJumpCmd(0x508));
                }
                newEvent.Logic(flag2, 0x0, 0x8, EventScript.GetIncrCmd(flag2));
                newEvent.OpenDialogueBox();
                newEvent.Add(0x7F); // enter
                newEvent.Add(0x59);
                newEvent.Add(weaponByteIndent(orbSwaps[i])); // text shift
                newEvent.AddDialogue("Got " + SomVanillaValues.weaponByteToName(orbSwaps[i]), null);
                newEvent.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x05));
                newEvent.Add(0x09); // 's orb
                newEvent.End();
            }
        }

        private static byte weaponByteIndent(int weapon)
        {
            // centering for weapon orb message
            return (byte)(8 - SomVanillaValues.weaponByteToName(weapon).Length / 2);
        }
    }
}
