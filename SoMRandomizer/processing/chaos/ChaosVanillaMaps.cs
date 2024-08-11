using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.chaos
{
    /// <summary>
    /// Collection of vanilla maps used for chaos mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosVanillaMaps
    {
        public static List<ChaosVanillaMap> chaosMaps = new List<ChaosVanillaMap>();
        public static ChaosVanillaMap startMap = new ChaosVanillaMap(128,
        new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0,1 }, 1, 45, 51),
        }.ToList()
        , false, new byte[] { }, false, new int[] { 0, 1, 2 }, false, new int[] {2,3 });

        // generate after
        public static Dictionary<int, List<ChaosVanillaMap>> chaosMapsByExitNum = new Dictionary<int, List<ChaosVanillaMap>>();
        public static Dictionary<int, List<ChaosVanillaMap>> townMapsByExitNum = new Dictionary<int, List<ChaosVanillaMap>>();

        public static Dictionary<int, ChaosBossMap> bossMaps = new Dictionary<int, ChaosBossMap>();
        public static ChaosBossMap finalBossMap = new ChaosBossMap(253, 18, 22, false, new byte[] { 0 }, new byte[] { });

        public static ChaosVanillaMap creditsMap = new ChaosVanillaMap(180, new ChaosDoor[] {
            new ChaosDoor(new int[]{ }, 0, 8, 6)
        }.ToList(), false, new byte[] { }, false, new int[] { }, false, new int[] { });

        private static byte[] cavePalettes = new byte[] { 34, 101, 102, 106, };
        private static byte[] witchForestPalettes = new byte[] { };
        private static byte[] subwayPalettes = new byte[] { };
        private static byte[] upperLandPalettes = new byte[] { 63, 64, 65, 66 };
        private static byte[] palacePalettes = new byte[] { 46, 47, 55, 74, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, };
        private static byte[] ruinsPalettes = new byte[] { 56, 58, 85, 86, 87, };
        private static byte[] ruinsWithCloudsPalettes = new byte[] { 33, 60, 68, 75, 79, };
        private static byte[] forestPalettes = new byte[] { 25, 26, 37, 62, 63, 64, 65, 66, };
        private static byte[] desertPalettes = new byte[] { };
        private static byte[] pureLandPalettes = new byte[] { 54, 104, 115, };
        private static byte[] ruinsExteriorPalettes = new byte[] { 38, 41, 56, 58, 81, 82, 83, 84, 85, };

        static ChaosVanillaMaps()
        {
            // 0 up 1 right 2 down 3 left, 4 any

            // ///////////////////////////////////////////////
            // two-exit maps
            // ///////////////////////////////////////////////
            // sage johk map with the pumpkins and shit
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_JEHK_DUNGEON,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 1,2,3,4 }, 0, 110, 8),
                    new ChaosDoor(new int[]{ 5,6,7 }, 2, 27, 38),
                }.ToList()
                , false, cavePalettes, false, new int[] { }, false, new int[] { }));

            // witch's forest map 1
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_WITCHFOREST_A,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0 }, 4, 28, 11),
                    new ChaosDoor(new int[]{ 1 }, 4, 10, 37),
                }.ToList()
                , false, witchForestPalettes, true, new int[] { }, true, new int[] { }));

            // witch's forest map 2
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_WITCHFOREST_D,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0 }, 4, 11, 11),
                    new ChaosDoor(new int[]{ 1 }, 4, 25, 38),
                }.ToList()
                , false, witchForestPalettes, true, new int[] { }, false, new int[] { }));

            // upper land
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UPPERLAND_SOUTHWEST,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 0, 11, 21),
                                new ChaosDoor(new int[]{ 3,4,5 }, 1, 39, 51),
                }.ToList()
                , false, upperLandPalettes, true, new int[] { }, false, new int[] { }));

            // subway
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UNDERSEA_SUBWAY_AREA_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0 }, 0, 12, 13),
                                new ChaosDoor(new int[]{ 2,3,4,5 }, 0, 6, 28),
                }.ToList()
                , false, new byte[] { 34, 37, 57, }, false, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UNDERSEA_SUBWAY_A,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0 }, 2, 8, 16),
                                new ChaosDoor(new int[]{ 1 }, 2, 118, 16),
                }.ToList()
                , false, subwayPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UNDERSEA_SUBWAY_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0 }, 2, 8, 16),
                                new ChaosDoor(new int[]{ 1 }, 2, 118, 16),
                }.ToList()
                , false, subwayPalettes, false, new int[] { }, true, new int[] { }));

            // luna palace
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_LUNAPALACE_NORMAL,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 30, 12),
                                new ChaosDoor(new int[]{ 2,3 }, 2, 30, 33),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, true, new int[] { }));

            // upper land winter
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UPPERLAND_WINTER,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 1, 39, 20),
                                new ChaosDoor(new int[]{ 3,4 }, 2, 11, 47),
                }.ToList()
                , false, upperLandPalettes, true, new int[] { }, false, new int[] { }));

            // upper land fall
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UPPERLAND_FALL,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 3, 8, 24),
                                new ChaosDoor(new int[]{ 2,3,4 }, 2, 32, 47),
                }.ToList()
                , false, upperLandPalettes, true, new int[] { }, false, new int[] { }));

            // cliffs outside jehk place
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_JEHKCAVE_EXTERIOR,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 1,2 }, 0, 27, 20),
                                new ChaosDoor(new int[]{ 3 }, 2, 19, 55),
                }.ToList()
                , false, new byte[] { }, true, new int[] { 0 }, false, new int[] { }));

            // desert
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_DESERT_D,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 0, 23, 14),
                                new ChaosDoor(new int[]{ 3,4,5 }, 3, 7, 26),
                }.ToList()
                , false, desertPalettes, true, new int[] { }, false, new int[] { }));

            // ice castle entrance
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_ICECASTLE_EXTERIOR,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 19, 15),
                                new ChaosDoor(new int[]{ 2,3 }, 2, 19, 24),
                }.ToList()
                , false, new byte[] { }, true, new int[] { }, true, new int[] { }));

            // ice country
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_ICECOUNTRY_SOUTHWEST,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 16, 17),
                                new ChaosDoor(new int[]{ 2,3 }, 1, 38, 31),
                }.ToList()
                , false, new byte[] { 92, 94, }, true, new int[] { }, false, new int[] { }));

            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_ICECOUNTRY_NORTHEAST,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0 }, 1, 54, 26),
                                new ChaosDoor(new int[]{ 1,2 }, 2, 13, 39),
                }.ToList()
                , false, new byte[] { 92, 94, }, true, new int[] { }, false, new int[] { }));

            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PATH_SOUTH_OF_ICE_CASTLE,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 15, 17),
                                new ChaosDoor(new int[]{ 2,3 }, 2, 15, 39),
                }.ToList()
                , false, new byte[] { 92, 94, }, true, new int[] { }, true, new int[] { }));

            // desert fire palace entrance thing
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_FIREPALACE_ENTRANCE,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 29, 19),
                                new ChaosDoor(new int[]{ 2,3 }, 2, 25, 59),
                }.ToList()
                , false, new byte[] { 55, 94, }, true, new int[] { }, false, new int[] { }));

            // desert
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_DESERT_K,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 0, 23, 14),
                                new ChaosDoor(new int[]{ 3,4,5 }, 1, 39, 29),
                }.ToList()
                , false, desertPalettes, true, new int[] { }, false, new int[] { }));

            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_DESERT_STARS_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2,3 }, 0, 23, 14),
                                new ChaosDoor(new int[]{ 4,5,6 }, 2, 21, 40),
                }.ToList()
                , false, desertPalettes, true, new int[] { }, false, new int[] { }));

            // entrance to undine cave
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UNDINECAVE_EXTERIOR,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 12, 8),
                                new ChaosDoor(new int[]{ 2,3,4 }, 3, 5, 15),
                }.ToList()
                , false, new byte[] { 37, }, true, new int[] { }, true, new int[] { }));

            // lumina palace
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_LUMINATOWER_2F,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 8,9, }, 0, 23, 27),
                                new ChaosDoor(new int[]{ 10,11, }, 2, 11, 30),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, true, new int[] { }));

            // path to potos
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_RABITEFIELD_A,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 3, 17, 37),
                                new ChaosDoor(new int[]{ 3, }, 3, 12, 61),
                }.ToList()
                , false, new byte[] { 37, }, true, new int[] { }, false, new int[] { 2 }));

            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_RABITEFIELD_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 1, }, 1, 31, 11),
                                new ChaosDoor(new int[]{ 2, }, 1, 31, 31),
                }.ToList()
                , false, new byte[] { 37, }, true, new int[] { }, true, new int[] { 0 }));

            // water palace hallway
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_WATERPALACE_HALLWAY_ENEMY,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 9, 8),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 9, 25),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, true, new int[] { }));

            // ruins
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PANDORA_RUINS_A,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 8,9, }, 2, 13, 48),
                                new ChaosDoor(new int[]{ 10,11,12, }, 2, 7, 55),
                }.ToList()
                , true, ruinsWithCloudsPalettes, true, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PANDORA_RUINS_C,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 0, 56, 11),
                                new ChaosDoor(new int[]{ 1,2, }, 2, 12, 35),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));

            // witch castle entrance
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_WITCHCASTLE_EXTERIOR,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 23, 20),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 23, 48),
                }.ToList()
                , false, ruinsExteriorPalettes, true, new int[] { }, false, new int[] { }));

            // pure land
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_C,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 15, 15),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 15, 27),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_E,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 1, 32, 21),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 11, 27),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_J,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 18, 15),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 18, 28),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_N,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 15, 16),
                                new ChaosDoor(new int[]{ 2,3, }, 0, 9, 45),
                }.ToList()
                , false, cavePalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_O,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 2, 11, 19),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 18, 20),
                }.ToList()
                , false, cavePalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_P,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 15, 16),
                                new ChaosDoor(new int[]{ 2,3, }, 0, 20, 24),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_R,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 14, 20),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 19, 37),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_S,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 1, 32, 21),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 12, 27),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_V,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 3, 10, 17),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 18, 29),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));

            // matango
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_MATANGO_BACKYARD,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 0, 30, 10),
                                new ChaosDoor(new int[]{ 1,2, }, 2, 17, 47),
                }.ToList()
                , true, new byte[] { 40, 51, 75, 78, }, true, new int[] { }, true, new int[] { }));

            // more pureland
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_X,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 27, 5),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 21, 26),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));

            // manafort int
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_MANAFORT_INTERIOR_C,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 1, }, 4, 97, 90),
                    new ChaosDoor(new int[]{ 2, }, 4, 37, 96),
                }.ToList()
                , false, new byte[] { }, false, new int[] { }, false, new int[] { }));

            // gnome palace or something - leftmost room
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_EARTHPALACE_TRANSITIONS,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0,1, }, 0, 8, 10),
                    new ChaosDoor(new int[]{ 10,11 }, 2, 15, 21),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));
            // middle room
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_EARTHPALACE_TRANSITIONS,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 4,5, }, 0, 27, 11),
                    new ChaosDoor(new int[]{ 6,7 }, 2, 27, 20),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));
            // right room
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_EARTHPALACE_TRANSITIONS,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 2,3, }, 0, 40, 10),
                    new ChaosDoor(new int[]{ 8,9 }, 2, 40, 20),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));

            // undine cave entrance
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UNDINECAVE_FISH_ROOM,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 11, 10),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 18, 21),
                }.ToList()
                , false, cavePalettes, true, new int[] { }, true, new int[] { }));

            // gaia's navel entrance north
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_GAIASNAVEL_INTERIOR_A_ENEMY,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 2, 23, 14),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 28, 31),
                }.ToList()
                , false, cavePalettes, true, new int[] { }, false, new int[] { }));
            // south
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_GAIASNAVEL_INTERIOR_A_ENEMY,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 4,5, }, 0, 28, 48),
                                new ChaosDoor(new int[]{ 10,11, }, 2, 23, 70),
                }.ToList()
                , false, cavePalettes, true, new int[] { }, false, new int[] { }));

            // gaia's navel
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_GAIASNAVEL_INTERIOR_C,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 16, 17),
                                new ChaosDoor(new int[]{ 2,3, }, 0, 20, 26),
                }.ToList()
                , false, cavePalettes, true, new int[] { }, false, new int[] { }));

            // witch castle chairs room
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_WITCHCASTLE_INTERIOR_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2, }, 0, 10, 10),
                                new ChaosDoor(new int[]{ 3, }, 2, 10, 20),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, true, new int[] { }));
            // pre-spikey
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_WITCHCASTLE_NESOBERI,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 12, 12),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 12, 30),
                }.ToList()
                , false, ruinsPalettes, true, new int[] { 0, 3, 4, 5, 6, }, true, new int[] { }));
            // gnome palace whip room
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_EARTHPALACE_INTERIOR_B,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0,1, }, 0, 12, 11),
                    new ChaosDoor(new int[]{ 16,17 }, 2, 38, 38),
                }.ToList()
                , false, palacePalettes, true, new int[] { }, false, new int[] { }));

            // cave north
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_MATANGOCAVE_C,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 2, 6, 14),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 27, 17),
                }.ToList()
                , false, cavePalettes, true, new int[] { }, false, new int[] { }));
            // and south
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_MATANGOCAVE_C,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 4,5, }, 0, 10, 29),
                                new ChaosDoor(new int[]{ 8,9, }, 2, 25, 32),
                }.ToList()
                , false, cavePalettes, true, new int[] { }, false, new int[] { }));

            // cave
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_MATANGOCAVE_D,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 2,3, }, 2, 13, 22),
                                new ChaosDoor(new int[]{ 4,5, }, 2, 19, 34),
                }.ToList()
                , false, cavePalettes, false, new int[] { }, false, new int[] { }));

            // palace A
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_FIREPALACE_C,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0,1, }, 0, 7, 11),
                    new ChaosDoor(new int[]{ 2,3 }, 0, 17, 11),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));
            // palace B
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_FIREPALACE_C,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 4,5, }, 0, 13, 22),
                    new ChaosDoor(new int[]{ 6,7 }, 0, 20, 22),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));
            // palace C
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_FIREPALACE_E,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 2,3, }, 0, 5, 17),
                    new ChaosDoor(new int[]{ 6,7 }, 2, 40, 21),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));
            // palace D
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_FIREPALACE_E,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 4,5, }, 0, 34, 19),
                    new ChaosDoor(new int[]{ 8,9 }, 2, 46, 21),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));
            // palace E
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_FIREPALACE_H,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 4,5, }, 0, 22, 16),
                    new ChaosDoor(new int[]{ 8,9, }, 2, 26, 20),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));

            // ice castle
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_ICECASTLE_INTERIOR_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 0, 10, 11),
                                new ChaosDoor(new int[]{ 1,2,3, }, 2, 10, 18),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_ICECASTLE_INTERIOR_G,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2, }, 0, 10, 10),
                                new ChaosDoor(new int[]{ 3, }, 2, 10, 19),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_ICECASTLE_INTERIOR_H,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 0, 31, 12),
                                new ChaosDoor(new int[]{ 1,2, }, 2, 4, 15),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_ICECASTLE_INTERIOR_J,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 1, }, 4, 15, 18),
                                new ChaosDoor(new int[]{ 2, }, 2, 7, 33),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, false, new int[] { }));

            // shade cave
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_SHADEPALACE_INTERIOR_A,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 9, 8),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 9, 13),
                }.ToList()
                , false, cavePalettes, true, new int[] { }, false, new int[] { }));

            // palace 1
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_SHADEPALACE_INTERIOR_C,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 4,5, }, 0, 8, 10),
                    new ChaosDoor(new int[]{ 8,9 }, 2, 8, 24),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));
            // palace 2
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_SHADEPALACE_INTERIOR_C,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 6,7, }, 0, 26, 14),
                    new ChaosDoor(new int[]{ 10,11 }, 2, 26, 24),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));

            // more shade palace
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_SHADEPALACE_INTERIOR_G,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0,1, }, 0, 9, 5),
                    new ChaosDoor(new int[]{ 2,3 }, 2, 9, 22),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, true, new int[] { }));

            // idk indoors somewhere
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTC_INTERIOR_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2, }, 0, 10, 10),
                                new ChaosDoor(new int[]{ 3, }, 2, 10, 19),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTC_INTERIOR_D,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 2, 13, 17),
                                new ChaosDoor(new int[]{ 1,2,3, }, 2, 8, 18),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, true, new int[] { }));

            // indoors A
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTC_INTERIOR_H,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 2, 16, 17),
                                new ChaosDoor(new int[]{ 8, }, 2, 10, 32),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, false, new int[] { }));
            // indoors B
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTC_INTERIOR_H,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 4,5, }, 0, 17, 26),
                                new ChaosDoor(new int[]{ 9, }, 2, 22, 32),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, false, new int[] { }));
            // indoors C
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTC_INTERIOR_H,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 2,3, }, 2, 23, 17),
                                new ChaosDoor(new int[]{ 6,7, }, 0, 29, 29),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, false, new int[] { }));

            // indoors
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTC_INTERIOR_K,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 0, 10, 11),
                                new ChaosDoor(new int[]{ 1,2,3, }, 2, 10, 18),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTC_INTERIOR_L,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 6, 12),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 16, 28),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, true, new int[] { }));

            // ruins A
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 0, 72, 14),
                                new ChaosDoor(new int[]{ 4, }, 2, 72, 22),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // ruins B
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 1, }, 2, 19, 18),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 11, 21),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // ruins C
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 5, }, 0, 9, 42),
                                new ChaosDoor(new int[]{ 9, }, 2, 9, 52),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // ruins D
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 6, }, 0, 40, 42),
                                new ChaosDoor(new int[]{ 10, }, 2, 40, 52),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // ruins E holy shit this map has a lot of rooms
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 7, }, 0, 72, 42),
                                new ChaosDoor(new int[]{ 8, }, 2, 72, 50),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));

            // ruins 1
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_D,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 2, }, 2, 35, 18),
                                new ChaosDoor(new int[]{ 3, }, 2, 53, 18),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // ruins 2
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_D,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 15, }, 0, 13, 92),
                                new ChaosDoor(new int[]{ 16, }, 0, 31, 92),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // ruins 3
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_D,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 0, 94, 14),
                                new ChaosDoor(new int[]{ 4, }, 2, 94, 22),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // ruins 4
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_D,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 14, }, 0, 92, 78),
                                new ChaosDoor(new int[]{ 17,18, }, 2, 93, 89),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));

            // ruins i
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_E,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 2, 44, 18),
                                new ChaosDoor(new int[]{ 1,2, }, 2, 35, 22),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // ruins ii
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_F,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 10, 15),
                                new ChaosDoor(new int[]{ 4,5, }, 0, 20, 32),
                }.ToList()
                , true, ruinsWithCloudsPalettes, true, new int[] { }, true, new int[] { }));

            // holy shit there are so many ruins maps A
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_G,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 2, 10, 18),
                                new ChaosDoor(new int[]{ 1, }, 2, 28, 18),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // holy shit there are so many ruins maps B
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_G,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 2, }, 2, 54, 18),
                                new ChaosDoor(new int[]{ 3, }, 2, 72, 18),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // holy shit there are so many ruins maps C
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_G,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 4, }, 0, 12, 39),
                                new ChaosDoor(new int[]{ 8, }, 2, 12, 48),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // holy shit there are so many ruins maps D
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_G,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 5, }, 0, 33, 39),
                                new ChaosDoor(new int[]{ 9, }, 2, 33, 48),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // holy shit there are so many ruins maps E
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_G,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 6, }, 0, 56, 39),
                                new ChaosDoor(new int[]{ 10, }, 2, 56, 48),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));
            // holy shit there are so many ruins maps F
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_G,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 7, }, 0, 77, 39),
                                new ChaosDoor(new int[]{ 11, }, 2, 77, 48),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));

            // undersea dingaling
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UNDERSEA_AXE_ROOM_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 14, 18),
                                new ChaosDoor(new int[]{ 3,4, }, 2, 67, 61),
                }.ToList()
                , true, new byte[] { }, true, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UNDERSEA_AXE_ROOM_C,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 4,5, }, 2, 10, 48),
                                new ChaosDoor(new int[]{ 6,7, }, 2, 16, 54),
                }.ToList()
                , true, new byte[] { }, true, new int[] { }, true, new int[] { }));



            // ///////////////////////////////////////////////
            // three-exit maps
            // ///////////////////////////////////////////////

            // 0 up 1 right 2 down 3 left, 4 any

            // near gaia's navel
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PANDORA_WEST_FIELD,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 21, 25),
                                new ChaosDoor(new int[]{ 3,4 }, 3, 14, 39),
                                new ChaosDoor(new int[]{ 5,6 }, 1, 85, 92),
                }.ToList()
                , false, forestPalettes, true, new int[] { }, false, new int[] { 2 }));

            // between gaia's navel and witch forest
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_GAIASNAVEL_NORTH_PATH,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 9, 14),
                                new ChaosDoor(new int[]{ 3,4,5 }, 1, 26, 40),
                                new ChaosDoor(new int[]{ 6,7,8,9,10 }, 2, 15, 48),
                }.ToList()
                , false, forestPalettes, true, new int[] { 2 }, true, new int[] { }));

            // witch forest map 2
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_WITCHFOREST_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0 }, 4, 24, 11),
                                new ChaosDoor(new int[]{ 1 }, 4, 22, 22),
                                new ChaosDoor(new int[]{ 2 }, 4, 13, 38),
                }.ToList()
                , false, witchForestPalettes, false, new int[] { }, false, new int[] { }));


            // upper land
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UPPERLAND_NORTHWEST,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 29, 17),
                                new ChaosDoor(new int[]{ 2,3, }, 1, 41, 50),
                                new ChaosDoor(new int[]{ 4,5, }, 2, 20, 65),
                }.ToList()
                , false, upperLandPalettes, true, new int[] { }, false, new int[] { }));

            // cliffs outside shade place
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_SHADEPALACE_EXTERIOR,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 11, 8),
                                new ChaosDoor(new int[]{ 2,3 }, 0, 50, 8),
                                new ChaosDoor(new int[]{ 4,5 }, 0, 33, 17),
                }.ToList()
                , false, new byte[] { }, true, new int[] { }, true, new int[] { }));

            // ice country
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_ICECOUNTRY_NORTHWEST,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 16, 17),
                                new ChaosDoor(new int[]{ 2,3 }, 0, 36, 17),
                                new ChaosDoor(new int[]{ 4,5 }, 2, 35, 55),
                }.ToList()
                , false, new byte[] { 92, 94, }, true, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_ICECOUNTRY_SOUTHEAST,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 17, 17),
                                new ChaosDoor(new int[]{ 3,4 }, 1, 39, 30),
                                new ChaosDoor(new int[]{ 5,6,7 }, 3, 7, 38),
                }.ToList()
                , false, new byte[] { 92, 94, }, true, new int[] { }, false, new int[] { 2 }));

            // desert
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_DESERT_G,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 3, 7, 26),
                                new ChaosDoor(new int[]{ 3,4,5 }, 1, 40, 29),
                                new ChaosDoor(new int[]{ 6,7,8 }, 2, 21, 40),
                }.ToList()
                , false, desertPalettes, true, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_DESERT_J,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 0, 23, 14),
                                new ChaosDoor(new int[]{ 3,4,5 }, 3, 7, 26),
                                new ChaosDoor(new int[]{ 6,7,8 }, 1, 40, 29),
                }.ToList()
                , false, desertPalettes, true, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_DESERT_L,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 3, 7, 26),
                                new ChaosDoor(new int[]{ 3,4,5 }, 1, 40, 29),
                                new ChaosDoor(new int[]{ 6,7,8 }, 2, 21, 40),
                }.ToList()
                , false, desertPalettes, true, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_DESERT_STARS_A,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 0, 23, 14),
                                new ChaosDoor(new int[]{ 3,4,5 }, 3, 7, 26),
                                new ChaosDoor(new int[]{ 6,7,8 }, 1, 40, 29),
                }.ToList()
                , false, desertPalettes, true, new int[] { }, false, new int[] { }));

            // pure land
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 18, 15),
                                new ChaosDoor(new int[]{ 2,3, }, 0, 10, 20),
                                new ChaosDoor(new int[]{ 4,5, }, 2, 18, 28),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_F,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 15, 19),
                                new ChaosDoor(new int[]{ 2,3, }, 3, 13, 28),
                                new ChaosDoor(new int[]{ 4,5, }, 2, 16, 44),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_G,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 19, 21),
                                new ChaosDoor(new int[]{ 2,3, }, 1, 31, 37),
                                new ChaosDoor(new int[]{ 4,5, }, 2, 17, 39),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_K,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 7, 15),
                                new ChaosDoor(new int[]{ 2,3, }, 0, 23, 21),
                                new ChaosDoor(new int[]{ 4,5, }, 2, 16, 29),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_PURELANDS_T,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 34, 18),
                                new ChaosDoor(new int[]{ 2,3, }, 3, 12, 19),
                                new ChaosDoor(new int[]{ 4,5, }, 3, 15, 33),
                }.ToList()
                , false, pureLandPalettes, false, new int[] { }, true, new int[] { }));

            // gaia's navel
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_GAIASNAVEL_INTERIOR_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 8,9, }, 0, 9, 67),
                                new ChaosDoor(new int[]{ 10,11, }, 0, 27, 77),
                                new ChaosDoor(new int[]{ 12,13, }, 0, 24, 85),
                }.ToList()
                , false, cavePalettes, true, new int[] { }, false, new int[] { }));

            // palace
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_FIREPALACE_H,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0,1, }, 2, 26, 9),
                    new ChaosDoor(new int[]{ 2,3, }, 2, 7, 12),
                    new ChaosDoor(new int[]{ 6,7, }, 2, 11, 20),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));

            // ice palace
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_ICECASTLE_INTERIOR_C,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 0, 7, 9),
                                new ChaosDoor(new int[]{ 1, }, 0, 14, 9),
                                new ChaosDoor(new int[]{ 2,3, }, 2, 11, 18),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, false, new int[] { }));

            // palace 3
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_SHADEPALACE_INTERIOR_C,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0,1, }, 0, 20, 8),
                    new ChaosDoor(new int[]{ 2,3 }, 0, 32, 8),
                    new ChaosDoor(new int[]{ 12,13 }, 2, 32, 24),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));

            // castle
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTC_INTERIOR_G,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1, }, 0, 13, 12),
                                new ChaosDoor(new int[]{ 2,3, }, 0, 18, 12),
                                new ChaosDoor(new int[]{ 4,5, }, 2, 16, 28),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { 0, 1 }, true, new int[] { }));

            // ruins F
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_INTERIOR_B,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 11, }, 0, 15, 77),
                                new ChaosDoor(new int[]{ 12, }, 0, 37, 77),
                                new ChaosDoor(new int[]{ 13, }, 0, 59, 77),
                }.ToList()
                , true, ruinsWithCloudsPalettes, false, new int[] { }, false, new int[] { }));

            // palace
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_GRANDPALACE_INTERIOR_A,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0,1, }, 0, 25, 33),
                    new ChaosDoor(new int[]{ 2,3, }, 0, 25, 40),
                    new ChaosDoor(new int[]{ 4,5, }, 2, 25, 58),
                }.ToList()
                , false, palacePalettes, true, new int[] { }, false, new int[] { }));

            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_GRANDPALACE_INTERIOR_B,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0,1, }, 0, 10, 18),
                    new ChaosDoor(new int[]{ 2,3, }, 0, 31, 18),
                    new ChaosDoor(new int[]{ 4,5, }, 2, 21, 25),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, false, new int[] { }));

            // ///////////////////////////////////////////////
            // four-exit maps
            // ///////////////////////////////////////////////

            // gaia's navel exterior
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_GAIASNAVEL_EXTERIOR,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 21, 21),
                                new ChaosDoor(new int[]{ 3,4 }, 0, 46, 46),
                                new ChaosDoor(new int[]{ 5,6,}, 0, 51, 48),
                                new ChaosDoor(new int[]{ 7,8 }, 2, 27, 84),
                }.ToList()
                , false, forestPalettes, true, new int[] { }, false, new int[] { 2 }));

            // upper land
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_UPPERLAND_SUMMER,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2,3 }, 0, 31, 19),
                                new ChaosDoor(new int[]{ 4,5,6 }, 0, 11, 20),
                                new ChaosDoor(new int[]{ 7,9 }, 3, 8, 41),
                                new ChaosDoor(new int[]{ 8,10 }, 1, 41, 41),
                }.ToList()
                , false, upperLandPalettes, true, new int[] { 11 }, true, new int[] { }));

            // desert
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_DESERT_F,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 0, 23, 14),
                                new ChaosDoor(new int[]{ 3,4,5 }, 3, 7, 26),
                                new ChaosDoor(new int[]{ 6,7,8 }, 1, 40, 29),
                                new ChaosDoor(new int[]{ 9,10,11 }, 2, 21, 40),
                }.ToList()
                , false, desertPalettes, true, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_DESERT_F,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 0, 23, 14),
                                new ChaosDoor(new int[]{ 3,4,5 }, 3, 7, 26),
                                new ChaosDoor(new int[]{ 6,7,8 }, 1, 40, 29),
                                new ChaosDoor(new int[]{ 9,10,11 }, 2, 21, 40),
                }.ToList()
                , false, desertPalettes, true, new int[] { }, true, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_DESERT_I,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1,2 }, 0, 23, 14),
                                new ChaosDoor(new int[]{ 3,4,5 }, 3, 7, 26),
                                new ChaosDoor(new int[]{ 6,7,8 }, 1, 40, 29),
                                new ChaosDoor(new int[]{ 9,10,11 }, 2, 21, 40),
                }.ToList()
                , false, desertPalettes, true, new int[] { }, true, new int[] { }));

            // witch castle interior
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_WITCHCASTLE_INTERIOR_E,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0, }, 0, 9, 11),
                                new ChaosDoor(new int[]{ 1, }, 0, 16, 11),
                                new ChaosDoor(new int[]{ 2, }, 0, 23, 11),
                                new ChaosDoor(new int[]{ 3, }, 2, 16, 25),
                }.ToList()
                , false, ruinsPalettes, false, new int[] { }, true, new int[] { }));

            // palace
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_FIREPALACE_D,
                new ChaosDoor[] {
                    new ChaosDoor(new int[]{ 0,1, }, 0, 17, 25),
                    new ChaosDoor(new int[]{ 2,3, }, 2, 29, 24),
                    new ChaosDoor(new int[]{ 4,5, }, 2, 6, 25),
                    new ChaosDoor(new int[]{ 6,7, }, 2, 17, 27),
                }.ToList()
                , false, palacePalettes, false, new int[] { }, true, new int[] { }));

            // palace sewer shit
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_EMPIRE_SEWERS,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 4,5 }, 2, 16, 13),
                                new ChaosDoor(new int[]{ 8,9 }, 0, 9, 24),
                }.ToList()
                , false, palacePalettes, true, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_EMPIRE_SEWERS,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 0,1 }, 0, 34, 11),
                                new ChaosDoor(new int[]{ 6,7 }, 0, 44, 20),
                }.ToList()
                , false, palacePalettes, true, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_EMPIRE_SEWERS,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 2,3 }, 0, 55, 11),
                                new ChaosDoor(new int[]{ 10,11 }, 2, 58, 31),
                }.ToList()
                , false, palacePalettes, true, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_EMPIRE_SEWERS,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 14,15 }, 0, 9, 44),
                                new ChaosDoor(new int[]{ 18,19 }, 2, 8, 64),
                }.ToList()
                , false, palacePalettes, true, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_EMPIRE_SEWERS,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 12,13 }, 0, 30, 43),
                                new ChaosDoor(new int[]{ 20,21 }, 2, 38, 65),
                }.ToList()
                , false, palacePalettes, true, new int[] { }, false, new int[] { }));
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_EMPIRE_SEWERS,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 16,17 }, 2, 61, 48),
                                new ChaosDoor(new int[]{ 22,23 }, 2, 57, 65),
                }.ToList()
                , false, palacePalettes, true, new int[] { }, false, new int[] { }));

            // northtown ruins outside
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_NTR_ENTRANCE,
                new ChaosDoor[] {
                                new ChaosDoor(new int[]{ 4,5 }, 0, 22, 33),
                                new ChaosDoor(new int[]{ 6,7,8,9 }, 2, 22, 77),
                }.ToList()
                , false, ruinsExteriorPalettes, true, new int[] { }, false, new int[] { }));


            // water palace outside
            chaosMaps.Add(new ChaosVanillaMap(MAPNUM_WATERPALACE_EXTERIOR,
                new ChaosDoor[] {
                                            new ChaosDoor(new int[]{ 0,3 }, 1, 50, 27),
                                            new ChaosDoor(new int[]{ 1,2 }, 0, 23, 29),
                                            new ChaosDoor(new int[]{ 4,5 }, 2, 24, 54),
                }.ToList()
                , false, new byte[] { }, true, new int[] { 0 }, false, new int[] { }));
            
            foreach (ChaosVanillaMap map in chaosMaps)
            {
                int exitNum = map.doors.Count;
                if (!chaosMapsByExitNum.ContainsKey(exitNum))
                {
                    chaosMapsByExitNum[exitNum] = new List<ChaosVanillaMap>();
                }
                chaosMapsByExitNum[exitNum].Add(map);

                if (map.townMap)
                {
                    if (!townMapsByExitNum.ContainsKey(exitNum))
                    {
                        townMapsByExitNum[exitNum] = new List<ChaosVanillaMap>();
                    }
                    townMapsByExitNum[exitNum].Add(map);
                }
            }
            
            bossMaps[0x57] = new ChaosBossMap(MAPNUM_MANTISANT_ARENA, 17, 19, false, new byte[] { 0 }, new byte[] { }); // mantis ant
            bossMaps[0x58] = new ChaosBossMap(MAPNUM_WALLFACE_ARENA, 13, 19, false, new byte[] { 0 }, new byte[] { }); // wall face
            bossMaps[0x59] = new ChaosBossMap(MAPNUM_TROPICALLO_ARENA, 25, 42, false, new byte[] { 0 }, new byte[] { 1,2 }); // tropicallo
            bossMaps[0x5A] = new ChaosBossMap(MAPNUM_MINOTAUR_ARENA, 16, 24, false, new byte[] { 0 }, new byte[] { }); // minotaur
            bossMaps[0x5B] = new ChaosBossMap(MAPNUM_SPIKEY_ARENA, 13, 23, false, new byte[] { 0 }, new byte[] { }); // spikey
            bossMaps[0x5C] = new ChaosBossMap(MAPNUM_JABBERWOCKY_ARENA, 16, 46, false, new byte[] { 0 }, new byte[] { 1,2 }); // jabberwocky
            bossMaps[0x5D] = new ChaosBossMap(MAPNUM_SPRINGBEAK_ARENA, 18, 22, false, new byte[] { 0 }, new byte[] { }); // spring beak
            bossMaps[0x5E] = new ChaosBossMap(MAPNUM_FROSTGIGAS_ARENA, 9, 19, false, new byte[] { 0 }, new byte[] { }); // frost gigas
            bossMaps[0x5F] = new ChaosBossMap(MAPNUM_SNAPDRAGON_ARENA, 32, 28, false, new byte[] { 0 }, new byte[] { 1,2,3,4,5,6 }); // snap dragon
            bossMaps[0x60] = new ChaosBossMap(MAPNUM_MECHRIDER1_ARENA, 19, 15, false, new byte[] { 0 }, new byte[] { }); // mech rider 1
            bossMaps[0x61] = new ChaosBossMap(MAPNUM_DOOMSWALL_ARENA, 13, 19, false, new byte[] { 0 }, new byte[] { }); // doom's wall
            bossMaps[0x62] = new ChaosBossMap(MAPNUM_VAMPIRE_ARENA, 14, 26, true, new byte[] { 2 }, new byte[] { 0, 1 }); // vampire
            bossMaps[0x63] = new ChaosBossMap(MAPNUM_METALMANTIS_ARENA, 20, 12, false, new byte[] { 0 }, new byte[] { }); // metal mantis
            bossMaps[0x64] = new ChaosBossMap(MAPNUM_MECHRIDER2_ARENA, 9, 19, false, new byte[] { 0 }, new byte[] { 1 }); // mech rider 2
            bossMaps[0x65] = new ChaosBossMap(MAPNUM_KILROY_ARENA, 19, 26, false, new byte[] { 0 }, new byte[] { }); // kilroy - pieces don't parse in editor, but seems to work fine here
            bossMaps[0x66] = new ChaosBossMap(MAPNUM_GORGONBULL_ARENA, 15, 25, false, new byte[] { 0 }, new byte[] { }); // gorgon bull
            bossMaps[0x68] = new ChaosBossMap(MAPNUM_BOREALFACE_ARENA, 11, 10, false, new byte[] { 0 }, new byte[] { }); // boreal face
            bossMaps[0x69] = new ChaosBossMap(MAPNUM_GREATVIPER_ARENA, 22, 23, true, new byte[] { 0 }, new byte[] { }); // great viper
            bossMaps[0x6A] = new ChaosBossMap(MAPNUM_LIMESLIME_ARENA, 11, 16, false, new byte[] { 0 }, new byte[] { }); // lime slime
            bossMaps[0x6D] = new ChaosBossMap(MAPNUM_HYDRA_ARENA, 15, 27, true, new byte[] { 3 }, new byte[] { 0,1,2 }); // hydra
            bossMaps[0x6E] = new ChaosBossMap(MAPNUM_WATERMELON_ARENA, 9, 16, false, new byte[] { 0 }, new byte[] { }); // aehggirpasopklfn
            bossMaps[0x6F] = new ChaosBossMap(MAPNUM_HEXAS_ARENA, 11, 24, false, new byte[] { 2 }, new byte[] {0,1}); // hexas
            bossMaps[0x70] = new ChaosBossMap(MAPNUM_KETTLEKIN_ARENA, 11, 15, false, new byte[] { 3 }, new byte[] { 0, 1, 2 }); // kettlekin
            bossMaps[0x71] = new ChaosBossMap(MAPNUM_TONPOLE_ARENA, 16, 17, false, new byte[] { 0 }, new byte[] { }); // tonpole
            bossMaps[0x72] = new ChaosBossMap(MAPNUM_MECHRIDER3_ARENA, 6, 11, false, new byte[] { 0 }, new byte[] { }); // mech rider 3
            bossMaps[0x73] = new ChaosBossMap(MAPNUM_SNOWDRAGON_ARENA, 12, 23, false, new byte[] { 0 }, new byte[] { }); // snow dragon
            bossMaps[0x74] = new ChaosBossMap(MAPNUM_FIREGIGAS_ARENA, 16, 25, false, new byte[] { 0 }, new byte[] { }); // fire gigas
            bossMaps[0x75] = new ChaosBossMap(MAPNUM_REDDRAGON_ARENA, 20, 23, false, new byte[] { 0 }, new byte[] { }); // red dragon
            bossMaps[0x76] = new ChaosBossMap(MAPNUM_AXEBEAK_ARENA, 19, 29, false, new byte[] { 0 }, new byte[] { }); // axe beak
            bossMaps[0x77] = new ChaosBossMap(MAPNUM_BLUEDRAGON_ARENA, 13, 20, false, new byte[] { 0 }, new byte[] { }); // blue dragon
            bossMaps[0x78] = new ChaosBossMap(MAPNUM_BUFFY_ARENA, 17, 47, false, new byte[] { 0 }, new byte[] { }); // buffy
            bossMaps[0x79] = new ChaosBossMap(MAPNUM_DARKLICH_ARENA_B, 18, 29, false, new byte[] { 2 }, new byte[] { 0,1 }); // lich
            bossMaps[0x7B] = new ChaosBossMap(MAPNUM_DRAGONWORM_ARENA, 18, 27, false, new byte[] { 0 }, new byte[] { }); // dragon worm
            bossMaps[0x7C] = new ChaosBossMap(MAPNUM_DREADSLIME_ARENA, 11, 16, false, new byte[] { 0 }, new byte[] { }); // dread slime
            bossMaps[0x7D] = new ChaosBossMap(MAPNUM_THUNDERGIGAS_ARENA, 24, 40, false, new byte[] { 0 }, new byte[] { }); // thunder gigas
        }
    }
}
