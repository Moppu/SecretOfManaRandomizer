using SoMRandomizer.util;

namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// 8-byte map header that appears as the first "object" on every map.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MapHeader : VanillaStructure
    {
        // byte 0 bits 0xC0 are unknown
        private const int ANIMATED_TILES_BYTE = 0;
        private const int ANIMATED_TILES_BITS = 0x80; // set to enable animation; disable for frozen
        private const int UNKNOWN_TILE_FLAG_BYTE = 0;
        private const int UNKNOWN_TILE_FLAG_BITS = 0x40; // something to do with the animated tiles; unsure
        private const int TILESET8_BYTE = 0;
        private const int TILESET8_BITS = 0x3F;
        private const int PALETTE_ANIMATED_BYTE = 1;
        private const int PALETTE_ANIMATED_BITS = 0x80;
        private const int PALSET_BYTE = 1;
        private const int PALSET_BITS = 0x7F;
        private const int LAYER2_OFFSET_BYTE = 2;
        private const int LAYER2_OFFSET_BITS = 0x80;
        private const int LAYER1_OFFSET_BYTE = 2;
        private const int LAYER1_OFFSET_BITS = 0x40;
        private const int TILESET16_BYTE = 2;
        private const int TILESET16_BITS = 0x3F;
        private const int MODE7_BYTE = 3;
        private const int MODE7_BITS = 0xF8;
        private const int WALKON_EVENT_BYTE = 3;
        private const int WALKON_EVENT_BITS = 0x04;
        // byte 3 bit 0x02 is unknown
        private const int SPECIAL_EVENT_BYTE = 3;
        private const int SPECIAL_EVENT_BITS = 0x01;
        private const int FLAMMIE_ENABLED_BYTE = 4;
        private const int FLAMMIE_ENABLED_BITS = 0x80;
        private const int MAGICROPE_ENABLED_BYTE = 4;
        private const int MAGICROPE_ENABLED_BITS = 0x40;
        private const int BG_SCROLLING_BYTE = 4;
        private const int BG_SCROLLING_BITS = 0x3F;
        private const int DISPLAY_SETTINGS_BYTE = 5;
        private const int DISPLAY_SETTINGS_BITS = 0xFF;
        // byte 6 is unknown .. not sure vanilla ever uses it?
        private const int NPC_PALETTE_BYTE = 7;
        private const int NPC_PALETTE_BITS = 0xFF;
        public MapHeader() : base(new byte[8]) { }
        public MapHeader(byte[] headerValue) : base(headerValue) { }

        public bool getAnimatedTiles()
        {
            return get(ANIMATED_TILES_BYTE, ANIMATED_TILES_BITS) > 0;
        }

        public void setAnimatedTiles(bool animatedTiles)
        {
            set(ANIMATED_TILES_BYTE, ANIMATED_TILES_BITS, (byte)(animatedTiles ? 1 : 0));
        }

        public bool getUnknownTileFlag()
        {
            return get(UNKNOWN_TILE_FLAG_BYTE, UNKNOWN_TILE_FLAG_BITS) > 0;
        }

        public void setUnknownTileFlag(bool unknownTileFlag)
        {
            set(UNKNOWN_TILE_FLAG_BYTE, UNKNOWN_TILE_FLAG_BITS, (byte)(unknownTileFlag ? 1 : 0));
        }

        public bool getPaletteAnimated()
        {
            return get(PALETTE_ANIMATED_BYTE, PALETTE_ANIMATED_BITS) > 0;
        }

        public void setPaletteAnimated(bool paletteAnimated)
        {
            set(PALETTE_ANIMATED_BYTE, PALETTE_ANIMATED_BITS, (byte)(paletteAnimated ? 1 : 0));
        }

        // extract palette set index for composite map, from map header
        public byte getPaletteSet()
        {
            return get(PALSET_BYTE, PALSET_BITS);
        }

        public void setPaletteSet(byte palSet)
        {
            set(PALSET_BYTE, PALSET_BITS, palSet);
        }

        // extract 8x8 tileset index for composite map, from map header
        public byte getTileset8()
        {
            return get(TILESET8_BYTE, TILESET8_BITS);
        }

        public void setTileset8(byte tileset8)
        {
            set(TILESET8_BYTE, TILESET8_BITS, tileset8);
        }

        // extract 16x16 tileset index for composite map, from map header
        public byte getTileset16()
        {
            return get(TILESET8_BYTE, TILESET8_BITS);
        }

        public void setTileset16(byte tileset16)
        {
            set(TILESET16_BYTE, TILESET16_BITS, tileset16);
        }

        // true if layer1 uses the second half of the tileset16 (384 total tiles)
        public bool getLayer1UsesForegroundTiles()
        {
            return get(LAYER1_OFFSET_BYTE, LAYER1_OFFSET_BITS) > 0;
        }

        public void setLayer1UsesForegroundTiles(bool layer1Foreground)
        {
            set(LAYER1_OFFSET_BYTE, LAYER1_OFFSET_BITS, (byte)(layer1Foreground ? 1 : 0));
        }

        // true if layer2 uses the second half of the tileset16 (384 total tiles)
        public bool getLayer2UsesForegroundTiles()
        {
            return get(LAYER2_OFFSET_BYTE, LAYER2_OFFSET_BITS) > 0;
        }

        public void setLayer2UsesForegroundTiles(bool layer2Foreground)
        {
            set(LAYER2_OFFSET_BYTE, LAYER2_OFFSET_BITS, (byte)(layer2Foreground ? 1 : 0));
        }

        // true if mode 7 map, like mana beast or slime bosses
        public bool getMode7Enabled()
        {
            return get(MODE7_BYTE, MODE7_BITS) > 0;
        }

        public void setMode7Enabled(bool mode7Enabled)
        {
            set(MODE7_BYTE, MODE7_BITS, (byte)(mode7Enabled ? MODE7_BITS : 0));
        }

        // true if the first event in the list should be executed when entering the map, and object events should start at the next one
        public bool getWalkonEventEnabled()
        {
            return get(WALKON_EVENT_BYTE, WALKON_EVENT_BITS) > 0;
        }

        public void setWalkonEventEnabled(bool walkonEventEnabled)
        {
            set(WALKON_EVENT_BYTE, WALKON_EVENT_BITS, (byte)(walkonEventEnabled ? 1 : 0));
        }

        // true if the first event in the list (or next after walk-on, if it's enabled) should be executed when stepping on tiles of a certain collision type, and object events should start at the next one
        public bool getSpecialEventEnabled()
        {
            return get(SPECIAL_EVENT_BYTE, SPECIAL_EVENT_BITS) > 0;
        }

        public void setSpecialEventEnabled(bool specialEventEnabled)
        {
            set(SPECIAL_EVENT_BYTE, SPECIAL_EVENT_BITS, (byte)(specialEventEnabled ? 1 : 0));
        }

        // true if flammie drum can be used on map
        public bool getFlammieEnabled()
        {
            return get(FLAMMIE_ENABLED_BYTE, FLAMMIE_ENABLED_BITS) > 0;
        }

        public void setFlammieEnabled(bool flammieEnabled)
        {
            set(FLAMMIE_ENABLED_BYTE, FLAMMIE_ENABLED_BITS, (byte)(flammieEnabled ? 1 : 0));
        }

        // true if magic rope can be used on map
        public bool getMagicRopeEnabled()
        {
            return get(MAGICROPE_ENABLED_BYTE, MAGICROPE_ENABLED_BITS) > 0;
        }

        public void setMagicRopeEnabled(bool magicRopeEnabled)
        {
            set(MAGICROPE_ENABLED_BYTE, MAGICROPE_ENABLED_BITS, (byte)(magicRopeEnabled ? 1 : 0));
        }

        // settings for movement of background layers
        // i think this might be an index into another table somewhere but i don't remember
        public byte getBgScrolling()
        {
            return get(BG_SCROLLING_BYTE, BG_SCROLLING_BITS);
        }

        public void setBgScrolling(byte bgScrolling)
        {
            set(BG_SCROLLING_BYTE, BG_SCROLLING_BITS, bgScrolling);
        }

        // index into table of display settings for bg layers enabled, transparency, mosaic effect, and other stuff
        public byte getDisplaySetting()
        {
            return get(DISPLAY_SETTINGS_BYTE, DISPLAY_SETTINGS_BITS);
        }

        public void setDisplaySettings(byte displaySettings)
        {
            set(DISPLAY_SETTINGS_BYTE, DISPLAY_SETTINGS_BITS, displaySettings);
        }

        // palette index for NPCs on this map, or 0xFF for hostile map
        public byte getNpcPalette()
        {
            return get(NPC_PALETTE_BYTE, NPC_PALETTE_BITS);
        }

        public void setNpcPalette(byte npcPalette)
        {
            set(NPC_PALETTE_BYTE, NPC_PALETTE_BITS, npcPalette);
        }

        public override string ToString()
        {
            return "[Map Header: " +
                "t8=" + getTileset8().ToString("X2") + "; " +
                "t16=" + getTileset16().ToString("X2") + "; " +
                "pal=" + getPaletteSet().ToString("X2") + "; " +
                "walkon=" + getWalkonEventEnabled() + "; " +
                "spec=" + getSpecialEventEnabled() + "; " +
                "L1fg=" + getLayer1UsesForegroundTiles() + "; " +
                "L2fg=" + getLayer2UsesForegroundTiles() + "; " +
                "<" + DataUtil.byteArrayToHexString(getRomValue()) + ">" +
                "]";
        }
    }
}
