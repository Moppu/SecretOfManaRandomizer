using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// A simple hack to cut the treasure chest animation short.
    /// </summary>
    /// 
    /// <remarks>Author: Zhade</remarks>
    public class ChestOpeningSpeedup : RandoProcessor
    {
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_FASTER_CHESTS))
            {
                return false;
            }

            // from zhadeth's research:
            // "Ok got chests to open faster, it indeed is waiting for animations to finish, both the character and the chest. To skip it, put FF (anim code for 'End of animation') at these offsets:
            // D15FB8(PCs: Breaking Lock)
            // D1ED01(Chest: Lock Beign Broken(Left / Right))
            // D1EC84(Chest: Lock Beign Broken(Up / Down)) "

            outRom[0x115FB8] = 0xFF;
            outRom[0x11ED01] = 0xFF;
            outRom[0x11EC84] = 0xFF;

            return true;
        }

        protected override string getName()
        {
            return "Chests open without shaking";
        }
    }
}
