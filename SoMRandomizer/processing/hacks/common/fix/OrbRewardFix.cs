using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.vanillarando;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.fix
{
    /// <summary>
    /// Hack that fixes an event sequence issue with getting weapon orb rewards over the vanilla max.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OrbRewardFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Weapon orb event fix";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_ORB_REWARD_FIX))
            {
                return false;
            }

            // Glove
            context.replacementEvents[0x500] = getEvent(0);
            // Sword
            context.replacementEvents[0x501] = getEvent(1);
            // Axe
            context.replacementEvents[0x502] = getEvent(2);
            // Spear
            context.replacementEvents[0x503] = getEvent(3);
            // Whip
            context.replacementEvents[0x504] = getEvent(4);
            // Bow
            context.replacementEvents[0x505] = getEvent(5);
            // Boomerang
            context.replacementEvents[0x506] = getEvent(6);
            // Javelin
            context.replacementEvents[0x507] = getEvent(7);

            return true;
        }

        private static EventScript getEvent(byte weaponType)
        {
            // vanilla event minus the jump to event 0x508, which causes problems if you're over max
            byte eventFlag1 = (byte)(0xB8 + weaponType);
            byte eventFlag2 = (byte)(0xC0 + weaponType);
            EventScript eventData = new EventScript();
            eventData.Logic(eventFlag1, 0x0, 0x8, EventScript.GetIncrCmd(eventFlag1)); // orb level A
            eventData.Logic(eventFlag2, 0x0, 0x8, EventScript.GetIncrCmd(eventFlag2)); // orb level A

            eventData.OpenDialogueBox();

            eventData.Add(0x7F); // enter
            eventData.Add(0x59);
            eventData.Add(0x06); // text shift
            eventData.AddDialogue("Got " + SomVanillaValues.weaponByteToName(weaponType), null);
            eventData.Jump(0x509); // 's orb
            eventData.End();
            return eventData;
        }
    }
}
