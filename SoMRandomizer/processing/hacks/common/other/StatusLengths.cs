using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Increase lengths of status conditions (both positive and negative) as vanilla's were pretty short.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class StatusLengths : RandoProcessor
    {
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_STATUS_LENGTHS))
            {
                return false;
            }
            
            // vanilla loading the timer for statuses and &ing it with 0x03 to determine when to tick down individual
            // status timers.  modify the constant we "and" by to extend status timers
            //    $C0/3C38 BD B9 E1    LDA $E1B9,x[$7E:E3B9]   A:0000 X:0200 Y:0056 P:envMxdIzC
            //    $C0/3C3B 29 03       AND #$03                A:0074 X:0200 Y:0056 P:envMxdIzC
            //    $C0/3C3D D0 48       BNE $48    [$3C87]

            // quadruple status tick length
            outRom[0x3C3C] = 0x0F;
            // half weapon-initiated status lengths (which were quadrupled)
            outRom[0x8E3C1] = 0x0A;

            return true;
        }

        protected override string getName()
        {
            return "Extend status lengths";
        }
    }
}
