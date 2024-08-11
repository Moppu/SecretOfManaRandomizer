using System;
using System.Collections.Generic;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.processing.hacks.ancientcave;
using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.procgen;
using SoMRandomizer.processing.hacks.common.enhancement;
using SoMRandomizer.processing.hacks.common.music;
using SoMRandomizer.processing.hacks.openworld;

namespace SoMRandomizer.processing.ancientcave
{
    /// <summary>
    /// Top-level class used to generate ancient cave ROMs.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AncientCaveGenerator : RomGenerator
    {
        public AncientCaveGenerator()
        {
            // modify starting door to the first one we generate (0x0a)
            addModeSpecificHack(new StartingDoor());
            // this needs to be before the main AC randomization, so the offset is available to set continue doors
            addModeSpecificHack(new DoorExpansion());
            // this needs to be before the main AC randomization, so the offset is available to modify animation settings
            addModeSpecificHack(new AnimatedPaletteSimplification());
            // tileset changes for island maps
            addModeSpecificHack(new AcIslandTilesetChanges());
            // tileset changes for cave maps
            addModeSpecificHack(new AcCaveTilesetChanges());
            // tileset changes for manafort interior maps
            addModeSpecificHack(new AcManafortTilesetChanges());
            // show timer and floor number on screen
            addModeSpecificHack(new Layer3Changes());
            // allow timer value to be printed in the ending sequence
            addModeSpecificHack(new TimerDialogue());
            // sell all unused gear option at neko
            addModeSpecificHack(new SellAllGear());
            // actual randomization and map generation for ancient cave mode
            addModeSpecificHack(new AncientCaveRandomizer());
            // enemy scaling for ancient cave
            addModeSpecificHack(new EnemyChanges());
            // enemy drops for ancient cave
            addModeSpecificHack(new DropChanges());
            // random shops for ancient cave
            addModeSpecificHack(new ShopChanges());
            // changes for death in ancient cave
            addModeSpecificHack(new DeathEventDoorwayRemoval());
            // changes for magic rope in ancient cave
            addModeSpecificHack(new MagicRope());
            // adjustments to magic defense for ancient cave
            addModeSpecificHack(new MagDefAdjustment());
            // adjustments to enemy magic power for ancient cave
            addModeSpecificHack(new EnemyBlackMagicPower());
            // print statistics at the end of the run
            addModeSpecificHack(new Statistics());
            // stick my name and the game mode on the title screen
            addModeSpecificHack(new TitleScreenAddition());
            // randomize music
            addModeSpecificHack(new CustomMusic());
        }

        public static Dictionary<string, int> LENGTH_CONVERSIONS = new Dictionary<string, int> { { "short", 8 }, { "medium", 16 }, { "long", 24 }, };
        protected override void generate(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context)
        {
            applyHacks(origRom, outRom, seed, settings, context);
        }
    }
}
