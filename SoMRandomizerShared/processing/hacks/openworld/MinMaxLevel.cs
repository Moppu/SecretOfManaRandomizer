using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Hack that sets up a minimum and/or maximum level for enemies, based on the approximate vanilla level of enemies in that zone.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MinMaxLevel : RandoProcessor
    {
        public static string MINMAXLEVEL_MIN_OFFSET_HIROM = "enemyMinLevelOffset";
        public static string MINMAXLEVEL_MAX_OFFSET_HIROM = "enemyMaxLevelOffset";

        protected override string getName()
        {
            return "Open world enemy min/max level";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool doMinLevel = settings.getBool(OpenWorldSettings.PROPERTYNAME_MIN_ENEMY_LEVELS);
            bool doMaxLevel = settings.getBool(OpenWorldSettings.PROPERTYNAME_MAX_ENEMY_LEVELS);
            context.workingData.setInt(MINMAXLEVEL_MIN_OFFSET_HIROM, 0);
            context.workingData.setInt(MINMAXLEVEL_MAX_OFFSET_HIROM, 0);
            if(!doMinLevel && !doMaxLevel)
            {
                Logging.log("Skipping min/max level; neither setting enabled");
                return false;
            }

            // navel/forest 5
            // earthpalace/pandora 10
            // upperland 15
            // desert/icecountry 20
            // fire palace 20
            // northtown 25
            // shade 30
            // lumina/mountain 35
            // sunken 40
            // pure lands 45
            // manafort 50
            byte[] minLevels = new byte[512];
            for (int i = 0; i < 512; i++)
            {
                minLevels[i] = 0;
            }

            minLevels[MAPNUM_POTOS_SOUTH_FIELD] = 0; // outside potos
            minLevels[MAPNUM_JEHK_DUNGEON] = 35; // jehk johk training
            minLevels[MAPNUM_EMPIRE_SEWERS] = 25; // ST sewer
            minLevels[MAPNUM_NTR_ENTRANCE] = 25; // NTR entrance
            minLevels[MAPNUM_PANDORA_WEST_FIELD] = 0; // outside pandora
            minLevels[MAPNUM_GAIASNAVEL_EXTERIOR] = 5; // outside g/n
            minLevels[MAPNUM_GAIASNAVEL_NORTH_PATH] = 5; // flower guy map
            minLevels[MAPNUM_WITCHFOREST_A] = 5; // witch forest
            minLevels[MAPNUM_WITCHFOREST_B] = 5; // witch forest
            minLevels[MAPNUM_WITCHFOREST_C] = 5; // witch forest
            minLevels[MAPNUM_WITCHFOREST_D] = 5; // witch forest
            minLevels[MAPNUM_LUNAPALACE_SPACE] = 35; // luna palace
            minLevels[MAPNUM_UPPERLAND_NORTHWEST] = 15; // upper land
            minLevels[MAPNUM_UPPERLAND_SOUTHWEST] = 15; // upper land
            minLevels[MAPNUM_UPPERLAND_SOUTHEAST] = 15; // upper land
            minLevels[MAPNUM_UNDERSEA_SUBWAY_AREA_A] = 40; // subway
            minLevels[MAPNUM_UNDERSEA_SUBWAY_AREA_B] = 40; // subway
            minLevels[MAPNUM_UNDERSEA_SUBWAY_A] = 40; // subway
            minLevels[MAPNUM_UNDERSEA_SUBWAY_B] = 40; // subway
            minLevels[MAPNUM_LUNAPALACE_NORMAL] = 35; // luna palace
            minLevels[MAPNUM_UPPERLAND_WINTER] = 15; // upper land
            minLevels[MAPNUM_UPPERLAND_SPRING] = 15; // upper land
            minLevels[MAPNUM_UPPERLAND_SUMMER] = 15; // upper land
            minLevels[MAPNUM_UPPERLAND_FALL] = 15; // upper land
            minLevels[MAPNUM_UPPERLAND_WINTER_WITH_MOOGLES] = 15; // upper land
            minLevels[MAPNUM_SPRINGBEAK_ARENA] = 15; // spring beak
            minLevels[MAPNUM_SHADEPALACE_EXTERIOR] = 30; // shade mountains
            minLevels[MAPNUM_JEHKCAVE_EXTERIOR] = 30; // shade mountains
            minLevels[MAPNUM_DESERT_A] = 20; // desert
            minLevels[MAPNUM_DESERT_B] = 20; // desert
            minLevels[MAPNUM_DESERT_C] = 20; // desert
            minLevels[MAPNUM_DESERT_D] = 20; // desert
            minLevels[MAPNUM_DOPPEL_ARENA] = 35; // doppleganger
            minLevels[MAPNUM_ICECASTLE_EXTERIOR] = 20; // ice castle
            minLevels[MAPNUM_ICECOUNTRY_NORTHWEST] = 20; // ice country
            minLevels[MAPNUM_ICECOUNTRY_SOUTHWEST] = 20; // ice country
            minLevels[MAPNUM_BOREALFACE_ARENA] = 20; // boreal face
            minLevels[MAPNUM_ICECOUNTRY_NORTHEAST] = 20; // ice country
            minLevels[MAPNUM_ICECOUNTRY_SOUTHEAST] = 20; // ice country
            minLevels[MAPNUM_NTC_ENTRANCE] = 25; // NTC
            minLevels[MAPNUM_PATH_SOUTH_OF_ICE_CASTLE] = 20; // ice country
            minLevels[MAPNUM_DESERT_F] = 20; // desert
            minLevels[MAPNUM_DESERT_G] = 20; // desert
            minLevels[MAPNUM_DESERT_H] = 20; // desert
            minLevels[MAPNUM_DESERT_I] = 20; // desert
            minLevels[MAPNUM_FIREPALACE_ENTRANCE] = 20; // fp
            minLevels[MAPNUM_DESERT_J] = 20; // desert
            minLevels[MAPNUM_DESERT_K] = 20; // desert
            minLevels[MAPNUM_DESERT_L] = 20; // desert
            minLevels[MAPNUM_DESERT_STARS_A] = 20; // desert
            minLevels[MAPNUM_DESERT_STARS_B] = 20; // desert
            minLevels[MAPNUM_DESERT_STARS_C] = 20; // desert
            minLevels[MAPNUM_MECHRIDER2_ARENA] = 25; // NTC mech rider
            minLevels[MAPNUM_UNDINECAVE_EXTERIOR] = 10; // outside undine cave
            minLevels[MAPNUM_BLUESPIKE_ARENA] = 35; // lumina blue spike
            minLevels[MAPNUM_LUMINATOWER_1F] = 35; // lumina tower
            minLevels[MAPNUM_LUMINATOWER_2F] = 35; // lumina tower
            minLevels[MAPNUM_GORGONBULL_ARENA] = 35; // lumina gorgon bull
            minLevels[MAPNUM_TASNICA_THRONE_ROOM_ENEMY] = 35; // tasnica miniboss
            minLevels[MAPNUM_RABITEFIELD_A] = 0; // potos area
            minLevels[MAPNUM_RABITEFIELD_B] = 0; // potos area
            minLevels[MAPNUM_WATERPALACE_EXTERIOR] = 10; // outside water palace
            minLevels[MAPNUM_WATERPALACE_HALLWAY_ENEMY] = 10; // inside water palace
            minLevels[MAPNUM_JABBERWOCKY_ARENA] = 10; // jabberwocky
            minLevels[MAPNUM_PANDORA_RUINS_A] = 10; // pandora ruins
            minLevels[MAPNUM_PANDORA_RUINS_B] = 10; // pandora ruins
            minLevels[MAPNUM_PANDORA_RUINS_C] = 10; // pandora ruins
            minLevels[MAPNUM_PANDORA_RUINS_ROOMS] = 10; // pandora ruins
            minLevels[MAPNUM_WALLFACE_ARENA] = 10; // pandora ruins wall
            minLevels[MAPNUM_SUNKENCONTINENT_EXTERIOR] = 40; // sunken outdoors
            minLevels[MAPNUM_WITCHCASTLE_EXTERIOR] = 5; // witch castle outdoors
            minLevels[MAPNUM_PURELANDS_A_AND_MANA_TREE] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_B] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_C] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_D] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_E] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_F] = 45; // purelands
            minLevels[MAPNUM_DRAGONWORM_ARENA] = 45; // purelands (boss)
            minLevels[MAPNUM_PURELANDS_G] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_H] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_I] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_J] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_K] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_L] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_M] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_N] = 45; // purelands
            minLevels[MAPNUM_AXEBEAK_ARENA] = 45; // purelands (boss)
            minLevels[MAPNUM_PURELANDS_O] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_P] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_Q] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_R] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_S] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_T] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_U] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_V] = 45; // purelands
            minLevels[MAPNUM_MATANGO_BACKYARD] = 15; // matango
            minLevels[MAPNUM_PURELANDS_W] = 45; // purelands
            minLevels[MAPNUM_THUNDERGIGAS_ARENA] = 45; // purelands (boss)
            minLevels[MAPNUM_PURELANDS_X] = 45; // purelands
            minLevels[MAPNUM_PURELANDS_Y] = 45; // purelands
            minLevels[MAPNUM_SNOWDRAGON_ARENA] = 45; // purelands (boss)
            minLevels[MAPNUM_REDDRAGON_ARENA] = 45; // purelands (boss)
            minLevels[MAPNUM_BLUEDRAGON_ARENA] = 45; // purelands (boss)
            minLevels[MAPNUM_MECHRIDER3_ARENA] = 40; // mech rider 3
            minLevels[MAPNUM_WATERMELON_ARENA] = 40; // watermelon
            minLevels[MAPNUM_BUFFY_ARENA] = 50; // buffy
            minLevels[MAPNUM_DARKLICH_ARENA] = 55; // lich
            minLevels[MAPNUM_DARKLICH_ARENA_B] = 55; // lich copy?
            minLevels[MAPNUM_MANAFORT_INTERIOR_A] = 50; // manafort
            minLevels[MAPNUM_MANAFORT_INTERIOR_B] = 50; // manafort
            minLevels[MAPNUM_MANAFORT_INTERIOR_C] = 50; // manafort
            minLevels[MAPNUM_MANABEAST_ARENA] = 60; // mana beast
            minLevels[MAPNUM_MANTISANT_ARENA] = 0; // mantis ant
            minLevels[MAPNUM_EARTHPALACE_TRANSITIONS] = 10; // ep
            minLevels[MAPNUM_UNDINECAVE_FISH_ROOM] = 10; // undine cave
            minLevels[MAPNUM_TONPOLE_ARENA] = 10; // tonpole
            minLevels[MAPNUM_GAIASNAVEL_INTERIOR_A_ENEMY] = 5; // gn
            minLevels[MAPNUM_GAIASNAVEL_INTERIOR_B] = 5; // gn
            minLevels[MAPNUM_GAIASNAVEL_INTERIOR_C] = 5; // gn
            minLevels[MAPNUM_MAGICROPE] = 5; // gn
            minLevels[MAPNUM_TROPICALLO_ARENA] = 5; // tropicallo
            minLevels[MAPNUM_WITCHCASTLE_INTERIOR_A] = 5; // witch castle
            minLevels[MAPNUM_WITCHCASTLE_INTERIOR_B] = 5; // witch castle
            minLevels[MAPNUM_WITCHCASTLE_INTERIOR_C] = 5; // witch castle
            minLevels[MAPNUM_WITCHCASTLE_INTERIOR_D] = 5; // witch castle
            minLevels[MAPNUM_WITCHCASTLE_INTERIOR_E] = 5; // witch castle
            minLevels[MAPNUM_WITCHCASTLE_INTERIOR_F] = 5; // witch castle
            minLevels[MAPNUM_WITCHCASTLE_NESOBERI] = 5; // witch castle
            minLevels[MAPNUM_MATANGOCAVE_GNOME_ORB] = 15; // matango
            minLevels[MAPNUM_SPIKEY_ARENA] = 5; // spikey
            minLevels[MAPNUM_EARTHPALACE_INTERIOR_A] = 10; // ep
            minLevels[MAPNUM_EARTHPALACE_INTERIOR_B] = 10; // ep
            minLevels[MAPNUM_KILROY_ARENA] = 10; // kilroy
            minLevels[MAPNUM_MATANGOCAVE_A] = 15; // matango
            minLevels[MAPNUM_MATANGOCAVE_B] = 15; // matango
            minLevels[MAPNUM_MATANGOCAVE_C] = 15; // matango
            minLevels[MAPNUM_MATANGOCAVE_D] = 15; // matango
            minLevels[MAPNUM_GREATVIPER_ARENA] = 15; // matango snake
            minLevels[MAPNUM_FIREPALACE_A] = 20; // fp
            minLevels[MAPNUM_FIREPALACE_B] = 20; // fp
            minLevels[MAPNUM_FIREPALACE_C] = 20; // fp
            minLevels[MAPNUM_FIREPALACE_D] = 20; // fp
            minLevels[MAPNUM_FIREPALACE_E] = 20; // fp
            minLevels[MAPNUM_FIREPALACE_F] = 20; // fp
            minLevels[MAPNUM_WITCHCASTLE_ROOM_A] = 5; // witch castle
            minLevels[MAPNUM_WITCHCASTLE_ROOM_B] = 5; // witch castle
            minLevels[MAPNUM_FIREPALACE_G] = 20; // fp
            minLevels[MAPNUM_FIREPALACE_H] = 20; // fp
            minLevels[MAPNUM_MINOTAUR_ARENA] = 20; // fp bull
            minLevels[MAPNUM_FIREPALACE_FIRST_ORB] = 20; // fp
            minLevels[MAPNUM_ICECASTLE_INTERIOR_A] = 20; // ice castle
            minLevels[MAPNUM_ICECASTLE_INTERIOR_B] = 20; // ice castle
            minLevels[MAPNUM_ICECASTLE_INTERIOR_C] = 20; // ice castle
            minLevels[MAPNUM_ICECASTLE_INTERIOR_D] = 20; // ice castle
            minLevels[MAPNUM_ICECASTLE_INTERIOR_E] = 20; // ice castle
            minLevels[MAPNUM_ICECASTLE_INTERIOR_F] = 20; // ice castle
            minLevels[MAPNUM_ICECASTLE_INTERIOR_G] = 20; // ice castle
            minLevels[MAPNUM_ICECASTLE_INTERIOR_H] = 20; // ice castle
            minLevels[MAPNUM_ICECASTLE_INTERIOR_I] = 20; // ice castle
            minLevels[MAPNUM_TRIPLETONPOLE_ARENA] = 20; // ice castle tonpole
            minLevels[MAPNUM_ICECASTLE_INTERIOR_J] = 20; // ice castle
            minLevels[MAPNUM_FROSTGIGAS_ARENA] = 20; // ice castle gigas
            minLevels[MAPNUM_ICECASTLE_INTERIOR_K] = 20; // ice castle
            minLevels[MAPNUM_SHADEPALACE_INTERIOR_A] = 30; // dp
            minLevels[MAPNUM_SHADEPALACE_INTERIOR_B] = 30; // dp
            minLevels[MAPNUM_SHADEPALACE_INTERIOR_C] = 30; // dp
            minLevels[MAPNUM_SHADEPALACE_INTERIOR_D] = 30; // dp
            minLevels[MAPNUM_SHADEPALACE_INTERIOR_E] = 30; // dp
            minLevels[MAPNUM_SHADEPALACE_INTERIOR_F] = 30; // dp
            minLevels[MAPNUM_SHADEPALACE_INTERIOR_G] = 30; // dp
            minLevels[MAPNUM_SHADEPALACE_INTERIOR_H] = 30; // dp
            minLevels[MAPNUM_METALMANTIS_ARENA] = 25; // ntc metal mantis
            minLevels[MAPNUM_NTC_INTERIOR_B] = 25; // ntc
            minLevels[MAPNUM_NTC_INTERIOR_D] = 25; // ntc
            minLevels[MAPNUM_NTC_INTERIOR_E] = 25; // ntc
            minLevels[MAPNUM_NTC_INTERIOR_F] = 25; // ntc
            minLevels[MAPNUM_NTC_INTERIOR_G] = 25; // ntc
            minLevels[MAPNUM_NTC_INTERIOR_H] = 25; // ntc
            minLevels[MAPNUM_NTC_INTERIOR_I] = 25; // ntc
            minLevels[MAPNUM_NTC_INTERIOR_K] = 25; // ntc
            minLevels[MAPNUM_NTC_INTERIOR_L] = 25; // ntc
            minLevels[MAPNUM_NTC_INTERIOR_M] = 25; // ntc
            minLevels[MAPNUM_NTR_INTERIOR_A] = 25; // ntr
            minLevels[MAPNUM_NTR_INTERIOR_B] = 25; // ntr
            minLevels[MAPNUM_NTR_INTERIOR_C] = 25; // ntr
            minLevels[MAPNUM_NTR_INTERIOR_D] = 25; // ntr
            minLevels[MAPNUM_NTR_INTERIOR_E] = 25; // ntr
            minLevels[MAPNUM_NTR_INTERIOR_F] = 25; // ntr
            minLevels[MAPNUM_NTR_INTERIOR_G] = 25; // ntr
            minLevels[MAPNUM_DOOMSWALL_ARENA] = 25; // ntr wall
            minLevels[MAPNUM_VAMPIRE_ARENA] = 25; // ntr vampire
            minLevels[MAPNUM_NTR_INTERIOR_H] = 25; // ntr
            minLevels[MAPNUM_GRANDPALACE_INTERIOR_A] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_INTERIOR_B] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_INTERIOR_C] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_INTERIOR_D] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_INTERIOR_E] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_INTERIOR_F] = 40; // sunken
            minLevels[MAPNUM_HEXAS_ARENA] = 40; // sunken hexas
            minLevels[MAPNUM_GRANDPALACE_GNOME_ORB] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_UNDINE_ORB] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_SYLPHID_ORB] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_SALAMANDO_ORB] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_LUMINA_ORB] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_SHADE_ORB] = 40; // sunken
            minLevels[MAPNUM_GRANDPALACE_LUNA_ORB] = 40; // sunken
            minLevels[MAPNUM_SNAPDRAGON_ARENA] = 40; // sunken tonpole thing
            minLevels[MAPNUM_UNDERSEA_AXE_ROOM_A] = 40; // sunken
            minLevels[MAPNUM_UNDERSEA_AXE_ROOM_B] = 40; // sunken
            minLevels[MAPNUM_HYDRA_ARENA] = 40; // sunken hydra
            minLevels[MAPNUM_UNDERSEA_AXE_ROOM_C] = 40; // sunken
            minLevels[MAPNUM_DREADSLIME_ARENA] = 50; // dread slime
            minLevels[MAPNUM_LIMESLIME_ARENA] = 30; // lime slime
            minLevels[MAPNUM_FIREGIGAS_ARENA] = 10; // ep fire gigas

            for (int i = 0; i < 512; i++)
            {
                if(minLevels[i] != 0)
                {
                    // off by one; 00 = level 01
                    minLevels[i]--;
                }
            }

            // write tables that get used later by EnemiesAtYourLevel in the enemy level loader
            if (doMinLevel)
            {
                CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 512);
                context.workingData.setInt(MINMAXLEVEL_MIN_OFFSET_HIROM, context.workingOffset + 0xC00000);
                for (int i = 0; i < 512; i++)
                {
                    outRom[context.workingOffset++] = minLevels[i];
                }
            }

            if (doMaxLevel)
            {
                byte[] maxLevels = new byte[512];
                for (int i = 0; i < 512; i++)
                {
                    maxLevels[i] = (byte)(minLevels[i] + 5);
                }

                CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 512);
                context.workingData.setInt(MINMAXLEVEL_MAX_OFFSET_HIROM, context.workingOffset + 0xC00000);
                for (int i = 0; i < 512; i++)
                {
                    outRom[context.workingOffset++] = maxLevels[i];
                }
            }
            return true;
        }

    }
}
