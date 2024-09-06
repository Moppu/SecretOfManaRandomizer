using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Open world replacement for the new game event, that removes most of the vanilla stuff.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NewGameEvent : RandoProcessor
    {
        protected override string getName()
        {
            return "Event 0x400 - new game";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // ////////////////////////////////////////////////////
            // x400 - intro with the manafort backdrop and shit
            // ////////////////////////////////////////////////////
            EventScript newEvent400 = new EventScript();
            context.replacementEvents[0x400] = newEvent400;
            newEvent400.IncrFlag(EventFlags.DIALOGUE_NO_BORDER_FLAG); // actually don't think this is needed anymore?
            string goalType = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
            // ton of shit cut out in here
            if (goalType == OpenWorldGoalProcessor.GOAL_GIFTMODE || goalType == OpenWorldGoalProcessor.GOAL_REINDEER || flammieDrumInLogic)
            {
                // enable menus as if we got the sword in vanilla
                newEvent400.IncrFlag(EventFlags.UI_DISPLAY_FLAG);
                newEvent400.IncrFlag(EventFlags.UI_DISPLAY_FLAG);
                if (flammieDrumInLogic)
                {
                    // clear out the npcs
                    newEvent400.SetFlag(EventFlags.POTOS_FLAG, 0xA);
                }
                // run door 0x82 as the start door.  note that OpenWorldSupportingMapChanges changes this to one of a couple different
                // locations based on settings.
                newEvent400.Door(0x82);
            }
            else if (!flammieDrumInLogic)
            {
                // vanilla start door - start on the log
                newEvent400.Door(0x00);
            }

            newEvent400.DecrFlag(EventFlags.DIALOGUE_NO_BORDER_FLAG); // actually don't think this is needed anymore?
            newEvent400.Jump(0x106); // -> event x106, which we also replace
            newEvent400.End();
            return true;
        }
    }
}
