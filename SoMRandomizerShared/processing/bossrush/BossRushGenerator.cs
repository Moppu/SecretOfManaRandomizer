using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.bossrush;
using SoMRandomizer.processing.hacks.common.enhancement;
using SoMRandomizer.processing.hacks.common.music;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.processing.hacks.common.procgen;
using System;

namespace SoMRandomizer.processing.bossrush
{
    /// <summary>
    /// Top-level class used to generate boss rush ROMs.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BossRushGenerator : RomGenerator
    {
        public BossRushGenerator()
        {
            // modify starting door to 0x0a for boss rush
            addModeSpecificHack(new StartingDoor());
            // allow more doors and change their location in rom
            addModeSpecificHack(new DoorExpansion());
            // this needs to be before the main AC randomization, so the offset is available to modify animation settings
            addModeSpecificHack(new AnimatedPaletteSimplification());
            // show timer on screen
            addModeSpecificHack(new Layer3Changes());
            // print dialogue in event text
            addModeSpecificHack(new TimerDialogue());
            // choose the randomized boss order
            addModeSpecificHack(new BossOrderRandomizer());
            // generate events for boss rush
            addModeSpecificHack(new BossRushEventGenerator());
            // generate maps for boss rush
            addModeSpecificHack(new BossRushRandomizer());
            // scale mana beast weapon damage down
            addModeSpecificHack(new ManaBeastDamageScaling());
            // boss scaling for boss rush
            addModeSpecificHack(new EnemyChanges());
            // drops table by floor
            addModeSpecificHack(new DropChanges());
            // randomized shops
            addModeSpecificHack(new ShopChanges());
            // modify starting level for boss rush
            addModeSpecificHack(new StartingLevel());
            // change death doorway for boss rush
            addModeSpecificHack(new DeathEventDoorwayRemoval());
            // magic rope changes for boss rush
            addModeSpecificHack(new MagicRope());
            // magic defense adjustment for boss rush
            addModeSpecificHack(new MagDefAdjustment());
            // enemy magic power adjustment for boss rush
            addModeSpecificHack(new EnemyBlackMagicPower());
            // ability to print statistics for the run
            addModeSpecificHack(new Statistics());
            // limit mp absorb spell to only take 3 mp max
            addModeSpecificHack(new MpAbsorbLimiter());
            // remove unlimited mp for mode 7 bosses
            addModeSpecificHack(new LimitMode7BossMp());
            // give everyone charge levels as soon as the weapon is upgraded at watts
            addModeSpecificHack(new InstantWeaponLevels());
            // put the mode name and my name on the title screen
            addModeSpecificHack(new TitleScreenAddition());
            // random music
            addModeSpecificHack(new CustomMusic());
        }

        protected override void generate(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context)
        {
            applyHacks(origRom, outRom, seed, settings, context);
        }
    }
}
