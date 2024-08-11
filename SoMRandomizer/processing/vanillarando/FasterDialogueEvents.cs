using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.vanillarando
{
    /// <summary>
    /// Processing of vanilla events for vanilla rando mode, including shortening them to make the run faster.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FasterDialogueEvents : RandoProcessor
    {
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            List<int> newElementMapping = context.workingData.getIntArray(ElementSwaps.VANILLARANDO_ELEMENTLIST).ToList();
            if(settings.getBool(VanillaRandoSettings.PROPERTYNAME_DIALOGUE_CUTS))
            {
                processShortening(origRom, outRom, context.randomFunctional, newElementMapping, context);
            }
            else
            {
                processNoShortening(outRom, newElementMapping, context);
                // fix mana tree destruction scrolling for scroll hack
                outRom[0xa7a6f] = 0x14;
            }
            return true;
        }

        protected override string getName()
        {
            return "Dialogue shortening for vanilla rando";
        }

        public void processNoShortening(byte[] rom, List<int> newElementMapping, RandoContext context)
        {
            // 0x1e7: spikey intro .. adjust the flag
            EventScript newEvent1E7 = new EventScript();
            context.replacementEvents[0x1E7] = newEvent1E7;
            newEvent1E7.Logic(EventFlags.WITCHCASTLE_FLAG, 0x4, 0xF, EventScript.GetJumpCmd(0));
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
                newEvent1E7.Sleep(2);
                newEvent1E7.DecrFlag(EventFlags.WITCHCASTLE_FLAG);
                newEvent1E7.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                newEvent1E7.Sleep(2);
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
                newEvent1E7.Sleep(0xC);
            }
            newEvent1E7.Door(0x13A); // spikey fight
            newEvent1E7.Jump(0x704); // boss theme
            newEvent1E7.End();

            // now elements - make sure they give you the randomized ones
            List<int> attributeValues = new int[] { 0xC8, 0xC9, 0xCA, 0xCB, 0xCF, 0xCE, 0xCC, 0xCD }.ToList();

            List<int> newAttributeIds = new List<int>();
            List<int> newAttributeValuesGirl = new List<int>();
            List<int> newAttributeValuesSprite = new List<int>();
            // gnome, undine, sala, sylph, lumina, shade, luna, dryad
            for(int i=0; i < 8; i++)
            {
                int newAttribId = attributeValues[newElementMapping[i]];
                newAttributeIds.Add(newAttribId);
                int girlValue = 0x38;
                int spriteValue = 0x07;
                if(newAttribId == 0xCD)
                {
                    // dryad
                    girlValue = 0x18;
                    spriteValue = 0x03;
                }
                else if(newAttribId == 0xCE)
                {
                    // shade
                    girlValue = 0;
                }
                else if (newAttribId == 0xCF)
                {
                    // lumina
                    spriteValue = 0;
                }
                newAttributeValuesGirl.Add(girlValue);
                newAttributeValuesSprite.Add(spriteValue);
            }
        }

        public void processShortening(byte[] origRom, byte[] rom, Random r, List<int> newElementMapping, RandoContext context)
        {
            List<string> elementNames = new List<string>();
            elementNames.Add("Gnome");
            elementNames.Add("Undine");
            elementNames.Add("Salamando");
            elementNames.Add("Sylphid");
            elementNames.Add("Lumina");
            elementNames.Add("Shade");
            elementNames.Add("Luna");
            elementNames.Add("Dryad");


            // ////////////////////////////////////////////////////
            // x81 - water palace seed/jabberwocky dialogue
            // ////////////////////////////////////////////////////
            // from x84
            EventScript newEvent81 = new EventScript();
            context.replacementEvents[0x81] = newEvent81;
            newEvent81.SetFlag(EventFlags.WATER_SEED, 2);
            newEvent81.Door(0xA0); // jabberwocky
            newEvent81.Jsr(0x6EB);  // nfi but it sets A4 to 0F so i can't go back down the steps
            newEvent81.Jump(0x704); // boss theme
            newEvent81.End();


            // x212 - post jabberwocky
            EventScript newEvent212 = new EventScript();
            context.replacementEvents[0x212] = newEvent212;
            newEvent212.Logic(EventFlags.WATERPALACE_JEMADIALOGUE_FLAG, 0x1, 0xF, EventScript.GetJumpCmd(0x213));
            // stuff from jabberwocky death event 20c
            newEvent212.Jsr(0x505); // orb reward
            newEvent212.SetFlag(EventFlags.WATER_PALACE_FLAG, 0xE);
            // 1E gets set later
            newEvent212.SetFlag(EventFlags.WATER_PALACE_HOSTILITY_FLAG, 0);
            newEvent212.Jsr(0x6F6); // ?
            newEvent212.Door(0xA1); // jabberwocky out
            newEvent212.Jsr(0x7C3); // music?
            // stuff from jema dialogue 212
            newEvent212.Jsr(0x504); // orb reward
            newEvent212.IncrFlag(EventFlags.WATERPALACE_JEMADIALOGUE_FLAG); // one-time event
            newEvent212.AddDialogueBox("Restored Water seed.\nNow take the Potos cannon\nto the Upper Land.");
            // stuff from 12E - seed touch
            newEvent212.SetFlag(EventFlags.WATER_SEED, 1);
            // stuff from 13D, 13E - luka talk event
            newEvent212.SetFlag(EventFlags.UPPERLAND_UNLOCK_FLAG, 7); // idk story shit maybe
            newEvent212.SetFlag(EventFlags.DOOR_CONTROL_FLAG_1, 1); // steps avail
            newEvent212.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent212.End();


            EventScript newEvent20C = new EventScript();
            context.replacementEvents[0x20c] = newEvent20C;
            newEvent20C.Jump(0x212);
            newEvent20C.End();

            // x100 - reference to x400, used on map 8


            // ////////////////////////////////////////////////////
            // x101 - waterfall post-fall dialogue
            // ////////////////////////////////////////////////////
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


            // ////////////////////////////////////////////////////
            // x102 - intro walk-over sword dialogue
            // ////////////////////////////////////////////////////
            EventScript newEvent102 = new EventScript();
            context.replacementEvents[0x102] = newEvent102;
            newEvent102.End();


            // ////////////////////////////////////////////////////
            // x103 - intro sword pull dialogue
            // ////////////////////////////////////////////////////
            EventScript newEvent103 = new EventScript();
            context.replacementEvents[0x103] = newEvent103;

            // /////////////////////
            // MOPPLE: should be a supporting method on EventScript for these SET_CHARACTER_ATTRIBUTE things
            newEvent103.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            newEvent103.Add(0x00);
            newEvent103.Add(0x90);
            newEvent103.Add(0x01);
            newEvent103.Add(EventCommandEnum.PLAYER_ANIM.Value);
            newEvent103.Add(0x00);
            newEvent103.Add(0xAD);
            newEvent103.Sleep(0x08);
            newEvent103.IncrFlag(EventFlags.POTOS_FLAG);
            newEvent103.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent103.IncrFlag(EventFlags.UI_DISPLAY_FLAG);
            newEvent103.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            newEvent103.Add(0x00);
            newEvent103.Add(0x90);
            newEvent103.Add(0x00);
            newEvent103.AddDialogueBox(VanillaEventUtil.BOY_NAME_INDICATOR + ": Neat, a sword!");
            newEvent103.IncrFlag(EventFlags.TEMPORARY_DIALOGUE_CHOICE_FLAG);
            newEvent103.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent103.DecrFlag(EventFlags.TEMPORARY_DIALOGUE_CHOICE_FLAG);
            newEvent103.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent103.IncrFlag(EventFlags.UI_DISPLAY_FLAG);
            newEvent103.Jsr(0x5F);
            newEvent103.IncrFlag(EventFlags.SWORD_ORBS_FLAG_B);

            newEvent103.End();


            // ////////////////////////////////////////////////////
            // x104 - intro walk-over sword dialogue
            // ////////////////////////////////////////////////////
            EventScript newEvent104 = new EventScript();
            context.replacementEvents[0x104] = newEvent104;
            newEvent104.End();


            // ////////////////////////////////////////////////////
            // x105 - rabite?
            // ////////////////////////////////////////////////////
            EventScript newEvent105 = new EventScript();
            context.replacementEvents[0x105] = newEvent105;
            List<List<byte>> rabiteDialogues = new List<List<byte>>();
            newEvent105.Logic(EventFlags.POTOS_FLAG, 0x3, 0xF, EventScript.GetJumpCmd(0));
            newEvent105.SetFlag(EventFlags.POTOS_FLAG, 3); // only show the rabite dialogue once
            newEvent105.Jsr(0x748); // song fade out
            newEvent105.Jump(0x70E); // song
            newEvent105.End();


            // ////////////////////////////////////////////////////
            // x106 - waterfall intro
            // ////////////////////////////////////////////////////
            EventScript newEvent106 = new EventScript();
            context.replacementEvents[0x106] = newEvent106;
            newEvent106.Jsr(0x73D);
            newEvent106.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent106.Add(0x01);
            newEvent106.Add(0x1C);
            newEvent106.Add(0x4A);
            newEvent106.Add(0x2F);
            newEvent106.Jsr(0x78D);
            newEvent106.Add(0x06); // idk
            // keep doing movements but skip the dialogue
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x00);
            newEvent106.Add(0x03);
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x04);
            newEvent106.Add(0xC8);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x00);
            newEvent106.Add(0xC0);
            // boy: hey!
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x04);
            newEvent106.Add(0xC8);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            // guys!
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x04);
            newEvent106.Add(0xC8);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            // wait
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x04);
            newEvent106.Add(0xC8);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            // up!
            newEvent106.SetFlag(EventFlags.WATERFALL_BOY_VISIBILITY_FLAG, 1);
            newEvent106.Sleep(0x10);
            newEvent106.Add(EventCommandEnum.INVIS_B.Value);
            newEvent106.Add(0x09); // idk

            newEvent106.Add(EventCommandEnum.PLAYER_ANIM.Value);
            newEvent106.Add(0x00);
            newEvent106.Add(0x88);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent106.Add(EventCommandEnum.PLAYER_ANIM.Value);
            newEvent106.Add(0x00);
            newEvent106.Add(0x88);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            // bunch of dialogue skipped here

            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x00);
            newEvent106.Add(0x00);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent106.Add(EventCommandEnum.CHARACTER_ANIM.Value);
            newEvent106.Add(0x00);
            newEvent106.Add(0x88);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);


            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x05);
            newEvent106.Add(0x80);
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x06);
            newEvent106.Add(0x80);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent106.SetFlag(EventFlags.WALK_THROUGH_WALLS_FLAG, 4); // nfi why 4

            // falling animation
            newEvent106.Add(EventCommandEnum.PLAYER_ANIM.Value);
            newEvent106.Add(0x00);
            newEvent106.Add(0xA0);

            newEvent106.Sleep(0x0C);

            // watch the boy character fall, do nothing
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x05);
            newEvent106.Add(0x42);
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x06);
            newEvent106.Add(0x42);
            newEvent106.Jsr(0x748); // fade out music
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent106.Add(EventCommandEnum.INVIS_A.Value);
            newEvent106.SetFlag(EventFlags.WALK_THROUGH_WALLS_FLAG, 0);
            newEvent106.Sleep(0x10);

            // face each other.  shrug.
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x05);
            newEvent106.Add(0x80);
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x06);
            newEvent106.Add(0xC0);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent106.Sleep(0x0C);

            string[] introDialogues = new string[] {
            "Who was that idiot?",
            "Look for me later at the\nMantis Ant fight",
            "And then there were two",
            "Let's see what's to\nthe left",
            "Thanks for choosing Secret\nof Mana Randomizer",
            "Blame Moppleton for\nany softlocks :3",
            "Don't forget speedup\nspell is useful now",
            "Evil Gate works on\nbosses now!",
            };
            string introDialogue = introDialogues[r.Next() % introDialogues.Length];
            newEvent106.AddAutoTextDialogueBox(introDialogue, 0x0A);

            // oh well, let's go back
            newEvent106.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent106.Add(0x05);
            newEvent106.Add(0xC0);
            newEvent106.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent106.Add(EventCommandEnum.CHARACTER_ANIM.Value);
            newEvent106.Add(0x05);
            newEvent106.Add(0x82);
            newEvent106.Add(EventCommandEnum.CHARACTER_ANIM.Value);
            newEvent106.Add(0x06);
            newEvent106.Add(0x82);
            newEvent106.Sleep(0x04);
            newEvent106.Door(0x82);
            newEvent106.Logic(EventFlags.UI_DISPLAY_FLAG, 0, 0, EventScript.GetJumpCmd(0x101));
            newEvent106.Add(EventCommandEnum.INVIS_B.Value);
            newEvent106.End();


            // ////////////////////////////////////////////////////
            // x10c - elder -> mantis ant
            // ////////////////////////////////////////////////////
            EventScript newEvent10C = new EventScript();
            context.replacementEvents[0x10c] = newEvent10C;
            newEvent10C.SetFlag(EventFlags.POTOS_FLAG, 5);
            newEvent10C.Add(0x06); // idk
            newEvent10C.Add(EventCommandEnum.MOVE_EVERYONE_TO_P1.Value);
            newEvent10C.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent10C.Add(0x08);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x00);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x06);
            newEvent10C.Add(0x40);
            newEvent10C.Add(0x06); // idk
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x90);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x05);
            newEvent10C.Add(0x40);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x06);
            newEvent10C.Add(0x90);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x4E);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x06);
            newEvent10C.Add(0x40);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            // stuff removed

            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x05);
            newEvent10C.Add(0x40);
            // slap fight!
            newEvent10C.AddAutoTextDialogueBox("Slap fight!", 5);

            newEvent10C.Jsr(0x787); // smacking sound, i think
            newEvent10C.Add(EventCommandEnum.PLAYER_ANIM.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x8C);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value); // push downward
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x50);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.Jsr(0x787); // smacking sound, i think
            newEvent10C.Add(EventCommandEnum.PLAYER_ANIM.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x8C);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value); // push downward
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x50);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.Jsr(0x787); // smacking sound, i think
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x44);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x00);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0xD8);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x56);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x80);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0xC0);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            // it's your fault!
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0xD8);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0xD8);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x80);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0xD8);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x58);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0xD8);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x00);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0xC8);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x52);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x58);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x00);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x60);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x00);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x44);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            // please stop

            newEvent10C.Jsr(0x785); // earthquake sound
            newEvent10C.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent10C.Add(0x02);
            newEvent10C.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent10C.Add(0x08);
            newEvent10C.Add(EventCommandEnum.MOVE_EVERYONE_TO_P1.Value);
            newEvent10C.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent10C.Add(0x03);

            // elliot running in circles
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x90);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0xE0);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x50);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x40);
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(0x90);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent10C.IncrFlag(EventFlags.POTOS_FLAG);
            newEvent10C.Jsr(0x798); // hole sound?
            newEvent10C.Jsr(0x748); // fade out music?
            newEvent10C.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0x00);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent10C.Add(EventCommandEnum.REFRESH_MAP.Value); // open the hole
            newEvent10C.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // delete elliot

            newEvent10C.Add(EventCommandEnum.PLAYER_ANIM.Value);
            newEvent10C.Add(0x00);
            newEvent10C.Add(0xAE);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent10C.Add(EventCommandEnum.INVIS_A.Value);
            // ahhhh

            newEvent10C.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent10C.Add(0x04);
            newEvent10C.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent10C.Door(0x8D); // mantis ant map
            newEvent10C.Add(EventCommandEnum.INVIS_B.Value);
            newEvent10C.IncrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);
            newEvent10C.Jsr(0x7C4); // boss theme
            newEvent10C.SetFlag(EventFlags.DEATH_TYPE_FLAG, 1);
            newEvent10C.DecrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);
            newEvent10C.Add(0x07); //idk
            newEvent10C.End();


            // x112 - after mantis ant
            EventScript newEvent112 = new EventScript();
            context.replacementEvents[0x112] = newEvent112;
            newEvent112.Jsr(0x501); // sword orb
            newEvent112.SetFlag(EventFlags.DEATH_TYPE_FLAG, 2);
            newEvent112.Add(EventCommandEnum.PLAY_SOUND.Value); // town theme
            newEvent112.Add(0x01);
            newEvent112.Add(0x14);
            newEvent112.Add(0x00);
            newEvent112.Add(0xFF);
            newEvent112.SetFlag(EventFlags.POTOS_FLAG, 7); // idk probably the way stuff looks on potos map
            newEvent112.Door(0x8E); // leave hole
            // a few random jema dialogues post-fight
            string[] jemaDialogues = new string[] {
                "JEMA: Try not to fall in\nany more holes.",
                "JEMA: Wouldn't it be\nneat if I was playable",
                "JEMA: I'd help you save\nthe world but I'm busy",
            };
            string jemaDialogue = jemaDialogues[r.Next() % jemaDialogues.Length];
            newEvent112.AddDialogueBox(jemaDialogue);
            newEvent112.SetFlag(EventFlags.POTOS_FLAG, 9);
            newEvent112.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent112.End();


            // 1E1 can be reused since i remove the witch in that spot
            // 10E is timothy's post mantis ant dialogue, maybe he should say something dumb
            EventScript newEvent10E = new EventScript();
            context.replacementEvents[0x10e] = newEvent10E;
            newEvent10E.Jump(0x1E1);
            newEvent10E.End();


            // timothy says random stupid shit
            EventScript newEvent1E1 = new EventScript();
            context.replacementEvents[0x1e1] = newEvent1E1;
            string[] timothyDialogues = new string[] {
                "You disgust me",
                "Pointy hats are so\nin right now",
                "Dude go put that\nsword back",
                "You know they're gonna\nmake me fill in that\nhole",
                "Thanks that hole looks\nso nice over there",
                "I figured a rabite\nate you",
                "I also have randomized\ndialogue!",
            };
            string timothyDialogue = timothyDialogues[r.Next() % timothyDialogues.Length];
            newEvent1E1.AddDialogueBox(timothyDialogue);
            newEvent1E1.End();


            // x120 - dyluck stuff near water palace
            // x126 - intro sword dialogue

            // ////////////////////////////////////////////////////
            // x127, x13A, x200, x201, x202, x203, x204 - luka dialogue .. also x130
            ////////////////////////////////////////////////////
            EventScript newEvent127 = new EventScript();
            context.replacementEvents[0x127] = newEvent127;
            newEvent127.Jump(0x204);
            newEvent127.End();


            EventScript newEvent204 = new EventScript();
            context.replacementEvents[0x204] = newEvent204;

            // allow walking through walls
            newEvent204.IncrFlag(EventFlags.WALK_THROUGH_WALLS_FLAG);
            newEvent204.IncrFlag(EventFlags.DOOR_CONTROL_FLAG_3);

            newEvent204.Jsr(0x791); // sound?
            newEvent204.Jsr(0x78C); // sound?

            newEvent204.Add(EventCommandEnum.REFRESH_MAP.Value);

            newEvent204.SetFlag(EventFlags.WATER_PALACE_FLAG, 0xB);
            newEvent204.SetFlag(EventFlags.PANDORA_RUINS_FLAG, 0x1);
            newEvent204.SetFlag(EventFlags.WATER_SEED, 0x1);
            newEvent204.IncrFlag(EventFlags.DOOR_CONTROL_FLAG_2);

            newEvent204.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent204.Add(0x00);
            newEvent204.Add(0x38);

            newEvent204.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent204.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent204.Add(0x00);
            newEvent204.Add(0x38);

            newEvent204.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent204.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent204.Add(0x08); // center camera

            newEvent204.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent204.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent204.Add(0x00);
            newEvent204.Add(0xD0); // left 10

            newEvent204.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

            newEvent204.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent204.Add(0x00);
            newEvent204.Add(0x00);//0x1C); // up the platform

            newEvent204.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            // 98f2a
            newEvent204.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent204.Add(0x00); // screen flash
            newEvent204.Add(EventCommandEnum.SLEEP_FOR.Value);
            newEvent204.Add(0x01);
            newEvent204.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent204.Add(0x01); // screen flash off

            newEvent204.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent204.Add(0x05); // palette change
            newEvent204.Add(0x69);
            newEvent204.Add(0x76);
            newEvent204.Jsr(0x782); // sound?
            newEvent204.Sleep(0x10);
            newEvent204.Jsr(0x782); // sound?
            newEvent204.Sleep(0x10);
            newEvent204.Jsr(0x782); // sound?
            newEvent204.Sleep(0x10);
            newEvent204.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent204.Add(0x07); // palette change back
            newEvent204.Jsr(0x594); // mana power increment?
            newEvent204.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
            // MOPPLE: the values for these should be constants somewhere
            newEvent204.Add(0x9B); // spear
            newEvent204.IncrFlag(EventFlags.SPEAR_ORBS_FLAG_B);

            newEvent204.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent204.Add(0x01);
            newEvent204.Add(0x2F);
            newEvent204.Add(0x0A);
            newEvent204.Add(0xFF);

            newEvent204.AddDialogueBox("Got the seed's power, and\nthe Spear!\nGo to Gaia's Navel.");

            newEvent204.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent204.Add(0x01);
            newEvent204.Add(0x1D);
            newEvent204.Add(0x1B);
            newEvent204.Add(0x8F);

            newEvent204.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent204.SetFlag(0xFD, 0); // nfi
            newEvent204.SetFlag(0xFE, 2); // nfi

            // disallow walk through walls
            newEvent204.DecrFlag(EventFlags.WALK_THROUGH_WALLS_FLAG);
            newEvent204.End();


            // object [2] (Luka A) on map 139 needs to move from y=16 to y=20
            // object [4] (Jema A) should be invisible - 00, FF
            // object [5] (Jema B) should be invisible - 00, FF
            int waterPalaceMapNum = MAPNUM_WATERPALACE_LUKA_DIALOGUE;
            int mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, waterPalaceMapNum);
            mapObjOffset += 8; // skip header
            rom[mapObjOffset + 8 * 2 + 3] += 4; // adjust y pos of obj 2
            rom[mapObjOffset + 8 * 4 + 0] = 0; // adjust event data of obj 4
            rom[mapObjOffset + 8 * 4 + 1] = 0xFF; // adjust event data of obj 4
            rom[mapObjOffset + 8 * 5 + 0] = 0; // adjust event data of obj 5
            rom[mapObjOffset + 8 * 5 + 1] = 0xFF; // adjust event data of obj 5


            // x146 [disappearing], x167 [ruins entrance] - phanna
            EventScript newEvent146 = new EventScript();
            context.replacementEvents[0x146] = newEvent146;
            newEvent146.Logic(EventFlags.ELEMENT_GNOME_FLAG, 0, 0, EventScript.GetJumpCmd(0));
            newEvent146.SetFlag(EventFlags.PANDORA_RUINS_FLAG, 7);
            newEvent146.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // force update
            newEvent146.Jsr(0x793); // i think the disappearing sound
            newEvent146.End();


            EventScript newEvent167 = new EventScript();
            context.replacementEvents[0x167] = newEvent167;
            newEvent167.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent167.Add(0x80);
            newEvent167.Add(0x3F);
            newEvent167.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent167.IncrFlag(EventFlags.PANDORA_RUINS_FLAG); // phanna visibility
            newEvent167.IncrFlag(EventFlags.PANDORA_PHANNA_FLAG); // guard visibility
            newEvent167.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // update to make her disappear
            newEvent167.End();


            // x168 - "Watch out!" when you walk in ruins
            EventScript newEvent168 = new EventScript();
            context.replacementEvents[0x168] = newEvent168;
            newEvent168.IncrFlag(EventFlags.PANDORA_PHANNA_FLAG); // one-time event
            newEvent168.Add(0x06);
            newEvent168.End();


            // ////////////////////////////////////////////////////
            // x16d, x16e, x173 - thanatos at ruins
            // ////////////////////////////////////////////////////

            // thanatos intro, start of wall fight
            EventScript newEvent16D = new EventScript();
            context.replacementEvents[0x16d] = newEvent16D;
            newEvent16D.Add(0x06); // ?
            newEvent16D.IncrFlag(EventFlags.PANDORA_PHANNA_FLAG); // used to create phanna, dyluck
            newEvent16D.IncrFlag(EventFlags.PANDORA_PHANNA_FLAG); // used to create phanna, dyluck
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
            newEvent16D.IncrFlag(EventFlags.PANDORA_PHANNA_FLAG); // used to create phanna, dyluck
            newEvent16D.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent16D.Add(EventCommandEnum.INVIS_A.Value);
            newEvent16D.Door(0x32); // wall boss map
            newEvent16D.IncrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);
            newEvent16D.Jsr(0xF7); // shrug, some positioning thing
            newEvent16D.DecrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);
            newEvent16D.Jump(0x704); // boss theme
            newEvent16D.End();


            // 16e - wall death event + thanatos dialogue
            EventScript newEvent16E = new EventScript();
            context.replacementEvents[0x16e] = newEvent16E;
            newEvent16E.SetFlag(EventFlags.PANDORA_PHANNA_FLAG, 7);
            newEvent16E.Jsr(0x505); // weapon orb
            newEvent16E.Jsr(0x7C3); // music?
            // combine with x173 - killing the wall just makes you warp out to fixed pandora
            newEvent16E.SetFlag(EventFlags.PANDORA_RUINS_FLAG, 9);
            newEvent16E.SetFlag(EventFlags.PANDORA_GIRL_FLAG, 7);
            newEvent16E.IncrFlag(EventFlags.PANDORA_PHANNA_FLAG);
            newEvent16E.IncrFlag(EventFlags.KILROY_EVENT_FLAG);
            newEvent16E.IncrFlag(EventFlags.GAIAS_NAVEL_SPRITE_FLAG);
            newEvent16E.SetFlag(EventFlags.WATER_SEED, 0xF);
            newEvent16E.Door(0x25); // out of wall boss map
            newEvent16E.AddDialogueBox("Now go do Kilroy in\nGaia's Navel.");
            newEvent16E.Jump(0x613); // music?
            newEvent16E.End();


            // ////////////////////////////////////////////////////
            // x186, x1da, x1e5, x1e6 - girl intro
            // ////////////////////////////////////////////////////
            EventScript newEvent1DA = new EventScript();
            context.replacementEvents[0x1da] = newEvent1DA;
            // new for bug fixes
            newEvent1DA.Add(0x4a); // this is some sort of hp check for the enemies
            newEvent1DA.Add(0x05);
            newEvent1DA.Add(0x80);
            newEvent1DA.Add(0xe9);
            newEvent1DA.Jump(0x1D9); // jump to 1D9

            newEvent1DA.Add(0x4a); // this is some sort of hp check for the enemies
            newEvent1DA.Add(0x06);
            newEvent1DA.Add(0x80);
            newEvent1DA.Add(0xe9);
            newEvent1DA.Jump(0x1D9); // jump to 1D9

            newEvent1DA.Add(0x38); // ???
            newEvent1DA.Add(0x02);
            newEvent1DA.Jump(0x1E6); // jump to 1E6
            newEvent1DA.Logic(EventFlags.GIRL_PERMANENT_FLAG, 0x1, 0xF, EventScript.GetJsrCmd(0x1E5)); // jump to girl rejoining with no rename
            newEvent1DA.IncrFlag(EventFlags.WITCHFOREST_GIRL_FLAG);
            newEvent1DA.Add(EventCommandEnum.NAME_CHARACTER.Value);
            newEvent1DA.Add(0x01); // name the girl
            newEvent1DA.IncrFlag(EventFlags.GIRL_IN_PARTY_FLAG);
            newEvent1DA.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent1DA.Add(EventCommandEnum.ADD_CHARACTER.Value);
            newEvent1DA.Add(0x01); // add the girl
            // if don't have fist weapon, add it
            newEvent1DA.Logic(EventFlags.GLOVE_ORBS_FLAG_B, 0x0, 0x0, EventScript.GetIncrCmd(EventFlags.GLOVE_ORBS_FLAG_B));
            newEvent1DA.SetFlag(EventFlags.PANDORA_RUINS_FLAG, 5);
            newEvent1DA.Logic(EventFlags.PANDORA_GIRL_FLAG, 0x0, 0x2, EventScript.GetJsrCmd(0x3F));
            newEvent1DA.Door(0x50); // exit werewolf room
            newEvent1DA.Jump(0x616); // music and stuff at the end of 1DA
            newEvent1DA.End();


            // x187 - girl dialogue
            // x1b0, x1b2, x1b5, x1b6 - watts
            // ////////////////////////////////////////////////////
            // x1b9, x1d3, x1d4, x1dd - sprite intro
            // ////////////////////////////////////////////////////
            // tropicallo death event
            EventScript newEvent1B9 = new EventScript();
            context.replacementEvents[0x1b9] = newEvent1B9;
            newEvent1B9.Jsr(0x503); // spear orb
            newEvent1B9.IncrFlag(EventFlags.GAIAS_NAVEL_WATTS_FLAG);
            newEvent1B9.Jsr(0x7CF); // music change
            newEvent1B9.Door(0x51); // back to town map
            newEvent1B9.Add(0x06); // ?
            // talking to sprite normally triggers the next part here, we'll do it all in one
            // that's event x1D3
            newEvent1B9.Add(EventCommandEnum.NAME_CHARACTER.Value);
            newEvent1B9.Add(0x02); // name the sprite
            newEvent1B9.IncrFlag(EventFlags.SPRITE_IN_PARTY_FLAG);
            newEvent1B9.SetFlag(EventFlags.GAIAS_NAVEL_WATTS_FLAG, 6);
            newEvent1B9.IncrFlag(EventFlags.GAIAS_NAVEL_SPRITE_FLAG);
            newEvent1B9.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent1B9.Add(EventCommandEnum.ADD_CHARACTER.Value);
            newEvent1B9.Add(0x02); // add the sprite
            newEvent1B9.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent1B9.Add(0x01); // sprite join music
            newEvent1B9.Add(0x35);
            newEvent1B9.Add(0x0F);
            newEvent1B9.Add(0xFF);
            newEvent1B9.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
            newEvent1B9.Add(0xAD); // bow and arrow
            newEvent1B9.Add(EventCommandEnum.NAME_CHARACTER.Value);
            newEvent1B9.Add(0x08); // used for "update weapons" here
            newEvent1B9.Add(EventCommandEnum.HEAL.Value);
            newEvent1B9.Add(0x44); // full heal, presumably
            newEvent1B9.AddAutoTextDialogueBox("Got the bow and arrows,\nand the Sprite!", 0x14); // let the music play
            newEvent1B9.Logic(EventFlags.BOOMERANG_ORBS_FLAG_B, 0x0, 0x0, EventScript.GetIncrCmd(EventFlags.BOOMERANG_ORBS_FLAG_B));
            newEvent1B9.IncrFlag(EventFlags.BOW_ORBS_FLAG_B);
            newEvent1B9.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent1B9.Add(0x01);
            newEvent1B9.Add(0x1F);
            newEvent1B9.Add(0x1A);
            newEvent1B9.Add(0x8F); // copy event 71F, since it's not a subroutine
            newEvent1B9.SetFlag(EventFlags.GAIAS_NAVEL_LAVA_DRAIN_SWITCH, 9);
            newEvent1B9.IncrFlag(EventFlags.EARTHPALACE_FLAG);
            newEvent1B9.Door(0x52); // leave tropicallo map
            // now we talk to the dwarf dude, and he gives us the arrows, and teleports us out.
            // event 1D4
            newEvent1B9.End();


            // x1be - tropicallo

            // ////////////////////////////////////////////////////
            // x1c7, x1ca, x1cc, x1ce, x1cf, x1d0 - rabiteman .. mainly 1cc, the map 297 walkon
            // ////////////////////////////////////////////////////
            EventScript newEvent1CC = new EventScript();
            context.replacementEvents[0x1cc] = newEvent1CC;
            // event flag x15 should be 05
            newEvent1CC.Logic(EventFlags.GAIAS_NAVEL_SPRITE_FLAG, 0x4, 0xF, EventScript.GetJumpCmd(0));
            newEvent1CC.SetFlag(EventFlags.GAIAS_NAVEL_SPRITE_FLAG, 5);
            newEvent1CC.AddDialogueBox("Sorry, the Rabiteman show's\non break today.");
            newEvent1CC.End();


            // ////////////////////////////////////////////////////
            // x1dc, x1e1, x1e7, x1ed, x1f6 - witch castle dialogue
            // ////////////////////////////////////////////////////
            EventScript newEvent1DC = new EventScript();
            context.replacementEvents[0x1dc] = newEvent1DC;
            newEvent1DC.Logic(EventFlags.WITCHCASTLE_FLAG, 0x2, 0xF, EventScript.GetJumpCmd(0));
            newEvent1DC.Add(0x06); // ?
            newEvent1DC.Add(EventCommandEnum.MOVE_EVERYONE_TO_P1.Value);
            newEvent1DC.SetFlag(EventFlags.WITCHCASTLE_FLAG, 2); // open the gates
            newEvent1DC.Jsr(0x78C); // play the open gate sound
            newEvent1DC.Add(EventCommandEnum.REFRESH_MAP.Value); // apply event flag 17
            newEvent1DC.End();


            // map 306, remove the witch at the top of the room since sometimes she can be despawned
            int witchMapNum = MAPNUM_WITCHCASTLE_NESOBERI;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, witchMapNum);
            mapObjOffset += 8; // skip header
            rom[mapObjOffset + 8 * 0 + 0] = 0; // adjust event data of obj 0
            rom[mapObjOffset + 8 * 0 + 1] = 0xFF; // adjust event data of obj 0


            // spikey intro
            EventScript newEvent1E7 = new EventScript();
            context.replacementEvents[0x1e7] = newEvent1E7;
            newEvent1E7.Logic(EventFlags.WITCHCASTLE_FLAG, 0x4, 0xF, EventScript.GetJumpCmd(0));
            newEvent1E7.SetFlag(EventFlags.WITCHCASTLE_FLAG, 3); // set this because we removed the npc, due to spawning issues
            newEvent1E7.Add(EventCommandEnum.REFRESH_MAP.Value); // apply event flag 17
            newEvent1E7.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent1E7.Add(0x00);
            newEvent1E7.Add(0x20);
            newEvent1E7.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            // dyluck flash
            for (int i = 0; i < 3; i++)
            {
                newEvent1E7.IncrFlag(EventFlags.WITCHCASTLE_FLAG);
                newEvent1E7.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                newEvent1E7.Sleep(0x02);
                newEvent1E7.DecrFlag(EventFlags.WITCHCASTLE_FLAG);
                newEvent1E7.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                newEvent1E7.Sleep(0x02);
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
            for (int i = 0; i < 3; i++)
            {
                // spikey gate opens
                newEvent1E7.Jsr(0x1F0);
                newEvent1E7.Sleep(0x0C);
            }
            newEvent1E7.Door(0x13A); // spikey fight
            newEvent1E7.Jump(0x704); // boss theme
            newEvent1E7.End();


            // elinee shriek
            EventScript newEvent1ED = new EventScript();
            context.replacementEvents[0x1ed] = newEvent1ED;
            newEvent1ED.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent1ED.Add(0x02); // elinee transform sound
            newEvent1ED.Add(0x87);
            newEvent1ED.Add(0x0F);
            newEvent1ED.Add(0x88);
            for(int i=0; i < 3; i++)
            {
                newEvent1ED.IncrFlag(EventFlags.WITCHCASTLE_FLAG);
                newEvent1ED.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                newEvent1ED.Sleep(0x01);
                newEvent1ED.DecrFlag(EventFlags.WITCHCASTLE_FLAG);
                newEvent1ED.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                newEvent1ED.Sleep(0x01);
            }
            newEvent1ED.IncrFlag(EventFlags.WITCHCASTLE_FLAG);
            newEvent1ED.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            // show the two chests on the right
            newEvent1ED.IncrFlag(EventFlags.NEXT_TO_WHIP_CHEST_VISIBILITY_FLAG);
            newEvent1ED.IncrFlag(EventFlags.WHIP_CHEST_VISIBILITY_FLAG);
            newEvent1ED.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent1ED.End();


            // 1F6 dyluck rant & luka message outside castle
            EventScript newEvent1F6 = new EventScript();
            context.replacementEvents[0x1f6] = newEvent1F6;
            newEvent1F6.Jsr(0x7DA); // dyluck theme
            newEvent1F6.IncrFlag(EventFlags.WHIP_CHEST_VISIBILITY_FLAG);
            newEvent1F6.Logic(EventFlags.WATER_PALACE_FLAG, 0xC, 0xC, EventScript.GetIncrCmd(EventFlags.WATER_PALACE_FLAG));
            newEvent1F6.AddDialogueBox("Now head to the water\npalace for " + elementNames[newElementMapping[1]] + ".");
            newEvent1F6.End();


            // ////////////////////////////////////////////////////
            // x23b, 236 - gnome intro
            // ////////////////////////////////////////////////////
            EventScript newEvent23B = new EventScript();
            context.replacementEvents[0x23b] = newEvent23B;
            newEvent23B.Logic(EventFlags.EARTHPALACE_FLAG, 0x3, 0xF, EventScript.GetJumpCmd(0));
            newEvent23B.Jump(0x236); // jump to event 236
            newEvent23B.End();


            EventScript newEvent236 = new EventScript();
            context.replacementEvents[0x236] = newEvent236;
            newEvent236.IncrFlag(EventFlags.EARTHPALACE_FLAG);
            newEvent236.IncrFlag(EventFlags.EARTHPALACE_FLAG);
            newEvent236.IncrFlag(EventFlags.EARTHPALACE_FLAG);
            newEvent236.Door(0x14B); // door to fire gigas room
            newEvent236.Jump(0x704); // boss theme
            newEvent236.End();


            // 22E - gnome after boss
            EventScript newEvent22E = new EventScript();
            context.replacementEvents[0x22e] = newEvent22E;
            newEvent22E.IncrFlag(EventFlags.ELEMENT_GNOME_FLAG);
            newEvent22E.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // makes the gnome disappear i think
            newEvent22E.IncrFlag(EventFlags.EARTHPALACE_FLAG);
            setAttribute(0, newElementMapping, newEvent22E); // stuff from event x580
            newEvent22E.SetFlag(EventFlags.EARTH_SEED, 1); // stuff from event x592 - gnome seed power
            newEvent22E.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent22E.Jsr(0x594); // update mana power
            newEvent22E.IncrFlag(EventFlags.EARTHPALACE_FLAG); // make the seed do nothing when you talk to it
            newEvent22E.AddDialogueBox("Got " + elementNames[newElementMapping[0]] + " spells, and the \nGnome seed's power.\nPandora next!");
            newEvent22E.End();


            // ////////////////////////////////////////////////////
            // x250 - spring beak intro
            // ////////////////////////////////////////////////////
            EventScript newEvent250 = new EventScript();
            context.replacementEvents[0x250] = newEvent250;
            newEvent250.Logic(EventFlags.UPPERLAND_SPRINGBEAK_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(0x704)); // boss theme
            newEvent250.Logic(EventFlags.UPPERLAND_SPRINGBEAK_FLAG, 0x2, 0xF, EventScript.GetJumpCmd(0x7CB)); // post-boss theme?
            newEvent250.IncrFlag(EventFlags.UPPERLAND_SPRINGBEAK_FLAG); // move on
            newEvent250.Jsr(0x79A); // spring beak screech
            newEvent250.Sleep(0x10);
            newEvent250.Jump(0x704); // boss theme
            newEvent250.End();


            // ////////////////////////////////////////////////////
            // x259 - landing in upper land
            // ////////////////////////////////////////////////////
            EventScript newEvent259 = new EventScript();
            context.replacementEvents[0x259] = newEvent259;
            newEvent259.SetFlag(EventFlags.UPPERLAND_MOOGLES_FLAG, 2); // skip moogle dialogue
            newEvent259.End();


            // ////////////////////////////////////////////////////
            // x4E2 - getting sylphid
            // ////////////////////////////////////////////////////
            EventScript newEvent4E2 = new EventScript();
            context.replacementEvents[0x4e2] = newEvent4E2;
            newEvent4E2.Add(0x06);
            newEvent4E2.Logic(EventFlags.UPPERLAND_PROGRESS_FLAG, 0xD, 0xE, EventScript.GetJumpCmd(0x24F));
            newEvent4E2.OpenDialogueBox();
            newEvent4E2.Logic(EventFlags.UPPERLAND_PROGRESS_FLAG, 0x5, 0xC, EventScript.GetJumpCmd(0x253));
            // normal path
            newEvent4E2.Jsr(0x796); // sylphid summon sound
            newEvent4E2.SetFlag(EventFlags.UPPERLAND_PROGRESS_FLAG, 4); // sylphid appears
            newEvent4E2.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            setAttribute(3, newElementMapping, newEvent4E2);
            newEvent4E2.SetFlag(EventFlags.ELEMENT_SYLPHID_FLAG, 2);
            newEvent4E2.SetFlag(EventFlags.WIND_SEED, 1); // as if you touched the sylphid seed too
            newEvent4E2.SetFlag(EventFlags.TOTAL_MANA_POWER_FLAG, 3); // don't use the subroutine because if you touch the mana seed first it increments twice.
            newEvent4E2.AddDialogue("Got seed's power, and\n" + elementNames[newElementMapping[3]] + " spells.");
            newEvent4E2.CloseDialogueBox();
            newEvent4E2.IncrFlag(EventFlags.UPPERLAND_PROGRESS_FLAG); // make sylphid disappear
            newEvent4E2.Add(EventCommandEnum.REFRESH_OBJECTS.Value); // refresh
            newEvent4E2.End();


            // ////////////////////////////////////////////////////
            // 4E3 - matango king dialogue
            // ////////////////////////////////////////////////////
            EventScript newEvent4E3 = new EventScript();
            context.replacementEvents[0x4e3] = newEvent4E3;
            newEvent4E3.Logic(EventFlags.MATANGO_PROGRESS_FLAG, 0x3, 0x7, EventScript.GetJumpCmd(0x276));
            newEvent4E3.Logic(EventFlags.MATANGO_PROGRESS_FLAG, 0x8, 0x9, EventScript.GetJumpCmd(0x277));
            newEvent4E3.Logic(EventFlags.MATANGO_PROGRESS_FLAG, 0xA, 0xF, EventScript.GetJumpCmd(0x279));
            // default path
            newEvent4E3.AddDialogueBox("Go do the thing");
            newEvent4E3.Logic(EventFlags.MATANGO_PROGRESS_FLAG, 0x3, 0xF, EventScript.GetJumpCmd(0));
            newEvent4E3.SetFlag(EventFlags.MATANGO_PROGRESS_FLAG, 3);
            newEvent4E3.End();


            // ////////////////////////////////////////////////////
            // x4e4 - flammie intro in matango cave + following dialogue
            // ////////////////////////////////////////////////////
            EventScript newEvent4E4 = new EventScript();
            context.replacementEvents[0x4e4] = newEvent4E4;
            newEvent4E4.Logic(EventFlags.MATANGO_PROGRESS_FLAG, 0x7, 0xF, EventScript.GetJumpCmd(0));
            newEvent4E4.Door(0x1CE);
            newEvent4E4.SetFlag(EventFlags.MATANGO_PROGRESS_FLAG, 8);
            newEvent4E4.AddDialogueBox("We'll leave this here for\nnow. Off to Kakkara and\nthe ice country!");
            newEvent4E4.End();


            // ////////////////////////////////////////////////////
            // x36f - kilroy intro
            // ////////////////////////////////////////////////////
            // map 316, the room before kilroy, remove all npcs; objects 0/1/2/3 - remove
            int kilroyEventMapNum = MAPNUM_KILROYSHIP_INTERIOR;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, kilroyEventMapNum);
            mapObjOffset += 8; // skip header
            rom[mapObjOffset + 8 * 0 + 0] = 0; // adjust event data of obj 0
            rom[mapObjOffset + 8 * 0 + 1] = 0xFF; // adjust event data of obj 0
            rom[mapObjOffset + 8 * 1 + 0] = 0; // adjust event data of obj 1
            rom[mapObjOffset + 8 * 1 + 1] = 0xFF; // adjust event data of obj 1
            rom[mapObjOffset + 8 * 2 + 0] = 0; // adjust event data of obj 2
            rom[mapObjOffset + 8 * 2 + 1] = 0xFF; // adjust event data of obj 2
            rom[mapObjOffset + 8 * 3 + 0] = 0; // adjust event data of obj 3
            rom[mapObjOffset + 8 * 3 + 1] = 0xFF; // adjust event data of obj 3


            EventScript newEvent36F = new EventScript();
            context.replacementEvents[0x36f] = newEvent36F;
            newEvent36F.Add(0x06);
            newEvent36F.IncrFlag(EventFlags.KILROY_EVENT_FLAG);
            newEvent36F.IncrFlag(EventFlags.KILROY_EVENT_FLAG);
            newEvent36F.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent36F.End();


            // 93a62
            // ////////////////////////////////////////////////////
            // x400 - intro with the manafort backdrop and shit - remove entirely
            // ////////////////////////////////////////////////////
            EventScript newEvent400 = new EventScript();
            context.replacementEvents[0x400] = newEvent400;
            newEvent400.IncrFlag(EventFlags.DIALOGUE_NO_BORDER_FLAG);
            // ton of shit cut out in here
            newEvent400.Door(0x00); // door 0 goes to the log
            newEvent400.DecrFlag(EventFlags.DIALOGUE_NO_BORDER_FLAG);
            newEvent400.Jump(0x106);
            newEvent400.End();


            // ////////////////////////////////////////////////////
            // x4e6 - elder talk before getting kicked out
            // ////////////////////////////////////////////////////
            EventScript newEvent4E6 = new EventScript();
            context.replacementEvents[0x4e6] = newEvent4E6;
            string[] elderDialogues = new string[] {
                "ELDER: Oh hi!\nYou're kicked out bye.",
                "ELDER: You know what\nhappens now.",
                "ELDER: Come back when\nyou kill a mana beast.",
            };
            string elderDialogue = elderDialogues[r.Next() % elderDialogues.Length];
            newEvent4E6.AddDialogueBox(elderDialogue);
            newEvent4E6.Jsr(0x734); // music
            newEvent4E6.IncrFlag(EventFlags.POTOS_FLAG);
            newEvent4E6.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent4E6.End();


            // ////////////////////////////////////////////////////
            // x581 - undine
            // ////////////////////////////////////////////////////
            EventScript newEvent581 = new EventScript();
            context.replacementEvents[0x581] = newEvent581;
            newEvent581.Jsr(0x41B); // full heal
            newEvent581.Add(EventCommandEnum.HEAL.Value);
            newEvent581.Add(0x84); // idk more heals
            newEvent581.Add(0x06);
            newEvent581.IncrFlag(EventFlags.JAVELIN_ORBS_FLAG_B);
            newEvent581.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
            newEvent581.Add(0xBF); // pole dart
            newEvent581.Add(EventCommandEnum.NAME_CHARACTER.Value);
            newEvent581.Add(0x08); // used for "update weapons" here
            newEvent581.Add(EventCommandEnum.HEAL.Value);
            newEvent581.Add(0x44); // shrug
            setAttribute(1, newElementMapping, newEvent581);
            newEvent581.AddDialogueBox("Got " + elementNames[newElementMapping[1]] + " spells.\nGo open Earth Palace.");
            // flicker undine out
            for (int i = 0; i < 2; i++)
            {
                newEvent581.IncrFlag(EventFlags.ELEMENT_UNDINE_FLAG);
                newEvent581.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                newEvent581.DecrFlag(EventFlags.ELEMENT_UNDINE_FLAG);
                newEvent581.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            }
            newEvent581.IncrFlag(EventFlags.ELEMENT_UNDINE_FLAG);
            newEvent581.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent581.End();


            // ////////////////////////////////////////////////////
            // x289 - kakkara intro
            // ////////////////////////////////////////////////////
            EventScript newEvent289 = new EventScript();
            context.replacementEvents[0x289] = newEvent289;
            newEvent289.Jsr(0x7C2); // kakkara theme
            newEvent289.Logic(EventFlags.LOST_IN_DESERT_FLAG, 0x1, 0xF, EventScript.GetJumpCmd(0));
            newEvent289.IncrFlag(EventFlags.LOST_IN_DESERT_FLAG);
            string[] desertDialogue1s = new string[] {
                "Skip this part...",
                "Not interested",
                "Pass on this",
                "Nope...",
            };
            string desertDialogue1 = desertDialogue1s[r.Next() % desertDialogue1s.Length];
            newEvent289.AddDialogueBox(desertDialogue1);
            newEvent289.IncrFlag(EventFlags.DESERT_SHIP_FLAG);
            newEvent289.Door(0x1A3);
            string[] desertDialogue2s = new string[] {
                "Yeah let's skip this too.",
                "No thanks",
                "Skip to the boss please"
            };
            string desertDialogue2 = desertDialogue2s[r.Next() % desertDialogue2s.Length];
            newEvent289.AddDialogueBox(desertDialogue2);
            newEvent289.SetFlag(EventFlags.DESERT_SHIP_FLAG, 0xB); // event x291 sergo dialogue, x295/x296/x29b sprite dialogue, x29c girl dialogue
            newEvent289.Door(0x193); // mech rider 1
            newEvent289.Jsr(0x7C4); // boss theme
            newEvent289.End();


            // 2A1 is the desert mech rider death event - combine with 2A7(2A8) for removing the shit after
            EventScript newEvent2A1 = new EventScript();
            context.replacementEvents[0x2A1] = newEvent2A1;
            newEvent2A1.Jsr(0x504); // orb
            newEvent2A1.Jsr(0x7C2); // kakkara theme
            newEvent2A1.SetFlag(EventFlags.DESERT_SHIP_FLAG, 0xE);
            newEvent2A1.Door(0x194); // out into normal desert
            newEvent2A1.End();


            // ////////////////////////////////////////////////////
            // x28A - kakkara map 67 event .. also x28C, x28B - ignore/see above
            // ////////////////////////////////////////////////////

            // ////////////////////////////////////////////////////
            // x384 - salamando intro - combine with x38C, x38D, x582
            // ////////////////////////////////////////////////////
            // jump to event 38d
            EventScript newEvent384 = new EventScript();
            context.replacementEvents[0x384] = newEvent384;
            newEvent384.Jump(0x38D);
            newEvent384.End();


            EventScript newEvent38D = new EventScript();
            context.replacementEvents[0x38d] = newEvent38D;
            newEvent38D.IncrFlag(EventFlags.SALAMANDO_STOVE_FLAG); // get rid of the dude i think
            newEvent38D.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            // 38d stuff
            newEvent38D.IncrFlag(EventFlags.SALAMANDO_STOVE_FLAG); // ?
            newEvent38D.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent38D.IncrFlag(EventFlags.SALAMANDO_STOVE_FLAG); // show salamando ?
            newEvent38D.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent38D.SetFlag(EventFlags.ELEMENT_SALAMANDO_FLAG, 2); // salamando stuff
            setAttribute(2, newElementMapping, newEvent38D);
            // 582
            newEvent38D.AddDialogueBox("Got " + elementNames[newElementMapping[2]] + "!");
            newEvent38D.IncrFlag(EventFlags.SALAMANDO_STOVE_FLAG); // hide salamando ?
            newEvent38D.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent38D.Door(0x142); // cold version of the map?
            newEvent38D.IncrFlag(EventFlags.SALAMANDO_STOVE_FLAG);
            newEvent38D.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent38D.Jump(0x724); // snowy music
            newEvent38D.End();


            // ////////////////////////////////////////////////////
            // x399 - mara
            // ////////////////////////////////////////////////////
            EventScript newEvent399 = new EventScript();
            context.replacementEvents[0x399] = newEvent399;
            newEvent399.Logic(EventFlags.SOUTHTOWN_MARA_FLAG, 0x3, 0x3, EventScript.GetJumpCmd(0x397));
            newEvent399.Logic(EventFlags.SOUTHTOWN_MARA_FLAG, 0x4, 0xF, EventScript.GetJumpCmd(0x396));
            newEvent399.Logic(EventFlags.SOUTHTOWN_MARA_FLAG, 0x1, 0x1, EventScript.GetIncrCmd(EventFlags.SOUTHTOWN_MARA_FLAG));
            newEvent399.AddDialogueBox("Just go talk to the\ndude you won't even need\na password");
            newEvent399.End();


            // x3a4, x3a5, x3a6, x3a7 - password entry 
            EventScript newEvent3A4 = new EventScript();
            context.replacementEvents[0x3a4] = newEvent3A4;
            newEvent3A4.Add(0x06);
            newEvent3A4.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent3A4.Add(0x80);
            newEvent3A4.Add(0x90);
            newEvent3A4.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent3A4.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent3A4.Add(0x80);
            newEvent3A4.Add(0xC0);
            newEvent3A4.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            string[] guardDialogues = new string[] {
                "Alright get in",
                "I forgot the password\nanyway",
                "Pardon me",
            };
            string guardDialogue = guardDialogues[r.Next() % guardDialogues.Length];
            newEvent3A4.AddDialogueBox(guardDialogue);
            newEvent3A4.SetFlag(EventFlags.TEMPORARY_DIALOGUE_CHOICE_FLAG, 1);
            newEvent3A4.End();


            // x3aa - northtown krissie dialogue
            EventScript newEvent3AA = new EventScript();
            context.replacementEvents[0x3aa] = newEvent3AA;
            string[] krissieDialogues = new string[]{
                "Hi go check out the \nruins",
                "Excited for another\nruins dungeon?!"
            };
            string krissieDialogue = krissieDialogues[r.Next() % krissieDialogues.Length];
            newEvent3AA.AddDialogueBox(krissieDialogue);
            newEvent3AA.IncrFlag(EventFlags.NORTHTOWN_DIALOGUE_FLAG);
            newEvent3AA.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent3AA.End();


            // ////////////////////////////////////////////////////
            // x555 - dyluck at northtown ruins
            // ////////////////////////////////////////////////////
            EventScript newEvent555 = new EventScript();
            context.replacementEvents[0x555] = newEvent555;
            newEvent555.OpenDialogueBox();
            newEvent555.Logic(EventFlags.NORTHTOWN_RUINS_FLAG, 0x5, 0xF, EventScript.GetJumpCmd(0x557));
            newEvent555.Jsr(0x7C4); // boss music
            newEvent555.AddDialogue("Go to the next room");
            newEvent555.CloseDialogueBox();
            newEvent555.IncrFlag(EventFlags.NORTHTOWN_RUINS_FLAG);
            newEvent555.End();


            // ////////////////////////////////////////////////////
            // x558 - thanatos at northtown ruins
            // ////////////////////////////////////////////////////
            // map 408 - remove girl; object 0
            int vampireMapNum = MAPNUM_VAMPIRE_ARENA;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, vampireMapNum);
            mapObjOffset += 8; // skip header
            rom[mapObjOffset + 8 * 0 + 0] = 0; // adjust event data of obj 4
            rom[mapObjOffset + 8 * 0 + 1] = 0xFF; // adjust event data of obj 4


            EventScript newEvent558 = new EventScript();
            context.replacementEvents[0x558] = newEvent558;
            newEvent558.IncrFlag(EventFlags.NORTHTOWN_RUINS_FLAG);
            newEvent558.IncrFlag(EventFlags.NORTHTOWN_RUINS_FLAG);
            newEvent558.Door(0x318); // vampire boss fight
            newEvent558.Jump(0x704); // boss music
            newEvent558.End();


            // ////////////////////////////////////////////////////
            // x559 - vampire death event
            // ////////////////////////////////////////////////////
            EventScript newEvent559 = new EventScript();
            context.replacementEvents[0x559] = newEvent559;
            newEvent559.Jsr(0x506); // orb reward
            newEvent559.Jsr(0x7D8); // music?
            newEvent559.IncrFlag(EventFlags.NORTHTOWN_RUINS_FLAG);
            newEvent559.Door(0x2D0); // exit vampire boss fight to the beginning of ruins
            newEvent559.SetFlag(EventFlags.NORTHTOWN_DIALOGUE_FLAG, 3);
            // let's bring x511 in here too, so we don't have to go talk to krissy in town
            newEvent559.AddDialogueBox("To the castle now");
            newEvent559.SetFlag(EventFlags.NORTHTOWN_CASTLE_FLAG, 0);
            newEvent559.End();


            // ////////////////////////////////////////////////////
            // x55e - phanna at northtown ruins
            // ////////////////////////////////////////////////////
            EventScript newEvent55E = new EventScript();
            context.replacementEvents[0x55e] = newEvent55E;
            newEvent55E.AddDialogueBox("I'm going somewhere with\ndifferent music");
            newEvent55E.IncrFlag(EventFlags.NORTHTOWN_PHANNA_FLAG);
            newEvent55E.IncrFlag(EventFlags.NORTHTOWN_PHANNA_FLAG);
            newEvent55E.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent55E.End();


            // northtown castle

            // x520 - dialogue
            EventScript newEvent520 = new EventScript();
            context.replacementEvents[0x520] = newEvent520;
            newEvent520.End();


            // x51d - emperor dialogue in castle - skip jail stuff and jump to boss (metal mantis normally)
            EventScript newEvent51D = new EventScript();
            context.replacementEvents[0x51d] = newEvent51D;
            newEvent51D.Logic(EventFlags.NORTHTOWN_CASTLE_FLAG, 0x1, 0xF, EventScript.GetJumpCmd(0));
            newEvent51D.SetFlag(EventFlags.NORTHTOWN_CASTLE_FLAG, 3);
            newEvent51D.Door(0x345); // metal mantis boss fight; skip jail shit
            newEvent51D.Jump(0x704); // boss theme
            newEvent51D.End();


            // x52e - northtown before mech rider
            EventScript newEvent52E = new EventScript();
            context.replacementEvents[0x52e] = newEvent52E;
            newEvent52E.SetFlag(EventFlags.NORTHTOWN_CASTLE_PROGRESS_FLAG, 5);
            newEvent52E.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent52E.Door(0x35D); // mech rider 2 boss fight
            newEvent52E.Jump(0x704); // boss theme
            newEvent52E.End();


            // x52f - mech rider 2 death event + x532 truffle dialogue + x278
            EventScript newEvent52F = new EventScript();
            context.replacementEvents[0x52f] = newEvent52F;
            newEvent52F.Jsr(0x507); // orb reward
            newEvent52F.IncrFlag(EventFlags.NORTHTOWN_CASTLE_PROGRESS_FLAG);
            newEvent52F.IncrFlag(EventFlags.NORTHTOWN_CASTLE_PROGRESS_FLAG);
            newEvent52F.IncrFlag(EventFlags.NORTHTOWN_CASTLE_PROGRESS_FLAG);
            newEvent52F.SetFlag(EventFlags.MATANGO_PROGRESS_FLAG, 0xA);
            newEvent52F.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
            newEvent52F.Add(0x47); // flammie drum
            newEvent52F.AddDialogueBox("Next destinations:\nShade, Lumina,\nLuna, Tasnica");
            newEvent52F.Add(EventCommandEnum.FLAMMIE_FROM_POS.Value);
            newEvent52F.Add(0x64);
            newEvent52F.End();


            // x543 (boss dialogue), x586 (after), x58D (seed) - shade (map 375)
            EventScript newEvent543 = new EventScript();
            context.replacementEvents[0x543] = newEvent543;
            newEvent543.Door(0x3A); // slime boss
            newEvent543.SetFlag(EventFlags.DEATH_TYPE_FLAG, 0xF);
            newEvent543.Jsr(0x7C4); // boss theme
            newEvent543.End();


            EventScript newEvent586 = new EventScript();
            context.replacementEvents[0x586] = newEvent586;
            setAttribute(5, newElementMapping, newEvent586);
            newEvent586.SetFlag(EventFlags.ELEMENT_SHADE_FLAG, 2); // idk make shade disappear i think
            newEvent586.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent586.IncrFlag(EventFlags.TOTAL_MANA_POWER_FLAG); // mana power+
            newEvent586.SetFlag(EventFlags.DARK_SEED, 1); // shade seed
            newEvent586.AddDialogueBox("Got " + elementNames[newElementMapping[5]] + " magic, and the\nseed's power!");
            newEvent586.End();


            // x59d, x59e, x59f - gold door locked
            EventScript newEvent59E = new EventScript();
            context.replacementEvents[0x59e] = newEvent59E;
            newEvent59E.Logic(EventFlags.LUMINA_TOWER_PROGRESS_FLAG, 0x0, 0x0, EventScript.GetIncrCmd(EventFlags.LUMINA_TOWER_PROGRESS_FLAG)); // just unlock
            newEvent59E.Door(0x29E); // in
            newEvent59E.Logic(EventFlags.LUMINA_TOWER_PROGRESS_FLAG, 0x0, 0x4, EventScript.GetJumpCmd(0x72A));
            newEvent59E.Jump(0x71D);
            newEvent59E.End();


            EventScript newEvent59F = new EventScript();
            context.replacementEvents[0x59f] = newEvent59F;
            newEvent59F.Logic(EventFlags.LUMINA_TOWER_PROGRESS_FLAG, 0x0, 0x0, EventScript.GetIncrCmd(EventFlags.LUMINA_TOWER_PROGRESS_FLAG)); // just unlock
            newEvent59F.Door(0x29F); // in
            newEvent59F.Logic(EventFlags.LUMINA_TOWER_PROGRESS_FLAG, 0x0, 0x4, EventScript.GetJumpCmd(0x72A));
            newEvent59F.Jump(0x71D);
            newEvent59F.End();

            // x587 lumina, x58c seed
            EventScript newEvent587 = new EventScript();
            context.replacementEvents[0x587] = newEvent587;
            setAttribute(4, newElementMapping, newEvent587);
            newEvent587.SetFlag(EventFlags.ELEMENT_LUMINA_FLAG, 2); // idk make lumina disappear i think
            newEvent587.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent587.IncrFlag(EventFlags.TOTAL_MANA_POWER_FLAG); // mana power+
            newEvent587.SetFlag(EventFlags.LIGHT_SEED, 1); // lumina seed
            newEvent587.AddDialogueBox("Got " + elementNames[newElementMapping[4]] + " magic, and the\nseed's power!");
            newEvent587.End();


            // x584 luna, x58e seed
            EventScript newEvent584 = new EventScript();
            context.replacementEvents[0x584] = newEvent584;
            setAttribute(6, newElementMapping, newEvent584);
            newEvent584.SetFlag(EventFlags.ELEMENT_LUNA_FLAG, 2); // idk make luna disappear i think
            newEvent584.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent584.IncrFlag(EventFlags.TOTAL_MANA_POWER_FLAG); // mana power+
            newEvent584.SetFlag(EventFlags.MOON_SEED, 1); // luna seed
            newEvent584.AddDialogueBox("Got " + elementNames[newElementMapping[6]] + " magic, and the\nseed's power!");
            newEvent584.End();


            // x2D2 - jema at tasnica, 2D4 - king
            // warp to king? door x364
            EventScript newEvent2D2 = new EventScript();
            context.replacementEvents[0x2d2] = newEvent2D2;
            newEvent2D2.AddDialogueBox("I'll warp you\nto the thing");
            newEvent2D2.IncrFlag(EventFlags.TASNICA_FLAG);
            newEvent2D2.Door(0x364); // king room
            newEvent2D2.End();


            EventScript newEvent2D4 = new EventScript();
            context.replacementEvents[0x2d4] = newEvent2D4;
            newEvent2D4.SetFlag(EventFlags.TASNICA_FLAG, 3);
            newEvent2D4.Jump(0x2D0); // boss music and warp
            newEvent2D4.End();


            // x2d1 - after tasnica king fight
            EventScript newEvent2D1 = new EventScript();
            context.replacementEvents[0x2d1] = newEvent2D1;
            // new - fix fight skip
            newEvent2D1.Add(0x4a); // this is some sort of hp check for the enemies
            newEvent2D1.Add(0x04);
            newEvent2D1.Add(0x80);
            newEvent2D1.Add(0xe9);
            newEvent2D1.Jump(0x2CF); // jump to 2CF
            newEvent2D1.IncrFlag(EventFlags.TASNICA_FLAG);
            newEvent2D1.Door(0x367); // king room
            newEvent2D1.Jsr(0x7CA); // music
            newEvent2D1.Jsr(0x501); // sword orb
            // combine with x2D6, the jema dialogue afterward
            newEvent2D1.IncrFlag(EventFlags.TASNICA_FLAG);
            newEvent2D1.IncrFlag(EventFlags.TASNICA_FLAG);
            newEvent2D1.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            string[] jemaTasnicaDialogue2s = new string[] {
            "Sage Jehk or Joch or\nJecht or whatever is\nnext",
            "Mountains're next",
            };
            string jemaTasnicaDialogue2 = jemaTasnicaDialogue2s[r.Next() % jemaTasnicaDialogue2s.Length];
            newEvent2D1.AddDialogueBox(jemaTasnicaDialogue2);
            newEvent2D1.End();


            // x3dd jehk johk
            // ugh this event
            EventScript newEvent3DD = new EventScript();
            context.replacementEvents[0x3dd] = newEvent3DD;
            newEvent3DD.Add(0x06); // 
            newEvent3DD.Logic(EventFlags.DARK_SEED, 0x0, 0x0, EventScript.GetJumpCmd(0x21));
            newEvent3DD.Logic(EventFlags.LIGHT_SEED, 0x0, 0x0, EventScript.GetJumpCmd(0x22));
            newEvent3DD.Logic(EventFlags.MOON_SEED, 0x0, 0x0, EventScript.GetJumpCmd(0x23));
            newEvent3DD.Logic(EventFlags.TASNICA_FLAG, 0x0, 0x5, EventScript.GetJumpCmd(0x20));
            newEvent3DD.Logic(EventFlags.JEHK_CAVE_FLAG, 0x1, 0x4, EventScript.GetJumpCmd(0x24));
            newEvent3DD.Logic(EventFlags.JEHK_CAVE_FLAG, 0x6, 0xF, EventScript.GetJumpCmd(0x26));
            newEvent3DD.SetFlag(EventFlags.JEHK_CAVE_FLAG, 1); // make other jehk thing appear
            newEvent3DD.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            // + x3DC stuff -> x3DA
            newEvent3DD.Jump(0x3DA);
            newEvent3DD.End();


            EventScript newEvent3DA = new EventScript();
            context.replacementEvents[0x3da] = newEvent3DA;
            newEvent3DA.Logic(EventFlags.JEHK_CAVE_FLAG, 0x4, 0x4, EventScript.GetJumpCmd(0));
            newEvent3DA.Jsr(0x79C); // sound
            newEvent3DA.IncrFlag(EventFlags.JEHK_CAVE_FLAG);
            newEvent3DA.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent3DA.Jsr(0x79C); // sound
            newEvent3DA.IncrFlag(EventFlags.JEHK_CAVE_FLAG);
            newEvent3DA.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent3DA.Jsr(0x79C); // sound
            newEvent3DA.IncrFlag(EventFlags.JEHK_CAVE_FLAG);
            newEvent3DA.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent3DA.End();


            // x4e5 - end of jehk test thing
            EventScript newEvent4E5 = new EventScript();
            context.replacementEvents[0x4e5] = newEvent4E5;
            newEvent4E5.Logic(EventFlags.JEHK_CAVE_FLAG, 0x6, 0xF, EventScript.GetJumpCmd(0x460));
            newEvent4E5.Add(0x4A); // idk something to do with the enemy characters
            newEvent4E5.Add(0x04);
            newEvent4E5.Add(0x91);
            newEvent4E5.Add(0x7F);
            newEvent4E5.Jump(0x3DE);
            newEvent4E5.Add(0x4A); // idk something to do with the enemy characters
            newEvent4E5.Add(0x05);
            newEvent4E5.Add(0x91);
            newEvent4E5.Add(0x7F);
            newEvent4E5.Jump(0x3DE);
            newEvent4E5.Add(0x4A); // idk something to do with the enemy characters
            newEvent4E5.Add(0x06);
            newEvent4E5.Add(0x91);
            newEvent4E5.Add(0x7F);
            newEvent4E5.Jump(0x3DE);
            newEvent4E5.SetFlag(EventFlags.JEHK_CAVE_FLAG, 6);
            newEvent4E5.Door(0x38); // back to jehk room
            newEvent4E5.Add(0x06);
            string[] jehkDialogues = new string[] {
            "To the sunken continent!\nIt's the purple one",
            "Now I guess we go raise\nthat purple continent",
            "To the Grand Palace, the\nmost tedious part of\nthe game",
            };
            string jehkDialogue = jehkDialogues[r.Next() % jehkDialogues.Length];
            newEvent4E5.AddDialogueBox(jehkDialogue);
            newEvent4E5.Jsr(0x41B); // heals i think
            newEvent4E5.Add(EventCommandEnum.HEAL.Value);
            newEvent4E5.Add(0x84);
            newEvent4E5.End();


            // x4b3 - emperor at sunken continent before watermelon thing
            EventScript newEvent4B3 = new EventScript();
            context.replacementEvents[0x4b3] = newEvent4B3;
            // fuck up all the mana seeds
            newEvent4B3.SetFlag(EventFlags.WATER_SEED, 0x02);
            newEvent4B3.SetFlag(EventFlags.EARTH_SEED, 0x02);
            newEvent4B3.SetFlag(EventFlags.WIND_SEED, 0x02);
            newEvent4B3.SetFlag(EventFlags.FIRE_SEED, 0x02);
            newEvent4B3.SetFlag(EventFlags.LIGHT_SEED, 0x02);
            newEvent4B3.SetFlag(EventFlags.DARK_SEED, 0x02);
            newEvent4B3.SetFlag(EventFlags.MOON_SEED, 0x02);
            newEvent4B3.SetFlag(EventFlags.WATER_PALACE_FLAG, 0xF); // idk
            newEvent4B3.IncrFlag(EventFlags.LOST_CONTINENT_WATERMELON_FLAG); // probably watermelon boss visibility
            newEvent4B3.Door(0x358); // watermelon boss
            newEvent4B3.Jsr(0x7C4); // boss music
            newEvent4B3.End();


            // x4b6 - dryad/seed + x585 spells
            EventScript newEvent4B6 = new EventScript();
            context.replacementEvents[0x4b6] = newEvent4B6;
            newEvent4B6.SetFlag(EventFlags.ELEMENT_DRYAD_FLAG, 1); // dryad appears
            newEvent4B6.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            setAttribute(7, newElementMapping, newEvent4B6);
            newEvent4B6.AddDialogueBox("Got " + elementNames[newElementMapping[7]] + " magic, and the\nseed's power!");
            newEvent4B6.SetFlag(EventFlags.ELEMENT_DRYAD_FLAG, 2); // dryad disappears
            newEvent4B6.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent4B6.IncrFlag(EventFlags.LOST_CONTINENT_PROGRESS_FLAG_1);
            newEvent4B6.IncrFlag(EventFlags.LOST_CONTINENT_WATERMELON_FLAG);
            newEvent4B6.SetFlag(EventFlags.DRYAD_AREA_FLAG_1, 0);
            newEvent4B6.SetFlag(EventFlags.DRYAD_AREA_FLAG_2, 0);
            newEvent4B6.SetFlag(EventFlags.NORTHTOWN_PHANNA_FLAG, 3);
            newEvent4B6.IncrFlag(EventFlags.NORTHTOWN_CASTLE_PROGRESS_FLAG);
            newEvent4B6.Jsr(0x71E); // music
            newEvent4B6.Add(0x1F);
            newEvent4B6.Add(0x03); // flammie scene
            newEvent4B6.End();


            // x59b - post-hydra dialogue
            EventScript newEvent59B = new EventScript();
            context.replacementEvents[0x59b] = newEvent59B;
            newEvent59B.Jsr(0x504); // whip's orb
            newEvent59B.SetFlag(EventFlags.LOST_CONTINENT_SWITCHES_FLAG_1, 4); // idk
            newEvent59B.Jsr(0x7D0); // music?
            newEvent59B.Door(0x31B); // out
            newEvent59B.End();


            // x4c8 - kettle kin intro
            EventScript newEvent4C8 = new EventScript();
            context.replacementEvents[0x4c8] = newEvent4C8;
            newEvent4C8.Logic(EventFlags.LOST_CONTINENT_KETTLEKIN_FLAG, 0x3, 0xF, EventScript.GetJumpCmd(0));
            newEvent4C8.Jsr(0x7C4); // music
            newEvent4C8.SetFlag(EventFlags.LOST_CONTINENT_KETTLEKIN_FLAG, 2); // visibility
            newEvent4C8.Door(0x3C7); // boss door
            newEvent4C8.End();


            // x4ca - kettle kin death + dialogue
            EventScript newEvent4CA = new EventScript();
            context.replacementEvents[0x4ca] = newEvent4CA;
            newEvent4CA.SetFlag(EventFlags.LOST_CONTINENT_KETTLEKIN_FLAG, 3);
            newEvent4CA.Jsr(0x505); // orb
            newEvent4CA.Jsr(0x7DD); // music?
            newEvent4CA.Door(0x3FE); // door out
            newEvent4CA.Jump(0x628); // more stuff
            newEvent4CA.End();


            // x4a0 - looks kind of odd!
            EventScript newEvent4A0 = new EventScript();
            context.replacementEvents[0x4a0] = newEvent4A0;
            newEvent4A0.Logic(EventFlags.LOST_CONTINENT_PROGRESS_FLAG_1, 0x0, 0x0, EventScript.GetJumpCmd(0x49F));
            newEvent4A0.Logic(EventFlags.LOST_CONTINENT_HEXAS_FLAG, 0x3, 0xF, EventScript.GetJumpCmd(0));
            newEvent4A0.Jsr(0x748); // music?
            newEvent4A0.End();


            // x4a1 - hexas intro
            EventScript newEvent4A1 = new EventScript();
            context.replacementEvents[0x4a1] = newEvent4A1;
            newEvent4A1.Logic(EventFlags.LOST_CONTINENT_HEXAS_FLAG, 0x5, 0xF, EventScript.GetJumpCmd(0));
            newEvent4A1.SetFlag(EventFlags.LOST_CONTINENT_HEXAS_FLAG, 5);
            newEvent4A1.Door(0x3DF); // hexas fight
            newEvent4A1.Jsr(0x7C4); // music
            newEvent4A1.End();


            // x4a2 - hexas death
            EventScript newEvent4A2 = new EventScript();
            context.replacementEvents[0x4a2] = newEvent4A2;
            newEvent4A2.Jsr(0x504); // orb
            newEvent4A2.Jsr(0x7DC); // music?
            newEvent4A2.IncrFlag(EventFlags.LOST_CONTINENT_HEXAS_FLAG);
            newEvent4A2.Add(EventCommandEnum.REFRESH_MAP.Value);
            newEvent4A2.End();


            // x4a3 - mech rider 3 fight
            EventScript newEvent4A3 = new EventScript();
            context.replacementEvents[0x4a3] = newEvent4A3;
            newEvent4A3.Logic(EventFlags.LOST_CONTINENT_HEXAS_FLAG, 0x7, 0xF, EventScript.GetJumpCmd(0));
            newEvent4A3.Door(0x3F6); // mech rider 3 map
            newEvent4A3.Jsr(0x7C4); // music
            newEvent4A3.End();


            // x4a4 - mech rider 3 death
            EventScript newEvent4A4 = new EventScript();
            context.replacementEvents[0x4a4] = newEvent4A4;
            newEvent4A4.IncrFlag(EventFlags.LOST_CONTINENT_HEXAS_FLAG);
            newEvent4A4.Jsr(0x503); // orb
            newEvent4A4.Jsr(0x71E); // continent sinking sound
            newEvent4A4.IncrFlag(EventFlags.LOST_CONTINENT_HEXAS_FLAG);
            newEvent4A4.SetFlag(EventFlags.LOST_CONTINENT_PROGRESS_FLAG_1, 3);
            string[] postSunkenDialogues = new string[] {
            "Let's go to Pure Lands!",
            "Off to that volcano thing"
            };
            string postSunkenDialogue = postSunkenDialogues[r.Next() % postSunkenDialogues.Length];
            newEvent4A4.AddDialogueBox(postSunkenDialogue);
            newEvent4A4.Add(0x1F);
            newEvent4A4.Add(0x04); // continent scene
            newEvent4A4.End();


            // x4d8 - pure lands intro
            EventScript newEvent4D8 = new EventScript();
            context.replacementEvents[0x4d8] = newEvent4D8;
            newEvent4D8.IncrFlag(EventFlags.PURELANDS_INTRO_FLAG);
            newEvent4D8.Return();
            newEvent4D8.End();


            // x5f8 - tree blowing up at the end of pure lands
            EventScript newEvent5F8 = new EventScript();
            context.replacementEvents[0x5f8] = newEvent5F8;
            newEvent5F8.Logic(EventFlags.PURELAND_PROGRESS_FLAG, 0x0, 0x5, EventScript.GetJumpCmd(0x5DD));
            newEvent5F8.Jsr(0x5DA); // save screen
            newEvent5F8.Add(0x06);
            newEvent5F8.Add(EventCommandEnum.MOVE_EVERYONE_TO_P1.Value);
            newEvent5F8.Jsr(0x72C); // music?
            newEvent5F8.Add(EventCommandEnum.INVIS_A.Value);
            newEvent5F8.Door(0x5E); // mana tree map
            newEvent5F8.IncrFlag(EventFlags.WALK_THROUGH_WALLS_FLAG); // go through walls?
            newEvent5F8.Add(EventCommandEnum.CHARACTER_ANIM.Value); // animation
            newEvent5F8.Add(0x00);
            newEvent5F8.Add(0x8B);
            newEvent5F8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5F8.Sleep(0x14);
            newEvent5F8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5F8.Add(0x00);
            newEvent5F8.Add(0x00); // stop the animation
            newEvent5F8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5F8.Jsr(0x72D); // whale sound
            newEvent5F8.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent5F8.Add(0x06);
            newEvent5F8.Add(0xFF);
            newEvent5F8.Add(0x7F); // palette flash
            newEvent5F8.Sleep(0x20);
            newEvent5F8.SetFlag(EventFlags.PURELAND_PROGRESS_FLAG, 7);
            newEvent5F8.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent5F8.Sleep(0x20);
            newEvent5F8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5F8.Add(0x00);
            newEvent5F8.Add(0x68); // p1 back down
            newEvent5F8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5F8.Add(EventCommandEnum.MOVE_EVERYONE_TO_P1.Value);
            newEvent5F8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5F8.Add(0x01);
            newEvent5F8.Add(0x00); // p1 face up
            newEvent5F8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5F8.Add(0x02);
            newEvent5F8.Add(0xC8); // p2 move left
            newEvent5F8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5F8.Add(0x03);
            newEvent5F8.Add(0x88); // p3 move right
            newEvent5F8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5F8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5F8.Add(0x02);
            newEvent5F8.Add(0x48); // p2 move down
            newEvent5F8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5F8.Add(0x03);
            newEvent5F8.Add(0x48); // p3 move down
            newEvent5F8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5F8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5F8.Add(0x02);
            newEvent5F8.Add(0x00); // p2 face up
            newEvent5F8.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent5F8.Add(0x03);
            newEvent5F8.Add(0x00); // p3 face up
            newEvent5F8.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);
            newEvent5F8.Add(EventCommandEnum.INVIS_B.Value);
            newEvent5F8.Add(0x2D);
            newEvent5F8.Add(0x07); // palette unflash
            newEvent5F8.Sleep(0x20);
            newEvent5F8.SetFlag(EventFlags.PURELAND_PROGRESS_FLAG, 8);
            newEvent5F8.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent5F8.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent5F8.Add(0x01); // boss kill sound
            newEvent5F8.Add(0x18);
            newEvent5F8.Add(0x44);
            newEvent5F8.Add(0x8F);
            newEvent5F8.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent5F8.Add(0x06);
            newEvent5F8.Add(0xFF);
            newEvent5F8.Add(0x7F); // palette flash
            newEvent5F8.Sleep(0x40);
            newEvent5F8.DecrFlag(EventFlags.WALK_THROUGH_WALLS_FLAG); // don't go through walls?
            newEvent5F8.Door(0x368); // base of mana tree
            newEvent5F8.Jsr(0x734); // sad mana theme
            // max out the sword
            newEvent5F8.SetFlag(EventFlags.SWORD_ORBS_FLAG_B, 8);
            newEvent5F8.SetFlag(EventFlags.SWORD_ORBS_FLAG_A, 7);
            newEvent5F8.AddDialogueBox("Hmm, oh well.\nTime to go beat the game!");
            newEvent5F8.Jump(0x5F9); // set all the mana seed flags
            newEvent5F8.End();


            // x4e1 - thanatos finale
            EventScript newEvent4E1 = new EventScript();
            context.replacementEvents[0x4e1] = newEvent4E1;
            newEvent4E1.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            newEvent4E1.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            newEvent4E1.Door(0x2F); // lich boss fight
            newEvent4E1.Jump(0x70C); // lich fight theme
            newEvent4E1.End();


            // x42d - thanatos death
            EventScript newEvent42D = new EventScript();
            context.replacementEvents[0x42d] = newEvent42D;
            newEvent42D.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            newEvent42D.IncrFlag(EventFlags.MANAFORT_SWITCHES_FLAG_1);
            newEvent42D.Jsr(0x736); // music
            newEvent42D.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            newEvent42D.Add(0x02);
            newEvent42D.Add(0xCD);
            newEvent42D.Add(0x38); // mana magic
            newEvent42D.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            newEvent42D.Add(0x03);
            newEvent42D.Add(0xCD);
            newEvent42D.Add(0x07); // mana magic
            newEvent42D.End();


            // x429 before mana beast
            EventScript newEvent429 = new EventScript();
            context.replacementEvents[0x429] = newEvent429;
            newEvent429.Logic(EventFlags.MANAFORT_SWITCHES_FLAG_1, 0x0, 0x07, EventScript.GetJumpCmd(0));
            newEvent429.Door(0x3B); // before mana beast
            newEvent429.Jsr(0x737); // music
            newEvent429.AddDialogueBox("I hope you remembered to\nlevel Dryad");
            newEvent429.Jsr(0x739); // music
            newEvent429.Door(0xBC); // mana beast
            newEvent429.SetFlag(EventFlags.DEATH_TYPE_FLAG, 0xF);
            newEvent429.End();


            // ////////////////////////////////////////////////////
            // x66B - santa intro
            // ////////////////////////////////////////////////////
            EventScript newEvent66B = new EventScript();
            context.replacementEvents[0x66b] = newEvent66B;
            newEvent66B.Door(0x2CA); // santa boss fight
            newEvent66B.Jump(0x704); // boss music
            newEvent66B.End();


            // ////////////////////////////////////////////////////
            // x66D - post santa fight
            // ////////////////////////////////////////////////////
            EventScript newEvent66D = new EventScript();
            context.replacementEvents[0x66d] = newEvent66D;
            newEvent66D.SetFlag(EventFlags.DEATH_TYPE_FLAG, 2); // death change
            newEvent66D.SetFlag(EventFlags.ICE_CASTLE_SWITCHES_FLAG_2, 6); // switches
            newEvent66D.SetFlag(EventFlags.ICE_PALACE_FLAG, 4); // ice palace event flag
            newEvent66D.Jsr(0x506); // orb
            newEvent66D.Jsr(0x7C3); // music
            newEvent66D.Door(0x2CB); // exit santa boss fight
            // move door up a bit so i can see santa .. 83000 2CB*4 = B2C
            rom[0x83B2E] = 0x1A;
            newEvent66D.IncrFlag(EventFlags.ICE_PALACE_FLAG); // ice palace event flag
            newEvent66D.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
            newEvent66D.AddDialogueBox("Merry Christmas!");
            newEvent66D.IncrFlag(EventFlags.ICE_PALACE_FLAG);
            newEvent66D.Door(0x200); // exit thingy
            newEvent66D.Jump(0x636); // idk
            newEvent66D.End();


            // ////////////////////////////////////////////////////
            // x7fd - jema revive event
            // ////////////////////////////////////////////////////
            EventScript newEvent7FD = new EventScript();
            context.replacementEvents[0x7fd] = newEvent7FD;
            newEvent7FD.IncrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);
            newEvent7FD.AddDialogueBox("JEMA: You're pretty bad at\nthis, aren't you?");
            newEvent7FD.Jsr(0x41B); // restore
            newEvent7FD.DecrFlag(EventFlags.FREEZE_BOSS_AI_FLAG);
            newEvent7FD.End();


            // remove mandala npcs except one for faster walking
            int mandalaMapNum = MAPNUM_MANDALA_SOUTH;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, mandalaMapNum);
            mapObjOffset += 8; // skip header
            for(int i=0; i < 6; i++)
            {
                rom[mapObjOffset + 8 * i + 0] = 0x00;
                rom[mapObjOffset + 8 * i + 1] = 0xFF;
            }

            int southtownMapNum = MAPNUM_SOUTHTOWN;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, southtownMapNum);
            mapObjOffset += 8; // skip header
            // soldier
            rom[mapObjOffset + 8 * 3 + 0] = 0x00;
            rom[mapObjOffset + 8 * 3 + 1] = 0xFF;
            // lady in pink
            rom[mapObjOffset + 8 * 4 + 0] = 0x00;
            rom[mapObjOffset + 8 * 4 + 1] = 0xFF;
            // dude in lower left
            rom[mapObjOffset + 8 * 5 + 0] = 0x00;
            rom[mapObjOffset + 8 * 5 + 1] = 0xFF;
            // kid near him
            rom[mapObjOffset + 8 * 6 + 0] = 0x00;
            rom[mapObjOffset + 8 * 6 + 1] = 0xFF;
            // old dude
            rom[mapObjOffset + 8 * 7 + 0] = 0x00;
            rom[mapObjOffset + 8 * 7 + 1] = 0xFF;
            // soldier
            rom[mapObjOffset + 8 * 8 + 0] = 0x00;
            rom[mapObjOffset + 8 * 8 + 1] = 0xFF;

            int northtownMapNum = MAPNUM_NORTHTOWN;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, northtownMapNum);
            mapObjOffset += 8; // skip header

            rom[mapObjOffset + 8 * 0 + 0] = 0x00;
            rom[mapObjOffset + 8 * 0 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 1 + 0] = 0x00;
            rom[mapObjOffset + 8 * 1 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 3 + 0] = 0x00;
            rom[mapObjOffset + 8 * 3 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 4 + 0] = 0x00;
            rom[mapObjOffset + 8 * 4 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 5 + 0] = 0x00;
            rom[mapObjOffset + 8 * 5 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 6 + 0] = 0x00;
            rom[mapObjOffset + 8 * 6 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 8 + 0] = 0x00;
            rom[mapObjOffset + 8 * 8 + 1] = 0xFF;

            int pandoraMapNum = MAPNUM_PANDORA;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, pandoraMapNum);
            mapObjOffset += 8; // skip header

            rom[mapObjOffset + 8 * 1 + 0] = 0x00;
            rom[mapObjOffset + 8 * 1 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 2 + 0] = 0x00;
            rom[mapObjOffset + 8 * 2 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 4 + 0] = 0x00;
            rom[mapObjOffset + 8 * 4 + 1] = 0xFF;


            // walruses and such
            int snowTownMapNum = MAPNUM_TODO;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, snowTownMapNum);
            mapObjOffset += 8; // skip header
            rom[mapObjOffset + 8 * 2 + 0] = 0x00;
            rom[mapObjOffset + 8 * 2 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 4 + 0] = 0x00;
            rom[mapObjOffset + 8 * 4 + 1] = 0xFF;


            int matangoTownMapNum = MAPNUM_MATANGO_FRONTYARD;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, matangoTownMapNum);
            mapObjOffset += 8; // skip header
            rom[mapObjOffset + 8 * 3 + 0] = 0x00;
            rom[mapObjOffset + 8 * 3 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 4 + 0] = 0x00;
            rom[mapObjOffset + 8 * 4 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 6 + 0] = 0x00;
            rom[mapObjOffset + 8 * 6 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 7 + 0] = 0x00;
            rom[mapObjOffset + 8 * 7 + 1] = 0xFF;

            int kakkaraTownMapNum = MAPNUM_KAKKARA;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, kakkaraTownMapNum);
            mapObjOffset += 8; // skip header
            rom[mapObjOffset + 8 * 3 + 0] = 0x00;
            rom[mapObjOffset + 8 * 3 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 5 + 0] = 0x00;
            rom[mapObjOffset + 8 * 5 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 6 + 0] = 0x00;
            rom[mapObjOffset + 8 * 6 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 7 + 0] = 0x00;
            rom[mapObjOffset + 8 * 7 + 1] = 0xFF;

            // 58 is the one with pebblers
            int moogleTownMapNum = MAPNUM_UPPERLAND_MOOGLE_VILLAGE;
            mapObjOffset = VanillaMapUtil.getObjectOffset(origRom, moogleTownMapNum);
            mapObjOffset += 8; // skip header
            rom[mapObjOffset + 8 * 3 + 0] = 0x00;
            rom[mapObjOffset + 8 * 3 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 4 + 0] = 0x00;
            rom[mapObjOffset + 8 * 4 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 5 + 0] = 0x00;
            rom[mapObjOffset + 8 * 5 + 1] = 0xFF;
            rom[mapObjOffset + 8 * 6 + 0] = 0x00;
            rom[mapObjOffset + 8 * 6 + 1] = 0xFF;

            // event 24A - moogles in moogle town -> x258, flag x21 becomes 01
            // 218 is the seasons dialogue

            // event 24c is post-pebbler kill stuff, it has some 4As to check pebbler death
            EventScript newEvent24C = new EventScript();
            context.replacementEvents[0x24c] = newEvent24C;
            // pebbler death check
            newEvent24C.Add(0x4A);
            newEvent24C.Add(0x04);
            newEvent24C.Add(0x80);
            newEvent24C.Add(0xE9);
            newEvent24C.Jump(0x24E);
            newEvent24C.Add(0x4A);
            newEvent24C.Add(0x05);
            newEvent24C.Add(0x80);
            newEvent24C.Add(0xE9);
            newEvent24C.Jump(0x24E);
            newEvent24C.Add(0x4A);
            newEvent24C.Add(0x06);
            newEvent24C.Add(0x80);
            newEvent24C.Add(0xE9);
            newEvent24C.Jump(0x24E);
            newEvent24C.SetFlag(EventFlags.UPPERLAND_MOOGLES_FLAG, 3);
            newEvent24C.Door(0x1F0); // go to moogle town version of the map
            // jump to x711 after to play song - replace
            newEvent24C.Add(EventCommandEnum.PLAY_SOUND.Value);
            newEvent24C.Add(0x01);
            newEvent24C.Add(0x11);
            newEvent24C.Add(0x12);
            newEvent24C.Add(0x8F);
            // open dialogue box, then call the seasons hint event
            newEvent24C.OpenDialogueBox();
            newEvent24C.Jump(0x218); // jump to 218
            newEvent24C.End();
        }

        private void setAttribute(int elementId, List<int> newElementMapping, List<byte> eventData)
        {
            List<int> attributeValues = new int[] { 0xC8, 0xC9, 0xCA, 0xCB, 0xCF, 0xCE, 0xCC, 0xCD }.ToList();
            int newId = newElementMapping[elementId];
            int newValue = attributeValues[newId];

            eventData.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            eventData.Add(0x02); // girl
            eventData.Add((byte)newValue);
            if (newId == 5)
            {
                // shade
                eventData.Add(0x00);
            }
            else if(newId == 7)
            {
                // dryad
                eventData.Add(0x18);
            }
            else
            {
                eventData.Add(0x38);
            }
            eventData.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
            eventData.Add(0x03); // sprite
            eventData.Add((byte)newValue);
            if (newId == 4)
            {
                // lumina
                eventData.Add(0x00);
            }
            else if (newId == 7)
            {
                // dryad
                eventData.Add(0x03);
            }
            else
            {
                eventData.Add(0x07);
            }

        }
    }
}
