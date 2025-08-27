using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Open world hint event changes for injection later. See OpenWorldHints for where the actual hints are injected.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class HintEvents : RandoProcessor
    {
        public static byte[] OPENWORLD_HINT_INJECTION_PATTERN = new byte[] { 0x23, 0x45, 0x67, 0x89 };

        private static void injectHintPattern(EventScript eventData)
        {
            eventData.OpenDialogueBox();
            foreach (byte b in OPENWORLD_HINT_INJECTION_PATTERN)
            {
                eventData.Add(b);
            }
            eventData.DialogueWait();
            eventData.CloseDialogueBox();
        }

        protected override string getName()
        {
            return "Open world hint events";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // make krissie show up upstairs for hints
            int northtownCafeMapNum = MAPNUM_NORTHTOWN_INTERIOR_B;
            int mapObjOffset = 0x80000 + outRom[0x87000 + northtownCafeMapNum * 2] + (outRom[0x87000 + northtownCafeMapNum * 2 + 1] << 8);
            mapObjOffset += 8; // skip header
            outRom[mapObjOffset + 8 * 0 + 0] = 0; // adjust event data of obj 0
            outRom[mapObjOffset + 8 * 0 + 1] = 0xFF; // adjust event data of obj 0
            outRom[mapObjOffset + 8 * 5 + 0] = 0; // adjust event data of obj 5
            outRom[mapObjOffset + 8 * 5 + 1] = 0x0F; // adjust event data of obj 5 - event 3Ce - use this

            // event 3Ce - upstairs krissie
            EventScript newEvent3ce = new EventScript();
            context.replacementEvents[0x3ce] = newEvent3ce;
            injectHintPattern(newEvent3ce);
            newEvent3ce.End();

            // hint orbs at mandala
            EventScript newEvent3e9 = new EventScript();
            context.replacementEvents[0x3e9] = newEvent3e9;
            injectHintPattern(newEvent3e9);
            newEvent3e9.End();

            EventScript newEvent3ea = new EventScript();
            context.replacementEvents[0x3ea] = newEvent3ea;
            injectHintPattern(newEvent3ea);
            newEvent3ea.End();

            EventScript newEvent3eb = new EventScript();
            context.replacementEvents[0x3eb] = newEvent3eb;
            injectHintPattern(newEvent3eb);
            newEvent3eb.End();

            EventScript newEvent3ec = new EventScript();
            context.replacementEvents[0x3ec] = newEvent3ec;
            injectHintPattern(newEvent3ec);
            newEvent3ec.End();

            EventScript newEvent3ed = new EventScript();
            context.replacementEvents[0x3ed] = newEvent3ed;
            injectHintPattern(newEvent3ed);
            newEvent3ed.End();

            EventScript newEvent3ee = new EventScript();
            context.replacementEvents[0x3ee] = newEvent3ee;
            injectHintPattern(newEvent3ee);
            newEvent3ee.End();

            EventScript newEvent3ef = new EventScript();
            context.replacementEvents[0x3ef] = newEvent3ef;
            injectHintPattern(newEvent3ef);
            newEvent3ef.End();

            // rudolph, map 332, keep [0], remove [1], use event 37c for a hint
            int rudolphMapNum = MAPNUM_SANTAHOUSE_EXTERIOR;
            mapObjOffset = 0x80000 + outRom[0x87000 + rudolphMapNum * 2] + (outRom[0x87000 + rudolphMapNum * 2 + 1] << 8);
            mapObjOffset += 8; // skip header
            outRom[mapObjOffset + 8 * 0 + 0] = 0; // adjust event data of obj 0
            outRom[mapObjOffset + 8 * 0 + 1] = 0x0F; // adjust event data of obj 0
            outRom[mapObjOffset + 8 * 1 + 0] = 0; // adjust event data of obj 1
            outRom[mapObjOffset + 8 * 1 + 1] = 0xFF; // adjust event data of obj 1

            EventScript newEvent37c = new EventScript();
            context.replacementEvents[0x37c] = newEvent37c;
            injectHintPattern(newEvent37c);
            newEvent37c.End();

            // guy at the flowers near witch place
            EventScript newEvent21c = new EventScript();
            context.replacementEvents[0x21c] = newEvent21c;
            injectHintPattern(newEvent21c);
            newEvent21c.End();

            // dyluck - give a hint instead of the usual bullshit
            int dyluckMapNum = MAPNUM_NTR_INTERIOR_C;
            mapObjOffset = 0x80000 + outRom[0x87000 + dyluckMapNum * 2] + (outRom[0x87000 + dyluckMapNum * 2 + 1] << 8);
            mapObjOffset += 8; // skip header
            outRom[mapObjOffset + 8 * 0 + 0] = 0; // adjust event data of obj 0
            outRom[mapObjOffset + 8 * 0 + 1] = 0x0F; // adjust event data of obj 0

            EventScript newEvent555 = new EventScript();
            context.replacementEvents[0x555] = newEvent555;
            injectHintPattern(newEvent555);
            newEvent555.End();

            // potos elder is a hint - 114
            EventScript newEvent114 = new EventScript();
            context.replacementEvents[0x114] = newEvent114;
            injectHintPattern(newEvent114);
            newEvent114.End();

            // 3f flag set to 6 by dopple fight
            // x3dc x3dd jehk johk
            EventScript newEvent3dd = new EventScript();
            context.replacementEvents[0x3dd] = newEvent3dd;
            injectHintPattern(newEvent3dd);
            newEvent3dd.End();

            EventScript newEvent3dc = new EventScript();
            context.replacementEvents[0x3dc] = newEvent3dc;
            injectHintPattern(newEvent3dc);
            newEvent3dc.End();

            // tasnica king
            EventScript newEvent2d7 = new EventScript();
            context.replacementEvents[0x2d7] = newEvent2d7;
            injectHintPattern(newEvent2d7);
            newEvent2d7.End();

            // signs
            // potos cannon travel - 606
            EventScript newEvent606 = new EventScript();
            context.replacementEvents[0x606] = newEvent606;
            injectHintPattern(newEvent606);
            newEvent606.End();

            // water palace neko's - 607
            EventScript newEvent607 = new EventScript();
            context.replacementEvents[0x607] = newEvent607;
            injectHintPattern(newEvent607);
            newEvent607.End();

            // beward of goblins sign - 608
            EventScript newEvent608 = new EventScript();
            context.replacementEvents[0x608] = newEvent608;
            injectHintPattern(newEvent608);
            newEvent608.End();

            // three way potos sign - 600
            EventScript newEvent600 = new EventScript();
            context.replacementEvents[0x600] = newEvent600;
            injectHintPattern(newEvent600);
            newEvent600.End();

            // pandora sign - 601
            EventScript newEvent601 = new EventScript();
            context.replacementEvents[0x601] = newEvent601;
            injectHintPattern(newEvent601);
            newEvent601.End();

            // kippo sign - 602
            EventScript newEvent602 = new EventScript();
            context.replacementEvents[0x602] = newEvent602;
            injectHintPattern(newEvent602);
            newEvent602.End();

            // gaia's navel sign - 609
            EventScript newEvent609 = new EventScript();
            context.replacementEvents[0x609] = newEvent609;
            injectHintPattern(newEvent609);
            newEvent609.End();

            // water palace sign - 604
            EventScript newEvent604 = new EventScript();
            context.replacementEvents[0x604] = newEvent604;
            injectHintPattern(newEvent604);
            newEvent604.End();

            // potos forest sign - 605
            EventScript newEvent605 = new EventScript();
            context.replacementEvents[0x605] = newEvent605;
            injectHintPattern(newEvent605);
            newEvent605.End();

            return true;
        }
    }
}
