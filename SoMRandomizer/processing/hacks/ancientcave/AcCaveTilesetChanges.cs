using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.ancientcave
{
    /// <summary>
    /// Hack to make a few decorative rock tiles in the cave tileset passable so they can be used more freely in ancient cave generations.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AcCaveTilesetChanges : RandoProcessor
    {
        protected override string getName()
        {
            return "Changes to cave tileset for ancient cave";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            int tilesetNum = 10;

            // make a few decorative tiles passable for more interesting looking caves
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 29] = 8;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 45] = 8;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 169] = 8;

            return true;
        }
    }
}
