using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Top-level event replacer for open world, including all necessary dialogue, prize locations, shops, and other stuff.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldEvents : RandoProcessor
    {
        protected override string getName()
        {
            return "Open world events";
        }

        // intro sequence:
        // new game menu runs door 0x75
        // this loads map 8, which is an empty map with a walk-on trigger event of 0x100
        // event 0x100 loads 0x400 if you haven't started the game (event flag 0x00 == 0)
        // note that if you HAVE started the game (somehow), it runs event 0x1ff instead which dumps you in potos
        // event 0x400 is the cutscene event that plays in the vanilla game that shows the mana fort and other stuff,
        // and eventually runs door 0x00 and event 0x106, which gets us to the waterfall event
        // this runs event 0x101, which is the waterfall fall and recovery
        // touching the sword is event 0x103, which sets flag 0x00 on, enabling the UI, gives you the sword, etc
        // ..
        // for the most part we keep the same sequence here but cut a lot of shit out
        // also for christmas modes, we combine event 0x103 in the startup to give the sword immediately
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // event 0x400 -> call event 0x106
            new NewGameEvent().add(origRom, outRom, seed, settings, context);
            // event 0x106, and in some cases 0x101.
            // depending on options this may call 0x103 (sword grab event) directly,
            // or end and wait for you to grab it.
            // also populates event 0x107, which we now use as the initialization of event flags.
            new IntroEvent().add(origRom, outRom, seed, settings, context);
            // initialization after grabbing sword, or being given it by automated startup
            new SwordGrabEvent().add(origRom, outRom, seed, settings, context);
            // events associated with open world prizes, into which randomized prizes will be injected later
            new PrizeEvents().add(origRom, outRom, seed, settings, context);
            // hint events for open world, into which hint text will be injected later
            new HintEvents().add(origRom, outRom, seed, settings, context);
            // that lady in pandora that tells you about pokemon
            new PokedexEvent().add(origRom, outRom, seed, settings, context);
            // neko stuff
            new NekoEvents().add(origRom, outRom, seed, settings, context);
            // changes to events in support of open world progress
            new OpenWorldSupportingEvents().add(origRom, outRom, seed, settings, context);
            // changes to object positioning etc on maps to facilitate open world progress
            new OpenWorldSupportingMapChanges().add(origRom, outRom, seed, settings, context);
            // stupid dialogue changes and other mostly pointless stuff
            new MiscDialogue().add(origRom, outRom, seed, settings, context);

            return true;
        }
    }
}
