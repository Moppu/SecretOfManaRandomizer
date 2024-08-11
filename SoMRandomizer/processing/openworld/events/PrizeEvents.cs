using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System.Collections.Generic;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Replacement events where open world prizes are injected.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class PrizeEvents : RandoProcessor
    {
        public static byte[] OPENWORLD_EVENT_INJECTION_PATTERN = new byte[] { 0x12, 0x34, 0x56, 0x78 };

        public static void injectReplacementPattern(List<byte> eventData, byte index)
        {
            injectReplacementPattern(eventData, index, true);
        }

        public static void injectReplacementPatternAutoDialogue(List<byte> eventData, byte index, byte wait)
        {
            injectReplacementPatternAutoDialogue(eventData, index, true, wait);
        }

        public static void injectReplacementPattern(List<byte> eventData, byte index, bool dialogue)
        {
            if (dialogue)
            {
                eventData.Add(EventCommandEnum.OPEN_DIALOGUE.Value);
            }
            foreach (byte b in OPENWORLD_EVENT_INJECTION_PATTERN)
            {
                eventData.Add(b);
            }
            eventData.Add(index);
            if (dialogue)
            {
                eventData.Add(EventCommandEnum.SLEEP_FOR.Value);
                eventData.Add(0x00);
                eventData.Add(EventCommandEnum.CLOSE_DIALOGUE.Value);
            }
        }

        public static void injectReplacementPatternAutoDialogue(List<byte> eventData, byte index, bool dialogue, byte wait)
        {
            if (dialogue)
            {
                eventData.Add(EventCommandEnum.OPEN_DIALOGUE.Value);
            }
            foreach (byte b in OPENWORLD_EVENT_INJECTION_PATTERN)
            {
                eventData.Add(b);
            }
            eventData.Add(index);
            if (dialogue)
            {
                eventData.Add(EventCommandEnum.SLEEP_FOR.Value);
                eventData.Add(wait);
                eventData.Add(EventCommandEnum.CLOSE_DIALOGUE.Value);
            }
        }

        protected override string getName()
        {
            return "Open world prize events";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool restrictiveLogic = settings.get(OpenWorldSettings.PROPERTYNAME_LOGIC_MODE) == "restrictive";
            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            bool fastManaFort = context.workingData.getBool(OpenWorldGoalProcessor.MANA_FORT_ACCESSIBLE_INDICATOR);
            bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
            // 0x396: used by various other prize events to indicate you've already visited the check/claimed the prize
            EventScript newEvent396 = new EventScript();
            context.replacementEvents[0x396] = newEvent396;
            // no dialogue-open; assume already open
            newEvent396.AddDialogue("Already got this!");
            newEvent396.CloseDialogueBox();
            newEvent396.End();


            // 0x532: sword pedestal event for flammie drum in logic
            EventScript newEvent532 = new EventScript();
            context.replacementEvents[0x532] = newEvent532;
            injectReplacementPattern(newEvent532, 0);
            newEvent532.End();


            // 0x112: mantis ant death
            EventScript newEvent112 = new EventScript();
            context.replacementEvents[0x112] = newEvent112;
            newEvent112.Jsr(0x501); // sword orb
            newEvent112.SetFlag(EventFlags.POTOS_FLAG, 0xC); // mark mantis ant as dead
            injectReplacementPattern(newEvent112, 0);
            newEvent112.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent112.Add(0x01); // victory music
            newEvent112.Add(0x14);
            newEvent112.Add(0x00);
            newEvent112.Add(0xFF);
            newEvent112.Door(0x8E);
            newEvent112.End();


            // 0x132: luka at water palace
            EventScript newEvent132 = new EventScript();
            context.replacementEvents[0x132] = newEvent132;
            injectReplacementPattern(newEvent132, 1);
            injectReplacementPattern(newEvent132, 0);
            newEvent132.SetFlag(EventFlags.WATER_PALACE_FLAG, 0xB); // mark this whole sequence as done
            newEvent132.End();


            // 0x581: undine
            EventScript newEvent581 = new EventScript();
            context.replacementEvents[0x581] = newEvent581;
            newEvent581.Jsr(0x41B); // full heal
            newEvent581.Add(EventCommandEnum.HEAL.Value);
            newEvent581.Add(0x84); // idk more heals
            newEvent581.Add(0x06);
            newEvent581.Add(EventCommandEnum.HEAL.Value);
            newEvent581.Add(0x44); // shrug
            injectReplacementPattern(newEvent581, 0); // undine
            injectReplacementPattern(newEvent581, 1); // poledart
            newEvent581.End();


            // 0x1b0: watts at gaia navel - give one prize, then do normal watts stuff
            EventScript newEvent1b0 = new EventScript();
            context.replacementEvents[0x1b0] = newEvent1b0;
            newEvent1b0.Logic(EventFlags.GAIAS_NAVEL_WATTS_FLAG, 0x8, 0xE, EventScript.GetJumpCmd(0x1B7)); // normal watts
            injectReplacementPattern(newEvent1b0, 0);
            newEvent1b0.IncrFlag(EventFlags.GAIAS_NAVEL_WATTS_FLAG); // 7->8
            newEvent1b0.End();


            // 0x22e: gnome after boss
            EventScript newEvent22e = new EventScript();
            context.replacementEvents[0x22e] = newEvent22e;
            if (restrictiveLogic)
            {
                newEvent22e.Logic(EventFlags.EARTH_SEED, 0x0, 0x0, EventScript.GetJumpCmd(0x133)); // request earth seed if you don't have it
            }
            injectReplacementPattern(newEvent22e, 0); // gnome
            injectReplacementPattern(newEvent22e, 1); // earth seed
            newEvent22e.Add(0x09); // shrug
            newEvent22e.IncrFlag(EventFlags.EARTHPALACE_FLAG);
            newEvent22e.End();


            // 0x366: dwarf elder - give us midge mallet replacement
            EventScript newEvent366 = new EventScript();
            context.replacementEvents[0x366] = newEvent366;
            injectReplacementPattern(newEvent366, 0); // midge mallet
            newEvent366.End();


            // 0x16e: pandora wall death
            EventScript newEvent16E = new EventScript();
            context.replacementEvents[0x16E] = newEvent16E;
            newEvent16E.SetFlag(EventFlags.PANDORA_PHANNA_FLAG, 7);
            newEvent16E.Jsr(0x505); // weapon orb
            injectReplacementPattern(newEvent16E, 0); // treasure
            newEvent16E.Jsr(0x7C3); // music?
            newEvent16E.SetFlag(EventFlags.PANDORA_RUINS_FLAG, 0xA); // unlock treasure room
            newEvent16E.IncrFlag(EventFlags.PANDORA_PHANNA_FLAG);
            newEvent16E.Door(0x25); // out of wall boss map
            newEvent16E.Jump(0x613); // music?
            newEvent16E.End();


            // 0x1b9: tropicallo death - give two prizes
            EventScript newEvent1B9 = new EventScript();
            context.replacementEvents[0x1B9] = newEvent1B9;
            newEvent1B9.Jsr(0x503); // weapon orb
            newEvent1B9.IncrFlag(EventFlags.GAIAS_NAVEL_SPRITE_FLAG); // x15 7->8 so no more tropicallo fights
            injectReplacementPattern(newEvent1B9, 0); // sprite
            injectReplacementPattern(newEvent1B9, 1); // bow
            newEvent1B9.Door(0x52); // exit tropicallo fight
            newEvent1B9.Jsr(0x7CF); // dwarf music
            newEvent1B9.End();


            // 0x689: whip chest at elinee - one prize
            EventScript newEvent689 = new EventScript();
            context.replacementEvents[0x689] = newEvent689;
            newEvent689.SetFlag(EventFlags.WHIP_CHEST_VISIBILITY_FLAG, 2); // makes the chest disappear
            newEvent689.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            injectReplacementPattern(newEvent689, 0);
            newEvent689.End();


            // 0x688: chest next to whip chest at elinee - one prize
            EventScript newEvent688 = new EventScript();
            context.replacementEvents[0x688] = newEvent688;
            newEvent688.SetFlag(EventFlags.NEXT_TO_WHIP_CHEST_VISIBILITY_FLAG, 2); // makes the chest disappear
            newEvent688.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            injectReplacementPattern(newEvent688, 0);
            newEvent688.End();


            // 0x683: pandora chest 1 - one prize
            EventScript newEvent683 = new EventScript();
            context.replacementEvents[0x683] = newEvent683;
            injectReplacementPattern(newEvent683, 0);
            newEvent683.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // remove after opening
            newEvent683.End();


            // 0x684: pandora chest 2 - one prize
            EventScript newEvent684 = new EventScript();
            context.replacementEvents[0x684] = newEvent684;
            injectReplacementPattern(newEvent684, 0);
            newEvent684.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // remove after opening
            newEvent684.End();


            // 0x685: pandora chest 3 - one prize
            EventScript newEvent685 = new EventScript();
            context.replacementEvents[0x685] = newEvent685;
            injectReplacementPattern(newEvent685, 0);
            newEvent685.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // remove after opening
            newEvent685.End();


            // 0x686: pandora chest 4 - one prize
            EventScript newEvent686 = new EventScript();
            context.replacementEvents[0x686] = newEvent686;
            injectReplacementPattern(newEvent686, 0);
            newEvent686.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // remove after opening
            newEvent686.End();


            // 0x188: girl at pandora - one prize
            EventScript newEvent188 = new EventScript();
            context.replacementEvents[0x188] = newEvent188;
            injectReplacementPattern(newEvent188, 0);
            newEvent188.End();


            // 0xaa: lighthouse guy - one prize
            EventScript newEventaa = new EventScript();
            context.replacementEvents[0xaa] = newEventaa;
            injectReplacementPattern(newEventaa, 0);
            newEventaa.End();


            // 0x680: potos chest - one prize
            EventScript newEvent680 = new EventScript();
            context.replacementEvents[0x680] = newEvent680;
            injectReplacementPattern(newEvent680, 0);
            newEvent680.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            newEvent680.End();


            // 0x687: magic rope chest - one prize
            EventScript newEvent687 = new EventScript();
            context.replacementEvents[0x687] = newEvent687;
            injectReplacementPattern(newEvent687, 0);
            newEvent687.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            newEvent687.End();


            // 0x68a: fire palace chest 1 - one prize
            EventScript newEvent68a = new EventScript();
            context.replacementEvents[0x68a] = newEvent68a;
            injectReplacementPattern(newEvent68a, 0);
            newEvent68a.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            newEvent68a.End();


            // 0x68b: fire palace chest 2 - one prize
            EventScript newEvent68b = new EventScript();
            context.replacementEvents[0x68b] = newEvent68b;
            injectReplacementPattern(newEvent68b, 0);
            newEvent68b.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            newEvent68b.End();


            // 0x68d: northtown castle chest - one prize
            EventScript newEvent68d = new EventScript();
            context.replacementEvents[0x68d] = newEvent68d;
            injectReplacementPattern(newEvent68d, 0);
            newEvent68d.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            newEvent68d.End();


            // 0x68e: shade palace chest - one prize
            EventScript newEvent68e = new EventScript();
            context.replacementEvents[0x68e] = newEvent68e;
            injectReplacementPattern(newEvent68e, 0);
            newEvent68e.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            newEvent68e.End();


            // a list of events to be used by individual orb rewards.
            // in vanilla, orb rewards for a given weapon all share the same event. 
            // each chest is intended to make you hit a certain level, and if you bypass that level, it just disappears.
            // we don't really want that behavior for open world, so we separate orbs out all into separate
            // events with unique flags, making use of some events that vanilla doesn't use.
            // 19 prizes total.
            int[] orbChestReplacementEvents = new int[] {
                // note that the first two undersea ones are unreachable in vanilla, but they do exist on some map
                // 0x670: moogle village glove orb chest
                0x670, // vanilla: glove's orb (undersea subway area, moogle village, ice castle, shade palace)
                // 0x671: ice castle glove orb chest
                0x671, // vanilla: sword's orb (undersea subway area, pandora chest room, ntr, grand palace after whip platform)
                // 0x672: shade palace glove orb chest
                0x672, // vanilla: axe's orb (moogle village, lumina tower, fire palace small maze room, NTC room on the right)
                // 0x673: pandora treasure room sword orb chest
                0x673, // vanilla: spear's orb (outside NTR, lumina tower, pandora chest room, santa's house)
                // 0x674: NTR sword orb chest
                0x674, // vanilla: whip's orb (before kilroy, NTC room on the right)
                // 0x675: grand palace sword orb chest
                0x675, // vanilla: bow's orb (outside NTR)
                // 0x676: moogle village axe orb chest
                0x676, // vanilla: boomerang's orb (just inside undersea area)
                // 0x677: lumina tower axe orb chest
                0x677, // vanilla: javelin's orb (matango inn)
                // 0x678: fire palace axe orb chest
                0x678, // vanilla: unused
                // 0x679: NTC axe orb chest
                0x679, // vanilla: unused
                // 0x67a: NTR spear orb chest
                0x67a, // vanilla: unused
                // 0x67b: lumina tower spear orb chest
                0x67b, // vanilla: unused
                // 0x67c: pandora spear orb chest
                0x67c, // vanilla: unused
                // 0x67d: santa spear orb chest
                0x67d, // vanilla: unused
                // 0x68c: kilroy whip orb chest
                0x68c, // vanilla: unused
                // 0x68f: NTC whip orb chest
                0x68f, // vanilla: unused 
                // 0x690: NTR bow orb chest
                0x690, // vanilla: unused
                // 0x691: undersea boomerang orb chest
                0x691, // vanilla: unused
                // 0x692: matango inn javelin orb chest
                0x692, // vanilla: unused
            };
            foreach (int orbChestEventNum in orbChestReplacementEvents)
            {
                EventScript newEvent = new EventScript();
                context.replacementEvents[orbChestEventNum] = newEvent;
                injectReplacementPattern(newEvent, 0);
                newEvent.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
                newEvent.End();
            }


            // 0x36a: kilroy death - one prize
            EventScript newEvent36a = new EventScript();
            context.replacementEvents[0x36a] = newEvent36a;
            newEvent36a.Jsr(0x507); // orb
            injectReplacementPattern(newEvent36a, 0); // kilroy prize
            newEvent36a.Jsr(0x7cf); // victory music?
            newEvent36a.IncrFlag(EventFlags.KILROY_EVENT_FLAG);
            newEvent36a.IncrFlag(EventFlags.KILROY_EVENT_FLAG);
            newEvent36a.Door(0x1c3); // doorway
            newEvent36a.End(); // don't increment 0x50, so water palace doesn't get fucky


            // 0x4e2: sylphid palace - two prizes at the elder dude
            EventScript newEvent4E2 = new EventScript();
            context.replacementEvents[0x4E2] = newEvent4E2;
            newEvent4E2.Add(0x06);
            newEvent4E2.OpenDialogueBox();
            newEvent4E2.Logic(EventFlags.UPPERLAND_PROGRESS_FLAG, 0x5, 0xC, EventScript.GetJumpCmd(0x253)); // save/restore event if we've already gotten the prize
            newEvent4E2.Jsr(0x796); // sylphid summon sound
            newEvent4E2.SetFlag(EventFlags.UPPERLAND_PROGRESS_FLAG, 4); // sylphid appears
            newEvent4E2.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            if (restrictiveLogic)
            {
                newEvent4E2.Logic(EventFlags.WIND_SEED, 0x0, 0x0, EventScript.GetJumpCmd(0x133)); // don't have wind seed? request wind seed
            }
            injectReplacementPattern(newEvent4E2, 0);
            injectReplacementPattern(newEvent4E2, 1);
            newEvent4E2.IncrFlag(EventFlags.UPPERLAND_PROGRESS_FLAG); // mark this as gotten so we can run the save/restore event next time
            newEvent4E2.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            newEvent4E2.End();


            // 0x589: salamando npc i added in fire palace. this repurposes the vanilla "talk to the seed" event where the screen flashes
            EventScript newEvent589 = new EventScript();
            context.replacementEvents[0x589] = newEvent589;
            if (restrictiveLogic)
            {
                newEvent589.Logic(EventFlags.FIRE_SEED, 0x0, 0x0, EventScript.GetJumpCmd(0x133)); // don't have fire seed? request fire seed
            }
            injectReplacementPattern(newEvent589, 0);
            newEvent589.End();


            // 0x587: lumina - two prizes
            EventScript newEvent587 = new EventScript();
            context.replacementEvents[0x587] = newEvent587;
            if (restrictiveLogic)
            {
                newEvent587.Logic(EventFlags.LIGHT_SEED, 0x0, 0x0, EventScript.GetJumpCmd(0x133)); // don't have light seed? request light seed
            }
            injectReplacementPattern(newEvent587, 0);
            injectReplacementPattern(newEvent587, 1);
            newEvent587.End();


            // 0x586: shade - two prizes
            EventScript newEvent586 = new EventScript();
            context.replacementEvents[0x586] = newEvent586;
            if (restrictiveLogic)
            {
                newEvent586.Logic(EventFlags.DARK_SEED, 0x0, 0x0, EventScript.GetJumpCmd(0x133)); // don't have dark seed? request dark seed
            }
            injectReplacementPattern(newEvent586, 0);
            injectReplacementPattern(newEvent586, 1);
            newEvent586.End();


            // 0x584: luna - two prizes
            EventScript newEvent584 = new EventScript();
            context.replacementEvents[0x584] = newEvent584;
            if (restrictiveLogic)
            {
                newEvent584.Logic(EventFlags.MOON_SEED, 0x0, 0x0, EventScript.GetJumpCmd(0x133)); // don't have moon seed? request moon seed
            }
            injectReplacementPattern(newEvent584, 0);
            injectReplacementPattern(newEvent584, 1);
            newEvent584.End();


            // 0x4b6: dryad - two prizes
            EventScript newEvent4b6 = new EventScript();
            context.replacementEvents[0x4b6] = newEvent4b6;
            if (restrictiveLogic)
            {
                newEvent4b6.Logic(EventFlags.DRYAD_SEED, 0x0, 0x0, EventScript.GetJumpCmd(0x133)); // don't have dryad seed? request dryad seed
            }
            injectReplacementPattern(newEvent4b6, 0);
            injectReplacementPattern(newEvent4b6, 1);
            newEvent4b6.End();


            // 0x399: mara - one prize
            EventScript newEvent399 = new EventScript();
            context.replacementEvents[0x399] = newEvent399;
            injectReplacementPattern(newEvent399, 0);
            newEvent399.End();


            // 0x2f9: sea hare tail guy on turtle island - one prize
            EventScript newEvent2f9 = new EventScript();
            context.replacementEvents[0x2f9] = newEvent2f9;
            injectReplacementPattern(newEvent2f9, 0);
            newEvent2f9.End();


            // 0x2b2: aman at kakkara - one prize. the sea hare tail check is in a separate event (0x2b0)
            EventScript newEvent2b2 = new EventScript();
            context.replacementEvents[0x2b2] = newEvent2b2;
            newEvent2b2.IncrFlag(EventFlags.SEA_HARES_TAIL_FLAG);
            newEvent2b2.AddDialogueBox("oh sweet a sea hare\ntail! i love these");
            injectReplacementPattern(newEvent2b2, 0);
            newEvent2b2.End();


            // 0x38c: salamando in the stove in ice country - one prize
            EventScript newEvent38C = new EventScript();
            context.replacementEvents[0x38C] = newEvent38C;
            newEvent38C.Logic(EventFlags.SALAMANDO_STOVE_FLAG, 0x5, 0xF, EventScript.GetJumpCmd(0));
            injectReplacementPattern(newEvent38C, 0);
            newEvent38C.IncrFlag(EventFlags.SALAMANDO_STOVE_FLAG); // change stove sprite
            newEvent38C.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // update map objs
            newEvent38C.End();


            // 0x661: triple tonpole fight end - one prize
            EventScript newEvent66a = new EventScript();
            context.replacementEvents[0x66a] = newEvent66a;
            newEvent66a.IncrFlag(EventFlags.MULTI_TONPOLE_FLAG); // one counts for three
            newEvent66a.IncrFlag(EventFlags.MULTI_TONPOLE_FLAG);
            newEvent66a.IncrFlag(EventFlags.MULTI_TONPOLE_FLAG);
            newEvent66a.SetFlag(EventFlags.ICE_CASTLE_SWITCHES_FLAG_2, 5);
            newEvent66a.SetFlag(EventFlags.ICE_PALACE_FLAG, 3);
            injectReplacementPattern(newEvent66a, 0);
            newEvent66a.Door(0x2C3); // exit thingy
            newEvent66a.Jump(0x715); // music probably
            newEvent66a.End();


            // 0x66d: frost gigas death event - warp to santa and give a prize, then warp out
            EventScript newEvent66D = new EventScript();
            context.replacementEvents[0x66d] = newEvent66D;
            newEvent66D.SetFlag(EventFlags.ICE_CASTLE_SWITCHES_FLAG_2, 6); // switches
            newEvent66D.SetFlag(EventFlags.ICE_PALACE_FLAG, 4); // ice palace event flag
            newEvent66D.Jsr(0x506); // orb
            injectReplacementPattern(newEvent66D, 0);
            newEvent66D.Jsr(0x7C3); // music
            newEvent66D.Door(0x2CB); // exit santa boss fight
            newEvent66D.IncrFlag(EventFlags.ICE_PALACE_FLAG); // ice palace event flag
            newEvent66D.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            string santaDialogue;
            if (goal == OpenWorldGoalProcessor.GOAL_GIFTMODE || goal == OpenWorldGoalProcessor.GOAL_REINDEER)
            {
                santaDialogue = "Yeah idk I'm here too for some reason";
            }
            else
            {
                santaDialogue = "Merry Christmas!";
            }
            newEvent66D.AddDialogueBox(santaDialogue);
            newEvent66D.IncrFlag(EventFlags.ICE_PALACE_FLAG);
            newEvent66D.Door(0x200); // exit thingy
            newEvent66D.Jump(0x636); // idk
            newEvent66D.End();


            // 0x4e4: flammie in the matango cave after the snake boss - one prize
            EventScript newEvent4e4 = new EventScript();
            context.replacementEvents[0x4E4] = newEvent4e4;
            newEvent4e4.Logic(EventFlags.MATANGO_PROGRESS_FLAG, 0x7, 0xF, EventScript.GetJumpCmd(0));
            injectReplacementPattern(newEvent4e4, 0); // flammie prize
            newEvent4e4.Door(0x1CE);
            newEvent4e4.SetFlag(EventFlags.MATANGO_PROGRESS_FLAG, 8);
            newEvent4e4.End();


            // doom's wall death event - one prize
            EventScript newEvent553 = new EventScript();
            context.replacementEvents[0x553] = newEvent553;
            newEvent553.Jsr(0x504); // orb
            newEvent553.IncrFlag(EventFlags.NORTHTOWN_RUINS_FLAG); // double increment this flag so we don't have to talk to dyluck after
            newEvent553.IncrFlag(EventFlags.NORTHTOWN_RUINS_FLAG);
            injectReplacementPattern(newEvent553, 0); // wall prize
            newEvent553.Jump(0x647); // nfi
            newEvent553.End();


            // 0x559: vampire death event - one prize
            EventScript newEvent559 = new EventScript();
            context.replacementEvents[0x559] = newEvent559;
            newEvent559.Jsr(0x506); // orb reward
            injectReplacementPattern(newEvent559, 0); // vampire prize
            newEvent559.Jsr(0x7D8); // music?
            newEvent559.IncrFlag(EventFlags.NORTHTOWN_RUINS_FLAG);
            newEvent559.Door(0x2D0); // exit vampire boss fight to the beginning of ruins
            newEvent559.End();


            // 0x52f: mech rider 2 death - one prize then gtfo
            EventScript newEvent52f = new EventScript();
            context.replacementEvents[0x52f] = newEvent52f;
            newEvent52f.Jsr(0x507); // orb reward
            newEvent52f.IncrFlag(EventFlags.NORTHTOWN_CASTLE_PROGRESS_FLAG);
            newEvent52f.IncrFlag(EventFlags.NORTHTOWN_CASTLE_PROGRESS_FLAG);
            newEvent52f.IncrFlag(EventFlags.NORTHTOWN_CASTLE_PROGRESS_FLAG);
            injectReplacementPattern(newEvent52f, 0); // mech rider 2 prize
            if (flammieDrumInLogic)
            {
                newEvent52f.Door(0x9C); // door back into northtown
            }
            else
            {
                newEvent52f.Add(EventCommandEnum.FLAMMIE_FROM_POS.Value);
                newEvent52f.Add(0x64); // flammie out like vanilla
            }
            newEvent52f.End();


            // 0x4e5: end of the jehk doppelganger test thing - one prize
            EventScript newEvent4e5 = new EventScript();
            context.replacementEvents[0x4E5] = newEvent4e5;
            newEvent4e5.Logic(EventFlags.JEHK_CAVE_FLAG, 0x6, 0xF, EventScript.GetJumpCmd(0x460));
            newEvent4e5.Add(0x4A); // idk something to do with the enemy characters
            newEvent4e5.Add(0x04);
            newEvent4e5.Add(0x91);
            newEvent4e5.Add(0x7F);
            newEvent4e5.Add(0x13);
            newEvent4e5.Add(0xDE);
            newEvent4e5.Add(0x4A); // idk something to do with the enemy characters
            newEvent4e5.Add(0x05);
            newEvent4e5.Add(0x91);
            newEvent4e5.Add(0x7F);
            newEvent4e5.Add(0x13);
            newEvent4e5.Add(0xDE);
            newEvent4e5.Add(0x4A); // idk something to do with the enemy characters
            newEvent4e5.Add(0x06);
            newEvent4e5.Add(0x91);
            newEvent4e5.Add(0x7F);
            newEvent4e5.Add(0x13);
            newEvent4e5.Add(0xDE);
            newEvent4e5.SetFlag(EventFlags.JEHK_CAVE_FLAG, 6);
            injectReplacementPattern(newEvent4e5, 0); // jehk prize
            newEvent4e5.Door(0x38); // back to jehk room
            newEvent4e5.Add(0x06);
            newEvent4e5.Jsr(0x41B); // heals i think
            newEvent4e5.Add(EventCommandEnum.HEAL.Value);
            newEvent4e5.Add(0x84);
            newEvent4e5.End();


            // 0x5ed: dragon worm (pure lands first boss) - one prize
            EventScript newEvent5ed = new EventScript();
            context.replacementEvents[0x5ed] = newEvent5ed;
            newEvent5ed.Jsr(0x41b); // ?
            newEvent5ed.Jsr(0x502); // orb
            newEvent5ed.IncrFlag(EventFlags.PURELAND_PROGRESS_FLAG);
            injectReplacementPattern(newEvent5ed, 0); // dragon worm prize
            newEvent5ed.Jump(0x62a); // ?
            newEvent5ed.End();


            // 0x5ef: snow dragon (pure lands second boss probably) - one prize
            EventScript newEvent5ef = new EventScript();
            context.replacementEvents[0x5ef] = newEvent5ef;
            newEvent5ef.Jsr(0x505); // orb
            newEvent5ef.IncrFlag(EventFlags.PURELAND_PROGRESS_FLAG);
            injectReplacementPattern(newEvent5ef, 0); // snow dragon prize
            newEvent5ef.Jump(0x62a); // ?
            newEvent5ef.End();


            // 0x5f1: axe beak (pure lands third boss) - one prize
            EventScript newEvent5f1 = new EventScript();
            context.replacementEvents[0x5f1] = newEvent5f1;
            newEvent5f1.Jsr(0x507); // orb
            injectReplacementPattern(newEvent5f1, 0); // axebeak prize
            newEvent5f1.IncrFlag(EventFlags.PURELAND_PROGRESS_FLAG); // this probably opens the stairs
            newEvent5f1.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent5f1.Jump(0x62a); // ?
            newEvent5f1.End();


            // 0x5f3: red dragon (pure lands fourth boss? i think?) - one prize
            EventScript newEvent5f3 = new EventScript();
            context.replacementEvents[0x5f3] = newEvent5f3;
            newEvent5f3.Jsr(0x500); // orb
            injectReplacementPattern(newEvent5f3, 0); // red dragon prize
            newEvent5f3.IncrFlag(EventFlags.PURELAND_PROGRESS_FLAG);
            newEvent5f3.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent5f3.Jump(0x62a); // ?
            newEvent5f3.End();


            // 0x5f5: thunder gigas (pure lands fifth boss) - one prize
            EventScript newEvent5f5 = new EventScript();
            context.replacementEvents[0x5f5] = newEvent5f5;
            newEvent5f5.Jsr(0x501); // orb
            injectReplacementPattern(newEvent5f5, 0); // thunder gigas prize
            newEvent5f5.IncrFlag(EventFlags.PURELAND_PROGRESS_FLAG);
            newEvent5f5.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent5f5.Jump(0x62a); // ?
            newEvent5f5.End();


            // 0x5f7: blue dragon (pure lands sixth boss) - one prize
            EventScript newEvent5f7 = new EventScript();
            context.replacementEvents[0x5f7] = newEvent5f7;
            newEvent5f7.IncrFlag(EventFlags.PURELAND_PROGRESS_FLAG);
            newEvent5f7.Jsr(0x506); // orb
            injectReplacementPattern(newEvent5f7, 0); // blue dragon prize
            newEvent5f7.Jump(0x62a); // ?
            newEvent5f7.End();


            // 0x4a4: mech rider 3 death event - give one prize and unlock manafort if mode applicable
            EventScript newEvent4A4 = new EventScript();
            context.replacementEvents[0x4a4] = newEvent4A4;
            newEvent4A4.Jsr(0x503); // orb
            injectReplacementPattern(newEvent4A4, 0);
            newEvent4A4.Door(0x3F7); // exit thingy
            newEvent4A4.SetFlag(EventFlags.OPENWORLD_MANAFORT_ACCESSIBLE_FLAG, 2); // set manafort accessible regardless of mode
            if (!fastManaFort)
            {
                // if mode locks manafort, show a message that we unlocked it
                newEvent4A4.AddDialogueBox("Unlocked Mana Fortress.");
            }
            newEvent4A4.End();


            // 0x4b4: watermelon death - one prize
            EventScript newEvent4B4 = new EventScript();
            context.replacementEvents[0x4B4] = newEvent4B4;
            newEvent4B4.Jsr(0x503); // orb
            newEvent4B4.IncrFlag(EventFlags.LOST_CONTINENT_WATERMELON_FLAG);
            injectReplacementPattern(newEvent4B4, 0); // mech rider 2 prize
            newEvent4B4.Jsr(0x7D9); // music?
            newEvent4B4.Door(0x348); // out of watermelon fight
            newEvent4B4.End();


            // 0x4a2: hexas death - one prize
            EventScript newEvent4A2 = new EventScript();
            context.replacementEvents[0x4A2] = newEvent4A2;
            newEvent4A2.Jsr(0x504); // orb
            newEvent4A2.Jsr(0x7DC); // music?
            newEvent4A2.IncrFlag(EventFlags.LOST_CONTINENT_HEXAS_FLAG);
            newEvent4A2.Add(EventCommandEnum.REFRESH_MAP.Value);
            injectReplacementPattern(newEvent4A2, 0);
            newEvent4A2.End();


            // 0x527: metal mantis - one prize
            List<byte> newEvent527 = new List<byte>();
            context.replacementEvents[0x527] = newEvent527;
            newEvent527.Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + 0x05));
            newEvent527.Add(0x00); // orb
            newEvent527.Add(EventCommandEnum.INCREMENT_FLAG.Value);
            newEvent527.Add(0x58);
            newEvent527.Add(0x0A);
            injectReplacementPattern(newEvent527, 0);
            newEvent527.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x07));
            newEvent527.Add(0x26); // music
            newEvent527.Add(EventCommandEnum.END.Value);


            // 0x59b: hydra death - one prize
            EventScript newEvent59b = new EventScript();
            context.replacementEvents[0x59b] = newEvent59b;
            newEvent59b.Jsr(0x504); // whip's orb
            newEvent59b.SetFlag(EventFlags.LOST_CONTINENT_SWITCHES_FLAG_1, 4); // idk
            injectReplacementPattern(newEvent59b, 0);
            newEvent59b.Jsr(0x7D0); // music?
            newEvent59b.Door(0x31B); // out
            newEvent59b.End();


            // 0x4ca: kettle kin death - one prize
            EventScript newEvent4ca = new EventScript();
            context.replacementEvents[0x4ca] = newEvent4ca;
            newEvent4ca.SetFlag(EventFlags.LOST_CONTINENT_KETTLEKIN_FLAG, 3);
            newEvent4ca.Jsr(0x505); // orb
            injectReplacementPattern(newEvent4ca, 0);
            newEvent4ca.Jsr(0x7DD); // music?
            newEvent4ca.Door(0x3FE); // door out
            newEvent4ca.Jump(0x702); // do this instead for music
            newEvent4ca.End();


            // 0x422: buffy death - one prize
            EventScript newEvent422 = new EventScript();
            context.replacementEvents[0x422] = newEvent422;
            newEvent422.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            newEvent422.Jsr(0x731);
            injectReplacementPattern(newEvent422, 0);
            newEvent422.Door(0x93); // door out
            newEvent422.End();


            // 0x425: dread slime death - one prize
            EventScript newEvent425 = new EventScript();
            context.replacementEvents[0x425] = newEvent425;
            newEvent425.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            newEvent425.SetFlag(EventFlags.DEATH_TYPE_FLAG, 2); // ?
            newEvent425.Jsr(0x731);
            newEvent425.Door(0xC0); // door out - take this once so we can show prize text, off of the mode 7 map
            injectReplacementPattern(newEvent425, 0);
            newEvent425.Door(0xC0); // door out - take this again so we autosave with the prize
            newEvent425.Jump(0x42C); // duplicate music idk why
            newEvent425.End();


            // 0x2d1: tasnica miniboss fight - one prize
            EventScript newEvent2d1 = new EventScript();
            context.replacementEvents[0x2d1] = newEvent2d1;
            newEvent2d1.Add(0x4a); // this is some sort of hp check for the enemies
            newEvent2d1.Add(0x04);
            newEvent2d1.Add(0x80);
            newEvent2d1.Add(0xe9);
            newEvent2d1.Add(0x12);
            newEvent2d1.Add(0xCF); // jump to 2CF
            newEvent2d1.IncrFlag(EventFlags.TASNICA_FLAG); // 
            injectReplacementPattern(newEvent2d1, 0);
            newEvent2d1.Door(0x367); // king room
            newEvent2d1.Jsr(0x7CA); // music
            newEvent2d1.IncrFlag(EventFlags.TASNICA_FLAG); // 
            newEvent2d1.IncrFlag(EventFlags.TASNICA_FLAG); // 
            newEvent2d1.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // 
            newEvent2d1.End();


            // 0x5f8: that long event where the mana tree blows up - one prize, but only in vanilla goals
            EventScript newEvent5f8 = new EventScript();
            context.replacementEvents[0x5f8] = newEvent5f8;
            newEvent5f8.Logic(EventFlags.PURELAND_PROGRESS_FLAG, 0x0, 0x5, EventScript.GetJumpCmd(0x5DD)); // only allow progress if you killed the last pureland boss
            newEvent5f8.Jsr(0x5DA); // save screen
            newEvent5f8.Add(0x06);
            newEvent5f8.Add(EventCommandEnum.MOVE_EVERYONE_TO_P1.Value);
            newEvent5f8.Jsr(0x72C); // music?
            newEvent5f8.Add(EventCommandEnum.INVIS_A.Value);
            newEvent5f8.Door(0x5E); // mana tree map
            newEvent5f8.IncrFlag(EventFlags.WALK_THROUGH_WALLS_FLAG); // go through walls?
            newEvent5f8.Add(0x34); // animation
            newEvent5f8.Add(0x00);
            newEvent5f8.Add(0x8B);
            newEvent5f8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5f8.Sleep(0x14);
            newEvent5f8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5f8.Add(0x00);
            newEvent5f8.Add(0x00); // stop the animation
            newEvent5f8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5f8.Jsr(0x72D); // whale sound
            newEvent5f8.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent5f8.Add(0x06);
            newEvent5f8.Add(0xFF);
            newEvent5f8.Add(0x7F); // palette flash
            newEvent5f8.Sleep(0x20);
            newEvent5f8.SetFlag(EventFlags.PURELAND_PROGRESS_FLAG, 7); // make the other version of the characters appear, i think
            newEvent5f8.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent5f8.Sleep(0x20);
            newEvent5f8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5f8.Add(0x00);
            newEvent5f8.Add(0x68); // p1 back down
            newEvent5f8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5f8.Add(EventCommandEnum.MOVE_EVERYONE_TO_P1.Value);
            newEvent5f8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5f8.Add(0x01);
            newEvent5f8.Add(0x00); // p1 face up
            newEvent5f8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5f8.Add(0x02);
            newEvent5f8.Add(0xC8); // p2 move left
            newEvent5f8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5f8.Add(0x03);
            newEvent5f8.Add(0x88); // p3 move right
            newEvent5f8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5f8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5f8.Add(0x02);
            newEvent5f8.Add(0x48); // p2 move down
            newEvent5f8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5f8.Add(0x03);
            newEvent5f8.Add(0x48); // p3 move down
            newEvent5f8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5f8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5f8.Add(0x02);
            newEvent5f8.Add(0x00); // p2 face up
            newEvent5f8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5f8.Add(0x03);
            newEvent5f8.Add(0x00); // p3 face up
            newEvent5f8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5f8.Add(EventCommandEnum.INVIS_B.Value);
            newEvent5f8.Add(0x2D);
            newEvent5f8.Add(0x07); // palette unflash
            newEvent5f8.Sleep(0x20);
            newEvent5f8.SetFlag(EventFlags.PURELAND_PROGRESS_FLAG, 8);
            newEvent5f8.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent5f8.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent5f8.Add(0x01); // boss kill sound
            newEvent5f8.Add(0x18);
            newEvent5f8.Add(0x44);
            newEvent5f8.Add(0x8F);
            newEvent5f8.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent5f8.Add(0x06);
            newEvent5f8.Add(0xFF);
            newEvent5f8.Add(0x7F); // palette flash
            newEvent5f8.Sleep(0x40);
            newEvent5f8.DecrFlag(EventFlags.WALK_THROUGH_WALLS_FLAG); // don't go through walls?
            if (goal == OpenWorldGoalProcessor.GOAL_MANABEAST)
            {
                injectReplacementPattern(newEvent5f8, 0);
            }
            newEvent5f8.Door(0x368); // base of mana tree
            newEvent5f8.Jsr(0x734); // sad mana theme
            if (goal == OpenWorldGoalProcessor.GOAL_MTR)
            {

                // check mana seeds; also need to do this in walk-on event 62a (for landing), but only if you're through the purelands (x46 == x8)
                newEvent5f8.Jump(0x4D8); // check end condition
            }
            newEvent5f8.End();


            return true;
        }
    }
}
