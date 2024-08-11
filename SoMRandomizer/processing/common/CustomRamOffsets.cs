
namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Custom RAM locations where rando keeps data.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CustomRamOffsets
    {
        // /////////////////////
        // Ancient cave statistics
        // /////////////////////
        // block seems untouched; 0x55 forever; can probably use for whatever
        public static int STATISTICS_BASE_OFFSET = 0x7E0400;

        // 00
        public static int PLAYER_KILLS_OFFSET_16BIT = STATISTICS_BASE_OFFSET + 0;
        // 02
        public static int ENEMY_KILLS_OFFSET_16BIT = STATISTICS_BASE_OFFSET + 2;
        // 04
        public static int NUM_CHESTS_OFFSET_16BIT = STATISTICS_BASE_OFFSET + 4;
        // 06
        public static int MOVE_DIST_32BIT = STATISTICS_BASE_OFFSET + 6;
        // 0a
        public static int TOTAL_DAMAGE_DEALT_32BIT = STATISTICS_BASE_OFFSET + 10;
        // 0e
        public static int TOTAL_P_DAMAGE_DEALT_32BIT = STATISTICS_BASE_OFFSET + 14;
        // 12
        public static int TOTAL_M_DAMAGE_DEALT_32BIT = STATISTICS_BASE_OFFSET + 18;
        // 16
        public static int TOTAL_DAMAGE_TAKEN_32BIT = STATISTICS_BASE_OFFSET + 22;
        // 1a
        public static int TOTAL_P_DAMAGE_TAKEN_32BIT = STATISTICS_BASE_OFFSET + 26;
        // 1e
        public static int TOTAL_M_DAMAGE_TAKEN_32BIT = STATISTICS_BASE_OFFSET + 30;

        // 22
        public static int FREE_MOVE_PLAYER_SELECT_BYTE = STATISTICS_BASE_OFFSET + 34;
        public static int FREE_MOVE_PLAYER1_COUNTER_16BIT = STATISTICS_BASE_OFFSET + 35;
        public static int FREE_MOVE_PLAYER2_COUNTER_16BIT = STATISTICS_BASE_OFFSET + 37;
        public static int FREE_MOVE_PLAYER3_COUNTER_16BIT = STATISTICS_BASE_OFFSET + 39;

        // x3, one per character
        public static int STATUSGLOW_STATUS_BYTE_1 = STATISTICS_BASE_OFFSET + 41;
        // x3, one per character
        public static int STATUSGLOW_STATUS_BYTE_2 = STATISTICS_BASE_OFFSET + 44;

        public static int MUSICRANDO_LASTSONG_BYTE = STATISTICS_BASE_OFFSET + 47;
        // next: +48
        public static int LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_A_16BIT = STATISTICS_BASE_OFFSET + 48;
        // next: +50
        public static int LEVELMATCH_CURRENT_MAX_LEVEL_OFFSET_B_16BIT = STATISTICS_BASE_OFFSET + 50;
        // next: +52
        public static int LEVELMATCH_CURRENT_MAX_LEVEL_8BIT = STATISTICS_BASE_OFFSET + 52;
        // next: +53
        public static int TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT = STATISTICS_BASE_OFFSET + 53;
        // next: +56
        public static int TIMED_LEVEL_VALUE_8BIT = STATISTICS_BASE_OFFSET + 56;
        // next: +57
        public static int DONT_AUTOSAVE = STATISTICS_BASE_OFFSET + 57;
        // next: +58
        public static int X_SCREEN_TEMP_LOCATION = STATISTICS_BASE_OFFSET + 58; // for FreeTargetting
        // next: +59
        public static int Y_SCREEN_TEMP_LOCATION = STATISTICS_BASE_OFFSET + 59;
        // next: +60
        public static int CONSUMABLE_BUY_MAX = STATISTICS_BASE_OFFSET + 60;
        // next: +61
        public static int CONSUMABLE_BUY_CURRENT = STATISTICS_BASE_OFFSET + 61;
        // next: +62
        public static int CONSUMABLE_INPUT_TIMER = STATISTICS_BASE_OFFSET + 62;
        // next: +63
        public static int BUYING_MULTIPLE_ITEMS = STATISTICS_BASE_OFFSET + 63;
        // next: +64
        public static int OPENWORLD_CURRENT_ENEMY_LEVEL = STATISTICS_BASE_OFFSET + 64;
        // next: +65

        // /////////////////////
        // Open world enemy level stuff
        // /////////////////////

        // note the handling for this also corrupts x81
        public static int LEVELMATCH_BOSSES_KILLED = 0x7ECF80; // event flag 0x80
        public static int LEVELMATCH_TIMER_FRAMES = 0x7ECF82; // event flag 0x82
        public static int LEVELMATCH_TIMER_SECONDS_LOW = 0x7ECF84; // event flag 0x84
        public static int LEVELMATCH_TIMER_SECONDS_HIGH = 0x7ECF86; // event flag 0x86

        public static int MESSAGE_TEMPORARY_BUFFER = 0x7E0AC0; // idk looks empty

    }
}
