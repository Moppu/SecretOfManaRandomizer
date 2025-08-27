using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.ancientcave.mapgen
{
    /// <summary>
    /// Map palette set loaded from the vanilla ROM.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaPaletteSetSource : PaletteSetSource
    {
        private int vanillaPaletteSetIndex;
        public VanillaPaletteSetSource(int vanillaPaletteSetIndex)
        {
            this.vanillaPaletteSetIndex = vanillaPaletteSetIndex;
        }

        public MapPaletteSet getPaletteData(RandoContext context)
        {
            return VanillaPaletteUtil.getMapPaletteSet(context.originalRom, vanillaPaletteSetIndex);
        }

        public int getVanillaPaletteSet()
        {
            return vanillaPaletteSetIndex;
        }
    }
}
