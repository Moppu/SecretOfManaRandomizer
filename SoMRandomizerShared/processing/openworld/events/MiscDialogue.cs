using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Random dumb shit that open world NPCs say that doesn't really have any bearing on the game.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MiscDialogue : RandoProcessor
    {
        protected override string getName()
        {
            return "Misc/silly dialogue changes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;

            List<string> allWattsDialogue = new string[]
            {
                VanillaEventUtil.wordWrapText("Rolled good weapon bonuses? Let me fix that for you"),
                VanillaEventUtil.wordWrapText("No refunds if I break your weapon"),
                VanillaEventUtil.wordWrapText("I can just forge you a Mana Sword now, how cool is that"),
                VanillaEventUtil.wordWrapText("If you see Gorgon Bull, keep his skull for me, I need a new hat."),
                VanillaEventUtil.wordWrapText("Wouldn't it be neat if I sold weapons? I don't, though."),
                VanillaEventUtil.wordWrapText("Congratulations, your next weapon will have triple Materia growth"),
                VanillaEventUtil.wordWrapText("100 free arrows with every bow upgrade!"),
                VanillaEventUtil.wordWrapText("Javelin is a way better weapon than it gets credit for"),
            }.ToList();

            // 0xbc: watts in gold city
            EventScript newEventBC = new EventScript();
            context.replacementEvents[0xBC] = newEventBC;
            int goldCityWattsDialogueIndex = r.Next() % allWattsDialogue.Count;
            newEventBC.AddDialogueBox(allWattsDialogue[goldCityWattsDialogueIndex]);
            List<byte> goldCityWattsDialogue = VanillaEventUtil.getBytes(allWattsDialogue[goldCityWattsDialogueIndex]);
            allWattsDialogue.RemoveAt(goldCityWattsDialogueIndex);
            newEventBC.Jump(0x23e);
            newEventBC.End();


            // 0x23d: watts in upper land
            EventScript newEvent23d = new EventScript();
            context.replacementEvents[0x23d] = newEvent23d;
            int upperLandWattsDialogueIndex = r.Next() % allWattsDialogue.Count;
            newEvent23d.AddDialogueBox(allWattsDialogue[upperLandWattsDialogueIndex]);
            allWattsDialogue.RemoveAt(upperLandWattsDialogueIndex);
            newEvent23d.Jump(0x23E);
            newEvent23d.End();


            // 0x2bc: watts in kakkara
            EventScript newEvent2bc = new EventScript();
            context.replacementEvents[0x2bc] = newEvent2bc;
            int kakkaraWattsDialogueIndex = r.Next() % allWattsDialogue.Count;
            newEvent2bc.AddDialogueBox(allWattsDialogue[kakkaraWattsDialogueIndex]);
            allWattsDialogue.RemoveAt(kakkaraWattsDialogueIndex);
            newEvent2bc.Jump(0x23E);
            newEvent2bc.End();


            // 0x3d9: watts in mandala
            EventScript newEvent3d9 = new EventScript();
            context.replacementEvents[0x3d9] = newEvent3d9;
            int mandalaWattsDialogueIndex = r.Next() % allWattsDialogue.Count;
            newEvent3d9.AddDialogueBox(allWattsDialogue[mandalaWattsDialogueIndex]);
            allWattsDialogue.RemoveAt(mandalaWattsDialogueIndex);
            newEvent3d9.Jump(0x23E);
            newEvent3d9.End();


            // 0x375: watts in ice country
            EventScript newEvent375 = new EventScript();
            context.replacementEvents[0x375] = newEvent375;
            newEvent375.Add(EventCommandEnum.OPEN_DIALOGUE.Value);
            int snowWattsDialogueIndex = r.Next() % allWattsDialogue.Count;
            newEvent375.AddDialogueBox(allWattsDialogue[snowWattsDialogueIndex]);
            allWattsDialogue.RemoveAt(snowWattsDialogueIndex);
            newEvent375.Jump(0x23E);
            newEvent375.End();


            // 0x267: watts in matango; occasionally he is hungry
            EventScript newEvent267 = new EventScript();
            context.replacementEvents[0x267] = newEvent267;
            List<byte> mushroom2Dialogue = VanillaEventUtil.getBytes("WATTS: man i bet these\nmushrooms are delicious");
            if ((r.Next() % 3) != 0)
            {
                int mushroomWattsDialogueIndex = r.Next() % allWattsDialogue.Count;
                newEvent267.AddDialogueBox(allWattsDialogue[mushroomWattsDialogueIndex]);
                allWattsDialogue.RemoveAt(mushroomWattsDialogueIndex);
            }
            else
            {
                newEvent267.AddDialogueBox("WATTS: man i bet these\nmushrooms are delicious");
            }
            newEvent267.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent267.Add(0x80);
            newEvent267.Add(0x10);
            newEvent267.Add(0x08);
            newEvent267.Add(EventCommandEnum.MOVE_CHARACTER.Value);
            newEvent267.Add(0x80);
            newEvent267.Add(0x40);
            newEvent267.Add(0x08);
            newEvent267.Jump(0x23e);
            newEvent267.End();


            // 0x378: watts in northtown
            EventScript newEvent378 = new EventScript();
            context.replacementEvents[0x378] = newEvent378;
            newEvent378.Jsr(0x480);
            newEvent378.Add(EventCommandEnum.OPEN_GP.Value);
            newEvent378.Add(0x5F);
            int ntWattsDialogueIndex = r.Next() % allWattsDialogue.Count;
            newEvent378.AddDialogueBox(allWattsDialogue[ntWattsDialogueIndex]);
            allWattsDialogue.RemoveAt(ntWattsDialogueIndex);
            newEvent378.Add(EventCommandEnum.WEAPON_UPGRADE_MENU.Value);
            newEvent378.Add(EventCommandEnum.CLOSE_GP.Value);
            newEvent378.End();


            // 0x3b7: the lady next to watts in northtown
            EventScript newEvent3b7 = new EventScript();
            context.replacementEvents[0x3b7] = newEvent3b7;
            newEvent3b7.AddDialogueBox("Sorry, silly hats only.\nget out of here");
            newEvent3b7.Door(0xC5); // out!
            newEvent3b7.End();


            // 0x18d: npc in pandora castle
            EventScript newEvent18d = new EventScript();
            context.replacementEvents[0x18d] = newEvent18d;
            newEvent18d.AddDialogueBox("Oh, am I in your way?\nHuh. Sure hope I move\nsoon then");
            newEvent18d.End();


            // 0x24a: moogle in upperlands
            EventScript newEvent24a = new EventScript();
            context.replacementEvents[0x24a] = newEvent24a;
            newEvent24a.AddDialogueBox("Moogle: Man, Mana moogles\nare so useless. I wish\nI was an FF6 moogle.\n...\nI mean, [Kupo.]");
            newEvent24a.End();


            // 0x4ab: jema outside the dryad palace
            EventScript newEvent4ab = new EventScript();
            context.replacementEvents[0x4ab] = newEvent4ab;
            // no "open dialogue" here since other events do it then jump here
            string sunkenContinentJemaDialogue = pickRandomDialogue(r, new string[]
            {
                "So, like, am I wearing\na helmet? Or is this\njust how my hair looks?",
                VanillaEventUtil.wordWrapText("I'm not a hint. Leave me alone."),
                VanillaEventUtil.wordWrapText("I bet I would have been playable in the SNES CD version"),
            }
            );
            newEvent4ab.AddDialogue(sunkenContinentJemaDialogue);
            newEvent4ab.CloseDialogueBox();
            newEvent4ab.End();


            // 0x118: potos npc A
            EventScript newEvent118 = new EventScript();
            context.replacementEvents[0x118] = newEvent118;
            string potosDialogue1 = pickRandomDialogue(r, new string[]
            {
                "Who let you back in?",
                VanillaEventUtil.wordWrapText("So, uh, when were you leaving again?"),
                VanillaEventUtil.wordWrapText("Weren't you supposed to be banished?"),
            });
            newEvent118.AddDialogueBox(potosDialogue1);
            newEvent118.End();


            // 0x115: potos npc B
            EventScript newEvent115 = new EventScript();
            context.replacementEvents[0x115] = newEvent115;
            string[] potosDialogues2 = new string[]
            {
                "????\nDidn't we throw you out?",
                VanillaEventUtil.wordWrapText("Sure taking your sweet time leaving town, aren't you?"),
                VanillaEventUtil.wordWrapText("Oh sure, invite yourself right back into town"),
            };
            string potosDialogue2 = pickRandomDialogue(r, potosDialogues2);
            newEvent115.AddDialogueBox(potosDialogue2);
            newEvent115.End();


            // 0x116: potos npc B
            EventScript newEvent116 = new EventScript();
            context.replacementEvents[0x116] = newEvent116;
            string potosDialogue3 = pickRandomDialogue(r, potosDialogues2);
            newEvent116.AddDialogueBox(potosDialogue3);
            newEvent116.End();


            // 0x1a0: pandora king has no info for you
            EventScript newEvent1a0 = new EventScript();
            context.replacementEvents[0x1a0] = newEvent1a0;
            newEvent1a0.AddDialogueBox(VanillaEventUtil.wordWrapText("Look not every NPC has custom dialogue in open world okay"));
            newEvent1a0.End();


            // 0x268: i think this was the mushroom next to watts
            EventScript newEvent268 = new EventScript();
            context.replacementEvents[0x268] = newEvent268;
            newEvent268.AddDialogueBox("blerf");
            newEvent268.End();


            // 0x262: purple matango mushroom
            EventScript newEvent262 = new EventScript();
            context.replacementEvents[0x262] = newEvent262;
            newEvent262.AddDialogueBox("hi i'm the only\npurple one");
            newEvent262.End();


            // 0x4e3: matango king. this is leftover dialogue from vanilla rando mode
            EventScript newEvent4E3 = new EventScript();
            context.replacementEvents[0x4E3] = newEvent4E3;
            newEvent4E3.AddDialogueBox("Go do the thing");
            newEvent4E3.End();


            // 0xb3: the guy in gold city who you normally talk to for the key and takes forever to walk to
            EventScript newEventB3 = new EventScript();
            context.replacementEvents[0xB3] = newEventB3;
            newEventB3.AddDialogue("well wasn't that a fun walk");
            newEventB3.End();


            // 0x119: annoying potos girl by the chest
            EventScript newEvent119 = new EventScript();
            context.replacementEvents[0x119] = newEvent119;
            newEvent119.AddDialogueBox("I'm just here to\nget in your way.");
            newEvent119.End();


            // 0x3e7: mandala watts where he's just settled in with some random people
            EventScript newEvent3d7 = new EventScript();
            context.replacementEvents[0x3d7] = newEvent3d7;
            newEvent3d7.AddDialogueBox(VanillaEventUtil.wordWrapText("Please get this dwarf out of my house"));
            newEvent3d7.End();


            // 0x3e8: mandala watts where he's just settled in with some random people
            EventScript newEvent3d8 = new EventScript();
            context.replacementEvents[0x3d8] = newEvent3d8;
            newEvent3d8.AddDialogueBox(VanillaEventUtil.wordWrapText("Oh sure, invite yourself right in. Everyone else is."));
            newEvent3d8.End();


            // 0x3e3: mandala guy with a weird shaped head
            EventScript newEvent3e3 = new EventScript();
            context.replacementEvents[0x3e3] = newEvent3e3;
            newEvent3e3.AddDialogueBox(VanillaEventUtil.wordWrapText("My head? Don't worry, it's just a flesh-colored hat."));
            newEvent3e3.End();


            // 0x3e2: guy outside the mandala hint orbs room
            EventScript newEvent3e2 = new EventScript();
            context.replacementEvents[0x3e2] = newEvent3e2;
            newEvent3e2.AddDialogueBox(VanillaEventUtil.wordWrapText("Free hints inside! No guarantees they're useful."));
            newEvent3e2.End();


            // 0x3e6: mandala guy in a secret-ish spot
            EventScript newEvent3e6 = new EventScript();
            context.replacementEvents[0x3e6] = newEvent3e6;
            newEvent3e6.AddDialogueBox(VanillaEventUtil.wordWrapText("This is my pointless secret spot, go find your own"));
            newEvent3e6.End();


            // 0x3d2: kid in mandala with a weird hat
            EventScript newEvent3d2 = new EventScript();
            context.replacementEvents[0x3d2] = newEvent3d2;
            newEvent3d2.AddDialogueBox(VanillaEventUtil.wordWrapText("I'm actually wearing a whole Rabite on my head"));
            newEvent3d2.End();


            // 0x220: i actually don't remember what this town is called
            EventScript newEvent220 = new EventScript();
            context.replacementEvents[0x220] = newEvent220;
            newEvent220.AddDialogueBox(VanillaEventUtil.wordWrapText("Welcome to.. uh. What's this place called again?"));
            newEvent220.End();


            // 0x222: girl near landing spot in uhh, that town near gaia's navel
            EventScript newEvent222 = new EventScript();
            context.replacementEvents[0x222] = newEvent222;
            newEvent222.AddDialogueBox("This place is completely\npointless lol");
            newEvent222.End();


            // 0x224: cannon guy in some house somewhere
            EventScript newEvent224 = new EventScript();
            context.replacementEvents[0x224] = newEvent224;
            newEvent224.AddDialogueBox("Blorf");
            newEvent224.End();


            // 0x225: dyluck father
            EventScript newEvent225 = new EventScript();
            context.replacementEvents[0x225] = newEvent225;
            newEvent225.AddDialogueBox(VanillaEventUtil.wordWrapText("Don't we seem a little old to be Dyluck's parents"));
            newEvent225.End();


            // 0x226: dyluck mother
            EventScript newEvent226 = new EventScript();
            context.replacementEvents[0x226] = newEvent226;
            newEvent226.AddDialogueBox(VanillaEventUtil.wordWrapText("Hey, how's my favorite character Dyluck doing?"));
            newEvent226.End();


            // 0x183: fella standing next to the pandora throne all the time
            EventScript newEvent183 = new EventScript();
            context.replacementEvents[0x183] = newEvent183;
            newEvent183.AddDialogueBox(VanillaEventUtil.wordWrapText("Sometimes late at night I sit in the chair and pretend I'm king"));
            newEvent183.End();


            // 0x20a: basement luka
            EventScript newEvent20A = new EventScript();
            context.replacementEvents[0x20A] = newEvent20A;
            newEvent20A.AddDialogueBox("lol i'm also down here");
            newEvent20A.End();

            return true;
        }

        private static string pickRandomDialogue(Random r, string[] choices)
        {
            return choices[r.Next() % choices.Length];
        }

    }
}
