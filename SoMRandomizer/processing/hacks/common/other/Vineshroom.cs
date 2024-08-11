using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// A minor change to the mushroom enemy/npc to make it look like a Vinesauce vineshroom,
    /// for really no other reason than it already looked pretty similar.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class Vineshroom : RandoProcessor
    {
        protected override string getName()
        {
            return "Mushroom enemies become vineshrooms";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // 190500 -> 191300
            int offset = 0x190500;
            byte[] resourceData = DataUtil.readResource("SoMRandomizer.Resources.vineshroom_tiles.bin");

            for (int i = 0; i < resourceData.Length; i++)
            {
                outRom[offset + i] = resourceData[i];
            }
            return true;
        }
    }
}
