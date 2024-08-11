using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Some minor adjustments to the world map to make it easier to land at Ice Country.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class IceCountryLandableFixes : RandoProcessor
    {
        protected override string getName()
        {
            return "Ice country landability changes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // world map landables to make ice country more tolerable
            // x67600
            // do this before xmas stuff because it changes it a little bit
            // this is sorted by map quadrant (16 total) and seems to have 0x18 bytes (0xc0 bits) per section
            for (int i = 0; i < 0x18; i++)
            {
                outRom[0x67630 + i] = outRom[0x67600 + i];
            }
            // also change to make landable: BB, 30
            byte[] tileIndexes = new byte[] { 0x30, 0xBB, 0xBA };
            foreach (byte tileIndex in tileIndexes)
            {
                outRom[0x67630 + (tileIndex / 8)] &= (byte)(~(1 << (tileIndex % 8)));
            }
            return true;
        }
    }
}
