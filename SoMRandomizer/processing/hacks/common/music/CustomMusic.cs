using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.procgen;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.hacks.common.music.ExternalMusic;
using static SoMRandomizer.processing.hacks.common.music.ExternalSamples;

namespace SoMRandomizer.processing.hacks.common.music
{
    /// <summary>
    /// Import custom music and randomize music, varying somewhat depending on game mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CustomMusic : RandoProcessor
    {
        public static string SONGTYPELABEL_OVERWORLD = "Overworld";
        public static string SONGTYPELABEL_DUNGEON = "Dungeon";
        public static string SONGTYPELABEL_BATTLE = "Battle";
        public static string SONGTYPELABEL_TOWN = "Town";
        public static string SONGTYPELABEL_FLIGHT = "Flight";
        public static string SONGTYPELABEL_PALACE = "Palace";
        public static string SONGTYPELABEL_VICTORY = "Victory";
        public static string SONGTYPELABEL_DEBUG = "Debug";

        protected override string getName()
        {
            return "Music rando + imported music";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool addFlammieDrumMusicHack = false;
            bool addMusicRandomizer = true;

            if(settings.getBool(CommonSettings.PROPERTYNAME_MUTE_MUSIC))
            {
                Logging.log("Muting music and ignoring all other music settings.");
                new MuteMusic().add(origRom, outRom, seed, settings, context);
                return false;
            }
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            if (randoMode == VanillaRandoSettings.MODE_KEY)
            {
                // vanilla rando - randomize music on load
                if(!settings.getBool(VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_MUSIC))
                {
                    return false;
                }
            }
            else if (randoMode == OpenWorldSettings.MODE_KEY)
            {
                // open world - randomize music on load
                if (!settings.getBool(OpenWorldSettings.PROPERTYNAME_RANDOMIZE_MUSIC))
                {
                    return false;
                }
            }
            else
            {
                // ac, boss rush, chaos - add the flammie drum that shuffles music
                addFlammieDrumMusicHack = true;
                addMusicRandomizer = false;
                if (randoMode == AncientCaveSettings.MODE_KEY)
                {
                    addMusicRandomizer = settings.getBool(AncientCaveSettings.PROPERTYNAME_RANDOM_MUSIC);
                }
            }

            Dictionary<string, List<List<byte>>> songReplacements = new Dictionary<string, List<List<byte>>>();
            Dictionary<byte, string> originalSongTypes = new Dictionary<byte, string>();
            SongExpansionInfo allSongData = doSongImport(outRom, context, songReplacements, originalSongTypes);
            if(addMusicRandomizer)
            {
                new RandomMusicHack().process(outRom, ref context.workingOffset, songReplacements, originalSongTypes);
            }
            if(addFlammieDrumMusicHack)
            {
                new FlammieDrum().process(outRom, ref context.workingOffset, allSongData, context.namesOfThings);
            }
            return true;
        }

        private SongExpansionInfo doSongImport(byte[] outRom, RandoContext context, Dictionary<string, List<List<byte>>> songReplacements, Dictionary<byte, string> originalSongTypes)
        {
            // allow customization of vanilla music categorization via vanillamusictypes.xml
            Dictionary<byte, string> customVanillaSongTypes = VanillaMusicTypesLoader.loadCustomVanillaMusicTypes();
            if (customVanillaSongTypes != null)
            {
                foreach (byte key in customVanillaSongTypes.Keys)
                {
                    originalSongTypes[key] = customVanillaSongTypes[key];
                }
            }
            else
            {
                // load defaults if the xml wasn't found/valid
                originalSongTypes[4] = SONGTYPELABEL_BATTLE; // boss theme
                originalSongTypes[57] = SONGTYPELABEL_BATTLE; // mana beast theme
                originalSongTypes[12] = SONGTYPELABEL_BATTLE; // dark lich theme

                originalSongTypes[1] = SONGTYPELABEL_FLIGHT; // flammie theme a
                originalSongTypes[2] = SONGTYPELABEL_FLIGHT; // flammie theme b
                originalSongTypes[3] = SONGTYPELABEL_FLIGHT; // flammie theme c

                originalSongTypes[0] = SONGTYPELABEL_OVERWORLD; // desert
                originalSongTypes[5] = SONGTYPELABEL_OVERWORLD; // far thunder
                originalSongTypes[6] = SONGTYPELABEL_OVERWORLD; // where the wind ends
                originalSongTypes[10] = SONGTYPELABEL_OVERWORLD; // upper land
                originalSongTypes[36] = SONGTYPELABEL_OVERWORLD; // a wish
                originalSongTypes[39] = SONGTYPELABEL_OVERWORLD; // pureland

                originalSongTypes[14] = SONGTYPELABEL_DUNGEON; // wild fields
                originalSongTypes[21] = SONGTYPELABEL_DUNGEON; // ice castle
                originalSongTypes[27] = SONGTYPELABEL_DUNGEON; // ruins
                originalSongTypes[38] = SONGTYPELABEL_DUNGEON; // castle dungeon/mana fort exterior
                originalSongTypes[42] = SONGTYPELABEL_DUNGEON; // palace dungeons
                originalSongTypes[49] = SONGTYPELABEL_DUNGEON; // manafort interior

                originalSongTypes[9] = SONGTYPELABEL_TOWN; // kakkara town theme
                originalSongTypes[13] = SONGTYPELABEL_TOWN; // empire town theme
                originalSongTypes[15] = SONGTYPELABEL_TOWN; // pandora town theme
                originalSongTypes[17] = SONGTYPELABEL_TOWN; // normal town theme
                originalSongTypes[22] = SONGTYPELABEL_TOWN; // matango town theme
                originalSongTypes[31] = SONGTYPELABEL_TOWN; // gaia town theme
                originalSongTypes[37] = SONGTYPELABEL_TOWN; // tasnica town theme

                originalSongTypes[29] = SONGTYPELABEL_PALACE; // palace theme

                originalSongTypes[20] = SONGTYPELABEL_VICTORY; // victory theme
            }

            songReplacements[SONGTYPELABEL_DUNGEON] = new List<List<byte>>();
            songReplacements[SONGTYPELABEL_BATTLE] = new List<List<byte>>();
            songReplacements[SONGTYPELABEL_TOWN] = new List<List<byte>>();
            songReplacements[SONGTYPELABEL_FLIGHT] = new List<List<byte>>();
            songReplacements[SONGTYPELABEL_OVERWORLD] = new List<List<byte>>();
            songReplacements[SONGTYPELABEL_PALACE] = new List<List<byte>>();
            songReplacements[SONGTYPELABEL_VICTORY] = new List<List<byte>>();
            songReplacements[SONGTYPELABEL_DEBUG] = new List<List<byte>>();
            
            // the two parameters that i think are echo related? for each vanilla song when playing via event command
            // they are required here to set up the music
            // for imported songs, we default the first byte to 0x13 but the song can override it, and the second byte is always 0x8f
            // these vanilla ones come from the events in the 0x700 range
            Dictionary<byte, byte[]> vanillaSongParameters = new Dictionary<byte, byte[]> { };
            for(byte i=0; i <= 0x3a; i++)
            {
                // these are not all in order, so parse the event 0x40 0x01 ss xx xx command to find the song id
                // it is however the first event command of all of these events, so we can just take bytes
                // [2], the song id, and [3] [4], the echo data.
                List<byte> ogEvent = VanillaEventUtil.getVanillaEvent(context.originalRom, i + 0x700);
                vanillaSongParameters[ogEvent[2]] = new byte[] { ogEvent[3], ogEvent[4] };
            }

            foreach(byte vanillaSongId in originalSongTypes.Keys)
            {
                string songType = originalSongTypes[vanillaSongId];
                songReplacements[songType].Add(new byte[] { vanillaSongId, vanillaSongParameters[vanillaSongId][0], vanillaSongParameters[vanillaSongId][1] }.ToList());
            }

            Logging.log("Begin music import.");

            // 33 total vanilla samples
            ushort sampleId = 33;
            List<ImportedSample> samples = ExternalSamples.getAllCustomSamples(ref sampleId);
            if(samples == null)
            {
                Logging.log("Imported sample parsing failed; proceeding with no imported music");
                return MusicExpander.expandMusic(outRom, ref context.workingOffset, 0, 0);
            }

            Dictionary<string, ushort> sampleNamesToIds = new Dictionary<string, ushort>();
            foreach(ImportedSample sample in samples)
            {
                sampleNamesToIds[sample.name] = (ushort)(sample.newId + 1);
            }

            // last vanilla song is technically 63 by the pointer list, though some of the last ones are empty
            ushort songId = 64;
            List<ImportedSong> songs = ExternalMusic.getAllCustomSongs(sampleNamesToIds, ref songId);
            if (songs == null)
            {
                Logging.log("Imported music parsing failed; proceeding with no imported music");
                return MusicExpander.expandMusic(outRom, ref context.workingOffset, 0, 0);
            }

            Logging.log("Imported samples: " + samples.Count, "debug");
            Logging.log("Imported songs: " + songs.Count, "debug");
            SongExpansionInfo info = MusicExpander.expandMusic(outRom, ref context.workingOffset, songs.Count, samples.Count);
            Logging.log("Sample offsets at " + info.sampleOffsetsOffset.ToString("X6"), "debug");
            Logging.log("Sample base frequencies at " + info.sampleFrequenciesOffset.ToString("X6"), "debug");
            Logging.log("Sample loops at " + info.sampleLoopsOffset.ToString("X6"), "debug");
            Logging.log("Sample ADSRs at " + info.sampleAdsrOffset.ToString("X6"), "debug");
            Logging.log("Song offsets at " + info.songOffsetsOffset.ToString("X6"), "debug");
            Logging.log("Song sample tables at " + info.sampleTablesOffset.ToString("X6"), "debug");
            foreach (ImportedSample sample in samples)
            {
                // write imported sample data, and make sure there's space in the current bank to do it.
                CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, sample.data.Length);
                Array.Copy(sample.data, 0, outRom, context.workingOffset, sample.data.Length);
                outRom[info.sampleOffsetsOffset + sample.newId * 3] = (byte)context.workingOffset;
                outRom[info.sampleOffsetsOffset + sample.newId * 3 + 1] = (byte)(context.workingOffset >> 8);
                outRom[info.sampleOffsetsOffset + sample.newId * 3 + 2] = (byte)((context.workingOffset >> 16) + 0xC0);
                Logging.log("Writing " + sample.data.Length + " bytes for imported BRR sample " + sample.name + " to " + context.workingOffset.ToString("X"), "debug");
                context.workingOffset += sample.data.Length;

                // for some reason base frequency is byteswapped in rom
                outRom[info.sampleFrequenciesOffset + sample.newId * 2 + 1] = (byte)sample.baseFreq;
                outRom[info.sampleFrequenciesOffset + sample.newId * 2] = (byte)(sample.baseFreq >> 8);

                // loop point table entry
                outRom[info.sampleLoopsOffset + sample.newId * 2] = (byte)sample.loop;
                outRom[info.sampleLoopsOffset + sample.newId * 2 + 1] = (byte)(sample.loop >> 8);

                // ADSR table entry
                outRom[info.sampleAdsrOffset + sample.newId * 2] = (byte)sample.adsr;
                outRom[info.sampleAdsrOffset + sample.newId * 2 + 1] = (byte)(sample.adsr >> 8);
            }

            foreach (ImportedSong song in songs)
            {
                // write imported song data, and make sure there's space in the current bank to do it.
                CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, song.data.Length);
                Array.Copy(song.data, 0, outRom, context.workingOffset, song.data.Length);
                outRom[info.songOffsetsOffset + song.newId * 3] = (byte)context.workingOffset;
                outRom[info.songOffsetsOffset + song.newId * 3 + 1] = (byte)(context.workingOffset >> 8);
                outRom[info.songOffsetsOffset + song.newId * 3 + 2] = (byte)((context.workingOffset >> 16) + 0xC0);
                Logging.log("Writing " + song.data.Length + " bytes for imported song " + song.newId + " " + song.name + " to " + context.workingOffset.ToString("X6"), "debug");
                IEnumerable<string> sampleTableStringLines = song.sampleTable.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
                Logging.log("Sample table: " + string.Join("; ", sampleTableStringLines), "debug");
                context.workingOffset += song.data.Length;

                for (int i = 0; i < 16; i++)
                {
                    ushort sampleTableId = (ushort)(song.sampleTable.ContainsKey(i) ? song.sampleTable[i] : 0);
                    outRom[info.sampleTablesOffset + song.newId * 32 + i * 2] = (byte)sampleTableId;
                    outRom[info.sampleTablesOffset + song.newId * 32 + i * 2 + 1] = (byte)(sampleTableId >> 8);
                }

                foreach (string category in song.categories)
                {
                    songReplacements[category].Add(new byte[] { (byte)song.newId, song.echoVal, 0x8F }.ToList());
                }
            }

            Logging.log("Music import complete.");
            return info;
        }

        // MOPPLE: previously this used to check if the total sample size for a song is > 31918 and log a warning;
        // should put this check back in
    }
}
