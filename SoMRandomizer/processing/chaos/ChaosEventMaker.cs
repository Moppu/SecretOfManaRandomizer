using SoMRandomizer.config;
using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.procgen;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.chaos
{
    /// <summary>
    /// Create the necessary events for chaos mode to work.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosEventMaker : RandoProcessor
    {
        public const string SHOP_EVENT_ID_PREFIX = "shopEventId";
        public const int NUM_SHOPS = 16;

        public const string BOSS_ENTRY_EVENT_ID_PREFIX = "bossEntryEventId";
        public const string BOSS_ENTRY_EVENT_NUM = "bossEntryEventNum";

        public const string BOSS_DEATH_EVENT_ID_PREFIX = "bossDeathEventId";
        public const string BOSS_DEATH_EVENT_NUM = "bossDeathEventNum";

        public const string MANABEAST_WALKON_EVENT = "manaBeastWalkonEventNum";
        public const string MANABEAST_DEATH_EVENT = "manaBeastDeathEventNum";

        public const string INITIAL_WALKON_EVENT = "initialWalkonEventNum";

        public const string ELEMENT_EVENT_ID_PREFIX = "elementEventId";

        public const string WATTS_EVENT = "wattsEventNum";

        public const string SEED_EVENT_ID_PREFIX = "seedEventId";

        // pattern to use here to indicate where further processing should inject a door command to enter a boss room
        public static byte[] BOSS_DOOR_INJECTION_PATTERN = new byte[] { 0xee, 0xee, 0xee };

        // pattern to use here to indicate where further processing should inject a door command to enter the next floor
        public static byte[] NEXT_FLOOR_DOOR_INJECTION_PATTERN = new byte[] { 0xff, 0xff, 0xff };

        protected override string getName()
        {
            return "Chaos mode event generator";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            int[] seedOldEvents = new int[] { 0x588, 0x58b, 0x58a, 0x589, 0x58c, 0x58d, 0x58e, 0x58f };
            Random r = context.randomFunctional;

            int eventNum = 0x100;
            Logging.log("Intro event = " + eventNum.ToString("X4"), "debug");
            context.workingData.setInt(INITIAL_WALKON_EVENT, eventNum);
            EventScript introEvent = new EventScript();
            context.replacementEvents[eventNum++] = introEvent;
            // skip out if 00 is 1+
            introEvent.Logic(EventFlags.UI_DISPLAY_FLAG, 0x1, 0xF, EventScript.GetJumpCmd(0));
            // ice castle theme
            introEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
            introEvent.Add(0x01);
            introEvent.Add(0x15);
            introEvent.Add(0x12);
            introEvent.Add(0x8F);

            // enable UI
            introEvent.IncrFlag(EventFlags.UI_DISPLAY_FLAG);
            introEvent.IncrFlag(EventFlags.UI_DISPLAY_FLAG);

            // flag 0x0B -> 0x05 for normal death
            introEvent.SetFlag(EventFlags.DEATH_TYPE_FLAG, 5);

            // you start with sword after bumping event flag 0x00 i think
            introEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add glove
            introEvent.Add(0x80);
            introEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add axe
            introEvent.Add(0x92);
            introEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add spear
            introEvent.Add(0x9B);
            introEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add whip
            introEvent.Add(0xA4);
            introEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add bow
            introEvent.Add(0xAD);
            introEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add boomerang
            introEvent.Add(0xB6);
            introEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add javelin
            introEvent.Add(0xBF);

            // add gold
            int startingGold = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.STARTING_GOLD);
            if (startingGold > 65535)
            {
                startingGold = 65535;
            }
            if (startingGold > 0)
            {
                introEvent.Add(EventCommandEnum.ADD_GOLD.Value);
                introEvent.Add((byte)startingGold);
                introEvent.Add((byte)(startingGold >> 8));
            }

            // name girl/sprite
            if(settings.getBool(ChaosSettings.PROPERTYNAME_INCLUDE_GIRL_CHARACTER))
            {
                introEvent.Add(EventCommandEnum.NAME_CHARACTER.Value);
                introEvent.Add(0x01);
            }
            if(settings.getBool(ChaosSettings.PROPERTYNAME_INCLUDE_SPRITE_CHARACTER))
            {
                introEvent.Add(EventCommandEnum.NAME_CHARACTER.Value);
                introEvent.Add(0x02);
            }

            // Add p2, p3
            if (settings.getBool(ChaosSettings.PROPERTYNAME_INCLUDE_GIRL_CHARACTER))
            {
                introEvent.Add(EventCommandEnum.ADD_CHARACTER.Value);
                introEvent.Add(0x01);
            }
            if (settings.getBool(ChaosSettings.PROPERTYNAME_INCLUDE_SPRITE_CHARACTER))
            {
                introEvent.Add(EventCommandEnum.ADD_CHARACTER.Value);
                introEvent.Add(0x02);
            }

            int startingConsumables = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.STARTING_CONSUMABLES);
            // add items
            // candy, chocolate, jam, herb, walnut, cup
            byte[] startingItems = new byte[] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45 };
            foreach (byte startingItem in startingItems)
            {
                for (int i = 0; i < startingConsumables; i++)
                {
                    introEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    introEvent.Add(startingItem);
                }
            }
            // flammie drum for music changing
            introEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
            introEvent.Add(0x47);
            // restore everyone's hp
            introEvent.Add(EventCommandEnum.HEAL.Value);
            introEvent.Add(0x00);
            double manaPowerBase = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.MANA_POWER + "_base");
            double manaPowerMul = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.MANA_POWER + "_growth");
            double manaPowerExp = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.MANA_POWER + "_exp");
            // mana power at starting value
            introEvent.SetFlag(EventFlags.TOTAL_MANA_POWER_FLAG, (byte)manaPowerBase);
            for (int i = 0; i < (int)manaPowerBase; i++)
            {
                // add seeds up to mana power
                introEvent.SetFlag((byte)(0x90 + i), 1);
            }
            introEvent.AddDialogueBox(
                "Chaos mode should work\n" +
               "a little better than\n" +
               "Ancient Cave, but may\n" +
               "still have some major\n" +
               "stat balancing issues.\n" +
               "Enjoy!\n");
            // remove boy character if applicable
            if (!settings.getBool(ChaosSettings.PROPERTYNAME_INCLUDE_BOY_CHARACTER))
            {
                introEvent.Add(EventCommandEnum.REMOVE_CHARACTER.Value);
                introEvent.Add(0x00);
            }
            // set timer flag on
            introEvent.SetFlag(EventFlags.PROCGEN_MODE_TIMER_RUNNING_FLAG, 1);
            // end intro event
            introEvent.End();


            // Watts event
            Logging.log("Watts event = " + eventNum.ToString("X4"), "debug");
            context.workingData.setInt(WATTS_EVENT, eventNum);
            EventScript wattsEvent = new EventScript();
            context.replacementEvents[eventNum++] = wattsEvent;

            // GP view
            wattsEvent.Add(EventCommandEnum.OPEN_GP.Value);
            wattsEvent.Add(0x5F);
            wattsEvent.AddDialogueBox("WATTS:Okay!\n Which one's ready?");
            // open the watts menu
            wattsEvent.Add(EventCommandEnum.WEAPON_UPGRADE_MENU.Value);
            // hide GP
            wattsEvent.Add(EventCommandEnum.CLOSE_GP.Value);
            // end event
            wattsEvent.End();


            int cancelEventNum = eventNum;
            List<byte> cancelEvent = new List<byte>();
            context.replacementEvents[eventNum++] = cancelEvent;
            cancelEvent.Add(0x5E); // close GP
            cancelEvent.Add(0x00); // end

            /*List<byte> dialogueShop = VanillaEventUtil.getBytes("Shop? Meow.\n");
            List<byte> dialogueBuy = VanillaEventUtil.getBytes("Buy");
            List<byte> dialogueSell = VanillaEventUtil.getBytes("Sell");
            List<byte> dialogueSellAll = VanillaEventUtil.getBytes("Sell all");

            List<byte> dialogueConfirm = VanillaEventUtil.getBytes("Sell all unequipped gear?\n");
            List<byte> dialogueConfirmYep = VanillaEventUtil.getBytes("Yep");
            List<byte> dialogueConfirmNope = VanillaEventUtil.getBytes("Nope");*/


            int sellEventNum = eventNum;
            EventScript sellEvent = new EventScript();
            context.replacementEvents[eventNum++] = sellEvent;
            // center camera
            sellEvent.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            sellEvent.Add(0x08);
            // sell
            sellEvent.Add(EventCommandEnum.SELL_MENU.Value); // sell event - 0x0e
            // close GP view for sell
            sellEvent.Add(EventCommandEnum.CLOSE_GP.Value);
            sellEvent.End();
            
            for (int i = 0; i < NUM_SHOPS; i++)
            {
                Logging.log("Shop " + i + " event = " + eventNum.ToString("X4"), "debug");
                context.workingData.setInt(SHOP_EVENT_ID_PREFIX + i, eventNum);
                EventScript shopEvent = new EventScript();
                context.replacementEvents[eventNum++] = shopEvent;

                // gp view
                shopEvent.Add(EventCommandEnum.OPEN_GP.Value);
                shopEvent.Add(0x5F);
                // next event to buy, the one after to sell all, and the one above to sell
                shopEvent.AddDialogueBoxWithChoices("Shop? Meow.\n",
                    new byte[] { 2, 8, 0x13 }, new string[] { "Buy", "Sell all", "Sell" }, 
                    new byte[][] { EventScript.GetJumpCmd(eventNum), EventScript.GetJumpCmd(eventNum + 1), EventScript.GetJumpCmd(sellEventNum) }, EventScript.GetJumpCmd(cancelEventNum));
                // end event
                shopEvent.End();


                // buy event
                EventScript buyEvent = new EventScript();
                context.replacementEvents[eventNum++] = buyEvent;
                // center camera
                buyEvent.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
                buyEvent.Add(0x08);
                // buy menu
                buyEvent.Add(EventCommandEnum.BUY_MENU.Value);
                buyEvent.Add((byte)i);
                // close GP view for buy
                buyEvent.Add(EventCommandEnum.CLOSE_GP.Value);
                // end event
                buyEvent.End();


                // sell all confirmation event
                EventScript sellAllConfirmationEvent = new EventScript();
                context.replacementEvents[eventNum++] = sellAllConfirmationEvent;
                // sell all confirmation
                sellAllConfirmationEvent.AddDialogueBoxWithChoices("Sell all unequipped gear?\n",
                    new byte[] { 2, 8 }, new string[] { "Yep", "Nope" }, new byte[][] { EventScript.GetJumpCmd(eventNum), EventScript.GetJumpCmd(cancelEventNum) }, EventScript.GetJumpCmd(cancelEventNum));
                // end
                sellAllConfirmationEvent.End();


                // sellall event
                EventScript sellAllEvent = new EventScript();
                context.replacementEvents[eventNum++] = sellAllEvent;
                // sell all
                sellAllEvent.Add(EventCommandEnum.CUSTOM_EVENT_COMMANDS.Value);
                sellAllEvent.Add(context.eventHackMgr.getCommandIndex(SellAllGear.EVENT_COMMAND_NAME_SELL_ALL));
                // close dialogue
                sellAllEvent.CloseDialogueBox();
                // close GP view
                sellAllEvent.Add(EventCommandEnum.CLOSE_GP.Value);
                // end event
                sellAllEvent.End();
            }

            // 4 is lumina 5 is shade
            byte[] elementAttributeFlags = new byte[] { 0xC8, 0xC9, 0xCA, 0xCB, 0xCF, 0xCE, 0xCC, 0xCD };

            List<int> elementEvents = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                // make element events
                // flag 0x10 - 0x17
                // gnome c8
                // undine c9
                // salamando ca
                // sylphid cb
                // luna cc
                // dryad cd
                // shade ce
                // lumina cf
                Logging.log("Element " + i + " event = " + eventNum.ToString("X4"), "debug");
                elementEvents.Add(eventNum);
                context.workingData.setInt(ELEMENT_EVENT_ID_PREFIX + i, eventNum);
                EventScript elementEvent = new EventScript();
                context.replacementEvents[eventNum++] = elementEvent;
                // event flag check - skip this and tell me to go away (next event)
                elementEvent.Logic((byte)(0x10 + i), 0x1, 0xF, EventScript.GetJumpCmd(eventNum));
                // event flag increment
                elementEvent.IncrFlag((byte)(0x10 + i));

                // girl
                if (i != 5) // shade
                {
                    elementEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    elementEvent.Add(0x02);
                    elementEvent.Add(elementAttributeFlags[i]);
                    if (i == 7)
                    {
                        // no mana magic
                        elementEvent.Add(0x18);
                    }
                    else
                    {
                        elementEvent.Add(0x38);
                    }
                }

                // sprite
                if (i != 4) // lumina
                {
                    elementEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    elementEvent.Add(0x03);
                    elementEvent.Add(elementAttributeFlags[i]);
                    if (i == 7)
                    {
                        // no mana magic
                        elementEvent.Add(0x03);
                    }
                    else
                    {
                        elementEvent.Add(0x07);
                    }
                }
                elementEvent.AddDialogueBox("Here, have some magic.");
                elementEvent.End();


                // next event: already got it
                EventScript elementGoAwayEvent = new EventScript();
                context.replacementEvents[eventNum++] = elementGoAwayEvent;
                elementGoAwayEvent.AddDialogueBox("Ok go away now.");
                // end
                elementGoAwayEvent.End();
            }


            // make up to n boss events
            // these should have a spot to inject a door for us to execute in the case the boss is already
            // dead, or to execute after it dies
            // use event flag 0x19+, 1->F; 1A, 1->F, etc for event flags
            // since they should always be killed in order, this should work fine
            int bossEventFlag = 0x30;
            int bossEventValue = 1;
            int maxBosses = 25;
            context.workingData.setInt(BOSS_ENTRY_EVENT_NUM, maxBosses);
            context.workingData.setInt(BOSS_DEATH_EVENT_NUM, maxBosses);
            for (int i=0; i <= maxBosses; i++)
            {
                Logging.log("Boss " + i + " entry/walkon event (with EEEEEE) = " + eventNum.ToString("X4"), "debug");
                // main transition event, to boss or next floor (if boss is dead)
                if (i < maxBosses)
                {
                    context.workingData.setInt(BOSS_ENTRY_EVENT_ID_PREFIX + i, eventNum);
                }
                else
                {
                    context.workingData.setInt(MANABEAST_WALKON_EVENT, eventNum);
                }
                EventScript bossEntryEvent = new EventScript();
                context.replacementEvents[eventNum++] = bossEntryEvent;
                // event flag check - skip boss if it's already dead
                // skip if event flag > bossEventValue
                bossEntryEvent.Logic((byte)(bossEventFlag), (byte)(bossEventValue + 1), 0xF, EventScript.GetJumpCmd(eventNum));
                // if we didn't skip, then take the door to the boss arena
                bossEntryEvent.AddRange(BOSS_DOOR_INJECTION_PATTERN);
                // disable custom music
                bossEntryEvent.SetFlag(EventFlags.PROCGEN_MODE_CUSTOM_MUSIC_FLAG, 0);
                // boss music
                // play boss theme (copied from event 704)
                if (i == maxBosses)
                {
                    // play the mana beast theme
                    bossEntryEvent.Jsr(0x739);
                }
                else
                {
                    bossEntryEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
                    bossEntryEvent.Add(0x01);
                    bossEntryEvent.Add(0x04);
                    bossEntryEvent.Add(0x04);
                    bossEntryEvent.Add(0xFF);
                }
                bossEntryEvent.End();


                // door/skip
                // next event: take the door to the next floor
                Logging.log("Boss " + i + " skip event (with FFFFFF) = " + eventNum.ToString("X4"), "debug");
                EventScript bossSkipEvent = new EventScript();
                int bossSkipEventNum = eventNum;
                context.replacementEvents[eventNum++] = bossSkipEvent;
                // door to next floor, to be filled in later
                bossSkipEvent.AddRange(NEXT_FLOOR_DOOR_INJECTION_PATTERN);
                // end
                bossSkipEvent.End();


                Logging.log("Boss " + i + " death event = " + eventNum.ToString("X4"), "debug");
                // boss death event
                if (i < maxBosses)
                {
                    context.workingData.setInt(BOSS_DEATH_EVENT_ID_PREFIX + i, eventNum);
                }
                EventScript bossDeathEvent = new EventScript();
                context.replacementEvents[eventNum++] = bossDeathEvent;
                // random orb prize
                // ^ subr 500 - 507
                bossDeathEvent.Jsr(0x500 + (byte)((r.Next() % 8)));
                // custom music off
                bossDeathEvent.SetFlag(EventFlags.PROCGEN_MODE_CUSTOM_MUSIC_FLAG, 0);
                // set the boss death event flag on so we skip this map next time
                bossDeathEvent.SetFlag((byte)bossEventFlag, (byte)(bossEventValue + 1));
                // ice castle theme
                bossDeathEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
                bossDeathEvent.Add(0x01);
                bossDeathEvent.Add(0x15);
                bossDeathEvent.Add(0x12);
                bossDeathEvent.Add(0x8F);
                // jump to the prev event, which takes the normal doorway
                bossDeathEvent.Jump(bossSkipEventNum);
                // end
                bossDeathEvent.End();

                // increment the thingies
                bossEventValue++;
                if(bossEventValue == 0x0F)
                {
                    bossEventValue = 0x01;
                    bossEventFlag++;
                }
            }

            Logging.log("Mana beast death event (with FFFFFF) = " + eventNum.ToString("X4"), "debug");
            context.workingData.setInt(MANABEAST_DEATH_EVENT, eventNum);
            EventScript manaBeastDeathEvent = new EventScript();
            context.replacementEvents[eventNum++] = manaBeastDeathEvent;
            // event flags
            manaBeastDeathEvent.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            manaBeastDeathEvent.IncrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);
            // set timer flag off
            manaBeastDeathEvent.SetFlag(EventFlags.PROCGEN_MODE_TIMER_RUNNING_FLAG, 0);
            // placeholder for door to ending map - 180
            manaBeastDeathEvent.AddRange(NEXT_FLOOR_DOOR_INJECTION_PATTERN);
            // ending music
            manaBeastDeathEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
            manaBeastDeathEvent.Add(0x01);
            manaBeastDeathEvent.Add(0x3a);
            manaBeastDeathEvent.Add(0x1b);
            manaBeastDeathEvent.Add(0x8f);
            manaBeastDeathEvent.OpenDialogueBox();
            manaBeastDeathEvent.AddDialogue("Hooray! You did it.\nTime: ");
            manaBeastDeathEvent.Add(EventCommandEnum.CUSTOM_EVENT_COMMANDS.Value); // print timer value - see TimerDialogue for where i add this command
            manaBeastDeathEvent.Add(context.eventHackMgr.getCommandIndex(TimerDialogue.EVENT_COMMAND_NAME_PRINT_TIMER));
            manaBeastDeathEvent.AddDialogue("\nThe end!");
            manaBeastDeathEvent.CloseDialogueBox(); // close dialogue
            manaBeastDeathEvent.Add(0x1f); // end game
            manaBeastDeathEvent.Add(0x11);
            // end of event
            manaBeastDeathEvent.End();

            // seed event fix; event 588-58f
            for (int i = 0; i < 8; i++)
            {
                Logging.log("Seed " + i + " event = " + eventNum.ToString("X4"), "debug");
                // add missing mana level increment (25 94 to JSR to event 594) into events 588-58f as needed
                // after the existing 25 90 to JSR to event 590.  this resolves an inconsistency in these events
                // where not all of them increment your total mana power.
                context.workingData.setInt(SEED_EVENT_ID_PREFIX + i, eventNum);
                // generate a copy of the existing event with the 25 94 added in as needed
                List<byte> copiedEventData = VanillaEventUtil.getVanillaEvent(origRom, seedOldEvents[i]);
                List<byte> newEventData = new List<byte>();
                context.replacementEvents[eventNum++] = newEventData;
                for (int o = 0; o < copiedEventData.Count; o++)
                {
                    newEventData.Add(copiedEventData[o]);
                    // add in missing mana level increment
                    if (o > 0 && o < copiedEventData.Count - 2 && copiedEventData[o - 1] == 0x25 && copiedEventData[o] == 0x90 &&
                            (copiedEventData[o + 1] != 0x25 || copiedEventData[o + 2] != 0x94))
                    {
                        newEventData.Add(0x25);
                        newEventData.Add(0x94);
                    }
                }
            }

            return true;
        }
    }
}
