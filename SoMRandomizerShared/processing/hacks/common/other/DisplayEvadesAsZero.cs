using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that displays some hidden damage/evades as zeros for better visibility of having hit a target's hitbox.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DisplayEvadesAsZero : RandoProcessor
    {
        protected override string getName()
        {
            return "Display zero damage for evades";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_SHOW_EVADES_AS_ZERO))
            {
                return false;
            }

            // MOPPLE: this doesn't show misses/evades as zero in all cases.  needs more work!

            // relevant block of code here.  set carry to indicate display on return.
            /*
$C0/50AB 8F 02 42 00 STA $004202[$00:4202]   A:0FF5 X:0600 Y:0200 P:envMxdIzC
$C0/50AF A5 8E       LDA $8E    [$00:038E]   A:0FF5 X:0600 Y:0200 P:envMxdIzC
$C0/50B1 4A          LSR A                   A:0F32 X:0600 Y:0200 P:envMxdIzC
$C0/50B2 8F 03 42 00 STA $004203[$00:4203]   A:0F19 X:0600 Y:0200 P:envMxdIzc
$C0/50B6 85 A4       STA $A4    [$00:03A4]   A:0F19 X:0600 Y:0200 P:envMxdIzc
$C0/50B8 EA          NOP                     A:0F19 X:0600 Y:0200 P:envMxdIzc
$C0/50B9 EA          NOP                     A:0F19 X:0600 Y:0200 P:envMxdIzc
$C0/50BA EA          NOP                     A:0F19 X:0600 Y:0200 P:envMxdIzc
$C0/50BB EA          NOP                     A:0F19 X:0600 Y:0200 P:envMxdIzc
$C0/50BC AF 17 42 00 LDA $004217[$00:4217]   A:0F19 X:0600 Y:0200 P:envMxdIzc
$C0/50C0 18          CLC                     A:0F17 X:0600 Y:0200 P:envMxdIzc
$C0/50C1 65 A4       ADC $A4    [$00:03A4]   A:0F17 X:0600 Y:0200 P:envMxdIzc
$C0/50C3 85 A4       STA $A4    [$00:03A4]   A:0F30 X:0600 Y:0200 P:envMxdIzc
$C0/50C5 A5 8D       LDA $8D    [$00:038D]   A:0F30 X:0600 Y:0200 P:envMxdIzc
$C0/50C7 C5 A4       CMP $A4    [$00:03A4]   A:0F2B X:0600 Y:0200 P:envMxdIzc
$C0/50C9 60          RTS                     A:0F2B X:0600 Y:0200 P:eNvMxdIzc

             */
            // - 0 damage display for boss evades? or dodge sound? 50c7 -> 38 (sec), 50c8 -> ea (nop)
            outRom[0x50c7] = 0x38;
            outRom[0x50c8] = 0xEA;
            
            return true;
        }
    }
}
