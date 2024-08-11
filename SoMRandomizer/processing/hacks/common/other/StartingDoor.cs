using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that changes the door executed when you start a new game, for certain modes that require it.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class StartingDoor : RandoProcessor
    {
        protected override string getName()
        {
            return "Change starting door for non-vanilla mode";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            if (randoMode == AncientCaveSettings.MODE_KEY)
            {
                // start at door 0x0A
                outRom[0x751FD] = 0x0A;
            }
            else if (randoMode == BossRushSettings.MODE_KEY)
            {
                // start at door 0x0A
                outRom[0x751FD] = 0x0A;
            }
            else if (randoMode == ChaosSettings.MODE_KEY)
            {
                // start at door 0x0A
                outRom[0x751FD] = 0x0A;
            }
            else
            {
                Logging.log("Unsupported mode for starting door change");
                return false;
            }
            return true;
        }
    }
}
