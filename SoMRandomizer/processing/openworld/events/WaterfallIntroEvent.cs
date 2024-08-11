using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Open world intro event for modes that start at the waterfall like vanilla.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class WaterfallIntroEvent : RandoProcessor
    {
        protected override string getName()
        {
            return "Event 0x106 & 0x101 - intro (waterfall)";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if (settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC) || 
                context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME) == OpenWorldGoalProcessor.GOAL_GIFTMODE || 
                context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME) == OpenWorldGoalProcessor.GOAL_REINDEER)
            {
                return false;
            }
            Random r = context.randomFunctional;
            EventScript eventData = (EventScript)context.replacementEvents[0x106];
            // vanilla intro event with the log, but shortened
            eventData.Jsr(0x73D);
            eventData.Add(EventCommandEnum.PLAY_SOUND.Value);
            eventData.Add(0x01);
            eventData.Add(0x1C);
            eventData.Add(0x4A);
            eventData.Add(0x2F);
            // keep doing movements but skip the dialogue
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x00);
            eventData.Add(0x03);
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x04);
            eventData.Add(0xC8);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x00);
            eventData.Add(0xC0);
            // boy: hey!
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x04);
            eventData.Add(0xC8);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            // guys!
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x04);
            eventData.Add(0xC8);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            // wait
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x04);
            eventData.Add(0xC8);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            // up!
            eventData.SetFlag(EventFlags.WATERFALL_BOY_VISIBILITY_FLAG, 1);
            eventData.Sleep(0x10);
            eventData.Add(EventCommandEnum.INVIS_B.Value);
            eventData.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // idk

            eventData.Add(EventCommandEnum.PLAYER_ANIM.Value);
            eventData.Add(0x00);
            eventData.Add(0x88);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            eventData.Add(EventCommandEnum.PLAYER_ANIM.Value);
            eventData.Add(0x00);
            eventData.Add(0x88);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            // bunch of dialogue skipped here

            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x00);
            eventData.Add(0x00);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            eventData.Add(EventCommandEnum.CHARACTER_ANIM.Value);
            eventData.Add(0x00);
            eventData.Add(0x88);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);


            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x05);
            eventData.Add(0x80);
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x06);
            eventData.Add(0x80);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            eventData.SetFlag(EventFlags.WALK_THROUGH_WALLS_FLAG, 4);

            // falling animation
            eventData.Add(EventCommandEnum.PLAYER_ANIM.Value);
            eventData.Add(0x00);
            eventData.Add(0xA0);

            eventData.Sleep(0x0C);

            // watch the boy character fall, do nothing
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x05);
            eventData.Add(0x42);
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x06);
            eventData.Add(0x42);
            eventData.Jsr(0x748);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            eventData.Add(EventCommandEnum.INVIS_A.Value);
            eventData.SetFlag(EventFlags.WALK_THROUGH_WALLS_FLAG, 0);
            eventData.Sleep(0x10);

            // face each other.  shrug.
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x05);
            eventData.Add(0x80);
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x06);
            eventData.Add(0xC0);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            eventData.Sleep(0x0C);

            // silly shit to say after you fall
            List<string> introDialogues = new string[] {
                    "Who was that idiot?",
                    "Look for me later at the\nMantis Ant fight",
                    "And then there were two",
                    "Let's see what's to\nthe left",
                    "Thanks for choosing Secret\nof Mana Randomizer",
                    "Blame Moppleton for\nany softlocks :3",
                    "Coming soon:\nSD3 Randomizer!\nby someone else.",
                    "Man, there's a\nrandomizer for everything\nthese days",
                    "Your ad here:\n10000 GP",
                    "Open world:\nnow 20% more open!",
                    "Totally not a ripoff of\nFF4 Free Enterprise",
                    "This is a good game\nand it works well",
                    "Oh no!\n\nAnyway",
                    VanillaEventUtil.wordWrapText("That's some bad hat, Harry"),
            }.ToList();

            string[] lttpPlaces = new string[]
            {
                "The Light World",
                "The Dark World",
                "Eastern Palace",
                "Desert Palace",
                "Tower of Hera",
                "Hyrule Castle",
                "Palace of Darkness",
                "Swamp Palace",
                "Skull Woods",
                "Thieves Town",
                "Ice Palace",
                "Misery Mire",
                "Turtle Rock",
                "Ganon's Tower",
                "Kakariko",
            };

            // stick a few of these uhh wtf do they call it
            // ambrosias in too
            int num = introDialogues.Count;

            for (int i = 0; i < num / 3; i++)
            {
                introDialogues.Add("Link, the boots are in\n" + lttpPlaces[r.Next() % lttpPlaces.Length]);
            }

            string introDialogue = introDialogues[r.Next() % introDialogues.Count];

            eventData.AddAutoTextDialogueBox(introDialogue, 0x0A);

            // oh well, let's go back
            eventData.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            eventData.Add(0x05);
            eventData.Add(0xC0);
            eventData.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            eventData.Add(EventCommandEnum.CHARACTER_ANIM.Value);
            eventData.Add(0x05);
            eventData.Add(0x82);
            eventData.Add(EventCommandEnum.CHARACTER_ANIM.Value);
            eventData.Add(0x06);
            eventData.Add(0x82);
            eventData.Sleep(0x04);
            eventData.Door(0x82);
            // event 0x107 - initialize event flags
            eventData.Logic(EventFlags.UI_DISPLAY_FLAG, 0x0, 0x0, EventScript.GetJsrCmd(0x107));
            // -> event x101, the falling event
            eventData.Jump(0x101);
            eventData.End();


            // 0x101: the fall part afterwatd; not much to this one
            EventScript newEvent101 = new EventScript();
            context.replacementEvents[0x101] = newEvent101;
            newEvent101.Sleep(0x10);
            newEvent101.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent101.Add(0x00);
            newEvent101.Add(0xC0);
            newEvent101.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent101.Add(EventCommandEnum.PLAYER_ANIM.Value);
            newEvent101.Add(0x00);
            newEvent101.Add(0xA2);
            newEvent101.Sleep(0x02);
            newEvent101.Add(EventCommandEnum.INVIS_B.Value);
            newEvent101.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent101.Add(0x02);
            newEvent101.Add(0x20);
            newEvent101.Add(0x00);
            newEvent101.Add(0x88);
            newEvent101.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent101.Add(0x05);
            newEvent101.Add(0x80);
            newEvent101.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent101.Jsr(0x5F);
            newEvent101.Jsr(0x749);
            newEvent101.End();

            return true;
        }
    }
}
