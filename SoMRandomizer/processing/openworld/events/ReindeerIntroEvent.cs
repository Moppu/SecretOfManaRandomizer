using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.hacks.openworld.XmasRandoData;
using static SoMRandomizer.processing.common.SomVanillaValues;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Intro event for open world reindeer mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ReindeerIntroEvent : RandoProcessor
    {
        // sort by easy/medium/hard, pick 2 easy, 2 hard, 4 medium
        private static List<ReplacementObject> easyReplacementLocations = new List<ReplacementObject>();
        private static List<ReplacementObject> mediumReplacementLocations = new List<ReplacementObject>();
        private static List<ReplacementObject> hardReplacementLocations = new List<ReplacementObject>();
        private static List<ReplacementObject> allReplacementLocations = new List<ReplacementObject>();
        public static ReplacementObject getByGlobalId(int globalId)
        {
            return allReplacementLocations[globalId];
        }
        public const string SELECTED_LOCATION_PREFIX = "reindeerLocation";

        protected override string getName()
        {
            return "Event 0x106 - intro (xmas reindeer)";
        }

        static ReindeerIntroEvent()
        {
            // easy difficulty
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_POTOS_SOUTH_FIELD, -1, new string[] { "near Potos" })); // south of potos
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_JEHK_DUNGEON, -1, new string[] { "in the mountains" })); // jehk dungeon
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_JEHK_LOBBY, 0, new string[] { "in the mountains" })); // jehk
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_JEHK_LOBBY, 1, new string[] { "in the mountains" })); // johk
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_WATERPALACE_WITCHCASTLE_CROSSROADS, 0, new string[] { "near the Water Palace" })); // dyluck s of water palace
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_WEST_FIELD, -1, new string[] { "near Pandora" })); // W of pandora
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_ENTRANCE, 0, new string[] { "in Tasnica" })); // guard outside tasnica
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_GAIASNAVEL_EXTERIOR, -1, new string[] { "near Gaia's Navel" })); // gaia's navel outside
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_EXTERIOR_B, 0, new string[] { "in Tasnica" })); // guard outside inside tasnica
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHFOREST_B, -1, new string[] { "in the Witch's Forest" })); // witch forest
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHFOREST_C, -1, new string[] { "in the Witch's Forest" })); // witch forest
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_A, 3, new string[] { "in Tasnica" })); // tasnica first room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_A, 4, new string[] { "in Tasnica" })); // tasnica first room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_A, 1, new string[] { "in Tasnica" })); // tasnica first room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_A, 6, new string[] { "in Tasnica" })); // tasnica first room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_A, 7, new string[] { "in Tasnica" })); // tasnica first room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_A, 5, new string[] { "in Tasnica" })); // tasnica first room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_A, 8, new string[] { "in Tasnica" })); // tasnica first room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_B, 0, new string[] { "in Tasnica" })); // tasnica second room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_B, 1, new string[] { "in Tasnica" })); // tasnica second room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_B, 2, new string[] { "in Tasnica" })); // tasnica second room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_B, 4, new string[] { "in Tasnica" })); // tasnica second room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_B, 5, new string[] { "in Tasnica" })); // tasnica second room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TASNICA_INTERIOR_B, 6, new string[] { "in Tasnica" })); // tasnica second room
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_SHADEPALACE_EXTERIOR, -1, new string[] { "in the mountains" }, new int[] { 4 })); // outside shade palace
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MANDALA_SOUTH, 2, new string[] { "in the mountains" })); // mountain town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MANDALA_SOUTH, 3, new string[] { "in the mountains" })); // mountain town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MANDALA_SOUTH, 5, new string[] { "in the mountains" })); // mountain town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MANDALA_NORTH, 3, new string[] { "in the mountains" })); // mountain town temple npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MANDALA_NORTH, 6, new string[] { "in the mountains" })); // mountain town temple npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MANDALA_NORTH, 2, new string[] { "in the mountains" })); // mountain town temple npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_DESERT_C, -1, new string[] { "in the desert" })); // desert right below kakkara
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NTC_ENTRANCE, -1, new string[] { "at North Town Castle" })); // outside NTC
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_KAKKARA, 2, new string[] { "in the desert" })); // kakkara npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_KAKKARA, 6, new string[] { "in the desert" })); // kakkara npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_KAKKARA, 7, new string[] { "in the desert" })); // kakkara npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_FIREPALACE_ENTRANCE, -1, new string[] { "in the desert" }, new int[] { 0, 1 })); // outside fire palace
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_UNDINECAVE_EXTERIOR, -1, new string[] { "near the Water Palace" })); // outside undine cave
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_GOLDCITY, 2, new string[] { "at Gold Island" })); // gold town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_GOLDCITY, 4, new string[] { "at Gold Island" })); // gold town npc - that asshole up on the ledge with the key dialogue
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_GOLDCITY, 7, new string[] { "at Gold Island" })); // gold town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_SOUTHTOWN, 3, new string[] { "in South Town" })); // south town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_SOUTHTOWN, 4, new string[] { "in South Town" })); // south town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_SOUTHTOWN, 5, new string[] { "in South Town" })); // south town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN, 0, new string[] { "in North Town" })); // north town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN, 2, new string[] { "in North Town" })); // north town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN, 6, new string[] { "in North Town" })); // north town npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_RABITEFIELD_B, -1, new string[] { "near Potos" })); // rabite field
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA, 1, new string[] { "near Pandora" })); // pandora exterior npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA, 2, new string[] { "near Pandora" })); // pandora exterior npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA, 4, new string[] { "near Pandora" })); // pandora exterior npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_KIPPO, 0, new string[] { "near Gaia's Navel" })); // useless town exterior npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_KIPPO, 1, new string[] { "near Gaia's Navel" })); // useless town exterior npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGO_BACKYARD, -1, new string[] { "near Matango" })); // outside matango cave
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGO_FRONTYARD, 6, new string[] { "near Matango" })); // S of matango
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TODO, 0, new string[] { "in the Ice Country" })); // walrus town
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TODO, 3, new string[] { "in the Ice Country" })); // walrus town
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_DWARFTOWN, 5, new string[] { "near Gaia's Navel" })); // dwarf town
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_DWARFTOWN, 6, new string[] { "near Gaia's Navel" })); // dwarf town
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGO_INTERIOR_LOBBY, 1, new string[] { "near Matango" })); // matango
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGO_INTERIOR_LOBBY, 3, new string[] { "near Matango" })); // matango
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGO_INTERIOR_THRONE, 2, new string[] { "near Matango" })); // matango
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGO_INTERIOR_THRONE, 3, new string[] { "near Matango" })); // matango
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TURTLEISLAND_INTERIOR, 2, new string[] { "on a turtle" })); // turtle interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TURTLEISLAND_INTERIOR, 3, new string[] { "on a turtle" })); // turtle interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TURTLEISLAND_INTERIOR, 4, new string[] { "on a turtle" })); // turtle interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_TURTLEISLAND_INTERIOR, 5, new string[] { "on a turtle" })); // turtle interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN_INTERIOR_A, 6, new string[] { "in North Town" })); // north town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN_INTERIOR_A, 7, new string[] { "in North Town" })); // north town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN_INTERIOR_B, 4, new string[] { "in North Town" })); // north town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN_INTERIOR_B, 10, new string[] { "in North Town" })); // north town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_GOLDCITY_INTERIOR, 1, new string[] { "at Gold Island" })); // gold town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_GOLDCITY_INTERIOR, 7, new string[] { "at Gold Island" })); // gold town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_GOLDCITY_INTERIOR, 9, new string[] { "at Gold Island" })); // gold town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_GOLDCITY_INTERIOR, 10, new string[] { "at Gold Island" })); // gold town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN_INTERIOR_C, 2, new string[] { "in North Town" })); // north town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN_INTERIOR_C, 4, new string[] { "in North Town" })); // north town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN_INTERIOR_C, 5, new string[] { "in North Town" })); // north town indoors npc
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN_INTERIOR_B2, 1, new string[] { "in North Town" })); // some town interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_NORTHTOWN_INTERIOR_B2, 4, new string[] { "in North Town" })); // some town interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_POTOS_INTERIOR_A, 0, new string[] { "near Potos" })); // potos interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_POTOS_INTERIOR_A, 1, new string[] { "near Potos" })); // potos interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_POTOS_INTERIOR_B, 5, new string[] { "near Potos" })); // potos interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_INTERIOR_A, 4, new string[] { "near Pandora" })); // some town interior (pandora?)
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_INTERIOR_A, 5, new string[] { "near Pandora" })); // some town interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_INTERIOR_A, 10, new string[] { "near Pandora" })); // some town interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_INTERIOR_A, 6, new string[] { "near Pandora" })); // some town interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_INTERIOR_A, 7, new string[] { "near Pandora" })); // some town interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_INTERIOR_A, 8, new string[] { "near Pandora" })); // some town interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_CASTLE_INTERIOR_A, 4, new string[] { "near Pandora" })); // pandora castle interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_CASTLE_INTERIOR_A, 5, new string[] { "near Pandora" })); // pandora castle interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_CASTLE_INTERIOR_A, 6, new string[] { "near Pandora" })); // pandora castle interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_CASTLE_INTERIOR_B, 1, new string[] { "near Pandora" })); // pandora castle interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_CASTLE_INTERIOR_B, 2, new string[] { "near Pandora" })); // pandora castle interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_CASTLE_INTERIOR_B, 6, new string[] { "near Pandora" })); // pandora castle interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_KIPPO_INTERIOR, 1, new string[] { "near Gaia's Navel" })); // some town interior - useless town
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_KIPPO_INTERIOR, 3, new string[] { "near Gaia's Navel" })); // some town interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_KIPPO_INTERIOR, 4, new string[] { "near Gaia's Navel" })); // some town interior
            easyReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_CASTLE_INTERIOR_B2, 3, new string[] { "near Pandora" })); // pandora castle interior

            // medium difficulty
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHFOREST_D, -1, new string[] { "in the Witch's Forest" })); // witch forest
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_UPPERLAND_NORTHWEST, -1, new string[] { "in the Upper Land" })); // upper land outside the mushroom town
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_UPPERLAND_SOUTHWEST, -1, new string[] { "in the Upper Land" })); // upper land
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_UPPERLAND_SOUTHEAST, -1, new string[] { "in the Upper Land" })); // upper land 
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_UPPERLAND_WINTER, -1, new string[] { "in the Upper Land" })); // upper land winter
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_UPPERLAND_SUMMER, -1, new string[] { "in the Upper Land" })); // upper land summer
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_UPPERLAND_FALL, -1, new string[] { "in the Upper Land" })); // upper land fall
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_EXTERIOR, -1, new string[] { "in the Ice Country" })); // outside ice palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECOUNTRY_NORTHWEST, -1, new string[] { "in the Ice Country" })); // ice country
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECOUNTRY_SOUTHWEST, -1, new string[] { "in the Ice Country" }, new int[] { 16 })); // ice country
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECOUNTRY_NORTHEAST, -1, new string[] { "in the Ice Country" })); // ice country
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECOUNTRY_SOUTHEAST, -1, new string[] { "in the Ice Country" }, new int[] { 9 })); // ice country
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PATH_SOUTH_OF_ICE_CASTLE, -1, new string[] { "in the Ice Country" })); // ice country S of ice palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_DESERT_J, -1, new string[] { "in the desert" })); // desert two below kakkara
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_DESERT_K, -1, new string[] { "in the desert" })); // desert below fire palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_DESERT_STARS_B, -1, new string[] { "in the desert" })); // desert below luna palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_LUMINATOWER_1F, -1, new string[] { "at Gold Island" })); // lumina tower
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_LUMINATOWER_2F, -1, new string[] { "at Gold Island" })); // lumina tower
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_RABITEFIELD_A, -1, new string[] { "near Potos" })); // rabite field
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_WATERPALACE_BASEMENT, 0, new string[] { "near the Water Palace" })); // the jail in water palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_RUINS_A, -1, new string[] { "near Pandora" })); // pandora ruins
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_RUINS_B, -1, new string[] { "near Pandora" })); // pandora ruins
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_RUINS_C, -1, new string[] { "near Pandora" })); // pandora ruins
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PANDORA_RUINS_ROOMS, -1, new string[] { "near Pandora" })); // pandora ruins
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_EXTERIOR, -1, new string[] { "in the Witch's Forest" }, new int[] { 0, 1 })); // witch castle outside
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_B, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_C, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_D, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_E, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_F, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_G, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_H, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_I, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_J, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_K, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_L, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_M, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_N, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_O, -1, new string[] { "in the Pure Lands" })); // pure lands
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_UNDINECAVE_FISH_ROOM, -1, new string[] { "near the Water Palace" })); // undine cave
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_GAIASNAVEL_INTERIOR_A_ENEMY, -1, new string[] { "near Gaia's Navel" })); // gaia navel
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_GAIASNAVEL_INTERIOR_B, -1, new string[] { "near Gaia's Navel" })); // gaia navel
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_GAIASNAVEL_INTERIOR_C, -1, new string[] { "near Gaia's Navel" })); // gaia navel
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_MAGICROPE, -1, new string[] { "near Gaia's Navel" })); // gaia navel
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_FIREPALACE_A, -1, new string[] { "in the desert" })); // fire palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_INTERIOR_A, -1, new string[] { "in the Ice Country" })); // ice palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_INTERIOR_B, -1, new string[] { "in the Ice Country" })); // ice palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_INTERIOR_C, -1, new string[] { "in the Ice Country" }, new int[] { 2, })); // ice palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_INTERIOR_D, -1, new string[] { "in the Ice Country" })); // ice palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_INTERIOR_E, -1, new string[] { "in the Ice Country" })); // ice palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_INTERIOR_F, -1, new string[] { "in the Ice Country" })); // ice palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_INTERIOR_G, -1, new string[] { "in the Ice Country" })); // ice palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_INTERIOR_H, -1, new string[] { "in the Ice Country" })); // ice palace
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_SHADEPALACE_INTERIOR_A, -1, new string[] { "in the mountains" })); // shade palace entrance
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTC_INTERIOR_B, -1, new string[] { "in North Town" })); // ntc
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTC_INTERIOR_D, -1, new string[] { "in North Town" })); // ntc
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTC_INTERIOR_E, -1, new string[] { "in North Town" })); // ntc
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTC_INTERIOR_F, -1, new string[] { "in North Town" })); // ntc
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTC_INTERIOR_G, -1, new string[] { "in North Town" })); // ntc
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTC_INTERIOR_H, -1, new string[] { "in North Town" })); // ntc
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTC_INTERIOR_I, -1, new string[] { "in North Town" })); // ntc
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTC_INTERIOR_L, -1, new string[] { "in North Town" })); // ntc
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTC_INTERIOR_M, -1, new string[] { "in North Town" })); // ntc
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTR_INTERIOR_A, -1, new string[] { "in North Town" })); // nt ruins
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTR_INTERIOR_B, -1, new string[] { "in North Town" }, new int[] { 20, })); // nt ruins
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTR_INTERIOR_D, -1, new string[] { "in North Town" })); // nt ruins
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTR_INTERIOR_E, -1, new string[] { "in North Town" })); // nt ruins
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTR_INTERIOR_F, -1, new string[] { "in North Town" })); // nt ruins
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_NTR_INTERIOR_G, -1, new string[] { "in North Town" })); // nt ruins
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_GRANDPALACE_INTERIOR_A, -1, new string[] { "in the Sunken Continent" })); // sunken continent
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_GRANDPALACE_INTERIOR_B, -1, new string[] { "in the Sunken Continent" }, new int[] { 0, 1, 2, })); // sunken continent
            mediumReplacementLocations.Add(new ReplacementObject(MAPNUM_UNDERSEA_AXE_ROOM_A, -1, new string[] { "in the Sunken Continent" })); // sunken continent

            // hard difficulty
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_UNDERSEA_SUBWAY_AREA_A, -1, new string[] { "in the Sunken Continent" })); // subway area in that shitty dungeon
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_UNDERSEA_SUBWAY_AREA_B, -1, new string[] { "in the Sunken Continent" })); // subway area in that shitty dungeon
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_P, -1, new string[] { "in the Pure Lands" })); // pure lands
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_Q, -1, new string[] { "in the Pure Lands" })); // pure lands
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_R, -1, new string[] { "in the Pure Lands" })); // pure lands
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_S, -1, new string[] { "in the Pure Lands" })); // pure lands
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_T, -1, new string[] { "in the Pure Lands" })); // pure lands
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_U, -1, new string[] { "in the Pure Lands" })); // pure lands
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_V, -1, new string[] { "in the Pure Lands" })); // pure lands
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_W, -1, new string[] { "in the Pure Lands" })); // pure lands
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_X, -1, new string[] { "in the Pure Lands" })); // pure lands
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_PURELANDS_Y, -1, new string[] { "in the Pure Lands" })); // pure lands
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_EARTHPALACE_TRANSITIONS, -1, new string[] { "near Gaia's Navel" })); // earth palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_INTERIOR_A, -1, new string[] { "in the Witch's Forest" })); // witch castle interior
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_INTERIOR_B, -1, new string[] { "in the Witch's Forest" })); // witch castle interior
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_INTERIOR_C, -1, new string[] { "in the Witch's Forest" })); // witch castle interior
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_INTERIOR_D, -1, new string[] { "in the Witch's Forest" })); // witch castle interior
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_INTERIOR_E, -1, new string[] { "in the Witch's Forest" })); // witch castle interior
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_INTERIOR_F, -1, new string[] { "in the Witch's Forest" })); // witch castle interior
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGOCAVE_GNOME_ORB, -1, new string[] { "near Matango" })); // matango cave room with the switch orb
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_JAIL, 0, new string[] { "in the Witch's Forest" })); // witch castle neko jail
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_JAIL, 1, new string[] { "in the Witch's Forest" })); // witch castle neko jail
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_JAIL, 2, new string[] { "in the Witch's Forest" })); // witch castle neko jail
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_EARTHPALACE_INTERIOR_A, -1, new string[] { "near Gaia's Navel" })); // earth palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_EARTHPALACE_INTERIOR_B, -1, new string[] { "near Gaia's Navel" })); // earth palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGOCAVE_A, -1, new string[] { "near Matango" })); // matango cave
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGOCAVE_B, -1, new string[] { "near Matango" })); // matango cave
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGOCAVE_C, -1, new string[] { "near Matango" })); // matango cave
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_MATANGOCAVE_D, -1, new string[] { "near Matango" })); // matango cave
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_FIREPALACE_B, -1, new string[] { "in the desert" })); // fire palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_FIREPALACE_C, -1, new string[] { "in the desert" })); // fire palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_FIREPALACE_D, -1, new string[] { "in the desert" })); // fire palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_FIREPALACE_E, -1, new string[] { "in the desert" }, new int[] { 1, 2 })); // fire palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_FIREPALACE_F, -1, new string[] { "in the desert" })); // fire palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_ROOM_A, -1, new string[] { "in the Witch's Forest" })); // witch castle
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_WITCHCASTLE_ROOM_B, -1, new string[] { "in the Witch's Forest" })); // witch castle
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_FIREPALACE_G, -1, new string[] { "in the desert" })); // fire palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_FIREPALACE_H, -1, new string[] { "in the desert" })); // fire palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_FIREPALACE_FIRST_ORB, -1, new string[] { "in the desert" })); // fire palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_INTERIOR_J, -1, new string[] { "in the Ice Country" }, new int[] { 2 })); // ice palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_ICECASTLE_INTERIOR_K, -1, new string[] { "in the Ice Country" })); // ice palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_SHADEPALACE_INTERIOR_B, -1, new string[] { "in the mountains" }, new int[] { 8 })); // shade palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_SHADEPALACE_INTERIOR_C, -1, new string[] { "in the mountains" })); // shade palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_SHADEPALACE_INTERIOR_D, -1, new string[] { "in the mountains" })); // shade palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_SHADEPALACE_INTERIOR_E, -1, new string[] { "in the mountains" })); // shade palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_SHADEPALACE_INTERIOR_F, -1, new string[] { "in the mountains" })); // shade palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_SHADEPALACE_INTERIOR_G, -1, new string[] { "in the mountains" })); // shade palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_SHADEPALACE_INTERIOR_H, -1, new string[] { "in the mountains" })); // shade palace
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_GRANDPALACE_INTERIOR_C, -1, new string[] { "in the Sunken Continent" })); // sunken continent
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_GRANDPALACE_INTERIOR_D, -1, new string[] { "in the Sunken Continent" })); // sunken continent
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_GRANDPALACE_INTERIOR_E, -1, new string[] { "in the Sunken Continent" })); // sunken continent
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_GRANDPALACE_INTERIOR_F, -1, new string[] { "in the Sunken Continent" })); // sunken continent
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_UNDERSEA_AXE_ROOM_B, -1, new string[] { "in the Sunken Continent" })); // sunken continent
            hardReplacementLocations.Add(new ReplacementObject(MAPNUM_UNDERSEA_AXE_ROOM_C, -1, new string[] { "in the Sunken Continent" })); // sunken continent

            allReplacementLocations.AddRange(easyReplacementLocations);
            allReplacementLocations.AddRange(mediumReplacementLocations);
            allReplacementLocations.AddRange(hardReplacementLocations);
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if (context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME) != OpenWorldGoalProcessor.GOAL_REINDEER)
            {
                return false;
            }

            // add on to existing event 0x106
            EventScript eventData = (EventScript)context.replacementEvents[0x106];
            // reindeer hunt thing
            // play the ice country music
            eventData.Add(EventCommandEnum.PLAY_SOUND.Value);
            eventData.Add(0x01);
            eventData.Add(0x24);
            eventData.Add(0x1c);
            eventData.Add(0x8f);
            int mapNum;
            int mapObjOffset;
            Random r = context.randomFunctional;
            string startingChar = context.workingData.get(OpenWorldCharacterSelection.STARTING_CHARACTER);
            string characterName = "(" + startingChar + ")";

            int[] santaDialogueEvents = new int[] { 0x137, 0x138, 0x139, 0x13a, 0x13b, 0x13c, 0x13d, 0x13e, 0x13f, 0x140, 0x200, 0x201, 0x202, 0x126, 0x122, 0x121 }; // for found 1-8 messages; with some extras

            string[] reindeerNames = new string[] {
                    "Dasher",
                    "Dancer",
                    "Prancer",
                    "Vixen",
                    "Comet",
                    "Cupid",
                    "Donner",
                    "Blitzen",
                };
            // normal santa event is 37e - need to replace it with a bunch of weird logic and shit
            // use TURTLE_ISLAND_NPC_VISIBILITY (6F) for this, and clear map 448/449 of this
            // also have to replace event 2fa/2fb not to use it in logic
            EventScript newEvent2fa = new EventScript();
            context.replacementEvents[0x2fa] = newEvent2fa;
            newEvent2fa.Jump(0x711);
            newEvent2fa.End();

            EventScript newEvent2fb = new EventScript();
            context.replacementEvents[0x2fb] = newEvent2fb;
            newEvent2fb.Door(0x276);
            newEvent2fb.End();

            // 6F here - total number, EventFlags.XMAS_EXTRA_FLAGS - individual ones

            List<int> difficultyIndexes = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                int id = r.Next() % 8;
                while (difficultyIndexes.Contains(id))
                {
                    id = r.Next() % 8;
                }
                difficultyIndexes.Add(id);
            }

            List<ReplacementObject> chosenReplacements = new List<ReplacementObject>();
            for (int i = 0; i < 8; i++)
            {
                int difficultyId = difficultyIndexes[i];
                List<ReplacementObject> allReplacementLocations = null;
                int globalIndexOffset = 0;
                if (difficultyId < 2)
                {
                    allReplacementLocations = easyReplacementLocations;
                }
                else if (difficultyId < 6)
                {
                    allReplacementLocations = mediumReplacementLocations;
                    globalIndexOffset += easyReplacementLocations.Count;
                }
                else
                {
                    allReplacementLocations = hardReplacementLocations;
                    globalIndexOffset += easyReplacementLocations.Count + mediumReplacementLocations.Count;
                }

                int id = r.Next() % allReplacementLocations.Count;
                ReplacementObject thisReplacement = allReplacementLocations[id];
                // picked duplicate; pick again
                int breakout = 0;
                while(chosenReplacements.Contains(thisReplacement))
                {
                    id = r.Next() % allReplacementLocations.Count;
                    thisReplacement = allReplacementLocations[id];
                    breakout++;
                    if(breakout > 1000)
                    {
                        // you know, just in case.
                        Logging.log(">>> Failed to pick unique reindeer for reindeer mode!");
                        break;
                    }
                }
                // index in entire list; easy/medium/hard
                int globalId = globalIndexOffset + id;
                // save this selected location to the context for other things that will need it
                context.workingData.setInt(SELECTED_LOCATION_PREFIX + i, globalId);
                chosenReplacements.Add(thisReplacement);
                //allReplacementLocations.RemoveAt(id);
                mapNum = thisReplacement.mapNum;
                mapObjOffset = VanillaMapUtil.getObjectOffset(outRom, mapNum);
                mapObjOffset += 8; // skip header

                int objNum = thisReplacement.index;
                if (objNum == -1)
                {
                    int nextMapObjOffset = VanillaMapUtil.getObjectOffset(outRom, mapNum + 1);
                    nextMapObjOffset += 8; // skip header
                    int numObjects = (nextMapObjOffset - mapObjOffset) / 8 - 1;
                    objNum = r.Next() % numObjects;

                    bool isEnemyId = outRom[mapObjOffset + 8 * objNum + 5] <= 83;
                    while (thisReplacement.indexExceptions.Contains(objNum) || !isEnemyId)
                    {
                        objNum = r.Next() % numObjects;
                        isEnemyId = outRom[mapObjOffset + 8 * objNum + 5] <= 83;
                    }
                    // get rid of the nearby stuff by setting the event flag (only if enemy)
                    // this helps ensure the reindeer spawns
                    int xPos = (outRom[mapObjOffset + 8 * objNum + 2] & 0x7F);
                    int yPos = (outRom[mapObjOffset + 8 * objNum + 3] & 0x7F);
                    for (int e = 0; e < numObjects; e++)
                    {
                        bool isObjEnemy = outRom[mapObjOffset + 8 * e + 5] <= 83;
                        if (isObjEnemy && e != objNum)
                        {
                            int thisX = (outRom[mapObjOffset + 8 * e + 2] & 0x7F);
                            int thisY = (outRom[mapObjOffset + 8 * e + 3] & 0x7F);
                            int xDist = Math.Abs(thisX - xPos);
                            int yDist = Math.Abs(thisY - yPos);
                            if (xDist < 25 && yDist < 25)
                            {
                                outRom[mapObjOffset + 8 * e + 0] = EventFlags.XMAS_EXTRA_FLAGS[i];
                                outRom[mapObjOffset + 8 * e + 1] = 0x1F; // only show up once we find the thing
                            }
                        }
                    }
                    thisReplacement.index = objNum;
                }

                int findEventNum = santaDialogueEvents[i + 8];
                EventScript findEvent = new EventScript();
                context.replacementEvents[findEventNum] = findEvent;
                findEvent.AddDialogueBox("It's " + reindeerNames[i] + "!");
                // set event flags and refresh map to delete it
                findEvent.SetFlag(EventFlags.XMAS_EXTRA_FLAGS[i], 1);
                findEvent.Add(EventCommandEnum.REFRESH_MAP.Value);
                findEvent.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                findEvent.End();

                // set the object data
                outRom[mapObjOffset + 8 * objNum + 0] = EventFlags.XMAS_EXTRA_FLAGS[i]; // adjust event data of obj i
                outRom[mapObjOffset + 8 * objNum + 1] = 0x00; // adjust event data of obj i
                outRom[mapObjOffset + 8 * objNum + 5] = 0xAD; // rudolph
                outRom[mapObjOffset + 8 * objNum + 3] &= 0x7F; // movement enable
                outRom[mapObjOffset + 8 * objNum + 6] = (byte)findEventNum; // event
                outRom[mapObjOffset + 8 * objNum + 7] = (byte)((findEventNum >> 8) | 0x40); // event
            }

            // santa dialogue for reindeer mode
            eventData.AddAutoTextDialogueBox(characterName + "!\n" +
                "You gotta help me, " + characterName + "!\n" +
                "\n" +

                "It's Christmas and all the\n" +
                "reindeer are gone, " + characterName + "!\n" +
                "\n" +

                "You've gotta help me\n" +
                "find all eight!  They\n" +
                "could be anywhere, " + characterName + "!\n" +

                "Ignore the one outside!\n" +
                "That's just Rudolph!\n" +
                "He's a hint like always.\n" +

                "You can find them, right?\n" +
                "Sure you can!\n" +
                "\n" +

                "Here, have a weapon to\n" +
                "start with!\n", 0x0A);

            // event 0x107 - initialize event flags
            eventData.Jsr(0x107);

            // -> event x103, the sword grab event
            eventData.Jump(0x103);
            eventData.End();

            EventScript santa_mainEvent = new EventScript();
            context.replacementEvents[0x37e] = santa_mainEvent;

            // santa dialogue per-step with hints
            for (int i = 0; i < 8; i++)
            {
                byte eventFlag = EventFlags.XMAS_EXTRA_FLAGS[i];
                santa_mainEvent.Logic(eventFlag, 0x0, 0x0, EventScript.GetJumpCmd(santaDialogueEvents[i]));
                EventScript newEvent_santaDialogue = new EventScript();
                // odd values of event flag; give reward then increment flag
                context.replacementEvents[santaDialogueEvents[i]] = newEvent_santaDialogue;
                newEvent_santaDialogue.AddDialogueBox(VanillaEventUtil.wordWrapText("Still haven't found " + reindeerNames[i] + "? Tried looking " + chosenReplacements[i].hintLocations[0] + "?"));
                newEvent_santaDialogue.End();
            }

            santa_mainEvent.AddDialogueBox("You did the thing!");
            // credits
            santa_mainEvent.Jump(0x42f);
            santa_mainEvent.End();

            return true;
        }
    }
}
