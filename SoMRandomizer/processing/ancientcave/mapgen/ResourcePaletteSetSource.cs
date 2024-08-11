using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.ancientcave.mapgen
{
    /// <summary>
    /// Map palette set loaded from an embedded resource.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ResourcePaletteSetSource : PaletteSetSource
    {
        private string resourceName;
        private int baseVanillaPalette;
        public ResourcePaletteSetSource(string resourceName, int baseVanillaPalette)
        {
            this.resourceName = resourceName;
            // animation settings are pulled from here
            this.baseVanillaPalette = baseVanillaPalette;
        }

        public MapPaletteSet getPaletteData(RandoContext context)
        {
            byte[] palData = DataUtil.readResource("SoMRandomizer.Resources.mapPalettes." + resourceName);
            return new MapPaletteSet(palData);
        }

        public int getVanillaPaletteSet()
        {
            return baseVanillaPalette;
        }
    }
}
