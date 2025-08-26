using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.hacks.common.fix
{
    /// <summary>
    /// Hack that removes the ability to cancel damage to a player by using items on them.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DamageCancelFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Fix damage canceling";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // allowType == "consumables" or "none" (or "all" to not call this)
            string allowType = settings.get(CommonSettings.PROPERTYNAME_DAMAGE_CANCEL_ALLOW_TYPE);
            if(allowType == "all")
            {
                // vanilla behavior; change nothing
                return false;
            }
            else if(allowType == "consumables")
            {
                Logging.log("Allowing damage canceling for consumables only");
            }
            /*
            $C0/550C 9E F1 E1    STZ $E1F1,x[$7E:E1F1]   A:0010 X:0000 Y:0000 P:envMxdIZc ** - 0x10 for 0x08 (moogle belt) * 2
            $C0/550F 9E F2 E1    STZ $E1F2,x[$7E:E1F2]   A:0010 X:0000 Y:0000 P:envMxdIZc **
            $C0/5512 9E 59 E0    STZ $E059,x[$7E:E059]   A:0010 X:0000 Y:0000 P:envMxdIZc - nfi but the other two are the pending damage
            $C0/5515 AA          TAX                     A:0010 X:0000 Y:0000 P:envMxdIZc
            $C0/5516 0A          ASL A                   A:0010 X:0010 Y:0000 P:envMxdIzc
            $C0/5517 0A          ASL A                   A:0020 X:0010 Y:0000 P:envMxdIzc
            $C0/5518 0A          ASL A                   A:0040 X:0010 Y:0000 P:envMxdIzc
            $C0/5519 7C 1C 55    JMP ($551C,x)[$C0:55B2] A:0080 X:0010 Y:0000 P:eNvMxdIzc 
            */
            if(allowType == "none")
            {
                // no custom code; just don't do the thing ever
                outRom[0x550c] = 0xEA;
                outRom[0x550d] = 0xEA;
                outRom[0x550e] = 0xEA;
                outRom[0x550f] = 0xEA;
                outRom[0x5510] = 0xEA;
                outRom[0x5511] = 0xEA;
            }
            else
            {
                // consumables only
                CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 20);

                outRom[0x550c] = 0x22;
                outRom[0x550d] = (byte)(context.workingOffset);
                outRom[0x550e] = (byte)(context.workingOffset >> 8);
                outRom[0x550f] = (byte)((context.workingOffset >> 16) + 0xC0);
                outRom[0x5510] = 0xEA;
                outRom[0x5511] = 0xEA;

                // a==0x10 (v==0x08) is moogle belt, a==0x12 (v==0x09) is midge mallet 
                // over12:
                // RTL
                // CMP #10
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = 0x10;

                // BNE over10 (1)
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x01;

                // RTL
                outRom[context.workingOffset++] = 0x6B;

                // over10:
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = 0x12;

                // CMP #12
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x01;

                // BNE over12
                outRom[context.workingOffset++] = 0x6B;

                // (removed code)
                // STZ $E1F1,x - zero out pending damage
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0xF1;
                outRom[context.workingOffset++] = 0xE1;

                // STZ $E1F2,x - zero out pending damage
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0xF2;
                outRom[context.workingOffset++] = 0xE1;

                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            return true;
        }
    }
}
