using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.ancientcave
{
    /// <summary>
    /// Hack to change one door type to return doors for mana fort interior tileset to simplify ancient cave generation.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AcManafortTilesetChanges : RandoProcessor
    {
        protected override string getName()
        {
            return "Changes to manafort interior tileset for ancient cave";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            int tilesetNum = 21;
            // layer1 x3C becomes return type collision (was trigger)
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 0x3C] = 0x16;

            return true;
        }
    }
}
