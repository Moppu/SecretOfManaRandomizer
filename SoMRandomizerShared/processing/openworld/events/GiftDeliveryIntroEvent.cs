using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.hacks.openworld.XmasRandoData;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Open world intro event for christmas gift mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class GiftDeliveryIntroEvent : RandoProcessor
    {
        public static int[] SANTA_DIALOGUE_EVENTS = new int[] { 0x137, 0x138, 0x139, 0x13a, 0x13b, 0x13c, 0x13d, 0x13e, 0x13f, 0x140, 0x200, 0x201, 0x202, 0x126, 0x122, 0x121 };

        public static NpcLocationData[] DELIVERY_LOCATIONS = new NpcLocationData[]
        {
            // possible gift deliveries, with their associated event, and any item dependencies to reach them
            new NpcLocationData("luka", "Luka at the Water Palace", 0x132, true, new string[] { }), //
            new NpcLocationData("wind palace elder", "The Elder at the Wind Palace", 0x4e2, false, new string[] { }),
            new NpcLocationData("dryad", "Dryad on the Lost Continent", 0x4b6, true, new string[] { }),
            new NpcLocationData("salamando", "Salamando in the Fire Palace", 0x589, false, new string[] { }),
            new NpcLocationData("undine", "Undine in the Tonpole Cave", 0x581, true, new string[] { }),
            new NpcLocationData("gnome", "Gnome in the Underground Palace", 0x22e, false, new string[] { }),
            new NpcLocationData("luna", "Luna at the Lunar Palace", 0x584, true, new string[] { }),
            new NpcLocationData("shade", "Shade at the Palace of Darkness", 0x586, false, new string[] { }),
            new NpcLocationData("lumina", "Lumina in the Gold Tower", 0x587, true, new string[] { }),
            new NpcLocationData("dwarf elder", "The Dwarf Elder in Gaia's Navel", 0x366, false, new string[] { }),
            new NpcLocationData("potos elder", "The Potos Elder", 0x114, false, new string[] { }),
            new NpcLocationData("elinee", "Elinee at her castle", 0x1ee, true, new string[] { "elinee" }),
            new NpcLocationData("watts", "Watts at Gaia's Navel", 0x1b0, false, new string[] { }),
            new NpcLocationData("jema", "Jema at the Lost Continent", 0x4ac, false, new string[] { }),
            new NpcLocationData("mara", "Mara in South Town", 0x399, true, new string[] { }),
            new NpcLocationData("pecard", "Pecard the Lighthouse Keeper", 0xaa, false, new string[] { }),
            new NpcLocationData("girl at pandora", "The Girl (Primm) at Pandora Castle", 0x188, true, new string[] { }),
            new NpcLocationData("jehk", "Jehk in the mountains", 0x3dd, false, new string[] { }),
            new NpcLocationData("johk", "Johk in the mountains", 0x3dc, false, new string[] { }),
            new NpcLocationData("dyluck", "Dyluck at North Town Ruins", 0x555, false, new string[] { }),
            new NpcLocationData("krissie", "Krissie in North Town", 0x3ce, true, new string[] { }),
            new NpcLocationData("pandora king", "The King of Pandora", 0x1a0, false, new string[] { }),
            new NpcLocationData("flower guy", "The Flower Guy North of Gaia's Navel", 0x21c, false, new string[] { }),
            new NpcLocationData("truffle", "King Truffle of Matango", 0x4e3, false, new string[] { }),
            new NpcLocationData("southtown password guy", "The Password Guy in South Town", 0x5b1, false, new string[] { }),
            new NpcLocationData("king amar", "King Amar of Kakkara", 0x2b0, false, new string[] { }),
            new NpcLocationData("rudolph", "Rudolph", 0x37c, false, new string[] { }),
        };
        public const string GIFT_DELIVERY_INDEX_PREFIX = "giftDelivery";

        protected override string getName()
        {
            return "Event 0x106 - intro (xmas gift delivery)";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME) != OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                return false;
            }
            Random r = context.randomFunctional;
            // add on to existing event 0x106
            EventScript eventData = (EventScript)context.replacementEvents[0x106];
            string startingChar = context.workingData.get(OpenWorldCharacterSelection.STARTING_CHARACTER);
            bool restrictiveLogic = settings.get(OpenWorldSettings.PROPERTYNAME_LOGIC_MODE) == "restrictive";
            // gift exchange mode
            int numGifts = settings.getInt(OpenWorldSettings.PROPERTYNAME_NUM_XMAS_GIFTS);

            // take these out of the pool for restrictive mode, since they'll ask for the seed
            string[] noRestrictiveModeXmasLocations = new string[] { "luka", "wind palace elder", "dryad", "salamando", "gnome", "luna", "shade", "lumina" };
            List<int> chosenDeliveryIndexes = new List<int>();
            for (int i = 0; i < numGifts; i++)
            {
                int giftIndex = r.Next() % DELIVERY_LOCATIONS.Length;
                // don't allow restrictive logic spots to be gifts
                while (chosenDeliveryIndexes.Contains(giftIndex) || (noRestrictiveModeXmasLocations.Contains(DELIVERY_LOCATIONS[giftIndex].key) && restrictiveLogic))
                {
                    giftIndex = r.Next() % DELIVERY_LOCATIONS.Length;
                }
                // publish to the context so the rest of the rando knows which ones we picked
                context.workingData.setInt(GIFT_DELIVERY_INDEX_PREFIX + i, giftIndex);
                chosenDeliveryIndexes.Add(giftIndex);
            }

            // some empty ones we can replace here
            int[] npcDialogueEvents = new int[] { 0x7DF, 0x7E0, 0x7E1, 0x7E2, 0x7E3, 0x7E4, 0x7E5, 0x7E6, 0x7E7, 0x7E8, 0x7E9, 0x7EA, 0x7EB, 0x7EC, 0x7ED, 0x7EE, };

            List<NpcLocationData> chosenDeliveryLocations = new List<NpcLocationData>();
            int id = 0;
            foreach (int index in chosenDeliveryIndexes)
            {
                chosenDeliveryLocations.Add(DELIVERY_LOCATIONS[index]);
                DELIVERY_LOCATIONS[index].newEventBeforeGift = npcDialogueEvents[id * 2 + 0];
                DELIVERY_LOCATIONS[index].newEventGiveGift = npcDialogueEvents[id * 2 + 1];
                id++;
            }

            // play the ice country music
            eventData.Add(EventCommandEnum.PLAY_SOUND.Value);
            eventData.Add(0x01);
            eventData.Add(0x24);
            eventData.Add(0x1c);
            eventData.Add(0x8f);

            string characterName = "(" + startingChar + ")";

            // replace santa dialogue with various instructions and rewards
            EventScript newEvent2fa = new EventScript();
            context.replacementEvents[0x2fa] = newEvent2fa;
            newEvent2fa.Jump(0x711);
            newEvent2fa.End();

            EventScript newEvent2fb = new EventScript();
            context.replacementEvents[0x2fb] = newEvent2fb;
            newEvent2fb.Door(0x276);
            newEvent2fb.End();

            // event flag 6f
            // 0 - on first delivery
            // 1 - first done (some extra dialogue from santa + gift inbetween)
            // 2 - on second delivery
            // 3 - second done
            // 4 - on third delivery
            // 5 - third done
            // 6 - on fourth delivery
            // 7 - fourth done
            // 8 - on fifth delivery
            // 9 - fifth done
            // a - on sixth delivery
            // b - sixth done
            // c - on seventh delivery
            // d - seventh done
            // e - on eighth delivery
            // f - eighth done - end game at santa

            EventScript santa_mainEvent = new EventScript();
            context.replacementEvents[0x37e] = santa_mainEvent;
            List<string> santaDeliveryDialogue = new List<string>();
            // make stuff for santa to say

            for (int i = 0; i < numGifts; i++)
            {
                int eventFlagValueA = i * 2;
                int eventFlagValueB = i * 2 + 1;
                santa_mainEvent.Logic(EventFlags.OPEN_WORLD_CHRISTMAS_PROGRESS, (byte)eventFlagValueA, (byte)eventFlagValueA, EventScript.GetJumpCmd(SANTA_DIALOGUE_EVENTS[eventFlagValueA]));
                santa_mainEvent.Logic(EventFlags.OPEN_WORLD_CHRISTMAS_PROGRESS, (byte)eventFlagValueB, (byte)eventFlagValueB, EventScript.GetJumpCmd(SANTA_DIALOGUE_EVENTS[eventFlagValueB]));

                string deliveryDialogue = "";
                string prizeDialogue = "";
                if (i == 0)
                {
                    string s1 = VanillaEventUtil.wordWrapText("The first one is to " + chosenDeliveryLocations[i].description + "! " + (chosenDeliveryLocations[i].female ? "She" : "He") + " wants:") + "\n";
                    string s2 = VanillaEventUtil.wordWrapText("%GIFT!");
                    deliveryDialogue = s1 + s2;
                    prizeDialogue = VanillaEventUtil.wordWrapText("You did it, " + characterName + "! You delivered the first one! Hold on, here, I've got something for you..");
                }
                else if (i < numGifts - 1)
                {
                    string s1 = VanillaEventUtil.wordWrapText("The next one is to " + chosenDeliveryLocations[i].description + "! " + (chosenDeliveryLocations[i].female ? "She" : "He") + " wants:") + "\n";
                    string s2 = VanillaEventUtil.wordWrapText("%GIFT!");
                    deliveryDialogue = s1 + s2;
                    prizeDialogue = VanillaEventUtil.wordWrapText("Alright! You did another one, " + characterName + "! What have I got for you? Let's see here..");
                }
                else
                {
                    string s1 = VanillaEventUtil.wordWrapText("The last one is to " + chosenDeliveryLocations[i].description + "! " + (chosenDeliveryLocations[i].female ? "She" : "He") + " wants:") + "\n";
                    string s2 = VanillaEventUtil.wordWrapText("%GIFT!");
                    deliveryDialogue = s1 + s2;
                    prizeDialogue = VanillaEventUtil.wordWrapText("You did it! You delivered all the things! You finished the thing! I'm all out of gifts! All I've got left is the unmodified, vanilla credits. Enjoy!");
                }
                santaDeliveryDialogue.Add(deliveryDialogue);

                EventScript newEvent_deliveryInfo = new EventScript();
                context.replacementEvents[SANTA_DIALOGUE_EVENTS[eventFlagValueA]] = newEvent_deliveryInfo;
                newEvent_deliveryInfo.AddDialogueBox(deliveryDialogue);
                newEvent_deliveryInfo.End();

                EventScript newEvent_reward = new EventScript();
                // odd values of event flag; give reward then increment flag
                context.replacementEvents[SANTA_DIALOGUE_EVENTS[eventFlagValueB]] = newEvent_reward;
                newEvent_deliveryInfo.AddDialogueBox(prizeDialogue);
                newEvent_reward.IncrFlag(EventFlags.OPEN_WORLD_CHRISTMAS_PROGRESS);
                if (i < numGifts - 1)
                {
                    PrizeEvents.injectReplacementPattern(newEvent_reward, 0);
                }
                else
                {
                    // credits
                    newEvent_reward.Jump(0x42f);
                }
                newEvent_reward.End();
            }

            santa_mainEvent.End();

            int numLines = 1;
            string s = santaDeliveryDialogue[0];
            string finalString = s;
            while (true)
            {
                int newLineIndex = s.IndexOf("\n");
                if (newLineIndex >= 0)
                {
                    try
                    {
                        s = s.Substring(newLineIndex + 1);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        s = "";
                    }
                    numLines++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < (numLines % 3); i++)
            {
                finalString += "\n";
            }

            // dialogue, then jump to 103
            eventData.AddAutoTextDialogueBox(characterName + "!\n" +
                "You gotta help me, " + characterName + "!\n" +
                "\n" +

                "The Christmas lists got all\n" +
                "mixed up, " + characterName + "!\n" +
                "\n" +

                "I got the wrong gift for\n" +
                "everyone!\n" +
                "\n" +

                "You've gotta help me\n" +
                "find the right gifts and\n" +
                "deliver them, " + characterName + "!\n" +

                "You can do that, right?\n" +
                "Sure you can!\n" +
                "\n" +

                "I've got eight deliveries\n" +
                "for you to run! Come back\n" +
                "after each one and I'll\n" +

                ".. I'll give you some of\n" +
                "the other stuff I've got!\n" +
                "\n" +

                finalString +

                "Oh, right! Here's\n" +
                "your starting weapon!\n", 0x0A);

            // event 0x107 - initialize event flags
            eventData.Jsr(0x107);

            // -> event x103, the sword grab event
            eventData.Jump(0x103);
            eventData.End();

            return true;
        }
    }
}
