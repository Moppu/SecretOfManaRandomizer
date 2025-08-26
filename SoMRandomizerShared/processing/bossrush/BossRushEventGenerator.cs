using SoMRandomizer.config;
using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.mapgen;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.procgen;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.common.VanillaBossMaps;

namespace SoMRandomizer.processing.bossrush
{
    /// <summary>
    /// Create the necessary events for boss rush mode to work.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BossRushEventGenerator : RandoProcessor
    {
        // entry to inbetween/reward map
        public const string RESTMAP_WALKON_EVENT_ID_PREFIX = "restMapEntryEventId";

        // boss death
        public const string BOSS_DEATH_EVENT_ID_PREFIX = "bossDeathEventId";

        // event for confirmation of attempting the next boss
        public const string NEXT_FLOOR_EVENT_ID_PREFIX = "nextFloorEventId";

        public const string PHANNA_EVENT_NUM = "phannaEventNum";
        public const string WATTS_EVENT_NUM = "wattsEventNum";
        public const string NEKO_EVENT_NUM = "nekoEventNum";

        // 1E + this to give one
        private static Dictionary<byte, string> consumables = new Dictionary<byte, string>();

        private static string[] gearNames = new string[]
        {
                    "",
                    "Bandanna",
                    "Hair Ribbon",
                    "Rabite Cap",
                    "Head Gear",
                    "Quill Cap",
                    "Steel Cap",
                    "Golden Tiara",
                    "Raccoon Cap",
                    "Quilted Hood",
                    "Tiger Cap",
                    "Circlet",
                    "Ruby Armet",
                    "Unicorn Helm",
                    "Dragon Helm",
                    "Duck Helm",
                    "Needle Helm",
                    "Cockatrice Cap",
                    "Amulet Helm",
                    "Griffin Helm",
                    "Faerie Crown",
                    "",
                    "Overalls",
                    "Kung Fu Suit",
                    "Midge Robe",
                    "Chain Vest",
                    "Spiky Suit",
                    "Kung Fu Dress",
                    "Fancy Overalls",
                    "Chest Guard",
                    "Golden Vest",
                    "Ruby Vest",
                    "Tiger Suit",
                    "Tiger Bikini",
                    "Magical Armor",
                    "Tortoise Mail",
                    "Flower Suit",
                    "Battle Suit",
                    "Vestguard",
                    "Vampire Cape",
                    "Power Suit",
                    "Faerie Cloak",
                    "",
                    "Faerie's Ring",
                    "Elbow Pad",
                    "Power Wrist",
                    "Cobra Bracelet",
                    "Wolf's Band",
                    "Silver Band",
                    "Golem Ring",
                    "Frosty Ring",
                    "Ivy Amulet",
                    "Gold Bracelet",
                    "Shield Ring",
                    "Lazuri Ring",
                    "Guardian Ring",
                    "Gauntlet",
                    "Ninja Gloves",
                    "Dragon Ring",
                    "Watcher Ring",
                    "Imp's Ring",
                    "Amulet Ring",
                    "Wristband"
        };

        private static string[] orbNames = new string[] {
                    "Glove",
                    "Sword",
                    "Axe",
                    "Spear",
                    "Whip",
                    "Bow",
                    "Boomerang",
                    "Javelin",
        };

        protected override string getName()
        {
            return "Event generator for boss rush mode";
        }

        public static Dictionary<int, string> bossNames = new Dictionary<int, string>();

        // copied from DropChanges for now.
        private static byte[] hatIds = new byte[64];
        private static byte[] armorIds = new byte[64];
        private static byte[] accessoryIds = new byte[64];

        static BossRushEventGenerator()
        {
            bossNames.Add(0x57, "Mantis Ant");
            bossNames.Add(0x58, "Wall Face");
            bossNames.Add(0x59, "Tropicallo");
            bossNames.Add(0x5A, "Minotaur");
            bossNames.Add(0x5B, "Spikey Tiger");
            bossNames.Add(0x5C, "Jabberwocky");
            bossNames.Add(0x5D, "Spring Beak");
            bossNames.Add(0x5E, "Frost Gigas");
            bossNames.Add(0x5F, "Snap Dragon");
            bossNames.Add(0x60, "Mech Rider I");
            bossNames.Add(0x61, "Doom's Wall");
            bossNames.Add(0x62, "Vampire");
            bossNames.Add(0x63, "Metal Mantis");
            bossNames.Add(0x64, "Mech Rider II");
            bossNames.Add(0x65, "Kilroy");
            bossNames.Add(0x66, "Gorgon Bull");
            bossNames.Add(0x68, "Boreal Face");
            bossNames.Add(0x69, "Great Viper");
            bossNames.Add(0x6A, "Lime Slime");
            bossNames.Add(0x6B, "Blue Spike");
            bossNames.Add(0x6D, "Hydra");
            bossNames.Add(0x6E, "Aegagropilon");
            bossNames.Add(0x6F, "Hexas");
            bossNames.Add(0x70, "Kettle Kin");
            bossNames.Add(0x71, "Tonpole");
            bossNames.Add(VanillaBossMap.TRIPLE_TONPOLE_OBJECT_INDICATOR, "Three Tonpoles");
            bossNames.Add(0x72, "Mech Rider III");
            bossNames.Add(0x73, "Snow Dragon");
            bossNames.Add(0x74, "Fire Gigas");
            bossNames.Add(0x75, "Red Dragon");
            bossNames.Add(0x76, "Axe Beak");
            bossNames.Add(0x77, "Blue Dragon");
            bossNames.Add(0x78, "Buffy");
            bossNames.Add(0x79, "Dark Lich");
            bossNames.Add(0x7B, "Dragon Worm");
            bossNames.Add(0x7C, "Dread Slime");
            bossNames.Add(0x7D, "Thunder Gigas");
            bossNames.Add(0x7F, "Mana Beast");

            consumables.Add(0x40, "Candy");
            consumables.Add(0x41, "Chocolate");
            consumables.Add(0x42, "Royal Jam");
            consumables.Add(0x43, "Faerie Walnut");
            consumables.Add(0x44, "Medical Herb");
            consumables.Add(0x45, "Cup of Wishes");
            consumables.Add(0x4A, "Barrel");

            for (int j = 0; j < 64; j++)
            {
                hatIds[j] = (byte)(0x01 + ((0x14 - 0x01 + 1) * j / 64.0));
                armorIds[j] = (byte)(0x16 + ((0x29 - 0x16 + 1) * j / 64.0));
                accessoryIds[j] = (byte)(0x2b + ((0x3e - 0x2b + 1) * j / 64.0));
                // faerie ring and wrist band wtf
                if (accessoryIds[j] == 0x3E)
                {
                    accessoryIds[j] = 0x2B;
                }
                else if (accessoryIds[j] == 0x2B)
                {
                    accessoryIds[j] = 0x3E;
                }

                // overalls, kung fu suit, midge robe .. never drop since we start with them
                if (armorIds[j] == 0x16 || armorIds[j] == 0x17 || armorIds[j] == 0x18)
                {
                    // chain vest instead
                    armorIds[j] = 0x19;
                }
                if (hatIds[j] == 0x01)
                {
                    // head gear instead
                    hatIds[j] = 0x04;
                }
                if (hatIds[j] == 0x02 || hatIds[j] == 0x03)
                {
                    // quill cap instead
                    hatIds[j] = 0x05;
                }
                if (accessoryIds[j] == 0x3E)
                {
                    // power wrist instead
                    accessoryIds[j] = 0x2D;
                }
            }
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // start index for generated events
            int eventNum = 0x180;

            Random random = context.randomFunctional;

            List<int> bossesInOrder = new List<int>();
            int numMaps = BossOrderRandomizer.allPossibleBosses.Count;
            // mana beast at the end
            for (int i = 0; i <= numMaps; i++)
            {
                bossesInOrder.Add(context.workingData.getInt(BossOrderRandomizer.BOSS_INDEX_PREFIX + i));
            }

            // make intro to first floor event
            context.workingData.setInt(RESTMAP_WALKON_EVENT_ID_PREFIX + "0", eventNum);
            makeStartingEvent(context, settings, ref eventNum);

            // boss events
            int numBosses = bossesInOrder.Count;
            Dictionary<int, int> bossDeathEventNextFloorCallOffsets = new Dictionary<int, int>();

            byte[] numOrbsGiven = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int floorNum = 0; floorNum < numBosses; floorNum++)
            {
                // ----------------- Next floor's rest map walkon event -------------------------
                // floor 0 is made special above in makeStartingEvent
                int nextFloor = floorNum + 1;
                int restMapEntryEvent = eventNum;
                context.workingData.setInt(RESTMAP_WALKON_EVENT_ID_PREFIX + nextFloor, eventNum);
                EventScript floorEntryEvent = new EventScript();
                context.replacementEvents[eventNum++] = floorEntryEvent;
                // music
                floorEntryEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
                floorEntryEvent.Add(0x01);
                floorEntryEvent.Add(0x02);
                floorEntryEvent.Add(0x0d);
                floorEntryEvent.Add(0xff);

                // two orbs
                // two general use items
                // two pieces of gear
                int numOrbs = 2;
                int numUsables = 3;
                int numGearPieces = 2;
                string treasureDialogueString = "Floor " + nextFloor + " Treasures:\n";
                int treasureLines = 1;

                for (int j = 0; j < numOrbs; j++)
                {
                    // increment B8+orb id
                    // glove, sword, axe, spear, whip, bow, boomerang, javelin
                    // only increment if B8+orb id currently 0-7
                    int orbId = random.Next() % 8;
                    int numCurrently = numOrbsGiven[orbId];
                    int rerollAttempts = 0;
                    while (numCurrently >= 8 && rerollAttempts < 1000)
                    {
                        orbId = random.Next() % 8;
                        numCurrently = numOrbsGiven[orbId];
                        rerollAttempts++;
                    }

                    if (rerollAttempts < 1000)
                    {
                        numOrbsGiven[orbId]++;
                        floorEntryEvent.IncrFlag((byte)(0xB8 + orbId));
                        treasureDialogueString += orbNames[orbId] + "'s Orb\n";
                        treasureLines++;
                    }
                }

                for (int j = 0; j < numUsables; j++)
                {
                    int consumableId = random.Next() % consumables.Count;
                    byte key = consumables.Keys.ElementAt(consumableId);
                    floorEntryEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    floorEntryEvent.Add(key);
                    treasureDialogueString += consumables[key] + "\n";
                    treasureLines++;
                }

                int baseIndex = (int)(nextFloor * 64 / 37.0);
                for (int j = 0; j < numGearPieces; j++)
                {
                    // range base-4 -> base+16
                    int index = baseIndex - 4 + (random.Next() % 20);
                    if (index < 0)
                    {
                        index = 0;
                    }
                    if (index > 63)
                    {
                        index = 63;
                    }
                    int gearType = random.Next() % 3;
                    if (gearType == 0)
                    {
                        // hats
                        floorEntryEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                        floorEntryEvent.Add(hatIds[index]);
                        treasureDialogueString += gearNames[hatIds[index]] + "\n";
                        treasureLines++;
                    }
                    else if (gearType == 1)
                    {
                        // armors
                        floorEntryEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                        floorEntryEvent.Add(armorIds[index]);
                        treasureDialogueString += gearNames[armorIds[index]] + "\n";
                        treasureLines++;
                    }
                    else if (gearType == 2)
                    {
                        // accessories
                        floorEntryEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                        floorEntryEvent.Add(accessoryIds[index]);
                        treasureDialogueString += gearNames[accessoryIds[index]] + "\n";
                        treasureLines++;
                    }
                }
                // dialogue box
                floorEntryEvent.AddDialogueBox(treasureDialogueString);
                // end
                floorEntryEvent.End();


                // ----------------- Boss room walkon event -------------------------
                // one event for going to boss - music change, enable a boss appearance flag, and a door placeholder that will get filled in later
                // second event for killing boss - grant a random orb, clear the boss visibility flag, and another placeholder door
                EventScript bossEntryEvent = new EventScript();
                int bossEntryEventNum = eventNum;
                context.replacementEvents[eventNum++] = bossEntryEvent;
                // disable custom music
                bossEntryEvent.SetFlag(EventFlags.PROCGEN_MODE_CUSTOM_MUSIC_FLAG, 0);
                // play boss theme (copied from event 704)
                bossEntryEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
                bossEntryEvent.Add(0x01);
                bossEntryEvent.Add(0x04);
                bossEntryEvent.Add(0x04);
                bossEntryEvent.Add(0xFF);
                // set event flag for visibility
                bossEntryEvent.SetFlag(EventFlags.PROCGEN_MODE_BOSS_VISIBILITY_FLAG, 1);
                // enter boss map
                int entryDoor = BossRushRandomizer.BASE_DOOR_NUM + floorNum * 2 + 1;
                bossEntryEvent.ExtendedDoor(entryDoor);
                // end event
                bossEntryEvent.End();


                // ----------------- Boss death event -------------------------
                context.workingData.setInt(BOSS_DEATH_EVENT_ID_PREFIX + floorNum, eventNum);
                EventScript bossDeathEvent = new EventScript();
                context.replacementEvents[eventNum++] = bossDeathEvent;
                // ----- for triple tonpole, we need to modify this slightly.
                // inc event flag 26 (starts at 1 to make boss appear)
                bossDeathEvent.IncrFlag(EventFlags.PROCGEN_MODE_BOSS_VISIBILITY_FLAG);
                // if event flag 26 is 1, jump to event 00
                bossDeathEvent.Logic(EventFlags.PROCGEN_MODE_BOSS_VISIBILITY_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(0));
                // otherwise, keep going
                // set event flag for invisibility
                bossDeathEvent.SetFlag(EventFlags.PROCGEN_MODE_BOSS_VISIBILITY_FLAG, 0);
                // custom music off
                bossDeathEvent.SetFlag(EventFlags.PROCGEN_MODE_CUSTOM_MUSIC_FLAG, 0);
                // doorway to take to exit boss arena to next floor's rest map
                int bossDeathDoor = BossRushRandomizer.BASE_DOOR_NUM + floorNum * 2 + 2;
                bossDeathEvent.ExtendedDoor(bossDeathDoor);
                // call walkon event for next map, since those seem to break when i do event doorways
                bossDeathEvent.Jump(restMapEntryEvent);
                // end event
                bossDeathEvent.End();
                context.workingData.setInt(NEXT_FLOOR_EVENT_ID_PREFIX + floorNum, eventNum);
                makeNextFloorEvent(context, bossesInOrder, floorNum, bossEntryEventNum, ref eventNum);
            }

            // ----------------------- Neko event ---------------------
            EventScript nekoEvent = new EventScript();
            context.workingData.setInt(NEKO_EVENT_NUM, eventNum);
            context.replacementEvents[eventNum++] = nekoEvent;
            // dialogue box
            nekoEvent.AddDialogueBox("Sell some stuff?\n");
            // center camera
            nekoEvent.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            nekoEvent.Add(0x08);
            // open gp view
            nekoEvent.Add(EventCommandEnum.OPEN_GP.Value);
            nekoEvent.Add(0x5F);
            // sell
            nekoEvent.Add(EventCommandEnum.SELL_MENU.Value);
            // close GP view for sell
            nekoEvent.Add(EventCommandEnum.CLOSE_GP.Value);
            // done
            nekoEvent.End();


            // ----------------------- Watts event ---------------------
            EventScript wattsEvent = new EventScript();
            context.workingData.setInt(WATTS_EVENT_NUM, eventNum);
            context.replacementEvents[eventNum++] = wattsEvent;
            // make the watts event
            // GP view
            wattsEvent.Add(EventCommandEnum.OPEN_GP.Value);
            wattsEvent.Add(0x5F);
            // dialogue box
            wattsEvent.AddDialogueBox("WATTS:Okay!\n Which one's ready?");
            // open the watts menu
            wattsEvent.Add(EventCommandEnum.WEAPON_UPGRADE_MENU.Value);
            // hide GP
            wattsEvent.Add(EventCommandEnum.CLOSE_GP.Value);
            // end event
            wattsEvent.End();


            // ----------------------- Phanna event ---------------------
            EventScript phannaEvent = new EventScript();
            context.workingData.setInt(PHANNA_EVENT_NUM, eventNum);
            context.replacementEvents[eventNum++] = phannaEvent;
            // mark # used with event flag x29
            int numRestores = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.RESTORES_LIMIT);
            if(numRestores > 15)
            {
                numRestores = 15;
            }

            // main event - jump if flag x29 is 1, 2, 3, etc .. continue if it's 0 (none used yet)
            for (int i=1; i <= numRestores; i++)
            {
                // + 1 to skip the yep event; see the loop below it for the different restore number events
                phannaEvent.Logic(EventFlags.BOSS_RUSH_PHANNA_HEALS_FLAG, (byte)i, (byte)i, EventScript.GetJumpCmd(eventNum + i));
            }
            // next event for yes, 0 for no/cancel
            int yepEvent = eventNum;
            phannaEvent.AddDialogueBoxWithChoices("Restore? You have " + numRestores + "\n" + "remaining.\n",
                new byte[] { 2, 8 }, new string[] { "Yep", "Nope" }, new byte[][] { EventScript.GetJumpCmd(yepEvent), EventScript.GetJumpCmd(0) }, EventScript.GetJumpCmd(0));
            // end event
            phannaEvent.End();

            EventScript phannaConfirmEvent = new EventScript();
            context.replacementEvents[eventNum++] = phannaConfirmEvent;
            // now the "yes" event
            // - restore then increment x29 by one
            // all hp
            phannaConfirmEvent.Add(EventCommandEnum.HEAL.Value);
            phannaConfirmEvent.Add(0x04);
            // all mp?
            phannaConfirmEvent.Add(EventCommandEnum.HEAL.Value);
            phannaConfirmEvent.Add(0x40);
            // ??
            phannaConfirmEvent.Add(EventCommandEnum.HEAL.Value);
            phannaConfirmEvent.Add(0x84);
            // status?
            phannaConfirmEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            phannaConfirmEvent.Add(0x01);
            phannaConfirmEvent.Add(0x90);
            phannaConfirmEvent.Add(0x00);
            phannaConfirmEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            phannaConfirmEvent.Add(0x01);
            phannaConfirmEvent.Add(0x91);
            phannaConfirmEvent.Add(0x00);

            phannaConfirmEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            phannaConfirmEvent.Add(0x02);
            phannaConfirmEvent.Add(0x90);
            phannaConfirmEvent.Add(0x00);
            phannaConfirmEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            phannaConfirmEvent.Add(0x02);
            phannaConfirmEvent.Add(0x91);
            phannaConfirmEvent.Add(0x00);

            phannaConfirmEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            phannaConfirmEvent.Add(0x03);
            phannaConfirmEvent.Add(0x90);
            phannaConfirmEvent.Add(0x00);
            phannaConfirmEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            phannaConfirmEvent.Add(0x03);
            phannaConfirmEvent.Add(0x91);
            phannaConfirmEvent.Add(0x00);

            // recovery sound effect
            phannaConfirmEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
            phannaConfirmEvent.Add(0x02);
            phannaConfirmEvent.Add(0x07);
            phannaConfirmEvent.Add(0x00);
            phannaConfirmEvent.Add(0x88);
            // update display
            phannaConfirmEvent.Add(0x35);
            // increment restores used
            phannaConfirmEvent.IncrFlag(EventFlags.BOSS_RUSH_PHANNA_HEALS_FLAG);
            phannaConfirmEvent.End();

            // now make all the sub-events
            for (int i = 1; i <= numRestores; i++)
            {
                EventScript phannaRestoresEvent = new EventScript();
                context.replacementEvents[eventNum++] = phannaRestoresEvent;

                if (i == numRestores)
                {
                    phannaRestoresEvent.AddDialogueBox("No restores left!");
                    // end event
                    phannaRestoresEvent.End();
                }
                else
                {
                    phannaRestoresEvent.AddDialogueBoxWithChoices("Restore? You have " + (numRestores - i) + "\n" + "remaining.\n",
                        new byte[] { 2, 8 }, new string[] { "Yep", "Nope" }, new byte[][] { EventScript.GetJumpCmd(yepEvent), EventScript.GetJumpCmd(0) }, EventScript.GetJumpCmd(0));
                    // end event
                    phannaRestoresEvent.End();

                }
            }

            EventScript manaBeastDeathEvent = new EventScript();
            context.workingData.setInt(ManaBeastMap.MANABEAST_DEATH_EVENT, eventNum);
            context.workingData.setInt(ManaBeastMap.MANABEAST_WALKON_EVENT, -1);
            context.replacementEvents[eventNum++] = manaBeastDeathEvent;
            // event flags
            manaBeastDeathEvent.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            manaBeastDeathEvent.IncrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);
            // set timer flag off
            manaBeastDeathEvent.SetFlag(EventFlags.PROCGEN_MODE_TIMER_RUNNING_FLAG, 0);
            // modified for changes in DoorExpansion.cs
            manaBeastDeathEvent.ExtendedDoor(0x3FF); // door 3FF
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
            manaBeastDeathEvent.AddDialogue("\nThe end. ");
            manaBeastDeathEvent.CloseDialogueBox();
            manaBeastDeathEvent.Add(0x1f); // end game
            manaBeastDeathEvent.Add(0x11);
            // end of event
            manaBeastDeathEvent.End();
            // A978A in event 7F8 - remove restore jump; replace with spaces
            EventScript event7f8;
            if(context.replacementEvents.ContainsKey(0x7F8))
            {
                event7f8 = (EventScript)context.replacementEvents[0x7F8];
            }
            else
            {
                event7f8 = VanillaEventUtil.getVanillaEvent(origRom, 0x7f8);
            }
            for(int i=0; i < event7f8.Count - 1; i++)
            {
                // JSR 0x41B
                if(event7f8[i] == EventCommandEnum.JUMP_SUBR_BASE.Value + 0x04 && event7f8[i + 1] == 0x1B)
                {
                    event7f8[i] = 0x80;
                    event7f8[i + 1] = 0x80;
                    break;
                }
            }
            return true;
        }

        private static void makeNextFloorEvent(RandoContext context, List<int> bossIds, int currentFloor, int bossEvent, ref int eventNum)
        {
            int bossNum = bossIds[currentFloor];

            string pronoun = "it";
            if(bossNum == VanillaBossMap.TRIPLE_TONPOLE_OBJECT_INDICATOR)
            {
                // three!
                pronoun = "them";
            }

            // -------------- Choice of whether to attempt next floor ------------------
            EventScript bossMaybeEntryEvent = new EventScript();
            context.replacementEvents[eventNum++] = bossMaybeEntryEvent;
            // next event for yes, 0 for no
            bossMaybeEntryEvent.AddDialogueBoxWithChoices("Up next:" + "\n" + bossNames[bossNum] + "!\n\n" + "Fight " + pronoun + " now?\n",
                new byte[] { 2, 8 }, new string[] { "Yep", "Nope" }, new byte[][] { EventScript.GetJumpCmd(eventNum), EventScript.GetJumpCmd(0) }, EventScript.GetJumpCmd(0));
            // end event
            bossMaybeEntryEvent.End();

            // -------------- Choice of 'yes' to enter next boss ------------------
            EventScript bossConfirmEntryEvent = new EventScript();
            context.replacementEvents[eventNum++] = bossConfirmEntryEvent;

            if (bossNum == SomVanillaValues.BOSSID_MANABEAST)
            {
                // mana beast start
                bossConfirmEntryEvent.SetFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1, 8);
                // play the mana beast theme
                bossConfirmEntryEvent.Jsr(0x739);
                // mana beast door
                bossConfirmEntryEvent.ExtendedDoor(0xbc); // see changes in DoorExpansion.cs
                // end event
                bossConfirmEntryEvent.End();
            }
            else
            {
                // yes event - jump to boss event
                bossConfirmEntryEvent.Jump(bossEvent);
                // end event
                bossConfirmEntryEvent.End();
            }
        }

        private static void makeStartingEvent(RandoContext context, RandoSettings settings, ref int eventNum)
        {
            EventScript startingEvent = new EventScript();
            context.replacementEvents[eventNum++] = startingEvent;
            // skip out if 00 is 1+
            startingEvent.Logic(EventFlags.UI_DISPLAY_FLAG, 0x1, 0xF, EventScript.GetJumpCmd(0));

            // enable UI
            startingEvent.IncrFlag(EventFlags.UI_DISPLAY_FLAG);
            startingEvent.IncrFlag(EventFlags.UI_DISPLAY_FLAG);

            // flag 0x0B -> 0x05 for normal death
            startingEvent.SetFlag(EventFlags.DEATH_TYPE_FLAG, 5);

            // you start with sword after bumping event flag 0x00 i think
            startingEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add glove
            startingEvent.Add(0x80);
            startingEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add axe
            startingEvent.Add(0x92);
            startingEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add spear
            startingEvent.Add(0x9B);
            startingEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add whip
            startingEvent.Add(0xA4);
            startingEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add bow
            startingEvent.Add(0xAD);
            startingEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add boomerang
            startingEvent.Add(0xB6);
            startingEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value); // add javelin
            startingEvent.Add(0xBF);

            int startingGold = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.STARTING_GOLD);
            if (startingGold > 65535)
            {
                startingGold = 65535;
            }
            if (startingGold > 0)
            {
                startingEvent.Add(EventCommandEnum.ADD_GOLD.Value);
                startingEvent.Add((byte)startingGold);
                startingEvent.Add((byte)(startingGold >> 8));
            }

            // name girl/sprite
            if (settings.getBool(BossRushSettings.PROPERTYNAME_INCLUDE_GIRL_CHARACTER))
            {
                startingEvent.Add(EventCommandEnum.NAME_CHARACTER.Value);
                startingEvent.Add(0x01);
            }
            if (settings.getBool(BossRushSettings.PROPERTYNAME_INCLUDE_SPRITE_CHARACTER))
            {
                startingEvent.Add(EventCommandEnum.NAME_CHARACTER.Value);
                startingEvent.Add(0x02);
            }

            // Add p2, p3
            if (settings.getBool(BossRushSettings.PROPERTYNAME_INCLUDE_GIRL_CHARACTER))
            {
                startingEvent.Add(EventCommandEnum.ADD_CHARACTER.Value);
                startingEvent.Add(0x01);
            }
            if (settings.getBool(BossRushSettings.PROPERTYNAME_INCLUDE_SPRITE_CHARACTER))
            {
                startingEvent.Add(EventCommandEnum.ADD_CHARACTER.Value);
                startingEvent.Add(0x02);
            }

            int startingConsumables = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.STARTING_CONSUMABLES);

            // add items
            // candy, chocolate, jam, herb, walnut, cup
            byte[] startingItems = new byte[] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45 };
            foreach (byte startingItem in startingItems)
            {
                for (int i = 0; i < startingConsumables; i++)
                {
                    startingEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    startingEvent.Add(startingItem);
                }
            }

            // flammie drum
            startingEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
            startingEvent.Add(0x47);

            // 4 is lumina 5 is shade
            byte[] elementAttributeFlags = new byte[] { 0xC8, 0xC9, 0xCA, 0xCB, 0xCF, 0xCE, 0xCC, 0xCD };
            for (int i = 0; i < 8; i++)
            {
                // girl spells
                if (i != 5)
                {
                    startingEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    startingEvent.Add(0x02);
                    startingEvent.Add(elementAttributeFlags[i]);
                    if (i == 7)
                    {
                        // cut mana magic
                        startingEvent.Add(0x18);
                    }
                    else
                    {
                        // all three spells
                        startingEvent.Add(0x38);
                    }
                }

                // sprite spells
                if (i != 4)
                {
                    startingEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    startingEvent.Add(0x03);
                    startingEvent.Add(elementAttributeFlags[i]);
                    if (i == 7)
                    {
                        // cut mana magic
                        startingEvent.Add(0x03);
                    }
                    else
                    {
                        // all three spells
                        startingEvent.Add(0x07);
                    }
                }
            }

            // and magic rope. 
            startingEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
            startingEvent.Add(0x46);

            // restore everyone's hp
            startingEvent.Add(EventCommandEnum.HEAL.Value);
            startingEvent.Add(0x00);

            // song
            startingEvent.Add(EventCommandEnum.PLAY_SOUND.Value);
            startingEvent.Add(0x01);
            startingEvent.Add(0x02);
            startingEvent.Add(0x0d);
            startingEvent.Add(0xff);

            // full mana power
            double manaPowerBase = 8.0;

            // mana power
            startingEvent.SetFlag(EventFlags.TOTAL_MANA_POWER_FLAG, (byte)manaPowerBase);

            for (int i = 0; i < (int)manaPowerBase; i++)
            {
                // set seeds enabled up to mana power
                startingEvent.SetFlag((byte)(0x90 + i), 1);
            }

            startingEvent.AddDialogueBox(
                "The stats in this mode\n" +
               "probably need some work\n" +
               "to make it appropriately\n" +
               "difficult. Feedback on\n" +
               "this is appreciated!\n");

            // remove boy character if applicable
            if (!settings.getBool(BossRushSettings.PROPERTYNAME_INCLUDE_BOY_CHARACTER))
            {
                startingEvent.Add(EventCommandEnum.REMOVE_CHARACTER.Value);
                startingEvent.Add(0x00);
            }

            // set timer flag on
            startingEvent.SetFlag(EventFlags.PROCGEN_MODE_TIMER_RUNNING_FLAG, 1);

            // end intro event
            startingEvent.End();
        }
    }
}
