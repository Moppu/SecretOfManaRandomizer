using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.hacks.common.util
{
    /// <summary>
    /// Replace specified map palette sets with generated ones.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MapPaletteSetReplacer
    {
        const int paletteSetOffset = 0xC8000;

        public void process(RandoContext context)
        {
            foreach(int palIndex in context.replacementMapPalettes.Keys)
            {
                MapPaletteSet replacementPalette = context.replacementMapPalettes[palIndex];
                replacementPalette.put(context.outputRom, paletteSetOffset + palIndex * 30 * 7);
            }
        }
    }
}
