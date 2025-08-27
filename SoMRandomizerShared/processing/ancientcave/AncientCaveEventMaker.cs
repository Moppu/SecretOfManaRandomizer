using SoMRandomizer.config;
using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.mapgen;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.procgen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.ancientcave
{
    /// <summary>
    /// Generates replacement events for the random NPCs to make them do useful things.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AncientCaveEventMaker
    {
        public static List<byte> UNKNOWN_DOOR_REPLACEMENT_PATTERN = new byte[] { 0xFF, 0xFF, 0xFF }.ToList();

        public const string BOSS_EVENT_FLOORNUM = "bossEventFloor";
        public const string BOSS_ENTRY_EVENT_EVENTNUM = "bossEntryEvent";
        public const string BOSS_DEATH_EVENT_EVENTNUM = "bossDeathEvent";
        public const string BOSS_EVENT_TOTAL = "bossEventNum";

        public const string WALKON_EVENT_EVENTNUM = "walkonEvent";
        public const string WALKON_EVENT_TOTAL = "walkonEventNum";

        public const string NUM_DIALOGUE_EVENTS = "numDialogueEvents";

        public const string USEFUL_EVENTS_EVENTID = "usefulEvents";
        public const string USEFUL_EVENTS_NPCID = "usefulEventsNpc";

        public const int START_EVENT_NUM = 0x100;
        public const int MANABEAST_WALKON_EVENT_NUM = 0x101;

        public Dictionary<int, List<NpcTaggedEvent>> process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Dictionary<int, List<int>> bossEvents = new Dictionary<int, List<int>>();
            int numMaps = AncientCaveGenerator.LENGTH_CONVERSIONS[settings.get(AncientCaveSettings.PROPERTYNAME_LENGTH)];
            double scalingRate = Math.Ceiling(24.0 / numMaps);
            Random r = context.randomFunctional;

            Dictionary<int, int> walkOnEvents = new Dictionary<int, int>();
            int eventNum = START_EVENT_NUM;
            // intro event
            walkOnEvents.Add(0, eventNum);
            context.workingData.setInt(WALKON_EVENT_EVENTNUM + 0, eventNum);
            context.workingData.setInt(WALKON_EVENT_TOTAL, 65);

            // Overwrite events 0x100 -> whatever for map entry events?
            // normally:
            // 400 (intro scenes)
            // 106 (waterfall scene)
            // 101 (falling)

            // -------------------------------
            // event 0x100: game start
            // -------------------------------

            EventScript currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;

            // skip out if 00 is 1+
            currentEvent.Logic(EventFlags.UI_DISPLAY_FLAG, 0x1, 0xF, EventScript.GetJumpCmd(0));

            // enable UI
            currentEvent.IncrFlag(EventFlags.UI_DISPLAY_FLAG);
            currentEvent.IncrFlag(EventFlags.UI_DISPLAY_FLAG);

            // flag 0x0B -> 0x05 for normal death
            currentEvent.SetFlag(EventFlags.DEATH_TYPE_FLAG, 5);

            // you start with sword after bumping event flag 0x00 i think (some other modes hack this out)
            currentEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add glove
            currentEvent.Add(0x80);
            currentEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add axe
            currentEvent.Add(0x92);
            currentEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add spear
            currentEvent.Add(0x9B);
            currentEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add whip
            currentEvent.Add(0xA4);
            currentEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add bow
            currentEvent.Add(0xAD);
            currentEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add boomerang
            currentEvent.Add(0xB6);
            currentEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add javelin
            currentEvent.Add(0xBF);

            // add gold
            int startingGold = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.STARTING_GOLD);
            if (startingGold > 65535)
            {
                startingGold = 65535;
            }
            if (startingGold > 0)
            {
                currentEvent.Add(EventCommandEnum.ADD_GOLD.Value);
                currentEvent.Add((byte)startingGold);
                currentEvent.Add((byte)(startingGold >> 8));
            }

            // name girl/sprite
            if (settings.getBool(AncientCaveSettings.PROPERTYNAME_INCLUDE_GIRL_CHARACTER))
            {
                currentEvent.Add(EventCommandEnum.NAME_CHARACTER.Value);
                currentEvent.Add(0x01);
            }
            if (settings.getBool(AncientCaveSettings.PROPERTYNAME_INCLUDE_SPRITE_CHARACTER))
            {
                currentEvent.Add(EventCommandEnum.NAME_CHARACTER.Value);
                currentEvent.Add(0x02);
            }

            // Add p2, p3
            if (settings.getBool(AncientCaveSettings.PROPERTYNAME_INCLUDE_GIRL_CHARACTER))
            {
                currentEvent.Add(EventCommandEnum.ADD_CHARACTER.Value);
                currentEvent.Add(0x01);
            }
            if (settings.getBool(AncientCaveSettings.PROPERTYNAME_INCLUDE_SPRITE_CHARACTER))
            {
                currentEvent.Add(EventCommandEnum.ADD_CHARACTER.Value);
                currentEvent.Add(0x02);
            }

            int startingConsumables = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.STARTING_CONSUMABLES);

            // add items
            // 3x, 1x, 0x candy, chocolate, jam, herb, walnut, cup
            byte[] startingItems = new byte[] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45 };
            foreach (byte startingItem in startingItems)
            {
                for (int i = 0; i < startingConsumables; i++)
                {
                    currentEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    currentEvent.Add(startingItem);
                }
            }

            // flammie drum
            currentEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
            currentEvent.Add(0x47);

            // and magic rope.
            currentEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
            currentEvent.Add(0x46);

            // restore everyone's hp
            currentEvent.Add(EventCommandEnum.HEAL.Value);
            currentEvent.Add(0x00);

            // add music injection pattern that the tileset for the map will replace
            currentEvent.AddRange(AncientCaveRandomizer.MUSIC_REPLACEMENT_PATTERN);

            double manaPowerBase = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.MANA_POWER + "_base");
            double manaPowerMul = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.MANA_POWER + "_growth") * scalingRate;
            double manaPowerExp = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.MANA_POWER + "_exp");

            // mana power base level
            currentEvent.SetFlag(0xFC, (byte)manaPowerBase);

            for (int i = 0; i < (int)manaPowerBase; i++)
            {
                // grant seeds up to mana power base
                currentEvent.SetFlag((byte)(0x90 + i), 1);
            }

            currentEvent.AddDialogueBox(
                "Ancient Cave mode still\n" +
               "needs a lot of work, so\n" +
               "please be aware that not\n" +
               "everything may work well,\n" +
               "balancing issues may\n" +
               "exist, and softlocks\n" +
               "might happen.  Enjoy!\n");

            // remove boy character if applicable
            if (!settings.getBool(AncientCaveSettings.PROPERTYNAME_INCLUDE_BOY_CHARACTER))
            {
                currentEvent.Add(EventCommandEnum.REMOVE_CHARACTER.Value);
                currentEvent.Add(0x00);
            }

            // set timer flag on
            currentEvent.SetFlag(EventFlags.PROCGEN_MODE_TIMER_RUNNING_FLAG, 1);

            // end intro event
            currentEvent.End();

            // -------------------------------
            // event 0x101: mana beast warp/intro
            // -------------------------------

            // Mana beast transition event for final floor
            eventNum++; // 0x101
            // mana beast warper event = x101
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;

            // mana beast
            currentEvent.SetFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1, 8);

            // play the mana beast theme by JSRing to vanilla event x739
            currentEvent.Jsr(0x739);

            // end event
            currentEvent.End();

            double manaPower = manaPowerBase;

            eventNum++; // start at 0x102
            // MOPPLE: it doesn't matter too much, but we can probably reduce this to just make numFloors of these now instead of 64;
            // previously it was useful to know the event offset where they ended, but they are tracked differently now
            for (int i = 1; i <= 64; i++)
            {
                // -------------------------------
                // event 0x102+: floor 2, 3, 4... entry events
                // -------------------------------
                currentEvent = new EventScript();
                context.replacementEvents[eventNum] = currentEvent;

                // mana power
                double manaPowerThisFloor = (manaPowerBase + Math.Pow(i, manaPowerExp) * manaPowerMul);
                Logging.log("Mana power for floor " + i + " = " + manaPowerThisFloor, "debug");
                if ((int)manaPowerThisFloor > (int)manaPower)
                {
                    // set mana power flag
                    currentEvent.SetFlag(0xFC, (byte)manaPowerThisFloor);

                    // set mana seed flags
                    for (int j = (int)(manaPower); j < (int)manaPowerThisFloor && j <= 8; j++)
                    {
                        currentEvent.SetFlag((byte)(0x90 + j), 1);
                    }
                }
                manaPower = manaPowerThisFloor;

                // return from event if custom music set
                currentEvent.Logic(EventFlags.PROCGEN_MODE_CUSTOM_MUSIC_FLAG, 0x1, 0xF, EventScript.GetJumpCmd(0));

                // otherwise set music, then end
                currentEvent.AddRange(AncientCaveRandomizer.MUSIC_REPLACEMENT_PATTERN);

                currentEvent.End();

                walkOnEvents.Add(i, eventNum);
                context.workingData.setInt(WALKON_EVENT_EVENTNUM + i, eventNum);
                eventNum++;
            }

            // eventNum = 0x142 after loop


            // -------------------------------
            // event 0x142: mana beast death, ending, statistics list
            // -------------------------------

            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;

            int manaBeastDeathEvent = eventNum;
            context.workingData.setInt(ManaBeastMap.MANABEAST_DEATH_EVENT, manaBeastDeathEvent);
            context.workingData.setInt(ManaBeastMap.MANABEAST_WALKON_EVENT, MANABEAST_WALKON_EVENT_NUM);

            // event flags
            currentEvent.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            currentEvent.IncrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);

            // set timer flag off
            currentEvent.SetFlag(EventFlags.PROCGEN_MODE_TIMER_RUNNING_FLAG, 0);

            // modified for changes in DoorExpansion.cs
            currentEvent.ExtendedDoor(0x3FF); // door 3FF

            // ending music
            currentEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
            currentEvent.Add(0x01);
            currentEvent.Add(0x3a);
            currentEvent.Add(0x1b);
            currentEvent.Add(0x8f);

            currentEvent.OpenDialogueBox();
            currentEvent.AddDialogue("Hooray! You did it.\nTime: ");
            // print timer value
            currentEvent.Add(EventCommandEnum.CUSTOM_EVENT_COMMANDS.Value);
            currentEvent.Add(context.eventHackMgr.getCommandIndex(TimerDialogue.EVENT_COMMAND_NAME_PRINT_TIMER));

            currentEvent.AddDialogue("\nOther statistics:");

            currentEvent.DialogueWait(); // wait for button press

            List<string> statisticsMessages = new List<string>();
            List<int> statisticsOffsets = new List<int>();
            List<bool> statistics32Bit = new List<bool>();

            // string length 19 for 32bit, 23 for 16bit
            statisticsMessages.Add("Total kills:");
            statisticsMessages.Add("Total deaths:");
            statisticsMessages.Add("");
            statisticsOffsets.Add(CustomRamOffsets.PLAYER_KILLS_OFFSET_16BIT);
            statisticsOffsets.Add(CustomRamOffsets.ENEMY_KILLS_OFFSET_16BIT);
            statistics32Bit.Add(false);
            statistics32Bit.Add(false);

            statisticsMessages.Add("Damage dealt:");
            statisticsMessages.Add("(Physical)");
            statisticsMessages.Add("(Magical)");
            statisticsOffsets.Add(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT);
            statisticsOffsets.Add(CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT);
            statisticsOffsets.Add(CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT);
            statistics32Bit.Add(true);
            statistics32Bit.Add(true);
            statistics32Bit.Add(true);

            statisticsMessages.Add("Damage taken:");
            statisticsMessages.Add("(Physical)");
            statisticsMessages.Add("(Magical)");
            statisticsOffsets.Add(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT);
            statisticsOffsets.Add(CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT);
            statisticsOffsets.Add(CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT);
            statistics32Bit.Add(true);
            statistics32Bit.Add(true);
            statistics32Bit.Add(true);

            statisticsMessages.Add("Distance walked:");
            statisticsMessages.Add("Chests opened:");
            statisticsMessages.Add("");
            statisticsOffsets.Add(CustomRamOffsets.MOVE_DIST_32BIT);
            statisticsOffsets.Add(CustomRamOffsets.NUM_CHESTS_OFFSET_16BIT);
            statistics32Bit.Add(true);
            statistics32Bit.Add(false);


            int actualMessageIndex = 0;

            for (int i = 0; i < statisticsMessages.Count; i++)
            {
                string str = statisticsMessages[i];

                if (str.Length > 0)
                {
                    bool is32Bit = statistics32Bit[actualMessageIndex];
                    int totalStrLength = is32Bit ? 19 : 22;
                    int padding = totalStrLength - str.Length;
                    for (int j = 0; j < padding; j++)
                    {
                        str = str + " ";
                    }

                    str = "\n" + str;
                    currentEvent.AddDialogue(str);

                    int offset = statisticsOffsets[actualMessageIndex];

                    if (is32Bit)
                    {
                        currentEvent.Add(EventCommandEnum.CUSTOM_EVENT_COMMANDS.Value);
                        currentEvent.Add(context.eventHackMgr.getCommandIndex(TimerDialogue.EVENT_COMMAND_NAME_PRINT_32_BIT_VALUE));
                        currentEvent.Add((byte)offset);
                        currentEvent.Add((byte)(offset >> 8));
                    }
                    else
                    {
                        currentEvent.Add(EventCommandEnum.CUSTOM_EVENT_COMMANDS.Value);
                        currentEvent.Add(context.eventHackMgr.getCommandIndex(TimerDialogue.EVENT_COMMAND_NAME_PRINT_16_BIT_VALUE));
                        currentEvent.Add((byte)offset);
                        currentEvent.Add((byte)(offset >> 8));
                    }
                    actualMessageIndex++;
                }
                else
                {
                    currentEvent.AddDialogue("\n");
                }
                if ((i % 3) == 2)
                {
                    // wait for button press every 3 lines
                    currentEvent.DialogueWait();
                }
            }

            currentEvent.AddDialogue("\nThe end!");

            currentEvent.DialogueWait(); // wait for button press one last time
            currentEvent.CloseDialogueBox(); // close dialogue

            currentEvent.Add(0x1f); // end game
            currentEvent.Add(0x11);

            // end of event
            currentEvent.End();

            // Now the NPC dialogue
            eventNum = 0x280;
            // -------------------------------
            // event 0x280+: NPC dialogue that is just comedian quotes
            // -------------------------------

            string[] quotesSrc = new string[] { "Quotes didn't load!" };
            switch (settings.get(AncientCaveSettings.PROPERTYNAME_DIALOGUE_SOURCE))
            {
                case "mitch":
                    quotesSrc = MitchHedbergQuotes.mitchQuotes;
                    break;
                case "demetri":
                    quotesSrc = DemetriMartinQuotes.quotes;
                    break;
            }

            context.workingData.setInt(NUM_DIALOGUE_EVENTS, quotesSrc.Length);

            foreach (string _quote in quotesSrc)
            {
                currentEvent = new EventScript();
                context.replacementEvents[eventNum] = currentEvent;
                string quote = _quote;
                if (settings.getBool(AncientCaveSettings.PROPERTYNAME_PROFANITY_FILTER))
                {
                    quote = quote.Replace("Fuck", "F---");
                    quote = quote.Replace("Shit", "S---");
                    quote = quote.Replace("fuck", "f---");
                    quote = quote.Replace("shit", "s---");
                }
                currentEvent.AddDialogueBox(quote);
                currentEvent.End();
                eventNum++;
            }

            bool useBosses = true;
            bool randomizeBosses = true;
            switch (settings.get(AncientCaveSettings.PROPERTYNAME_BOSS_FREQUENCY))
            {
                case "every":
                    useBosses = true;
                    randomizeBosses = false;
                    break;
                case "everyfew":
                    useBosses = true;
                    randomizeBosses = true;
                    break;
                case "final":
                    useBosses = false;
                    break;
            }

            int bossGranularity = numMaps / 8;
            if (useBosses)
            {
                int bossFloor = randomizeBosses ? (r.Next() % 3) + bossGranularity : 0;
                // don't put a boss on the final floor
                while (bossFloor < numMaps - 1)
                {
                    Logging.log("Boss on floor " + (bossFloor + 1) + ".", "spoiler");
                    bossEvents.Add(bossFloor, new List<int>());
                    bossFloor += randomizeBosses ? (r.Next() % 3) + bossGranularity : 1;
                }
            }


            Dictionary<int, List<NpcTaggedEvent>> events = new Dictionary<int, List<NpcTaggedEvent>>();

            // 3 per floor?  weight by number of floors somehow as well
            // put a couple of each type in in case we miss them
            // things:
            // P2/P3 join (nope)
            // npc ids = 8A, 8B, 8C
            // Spell NPCs = 90 gnome, 91 undine, 92 salamando, 93 sylphid, 94 lumina, 95 shade, 96 luna, 97 dryad
            eventNum = 0x180;
            // -------------------------------
            // event 0x180 - 0x18f: element dialogue & already got it events
            // -------------------------------

            // sorted in order of npc graphics
            byte[] elementAttributeFlags = new byte[] { 0xC8, 0xC9, 0xCA, 0xCB, 0xCF, 0xCE, 0xCC, 0xCD };
            List<int> elementEvents = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                currentEvent = new EventScript();
                context.replacementEvents[eventNum] = currentEvent;

                // make element events
                // flag 0x10 - 0x17
                // set char attribute - x38 for girl [39 02], x07 for sprite [39 03] (00 for lumina/shade with nothing)
                // gnome c8
                // undine c9
                // salamando ca
                // sylphid cb
                // luna cc
                // dryad cd
                // shade ce
                // lumina cf
                elementEvents.Add(eventNum);

                // event flag check - skip this and tell me to go away (next event)
                currentEvent.Logic((byte)(0x10 + i), 0x1, 0xF, EventScript.GetJumpCmd(eventNum + 1));

                // event flag increment
                currentEvent.IncrFlag((byte)(0x10 + i));

                // girl
                if (i != 5) // shade
                {
                    currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    currentEvent.Add(0x02); // girl
                    currentEvent.Add(elementAttributeFlags[i]);
                    if (i == 7)
                    {
                        // no mana magic
                        currentEvent.Add(0x18);
                    }
                    else
                    {
                        currentEvent.Add(0x38);
                    }
                }

                // sprite
                if (i != 4) // lumina
                {
                    currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    currentEvent.Add(0x03); // sprite
                    currentEvent.Add(elementAttributeFlags[i]);
                    if (i == 7)
                    {
                        // no mana magic
                        currentEvent.Add(0x03);
                    }
                    else
                    {
                        currentEvent.Add(0x07);
                    }
                }

                currentEvent.AddDialogueBox("Here, have some magic.");
                currentEvent.End();
                eventNum++;

                // next event: already got it
                currentEvent = new EventScript();
                context.replacementEvents[eventNum] = currentEvent;

                currentEvent.AddDialogueBox("Ok go away now.");
                currentEvent.End();
                eventNum++;
            }


            // -------------------------------
            // event 0x190: watts
            // -------------------------------
            int wattsEventNum = eventNum;
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;

            // GP view
            currentEvent.Add(EventCommandEnum.OPEN_GP.Value);
            currentEvent.Add(0x5F); // unsure what this is, a refresh gp value or something?

            currentEvent.AddDialogueBox("WATTS:Okay!\n Which one's ready?");

            // open the watts menu
            currentEvent.Add(0x0F);

            // hide GP
            currentEvent.Add(EventCommandEnum.CLOSE_GP.Value);

            // end event
            currentEvent.End();

            eventNum++;


            // -------------------------------
            // event 0x191, 0x192, 0x193: phanna HP restore, confirm, deny
            // -------------------------------
            int restoreEventNum = eventNum;
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;

            // subr 41B is restore from staying at an inn
            // next event: hp restore

            int restoreGoldAmount = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.RESTORE_COST);
            if (restoreGoldAmount > 65535)
            {
                restoreGoldAmount = 65535;
            }

            currentEvent.Add(EventCommandEnum.OPEN_GP.Value);
            currentEvent.Add(0x5F); // unsure what this is, a refresh gp value or something?

            // eventnum + 1 for yes, eventnum + 2 for cancel/no
            currentEvent.AddDialogueBoxWithChoices("HP MP Restore,\n" + restoreGoldAmount.ToString() + " Gold.\n",
                new byte[] { 2, 8 }, new string[] { "Sure", "Nah" }, new byte[][] { EventScript.GetJumpCmd(eventNum + 1), EventScript.GetJumpCmd(eventNum + 2) }, EventScript.GetJumpCmd(eventNum + 2));
            currentEvent.End();


            // event 0x192: confirm hp restore
            eventNum++;
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;

            currentEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);

            currentEvent.Add(EventCommandEnum.REMOVE_GOLD.Value);
            currentEvent.Add((byte)restoreGoldAmount);
            currentEvent.Add((byte)(restoreGoldAmount >> 8));

            currentEvent.Add(0x5F); // refresh GP display, i think?
            // this flag is hardcoded to be set by the vanilla REMOVE_GOLD command if we rolled under and couldn't afford the thing
            currentEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0xF, EventScript.GetJumpCmd(0x337)); // 0x337 is vanilla "not enough gold" event
            // after this point, we had enough gold, so do the thing
            // all hp
            currentEvent.Add(EventCommandEnum.HEAL.Value);
            currentEvent.Add(0x04);
            // all mp?
            currentEvent.Add(EventCommandEnum.HEAL.Value);
            currentEvent.Add(0x40);
            // ??
            currentEvent.Add(EventCommandEnum.HEAL.Value);
            currentEvent.Add(0x84);

            // status?
            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x01);
            currentEvent.Add(0x90);
            currentEvent.Add(0x00);
            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x01);
            currentEvent.Add(0x91);
            currentEvent.Add(0x00);

            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x02);
            currentEvent.Add(0x90);
            currentEvent.Add(0x00);
            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x02);
            currentEvent.Add(0x91);
            currentEvent.Add(0x00);

            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x03);
            currentEvent.Add(0x90);
            currentEvent.Add(0x00);
            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x03);
            currentEvent.Add(0x91);
            currentEvent.Add(0x00);

            // recovery sound effect
            currentEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
            currentEvent.Add(0x02);
            currentEvent.Add(0x07);
            currentEvent.Add(0x00);
            currentEvent.Add(0x88);

            // update display
            currentEvent.Add(0x35); // ?

            currentEvent.Add(EventCommandEnum.CLOSE_GP.Value); // close GP

            currentEvent.End();


            // event 0x193: decline hp restore
            eventNum++;
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;

            int cancelEventNum = eventNum;
            currentEvent.Add(EventCommandEnum.CLOSE_GP.Value); // close GP
            currentEvent.End();
            eventNum++;


            // -------------------------------
            // event 0x194-0x1c3: shop events. 16 of: base shop + sell, buy, sell all, sell all confirmation
            // -------------------------------

            // ------------------------------------------
            // sell event, shared by all shop events
            int sellEventNum = eventNum;
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;

            // center camera
            currentEvent.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            currentEvent.Add(0x08);

            // sell
            currentEvent.Add(EventCommandEnum.SELL_MENU.Value); // sell event - 0x0e

            // close GP view for sell
            currentEvent.Add(EventCommandEnum.CLOSE_GP.Value);
            currentEvent.End();
            eventNum++;

            List<int> shopEventIds = new List<int>();
            for (int i = 0; i < 16; i++)
            {
                // ------------------------------------------
                // event 0x194, 0x198, ... base shop + sell
                shopEventIds.Add(eventNum);
                currentEvent = new EventScript();
                context.replacementEvents[eventNum] = currentEvent;
                // gp view
                currentEvent.Add(EventCommandEnum.OPEN_GP.Value);
                currentEvent.Add(0x5F);
                // event+1 for buy, event+2 for sell all, or sell event num from above for sell
                currentEvent.AddDialogueBoxWithChoices("Shop? Meow.\n",
                    new byte[] { 0x2, 0x8, 0x13 }, new string[] { "Buy", "Sell all", "Sell" },
                    new byte[][] { EventScript.GetJumpCmd(eventNum + 1), EventScript.GetJumpCmd(eventNum + 2), EventScript.GetJumpCmd(sellEventNum) },
                    EventScript.GetJumpCmd(cancelEventNum));
                // end event
                currentEvent.End();
                eventNum++;


                // ------------------------------------------
                // buy event
                // event 0x195, 0x199, ... buy
                currentEvent = new EventScript();
                context.replacementEvents[eventNum] = currentEvent;
                // center camera
                currentEvent.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
                currentEvent.Add(0x08);
                // buy menu
                currentEvent.Add(EventCommandEnum.BUY_MENU.Value);
                currentEvent.Add((byte)i);
                // close GP view for buy
                currentEvent.Add(EventCommandEnum.CLOSE_GP.Value);
                // end event
                currentEvent.End();
                eventNum++;


                // ------------------------------------------
                // sell all event
                // event 0x196, 0x19a, ... sell all selection
                currentEvent = new EventScript();
                context.replacementEvents[eventNum] = currentEvent;
                // sell all confirmation
                // eventnum + 1 for yes, cancelevent for no/cancel
                currentEvent.AddDialogueBoxWithChoices("Sell all unequipped gear?\n", new byte[] { 2, 8 }, new string[] { "Yep", "Nope" },
                    new byte[][] { EventScript.GetJumpCmd(eventNum + 1), EventScript.GetJumpCmd(cancelEventNum) }, EventScript.GetJumpCmd(cancelEventNum));
                currentEvent.End();
                eventNum++;


                // ------------------------------------------
                // sell all confirmation event
                // event 0x197, 0x19b, ... sell all confirmation
                currentEvent = new EventScript();
                context.replacementEvents[eventNum] = currentEvent;
                // sell all
                currentEvent.Add(EventCommandEnum.CUSTOM_EVENT_COMMANDS.Value);
                currentEvent.Add(context.eventHackMgr.getCommandIndex(SellAllGear.EVENT_COMMAND_NAME_SELL_ALL));
                // close dialogue
                currentEvent.CloseDialogueBox();
                // close GP view
                currentEvent.Add(EventCommandEnum.CLOSE_GP.Value);
                // end event
                currentEvent.End();
                eventNum++;
            }

            // -------------------------------
            // event 0x1c4+: boss events. number varies by how many bosses we actually placed. 2 per boss - entry and death
            // -------------------------------
            context.workingData.setInt(BOSS_EVENT_TOTAL, bossEvents.Count);
            int floorEventIndex = 0;
            // boss events
            foreach (int floorNum in bossEvents.Keys)
            {
                // one event for going to boss - music change, enable a boss appearance flag, and a door placeholder that will get filled in later
                // second event for killing boss - grant a random orb, clear the boss visibility flag, and another placeholder door

                // -------------------------------
                // boss entry event
                currentEvent = new EventScript();
                context.replacementEvents[eventNum] = currentEvent;
                context.workingData.setInt(BOSS_EVENT_FLOORNUM + floorEventIndex, floorNum);
                bossEvents[floorNum].Add(eventNum);
                context.workingData.setInt(BOSS_ENTRY_EVENT_EVENTNUM + floorEventIndex, eventNum);
                // disable custom music
                currentEvent.SetFlag(EventFlags.PROCGEN_MODE_CUSTOM_MUSIC_FLAG, 0);
                // play boss theme (copied from event 704)
                currentEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
                currentEvent.Add(0x01);
                currentEvent.Add(0x04);
                currentEvent.Add(0x04);
                currentEvent.Add(0xFF);
                // set event flag for visibility
                currentEvent.SetFlag(EventFlags.PROCGEN_MODE_BOSS_VISIBILITY_FLAG, 1);
                // placeholder for doorway - event cmd 0x18 has been redesigned to take a 16 bit param for doorway so we can ref more than 0x400 of them
                currentEvent.AddRange(UNKNOWN_DOOR_REPLACEMENT_PATTERN);
                // end event
                currentEvent.End();
                eventNum++;


                // -------------------------------
                // boss death event
                currentEvent = new EventScript();
                context.replacementEvents[eventNum] = currentEvent;
                bossEvents[floorNum].Add(eventNum);
                context.workingData.setInt(BOSS_DEATH_EVENT_EVENTNUM + floorEventIndex, eventNum);

                // inc event flag 26 (starts at 1 to make boss appear)
                currentEvent.IncrFlag(EventFlags.PROCGEN_MODE_BOSS_VISIBILITY_FLAG);
                currentEvent.Logic(EventFlags.PROCGEN_MODE_BOSS_VISIBILITY_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(0)); // boss wasn't visible before?
                // otherwise, keep going
                // random orb prize
                // ^ subr 500 - 507
                currentEvent.Jsr(0x500 + (byte)((r.Next() % 8)));
                // set event flag for invisibility
                currentEvent.SetFlag(EventFlags.PROCGEN_MODE_BOSS_VISIBILITY_FLAG, 0);
                // custom music off
                currentEvent.SetFlag(EventFlags.PROCGEN_MODE_CUSTOM_MUSIC_FLAG, 0);
                // placeholder for doorway
                currentEvent.AddRange(UNKNOWN_DOOR_REPLACEMENT_PATTERN);

                // call walkon event for next map, since those seem to break when i do event doorways
                if (walkOnEvents.ContainsKey(floorNum + 1))
                {
                    currentEvent.Jump(walkOnEvents[floorNum + 1]);
                }
                // end event
                currentEvent.End();

                eventNum++;
                floorEventIndex++;
            }

            List<int> prev4Elements = new List<int>();
            prev4Elements.Add(-1);
            prev4Elements.Add(-1);
            prev4Elements.Add(-1);
            prev4Elements.Add(-1);

            for (int i = 0; i < numMaps; i++)
            {
                int shopId = (i * 16) / numMaps;
                List<NpcTaggedEvent> floorEvents = new List<NpcTaggedEvent>();
                events.Add(i, floorEvents);
                int numElements = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.NUM_ELEMENTS);
                if (numElements > 8)
                {
                    numElements = 8;
                }

                // doing it this way makes random.Next always get called the same amount of time and ensures equivalent future rng
                // (for mapgen, mostly) regardless of the number you pick here
                List<int> elementIds = new List<int>();
                for (int j = 0; j < 8; j++)
                {
                    elementIds.Add(r.Next() % 8);
                }

                // // Spell NPCs = 90 gnome, 91 undine, 92 salamando, 93 sylphid, 94 lumina, 95 shade, 96 luna, 97 dryad
                string[] elementNames = new string[] { "Gnome", "Undine", "Salamando", "Sylphid", "Lumina", "Shade", "Luna", "Dryad" };
                for (int j = 0; j < numElements; j++)
                {
                    // elements
                    int elementId = elementIds[j];
                    // reroll once if recent element
                    if (prev4Elements.Contains(elementId))
                    {
                        elementId = elementIds[(j + 1) % 8];
                    }

                    prev4Elements.RemoveAt(0);
                    prev4Elements.Add(elementId);

                    NpcTaggedEvent elementEvent = new NpcTaggedEvent();
                    elementEvent.npcId = (byte)(0x90 + elementId);
                    elementEvent.eventFlagId = (byte)(0x10 + elementId);
                    elementEvent.eventFlagMin = 0x00;
                    elementEvent.eventFlagMax = 0x00;
                    elementEvent.eventId = elementEvents[elementId];
                    elementEvent.mobile = false;
                    Logging.log("Element " + elementId + " (" + elementNames[elementId] + ") on floor " + (i + 1), "spoiler");
                    floorEvents.Add(elementEvent);
                }

                int numWatts = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.NUM_WATTS);

                for (int j = 0; j < numWatts; j++)
                {
                    NpcTaggedEvent wattsEvent = new NpcTaggedEvent();
                    wattsEvent.npcId = 0x9C;
                    wattsEvent.eventId = wattsEventNum;
                    floorEvents.Add(wattsEvent);
                }

                int numRestores = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.NUM_PHANNAS);

                for (int j = 0; j < numRestores; j++)
                {
                    NpcTaggedEvent restoreEvent = new NpcTaggedEvent();
                    restoreEvent.eventId = restoreEventNum;
                    restoreEvent.npcId = 0xA6; // phanna as heal npc for now
                    floorEvents.Add(restoreEvent);
                }

                NpcTaggedEvent shopEvent = new NpcTaggedEvent();
                shopEvent.eventId = shopEventIds[shopId];
                shopEvent.npcId = 0x99; // neko
                floorEvents.Add(shopEvent);

                List<int> npcEventIds = new List<int>();
                List<int> npcEventNpcIds = new List<int>();
                foreach (NpcTaggedEvent npcEvent in floorEvents)
                {
                    npcEventIds.Add(npcEvent.eventId);
                    npcEventNpcIds.Add(npcEvent.npcId);
                }
                context.workingData.setIntArray(USEFUL_EVENTS_EVENTID + i, npcEventIds.ToArray());
                context.workingData.setIntArray(USEFUL_EVENTS_NPCID + i, npcEventNpcIds.ToArray());
            }

            // -------------------------------
            // replacement death event -> allow continues
            // -------------------------------
            int newDeathEvent = eventNum;
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;

            // if mana beast flag set, warp to a different map (just use door ffff?), then unset it (flag 4e)
            currentEvent.Logic(EventFlags.MANAFORT_SWITCHES_FLAG_1, 0x8, 0x8, EventScript.GetJsrCmd(eventNum + 4));

            ///////////////////////////////////////////////////////

            // music
            currentEvent.Jsr(0x7C7);

            // fade out
            currentEvent.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            currentEvent.Add(0x06);
            currentEvent.Add(0xFF);
            currentEvent.Add(0xFF);

            // gp view
            currentEvent.Add(EventCommandEnum.OPEN_GP.Value);
            currentEvent.Add(0x5F);

            // eventNum+1 for yes, eventNum+2 to cancel/no
            int continueCost = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.CONTINUE_COST);
            currentEvent.AddDialogueBoxWithChoices("You died!\nContinue for " + continueCost + " GP?\n",
                new byte[] { 2, 8 }, new string[] { "Yep", "Nope" }, new byte[][] { EventScript.GetJumpCmd(eventNum + 1), EventScript.GetJumpCmd(eventNum + 2) }, EventScript.GetJumpCmd(eventNum + 2));
            currentEvent.End();
            eventNum++;


            // -------------------------------
            // continue: yes
            // -------------------------------
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;
            // try to remove the money
            currentEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
            currentEvent.Add(EventCommandEnum.REMOVE_GOLD.Value);
            currentEvent.Add((byte)continueCost);
            currentEvent.Add((byte)(continueCost >> 8));
            currentEvent.Add(0x5F); // refresh gp display, i think
            // check if we had the money
            int noMoneyEvent = eventNum + 2;
            currentEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0xF, EventScript.GetJumpCmd(noMoneyEvent));
            // close GP view for buy
            currentEvent.Add(EventCommandEnum.CLOSE_GP.Value);
            // all hp
            currentEvent.Add(EventCommandEnum.HEAL.Value);
            currentEvent.Add(0x04);
            // all mp?
            currentEvent.Add(EventCommandEnum.HEAL.Value);
            currentEvent.Add(0x40);
            // ??
            currentEvent.Add(EventCommandEnum.HEAL.Value);
            currentEvent.Add(0x84);
            // status?
            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x01);
            currentEvent.Add(0x90);
            currentEvent.Add(0x00);
            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x01);
            currentEvent.Add(0x91);
            currentEvent.Add(0x00);

            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x02);
            currentEvent.Add(0x90);
            currentEvent.Add(0x00);
            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x02);
            currentEvent.Add(0x91);
            currentEvent.Add(0x00);

            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x03);
            currentEvent.Add(0x90);
            currentEvent.Add(0x00);
            currentEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            currentEvent.Add(0x03);
            currentEvent.Add(0x91);
            currentEvent.Add(0x00);

            // recovery sound effect
            currentEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
            currentEvent.Add(0x02);
            currentEvent.Add(0x07);
            currentEvent.Add(0x00);
            currentEvent.Add(0x88);

            // execute continue door
            currentEvent.ExtendedDoor(EventScript.EXTENDED_DOOR_CONTINUE);

            // end event
            currentEvent.End();
            eventNum++;


            // -------------------------------
            // continue: no/cancel
            // -------------------------------
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;
            currentEvent.AddDialogueBox("Ok game over then.");
            // reset
            currentEvent.Add(0x1F);
            currentEvent.Add(0x07);
            // end event
            currentEvent.End();
            eventNum++;


            // -------------------------------
            // continue: no because no money
            // -------------------------------
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;
            currentEvent.AddDialogueBox("Not enough money!\nGame over.");
            // reset
            currentEvent.Add(0x1F);
            currentEvent.Add(0x07);
            // end event
            currentEvent.End();


            // -------------------------------
            // mana beast map escape so we can show dialog windows
            // -------------------------------
            eventNum++;
            currentEvent = new EventScript();
            context.replacementEvents[eventNum] = currentEvent;
            // execute continue door
            currentEvent.ExtendedDoor(EventScript.EXTENDED_DOOR_CONTINUE);
            // mana beast existence flag
            currentEvent.SetFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1, 0);
            // return sub
            currentEvent.Return();
            // end
            currentEvent.End();

            // 0x19459 to change the event num that gets executed when you die
            outRom[0x19459] = (byte)newDeathEvent;
            outRom[0x1945A] = (byte)(newDeathEvent >> 8);

            return events;
        }
    }

    public class NpcTaggedEvent
    {
        // data associated with this thing, probably will have more than eventId eventually
        public int eventId; // x100, x100 are intro/manabeast, x280 are useless dialogue, maybe x180 for now?
        public byte npcId = 0; // if zero, use random

        // optional; used to destroy npcs etc
        public byte eventFlagId = 0;
        public byte eventFlagMin = 0;
        public byte eventFlagMax = 0x0f;

        public bool mobile = true;
    }
}
