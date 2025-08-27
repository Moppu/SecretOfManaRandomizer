using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Custom neko dialogue for open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NekoEvents : RandoProcessor
    {
        protected override string getName()
        {
            return "Neko event changes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;

            List<string> allNekoDialogue = new string[]
            {
                "[Neko] pronunciation:\n[Nico] or [Neck-o]? Discuss\namongst yourselves.",
                VanillaEventUtil.wordWrapText("I also accept cat treats and fish as currency"),
                VanillaEventUtil.wordWrapText("(Startled, Neko puffs himself up to look larger and more threatening)"),
                VanillaEventUtil.wordWrapText("Break me off a piece of that Fancy Feast"),
                "Nico-nico-nii.",
                VanillaEventUtil.wordWrapText("(Neko finishes coughing up a hairball, then shows you his items)"),
                VanillaEventUtil.wordWrapText("Seriously? They named me [Neko?] It just means [Cat.]"),
                "Meow mix, meow mix\nPlease deliver",
                "Thanks for subscribing\nto cat facts! Text\nSTOP at any time to stop.",
                VanillaEventUtil.wordWrapText("Bring me catnip and I'll consider lowering my prices"),
                VanillaEventUtil.wordWrapText("(Neko hisses and raises his prices even more)"),
                VanillaEventUtil.wordWrapText("I'm the third strongest of all the Nekos."),
            }.ToList();


            // -------------------------------------------
            // 0x1d7: gaia's navel neko dialogue
            // -------------------------------------------
            EventScript newEvent1d7 = new EventScript();
            context.replacementEvents[0x1d7] = newEvent1d7;
            newEvent1d7.OpenDialogueBox();
            int navelNekoDialogueIndex = r.Next() % allNekoDialogue.Count;
            // no "close" here since 1DF is a choice box
            newEvent1d7.AddDialogue(allNekoDialogue[navelNekoDialogueIndex], null); // no wait because 1DF waits
            allNekoDialogue.RemoveAt(navelNekoDialogueIndex);
            newEvent1d7.Jump(0x1DF);
            newEvent1d7.End();


            // -------------------------------------------
            // 0x318: mountain neko dialogue
            // -------------------------------------------
            int mountainNekoDialogueIndex = r.Next() % allNekoDialogue.Count;
            EventScript newEvent318 = new EventScript();
            context.replacementEvents[0x318] = newEvent318;
            newEvent318.SetFlag(EventFlags.BUY_SHOP_ID_FLAG, 0xC);
            newEvent318.Add(EventCommandEnum.OPEN_DIALOGUE.Value);
            newEvent318.AddDialogue(allNekoDialogue[mountainNekoDialogueIndex]);
            newEvent318.Add(EventCommandEnum.CLEAR_DIALOGUE.Value);
            newEvent318.Add(0x06);
            newEvent318.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent318.Add(0x08); // center camera
            newEvent318.Add(EventCommandEnum.OPEN_GP.Value);
            newEvent318.Add(0x5F);
            addSaveBuySell(newEvent318, 0x312);
            newEvent318.End();


            // -------------------------------------------
            // 0x319: lost continent neko dialogue
            // -------------------------------------------
            EventScript newEvent319 = new EventScript();
            context.replacementEvents[0x319] = newEvent319;
            newEvent319.OpenDialogueBox();
            int dryadNekoDialogueIndex = r.Next() % allNekoDialogue.Count;
            newEvent319.AddDialogue(allNekoDialogue[dryadNekoDialogueIndex]);
            allNekoDialogue.RemoveAt(dryadNekoDialogueIndex);
            newEvent319.Jsr(0x480);
            newEvent319.Add(EventCommandEnum.OPEN_GP.Value);
            newEvent319.Add(0x5F);
            addBuySell(newEvent319, 0x30F);
            newEvent319.End();


            // -------------------------------------------
            // 0x1de: witch's castle neko dialogue
            // -------------------------------------------
            EventScript newEvent1de = new EventScript();
            context.replacementEvents[0x1de] = newEvent1de;
            newEvent1de.OpenDialogueBox();
            int prisonNekoDialogueIndex = r.Next() % allNekoDialogue.Count;
            newEvent1de.AddDialogue(allNekoDialogue[prisonNekoDialogueIndex], null); // no wait because 1DF waits
            allNekoDialogue.RemoveAt(prisonNekoDialogueIndex);
            newEvent1de.Jump(0x1DF);
            newEvent1de.End();


            // -------------------------------------------
            // 0x23c: upper land neko dialogue
            // -------------------------------------------
            EventScript newEvent23c = new EventScript();
            context.replacementEvents[0x23c] = newEvent23c;
            newEvent23c.OpenDialogueBox();
            int upperLandNekoDialogueIndex = r.Next() % allNekoDialogue.Count;
            newEvent23c.AddDialogue(allNekoDialogue[upperLandNekoDialogueIndex]);
            allNekoDialogue.RemoveAt(upperLandNekoDialogueIndex);
            newEvent23c.Add(EventCommandEnum.CLEAR_DIALOGUE.Value);
            newEvent23c.Jsr(0x480);
            newEvent23c.Add(EventCommandEnum.OPEN_GP.Value);
            newEvent23c.Add(0x5F);
            addSaveBuySell(newEvent23c, 0x306);
            newEvent23c.End();


            // -------------------------------------------
            // 0x65f: ice country neko dialogue
            // -------------------------------------------
            EventScript newEvent65f = new EventScript();
            context.replacementEvents[0x65f] = newEvent65f;
            newEvent65f.SetFlag(EventFlags.BUY_SHOP_ID_FLAG, 8);
            newEvent65f.Add(0x06);
            newEvent65f.Add(EventCommandEnum.GRAPHICAL_EFFECT.Value);
            newEvent65f.Add(0x08); // center camera
            newEvent65f.OpenDialogueBox();
            int iceCountryNekoDialogueIndex = r.Next() % allNekoDialogue.Count;
            newEvent65f.AddDialogue(allNekoDialogue[iceCountryNekoDialogueIndex]);
            allNekoDialogue.RemoveAt(iceCountryNekoDialogueIndex);
            newEvent65f.Add(EventCommandEnum.CLEAR_DIALOGUE.Value);
            newEvent65f.Jsr(0x480);
            newEvent65f.Add(EventCommandEnum.OPEN_GP.Value);
            newEvent65f.Add(0x5F);
            addSaveBuySell(newEvent65f, 0x308);
            newEvent65f.End();
            return true;
        }

        private void addBuySell(EventScript eventData, int buyMenuEvent)
        {
            eventData.AddDialogueWithChoices("Buy some more crap?\n   (", new byte[] { 5, 10 }, new string[] { "Buy", "Sell )" }, new byte[][] { EventScript.GetJumpCmd(buyMenuEvent), EventScript.GetJumpCmd(0x313) }, EventScript.GetJumpCmd(0xFF));
        }

        private void addSaveBuySell(EventScript eventData, int buyMenuEvent)
        {
            eventData.AddDialogueWithChoices("   Never trust autosave!\n    Buy some more crap?\n   (", new byte[] { 5, 12, 18 }, new string[] { "Save", "Buy", "Sell )" }, new byte[][] { EventScript.GetJumpCmd(0x4F), EventScript.GetJumpCmd(buyMenuEvent), EventScript.GetJumpCmd(0x313) }, EventScript.GetJumpCmd(0xFF));
        }
    }
}
