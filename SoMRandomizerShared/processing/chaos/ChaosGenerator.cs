using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.enhancement;
using SoMRandomizer.processing.hacks.common.music;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.processing.hacks.common.procgen;
using System;

namespace SoMRandomizer.processing.chaos
{
    /// <summary>
    /// Top-level class used to generate chaos mode ROMs.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosGenerator : RomGenerator
    {
        public ChaosGenerator()
        {
            // modify starting door to 0x0a for chaos
            addModeSpecificHack(new StartingDoor());
            // expand doors and move them in rom
            addModeSpecificHack(new DoorExpansion());
            // ability to sell all unused gear at neko
            addModeSpecificHack(new SellAllGear());
            // show timer on layer 3
            addModeSpecificHack(new Layer3Changes());
            // allow printing of timer in dialogue events
            addModeSpecificHack(new TimerDialogue());
            // make events for chaos mode
            addModeSpecificHack(new ChaosEventMaker());
            // make maps for chaos mode
            addModeSpecificHack(new ChaosRandomizer());
            // enemy scaling for chaos mode
            addModeSpecificHack(new EnemyChanges());
            // drop table changes for chaos mode
            addModeSpecificHack(new DropChanges());
            // shop changes for chaos mode
            addModeSpecificHack(new ShopChanges());
            // death event changes for chaos mode
            addModeSpecificHack(new DeathEventDoorwayRemoval());
            // magic rope changes for chaos mode
            addModeSpecificHack(new MagicRope());
            // magic defense adjustment for chaos mode
            addModeSpecificHack(new MagDefAdjustment());
            // enemy magic power adjustment for chaos mode
            addModeSpecificHack(new EnemyBlackMagicPower());
            // keep track of statistics
            addModeSpecificHack(new Statistics());
            // stick game mode and my name on the title screen
            addModeSpecificHack(new TitleScreenAddition());
            // randomize music
            addModeSpecificHack(new CustomMusic());
        }

        protected override void generate(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context)
        {
            applyHacks(origRom, outRom, seed, settings, context);
        }
    }
}
