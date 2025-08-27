using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Hack that changes max of a consumable from 4 to 7.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MaxItemsIncrease : RandoProcessor
    {
        protected override string getName()
        {
            return "Increase max consumables to 7";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_MAX_7_ITEMS))
            {
                return false;
            }

            // $C0/6559 C9 9F       CMP #$9F                A:00A3 X:004B Y:0003 P:eNvMxdIzc - remove check, allowing higher values
            // $C0/655B B0 EA BCS $EA[$6547]                A:00A3 X:004B Y:0003 P:envMxdIzC
            outRom[0x6559] = 0xEA;
            outRom[0x655A] = 0xEA;
            return true;            
        }
    }
}
