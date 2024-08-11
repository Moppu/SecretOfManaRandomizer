using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// I don't remember what this does but apparently all the procgen modes use it.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DeathEventDoorwayRemoval : RandoProcessor
    {
        protected override string getName()
        {
            return "Death event change for non-vanilla modes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // i don't remember what this was for. some display issue when dying in ancient cave etc. it takes out the door 0x75 transition
            // in vanilla event 0x7ff, which is called when the party wipes
            outRom[0xA98AF] = 0x2F;
            outRom[0xA98B0] = 0x00;
            return true;
        }
    }
}
