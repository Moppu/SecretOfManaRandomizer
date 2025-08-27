using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using System.Collections.Generic;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Hack that changes magic rope to use a table of doorways for each map, instead of vanilla logic that remembers the last outer/inner transition
    /// you made, and returns to that spot.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MagicRopeRefactor : RandoProcessor
    {
        protected override string getName()
        {
            return "Magic rope refactor for vanilla maps";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(OpenWorldSettings.PROPERTYNAME_REFACTOR_MAGIC_ROPE))
            {
                return false;
            }

            // map vanilla composite maps to vanilla door that magic rope will execute on those maps
            Dictionary<int, int> mapToDoor = new Dictionary<int, int>();
            mapToDoor[MAPNUM_JEHK_DUNGEON] = 0x123; // jehk johk big room -> door to MAPNUM_JEHKCAVE_EXTERIOR (62)
            mapToDoor[MAPNUM_JEHK_LOBBY] = 0x123; // jehk johk foyer -> door to MAPNUM_JEHKCAVE_EXTERIOR (62)
            mapToDoor[MAPNUM_EMPIRE_SEWERS] = 0x2b1; // sewers -> door to MAPNUM_SOUTHTOWN (114)
            mapToDoor[MAPNUM_WITCHFOREST_A] = 0x7d; // witch forest -> door to MAPNUM_GAIASNAVEL_NORTH_PATH (27)
            mapToDoor[MAPNUM_WITCHFOREST_B] = 0x7d; // witch forest -> door to MAPNUM_GAIASNAVEL_NORTH_PATH (27)
            mapToDoor[MAPNUM_WITCHFOREST_C] = 0x7d; // witch forest -> door to MAPNUM_GAIASNAVEL_NORTH_PATH (27)
            mapToDoor[MAPNUM_WITCHFOREST_D] = 0xb4; // witch forest -> door to MAPNUM_WITCHFOREST_C (30)
            mapToDoor[MAPNUM_LUNAPALACE_SPACE] = 0x23b; // luna palace space room -> door to MAPNUM_KARONFERRY_ENDPOINTS (32)
            mapToDoor[MAPNUM_TASNICA_INTERIOR_A] = 0x2ea; // tasnica -> door to MAPNUM_TASNICA_ENTRANCE (24)
            mapToDoor[MAPNUM_TASNICA_INTERIOR_B] = 0x2ea; // tasnica -> door to MAPNUM_TASNICA_ENTRANCE (24)
            mapToDoor[MAPNUM_UNDERSEA_SUBWAY_AREA_A] = 0x30a; // subway -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_UNDERSEA_SUBWAY_AREA_B] = 0x30a; // subway -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_UNDERSEA_SUBWAY_A] = 0x30a; // subway -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_UNDERSEA_SUBWAY_B] = 0x30a; // subway -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_LUNAPALACE_NORMAL] = 0x23b; // luna palace intro room -> door to MAPNUM_KARONFERRY_ENDPOINTS (32)
            mapToDoor[MAPNUM_TASNICA_THRONE_ROOM_NPC] = 0x2ea; // tasnica -> door to MAPNUM_TASNICA_ENTRANCE (24)
            mapToDoor[MAPNUM_UNDERSEA_SUBWAY_B_WITH_WATTS] = 0x30a; // subway -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GAIASNAVEL_INTERIOR_A_NEKO] = 0x1A; // gaia's navel right side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)
            mapToDoor[MAPNUM_LUMINATOWER_1F] = 0x2a0; // lumina tower -> door to MAPNUM_LUMINATOWER_EXTERIOR (116)
            mapToDoor[MAPNUM_LUMINATOWER_2F] = 0x2a0; // lumina tower -> door to MAPNUM_LUMINATOWER_EXTERIOR (116)
            mapToDoor[MAPNUM_LIGHTSEED] = 0x2a0; // lumina room -> door to MAPNUM_LUMINATOWER_EXTERIOR (116)
            mapToDoor[MAPNUM_FLAMMIE_IN_MATANGO_CAVE] = 0x190; // matango cave flammie room -> door to MAPNUM_MATANGO_BACKYARD (222)
            mapToDoor[MAPNUM_WATERPALACE_BASEMENT] = 0xB; // water palace basement -> door to MAPNUM_WATERPALACE_EXTERIOR (136)
            mapToDoor[MAPNUM_WATERPALACE_HALLWAY_NPC] = 0xB; // water palace hallway with jema -> door to MAPNUM_WATERPALACE_EXTERIOR (136)
            mapToDoor[MAPNUM_WATERPALACE_LUKA_DIALOGUE] = 0xB; // water palace main room -> door to MAPNUM_WATERPALACE_EXTERIOR (136)
            mapToDoor[MAPNUM_PANDORA_RUINS_A] = 0x25; // pandora ruins -> door to MAPNUM_PANDORA_RUINS_EXTERIOR (160)
            mapToDoor[MAPNUM_PANDORA_RUINS_B] = 0x25; // pandora ruins -> door to MAPNUM_PANDORA_RUINS_EXTERIOR (160)
            mapToDoor[MAPNUM_PANDORA_RUINS_C] = 0x25; // pandora ruins -> door to MAPNUM_PANDORA_RUINS_EXTERIOR (160)
            mapToDoor[MAPNUM_PANDORA_RUINS_D] = 0x25; // pandora ruins -> door to MAPNUM_PANDORA_RUINS_EXTERIOR (160)
            mapToDoor[MAPNUM_PANDORA_RUINS_ROOMS] = 0x25; // pandora ruins -> door to MAPNUM_PANDORA_RUINS_EXTERIOR (160)
            mapToDoor[MAPNUM_PANDORA_RUINS_D_AFTER_WALLFACE] = 0x25; // pandora ruins -> door to MAPNUM_PANDORA_RUINS_EXTERIOR (160)
            mapToDoor[MAPNUM_WITCHCASTLE_EXTERIOR] = 0xb4; // witch castle exterior -> door to MAPNUM_WITCHFOREST_C (30)
            mapToDoor[MAPNUM_FIREPALACE_UNDINE_ORB] = 0x243; // fire palace final orb room -> door to MAPNUM_FIREPALACE_ENTRANCE (90)
            mapToDoor[MAPNUM_FIREPALACE_EMPTY_ROOM] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)
            mapToDoor[MAPNUM_MANAFORT_INTERIOR_A] = 0x7e; // manafort room 1 -> door to MAPNUM_MANAFORT_EXTERIOR (255)
            mapToDoor[MAPNUM_MANAFORT_INTERIOR_B] = 0x7e; // manafort room 2 -> door to MAPNUM_MANAFORT_EXTERIOR (255)
            mapToDoor[MAPNUM_MANAFORT_INTERIOR_C] = 0x7e; // manafort room 3 -> door to MAPNUM_MANAFORT_EXTERIOR (255)
            mapToDoor[MAPNUM_MANAFORT_INTERIOR_PATH_TO_LICH] = 0x7e; // manafort room 4 (former mech rider arena) -> door to MAPNUM_MANAFORT_EXTERIOR (255)
            mapToDoor[MAPNUM_PANDORA_CASTLE_INTERIOR_A] = 0xd4; // pandora castle -> door to MAPNUM_PANDORA_CASTLE_EXTERIOR (145)
            mapToDoor[MAPNUM_PANDORA_CASTLE_INTERIOR_B] = 0xd4; // pandora castle -> door to MAPNUM_PANDORA_CASTLE_EXTERIOR (145)
            mapToDoor[MAPNUM_PANDORA_CASTLE_INTERIOR_C] = 0xd4; // pandora castle -> door to MAPNUM_PANDORA_CASTLE_EXTERIOR (145)
            mapToDoor[MAPNUM_PANDORA_CASTLE_INTERIOR_D] = 0xd4; // pandora castle -> door to MAPNUM_PANDORA_CASTLE_EXTERIOR (145)
            mapToDoor[MAPNUM_PANDORA_TREASURE_ROOM] = 0xd4; // pandora castle -> door to MAPNUM_PANDORA_CASTLE_EXTERIOR (145)

            mapToDoor[MAPNUM_EARTHPALACE_TRANSITIONS] = 0x149; // earth palace -> door to MAPNUM_DWARFTOWN (287)
            mapToDoor[MAPNUM_UNDINECAVE_FISH_ROOM] = 0x141; // undine cave -> door to MAPNUM_UNDINECAVE_EXTERIOR (109)
            mapToDoor[MAPNUM_EARTHSEED] = 0x149; // earth palace -> door to MAPNUM_DWARFTOWN (287)
            mapToDoor[MAPNUM_UNDINE] = 0x141; // undine cave -> door to MAPNUM_UNDINECAVE_EXTERIOR (109)

            mapToDoor[MAPNUM_PANDORA_CASTLE_INTERIOR_B2] = 0xd4; // pandora castle -> door to MAPNUM_PANDORA_CASTLE_EXTERIOR (145)

            mapToDoor[MAPNUM_GAIASNAVEL_INTERIOR_A_ENEMY] = 0x37; // gaia's navel left side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)
            mapToDoor[MAPNUM_GAIASNAVEL_INTERIOR_B] = 0x37; // gaia's navel left side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)
            mapToDoor[MAPNUM_GAIASNAVEL_INTERIOR_C] = 0x37; // gaia's navel left side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)

            mapToDoor[MAPNUM_DWARFTOWN] = 0x1A; // gaia's navel right side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)
            mapToDoor[MAPNUM_WATTS_STAIRS] = 0x1A; // gaia's navel right side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)

            mapToDoor[MAPNUM_MAGICROPE] = 0x37; // gaia's navel left side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)

            mapToDoor[MAPNUM_EARTHPALACE_ORB] = 0x1A; // gaia's navel right side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)
            mapToDoor[MAPNUM_DWARFTOWN_INN] = 0x1A; // gaia's navel right side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)
            mapToDoor[MAPNUM_DWARFTOWN_SHOP] = 0x1A; // gaia's navel right side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)
            mapToDoor[MAPNUM_WATTS] = 0x1A; // gaia's navel right side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)
            mapToDoor[MAPNUM_PATH_TO_RABITEMAN] = 0x1A; // gaia's navel right side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)
            mapToDoor[MAPNUM_RABITEMAN_BACKSTAGE] = 0x1A; // gaia's navel right side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)
            mapToDoor[MAPNUM_RABITEMAN] = 0x1A; // gaia's navel right side -> door to MAPNUM_GAIASNAVEL_EXTERIOR (25)

            mapToDoor[MAPNUM_WITCHCASTLE_INTERIOR_A] = 0xAF; // witch castle -> door to MAPNUM_WITCHCASTLE_EXTERIOR (196)
            mapToDoor[MAPNUM_WITCHCASTLE_INTERIOR_B] = 0xAF; // witch castle -> door to MAPNUM_WITCHCASTLE_EXTERIOR (196)
            mapToDoor[MAPNUM_WITCHCASTLE_INTERIOR_C] = 0xAF; // witch castle -> door to MAPNUM_WITCHCASTLE_EXTERIOR (196)
            mapToDoor[MAPNUM_WITCHCASTLE_INTERIOR_D] = 0xAF; // witch castle -> door to MAPNUM_WITCHCASTLE_EXTERIOR (196)
            mapToDoor[MAPNUM_WITCHCASTLE_INTERIOR_E] = 0xAF; // witch castle -> door to MAPNUM_WITCHCASTLE_EXTERIOR (196)
            mapToDoor[MAPNUM_WITCHCASTLE_INTERIOR_F] = 0xAF; // witch castle -> door to MAPNUM_WITCHCASTLE_EXTERIOR (196)
            mapToDoor[MAPNUM_WITCHCASTLE_NESOBERI] = 0xAF; // witch castle -> door to MAPNUM_WITCHCASTLE_EXTERIOR (196)

            mapToDoor[MAPNUM_MATANGOCAVE_GNOME_ORB] = 0x190; // matango cave -> door to MAPNUM_MATANGO_BACKYARD (222)

            mapToDoor[MAPNUM_WITCHCASTLE_JAIL] = 0xAF; // witch castle -> door to MAPNUM_WITCHCASTLE_EXTERIOR (196)

            mapToDoor[MAPNUM_KILROYSHIP_UPPER] = 0x52; // kilroy -> door to MAPNUM_DWARFTOWN (287)

            mapToDoor[MAPNUM_EARTHPALACE_INTERIOR_A] = 0x149; // earth palace -> door to MAPNUM_DWARFTOWN (287)
            mapToDoor[MAPNUM_EARTHPALACE_INTERIOR_B] = 0x149; // earth palace -> door to MAPNUM_DWARFTOWN (287)
            mapToDoor[MAPNUM_EARTHPALACE_INTERIOR_C] = 0x149; // earth palace -> door to MAPNUM_DWARFTOWN (287)

            mapToDoor[MAPNUM_KILROYSHIP_LOWER] = 0x52; // kilroy -> door to MAPNUM_DWARFTOWN (287)
            mapToDoor[MAPNUM_KILROYSHIP_INTERIOR] = 0x52; // kilroy -> door to MAPNUM_DWARFTOWN (287)

            // note this is a split map, including the bit before matango where you need the axe, so we return to upper land 
            // to not allow magic roping into matango (and potentially getting stuck)
            mapToDoor[MAPNUM_MATANGOCAVE_A] = 0x155; // matango cave -> door to MAPNUM_UPPERLAND_NORTHWEST (39)
            mapToDoor[MAPNUM_MATANGOCAVE_B] = 0x190; // matango cave -> door to MAPNUM_MATANGO_BACKYARD (222)
            mapToDoor[MAPNUM_MATANGOCAVE_C] = 0x190; // matango cave -> door to MAPNUM_MATANGO_BACKYARD (222)
            mapToDoor[MAPNUM_MATANGOCAVE_D] = 0x190; // matango cave -> door to MAPNUM_MATANGO_BACKYARD (222)

            mapToDoor[MAPNUM_FIREPALACE_A] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)
            mapToDoor[MAPNUM_FIREPALACE_B] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)
            mapToDoor[MAPNUM_FIREPALACE_C] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)
            mapToDoor[MAPNUM_FIREPALACE_D] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)
            mapToDoor[MAPNUM_FIREPALACE_E] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)
            mapToDoor[MAPNUM_FIREPALACE_F] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)

            mapToDoor[MAPNUM_WITCHCASTLE_ROOM_A] = 0xAF; // witch castle -> door to MAPNUM_WITCHCASTLE_EXTERIOR (196)
            mapToDoor[MAPNUM_WITCHCASTLE_ROOM_B] = 0xAF; // witch castle -> door to MAPNUM_WITCHCASTLE_EXTERIOR (196)

            mapToDoor[MAPNUM_FIREPALACE_G] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)
            mapToDoor[MAPNUM_FIREPALACE_H] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)
            mapToDoor[MAPNUM_FIREPALACE_FIRST_ORB] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)
            mapToDoor[MAPNUM_FIRESEED] = 0x243; // fire palace -> door to MAPNUM_FIREPALACE_ENTRANCE (90)

            mapToDoor[MAPNUM_LUNAPALACE_STAIRS] = 0x23b; // luna palace room before seed -> door to MAPNUM_KARONFERRY_ENDPOINTS (32)
            mapToDoor[MAPNUM_MOONSEED] = 0x23b; // luna palace seed room -> door to MAPNUM_KARONFERRY_ENDPOINTS (32)

            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_A] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)
            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_B] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)
            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_C] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)
            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_D] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)
            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_E] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)
            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_F] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)
            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_G] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)
            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_H] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)
            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_I] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)

            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_J] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)
            mapToDoor[MAPNUM_ICECASTLE_SANTA] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)

            mapToDoor[MAPNUM_ICECASTLE_INTERIOR_K] = 0x200; // ice castle -> door to MAPNUM_ICECASTLE_EXTERIOR (75)

            mapToDoor[MAPNUM_SHADEPALACE_INTERIOR_A] = 0x11B; // shade palace -> door to MAPNUM_SHADEPALACE_EXTERIOR (61)
            mapToDoor[MAPNUM_SHADEPALACE_INTERIOR_B] = 0x11B; // shade palace -> door to MAPNUM_SHADEPALACE_EXTERIOR (61)
            mapToDoor[MAPNUM_SHADEPALACE_INTERIOR_C] = 0x11B; // shade palace -> door to MAPNUM_SHADEPALACE_EXTERIOR (61)
            mapToDoor[MAPNUM_SHADEPALACE_INTERIOR_D] = 0x11B; // shade palace -> door to MAPNUM_SHADEPALACE_EXTERIOR (61)
            mapToDoor[MAPNUM_SHADEPALACE_INTERIOR_E] = 0x11B; // shade palace -> door to MAPNUM_SHADEPALACE_EXTERIOR (61)
            mapToDoor[MAPNUM_SHADEPALACE_INTERIOR_F] = 0x11B; // shade palace -> door to MAPNUM_SHADEPALACE_EXTERIOR (61)
            mapToDoor[MAPNUM_SHADEPALACE_INTERIOR_G] = 0x11B; // shade palace -> door to MAPNUM_SHADEPALACE_EXTERIOR (61)
            mapToDoor[MAPNUM_SHADEPALACE_INTERIOR_H] = 0x11B; // shade palace -> door to MAPNUM_SHADEPALACE_EXTERIOR (61)
            mapToDoor[MAPNUM_DARKSEED] = 0x11B; // shade palace -> door to MAPNUM_SHADEPALACE_EXTERIOR (61)

            mapToDoor[MAPNUM_NTC_INTERIOR_A] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_B] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_C] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_D] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_E] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_F] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_G] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_H] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_I] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_J] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_K] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_L] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_M] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)
            mapToDoor[MAPNUM_NTC_INTERIOR_N] = 0x333; // ntc -> door to MAPNUM_NTC_ENTRANCE (82)

            mapToDoor[MAPNUM_NTR_INTERIOR_A] = 0x2D0; // ntr -> door to MAPNUM_NTR_ENTRANCE (21)
            mapToDoor[MAPNUM_NTR_INTERIOR_B] = 0x2D0; // ntr -> door to MAPNUM_NTR_ENTRANCE (21)
            mapToDoor[MAPNUM_NTR_INTERIOR_C] = 0x2D0; // ntr -> door to MAPNUM_NTR_ENTRANCE (21)
            mapToDoor[MAPNUM_NTR_INTERIOR_D] = 0x2D0; // ntr -> door to MAPNUM_NTR_ENTRANCE (21)
            mapToDoor[MAPNUM_NTR_INTERIOR_E] = 0x2D0; // ntr -> door to MAPNUM_NTR_ENTRANCE (21)
            mapToDoor[MAPNUM_NTR_INTERIOR_F] = 0x2D0; // ntr -> door to MAPNUM_NTR_ENTRANCE (21)
            mapToDoor[MAPNUM_NTR_INTERIOR_G] = 0x2D0; // ntr -> door to MAPNUM_NTR_ENTRANCE (21)
            mapToDoor[MAPNUM_NTR_INTERIOR_H] = 0x2D0; // ntr -> door to MAPNUM_NTR_ENTRANCE (21)

            mapToDoor[MAPNUM_GRANDPALACE_INTERIOR_A] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_INTERIOR_B] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_INTERIOR_C] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_INTERIOR_D] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_INTERIOR_E] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_INTERIOR_F] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_HEXAS_ARENA] = 0x3d8; // dryad palace - upper section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_MECHRIDER3_BUTTON] = 0x3d8; // dryad palace - upper section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_GNOME_ORB] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_UNDINE_ORB] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_SYLPHID_ORB] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_SALAMANDO_ORB] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_LUMINA_ORB] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_SHADE_ORB] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_GRANDPALACE_LUNA_ORB] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)

            mapToDoor[MAPNUM_GRANDPALACE_INTERIOR_G] = 0x3AF; // dryad palace - orbs section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)

            mapToDoor[MAPNUM_SNAPDRAGON_ARENA] = 0x3d8; // dryad palace - upper section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_DRYADSEED] = 0x3d8; // dryad palace - upper section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)

            mapToDoor[MAPNUM_UNDERSEA_AXE_ROOM_A] = 0x30a; // dryad palace - undersea section (same door as subway area) -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_UNDERSEA_AXE_ROOM_B] = 0x30a; // dryad palace - undersea section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_HYDRA_ARENA_AFTER] = 0x30a; // dryad palace - undersea section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)
            mapToDoor[MAPNUM_UNDERSEA_AXE_ROOM_C] = 0x30a; // dryad palace - undersea section -> door to MAPNUM_SUNKENCONTINENT_EXTERIOR (195)

            int tableLocation = context.workingOffset;
            for (int i = 0; i < 512; i++)
            {
                if (mapToDoor.ContainsKey(i))
                {
                    outRom[context.workingOffset++] = (byte)mapToDoor[i];
                    outRom[context.workingOffset++] = (byte)(mapToDoor[i]>>8);
                }
                else
                {
                    outRom[context.workingOffset++] = 0;
                    outRom[context.workingOffset++] = 0;
                }
            }

            // code changes - ignore map flags, and read what goes into $0108 for the "return" door from the map above,
            // instead of storing it on a transition from outdoor to indoor maps and keeping it around
            /*
                $02/AB18 29 20 20    AND #$2020              A:8080 X:0004 Y:0004 P:eNvmXdIzc
                $02/AB1B F0 10       BEQ $10    [$AB2D]      A:0000 X:0004 Y:0004 P:envmXdIZc - remove
                $02/AB1D C9 20 20    CMP #$2020              A:2000 X:0001 Y:0004 P:envmXdIzc
                $02/AB20 F0 0B       BEQ $0B    [$AB2D]      A:2000 X:0001 Y:0004 P:eNvmXdIzc - remove
                $02/AB22 49 20 00    EOR #$0020              A:2000 X:0001 Y:0004 P:eNvmXdIzc
                $02/AB25 F0 03       BEQ $03    [$AB2A]      A:2020 X:0001 Y:0004 P:envmXdIzc - change to branch always
                $02/AB27 AD 0E 01    LDA $010E  [$00:010E]   A:2020 X:0001 Y:0004 P:envmXdIzc - load current door (skip now)
                $02/AB2A 8D 08 01    STA $0108  [$00:0108]   A:00D3 X:0001 Y:0004 P:envmXdIzc - save door for rope (replace)
                $02/AB2D E2 20       SEP #$20                A:00D3 X:0001 Y:0004 P:envmXdIzc - replace
                $02/AB2F 6B          RTL                     A:00D3 X:0001 Y:0004 P:envMXdIzc
             */

            outRom[0x2AB1B] = 0xEA;
            outRom[0x2AB1C] = 0xEA;

            outRom[0x2AB20] = 0xEA;
            outRom[0x2AB21] = 0xEA;

            outRom[0x2AB25] = 0x80; // branch always

            outRom[0x2AB2A] = 0x22;
            outRom[0x2AB2B] = (byte)(context.workingOffset);
            outRom[0x2AB2C] = (byte)(context.workingOffset >> 8);
            outRom[0x2AB2D] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x2AB2E] = 0xEA;
            // REP 10
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x10;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // LDA $7E00DC
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA $______,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)tableLocation;
            outRom[context.workingOffset++] = (byte)(tableLocation >> 8);
            outRom[context.workingOffset++] = (byte)((tableLocation >> 16) + 0xC0);
            // BEQ over
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x0D;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA $7E011E
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x1E;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x7E;
            // ORA #4000
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x40;
            // STA $7E011E - set magic rope available flag
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x1E;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x7E;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // over:
            // STA $0108
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x01;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // SEP 30
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x30;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            return true;
        }
    }
}
