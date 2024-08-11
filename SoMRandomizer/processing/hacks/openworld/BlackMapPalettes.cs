using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using static SoMRandomizer.processing.hacks.common.util.CharacterPaletteRandomizer;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Hack that blacks out all map palettes to make it difficult to navigate.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BlackMapPalettes : RandoProcessor
    {
        protected override string getName()
        {
            return "Black-out all map palettes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(OpenWorldSettings.PROPERTYNAME_OBSCURE_MAP_DATA))
            {
                return false;
            }

            int palStartOffset = 0xC7FE0;
            // for all palette sets that are used; i think the rest are not
            for (byte paletteSet = 24; paletteSet < 120; paletteSet++)
            {
                for (int palnum = 1; palnum < 8; palnum++)
                {
                    for (int col = 1; col < 16; col++)
                    {
                        // extract color; set r/g/b to 0; put it back
                        byte readPalSet = paletteSet;
                        byte p1 = outRom[palStartOffset + readPalSet * 30 * 7 + palnum * 30 + col * 2];
                        byte p2 = outRom[palStartOffset + readPalSet * 30 * 7 + palnum * 30 + col * 2 + 1];
                        SnesColor color = new SnesColor(new byte[] { p1, p2 });
                        color.setRed(0);
                        color.setGreen(0);
                        color.setBlue(0);
                        color.put(outRom, palStartOffset + paletteSet * 30 * 7 + palnum * 30 + col * 2);
                    }
                }
            }

            // displayset bg colors = black
            for(int dispSettingNum = 0; dispSettingNum < 64; dispSettingNum++)
            {
                outRom[0x80100 + dispSettingNum * 9 + 7] = 0;
                outRom[0x80100 + dispSettingNum * 9 + 8] = 0;
            }

            // world map
            int[] worldMapPalOffsets = new int[] { 0x67DFE, 0x6483E, };
            foreach (int worldMapPalOffset in worldMapPalOffsets)
            {
                // 0 = clouds
                // 1 = sky low
                // 2 = sky med
                // 3 = sky high
                // 4 = sky highest
                // 5,6,7,8,9,10,11,12 = tiles
                for (int palnum = 1; palnum < 13; palnum++)
                {
                    for (int col = 1; col < 16; col++)
                    {
                        byte p1 = outRom[worldMapPalOffset + palnum * 30 + col * 2];
                        byte p2 = outRom[worldMapPalOffset + palnum * 30 + col * 2 + 1];
                        SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });

                        thisCol.setRed(0);
                        thisCol.setGreen(0);
                        thisCol.setBlue(0);

                        thisCol.put(outRom, worldMapPalOffset + palnum * 30 + col * 2);
                    }
                }
            }
            return true;
        }
    }
}
