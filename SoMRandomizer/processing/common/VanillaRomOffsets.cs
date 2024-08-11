
namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Offsets of various bits of data in the vanilla ROM.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaRomOffsets
    {
        // 24-bit offsets to music data; 59 of them total
        public const int MUSIC_OFFSETS = 0x33D39;

        // 24-bit offsets to music sample data; 33 of them total
        public const int SAMPLE_OFFSETS = 0x33DF9;

        // offset to music sample loop data; 33 of them total
        public const int SAMPLE_LOOPS_OFFSET = 0x33E5C;

        // offset to music sample base frequency data; 33 of them total
        public const int SAMPLE_BASEFREQS_OFFSET = 0x33E9E;

        // offset to music sample ADSR data; 33 of them total
        public const int SAMPLE_ADSRS_OFFSET = 0x33EE0;

        // 16-bit pointers into bank 6 and 7 (with automatic rollover, i think?) for 512 RLE rows of world map data
        public const int WORLDMAP_ROWS_OFFSETS = 0x68000;

        // code used to render the title screen; compressed with LZ77.  See TitleScreenAddition.
        public const int TITLE_SCREEN_COMPRESSED_CODE_OFFSET = 0x77C00;

        // compressed block of tile arrangements for title screen; compressed with LZ77.  See TitleScreenAddition.
        public const int TITLE_SCREEN_COMPRESSED_TILE_PLACEMENTS = 0x7B480;

        // 4 byte doors between maps; see Door structure
        // 0x400 total (through 0x83FFF)
        public const int DOORS = 0x83000;

        // 16-bit pointers to trigger lists for each fullmap, within bank
        // 0x200 total (through 0x843FF)
        public const int MAP_TRIGGER_OFFSETS = 0x84000;

        // 16-bit pointers to map piece lists for each fullmap, within bank
        // 0x200 total (through 0x853FF)
        public const int MAP_PIECE_REFERENCE_OFFSETS = 0x85000;

        // 16-bit pointers to object lists for each fullmap, within bank
        // 0x200 total (through 0x873FF)
        public const int MAP_OBJECT_OFFSETS = 0x87000;

        // events 0x000 - 0x3ff
        public const int EVENTS_FIRST_BANK = 0x90000;
        // events 0x400 - 0x7ff
        public const int EVENTS_SECOND_BANK = 0xA0000;

        // 0x180 bytes per tileset
        public const int COLLISION_TABLE_START = 0xB0400;

        // location of tilesets; see VanillaTilesetUtil
        public const int TILESET16_OFFSETS = 0xB4000;

        // 30 * 7 bytes per palette set, and i think like 120 of them altogether
        public const int MAP_PALETTE_SETS_OFFSET = 0xC8000;

        // 16-bit pointers to map piece data
        // organized into several banks, as indicated by the few bytes before this
        // map piece 0 -> 0xA2 in bank 0x0D
        // map piece 0xA3 -> 0x205 in bank 0x0E
        // map piece 0x206 -> 0x300 in bank 0x0F
        // map piece 0x300 -> 0x3FF in bank 0x0C
        // data at these offsets is compressed; see SomMapCompressor for routines
        // MapPieceExpander re-organizes these into 24-bit offsets for expandability of map data
        public const int MAP_PIECE_OFFSETS = 0xD0004;

        // get the offset of the start of the bank containing the given offset
        public static int getBankStart(int romOffset)
        {
            return romOffset & 0xFF0000;
        }
    }
}
