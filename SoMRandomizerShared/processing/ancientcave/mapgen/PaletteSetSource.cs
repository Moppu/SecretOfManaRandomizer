using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.ancientcave.mapgen
{
    /// <summary>
    /// A method of loading a palette set.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public interface PaletteSetSource
    {
        MapPaletteSet getPaletteData(RandoContext context);
        int getVanillaPaletteSet();
    }
}
