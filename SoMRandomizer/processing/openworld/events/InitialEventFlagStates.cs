using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System.Collections.Generic;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Set initial event flag states for open world to allow the world to be open.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class InitialEventFlagStates : RandoProcessor
    {
        protected override string getName()
        {
            return "Event 0x107 - Initialization of event flags";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
            bool anySpellTriggers = context.workingData.getBool(OpenWorldClassSelection.ANY_MAGIC_EXISTS);
            string goalType = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            EventScript eventData = new EventScript();
            context.replacementEvents[0x107] = eventData;
            eventData.SetFlag(EventFlags.SUNKEN_CONTINENT_MODE_FLAG, 2); // raise the lost continent            
            eventData.SetFlag(EventFlags.WATERFALL_BOY_VISIBILITY_FLAG, 0); // boy found/visible in waterfall event
            eventData.SetFlag(EventFlags.POTOS_FLAG, 0xA); // x10: potos events are done and you're kicked out; mantis ant handled separately; sword get sets it to B
            eventData.SetFlag(EventFlags.WATER_PALACE_FLAG, 7); // x11: haven't talked to luka and gotten the spear yet
            eventData.SetFlag(EventFlags.PANDORA_RUINS_FLAG, 8); // x12: phanna is gone and ruins is open
            eventData.SetFlag(EventFlags.PANDORA_GIRL_FLAG, 7); // x13: all the girl stuff is done?
            eventData.SetFlag(EventFlags.GAIAS_NAVEL_WATTS_FLAG, 7); // x14: open the steps near watts, and earth palace entrance i think
            eventData.SetFlag(EventFlags.GAIAS_NAVEL_SPRITE_FLAG, 7); // x15: skip tropicallo event, rabiteman show, and all that bullshit
            eventData.SetFlag(EventFlags.WITCHFOREST_GIRL_FLAG, 1); // x16: don't meet the girl at the forest or do the werewolf scene
            eventData.SetFlag(EventFlags.WITCHCASTLE_FLAG, 2); // x17: open the thing you're supposed to have 3 characters stand on
            eventData.SetFlag(EventFlags.EARTHPALACE_FLAG, 1); // x18: normally set to 1 when you get the sprite; if we don't set this we have to double-cast on the orb
            eventData.SetFlag(EventFlags.PANDORA_PHANNA_FLAG, 2); // x1a: set up pandora ruins for entry & fight
            eventData.SetFlag(EventFlags.KILROY_EVENT_FLAG, 1); // x1b: enable the kilroy pit at gaia's navel
            eventData.SetFlag(EventFlags.UPPERLAND_UNLOCK_FLAG, 7); // x1e: cannon dude by potos will send you to upperland if you really want
            if (flammieDrumInLogic) // x20: skip the moogle crap and clear the sylphid orb, unless we need it for flammie logic
            {
                if (anySpellTriggers)
                {
                    eventData.SetFlag(EventFlags.UPPERLAND_MOOGLES_FLAG, 0x03); // upper land orb is there
                }
                else
                {
                    eventData.SetFlag(EventFlags.UPPERLAND_MOOGLES_FLAG, 0x04); // already unlocked
                }
            }
            else
            {
                eventData.SetFlag(EventFlags.UPPERLAND_MOOGLES_FLAG, 0x04); // already unlocked
            }
            eventData.SetFlag(EventFlags.UPPERLAND_PROGRESS_FLAG, 2); // x21: unlock the seasons doorway
            eventData.SetFlag(EventFlags.UPPERLAND_SPRINGBEAK_FLAG, 1); // x22: skip the i'm baaaack dialogue before spring beak
            eventData.SetFlag(EventFlags.MATANGO_PROGRESS_FLAG, 3); // x24: orb present, snake alive, skip matango scenes
            eventData.SetFlag(EventFlags.SALAMANDO_STOVE_FLAG, 3); // x27: salamando map is already cold
            eventData.SetFlag(EventFlags.DESERT_SHIP_FLAG, 0xE); // 0x28: whole ship event including mech rider 1 cleared.  we should use e->f to mark him as dead
            eventData.SetFlag(EventFlags.NORTHTOWN_CASTLE_FLAG, 4); // x34: open the castle
            eventData.SetFlag(EventFlags.NORTHTOWN_PHANNA_FLAG, 2); // x3a: no phanna at the NT ruins
            eventData.SetFlag(EventFlags.TASNICA_FLAG, 1); // x3c: skip jema in tasnica
            eventData.SetFlag(EventFlags.JEHK_CAVE_FLAG, 4); // x3f: open jehk cave
            eventData.SetFlag(EventFlags.LOST_CONTINENT_PROGRESS_FLAG_1, 2); // 0x41: raise purple island; do i want this 1 or 2?            
            eventData.SetFlag(EventFlags.LOST_CONTINENT_HEXAS_FLAG, 1); // 0x44: 01 for snap dragon; 05 for hexas to be accessible and visible on her map
            eventData.SetFlag(EventFlags.WITCH_FOREST_SLIDING_WALL_UNLOCK, 3); // 0x5f: open the sliding wall thing in witch's forest
            eventData.SetFlag(EventFlags.GAIAS_NAVEL_LAVA_DRAIN_SWITCH, 4); // x60: unlock the whole gaia's navel dungeon through the town
            eventData.SetFlag(EventFlags.OPEN_WORLD_TIMED_MODE_ENABLE, 1); // x73: enable timer for timed mode
            eventData.SetFlag(EventFlags.DEATH_TYPE_FLAG, 2); // real deaths, even at mantis ant
            // start with all weapon orbs option - set the flags for it here
            if (settings.getBool(OpenWorldSettings.PROPERTYNAME_START_WITH_ALL_WEAPON_ORBS))
            {
                for (int weaponOrbFlag = 0xB8; weaponOrbFlag <= 0xC7; weaponOrbFlag++)
                {
                    eventData.SetFlag((byte)weaponOrbFlag, 9);
                }
            }

            // 6F flag that we use for christmas mode progress
            if (goalType == OpenWorldGoalProcessor.GOAL_GIFTMODE || goalType == OpenWorldGoalProcessor.GOAL_REINDEER)
            {
                eventData.SetFlag(EventFlags.OPEN_WORLD_CHRISTMAS_PROGRESS, 0);
            }

            // if no characters that can cast spells are available, unlock all the element orbs
            if (!anySpellTriggers)
            {
                foreach (byte b in EventFlags.lostContinentElementOrbFlags)
                {
                    eventData.SetFlag(b, 1);
                }
                eventData.SetFlag(EventFlags.MATANGO_PROGRESS_FLAG, 4); // matango cave                
                eventData.SetFlag(EventFlags.EARTHPALACE_FLAG, 2); // earth palace
                eventData.SetFlag(EventFlags.FIRE_PALACE_SWITCHES_FLAG, 4); // fire palace
                eventData.SetFlag(EventFlags.FIRE_PALACE_COMPLETION_FLAG, 1); // fire palace
                eventData.SetFlag(EventFlags.LUNA_PALACE_FLAG, 2); // luna palace
            }
            else
            {
                // process individual orbs
                Dictionary<int, byte> crystalOrbColorMap = ElementSwaps.getCrystalOrbElementMap(context);
                // plando to set them to none
                if (crystalOrbColorMap.ContainsKey(ElementSwaps.ORBMAP_MATANGO) && crystalOrbColorMap[ElementSwaps.ORBMAP_MATANGO] == 0xFF)
                {
                    eventData.SetFlag(EventFlags.MATANGO_PROGRESS_FLAG, 4); // matango cave
                }
                if (crystalOrbColorMap.ContainsKey(ElementSwaps.ORBMAP_EARTHPALACE) && crystalOrbColorMap[ElementSwaps.ORBMAP_EARTHPALACE] == 0xFF)
                {
                    eventData.SetFlag(EventFlags.EARTHPALACE_FLAG, 2); // earth palace
                }

                // fire palace .. 2C=1 to unlock first orb, 2B=1 to unlock second, 2B=4 to unlock third
                // so if we set the third one to nothing, the second one also has to be nothing
                if (crystalOrbColorMap.ContainsKey(ElementSwaps.ORBMAP_FIREPALACE1) && crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE1] == 0xFF)
                {
                    eventData.SetFlag(EventFlags.FIRE_PALACE_COMPLETION_FLAG, 1); // fire palace 1
                }
                if (crystalOrbColorMap.ContainsKey(ElementSwaps.ORBMAP_FIREPALACE2) && crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE2] == 0xFF)
                {
                    eventData.SetFlag(EventFlags.FIRE_PALACE_SWITCHES_FLAG, 1); // fire palace 2
                }
                if (crystalOrbColorMap.ContainsKey(ElementSwaps.ORBMAP_FIREPALACE3) && crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE3] == 0xFF)
                {
                    eventData.SetFlag(EventFlags.FIRE_PALACE_SWITCHES_FLAG, 4); // fire palace 3
                }
                if (crystalOrbColorMap.ContainsKey(ElementSwaps.ORBMAP_LUNAPALACE) && crystalOrbColorMap[ElementSwaps.ORBMAP_LUNAPALACE] == 0xFF)
                {
                    eventData.SetFlag(EventFlags.LUNA_PALACE_FLAG, 2); // luna palace
                }
                if (crystalOrbColorMap.ContainsKey(ElementSwaps.ORBMAP_UPPERLAND) && crystalOrbColorMap[ElementSwaps.ORBMAP_UPPERLAND] == 0xFF)
                {
                    eventData.SetFlag(EventFlags.UPPERLAND_MOOGLES_FLAG, 4); // upper land outside for flammie drum in logic mode
                }
                for (int map = ElementSwaps.ORBMAP_GRANDPALACE_FIRST; map <= ElementSwaps.ORBMAP_GRANDPALACE_LAST; map++)
                {
                    if (crystalOrbColorMap.ContainsKey(map) && crystalOrbColorMap[map] == 0xFF)
                    {
                        eventData.SetFlag(EventFlags.lostContinentElementOrbFlags[map - ElementSwaps.ORBMAP_GRANDPALACE_FIRST], 1);
                    }
                }
            }
            if (context.workingData.getBool(OpenWorldGoalProcessor.MANA_FORT_ACCESSIBLE_INDICATOR))
            {
                eventData.SetFlag(EventFlags.OPENWORLD_MANAFORT_ACCESSIBLE_FLAG, 1);
            }
            eventData.Return();
            return true;
        }
    }
}
