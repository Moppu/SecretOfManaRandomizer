using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using System.Collections.Generic;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Open world replacement for event 0x103 for getting the starting weapon.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SwordGrabEvent : RandoProcessor
    {
        protected override string getName()
        {
            return "Event 0x103 - sword grab & other initialization";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
            bool startWithBoy = context.workingData.getBool(OpenWorldCharacterSelection.START_WITH_BOY);
            bool startWithGirl = context.workingData.getBool(OpenWorldCharacterSelection.START_WITH_GIRL);
            bool startWithSprite = context.workingData.getBool(OpenWorldCharacterSelection.START_WITH_SPRITE);
            string startingChar = context.workingData.get(OpenWorldCharacterSelection.STARTING_CHARACTER);

            List<byte> newEvent103 = new List<byte>();
            context.replacementEvents[0x103] = newEvent103;
            if (goal != OpenWorldGoalProcessor.GOAL_GIFTMODE && goal != OpenWorldGoalProcessor.GOAL_REINDEER && !flammieDrumInLogic)
            {
                // pulling sword
                newEvent103.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                newEvent103.Add(0x00);
                newEvent103.Add(0x90);
                newEvent103.Add(0x01);
                newEvent103.Add(EventCommandEnum.PLAYER_ANIM.Value);
                newEvent103.Add(0x00);
                newEvent103.Add(0xAD);
                newEvent103.Add(EventCommandEnum.SLEEP_FOR.Value);
                newEvent103.Add(0x08);
            }

            newEvent103.Add(EventCommandEnum.INCREMENT_FLAG.Value);
            newEvent103.Add(0x10);
            newEvent103.Add(0x09); // idk
            if (goal != OpenWorldGoalProcessor.GOAL_GIFTMODE && goal != OpenWorldGoalProcessor.GOAL_REINDEER && !flammieDrumInLogic)
            {
                newEvent103.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                newEvent103.Add(0x00);
            }
            newEvent103.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            newEvent103.Add(0x00);
            newEvent103.Add(0x90);
            newEvent103.Add(0x00);

            // finalize this flag and make sword npc invisible
            newEvent103.Add(EventCommandEnum.SET_FLAG.Value);
            newEvent103.Add(EventFlags.POTOS_FLAG);
            newEvent103.Add(0x0B);

            if (goal == OpenWorldGoalProcessor.GOAL_GIFTMODE || goal == OpenWorldGoalProcessor.GOAL_REINDEER || flammieDrumInLogic)
            {
                // button response is funky in these modes, so we play the event with a delay instead of b button to confirm
                PrizeEvents.injectReplacementPatternAutoDialogue(newEvent103, 0, 10);
            }
            else
            {
                PrizeEvents.injectReplacementPattern(newEvent103, 0);
            }

            newEvent103.Add(EventCommandEnum.INCREMENT_FLAG.Value);
            newEvent103.Add(0x0A);
            newEvent103.Add(0x09); // idk
            newEvent103.Add(EventCommandEnum.DECREMENT_FLAG.Value);
            newEvent103.Add(0x0A);
            newEvent103.Add(0x09); // idk
            if (goal != OpenWorldGoalProcessor.GOAL_GIFTMODE && goal != OpenWorldGoalProcessor.GOAL_REINDEER && !flammieDrumInLogic)
            {
                newEvent103.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                newEvent103.Add(0x00);
                // shake head
                newEvent103.Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + 0x00));
                newEvent103.Add(0x5F);
            }
            newEvent103.Add(EventCommandEnum.INCREMENT_FLAG.Value);
            newEvent103.Add(0xC1);

            bool anyCharsAdded = false;

            // if we should add the girl on startup, do it
            if (startWithGirl && startingChar != "girl")
            {
                newEvent103.Add(EventCommandEnum.NAME_CHARACTER.Value);
                newEvent103.Add(0x01); // name the girl

                newEvent103.Add(0x2B);
                newEvent103.Add(0x01);
                anyCharsAdded = true;
            }
            // if we should add the sprite on startup, do it
            if (startWithSprite && startingChar != "sprite")
            {
                newEvent103.Add(EventCommandEnum.NAME_CHARACTER.Value);
                newEvent103.Add(0x02); // name the sprite

                newEvent103.Add(0x2B);
                newEvent103.Add(0x02);
                anyCharsAdded = true;
            }
            // if we should add the boy on startup, do it
            if (startWithBoy && startingChar != "boy")
            {
                newEvent103.Add(EventCommandEnum.NAME_CHARACTER.Value);
                newEvent103.Add(0x00); // name the boy

                // add boy
                newEvent103.Add(0x2B);
                newEvent103.Add(0x00);
                anyCharsAdded = true;
            }

            if (startWithBoy)
            {
                newEvent103.Add(EventCommandEnum.SET_FLAG.Value);
                newEvent103.Add(0x0C);
                newEvent103.Add(0x01);
            }

            if (startWithGirl)
            {
                newEvent103.Add(EventCommandEnum.SET_FLAG.Value);
                newEvent103.Add(0x0D);
                newEvent103.Add(0x01);
            }

            if (startWithSprite)
            {
                newEvent103.Add(EventCommandEnum.SET_FLAG.Value);
                newEvent103.Add(0x0E);
                newEvent103.Add(0x01);
            }

            if (anyCharsAdded)
            {
                PrizeEvents.injectReplacementPattern(newEvent103, 1, false);
            }
            // flammie drum
            if (!flammieDrumInLogic)
            {
                newEvent103.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                newEvent103.Add(0x47);
            }

            // magic rope
            newEvent103.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
            newEvent103.Add(0x46);

            // game sits for a while while x103 plays; this lets you know it's done
            if (flammieDrumInLogic && goal != OpenWorldGoalProcessor.GOAL_GIFTMODE && goal != OpenWorldGoalProcessor.GOAL_REINDEER)
            {
                List<byte> flammieDrumIntroDialogue = VanillaEventUtil.getBytesDelay(VanillaEventUtil.wordWrapText("Okay you can play now"), 10, 0);
                newEvent103.Add(EventCommandEnum.OPEN_DIALOGUE.Value);
                foreach (byte b in flammieDrumIntroDialogue)
                {
                    newEvent103.Add(b);
                }
                newEvent103.Add(EventCommandEnum.SLEEP_FOR.Value);
                newEvent103.Add(0x0A);
                newEvent103.Add(EventCommandEnum.CLOSE_DIALOGUE.Value);
            }
            newEvent103.Add(EventCommandEnum.END.Value);

            return true;
        }
    }
}
