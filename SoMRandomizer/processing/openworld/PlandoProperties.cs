using System.Collections.Generic;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// Keys and values for open world plando.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class PlandoProperties
    {
        // so prize plando knows which ones to process and which are handled by other hacks
        public const string KEY_PREFIX_PRIZE = "p_";
        public const string KEY_PREFIX_NONPRIZE = "P_";
        public const string VALUE_PREFIX = "";

        // -----------------------
        // prize properties
        // -----------------------

        // these are the keys that make up the plando options string
        // note the plando window shows these with the dashes replaced by spaces

        // locations

        // pandora (and potos) region
        public const string KEY_LOCATION_POTOS_CHEST = KEY_PREFIX_PRIZE + "Potos-Chest";
        public const string KEY_LOCATION_MANTIS_ANT = KEY_PREFIX_PRIZE + "Mantis-Ant";
        public const string KEY_LOCATION_PANDORA_GIRL = KEY_PREFIX_PRIZE + "Pandora-Girl";
        public const string KEY_LOCATION_PANDORA_CHEST_1 = KEY_PREFIX_PRIZE + "Pandora-Chest-1";
        public const string KEY_LOCATION_PANDORA_CHEST_2 = KEY_PREFIX_PRIZE + "Pandora-Chest-2";
        public const string KEY_LOCATION_PANDORA_CHEST_3 = KEY_PREFIX_PRIZE + "Pandora-Chest-3";
        public const string KEY_LOCATION_PANDORA_CHEST_4 = KEY_PREFIX_PRIZE + "Pandora-Chest-4";
        public const string KEY_LOCATION_PANDORA_CHEST_5 = KEY_PREFIX_PRIZE + "Pandora-Chest-5";
        public const string KEY_LOCATION_PANDORA_CHEST_6 = KEY_PREFIX_PRIZE + "Pandora-Chest-6";
        public const string KEY_LOCATION_LUKA_1 = KEY_PREFIX_PRIZE + "Luka-1";
        public const string KEY_LOCATION_LUKA_2 = KEY_PREFIX_PRIZE + "Luka-2";
        public const string KEY_LOCATION_UNDINE_1 = KEY_PREFIX_PRIZE + "Undine-1";
        public const string KEY_LOCATION_UNDINE_2 = KEY_PREFIX_PRIZE + "Undine-2";
        public const string KEY_LOCATION_ELINEE_CHEST_1 = KEY_PREFIX_PRIZE + "Elinee_Chest-1";
        public const string KEY_LOCATION_ELINEE_CHEST_2 = KEY_PREFIX_PRIZE + "Elinee_Chest-2";
        public const string KEY_LOCATION_SWORD_PEDESTAL = KEY_PREFIX_PRIZE + "Sword-Pedestal";
        public const string KEY_LOCATION_ANY_PANDORA = KEY_PREFIX_PRIZE + "(Any-Pandora-region-location)";

        // gaia's navel
        public const string KEY_LOCATION_MAGIC_ROPE_CHEST = KEY_PREFIX_PRIZE + "Magic-Rope-Chest";
        public const string KEY_LOCATION_WATTS = KEY_PREFIX_PRIZE + "Watts";
        public const string KEY_LOCATION_DWARF_ELDER = KEY_PREFIX_PRIZE + "Dwarf-Elder";
        public const string KEY_LOCATION_TROPICALLO_1 = KEY_PREFIX_PRIZE + "Tropicallo-1";
        public const string KEY_LOCATION_TROPICALLO_2 = KEY_PREFIX_PRIZE + "Tropicallo-2";
        public const string KEY_LOCATION_GNOME_1 = KEY_PREFIX_PRIZE + "Gnome-1";
        public const string KEY_LOCATION_GNOME_2 = KEY_PREFIX_PRIZE + "Gnome-2";
        public const string KEY_LOCATION_KILROY = KEY_PREFIX_PRIZE + "Kilroy";
        public const string KEY_LOCATION_KILROY_CHEST = KEY_PREFIX_PRIZE + "Kilroy-Chest";
        public const string KEY_LOCATION_ANY_GAIAS_NAVEL = KEY_PREFIX_PRIZE + "(Any-Navel-location)";

        // upper land
        public const string KEY_LOCATION_MOOGLE_TOWN_CHEST_1 = KEY_PREFIX_PRIZE + "Moogle-Town-Chest-1";
        public const string KEY_LOCATION_MOOGLE_TOWN_CHEST_2 = KEY_PREFIX_PRIZE + "Moogle-Town-Chest-2";
        public const string KEY_LOCATION_SYLPHID_1 = KEY_PREFIX_PRIZE + "Sylphid-1";
        public const string KEY_LOCATION_SYLPHID_2 = KEY_PREFIX_PRIZE + "Sylphid-2";
        public const string KEY_LOCATION_MATANGO_INN_CHEST = KEY_PREFIX_PRIZE + "Matango-Inn-Chest";
        public const string KEY_LOCATION_MATANGO_CAVE_FLAMMIE = KEY_PREFIX_PRIZE + "Matango-Cave-Flammie";
        public const string KEY_LOCATION_ANY_UPPER_LAND = KEY_PREFIX_PRIZE + "(Any-Upper-Land-location)";

        // empire
        public const string KEY_LOCATION_SOUTHTOWN_MARA = KEY_PREFIX_PRIZE + "Southtown-Mara";
        public const string KEY_LOCATION_NTR_WEST_CHEST = KEY_PREFIX_PRIZE + "NTR-West-Chest";
        public const string KEY_LOCATION_NTR_EAST_CHEST = KEY_PREFIX_PRIZE + "NTR-East-Chest";
        public const string KEY_LOCATION_NTR_INNER_CHEST = KEY_PREFIX_PRIZE + "NTR-Inner-Chest";
        public const string KEY_LOCATION_NTR_WALL = KEY_PREFIX_PRIZE + "NTR-Wall";
        public const string KEY_LOCATION_NTR_VAMPIRE = KEY_PREFIX_PRIZE + "NTR-Vampire";
        public const string KEY_LOCATION_NTC_EAST_CHEST_1 = KEY_PREFIX_PRIZE + "NTC-East-Chest-1";
        public const string KEY_LOCATION_NTC_EAST_CHEST_2 = KEY_PREFIX_PRIZE + "NTC-East-Chest-2";
        public const string KEY_LOCATION_NTC_METAL_MANTIS = KEY_PREFIX_PRIZE + "NTC-Metal-Mantis";
        public const string KEY_LOCATION_NTC_INNER_CHEST = KEY_PREFIX_PRIZE + "NTC-Inner-Chest";
        public const string KEY_LOCATION_NTC_MECH_RIDER = KEY_PREFIX_PRIZE + "NTC-Mech-Rider";
        public const string KEY_LOCATION_GOLD_TOWER_CHEST_1 = KEY_PREFIX_PRIZE + "Gold-Tower-Chest-1";
        public const string KEY_LOCATION_GOLD_TOWER_CHEST_2 = KEY_PREFIX_PRIZE + "Gold-Tower-Chest-2";
        public const string KEY_LOCATION_LUMINA_1 = KEY_PREFIX_PRIZE + "Lumina-1";
        public const string KEY_LOCATION_LUMINA_2 = KEY_PREFIX_PRIZE + "Lumina-2";
        public const string KEY_LOCATION_ANY_EMPIRE = KEY_PREFIX_PRIZE + "(Any-Empire-location)";

        // ice country
        public const string KEY_LOCATION_SALAMANDO_STOVE = KEY_PREFIX_PRIZE + "Salamando-Stove";
        public const string KEY_LOCATION_TRIPLE_TONPOLE = KEY_PREFIX_PRIZE + "Triple-Tonpole";
        public const string KEY_LOCATION_ICE_CASTLE_CHEST = KEY_PREFIX_PRIZE + "Ice-Castle-Chest";
        public const string KEY_LOCATION_FROST_GIGAS = KEY_PREFIX_PRIZE + "Frost-Gigas";
        public const string KEY_LOCATION_ANY_ICE_COUNTRY = KEY_PREFIX_PRIZE + "(Any-Ice-Country-location)";

        // kakkara
        public const string KEY_LOCATION_SEA_HARE_TAIL_GIFT = KEY_PREFIX_PRIZE + "Sea-Hare-Tail-Gift";
        public const string KEY_LOCATION_FIRE_PALACE_CHEST_1 = KEY_PREFIX_PRIZE + "Fire-Palace-Chest-1";
        public const string KEY_LOCATION_FIRE_PALACE_CHEST_2 = KEY_PREFIX_PRIZE + "Fire-Palace-Chest-2";
        public const string KEY_LOCATION_FIRE_PALACE_CHEST_3 = KEY_PREFIX_PRIZE + "Fire-Palace-Chest-3";
        public const string KEY_LOCATION_FIRE_PALACE_SALAMANDO = KEY_PREFIX_PRIZE + "Fire-Palace-Chest-Salamando";
        public const string KEY_LOCATION_LUNA_1 = KEY_PREFIX_PRIZE + "Luna-1";
        public const string KEY_LOCATION_LUNA_2 = KEY_PREFIX_PRIZE + "Luna-2";
        public const string KEY_LOCATION_ANY_DESERT = KEY_PREFIX_PRIZE + "(Any-Desert-location)";

        // mountains
        public const string KEY_LOCATION_POD_GATED_CHEST = KEY_PREFIX_PRIZE + "POD-Gated-Chest";
        public const string KEY_LOCATION_POD_HALLWAY_CHEST = KEY_PREFIX_PRIZE + "POD-Hallway-Chest";
        public const string KEY_LOCATION_SHADE_1 = KEY_PREFIX_PRIZE + "Shade-1";
        public const string KEY_LOCATION_SHADE_2 = KEY_PREFIX_PRIZE + "Shade-2";
        public const string KEY_LOCATION_DOPPEL = KEY_PREFIX_PRIZE + "Doppel";
        public const string KEY_LOCATION_ANY_MOUNTAINS = KEY_PREFIX_PRIZE + "(Any-Mountains-location)";

        // lost continent
        public const string KEY_LOCATION_WATERMELON = KEY_PREFIX_PRIZE + "Watermelon";
        public const string KEY_LOCATION_DRYAD_1 = KEY_PREFIX_PRIZE + "Dryad-1";
        public const string KEY_LOCATION_DRYAD_2 = KEY_PREFIX_PRIZE + "Dryad-2";
        public const string KEY_LOCATION_HEXAS = KEY_PREFIX_PRIZE + "Hexas";
        public const string KEY_LOCATION_SPELL_ORB_AREA_CHEST = KEY_PREFIX_PRIZE + "Spell-Orb-Area-Chest";
        public const string KEY_LOCATION_UNDERSEA_AREA_CHEST = KEY_PREFIX_PRIZE + "Undersea-Area-Chest";
        public const string KEY_LOCATION_HYDRA = KEY_PREFIX_PRIZE + "Hydra";
        public const string KEY_LOCATION_KETTLE_KIN = KEY_PREFIX_PRIZE + "Kettle-Kin";
        public const string KEY_LOCATION_MECH_RIDER_3 = KEY_PREFIX_PRIZE + "Mech-Rider-3";
        public const string KEY_LOCATION_ANY_LOST_CONTINENT = KEY_PREFIX_PRIZE + "(Any-Lost-Continent-location)";

        // pure land
        public const string KEY_LOCATION_DRAGON_WORM = KEY_PREFIX_PRIZE + "Dragon-Worm";
        public const string KEY_LOCATION_SNOW_DRAGON = KEY_PREFIX_PRIZE + "Snow-Dragon";
        public const string KEY_LOCATION_AXE_BEAK = KEY_PREFIX_PRIZE + "Axe-Beak";
        public const string KEY_LOCATION_RED_DRAGON = KEY_PREFIX_PRIZE + "Red-Dragon";
        public const string KEY_LOCATION_THUNDER_GIGAS = KEY_PREFIX_PRIZE + "Thunder-Gigas";
        public const string KEY_LOCATION_BLUE_DRAGON = KEY_PREFIX_PRIZE + "Blue-Dragon";
        public const string KEY_LOCATION_MANA_TREE = KEY_PREFIX_PRIZE + "Mana-Tree";
        public const string KEY_LOCATION_ANY_PURE_LANDS = KEY_PREFIX_PRIZE + "(Any-Pure-Lands-location)";

        // misc
        public const string KEY_LOCATION_TURTLE_ISLAND = KEY_PREFIX_PRIZE + "Turtle-Island";
        public const string KEY_LOCATION_LIGHTHOUSE = KEY_PREFIX_PRIZE + "Lighthouse";
        public const string KEY_LOCATION_TASNICA_MINIBOSS = KEY_PREFIX_PRIZE + "Tasnica-Miniboss";
        public const string KEY_LOCATION_BUFFY = KEY_PREFIX_PRIZE + "Buffy";
        public const string KEY_LOCATION_DREAD_SLIME = KEY_PREFIX_PRIZE + "Dread-Slime";
        public const string KEY_LOCATION_ANY_MANA_SEED_PEDESTAL = KEY_PREFIX_PRIZE + "(Any-Mana-seed-pedestal)";
        public const string KEY_LOCATION_ANY_BOSS = KEY_PREFIX_PRIZE + "(Any-Boss)";
        public const string KEY_LOCATION_START_WITH = KEY_PREFIX_PRIZE + "(Start-with)";
        public const string KEY_LOCATION_NON_EXISTENT = KEY_PREFIX_PRIZE + "(NON-EXISTENT)";

        // prizes

        // characters
        public const string VALUE_PRIZE_BOY = "Boy";
        public const string VALUE_PRIZE_GIRL = "Girl";
        public const string VALUE_PRIZE_SPRITE = "Sprite";
        public const string VALUE_PRIZE_OTHER_CHARACTER_PREFIX = "Other-Character";
        public const string VALUE_PRIZE_OTHER_CHARACTER_1 = VALUE_PRIZE_OTHER_CHARACTER_PREFIX + "-1";
        public const string VALUE_PRIZE_OTHER_CHARACTER_2 = VALUE_PRIZE_OTHER_CHARACTER_PREFIX + "-2";
        public const string VALUE_PRIZE_ANY_CHARACTER = "(Any-Character)";

        // weapons
        public const string VALUE_PRIZE_GLOVE = "Glove";
        public const string VALUE_PRIZE_SWORD = "Sword";
        public const string VALUE_PRIZE_AXE = "Axe";
        public const string VALUE_PRIZE_SPEAR = "Spear";
        public const string VALUE_PRIZE_WHIP = "Whip";
        public const string VALUE_PRIZE_BOW = "Bow";
        public const string VALUE_PRIZE_BOOMERANG = "Boomerang";
        public const string VALUE_PRIZE_JAVELIN = "Javelin";
        public const string VALUE_PRIZE_ANY_WEAPON = "(Any-Weapon)";
        public const string VALUE_PRIZE_CUTTING_WEAPON = "(Cutting-Weapon)";

        // elements/spells
        public const string VALUE_PRIZE_UNDINE = "Undine";
        public const string VALUE_PRIZE_GNOME = "Gnome";
        public const string VALUE_PRIZE_SYLPHID = "Sylphid";
        public const string VALUE_PRIZE_SALAMANDO = "Salamando";
        public const string VALUE_PRIZE_LUMINA = "Lumina";
        public const string VALUE_PRIZE_SHADE = "Shade";
        public const string VALUE_PRIZE_LUNA = "Luna";
        public const string VALUE_PRIZE_DRYAD = "Dryad";
        public const string VALUE_PRIZE_ANY_SPELLS = "(Any-Spells)";

        // seeds
        public const string VALUE_PRIZE_WATER_SEED = "Water-Seed";
        public const string VALUE_PRIZE_EARTH_SEED = "Earth-Seed";
        public const string VALUE_PRIZE_WIND_SEED = "Wind-Seed";
        public const string VALUE_PRIZE_FIRE_SEED = "Fire-Seed";
        public const string VALUE_PRIZE_LIGHT_SEED = "Light-Seed";
        public const string VALUE_PRIZE_DARK_SEED = "Dark-Seed";
        public const string VALUE_PRIZE_MOON_SEED = "Moon-Seed";
        public const string VALUE_PRIZE_DRYAD_SEED = "Dryad-Seed";
        public const string VALUE_PRIZE_ANY_SEED = "(Any-Seed)";

        // weapon orbs
        public const string VALUE_PRIZE_GLOVE_ORB = "Glove-Orb";
        public const string VALUE_PRIZE_SWORD_ORB = "Sword-Orb";
        public const string VALUE_PRIZE_AXE_ORB = "Axe-Orb";
        public const string VALUE_PRIZE_SPEAR_ORB = "Spear-Orb";
        public const string VALUE_PRIZE_WHIP_ORB = "Whip-Orb";
        public const string VALUE_PRIZE_BOW_ORB = "Bow-Orb";
        public const string VALUE_PRIZE_BOOMERANG_ORB = "Boomerang-Orb";
        public const string VALUE_PRIZE_JAVELIN_ORB = "Javelin-Orb";
        public const string VALUE_PRIZE_ANY_WEAPON_ORB = "(Any-Weapon-Orb)";

        // misc
        public const string VALUE_PRIZE_MOOGLE_BELT = "Moogle-Belt";
        public const string VALUE_PRIZE_MIDGE_MALLET = "Midge-Mallet";
        public const string VALUE_PRIZE_SEA_HARE_TAIL = "Sea-Hare-Tail";
        public const string VALUE_PRIZE_GOLD_KEY = "Gold-Key";
        public const string VALUE_PRIZE_FLAMMIE_DRUM = "Flammie-Drum";
        public const string VALUE_PRIZE_MONEY = "Money";
        public const string VALUE_PRIZE_NOTHING = "Nothing";

        // -----------------------
        // non-prize properties
        // -----------------------

        // misc settings
        public const string KEY_BYPASS_VALIDATION = KEY_PREFIX_NONPRIZE + "bypassValidation";

        // orb elements
        public const string KEY_EARTH_PALACE_ORB_ELEMENT = KEY_PREFIX_NONPRIZE + "earthPalaceOrbEle";
        public const string KEY_MATANGO_ORB_ELEMENT = KEY_PREFIX_NONPRIZE + "matangoOrbEle";
        public const string KEY_LUNA_PALACE_ORB_ELEMENT = KEY_PREFIX_NONPRIZE + "lunaPalaceOrbEle";
        public const string KEY_GRAND_PALACE_ORB_1_ELEMENT = KEY_PREFIX_NONPRIZE + "gpOrb1Ele";
        public const string KEY_GRAND_PALACE_ORB_2_ELEMENT = KEY_PREFIX_NONPRIZE + "gpOrb2Ele";
        public const string KEY_GRAND_PALACE_ORB_3_ELEMENT = KEY_PREFIX_NONPRIZE + "gpOrb3Ele";
        public const string KEY_GRAND_PALACE_ORB_4_ELEMENT = KEY_PREFIX_NONPRIZE + "gpOrb4Ele";
        public const string KEY_GRAND_PALACE_ORB_5_ELEMENT = KEY_PREFIX_NONPRIZE + "gpOrb5Ele";
        public const string KEY_GRAND_PALACE_ORB_6_ELEMENT = KEY_PREFIX_NONPRIZE + "gpOrb6Ele";
        public const string KEY_GRAND_PALACE_ORB_7_ELEMENT = KEY_PREFIX_NONPRIZE + "gpOrb7Ele";
        public const string KEY_UPPER_LAND_ORB_ELEMENT = KEY_PREFIX_NONPRIZE + "upperLandOrbEle";
        public const string KEY_FIRE_PALACE_ORB_1_ELEMENT = KEY_PREFIX_NONPRIZE + "fpOrb1Ele";
        public const string KEY_FIRE_PALACE_ORB_2_ELEMENT = KEY_PREFIX_NONPRIZE + "fpOrb2Ele";
        public const string KEY_FIRE_PALACE_ORB_3_ELEMENT = KEY_PREFIX_NONPRIZE + "fpOrb3Ele";

        // weapon settings
        public const string KEY_BOY_WEAPON = KEY_PREFIX_NONPRIZE + "boyWeapon";
        public const string KEY_GIRL_WEAPON = KEY_PREFIX_NONPRIZE + "girlWeapon";
        public const string KEY_SPRITE_WEAPON = KEY_PREFIX_NONPRIZE + "spriteWeapon";
        public const string KEY_P2_WEAPON = KEY_PREFIX_NONPRIZE + "p2Weapon";
        public const string KEY_P3_WEAPON = KEY_PREFIX_NONPRIZE + "p3Weapon";

        // boss type by location
        public const string KEY_BOSS_MANTISANT = KEY_PREFIX_NONPRIZE + "mantisAnt";
        public const string KEY_BOSS_TROPICALLO = KEY_PREFIX_NONPRIZE + "tropicallo";
        public const string KEY_BOSS_SPIKEY = KEY_PREFIX_NONPRIZE + "spikeyTiger";
        public const string KEY_BOSS_TONPOLE = KEY_PREFIX_NONPRIZE + "tonpole";
        public const string KEY_BOSS_FIREGIGAS = KEY_PREFIX_NONPRIZE + "fireGigas";
        public const string KEY_BOSS_KILROY = KEY_PREFIX_NONPRIZE + "kilroy";
        public const string KEY_BOSS_WALLFACE = KEY_PREFIX_NONPRIZE + "wallFace";
        public const string KEY_BOSS_JABBERWOCKY = KEY_PREFIX_NONPRIZE + "jabberwocky";
        public const string KEY_BOSS_SPRINGBEAK = KEY_PREFIX_NONPRIZE + "springBeak";
        public const string KEY_BOSS_GREATVIPER = KEY_PREFIX_NONPRIZE + "greatViper";
        public const string KEY_BOSS_BOREALFACE = KEY_PREFIX_NONPRIZE + "borealFace";
        public const string KEY_BOSS_FROSTGIGAS = KEY_PREFIX_NONPRIZE + "frostGigas";
        public const string KEY_BOSS_MINOTAUR = KEY_PREFIX_NONPRIZE + "minotaur";
        public const string KEY_BOSS_DOOMSWALL = KEY_PREFIX_NONPRIZE + "doomsWall";
        public const string KEY_BOSS_VAMPIRE = KEY_PREFIX_NONPRIZE + "vampire";
        public const string KEY_BOSS_METALMANTIS = KEY_PREFIX_NONPRIZE + "metalMantis";
        public const string KEY_BOSS_MECHRIDER2 = KEY_PREFIX_NONPRIZE + "mechRider2";
        public const string KEY_BOSS_BLUESPIKE = KEY_PREFIX_NONPRIZE + "blueSpike";
        public const string KEY_BOSS_GORGONBULL = KEY_PREFIX_NONPRIZE + "gorgonBull";
        public const string KEY_BOSS_HYDRA = KEY_PREFIX_NONPRIZE + "hydra";
        public const string KEY_BOSS_KETTLEKIN = KEY_PREFIX_NONPRIZE + "kettleKin";
        public const string KEY_BOSS_SNAPDRAGON = KEY_PREFIX_NONPRIZE + "snapDragon";
        public const string KEY_BOSS_WATERMELON = KEY_PREFIX_NONPRIZE + "watermelon";
        public const string KEY_BOSS_HEXAS = KEY_PREFIX_NONPRIZE + "hexas";
        public const string KEY_BOSS_MECHRIDER3 = KEY_PREFIX_NONPRIZE + "mechRider3";
        public const string KEY_BOSS_DRAGONWORM = KEY_PREFIX_NONPRIZE + "dragonWorm";
        public const string KEY_BOSS_SNOWDRAGON = KEY_PREFIX_NONPRIZE + "snowDragon";
        public const string KEY_BOSS_AXEBEAK = KEY_PREFIX_NONPRIZE + "axeBeak";
        public const string KEY_BOSS_REDDRAGON = KEY_PREFIX_NONPRIZE + "redDragon";
        public const string KEY_BOSS_THUNDERGIGAS = KEY_PREFIX_NONPRIZE + "thunderGigas";
        public const string KEY_BOSS_BLUEDRAGON = KEY_PREFIX_NONPRIZE + "blueDragon";
        public const string KEY_BOSS_BUFFY = KEY_PREFIX_NONPRIZE + "buffy";
        public const string KEY_BOSS_DARKLICH = KEY_PREFIX_NONPRIZE + "darkLich";

        // boss element by location
        public const string KEY_BOSS_ELEMENT_MANTISANT = KEY_PREFIX_NONPRIZE + "mantisAntEle";
        public const string KEY_BOSS_ELEMENT_TROPICALLO = KEY_PREFIX_NONPRIZE + "tropicalloEle";
        public const string KEY_BOSS_ELEMENT_SPIKEY = KEY_PREFIX_NONPRIZE + "spikeyTigerEle";
        public const string KEY_BOSS_ELEMENT_TONPOLE = KEY_PREFIX_NONPRIZE + "tonpoleEle";
        public const string KEY_BOSS_ELEMENT_BITING_LIZARD = KEY_PREFIX_NONPRIZE + "bitingLizardEle";
        public const string KEY_BOSS_ELEMENT_FIREGIGAS = KEY_PREFIX_NONPRIZE + "fireGigasEle";
        public const string KEY_BOSS_ELEMENT_KILROY = KEY_PREFIX_NONPRIZE + "kilroyEle";
        public const string KEY_BOSS_ELEMENT_WALLFACE = KEY_PREFIX_NONPRIZE + "wallFaceEle";
        public const string KEY_BOSS_ELEMENT_JABBERWOCKY = KEY_PREFIX_NONPRIZE + "jabberwockyEle";
        public const string KEY_BOSS_ELEMENT_SPRINGBEAK = KEY_PREFIX_NONPRIZE + "springBeakEle";
        public const string KEY_BOSS_ELEMENT_GREATVIPER = KEY_PREFIX_NONPRIZE + "greatViperEle";
        public const string KEY_BOSS_ELEMENT_BOREALFACE = KEY_PREFIX_NONPRIZE + "borealFaceEle";
        public const string KEY_BOSS_ELEMENT_FROSTGIGAS = KEY_PREFIX_NONPRIZE + "frostGigasEle";
        public const string KEY_BOSS_ELEMENT_MINOTAUR = KEY_PREFIX_NONPRIZE + "minotaurEle";
        public const string KEY_BOSS_ELEMENT_DOOMSWALL = KEY_PREFIX_NONPRIZE + "doomsWallEle";
        public const string KEY_BOSS_ELEMENT_VAMPIRE = KEY_PREFIX_NONPRIZE + "vampireEle";
        public const string KEY_BOSS_ELEMENT_METALMANTIS = KEY_PREFIX_NONPRIZE + "metalMantisEle";
        public const string KEY_BOSS_ELEMENT_MECHRIDER2 = KEY_PREFIX_NONPRIZE + "mechRider2Ele";
        public const string KEY_BOSS_ELEMENT_BLUESPIKE = KEY_PREFIX_NONPRIZE + "blueSpikeEle";
        public const string KEY_BOSS_ELEMENT_GORGONBULL = KEY_PREFIX_NONPRIZE + "gorgonBullEle";
        public const string KEY_BOSS_ELEMENT_HYDRA = KEY_PREFIX_NONPRIZE + "hydraEle";
        public const string KEY_BOSS_ELEMENT_KETTLEKIN = KEY_PREFIX_NONPRIZE + "kettleKinEle";
        public const string KEY_BOSS_ELEMENT_SNAPDRAGON = KEY_PREFIX_NONPRIZE + "snapDragonEle";
        public const string KEY_BOSS_ELEMENT_WATERMELON = KEY_PREFIX_NONPRIZE + "watermelonEle";
        public const string KEY_BOSS_ELEMENT_HEXAS = KEY_PREFIX_NONPRIZE + "hexasEle";
        public const string KEY_BOSS_ELEMENT_MECHRIDER3 = KEY_PREFIX_NONPRIZE + "mechRider3Ele";
        public const string KEY_BOSS_ELEMENT_DRAGONWORM = KEY_PREFIX_NONPRIZE + "dragonWormEle";
        public const string KEY_BOSS_ELEMENT_SNOWDRAGON = KEY_PREFIX_NONPRIZE + "snowDragonEle";
        public const string KEY_BOSS_ELEMENT_AXEBEAK = KEY_PREFIX_NONPRIZE + "axeBeakEle";
        public const string KEY_BOSS_ELEMENT_REDDRAGON = KEY_PREFIX_NONPRIZE + "redDragonEle";
        public const string KEY_BOSS_ELEMENT_THUNDERGIGAS = KEY_PREFIX_NONPRIZE + "thunderGigasEle";
        public const string KEY_BOSS_ELEMENT_BLUEDRAGON = KEY_PREFIX_NONPRIZE + "blueDragonEle";
        public const string KEY_BOSS_ELEMENT_BUFFY = KEY_PREFIX_NONPRIZE + "buffyEle";
        public const string KEY_BOSS_ELEMENT_DARKLICH = KEY_PREFIX_NONPRIZE + "darkLichEle";

        // boss names for plando bosses
        public const string VALUE_BOSS_MANTISANT = "Mantis-Ant";
        public const string VALUE_BOSS_TROPICALLO = "Tropicallo";
        public const string VALUE_BOSS_SPIKEY = "Spikey-Tiger";
        public const string VALUE_BOSS_TONPOLE = "Tonpole";
        public const string VALUE_BOSS_FIREGIGAS = "Fire-Gigas";
        public const string VALUE_BOSS_KILROY = "Kilroy";
        public const string VALUE_BOSS_WALLFACE = "Wall-Face";
        public const string VALUE_BOSS_JABBERWOCKY = "Jabberwocky";
        public const string VALUE_BOSS_SPRINGBEAK = "Spring-Beak";
        public const string VALUE_BOSS_GREATVIPER = "Great-Viper";
        public const string VALUE_BOSS_BOREALFACE = "Boreal-Face";
        public const string VALUE_BOSS_FROSTGIGAS = "Frost-Gigas";
        public const string VALUE_BOSS_MINOTAUR = "Minotaur";
        public const string VALUE_BOSS_DOOMSWALL = "Doom's-Wall";
        public const string VALUE_BOSS_VAMPIRE = "Vampire";
        public const string VALUE_BOSS_METALMANTIS = "Metal-Mantis";
        public const string VALUE_BOSS_MECHRIDER2 = "Mech-Rider-2";
        public const string VALUE_BOSS_BLUESPIKE = "Blue-Spike";
        public const string VALUE_BOSS_GORGONBULL = "Gorgon-Bull";
        public const string VALUE_BOSS_HYDRA = "Hydra";
        public const string VALUE_BOSS_KETTLEKIN = "Kettle-Kin";
        public const string VALUE_BOSS_SNAPDRAGON = "Snap-Dragon";
        public const string VALUE_BOSS_WATERMELON = "Watermelon";
        public const string VALUE_BOSS_HEXAS = "Hexas";
        public const string VALUE_BOSS_MECHRIDER3 = "Mech-Rider-3";
        public const string VALUE_BOSS_DRAGONWORM = "Dragon-Worm";
        public const string VALUE_BOSS_SNOWDRAGON = "Snow-Dragon";
        public const string VALUE_BOSS_AXEBEAK = "Axe-Beak";
        public const string VALUE_BOSS_REDDRAGON = "Red-Dragon";
        public const string VALUE_BOSS_THUNDERGIGAS = "Thunder-Gigas";
        public const string VALUE_BOSS_BLUEDRAGON = "Blue-Dragon";
        public const string VALUE_BOSS_BUFFY = "Buffy";
        public const string VALUE_BOSS_DARKLICH = "Dark-Lich";

        public const string VALUE_BOSSELEMENT_NO_CHANGE = "(No change)";
        public const string VALUE_BOSSELEMENT_DEFAULT = "(Default)";
        public const string VALUE_BOSSELEMENT_ELEMENTLESS = "(Elementless)";
        public const string VALUE_BOSSELEMENT_UNDINE = "Undine";
        public const string VALUE_BOSSELEMENT_GNOME = "Gnome";
        public const string VALUE_BOSSELEMENT_SYLPHID = "Sylphid";
        public const string VALUE_BOSSELEMENT_SALAMANDO = "Salamando";
        public const string VALUE_BOSSELEMENT_LUMINA = "Lumina";
        public const string VALUE_BOSSELEMENT_SHADE = "Shade";
        public const string VALUE_BOSSELEMENT_LUNA = "Luna";
        public const string VALUE_BOSSELEMENT_DRYAD = "Dryad";

        public const string VALUE_ORBELEMENT_NO_CHANGE = "(No change)";
        public const string VALUE_ORBELEMENT_NONE = "(None)";
        public const string VALUE_ORBELEMENT_UNDINE = "Undine";
        public const string VALUE_ORBELEMENT_GNOME = "Gnome";
        public const string VALUE_ORBELEMENT_SYLPHID = "Sylphid";
        public const string VALUE_ORBELEMENT_SALAMANDO = "Salamando";
        public const string VALUE_ORBELEMENT_LUMINA = "Lumina";
        public const string VALUE_ORBELEMENT_SHADE = "Shade";
        public const string VALUE_ORBELEMENT_LUNA = "Luna";
        public const string VALUE_ORBELEMENT_DRYAD = "Dryad";

        public const string VALUE_WEAPON_NO_CHANGE = "(No change)";
        public const string VALUE_WEAPON_GLOVE = "Glove";
        public const string VALUE_WEAPON_SWORD = "Sword";
        public const string VALUE_WEAPON_AXE = "Axe";
        public const string VALUE_WEAPON_SPEAR = "Spear";
        public const string VALUE_WEAPON_WHIP = "Whip";
        public const string VALUE_WEAPON_BOW = "Bow";
        public const string VALUE_WEAPON_BOOMERANG = "Boomerang";
        public const string VALUE_WEAPON_JAVELIN = "Javelin";

        public static Dictionary<string, byte> BOSS_IDS_BY_PLANDO_VALUE = new Dictionary<string, byte>
        {
            { VALUE_BOSS_MANTISANT, 0x57 },
            { VALUE_BOSS_TROPICALLO, 0x59 },
            { VALUE_BOSS_SPIKEY, 0x5B },
            { VALUE_BOSS_TONPOLE, 0x71 },
            { VALUE_BOSS_FIREGIGAS, 0x74 },
            { VALUE_BOSS_KILROY, 0x65 },
            { VALUE_BOSS_WALLFACE, 0x58 },
            { VALUE_BOSS_JABBERWOCKY, 0x5C },
            { VALUE_BOSS_SPRINGBEAK, 0x5D },
            { VALUE_BOSS_GREATVIPER, 0x69 },
            { VALUE_BOSS_BOREALFACE, 0x68 },
            { VALUE_BOSS_FROSTGIGAS, 0x5E },
            { VALUE_BOSS_MINOTAUR, 0x5A },
            { VALUE_BOSS_DOOMSWALL, 0x61 },
            { VALUE_BOSS_VAMPIRE, 0x62 },
            { VALUE_BOSS_METALMANTIS, 0x63 },
            { VALUE_BOSS_MECHRIDER2, 0x64 },
            { VALUE_BOSS_BLUESPIKE, 0x6B },
            { VALUE_BOSS_GORGONBULL, 0x66 },
            { VALUE_BOSS_HYDRA, 0x6D },
            { VALUE_BOSS_KETTLEKIN, 0x70 },
            { VALUE_BOSS_SNAPDRAGON, 0x5F },
            { VALUE_BOSS_WATERMELON, 0x6E },
            { VALUE_BOSS_HEXAS, 0x6F },
            { VALUE_BOSS_MECHRIDER3, 0x72 },
            { VALUE_BOSS_DRAGONWORM, 0x7B },
            { VALUE_BOSS_SNOWDRAGON, 0x73 },
            { VALUE_BOSS_AXEBEAK, 0x76 },
            { VALUE_BOSS_REDDRAGON, 0x75 },
            { VALUE_BOSS_THUNDERGIGAS, 0x7D },
            { VALUE_BOSS_BLUEDRAGON, 0x77 },
            { VALUE_BOSS_BUFFY, 0x78 },
            { VALUE_BOSS_DARKLICH, 0x79 },
        };

        public static string displayToProperty(string displayString, string prefix)
        {
            return prefix + displayString.Replace(" ", "-");
        }

        public static string propertyToDisplay(string displayString)
        {
            return displayString.Replace("-", " ").Replace(KEY_PREFIX_PRIZE, "").Replace(KEY_PREFIX_NONPRIZE, "");
        }

    }
}
