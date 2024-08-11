namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// A listing of event flags with their vanilla purpose, and anything they are repurposed for in rando.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EventFlags
    {
        // set when you get the sword to turn the layer 3 HUD on
        // LOTS OF STUFF BREAKS if you don't enable this.
        public const byte UI_DISPLAY_FLAG = 0x00; 
        // 00 = don't, 01 = do
        public const byte WALK_THROUGH_WALLS_FLAG = 0x01;
        public const byte SUNKEN_CONTINENT_MODE_FLAG = 0x02;
        public const byte WHIP_GROUPING_FLAG = 0x03;
        // from beginning where she can leave if you do the wrong shit
        public const byte GIRL_PERMANENT_FLAG = 0x04; 
        public const byte RABITEMAN_SHOW_FLAG = 0x05;
        // 6,7,8 unused
        // changes the music in potos plus makes him babble on about shit if you open the chest
        public const byte POTOS_ELDER_BACKSTORY_FLAG = 0x09;
        // used in a bunch of spots
        public const byte TEMPORARY_DIALOGUE_CHOICE_FLAG = 0x0A; 
        public const byte DEATH_TYPE_FLAG = 0x0B;
        // for map 256
        public const byte WATERFALL_BOY_VISIBILITY_FLAG = 0x0C; 
        public const byte GIRL_IN_PARTY_FLAG = 0x0D;
        public const byte SPRITE_IN_PARTY_FLAG = 0x0E;
        // seems hardcoded to be set by the gold subtraction event
        public const byte NOT_ENOUGH_GOLD_FLAG = 0x0F; 

        // normally: 0x00 -> 0x0B at end of potos sequence
        // now: 0x0B default, 0x0C killed mantis ant
        public const byte POTOS_FLAG = 0x10;
        // normally: 00 default, 01-02 talking to dyluck, 03 talking to jema, 
        // 0b after all the luka dialogue?
        // 0c after gaia's navel for some reason?
        // 0d after spikey tiger/dyluck dialogue
        // 0e when you get the seed back from kilroy
        // 0f set at lost continent
        // now: 0x00 default, set to 0x0B at luka dialogue? (event 0x127 / 0x204 in rando changes)
        public const byte WATER_PALACE_FLAG = 0x11;

        public const byte PANDORA_RUINS_FLAG = 0x12;

        public const byte PANDORA_GIRL_FLAG = 0x13; // let's set this to 7 initially?

        public const byte GAIAS_NAVEL_WATTS_FLAG = 0x14;

        public const byte GAIAS_NAVEL_SPRITE_FLAG = 0x15;

        public const byte WITCHFOREST_GIRL_FLAG = 0x16;

        public const byte WITCHCASTLE_FLAG = 0x17;

        public const byte EARTHPALACE_FLAG = 0x18;

        // 0x19 unused? - jabberwocky now
        public const byte OPENWORLD_JABBERWOCKY_FLAG = 0x19;

        public const byte PANDORA_PHANNA_FLAG = 0x1A;

        // also 0x50?
        public const byte KILROY_EVENT_FLAG = 0x1B;

        public const byte EARTHPALACE_SWITCH_FLAG_1 = 0x1C;
        public const byte EARTHPALACE_SWITCH_FLAG_2 = 0x1D;

        public const byte UPPERLAND_UNLOCK_FLAG = 0x1E;

        // 0x1f unused?

        public const byte UPPERLAND_MOOGLES_FLAG = 0x20;

        public const byte UPPERLAND_PROGRESS_FLAG = 0x21;

        public const byte UPPERLAND_SPRINGBEAK_FLAG = 0x22;

        // 0x23 unused?

        public const byte MATANGO_PROGRESS_FLAG = 0x24;

        // normally: 0x00 initially, 0x03 after killing all the tonpoles
        public const byte MULTI_TONPOLE_FLAG = 0x25;

        // killing icy tropicallo sets it to 2, tonpoles set it to 3, and santa sets it to 4-5
        public const byte ICE_PALACE_FLAG = 0x26;
        // repurpose this for boss visibility in procgen modes
        public const byte PROCGEN_MODE_BOSS_VISIBILITY_FLAG = 0x26;

        // 3 initially; 9 after getting salamando
        public const byte SALAMANDO_STOVE_FLAG = 0x27; 
        // repurpose this for timer running in procgen modes
        public const byte PROCGEN_MODE_TIMER_RUNNING_FLAG = 0x27; 

        // 0x0b to be done with desert ship & at mech rider; 0x0c when he's dead .. need this to be 0x0e initially to disable event 288/289
        // we can use 0x0f for him being dead
        public const byte DESERT_SHIP_FLAG = 0x28;
        // repurpose for procgen modes
        public const byte PROCGEN_MODE_CUSTOM_MUSIC_FLAG = 0x28; 

        public const byte SEA_HARES_TAIL_FLAG = 0x29;
        public const byte BOSS_RUSH_PHANNA_HEALS_FLAG = 0x29; // boss rush only; number of heals left

        // 0x2a unused?

        public const byte FIRE_PALACE_SWITCHES_FLAG = 0x2B;

        public const byte FIRE_PALACE_COMPLETION_FLAG = 0x2C;

        // 03 at wall, 04 when it dies, incremented to 05 by dyluck normally.. have wall double-increment it? (553) also remove dyluck, or have him be a hint?
        public const byte NORTHTOWN_RUINS_FLAG = 0x2D; 

        // potos area goblin capture thing
        // normally: 0x00 initially, 0x01 if captured, 0x02 if got girl after
        public const byte GOBLIN_EVENT_FLAG = 0x2E;

        public const byte RABITEMAN_FLAG = 0x2F;

        public const byte SOUTHTOWN_MARA_FLAG = 0x30;

        public const byte SOUTHTOWN_GUARD_VISIBILITY_FLAG = 0x31;

        public const byte NORTHTOWN_DIALOGUE_FLAG = 0x32;

        // doesn't seem to be used?  always 0
        public const byte NORTHTOWN_NPC_VISIBILITY_FLAG = 0x33;

        public const byte NORTHTOWN_CASTLE_FLAG = 0x34;

        // also used by dryad event for some reason?
        public const byte NORTHTOWN_CASTLE_PROGRESS_FLAG = 0x35;

        public const byte LOST_IN_DESERT_FLAG = 0x36;

        // includes key
        public const byte LUMINA_TOWER_PROGRESS_FLAG = 0x37;

        public const byte SHADE_PALACE_PROGRESS_FLAG_1 = 0x38;
        public const byte SHADE_PALACE_PROGRESS_FLAG_2 = 0x39;

        // also used by dryad event for some reason?
        public const byte NORTHTOWN_PHANNA_FLAG = 0x3A;

        public const byte LUNA_PALACE_FLAG = 0x3B;

        public const byte TASNICA_FLAG = 0x3C;

        // 0x3d unused?

        // appears not to be used
        public const byte MOUNTAIN_NPC_VISIBILITY_FLAG = 0x3E;

        public const byte JEHK_CAVE_FLAG = 0x3F;

        public const byte LOST_CONTINENT_SWITCHES_FLAG_1 = 0x40;

        // 1-2 when continent is up, 3 when it sinks, 4 eventually too
        public const byte LOST_CONTINENT_PROGRESS_FLAG_1 = 0x41;

        public const byte LOST_CONTINENT_SWITCHES_FLAG_2 = 0x42;

        // four colors switch
        public const byte LOST_CONTINENT_SWITCHES_FLAG_3 = 0x43;

        // 5 during hexas fight, 6 in death event; 2->8 allows passage to her map
        public const byte LOST_CONTINENT_HEXAS_FLAG = 0x44;

        public const byte LOST_CONTINENT_WATERMELON_FLAG = 0x45;

        public const byte PURELAND_PROGRESS_FLAG = 0x46;

        public const byte LOST_CONTINENT_KETTLEKIN_FLAG = 0x47;

        // in the 8 elements area
        public const byte LOST_CONTINENT_SWITCHES_FLAG_4 = 0x48;

        // in the 8 elements area
        public const byte LOST_CONTINENT_SWITCHES_FLAG_5 = 0x49;

        // lost continent dryad area
        public const byte DRYAD_AREA_FLAG_1 = 0x4A;
        // looks really similar to 4B?
        public const byte DRYAD_AREA_FLAG_2 = 0x4B;
        // also similar
        public const byte DRYAD_AREA_FLAG_3 = 0x4C;

        public const byte PHANNA_DIALOGUE_FLAG = 0x4D;

        public const byte MANAFORT_SWITCHES_FLAG_1 = 0x4E;
        public const byte MANAFORT_SWITCHES_FLAG_2 = 0x4F;

        // should keep this zero, or reuse for something else - using for jabberwocky
        public const byte WATER_PALACE_HOSTILITY_FLAG = 0x50;

        // 0x51 unused? the events where it appears aren't referenced
        // 0x52 also

        public const byte WITCH_CASTLE_SWITCHES_FLAG = 0x53;

        public const byte WITCH_CASTLE_WHIP_DIALOGUE_FLAG = 0x54;

        public const byte ICE_CASTLE_SWITCHES_FLAG_1 = 0x55;
        public const byte ICE_CASTLE_SWITCHES_FLAG_2 = 0x56;

        public const byte CANNON_TRAVEL_VISIBILITY_FLAG = 0x57;

        // use this for metal mantis.
        public const byte RABITEMAN_GOLD_FLAG = 0x58;

        // doesn't get much use
        public const byte WATERPALACE_JEMADIALOGUE_FLAG = 0x59;

        public const byte SOUTHTOWN_PASSWORD_ENTRY_FLAG = 0x5A;

        // doesn't get much use
        public const byte WATERPALACE_LUKADIALOGUE_FLAG = 0x5B;

        public const byte MIDGE_MALLET_FLAG = 0x5C;

        public const byte PURELANDS_INTRO_FLAG = 0x5D;

        public const byte WITCH_FOREST_AXE_UNLOCK = 0x5E;

        public const byte WITCH_FOREST_SLIDING_WALL_UNLOCK = 0x5F;

        public const byte GAIAS_NAVEL_LAVA_DRAIN_SWITCH = 0x60;

        // not actually sure what this is for
        public const byte GAIAS_NAVEL_CRYSTAL_ORB = 0x61;

        // 0x62-0x67 unused?
        public const byte CREDITS_FLAG_1 = 0x68;
        // 0x69-0x6e unused?

        public const byte TURTLE_ISLAND_NPC_VISIBILITY = 0x6F;
        public const byte OPEN_WORLD_CHRISTMAS_PROGRESS = 0x6F; // repurpose for gifts/reindeer mode

        // 0x70 seems to always be zero and there is some logic that depends on that?
        // 0x71, 0x72 also

        // 0x73-0x7e unused? - 73 to control timer on/off
        public const byte OPEN_WORLD_TIMED_MODE_ENABLE = 0x73;

        // 0x7f only used in event 8, which isn't used?
        // ^ seems to control whether the mana magic full animation plays

        // (reserved for saveram crap)
        // 0x80-0x8f not used - 0x80 for # bosses killed
        // using these for event logic to not double-give seeds - plus they count for game logic i think?
        // at the very least they determine how to render the seeds in the status menu
        // 0x90 - 0x97 mana seed power active
        public const byte WATER_SEED = 0x90;
        public const byte EARTH_SEED = 0x91;
        public const byte WIND_SEED = 0x92;
        public const byte FIRE_SEED = 0x93;
        public const byte LIGHT_SEED = 0x94;
        public const byte DARK_SEED = 0x95;
        public const byte MOON_SEED = 0x96;
        public const byte DRYAD_SEED = 0x97;

        // 0x98 - 0x9f element attacks for crystal orbs (see ElementSwaps)

        // 0xa0 - 0xa3 unused - use a0 for fire seed - didn't need this; use it for tonpole instead? since he previously used undine flag
        public const byte OPEN_WORLD_TONPOLE_FLAG = 0xA0;

        // use multiple if you have multiple doors on the map; reset on entry
        public const byte DOOR_CONTROL_FLAG_1 = 0xA4;
        public const byte DOOR_CONTROL_FLAG_2 = 0xA5;
        public const byte DOOR_CONTROL_FLAG_3 = 0xA6;
        public const byte DOOR_CONTROL_FLAG_4 = 0xA7;

        // seems to be used for whether or not you got them?
        // also visibility to make them flicker - removed
        // using these for event logic to not double-give elements
        public const byte ELEMENT_GNOME_FLAG = 0xA8;
        public const byte ELEMENT_UNDINE_FLAG = 0xA9;
        public const byte ELEMENT_SALAMANDO_FLAG = 0xAA;
        public const byte ELEMENT_SYLPHID_FLAG = 0xAB;
        public const byte ELEMENT_LUMINA_FLAG = 0xAC;
        public const byte ELEMENT_SHADE_FLAG = 0xAD;
        public const byte ELEMENT_LUNA_FLAG = 0xAE;
        public const byte ELEMENT_DRYAD_FLAG = 0xAF;

        // b0 - we'll use it for having moogle belt in open world
        // 0xb0 unused? 0xb1 in one watts sword scene, then 0xb2-0xb7 unused
        // b2 - manafort open
        // ^ b0-b7 actually used for something?  we moved these to 0x7x
        // some sort of weapon thing

        // 0xb8-0xbf weapon orbs gained, for chest visibility etc
        public const byte GLOVE_ORBS_FLAG_A = 0xB8;
        public const byte SWORD_ORBS_FLAG_A = 0xB9;
        public const byte AXE_ORBS_FLAG_A = 0xBA;
        public const byte SPEAR_ORBS_FLAG_A = 0xBB;
        public const byte WHIP_ORBS_FLAG_A = 0xBC;
        public const byte BOW_ORBS_FLAG_A = 0xBD;
        public const byte BOOMERANG_ORBS_FLAG_A = 0xBE;
        public const byte JAVELIN_ORBS_FLAG_A = 0xBF;

        // 0xc0-0xc7 - same except it's locked to 1-9?
        public const byte GLOVE_ORBS_FLAG_B = 0xC0;
        public const byte SWORD_ORBS_FLAG_B = 0xC1;
        public const byte AXE_ORBS_FLAG_B = 0xC2;
        public const byte SPEAR_ORBS_FLAG_B = 0xC3;
        public const byte WHIP_ORBS_FLAG_B = 0xC4;
        public const byte BOW_ORBS_FLAG_B = 0xC5;
        public const byte BOOMERANG_ORBS_FLAG_B = 0xC6;
        public const byte JAVELIN_ORBS_FLAG_B = 0xC7;

        // 0xc8-0xcf weapons
        public const byte HAVE_GLOVE = 0xC8;
        public const byte HAVE_SWORD = 0xC9;
        public const byte HAVE_AXE = 0xCA;
        public const byte HAVE_SPEAR = 0xCB;
        public const byte HAVE_WHIP = 0xCC;
        public const byte HAVE_BOW = 0xCD;
        public const byte HAVE_BOOMERANG = 0xCE;
        public const byte HAVE_JAVELIN = 0xCF;

        // 0xd0-0xe7 chest visibility; most are unused
        public const byte POTOS_CHEST_VISIBILITY_FLAG = 0xD0;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_D1 = 0xD1;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_D2 = 0xD2;
        // the other two chests are orbs and use the *_ORBS_FLAG_A flags above
        public const byte PANDORA_CHEST_VISIBILITY_FLAG_D3 = 0xD3; // lower right chest
        public const byte PANDORA_CHEST_VISIBILITY_FLAG_D4 = 0xD4; // upper left chest
        public const byte PANDORA_CHEST_VISIBILITY_FLAG_D5 = 0xD5; // upper middle chest
        public const byte PANDORA_CHEST_VISIBILITY_FLAG_D6 = 0xD6; // upper right chest
        public const byte MAGICROPE_CHEST_VISIBILITY_FLAG = 0xD7;
        public const byte NEXT_TO_WHIP_CHEST_VISIBILITY_FLAG = 0xD8;
        public const byte WHIP_CHEST_VISIBILITY_FLAG = 0xD9;
        public const byte FIREPALACE_CHEST_1_VISIBILITY_FLAG = 0xDA; // through one spell orb
        public const byte FIREPALACE_CHEST_2_VISIBILITY_FLAG = 0xDB; // through two spell orbs
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_DC = 0xDC;
        public const byte NTC_BASEMENT_CHEST_VISIBILITY_FLAG = 0xDD;
        public const byte POD_HALLWAY_CHEST_VISIBILITY_FLAG = 0xDE;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_DF = 0xDF;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_E0 = 0xE0;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_E1 = 0xE1;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_E2 = 0xE2;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_E3 = 0xE3;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_E4 = 0xE4;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_E5 = 0xE5;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_E6 = 0xE6;
        public const byte UNUSED_CHEST_VISIBILITY_FLAG_E7 = 0xE7;

        // 0xe8-0xef crystal orb rooms in lost continent
        public const byte GRANDPALACE_GNOME_ORB_FLAG = 0xE8;
        public const byte GRANDPALACE_UNDINE_ORB_FLAG = 0xE9;
        public const byte GRANDPALACE_SYLPHID_ORB_FLAG = 0xEA;
        public const byte GRANDPALACE_SALAMANDO_ORB_FLAG = 0xEB;
        public const byte GRANDPALACE_LUMINA_ORB_FLAG = 0xEC;
        public const byte GRANDPALACE_SHADE_ORB_FLAG = 0xED;
        public const byte GRANDPALACE_LUNA_ORB_FLAG = 0xEE;
        // EF unused?  there's no dryad orb
        public static byte[] lostContinentElementOrbFlags = 
            new byte[] { GRANDPALACE_GNOME_ORB_FLAG, GRANDPALACE_UNDINE_ORB_FLAG, GRANDPALACE_SYLPHID_ORB_FLAG, GRANDPALACE_SALAMANDO_ORB_FLAG,
                GRANDPALACE_LUMINA_ORB_FLAG, GRANDPALACE_SHADE_ORB_FLAG, GRANDPALACE_LUNA_ORB_FLAG };

        // 0xf0-0xff special shit - 
        // f7 is something to do with the credits
        // f8 makes dialogue have no box
        public const byte DIALOGUE_NO_BORDER_FLAG = 0xF8;
        // f9 is some kind of marker for how much gold you spent in dialogue
        // fa seems to indicate the ID of the shop that will open
        public const byte BUY_SHOP_ID_FLAG = 0xFA;
        // fc is total mana power (max spell levels, and also it factors into damage a bit)
        public const byte TOTAL_MANA_POWER_FLAG = 0xFC;
        // fd, fe - something to do with map loading? they are cleared on every load

        // set by some boss intro events to make the boss just sit there until the dialogue finishes
        public const byte FREEZE_BOSS_AI_FLAG = 0xFF;

        public const byte OPENWORLD_MOOGLE_BELT_FLAG = 0x74;
        public const byte OPENWORLD_MANAFORT_ACCESSIBLE_FLAG = 0x75;
        public static byte[] OPENWORLD_GOLD_FLAGS = new byte[] {
            0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, // previously gold drop flags
        };

        // for weapon orb prizes in open world
        public static byte[] OPENWORLD_ORB_FLAGS = new byte[] {
            0x04, 0x05, 0x06, 0x07, 0x08, 0x1F, 0x23, 0x3d, 0x3e, 0x51, 0x52, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67,
            0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e,
            // gold, and other open world items
            0xA1, 0xA2, 0xA3,
            // formerly chest flags
            0xD3, 0xD4, 0xD5, 0xD6, 0xD0, 0xD7, 0xDA, 0xDB, 0xDD, 0xDE,
            0xdc, 0xdf, 0xe0, 0xe1, 0xe2
        };

        // MORE FREE FLAGS: 09, 2A, 33; 4E, 4F if no manafort
        // potentials: 0B (death type - replace event to not look at it)
        // * 31 (just move the dude)
        // * 59 (remove jema on map 139? so event 212 can't be called.. his visibility is 1E 5-7 and we set it to 7 .. actually we already remove him, this one should be fine)
        // * 5A - just needed to remove password guy logic
        public static byte[] XMAS_EXTRA_FLAGS = new byte[] { 0x09, 0x2A, 0x31, 0x33, 0x4E, 0x4F, 0x59, 0x5A, };

        // 76-7e for gold now? 9 of them

        // C8-CF for have gotten weapon flag?  can't use B8-BF or C0-C7
        // what's setting CD? oh the gold drops.. hmm
    }
}
