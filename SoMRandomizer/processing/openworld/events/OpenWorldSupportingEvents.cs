using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System.Collections.Generic;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Progression related event replacements for open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    class OpenWorldSupportingEvents : RandoProcessor
    {
        protected override string getName()
        {
            return "Supporting events for open world";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool restrictiveLogic = settings.get(OpenWorldSettings.PROPERTYNAME_LOGIC_MODE) == "restrictive";
            bool anySpellsAvailable = context.workingData.getBool(OpenWorldClassSelection.ANY_MAGIC_EXISTS);
            Dictionary<int, byte> crystalOrbColorMap = ElementSwaps.getCrystalOrbElementMap(context);
            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);

            // 0x105: rabite event - remove dialogue
            EventScript newEvent105 = new EventScript();
            context.replacementEvents[0x105] = newEvent105;
            newEvent105.Jump(0x70E); // change music
            newEvent105.End();


            // 0x10b: mantis ant fight trigger (used to be guard guy dialogue)
            EventScript newEvent10B = new EventScript();
            context.replacementEvents[0x10B] = newEvent10B;
            newEvent10B.Logic(EventFlags.POTOS_FLAG, 0x0C, 0x0F, EventScript.GetJumpCmd(0)); // do nothing if already did mantis ant
            newEvent10B.AddDialogueBoxWithChoices("Fight the Mantis Ant?\n", new byte[] { 2, 8 }, new string[] { "Yep", "Nope" }, new byte[][] { EventScript.GetJumpCmd(0x10C), EventScript.GetJumpCmd(0) }, EventScript.GetJumpCmd(0));
            newEvent10B.End();


            // 0x10c: mantis event "yep" option - warp you in
            EventScript newEvent10C = new EventScript();
            context.replacementEvents[0x10C] = newEvent10C;
            newEvent10C.Door(0x8D);
            newEvent10C.Jsr(0x7C4); // boss theme
            newEvent10C.End();


            // 0x127: stepping on the bridge tile in water palace - cut the vanilla event of introducing luka
            EventScript newEvent127 = new EventScript();
            context.replacementEvents[0x127] = newEvent127;
            newEvent127.SetFlag(EventFlags.DOOR_CONTROL_FLAG_3, 1);
            newEvent127.Jsr(0x791); // sound?
            newEvent127.Jsr(0x78C); // sound?
            newEvent127.SetFlag(EventFlags.DOOR_CONTROL_FLAG_2, 1);
            newEvent127.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent127.SetFlag(0xFD, 0); // nfi
            newEvent127.SetFlag(0xFE, 2); // nfi
            newEvent127.End();


            // 0x130: talking to luka directly
            EventScript newEvent130 = new EventScript();
            context.replacementEvents[0x130] = newEvent130;


            // 0x133: shared event to request a seed when you're missing it; restrictive logic only
            if (restrictiveLogic)
            {
                EventScript newEvent133 = new EventScript();
                context.replacementEvents[0x133] = newEvent133;
                newEvent133.AddDialogueBox("Bring me the seed!");
                newEvent133.End();

                // request water seed
                newEvent130.Logic(EventFlags.WATER_SEED, 0, 0, EventScript.GetJumpCmd(0x133)); // if seed flag is 0, jump to bring me the seed event created above
            }

            newEvent130.Logic(EventFlags.WATER_PALACE_FLAG, 0xB, 0xF, EventScript.GetJumpCmd(0x134)); // save dialogue if you've already gotten the prize
            newEvent130.Jsr(0x132); // gift event
            newEvent130.End();


            // 0x123: jema outside water palace as jabberwocky entryway
            EventScript newEvent123 = new EventScript();
            context.replacementEvents[0x123] = newEvent123;
            newEvent123.Logic(EventFlags.OPENWORLD_JABBERWOCKY_FLAG, 0x1, 0xF, EventScript.GetJumpCmd(0x131)); // should never be encountered since jema disappears, but handle already having done it
            // the cancel/no event here walks away by a couple tiles (see below)
            newEvent123.AddDialogueBoxWithChoices("Fight the Jabberwocky?\n", new byte[] { 2, 8 }, new string[] { "Yep", "Nope" }, new byte[][] { EventScript.GetJumpCmd(0x84), EventScript.GetJumpCmd(0x131) }, EventScript.GetJumpCmd(0x131));
            newEvent123.End();


            // 0x131: jabberwocky "nope" option
            EventScript newEvent131 = new EventScript();
            context.replacementEvents[0x131] = newEvent131;
            // MOPPLE: there should be EventScript methods to support this command where you pass in the direction, character num, and distance,
            // to make it easier
            newEvent131.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent131.Add(0x00); // just step down a bit
            newEvent131.Add(0x50);
            newEvent131.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent131.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent131.Add(0x08); // center camera
            newEvent131.End();


            // 0x84: jabberwocky "yep" option - warp you in
            EventScript newEvent84 = new EventScript();
            context.replacementEvents[0x84] = newEvent84;
            newEvent84.Jsr(0x6F6); // shut all the doors in water palace
            newEvent84.SetFlag(EventFlags.WATER_PALACE_HOSTILITY_FLAG, 1); // jabberwocky visibility; will set back to 0 later
            newEvent84.Door(0xA0);
            newEvent84.Jump(0x704); // boss theme
            newEvent84.End();


            // 0x20c: jabberwocky death event. note there are no open world prizes here since it blocks luka, who has two
            EventScript newEvent20C = new EventScript();
            context.replacementEvents[0x20C] = newEvent20C;
            newEvent20C.SetFlag(EventFlags.OPENWORLD_JABBERWOCKY_FLAG, 1); // done with jabberwocky
            newEvent20C.Jsr(0x505); // orb
            newEvent20C.SetFlag(EventFlags.WATER_PALACE_HOSTILITY_FLAG, 0);
            newEvent20C.SetFlag(EventFlags.DOOR_CONTROL_FLAG_1, 1); // as if we had stepped on the first water palace button to open the steps
            newEvent20C.Door(0xA1); // jabberwocky return door
            newEvent20C.Jsr(0x7C3); // music?
            newEvent20C.End();


            // 0x11f: remove goblin event trigger on map 16
            EventScript newEvent11F = new EventScript();
            context.replacementEvents[0x11F] = newEvent11F;
            newEvent11F.End();


            // 0x16d: thanatos at pandora ruins leading into wall fight on another map
            EventScript newEvent16D = new EventScript();
            context.replacementEvents[0x16D] = newEvent16D;
            newEvent16D.Add(0x06); // ?
            newEvent16D.IncrFlag(EventFlags.PANDORA_PHANNA_FLAG);
            newEvent16D.IncrFlag(EventFlags.PANDORA_PHANNA_FLAG);
            newEvent16D.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent16D.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent16D.Add(0x08); // center camera
            newEvent16D.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent16D.Add(0x80);
            newEvent16D.Add(0x20); // thanatos up
            newEvent16D.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent16D.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent16D.Add(0x80);
            newEvent16D.Add(0x40); // thanatos look down
            newEvent16D.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent16D.Add(0x01);
            newEvent16D.Add(0x10); // boy up
            newEvent16D.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent16D.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent16D.Add(0x02);
            newEvent16D.Add(0x10); // girl up
            newEvent16D.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent16D.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent16D.Add(0x03);
            newEvent16D.Add(0x10); // sprite up
            newEvent16D.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent16D.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent16D.Add(0x08); // center camera
            newEvent16D.Add(EventCommandEnum.MOVE_EVERYONE_TO_P1.Value);
            newEvent16D.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent16D.Add(0x80);
            newEvent16D.Add(0x20); // thanatos up
            newEvent16D.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent16D.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent16D.Add(0x80);
            newEvent16D.Add(0x40); // thanatos look down
            newEvent16D.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent16D.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent16D.Add(0x01);
            newEvent16D.Add(0x10); // boy up
            newEvent16D.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent16D.Add(0x02);
            newEvent16D.Add(0x10); // girl up
            newEvent16D.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent16D.Add(0x03);
            newEvent16D.Add(0x10); // sprite up
            newEvent16D.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent16D.Jsr(0x480); // shrug, some positioning thing
            newEvent16D.Jsr(0x798); // sound of the hole opening
            newEvent16D.IncrFlag(EventFlags.PANDORA_PHANNA_FLAG);
            newEvent16D.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent16D.Add(EventCommandEnum.INVIS_A.Value);
            newEvent16D.Door(0x32);
            newEvent16D.IncrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);
            newEvent16D.Jsr(0xF7); // shrug, some positioning thing
            newEvent16D.DecrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);
            newEvent16D.Jump(0x704); // boss theme
            newEvent16D.Add(EventCommandEnum.END.Value);


            // 0x1e7: spikey intro
            EventScript newEvent1E7 = new EventScript();
            context.replacementEvents[0x1E7] = newEvent1E7;
            newEvent1E7.Logic(EventFlags.WITCHCASTLE_FLAG, 0x4, 0xF, EventScript.GetJumpCmd(0)); // break out if already did this
            newEvent1E7.SetFlag(EventFlags.WITCHCASTLE_FLAG, 3); // set this because we removed the npc, due to spawning issues
            newEvent1E7.Add(EventCommandEnum.REFRESH_MAP.Value); // apply event flag 17
            newEvent1E7.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent1E7.Add(0x00);
            newEvent1E7.Add(0x20);
            newEvent1E7.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            // dyluck flash - probably not actually necessary
            for (int i = 0; i < 3; i++)
            {
                newEvent1E7.IncrFlag(EventFlags.WITCHCASTLE_FLAG);
                newEvent1E7.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                newEvent1E7.Add(EventCommandEnum.SLEEP_FOR.Value);
                newEvent1E7.Add(0x02);
                newEvent1E7.DecrFlag(EventFlags.WITCHCASTLE_FLAG);
                newEvent1E7.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                newEvent1E7.Add(EventCommandEnum.SLEEP_FOR.Value);
                newEvent1E7.Add(0x02);
            }
            newEvent1E7.IncrFlag(EventFlags.WITCHCASTLE_FLAG);
            newEvent1E7.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent1E7.IncrFlag(EventFlags.WITCHCASTLE_FLAG);
            newEvent1E7.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent1E7.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent1E7.Add(0x00);
            newEvent1E7.Add(0x20);
            newEvent1E7.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent1E7.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent1E7.Add(0x08); // center cam
            // i think this is the slow open of the spikey gate
            for (int i = 0; i < 3; i++)
            {
                newEvent1E7.Jsr(0x1F0);
                newEvent1E7.Add(EventCommandEnum.SLEEP_FOR.Value);
                newEvent1E7.Add(0x0C);
            }
            newEvent1E7.Door(0x13A); // spikey fight
            newEvent1E7.Jump(0x704); // boss theme
            newEvent1E7.End();


            // 0x1ed: elinee dialogue post spikey fight - show the two chests
            EventScript newEvent1ED = new EventScript();
            context.replacementEvents[0x1ED] = newEvent1ED;
            // elinee shriek
            newEvent1ED.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent1ED.Add(0x02); // elinee transform sound
            newEvent1ED.Add(0x87);
            newEvent1ED.Add(0x0F);
            newEvent1ED.Add(0x88);
            for (int i = 0; i < 3; i++)
            {
                newEvent1ED.IncrFlag(EventFlags.WITCHCASTLE_FLAG);
                newEvent1ED.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                newEvent1ED.Add(EventCommandEnum.SLEEP_FOR.Value);
                newEvent1ED.Add(0x01);
                newEvent1ED.DecrFlag(EventFlags.WITCHCASTLE_FLAG);
                newEvent1ED.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                newEvent1ED.Add(EventCommandEnum.SLEEP_FOR.Value);
                newEvent1ED.Add(0x01);
            }
            newEvent1ED.IncrFlag(EventFlags.WITCHCASTLE_FLAG);
            newEvent1ED.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            // show the two chests on the right
            newEvent1ED.IncrFlag(EventFlags.NEXT_TO_WHIP_CHEST_VISIBILITY_FLAG);
            newEvent1ED.IncrFlag(EventFlags.WHIP_CHEST_VISIBILITY_FLAG);
            newEvent1ED.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent1ED.Add(EventCommandEnum.END.Value);


            // 0x1f6: dyluck rant & luka message outside castle - remove all but keep the dyluck music
            EventScript newEvent1F6 = new EventScript();
            context.replacementEvents[0x1F6] = newEvent1F6;
            newEvent1F6.Jsr(0x7DA); // dyluck theme
            newEvent1F6.End();


            // 0x365: jema in dwarf town as tropicallo entryway
            EventScript newEvent365 = new EventScript();
            context.replacementEvents[0x365] = newEvent365;
            // jump to 0 if we already beat it
            newEvent365.Logic(EventFlags.GAIAS_NAVEL_SPRITE_FLAG, 0x8, 0xF, EventScript.GetJumpCmd(0));
            newEvent365.AddDialogueBoxWithChoices("Fight the Tropicallo?\n", new byte[] { 2, 8 }, new string[] { "Yep", "Nope" }, new byte[][] { EventScript.GetJumpCmd(0x1BE), EventScript.GetJumpCmd(0) }, EventScript.GetJumpCmd(0));
            newEvent365.End(); // end


            // 0x1be: warp to tropicallo arena if you hit yes above
            EventScript newEvent1BE = new EventScript();
            context.replacementEvents[0x1BE] = newEvent1BE;
            newEvent1BE.Door(0x51); // tropicallo fight
            newEvent1BE.Jump(0x704); // boss theme
            newEvent1BE.End();


            // 0x543: transition into lime slime arena - remove the text about shade testing you
            EventScript newEvent543 = new EventScript();
            context.replacementEvents[0x543] = newEvent543;
            newEvent543.Door(0x3A); // slime boss
            newEvent543.SetFlag(EventFlags.DEATH_TYPE_FLAG, 0xF);
            newEvent543.Jsr(0x7C4); // boss theme
            newEvent543.End();


            // 0x16a, 0x16b: don't lock the door leaving wall boss area
            EventScript newEvent16a = new EventScript();
            context.replacementEvents[0x16a] = newEvent16a;
            newEvent16a.Door(0x31);
            newEvent16a.End();

            EventScript newEvent16b = new EventScript();
            context.replacementEvents[0x16b] = newEvent16b;
            newEvent16b.Door(0x33);
            newEvent16b.End();


            // 0x134: luka saving - take out the 1B kilroy logic
            EventScript newEvent134 = new EventScript();
            context.replacementEvents[0x134] = newEvent134;
            newEvent134.SetFlag(0xFD, 0);
            newEvent134.SetFlag(0xFE, 2);
            newEvent134.OpenDialogueBox();
            newEvent134.AddDialogue("LUKA:I'll restore you.\n");
            newEvent134.Jsr(0x41B); // character movements
            newEvent134.Add(EventCommandEnum.HEAL.Value);
            newEvent134.Add(0x84);
            newEvent134.AddDialogueWithChoices("Want to save the game?\n   (", new byte[] { 0x5, 0xA }, new string[] { "Yes", "No  )" }, new byte[][] { new byte[] { 0x1F, 0x06 }, EventScript.GetJumpCmd(0x136) }, EventScript.GetJumpCmd(0x136));
            newEvent134.End();


            // 0x136: remove the dialogue following 0x134
            EventScript newEvent136 = new EventScript();
            context.replacementEvents[0x136] = newEvent136;
            newEvent136.End();


            // 0x1bd: girl trying to leave at gaia's navel - remove 
            EventScript newEvent1bd = new EventScript();
            context.replacementEvents[0x1bd] = newEvent1bd;
            newEvent1bd.End();


            // 0x231: always allow entrance to undine cave
            EventScript newEvent231 = new EventScript();
            context.replacementEvents[0x231] = newEvent231;
            newEvent231.Door(0x140); // doorway
            newEvent231.End();


            // 0x23a: smacking the first spell orb with a weapon has a message; remove it
            EventScript newEvent23a = new EventScript();
            context.replacementEvents[0x23a] = newEvent23a;
            newEvent23a.End();


            // 0x237: fire gigas death
            // don't increment A8 after fire gigas otherwise we can't find gnome
            EventScript newEvent237 = new EventScript();
            context.replacementEvents[0x237] = newEvent237;
            newEvent237.IncrFlag(EventFlags.EARTHPALACE_FLAG);
            newEvent237.Jsr(0x502); // orb
            newEvent237.Door(0xDE);
            newEvent237.Jsr(0x7D2); // music
            newEvent237.End();


            // x42d: thanatos death
            // some open world logic to figure out who should get mana magic
            EventScript newEvent42d = new EventScript();
            context.replacementEvents[0x42d] = newEvent42d;
            newEvent42d.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            newEvent42d.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            newEvent42d.Jsr(0x736); // music
            // if we don't have dryad, exit event
            newEvent42d.Logic(EventFlags.ELEMENT_DRYAD_FLAG, 0, 0, EventScript.GetJumpCmd(0));
            // otherwise give mana magic to whoever wants it
            string boyClass = context.workingData.get(OpenWorldClassSelection.BOY_CLASS);
            string girlClass = context.workingData.get(OpenWorldClassSelection.GIRL_CLASS);
            string spriteClass = context.workingData.get(OpenWorldClassSelection.SPRITE_CLASS);
            Dictionary<int, int> spellGrantValues = new Dictionary<int, int>();
            if (boyClass == "OGgirl")
            {
                spellGrantValues[1] = 0x38;
            }
            else if (boyClass == "OGsprite")
            {
                spellGrantValues[1] = 0x07;
            }
            if (girlClass == "OGgirl")
            {
                spellGrantValues[2] = 0x38;
            }
            else if (girlClass == "OGsprite")
            {
                spellGrantValues[2] = 0x07;
            }
            if (spriteClass == "OGgirl")
            {
                spellGrantValues[3] = 0x38;
            }
            else if (spriteClass == "OGsprite")
            {
                spellGrantValues[3] = 0x07;
            }
            foreach (int key in spellGrantValues.Keys)
            {
                newEvent42d.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                newEvent42d.Add((byte)key); // char
                newEvent42d.Add(0xCD); // dryad
                newEvent42d.Add((byte)spellGrantValues[key]); // all 3 spells
            }
            newEvent42d.End();


            // x4e1: thanatos finale
            EventScript newEvent4e1 = new EventScript();
            context.replacementEvents[0x4e1] = newEvent4e1;
            newEvent4e1.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            newEvent4e1.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            newEvent4e1.Door(0x2F); // lich boss fight
            newEvent4e1.Jump(0x70C); // lich fight theme
            newEvent4e1.End();


            // x429: before mana beast
            EventScript newEvent429 = new EventScript();
            context.replacementEvents[0x429] = newEvent429;
            newEvent429.Logic(EventFlags.MANAFORT_SWITCHES_FLAG_1, 0x0, 0x7, EventScript.GetJumpCmd(0));
            newEvent429.Door(0x3B); // before mana beast door
            newEvent429.Jsr(0x737); // music that plays for a couple seconds lol
            newEvent429.Jsr(0x739); // mana beast fight music
            newEvent429.Door(0xBC); // mana beast door
            newEvent429.SetFlag(EventFlags.DEATH_TYPE_FLAG, 0xF);
            newEvent429.End();


            // 0x578: sunken continent orbs - remove the text box
            EventScript newEvent578 = new EventScript();
            context.replacementEvents[0x578] = newEvent578;
            newEvent578.Jsr(0x782); // sound
            newEvent578.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            newEvent578.Jsr(0x798); // uhh another sound?
            newEvent578.End();


            // 0x36f: shorten kilroy intro
            EventScript newEvent36f = new EventScript();
            context.replacementEvents[0x36F] = newEvent36f;
            newEvent36f.Add(0x06);
            newEvent36f.IncrFlag(EventFlags.KILROY_EVENT_FLAG);
            newEvent36f.IncrFlag(EventFlags.KILROY_EVENT_FLAG);
            newEvent36f.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            newEvent36f.End();


            // 0x368: don't stop you from entering the kilroy area without the magic rope flag set
            EventScript newEvent368 = new EventScript();
            context.replacementEvents[0x368] = newEvent368;
            newEvent368.Door(0x1C2);
            newEvent368.Jump(0x720);  // music
            newEvent368.End();


            // 0x510: remove a bunch of krissie dialogue in northtown
            EventScript newEvent510 = new EventScript();
            context.replacementEvents[0x510] = newEvent510;
            newEvent510.End();


            // 0x3a9: remove a bunch more krissie dialogue in northtown
            EventScript newEvent3a9 = new EventScript();
            context.replacementEvents[0x3a9] = newEvent3a9;
            newEvent3a9.End();


            // 0x558: vampire boss fight intro - shorten
            EventScript newEvent558 = new EventScript();
            context.replacementEvents[0x558] = newEvent558;
            newEvent558.IncrFlag(EventFlags.NORTHTOWN_RUINS_FLAG);
            newEvent558.IncrFlag(EventFlags.NORTHTOWN_RUINS_FLAG);
            newEvent558.Door(0x318); // vampire boss fight
            newEvent558.Jump(0x704); // boss music
            newEvent558.Add(EventCommandEnum.END.Value);


            // 0x52e: lead-in to the mech rider 2 fight on the northtown roof
            EventScript newEvent52e = new EventScript();
            context.replacementEvents[0x52e] = newEvent52e;
            newEvent52e.Logic(EventFlags.NORTHTOWN_CASTLE_PROGRESS_FLAG, 0x5, 0xF, EventScript.GetJumpCmd(0)); // don't warp again
            newEvent52e.SetFlag(EventFlags.NORTHTOWN_CASTLE_PROGRESS_FLAG, 5);
            newEvent52e.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent52e.Door(0x35D); // mech rider 2 boss fight
            newEvent52e.Jump(0x704); // boss theme
            newEvent52e.End();


            // 0x621: walkon event for a few northtown spots - cut some stuff out
            EventScript newEvent621 = new EventScript();
            context.replacementEvents[0x621] = newEvent621;
            newEvent621.SetFlag(0xFD, 1); // ?
            newEvent621.SetFlag(0xFE, 1); // ?
            newEvent621.Jump(0x70D); // music
            newEvent621.End();


            // 0x27c: matango falling rock - remove dialogue box
            EventScript newEvent27c = new EventScript();
            context.replacementEvents[0x27c] = newEvent27c;
            newEvent27c.IncrFlag(EventFlags.MATANGO_PROGRESS_FLAG);
            newEvent27c.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh to remove the orb
            newEvent27c.Jsr(0x798); // sound effect
            newEvent27c.End();


            // 0x2c9: fire palace spell orb trigger - remove dialogue box
            EventScript newEvent2c9 = new EventScript();
            context.replacementEvents[0x2c9] = newEvent2c9;
            newEvent2c9.IncrFlag(EventFlags.FIRE_PALACE_COMPLETION_FLAG);
            newEvent2c9.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh to remove the orb
            newEvent2c9.Add(EventCommandEnum.REFRESH_MAP.Value); // refresh to update the map
            newEvent2c9.Add(0x2D); // palette flash:
            newEvent2c9.Add(0x07); // restore
            newEvent2c9.End();


            // 0x36d: kilroy intro - shorten
            EventScript newEvent36d = new EventScript();
            context.replacementEvents[0x36d] = newEvent36d;
            newEvent36d.Jsr(0x7C4); // boss theme
            newEvent36d.End();


            // 0x485: shorter inn stay animation
            EventScript newEvent485 = new EventScript();
            context.replacementEvents[0x485] = newEvent485;
            newEvent485.Jsr(0x797);
            newEvent485.Add(0x5F); // idk some money menu thing (refresh?)
            newEvent485.Sleep(0x08);
            newEvent485.Add(EventCommandEnum.CLOSE_GP.Value);
            newEvent485.Add(0x06);
            newEvent485.SetFlag(EventFlags.WALK_THROUGH_WALLS_FLAG, 1);
            newEvent485.Jsr(0x41B); // heal
            newEvent485.Add(EventCommandEnum.HEAL.Value);
            newEvent485.Add(0x84); // mp, too?
            newEvent485.SetFlag(EventFlags.WALK_THROUGH_WALLS_FLAG, 0);
            newEvent485.Add(0x07); // ?
            newEvent485.Jump(0x50); // save option
            newEvent485.End();


            // 0x23b: gnome intro
            EventScript newEvent23b = new EventScript();
            context.replacementEvents[0x23b] = newEvent23b;
            newEvent23b.Logic(EventFlags.EARTHPALACE_FLAG, 0x3, 0xF, EventScript.GetJumpCmd(0));
            // still jump to event 236, like in rando mode
            newEvent23b.Jump(0x236);
            newEvent23b.End();


            // 0x236: transition into fire gigas fight
            EventScript newEvent236 = new EventScript();
            context.replacementEvents[0x236] = newEvent236;
            newEvent236.IncrFlag(EventFlags.EARTHPALACE_FLAG);
            newEvent236.IncrFlag(EventFlags.EARTHPALACE_FLAG);
            newEvent236.IncrFlag(EventFlags.EARTHPALACE_FLAG);
            newEvent236.Door(0x14B); // door to fire gigas room
            newEvent236.Jump(0x704); // boss theme
            newEvent236.End();


            // 0x66b: frost gigas fight intro
            EventScript newEvent66b = new EventScript();
            context.replacementEvents[0x66b] = newEvent66b;
            newEvent66b.Door(0x2CA); // santa boss fight
            newEvent66b.Jump(0x704); // boss music
            newEvent66b.End();


            // 0x2b0: amar decision + default text
            EventScript newEvent2b0 = new EventScript();
            context.replacementEvents[0x2b0] = newEvent2b0;
            newEvent2b0.Logic(EventFlags.SEA_HARES_TAIL_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(0x2B2)); // if we got sea hare tail, jump to 0x2b2 (prize event)
            newEvent2b0.Logic(EventFlags.SEA_HARES_TAIL_FLAG, 0x2, 0xF, EventScript.GetJumpCmd(0x2B3)); // if we already got the reward, jump to 0x2b3
            // otherwise, ask for the sea hare tail
            newEvent2b0.AddDialogueBox("yo bring me a sea\nhare tail");
            newEvent2b0.End();


            // 0x2b4: the guy next to amar
            EventScript newEvent2b4 = new EventScript();
            context.replacementEvents[0x2b4] = newEvent2b4;
            newEvent2b4.Logic(EventFlags.SEA_HARES_TAIL_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(0x2BF)); // if we got sea hare tail, jump to 0x0x2bf, which eventually gets us the prize
            newEvent2b4.Logic(EventFlags.SEA_HARES_TAIL_FLAG, 0x2, 0xF, EventScript.GetJumpCmd(0x2BA)); // if we already got the prize, jump to 0x2ba
            // otherwise, ask for the sea hare tail
            newEvent2b4.AddDialogueBox("yo bring that guy a\nsea hare tail");
            newEvent2b4.End();


            // 0x2bf: the guy next to amar giving you the sea hare tail prize
            EventScript newEvent2bf = new EventScript();
            context.replacementEvents[0x2bf] = newEvent2bf;
            newEvent2bf.Add(0x32); // turn to look at the other dude
            newEvent2bf.Add(0x80);
            newEvent2bf.Add(0xC0);
            newEvent2bf.AddDialogueBox("hey check this out");
            // unconditional jump to amar's sea hare tail prize event
            newEvent2bf.Jump(0x2B2);
            newEvent2bf.End();


            // 0x2ba: the guy next to amar after you already got the sea hare tail prize. do you really have any reason to talk to him again?
            EventScript newEvent2ba = new EventScript();
            context.replacementEvents[0x2ba] = newEvent2ba;
            newEvent2ba.Add(0x32); // turn to look at the other dude
            newEvent2ba.Add(0x80);
            newEvent2ba.Add(0xC0);
            newEvent2ba.AddDialogueBox("blarf");
            newEvent2ba.End();


            // 0x2b3: amar after you got the sea hare tail prize. no amount of sea hare tails will ever be enough for him
            EventScript newEvent2b3 = new EventScript();
            context.replacementEvents[0x2b3] = newEvent2b3;
            newEvent2b3.AddDialogueBox("bring more");
            newEvent2b3.End();


            // 0x63d: remove text about the spell orb/heat in fire palace as well as the palette shift
            EventScript newEvent63D = new EventScript();
            context.replacementEvents[0x63D] = newEvent63D;
            newEvent63D.End();


            // 0x289: landing in the desert. remove vanilla dialogue and progression
            EventScript newEvent289 = new EventScript();
            context.replacementEvents[0x289] = newEvent289;
            newEvent289.End();


            // 0x381: boreal face death event; exit to the "cold" version of the town map instead of the warm one
            EventScript newEvent381 = new EventScript();
            context.replacementEvents[0x381] = newEvent381;
            newEvent381.Jsr(0x505); // orb reward
            newEvent381.SetFlag(EventFlags.SALAMANDO_STOVE_FLAG, 4); // allow stove to be opened
            newEvent381.Door(0x145); // door 145 instead of 214
            newEvent381.Jump(0x724); // song x724
            newEvent381.End();


            // 0x382: walking down into the ice country town; if we haven't fought boreal face yet, go there instead
            EventScript newEvent382 = new EventScript();
            context.replacementEvents[0x382] = newEvent382;
            newEvent382.Logic(EventFlags.SALAMANDO_STOVE_FLAG, 0x3, 0x3, EventScript.GetJumpCmd(0x37F)); // boreal face; 3 if haven't fought it yet, 4 if we have
            newEvent382.Jump(0x38B); // not boreal face
            newEvent382.End();


            // 0x380: similarly, for the door leading up to boreal face, go to town if we already beat it
            EventScript newEvent380 = new EventScript();
            context.replacementEvents[0x380] = newEvent380;
            newEvent380.Logic(EventFlags.SALAMANDO_STOVE_FLAG, 0x3, 0x3, EventScript.GetJumpCmd(0x37F)); // boreal face; 3 if haven't fought it yet, 4 if we have
            newEvent380.Jump(0x383); // not boreal face
            newEvent380.End();


            // 0x63e: minotaur fight entry
            EventScript newEvent63e = new EventScript();
            context.replacementEvents[0x63e] = newEvent63e;
            newEvent63e.Logic(EventFlags.FIRE_PALACE_COMPLETION_FLAG, 0x2, 0xF, EventScript.GetJumpCmd(0)); // if the fire palace flag is >= 2, minotaur is already dead, so break out (jump to 00)
            newEvent63e.Add(0x06);
            newEvent63e.Add(0x03);
            newEvent63e.SetFlag(EventFlags.FIRE_PALACE_COMPLETION_FLAG, 2); // set visibility flag for the boss (to 2)
            newEvent63e.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent63e.Jump(0x704);
            newEvent63e.End();


            // 0x3a3: skip event logic on password guy so we can have flag 5a
            EventScript newEvent3a3 = new EventScript();
            context.replacementEvents[0x3a3] = newEvent3a3;
            newEvent3a3.Jump(0x5B1); // unconditional jump to 0x5b1 (below)
            newEvent3a3.End();

            // 0x5b1: don't allow entry into the sewer
            // note that in "flammie drum in logic" mode where sometimes you do have to go in the sewer,
            // we make this guy invisible
            EventScript newEvent5b1 = new EventScript();
            context.replacementEvents[0x5b1] = newEvent5b1;
            newEvent5b1.AddDialogueBox("take my advice\ndon't go in here\nthis dungeon blows");
            newEvent5b1.End();


            // 0x4b0: allow free entry into the dryad palace
            EventScript newEvent4B0 = new EventScript();
            context.replacementEvents[0x4B0] = newEvent4B0;
            newEvent4B0.Logic(EventFlags.LOST_CONTINENT_HEXAS_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(0x704)); // haven't fought snap dragon yet? play boss music
            newEvent4B0.End();


            // 0x62b: restrict access to mana fort if the flag indicates we shouldn't go there yet
            EventScript newEvent62b = new EventScript();
            context.replacementEvents[0x62b] = newEvent62b;
            newEvent62b.Logic(EventFlags.OPENWORLD_MANAFORT_ACCESSIBLE_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(0x658)); // flag hasn't been set yet; can't get in - leave (see event below)
            newEvent62b.SetFlag(0xFD, 1); // nfi they're in the original
            newEvent62b.SetFlag(0xFE, 0xB); // nfi they're in the original
            newEvent62b.Jump(0x726); // manafort exterior song
            newEvent62b.End();


            // 0x658: leave mana fort if we're not ready to go there yet
            EventScript newEvent658 = new EventScript();
            context.replacementEvents[0x658] = newEvent658;
            // this originally has a horrendous screen flash effect that we cut out here
            newEvent658.AddDialogueBox("Finish Sunken Continent\nto access Mana Fort!");
            newEvent658.Add(EventCommandEnum.FLAMMIE_FROM_POS.Value);
            newEvent658.Add(0xFF);
            newEvent658.End();


            // 0xe1: kakkara cannon event
            EventScript newEventE1 = new EventScript();
            context.replacementEvents[0xE1] = newEventE1;
            newEventE1.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
            newEventE1.Jsr(0x408);
            newEventE1.Add(EventCommandEnum.OPEN_GP.Value);
            newEventE1.Add(0x5F);
            // vanilla-ish dialogue
            newEventE1.AddDialogueBoxWithChoices("50 GP! 1: Matango,\n2: Ice Ctry, 3: Southtown\n", new byte[] { 0x2, 0x7, 0xC, }, new string[] { "1", "2", "3" }, 
                new byte[][] { EventScript.GetJumpCmd(0xE7), EventScript.GetJumpCmd(0xE8), EventScript.GetJumpCmd(0xE9) }, EventScript.GetJumpCmd(0xFF)); // 0xFF exits but also closes the GP box
            newEventE1.End();


            // 0x4b8: snap dragon death - set the flag to 5 to allow watermelon guy to appear.  no prize because it unlocks the dryad room
            EventScript newEvent4b8 = new EventScript();
            context.replacementEvents[0x4b8] = newEvent4b8;
            // MOPPLE: doing "refresh objects" after setting the flag in this event doesn't make the watermelon guy appear .. unsure why
            newEvent4b8.SetFlag(EventFlags.LOST_CONTINENT_HEXAS_FLAG, 5);
            newEvent4b8.Jsr(0x507); // orb
            newEvent4b8.Jsr(0x78C); // music
            newEvent4b8.SetFlag(EventFlags.DRYAD_AREA_FLAG_1, 1); // open left/center paths in this room
            newEvent4b8.SetFlag(EventFlags.DRYAD_AREA_FLAG_2, 1);
            newEvent4b8.SetFlag(EventFlags.DRYAD_AREA_FLAG_3, 1);
            newEvent4b8.Add(0x0A);
            newEvent4b8.Jsr(0x7D9); // music
            newEvent4b8.End();


            // 0x4b5: add dialogue to sheex or whatever his name is in the snap dragon arena to warp to the watermelon fight
            EventScript newEvent4b5 = new EventScript();
            context.replacementEvents[0x4b5] = newEvent4b5;
            newEvent4b5.Logic(EventFlags.LOST_CONTINENT_WATERMELON_FLAG, 0x2, 0xF, EventScript.GetJumpCmd(0)); // jump to 0 with just no dialogue if we already beat it
            newEvent4b5.AddDialogueBoxWithChoices("Fight the Watermelon?\n", new byte[] { 2, 8 }, new string[] { "Yep", "Nope" }, new byte[][] { EventScript.GetJumpCmd(0x4B3), EventScript.GetJumpCmd(0) }, EventScript.GetJumpCmd(0));
            newEvent4b5.End(); // end


            // 0x4b3: warp to the watermelon fight, as requested by the above event
            EventScript newEvent4B3 = new EventScript();
            context.replacementEvents[0x4B3] = newEvent4B3;
            newEvent4B3.IncrFlag(EventFlags.LOST_CONTINENT_WATERMELON_FLAG);
            newEvent4B3.Door(0x358); // watermelon fight
            newEvent4B3.Jump(0x704); // boss theme
            newEvent4B3.End();


            // 0x4a1: walkon event for the hexas room - play fight music if flags indicate she's still alive
            EventScript newEvent4A1 = new EventScript();
            context.replacementEvents[0x4A1] = newEvent4A1;
            newEvent4A1.Logic(EventFlags.LOST_CONTINENT_HEXAS_FLAG, 0x6, 0xF, EventScript.GetJumpCmd(0));
            newEvent4A1.Jsr(0x7C4); // music
            newEvent4A1.End();


            // 0x195: pandora chest room guard tells you to do the ruins first; he won't be there after you do
            EventScript newEvent195 = new EventScript();
            context.replacementEvents[0x195] = newEvent195;
            newEvent195.AddDialogueBox("What no go do the ruins\nthing first");
            newEvent195.End();


            // 0x233: tonpole death - no prizes since it unlocks undine in the next room
            EventScript newEvent233 = new EventScript();
            context.replacementEvents[0x233] = newEvent233;
            newEvent233.Jsr(0x500); // orb
            newEvent233.Jsr(0x784); // music
            newEvent233.IncrFlag(EventFlags.OPEN_WORLD_TONPOLE_FLAG);
            newEvent233.Add(0x0A);
            newEvent233.Jump(0x70B); // music again?
            newEvent233.End();


            // 0x232: walkon event for tonpole map - play the boss music if he's still alive
            EventScript newEvent232 = new EventScript();
            context.replacementEvents[0x232] = newEvent232;
            newEvent232.Logic(EventFlags.OPEN_WORLD_TONPOLE_FLAG, 0, 0, EventScript.GetJumpCmd(0x704)); // boss theme
            newEvent232.End();


            // 0x610: walkon event for potos; play normal town music in potos instead of sad music
            EventScript newEvent610 = new EventScript();
            context.replacementEvents[0x610] = newEvent610;
            newEvent610.SetFlag(0xFD, 0); // no idea what this is or why
            newEvent610.SetFlag(0xFE, 0); // no idea what this is or why
            newEvent610.Jump(0x711); // town theme
            newEvent610.End();


            // 0x2c1: karon ferry entry from below - say the luna orb element
            EventScript newEvent2c1 = new EventScript();
            context.replacementEvents[0x2c1] = newEvent2c1;
            newEvent2c1.Door(0x240);
            newEvent2c1.Jsr(0x480);
            newEvent2c1.Jsr(0x78D);
            newEvent2c1.Jsr(0x7A0);
            newEvent2c1.Sleep(0x40);
            if (anySpellsAvailable)
            {
                newEvent2c1.AddDialogueBox("Today's orb element is:\n" + SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[35], false) + "!");
            }
            newEvent2c1.Door(0x278);
            newEvent2c1.End();


            // 0x2c2: karon ferry going the other way
            EventScript newEvent2c2 = new EventScript();
            context.replacementEvents[0x2c2] = newEvent2c2;
            newEvent2c2.Door(0x241);
            newEvent2c2.Jsr(0x480);
            newEvent2c2.Jsr(0x78D);
            newEvent2c2.Jsr(0x7A0);
            newEvent2c2.Sleep(0x40);
            newEvent2c2.AddDialogueBox("You know you can just\nFlammie from that\nlast map, right?");
            newEvent2c2.Door(0x279);
            newEvent2c2.End();


            // 0x51d: jump right to the metal mantis fight without all the shit that's supposed to happen first
            EventScript newEvent51d = new EventScript();
            context.replacementEvents[0x51d] = newEvent51d;
            newEvent51d.Door(0x345); // metal mantis boss fight
            newEvent51d.Jump(0x704); // boss theme
            newEvent51d.End();


            // 0x4a3: determine whether you can enter the mech rider 3 fight
            EventScript newEvent4a3 = new EventScript();
            context.replacementEvents[0x4a3] = newEvent4a3;
            foreach (byte lostContinentElementOrbFlag in EventFlags.lostContinentElementOrbFlags)
            {
                newEvent4a3.Logic(lostContinentElementOrbFlag, 0, 0, EventScript.GetJumpCmd(0x487));
            }
            newEvent4a3.Logic(EventFlags.OPENWORLD_MANAFORT_ACCESSIBLE_FLAG, 0x2, 0xF, EventScript.GetJumpCmd(0));
            newEvent4a3.Door(0x3F6); // mech rider 3 map
            newEvent4a3.Jsr(0x7C4); // music
            newEvent4a3.End();


            // 0x487: error condition event for the mech rider 3 check
            EventScript newEvent487 = new EventScript();
            context.replacementEvents[0x487] = newEvent487;
            newEvent487.AddDialogueBox("Complete the lower\nsection of the palace\nto proceed!");
            newEvent487.End();


            // 0x4c5: when you hit the button on the right side of the snap dragon room, cut out the vanilla automated animation that happens
            EventScript newEvent4c5 = new EventScript();
            context.replacementEvents[0x4c5] = newEvent4c5;
            newEvent4c5.Add(0x06);
            newEvent4c5.Add(0x03);
            newEvent4c5.Jsr(0x78C);
            newEvent4c5.IncrFlag(EventFlags.DRYAD_AREA_FLAG_3);
            newEvent4c5.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent4c5.End();


            // 0x4c8: shorten kettle kin intro event
            EventScript newEvent4c8 = new EventScript();
            context.replacementEvents[0x4c8] = newEvent4c8;
            newEvent4c8.Logic(EventFlags.LOST_CONTINENT_KETTLEKIN_FLAG, 0x3, 0xF, EventScript.GetJumpCmd(0));
            newEvent4c8.Jsr(0x7C4); // music
            newEvent4c8.SetFlag(EventFlags.LOST_CONTINENT_KETTLEKIN_FLAG, 2); // visibility
            newEvent4c8.Door(0x3C7); // boss door
            newEvent4c8.End();


            // 0x2d4: warp to the tasnica miniboss fight
            EventScript newEvent2d4 = new EventScript();
            context.replacementEvents[0x2d4] = newEvent2d4;
            newEvent2d4.SetFlag(EventFlags.TASNICA_FLAG, 3);
            newEvent2d4.Jump(0x2D0); // boss music and warp
            newEvent2d4.End();


            // 0x4d7: show a message when we don't have enough seeds for MTR
            if (goal == OpenWorldGoalProcessor.GOAL_MTR)
            {
                int manaSeedsRequired = context.workingData.getInt(OpenWorldMtrSeedNumSelection.MANA_SEEDS_REQUIRED);
                EventScript newEvent4d7 = new EventScript();
                context.replacementEvents[0x4d7] = newEvent4d7;
                if (manaSeedsRequired == 8)
                {
                    newEvent4d7.AddDialogueBox("Return with all of the\nMana seeds to complete\nthe run!");
                }
                else
                {
                    newEvent4d7.AddDialogueBox("Return with " + manaSeedsRequired + " of the\nMana seeds to complete\nthe run!");
                }
                newEvent4d7.End();
            }


            // 0x62a: if we're in MTR mode, check current number of seeds when we arrive back at the purelands ending
            EventScript newEvent62a = new EventScript();
            context.replacementEvents[0x62a] = newEvent62a;
            if (goal == OpenWorldGoalProcessor.GOAL_MTR)
            {
                // this walkon is for all of purelands, so only run it if we've completed it
                newEvent62a.Logic(EventFlags.PURELAND_PROGRESS_FLAG, 0x8, 0xF, EventScript.GetJumpCmd(0x4D8));
            }
            newEvent62a.Jump(0x727); // pure lands music if we haven't finished pure lands
            newEvent62a.End();


            // 0x4d8: repurpose the pure lands intro message as a check for whether you finished MTR mode
            if (goal == OpenWorldGoalProcessor.GOAL_MTR)
            {
                int manaSeedsRequired = context.workingData.getInt(OpenWorldMtrSeedNumSelection.MANA_SEEDS_REQUIRED);
                EventScript newEvent4d8 = new EventScript();
                context.replacementEvents[0x4d8] = newEvent4d8;
                // use FC - total mana power
                // 0 -> required seeds minus 1 - jump out to 0x4d7 (above), which tells you to bring N seeds
                newEvent4d8.Logic(EventFlags.TOTAL_MANA_POWER_FLAG, 0, (byte)(manaSeedsRequired - 1), EventScript.GetJumpCmd(0x4D7));
                newEvent4d8.AddDialogueBox("Neat\nyou did the thing");
                newEvent4d8.Jump(0x42F); // credits
                newEvent4d8.End();
            }


            // 0x626: remove setting flag 0x0b so we get real deaths at jehk trials
            EventScript newEvent626 = new EventScript();
            context.replacementEvents[0x626] = newEvent626;
            newEvent626.SetFlag(0xFD, 1); // ?
            newEvent626.SetFlag(0xFE, 6); // ?
            newEvent626.Jump(0x713); // music
            newEvent626.End();


            // 0x637: remove setting flag 0x0b so we get real deaths at santa
            EventScript newEvent637 = new EventScript();
            context.replacementEvents[0x637] = newEvent637;
            newEvent637.Return();
            newEvent637.End();


            // 0x579: event to check whether you can walk back in the kettlekin door from the outside
            EventScript newEvent579 = new EventScript();
            context.replacementEvents[0x579] = newEvent579;
            newEvent579.Logic(EventFlags.LOST_CONTINENT_KETTLEKIN_FLAG, 0x0, 0x2, EventScript.GetJumpCmd(0x57A)); // error dialogue if progress isn't far enough
            newEvent579.Door(0x3E0); // otherwise execute the door
            newEvent579.End();


            // 0x57a: event to run when you can't get in the kettlekin door from the outside
            EventScript newEvent57A = new EventScript();
            context.replacementEvents[0x57A] = newEvent57A;
            newEvent57A.AddDialogueBox("no don't go in this way");
            newEvent57A.End();


            // 0x4c6: don't increment flag x44 if coming from lower sunken continent, since snap dragon depends on it being 01 from the start now
            EventScript newEvent4c6 = new EventScript();
            context.replacementEvents[0x4c6] = newEvent4c6;
            newEvent4c6.Jsr(0x78C); // sound?
            newEvent4c6.IncrFlag(EventFlags.LOST_CONTINENT_SWITCHES_FLAG_5); // allow passage
            newEvent4c6.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent4c6.End();


            // 0x51f: make the northtown castle gate button on the right side do nothing, since the gate is now always open
            EventScript newEvent51f = new EventScript();
            context.replacementEvents[0x51f] = newEvent51f;
            newEvent51f.End();


            // 0x547: transition to lime slime via modified flag values, or to shade area if you've been there already
            EventScript newEvent547 = new EventScript();
            context.replacementEvents[0x547] = newEvent547;
            newEvent547.Logic(EventFlags.SHADE_PALACE_PROGRESS_FLAG_1, 0x0, 0x5, EventScript.GetJumpCmd(0x543));
            newEvent547.Door(0x286);
            newEvent547.Jump(0x542);
            newEvent547.End();


            // 0x544: lime slime death event - set our modified flag for completion.  no prize here since it unlocks shade.
            EventScript newEvent544 = new EventScript();
            context.replacementEvents[0x544] = newEvent544;
            newEvent544.SetFlag(EventFlags.SHADE_PALACE_PROGRESS_FLAG_1, 6);
            newEvent544.Jsr(0x7C0);
            newEvent544.Door(0x286);
            newEvent544.Jsr(0x507);
            newEvent544.Jump(0x542);
            newEvent544.End();


            // 0x642: modify lumina map walkon event to not automatically set the progress flag
            EventScript newEvent642 = new EventScript();
            context.replacementEvents[0x642] = newEvent642;
            newEvent642.Jsr(0x7D2);
            newEvent642.End();

            return true;
        }
    }
}
