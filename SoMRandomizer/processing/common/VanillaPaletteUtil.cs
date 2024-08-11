using SoMRandomizer.processing.common.structure;
using System;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Utilities for loading and manipulating vanilla palettes.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaPaletteUtil
    {
        // get the specified vanilla palette set - seven 15-color palettes
        public static MapPaletteSet getMapPaletteSet(byte[] rom, int paletteSetIndex)
        {
            // 0xC8000 in vanilla ROM, 0xD2 bytes each
            byte[] palData = new byte[30 * 7];
            Array.Copy(rom, paletteSetIndex * 30 * 7 + VanillaRomOffsets.MAP_PALETTE_SETS_OFFSET, palData, 0, palData.Length);
            return new MapPaletteSet(palData);
        }
    }
}
