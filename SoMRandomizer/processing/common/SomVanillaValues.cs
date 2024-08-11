using System;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// A collection of vanilla mana values used by various hacks.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SomVanillaValues
    {
        // --------------------------------------
        // object ids for bosses
        // --------------------------------------
        public const byte BOSSID_MANTISANT = 87; // 0x57
        public const byte BOSSID_WALLFACE = 88; // 0x58 - requires special map due to layer2 rendering
        public const byte BOSSID_TROPICALLO = 89; // 0x59
        public const byte BOSSID_MINOTAUR = 90; // 0x5a
        public const byte BOSSID_SPIKEY = 91; // 0x5b
        public const byte BOSSID_JABBER = 92; // 0x5c
        public const byte BOSSID_SPRINGBEAK = 93; // 0x5d
        public const byte BOSSID_FROSTGIGAS = 94; // 0x5e
        public const byte BOSSID_SNAPDRAGON = 95; // 0x5f
        public const byte BOSSID_MECHRIDER1 = 96; // 0x60
        public const byte BOSSID_DOOMSWALL = 97; // 0x61 - requires special map due to layer2 rendering
        public const byte BOSSID_VAMPIRE = 98; // 0x62
        public const byte BOSSID_METALMANTIS = 99; // 0x63
        public const byte BOSSID_MECHRIDER2 = 100; // 0x64
        public const byte BOSSID_KILROY = 101; // 0x65
        public const byte BOSSID_GORGON = 102; // 0x66
        public const byte BOSSID_BRAMBLER = 103; // 0x67 - don't use directly; spawned by tropicallo
        public const byte BOSSID_BOREAL = 104; // 0x68
        public const byte BOSSID_GREATVIPER = 105; // 0x69
        public const byte BOSSID_LIMESLIME = 106; // 0x6a - requires special map settings because of mode7 rendering
        public const byte BOSSID_BLUESPIKE = 107; // 0x6b
        public const byte BOSSID_CHAMBERSEYE = 108; // 0x6c - don't use directly; spawned by wall face
        public const byte BOSSID_HYDRA = 109; // 0x6d
        public const byte BOSSID_WATERMELON = 110; // 0x6e - requires special map due to layer2 rendering
        public const byte BOSSID_HEXAS = 111; // 0x6f
        public const byte BOSSID_KETTLEKIN = 112; // 0x70
        public const byte BOSSID_TONPOLE = 113; // 0x71
        public const byte BOSSID_MECHRIDER3 = 114; // 0x72
        public const byte BOSSID_SNOWDRAGON = 115; // 0x73 - requires special map due to layer2 rendering
        public const byte BOSSID_FIREGIGAS = 116; // 0x74
        public const byte BOSSID_REDDRAGON = 117; // 0x75 - requires special map due to layer2 rendering
        public const byte BOSSID_AXEBEAK = 118; // 0x76
        public const byte BOSSID_BLUEDRAGON = 119; // 0x77 - requires special map due to layer2 rendering
        public const byte BOSSID_BUFFY = 120; // 0x78
        public const byte BOSSID_DARKLICH = 121; // 0x79 - requires special map due to layer2 rendering
        public const byte BOSSID_BITINGLIZARD = 122; // 0x7a - tonpole transformed form - normally we don't use it directly but it theoretically works solo
        public const byte BOSSID_DRAGONWORM = 123; // 0x7b
        public const byte BOSSID_DREADSLIME = 124; // 0x7c - requires special map settings because of mode7 rendering
        public const byte BOSSID_THUNDERGIGAS = 125; // 0x7d
        public const byte BOSSID_DOOMSEYE = 126; // 0x7e - don't use directly; spawned by doom's wall
        public const byte BOSSID_MANABEAST = 127; // 0x7f - requires special map settings because of mode7 rendering

        // vanilla composite map numbers
        // nothing in 0 - 7
        public const int MAPNUM_DEATH_MAP = 8;
        // nothing in 9 - 15
        public const int MAPNUM_POTOS_SOUTH_FIELD = 16;
        public const int MAPNUM_JEHK_DUNGEON = 17;
        public const int MAPNUM_JEHK_LOBBY = 18;
        public const int MAPNUM_WATERPALACE_WITCHCASTLE_CROSSROADS = 19;
        public const int MAPNUM_EMPIRE_SEWERS = 20;
        public const int MAPNUM_NTR_ENTRANCE = 21;
        public const int MAPNUM_TURTLEISLAND_EXTERIOR = 22;
        public const int MAPNUM_PANDORA_WEST_FIELD = 23;
        public const int MAPNUM_TASNICA_ENTRANCE = 24;
        public const int MAPNUM_GAIASNAVEL_EXTERIOR = 25;
        public const int MAPNUM_TASNICA_EXTERIOR_B = 26;
        public const int MAPNUM_GAIASNAVEL_NORTH_PATH = 27;
        public const int MAPNUM_WITCHFOREST_A = 28;
        public const int MAPNUM_WITCHFOREST_B = 29;
        public const int MAPNUM_WITCHFOREST_C = 30;
        public const int MAPNUM_WITCHFOREST_D = 31;
        public const int MAPNUM_KARONFERRY_ENDPOINTS = 32;
        public const int MAPNUM_KARONFERRY_TRAVELING_UP = 33;
        public const int MAPNUM_KARONFERRY_TRAVELING_DOWN = 34;
        public const int MAPNUM_LUNAPALACE_SPACE = 35;
        public const int MAPNUM_TASNICA_INTERIOR_A = 36;
        public const int MAPNUM_TASNICA_INTERIOR_B = 37;
        public const int MAPNUM_GIRL_VS_WEREWOLF_ARENA = 38;
        public const int MAPNUM_UPPERLAND_NORTHWEST = 39;
        public const int MAPNUM_UPPERLAND_SOUTHWEST = 40;
        public const int MAPNUM_UPPERLAND_SOUTHEAST = 41;
        public const int MAPNUM_UNDERSEA_SUBWAY_AREA_A = 42;
        public const int MAPNUM_UNDERSEA_SUBWAY_AREA_B = 43;
        public const int MAPNUM_UNDERSEA_SUBWAY_A = 44;
        public const int MAPNUM_UNDERSEA_SUBWAY_B = 45;
        public const int MAPNUM_KETTLEKIN_ARENA = 46;
        public const int MAPNUM_WINDPALACE_EXTERIOR = 47;
        public const int MAPNUM_LUNAPALACE_NORMAL = 48;
        public const int MAPNUM_UPPERLAND_WINTER = 49;
        // nothing in 50
        public const int MAPNUM_TASNICA_THRONE_ROOM_NPC = 51;
        public const int MAPNUM_UPPERLAND_SPRING = 52;
        public const int MAPNUM_UPPERLAND_SUMMER = 53;
        public const int MAPNUM_UPPERLAND_FALL = 54;
        public const int MAPNUM_UPPERLAND_WINTER_WITH_MOOGLES = 55;
        public const int MAPNUM_UPPERLAND_SPRITE_VILLAGE = 56;
        public const int MAPNUM_UPPERLAND_SEASONS_CANNON = 57;
        public const int MAPNUM_UPPERLAND_MOOGLE_VILLAGE_WITH_PEBBLERS = 58;
        public const int MAPNUM_UPPERLAND_MOOGLE_VILLAGE = 59;
        public const int MAPNUM_SPRINGBEAK_ARENA = 60;
        public const int MAPNUM_SHADEPALACE_EXTERIOR = 61;
        public const int MAPNUM_JEHKCAVE_EXTERIOR = 62;
        public const int MAPNUM_MANDALA_SOUTH = 63;
        public const int MAPNUM_MANDALA_NORTH = 64;
        public const int MAPNUM_MANDALA_ORB_ROOM = 65;
        public const int MAPNUM_MANDALA_ORB_HALLWAYS = 66;
        public const int MAPNUM_DESERT_A = 67;
        public const int MAPNUM_DESERT_B = 68;
        public const int MAPNUM_DESERT_C = 69;
        public const int MAPNUM_DESERT_D = 70;
        // same as 43, but only the small interior area with krissie and watts
        public const int MAPNUM_UNDERSEA_SUBWAY_B_WITH_WATTS = 71;
        public const int MAPNUM_DOPPEL_ARENA = 72;
        public const int MAPNUM_GAIASNAVEL_INTERIOR_A_NEKO = 73;
        // nothing in 74
        public const int MAPNUM_ICECASTLE_EXTERIOR = 75;
        public const int MAPNUM_ICECOUNTRY_NORTHWEST = 76;
        public const int MAPNUM_ICECOUNTRY_SOUTHWEST = 77;
        public const int MAPNUM_ICECOUNTRY_NEKO = 78;
        public const int MAPNUM_BOREALFACE_ARENA = 79;
        public const int MAPNUM_ICECOUNTRY_NORTHEAST = 80;
        public const int MAPNUM_ICECOUNTRY_SOUTHEAST = 81;
        public const int MAPNUM_NTC_ENTRANCE = 82;
        public const int MAPNUM_PATH_SOUTH_OF_ICE_CASTLE = 83;
        public const int MAPNUM_DESERT_E = 84;
        public const int MAPNUM_DESERT_F = 85;
        public const int MAPNUM_DESERT_G = 86;
        public const int MAPNUM_DESERT_H = 87;
        public const int MAPNUM_DESERT_I = 88;
        public const int MAPNUM_KAKKARA = 89;
        public const int MAPNUM_FIREPALACE_ENTRANCE = 90;
        public const int MAPNUM_DESERT_NPC_TEMPORARY_ROOM = 91;
        public const int MAPNUM_DESERT_J = 92;
        public const int MAPNUM_DESERT_K = 93;
        public const int MAPNUM_DESERT_L = 94;
        public const int MAPNUM_DESERT_STARS_A = 95;
        public const int MAPNUM_DESERT_STARS_B = 96;
        public const int MAPNUM_DESERT_STARS_C = 97;
        // these seem to be exact copies of each other?
        public const int MAPNUM_SANDSHIP_EXTERIOR_A = 98;
        public const int MAPNUM_SANDSHIP_EXTERIOR_B = 99;
        public const int MAPNUM_MECHRIDER2_ARENA = 100;
        // trimmed-down versions of other maps used to show credits scenes
        public const int MAPNUM_CREDITS_WATERFALL = 101;
        public const int MAPNUM_CREDITS_POTOS = 102;
        public const int MAPNUM_CREDITS_WATERPALACE = 103;
        public const int MAPNUM_CREDITS_PANDORA = 104;
        public const int MAPNUM_CREDITS_CANNONTRAVEL = 105;
        public const int MAPNUM_CREDITS_UPPERLANDS = 106;
        public const int MAPNUM_CREDITS_SPRITEVILLAGE = 107;
        public const int MAPNUM_CREDITS_SANDSHIP = 108;
        public const int MAPNUM_UNDINECAVE_EXTERIOR = 109;
        public const int MAPNUM_LIGHTHOUSE_ROOF = 110;
        public const int MAPNUM_BLUESPIKE_ARENA = 111;
        public const int MAPNUM_LIGHTHOUSE_BASE = 112;
        public const int MAPNUM_GOLDCITY = 113;
        public const int MAPNUM_SOUTHTOWN = 114;
        public const int MAPNUM_NORTHTOWN = 115;
        public const int MAPNUM_LUMINATOWER_EXTERIOR = 116;
        public const int MAPNUM_LUMINATOWER_1F = 117;
        public const int MAPNUM_LUMINATOWER_2F = 118;
        public const int MAPNUM_GORGONBULL_ARENA = 119;
        public const int MAPNUM_LIGHTSEED = 120;
        public const int MAPNUM_TASNICA_THRONE_ROOM_ENEMY = 121;
        public const int MAPNUM_NORTHTOWN_INTERIOR_A = 122;
        public const int MAPNUM_NORTHTOWN_INTERIOR_B = 123;
        public const int MAPNUM_SOUTHTOWN_INTERIOR_A = 124;
        public const int MAPNUM_GOLDCITY_INTERIOR = 125;
        public const int MAPNUM_NORTHTOWN_INTERIOR_C = 126;
        public const int MAPNUM_SOUTHTOWN_INTERIOR_B = 127;
        public const int MAPNUM_RUSTYSWORD = 128;
        public const int MAPNUM_RABITEFIELD_A = 129;
        public const int MAPNUM_RABITEFIELD_B = 130;
        public const int MAPNUM_POTOS = 131;
        public const int MAPNUM_NEKOSHOUSE_EXTERIOR = 132;
        public const int MAPNUM_GOBLIN_CUTSCENE = 133;
        public const int MAPNUM_FLAMMIE_IN_MATANGO_CAVE = 134;
        public const int MAPNUM_WATERPALACE_BASEMENT = 135;
        public const int MAPNUM_WATERPALACE_EXTERIOR = 136;
        public const int MAPNUM_WATERPALACE_HALLWAY_NPC = 137;
        // same map as 123?  not sure why it's duplicated
        public const int MAPNUM_NORTHTOWN_INTERIOR_B2 = 138;
        public const int MAPNUM_WATERPALACE_LUKA_DIALOGUE = 139;
        public const int MAPNUM_WATERPALACE_HALLWAY_ENEMY = 140;
        // nothing in 141
        public const int MAPNUM_WATERPALACE_EMPIRE_NPCS = 142;
        public const int MAPNUM_JABBERWOCKY_ARENA = 143;
        public const int MAPNUM_PANDORA = 144;
        public const int MAPNUM_PANDORA_CASTLE_EXTERIOR = 145;
        // more credits-only maps
        public const int MAPNUM_CREDITS_MATANGO = 146;
        public const int MAPNUM_CREDITS_SANTAHOUSE = 147;
        public const int MAPNUM_CREDITS_NORTHTOWN = 148;
        public const int MAPNUM_CREDITS_TASNICA = 149;
        public const int MAPNUM_CREDITS_JEHK = 150;
        public const int MAPNUM_CREDITS_WITCHCASTLE = 151;
        public const int MAPNUM_CREDITS_GAIASNAVEL = 152;
        public const int MAPNUM_CREDITS_NEKO = 153;
        // nothing in 154
        public const int MAPNUM_SOUTHTOWN_CANNON = 155;
        public const int MAPNUM_ICECOUNTRY_CANNON_SOUTH = 156;
        public const int MAPNUM_KAKKARA_CANNON = 157;
        public const int MAPNUM_UPPERLAND_NORTH_CANNON = 158;
        public const int MAPNUM_POTOS_CANNON = 159;
        public const int MAPNUM_PANDORA_RUINS_EXTERIOR = 160;
        public const int MAPNUM_PANDORA_RUINS_A = 161;
        public const int MAPNUM_PANDORA_RUINS_B = 162;
        public const int MAPNUM_PANDORA_RUINS_C = 163;
        public const int MAPNUM_PANDORA_RUINS_D = 164;
        public const int MAPNUM_PANDORA_RUINS_ROOMS = 165;
        public const int MAPNUM_WALLFACE_ARENA = 166;
        public const int MAPNUM_PANDORA_RUINS_D_AFTER_WALLFACE = 167;
        public const int MAPNUM_KIPPO = 168;
        // nothing in 169
        public const int MAPNUM_PANDORA_SOUTH = 170;
        public const int MAPNUM_GAIASNAVEL_CANNON = 171;
        // nothing in 172 - 175
        public const int MAPNUM_INTRO_MANAFORT = 176;
        public const int MAPNUM_INTRO_MECHRIDER3 = 177;
        public const int MAPNUM_INTRO_PURELANDS = 178;
        public const int MAPNUM_INTRO_MANATREE = 179;
        public const int MAPNUM_CREDITS_POST_MANABEAST = 180;
        // nothing in 181 - 193
        public const int MAPNUM_ICECOUNTRY_PARADISETOWN_WARM = 194;
        public const int MAPNUM_SUNKENCONTINENT_EXTERIOR = 195;
        public const int MAPNUM_WITCHCASTLE_EXTERIOR = 196;
        public const int MAPNUM_ICECOUNTRY_PARADISETOWN_COLD = 197;
        public const int MAPNUM_PURELANDS_A_AND_MANA_TREE = 198;
        public const int MAPNUM_PURELANDS_B = 199;
        public const int MAPNUM_PURELANDS_C = 200;
        public const int MAPNUM_PURELANDS_D = 201;
        public const int MAPNUM_PURELANDS_E = 202;
        public const int MAPNUM_PURELANDS_F = 203;
        public const int MAPNUM_DRAGONWORM_ARENA = 204;
        public const int MAPNUM_PURELANDS_G = 205;
        public const int MAPNUM_PURELANDS_H = 206;
        public const int MAPNUM_PURELANDS_I = 207;
        public const int MAPNUM_PURELANDS_J = 208;
        public const int MAPNUM_PURELANDS_K = 209;
        public const int MAPNUM_PURELANDS_L = 210;
        public const int MAPNUM_PURELANDS_M = 211;
        public const int MAPNUM_PURELANDS_N = 212;
        public const int MAPNUM_AXEBEAK_ARENA = 213;
        public const int MAPNUM_PURELANDS_O = 214;
        public const int MAPNUM_PURELANDS_P = 215;
        public const int MAPNUM_PURELANDS_Q = 216;
        public const int MAPNUM_PURELANDS_R = 217;
        public const int MAPNUM_PURELANDS_S = 218;
        public const int MAPNUM_PURELANDS_T = 219;
        public const int MAPNUM_PURELANDS_U = 220;
        public const int MAPNUM_PURELANDS_V = 221;
        public const int MAPNUM_MATANGO_BACKYARD = 222;
        public const int MAPNUM_MATANGO_FRONTYARD = 223;
        public const int MAPNUM_MATANGO = 224;
        public const int MAPNUM_PURELANDS_W = 225;
        public const int MAPNUM_THUNDERGIGAS_ARENA = 226;
        public const int MAPNUM_PURELANDS_X = 227;
        public const int MAPNUM_TODO = 228;
        public const int MAPNUM_PURELANDS_Y = 229;
        public const int MAPNUM_SNOWDRAGON_ARENA = 230;
        public const int MAPNUM_REDDRAGON_ARENA = 231;
        public const int MAPNUM_BLUEDRAGON_ARENA = 232;
        public const int MAPNUM_PURELANDS_GATE_A = 233;
        public const int MAPNUM_PURELANDS_GATE_B = 234;
        public const int MAPNUM_PURELANDS_GATE_C = 235;
        public const int MAPNUM_MANATREE_CUTSCENE = 236;
        public const int MAPNUM_MECHRIDER3_ARENA = 237;
        public const int MAPNUM_WATERMELON_ARENA = 238;
        // nothing in 239
        public const int MAPNUM_FIREPALACE_UNDINE_ORB = 240;
        public const int MAPNUM_FIREPALACE_EMPTY_ROOM = 241;
        // nothing in 242 - 243
        public const int MAPNUM_BUFFY_ARENA = 244;
        // two identical arenas, both with the thanatos/dyluck npcs and dark lich.  not sure why
        // the _B one seems to be the combat one, i'm guessing the other is the dialogue one
        public const int MAPNUM_DARKLICH_ARENA = 245;
        public const int MAPNUM_DARKLICH_ARENA_B = 246;
        // nothing in 247
        public const int MAPNUM_MANAFORT_INTERIOR_A = 248;
        public const int MAPNUM_MANAFORT_INTERIOR_B = 249;
        public const int MAPNUM_MANAFORT_INTERIOR_C = 250;
        public const int MAPNUM_MANAFORT_INTERIOR_PATH_TO_LICH = 251;
        // nothing in 252
        public const int MAPNUM_MANABEAST_ARENA = 253;
        // nothing in 254
        public const int MAPNUM_MANAFORT_EXTERIOR = 255;
        public const int MAPNUM_INTRO_LOG = 256;
        // a couple copies of this for some reason
        public const int MAPNUM_POTOS_INTERIOR_A = 257;
        public const int MAPNUM_POTOS_INTERIOR_B = 258;
        public const int MAPNUM_POTOS_INN = 259;
        public const int MAPNUM_MANTISANT_ARENA = 260;
        public const int MAPNUM_PANDORA_INTERIOR_A = 261;
        public const int MAPNUM_PANDORA_INN = 262;
        public const int MAPNUM_PANDORA_INTERIOR_B = 263;
        public const int MAPNUM_PANDORA_CASTLE_INTERIOR_A = 264;
        public const int MAPNUM_PANDORA_CASTLE_INTERIOR_B = 265;
        public const int MAPNUM_PANDORA_CASTLE_INTERIOR_C = 266;
        public const int MAPNUM_PANDORA_CASTLE_INTERIOR_D = 267;
        public const int MAPNUM_PANDORA_TREASURE_ROOM = 268;
        public const int MAPNUM_EARTHPALACE_TRANSITIONS = 269;
        public const int MAPNUM_SANDSHIP_INTERIOR_A = 270;
        public const int MAPNUM_SANDSHIP_INTERIOR_B = 271;
        // nothing in 272
        public const int MAPNUM_WINDSEED = 273;
        public const int MAPNUM_MATANGO_INN = 274;
        public const int MAPNUM_UNDINECAVE_FISH_ROOM = 275;
        public const int MAPNUM_EARTHSEED = 276;
        public const int MAPNUM_TONPOLE_ARENA = 277;
        public const int MAPNUM_UNDINE = 278;
        public const int MAPNUM_NEKOHOUSE_INTERIOR = 279;
        public const int MAPNUM_KIPPO_INTERIOR = 280;
        public const int MAPNUM_KIPPO_INN = 281;
        public const int MAPNUM_MATANGO_SHOP = 282;
        // not sure why a duplicate map
        public const int MAPNUM_PANDORA_CASTLE_INTERIOR_B2 = 283;
        public const int MAPNUM_GAIASNAVEL_INTERIOR_A_ENEMY = 284;
        public const int MAPNUM_GAIASNAVEL_INTERIOR_B = 285;
        public const int MAPNUM_GAIASNAVEL_INTERIOR_C = 286;
        public const int MAPNUM_DWARFTOWN = 287;
        public const int MAPNUM_WATTS_STAIRS = 288;
        public const int MAPNUM_MAGICROPE = 289;
        public const int MAPNUM_TROPICALLO_ARENA = 290;
        public const int MAPNUM_EARTHPALACE_ORB = 291;
        public const int MAPNUM_DWARFTOWN_INN = 292;
        public const int MAPNUM_DWARFTOWN_SHOP = 293;
        public const int MAPNUM_WATTS = 294;
        public const int MAPNUM_PATH_TO_RABITEMAN = 295;
        public const int MAPNUM_RABITEMAN_BACKSTAGE = 296;
        public const int MAPNUM_RABITEMAN = 297;
        public const int MAPNUM_TODO_INTERIOR = 298;
        public const int MAPNUM_TODO_INN = 299;
        public const int MAPNUM_WITCHCASTLE_INTERIOR_A = 300;
        public const int MAPNUM_WITCHCASTLE_INTERIOR_B = 301;
        public const int MAPNUM_WITCHCASTLE_INTERIOR_C = 302;
        public const int MAPNUM_WITCHCASTLE_INTERIOR_D = 303;
        public const int MAPNUM_WITCHCASTLE_INTERIOR_E = 304;
        public const int MAPNUM_WITCHCASTLE_INTERIOR_F = 305;
        public const int MAPNUM_WITCHCASTLE_NESOBERI = 306;
        public const int MAPNUM_MATANGOCAVE_GNOME_ORB = 307;
        public const int MAPNUM_WITCHCASTLE_JAIL = 308;
        public const int MAPNUM_SPIKEY_WITH_NPCS = 309;
        public const int MAPNUM_SPIKEY_ARENA = 310;
        public const int MAPNUM_KILROYSHIP_UPPER = 311;
        public const int MAPNUM_EARTHPALACE_INTERIOR_A = 312;
        public const int MAPNUM_EARTHPALACE_INTERIOR_B = 313;
        public const int MAPNUM_EARTHPALACE_INTERIOR_C = 314;
        public const int MAPNUM_KILROYSHIP_LOWER = 315;
        public const int MAPNUM_KILROYSHIP_INTERIOR = 316;
        public const int MAPNUM_KILROY_ARENA = 317;
        public const int MAPNUM_SANDSHIP_INTERIOR_C = 318;
        public const int MAPNUM_SANDSHIP_INTERIOR_D = 319;
        public const int MAPNUM_MATANGOCAVE_A = 320;
        public const int MAPNUM_MATANGOCAVE_B = 321;
        public const int MAPNUM_MATANGOCAVE_C = 322;
        public const int MAPNUM_MATANGOCAVE_D = 323;
        public const int MAPNUM_SANDSHIP_INTERIOR_E = 324;
        public const int MAPNUM_GREATVIPER_ARENA = 325;
        public const int MAPNUM_FIREPALACE_A = 326;
        public const int MAPNUM_FIREPALACE_B = 327;
        public const int MAPNUM_FIREPALACE_C = 328;
        public const int MAPNUM_FIREPALACE_D = 329;
        public const int MAPNUM_FIREPALACE_E = 330;
        public const int MAPNUM_FIREPALACE_F = 331;
        public const int MAPNUM_SANTAHOUSE_EXTERIOR = 332;
        public const int MAPNUM_SANTAHOUSE_INTERIOR = 333;
        // nothing in 334
        public const int MAPNUM_PARADISETOWN_INTERIOR = 335;
        public const int MAPNUM_MATANGO_INTERIOR_LOBBY = 336;
        public const int MAPNUM_MATANGO_INTERIOR_THRONE = 337;
        public const int MAPNUM_MATANGO_INTERIOR_CHEST = 338;
        public const int MAPNUM_SANDSHIP_INTERIOR_F = 339;
        public const int MAPNUM_MECHRIDER1_ARENA = 340;
        public const int MAPNUM_MANDALA_INN = 341;
        public const int MAPNUM_MANDALA_INTERIOR = 342;
        public const int MAPNUM_WITCHCASTLE_ROOM_A = 343;
        public const int MAPNUM_WITCHCASTLE_ROOM_B = 344;
        public const int MAPNUM_FIREPALACE_G = 345;
        public const int MAPNUM_FIREPALACE_H = 346;
        public const int MAPNUM_MINOTAUR_ARENA = 347;
        public const int MAPNUM_FIREPALACE_FIRST_ORB = 348;
        public const int MAPNUM_FIRESEED = 349;
        public const int MAPNUM_LUNAPALACE_STAIRS = 350;
        public const int MAPNUM_MOONSEED = 351;
        public const int MAPNUM_ICECASTLE_INTERIOR_A = 352;
        public const int MAPNUM_ICECASTLE_INTERIOR_B = 353;
        public const int MAPNUM_ICECASTLE_INTERIOR_C = 354;
        public const int MAPNUM_ICECASTLE_INTERIOR_D = 355;
        public const int MAPNUM_ICECASTLE_INTERIOR_E = 356;
        public const int MAPNUM_ICECASTLE_INTERIOR_F = 357;
        public const int MAPNUM_ICECASTLE_INTERIOR_G = 358;
        public const int MAPNUM_ICECASTLE_INTERIOR_H = 359;
        public const int MAPNUM_ICECASTLE_INTERIOR_I = 360;
        public const int MAPNUM_TRIPLETONPOLE_ARENA = 361;
        public const int MAPNUM_ICECASTLE_INTERIOR_J = 362;
        public const int MAPNUM_ICECASTLE_SANTA = 363;
        public const int MAPNUM_FROSTGIGAS_ARENA = 364;
        public const int MAPNUM_ICECASTLE_INTERIOR_K = 365;
        // nothing in 366
        public const int MAPNUM_SHADEPALACE_INTERIOR_A = 367;
        public const int MAPNUM_SHADEPALACE_INTERIOR_B = 368;
        public const int MAPNUM_SHADEPALACE_INTERIOR_C = 369;
        public const int MAPNUM_SHADEPALACE_INTERIOR_D = 370;
        public const int MAPNUM_SHADEPALACE_INTERIOR_E = 371;
        public const int MAPNUM_SHADEPALACE_INTERIOR_F = 372;
        public const int MAPNUM_SHADEPALACE_INTERIOR_G = 373;
        public const int MAPNUM_SHADEPALACE_INTERIOR_H = 374;
        public const int MAPNUM_DARKSEED = 375;
        public const int MAPNUM_KAKKARA_INN = 376;
        public const int MAPNUM_METALMANTIS_ARENA = 377;
        public const int MAPNUM_NTC_INTERIOR_A = 378;
        public const int MAPNUM_NTC_INTERIOR_B = 379;
        public const int MAPNUM_NTC_INTERIOR_C = 380;
        public const int MAPNUM_NTC_INTERIOR_D = 381;
        public const int MAPNUM_NTC_INTERIOR_E = 382;
        public const int MAPNUM_NTC_INTERIOR_F = 383;
        public const int MAPNUM_NTC_INTERIOR_G = 384;
        public const int MAPNUM_NTC_INTERIOR_H = 385;
        public const int MAPNUM_NTC_INTERIOR_I = 386;
        public const int MAPNUM_NTC_INTERIOR_J = 387;
        public const int MAPNUM_NTC_INTERIOR_K = 388;
        public const int MAPNUM_NTC_INTERIOR_L = 389;
        public const int MAPNUM_NTC_INTERIOR_M = 390;
        public const int MAPNUM_NTC_INTERIOR_N = 391;
        // nothing in 392
        public const int MAPNUM_LIGHTHOUSE_FLOOR_1 = 393;
        public const int MAPNUM_LIGHTHOUSE_FLOOR_2 = 394;
        public const int MAPNUM_LIGHTHOUSE_FLOOR_3 = 395;
        public const int MAPNUM_LIGHTHOUSE_FLOOR_4 = 396;
        // nothing in 397 - 399
        public const int MAPNUM_NTR_INTERIOR_A = 400;
        public const int MAPNUM_NTR_INTERIOR_B = 401;
        public const int MAPNUM_NTR_INTERIOR_C = 402;
        public const int MAPNUM_NTR_INTERIOR_D = 403;
        public const int MAPNUM_NTR_INTERIOR_E = 404;
        public const int MAPNUM_NTR_INTERIOR_F = 405;
        public const int MAPNUM_NTR_INTERIOR_G = 406;
        public const int MAPNUM_DOOMSWALL_ARENA = 407;
        public const int MAPNUM_VAMPIRE_ARENA = 408;
        public const int MAPNUM_NTR_INTERIOR_H = 409;
        public const int MAPNUM_DOOMSWALL_ARENA_AFTER = 410;
        // nothing in 411
        public const int MAPNUM_GRANDPALACE_INTERIOR_A = 412;
        public const int MAPNUM_GRANDPALACE_INTERIOR_B = 413;
        public const int MAPNUM_GRANDPALACE_INTERIOR_C = 414;
        public const int MAPNUM_GRANDPALACE_INTERIOR_D = 415;
        public const int MAPNUM_GRANDPALACE_INTERIOR_E = 416;
        public const int MAPNUM_GRANDPALACE_INTERIOR_F = 417;
        public const int MAPNUM_HEXAS_ARENA = 418;
        public const int MAPNUM_GRANDPALACE_MECHRIDER3_BUTTON = 419;
        public const int MAPNUM_GRANDPALACE_GNOME_ORB = 420;
        public const int MAPNUM_GRANDPALACE_UNDINE_ORB = 421;
        public const int MAPNUM_GRANDPALACE_SYLPHID_ORB = 422;
        public const int MAPNUM_GRANDPALACE_SALAMANDO_ORB = 423;
        public const int MAPNUM_GRANDPALACE_LUMINA_ORB = 424;
        public const int MAPNUM_GRANDPALACE_SHADE_ORB = 425;
        public const int MAPNUM_GRANDPALACE_LUNA_ORB = 426;
        // nothing in 427
        public const int MAPNUM_GRANDPALACE_INTERIOR_G = 428;
        // nothing in 429
        public const int MAPNUM_SNAPDRAGON_ARENA = 430;
        public const int MAPNUM_DRYADSEED = 431;
        public const int MAPNUM_UNDERSEA_AXE_ROOM_A = 432;
        public const int MAPNUM_UNDERSEA_AXE_ROOM_B = 433;
        public const int MAPNUM_HYDRA_ARENA = 434;
        public const int MAPNUM_HYDRA_ARENA_AFTER = 435;
        public const int MAPNUM_UNDERSEA_AXE_ROOM_C = 436;
        // nothing in 437-443
        public const int MAPNUM_SNOWDRAGON_AFTER = 444;
        public const int MAPNUM_REDDRAGON_AFTER = 445;
        public const int MAPNUM_BLUEDRAGON_AFTER = 446;
        // nothing in 447
        public const int MAPNUM_TURTLEISLAND_INTERIOR = 448;
        public const int MAPNUM_TURTLEISLAND_SEAHARETAIL = 449;
        // nothing in 450 - 487
        public const int MAPNUM_DREADSLIME_ARENA = 488;
        public const int MAPNUM_LIMESLIME_ARENA = 489;
        // nothing in 490 - 494
        public const int MAPNUM_FIREGIGAS_ARENA = 495;
        // nothing in 496 - 511


        public static string weaponByteToName(int weapon)
        {
            // weapon ordering is pretty consistent in vanilla and there should consistently work.
            switch (weapon)
            {
                case 0:
                    return "Glove";
                case 1:
                    return "Sword";
                case 2:
                    return "Axe";
                case 3:
                    return "Spear";
                case 4:
                    return "Whip";
                case 5:
                    return "Bow";
                case 6:
                    return "Boomerang";
                case 7:
                    return "Javelin";
            }
            return "";
        }

        public static int weaponByteFromName(string weaponName)
        {
            // weapon ordering is pretty consistent in vanilla and these should consistently work.
            switch (weaponName.ToLower())
            {
                case "glove":
                    return 0;
                case "sword":
                    return 1;
                case "axe":
                    return 2;
                case "spear":
                    return 3;
                case "whip":
                    return 4;
                case "bow":
                    return 5;
                case "boomerang":
                    return 6;
                case "javelin":
                    return 7;
            }
            throw new Exception("Unrecognized weapon name: " + weaponName);
        }

        private static string elementOrbByteToName(byte element)
        {
            // note this is mainly for element orbs; vanilla mana is VERY inconsistent with the ordering of its
            // 8 elements and you should not assume this is the correct order elsewhere
            switch (element)
            {
                case 0x81:
                    return "Gnome";
                case 0x82:
                    return "Undine";
                case 0x83:
                    return "Salamando";
                // swap sylphid and lumina back for better colors; they are erroneously swapped in vanilla
                case 0x85:
                    return "Sylphid";
                case 0x84:
                    return "Lumina";
                case 0x86:
                    return "Shade";
                case 0x87:
                    return "Luna";
                case 0x88:
                    return "Dryad";
            }
            return "";
        }

        public static string elementOrbByteToName(byte element, bool capitalizeName)
        {
            return capitalizeName ? elementOrbByteToName(element) : elementOrbByteToName(element).ToLower();
        }

        public static byte elementOrbNameToByte(string element)
        {
            // again, this is mainly for element orbs.

            // x81->x350 = gnome
            // x82->x351 = undine
            // x83->x353 = salamando
            // x84->x354 = lumina
            // x85->x352 = sylphid
            // x86->x355 = shade
            // x87->x356 = luna
            // x88->x357 = dryad
            switch (element.ToLower())
            {
                case "gnome":
                    return 0x81;
                case "undine":
                    return 0x82;
                case "salamando":
                    return 0x83;
                case "lumina":
                    return 0x84;
                case "sylphid":
                    return 0x85;
                case "shade":
                    return 0x86;
                case "luna":
                    return 0x87;
                case "dryad":
                    return 0x88;
            }
            // default none
            return 0xFF;
        }

    }
}
