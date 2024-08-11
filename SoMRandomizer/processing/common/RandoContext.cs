using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// A structure of shared information to be used by various randomizations and hacks while generating a rom.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class RandoContext
    {
        // random used for functional stuff
        public Random randomFunctional;
        // random used for cosmetic stuff; separate so that changing cosmetics will not impact functional rando
        public Random randomCosmetic;
        // vanilla rom
        public byte[] originalRom;
        // rando rom
        public byte[] outputRom;
        // offset in rom to put custom shit
        public int workingOffset = 0x200000;
        // settings shared between hacks
        public StringValueSettings workingData = new StringValueSettings();
        // plando (currently for open world only)
        public Dictionary<string, List<string>> plandoSettings = new Dictionary<string, List<string>>();
        // dialogue/events replaced for rando, by event id
        public Dictionary<int, List<byte>> replacementEvents = new Dictionary<int, List<byte>>();
        // pre-compressed with SomMapCompression
        public Dictionary<int, List<byte>> replacementMapPieces = new Dictionary<int, List<byte>>();
        // changes to names of things (which are also technically events)
        public NamesOfThings namesOfThings;
        // values to set up when you start a new game; ram offset->value
        public Dictionary<int, byte> initialValues = new Dictionary<int, byte>();
        // custom event types needed for rando
        public CustomEventManager eventHackMgr = new CustomEventManager();
        // generated full maps
        public Dictionary<int, FullMap> generatedMaps = new Dictionary<int, FullMap>();
        // generated doors
        public Dictionary<int, Door> replacementDoors = new Dictionary<int, Door>();
        // generated map palette sets
        public Dictionary<int, MapPaletteSet> replacementMapPalettes = new Dictionary<int, MapPaletteSet>();
    }
}
