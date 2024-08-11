using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Hack that allows 12-character name entries and removes a particularly annoying input delay in vanilla's name entry system.
    /// This doesn't change any dialogue to have space for 12 letters, so expect vanilla dialogue to look weird.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NameEntryChanges : RandoProcessor
    {
        protected override string getName()
        {
            return "12-letter name entry";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_NAME_ENTRY_CHANGES))
            {
                return false;
            }
            /*
             * this is the counter for input repeats on left/right, it counts up to 04 from like F0ish
            $C0/1E08 AD 01 A2    LDA $A201  [$7E:A201]   A:0003 X:0220 Y:0004 P:eNvMxdIzc **
            $C0/1E0B 1A          INC A                   A:00F2 X:0220 Y:0004 P:eNvMxdIzc **
            $C0/1E0C 8D 01 A2    STA $A201  [$7E:A201]   A:00F3 X:0220 Y:0004 P:eNvMxdIzc **
            $C0/1E0F C9 04       CMP #$04                A:00F3 X:0220 Y:0004 P:eNvMxdIzc
            $C0/1E11 D0 09       BNE $09    [$1E1C]      A:00F3 X:0220 Y:0004 P:eNvMxdIzC

                but doing LDA #04 here instead makes it zoom super fast
                // replace with:
                // JSL $CFFE00[$CF:FE00]  for inputs?
                // LDA $42
                // BNE over
                // LDA $44
                // BNE over
                // LDA #04
                // RTL
                // over:
                // (same code)
             */

            // fix the weird delay when you tap right/left by resetting the counter when you've let off the controls
            outRom[0x1E08] = 0x22;
            outRom[0x1E09] = (byte)context.workingOffset;
            outRom[0x1E0A] = (byte)(context.workingOffset >> 8);
            outRom[0x1E0B] = (byte)((context.workingOffset >> 16) + 0xC0);

            // JSL $CFFE00
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xFE;
            outRom[context.workingOffset++] = 0xCF;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // LDA $42
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x42;

            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x0A;

            // LDA $44
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x44;

            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x06;

            // LDA #0004
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0x00;

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // over:
            // (removed code)
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $A201
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0xA2;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            
            // initial pos
            // $C7/5029 A9 12       LDA #$12                A:0000 X:0220 Y:3021 P:envMxdIZC - 06 instead to start far enough back for 12 characters
            outRom[0x7502A] = 0x06;
            // $C7/502B 8D 57 A1    STA $A157  [$7E:A157]   A:0012 X:0220 Y:3021 P:envMxdIzC

            // $C0/2496 B9 00 00    LDA $0000,y[$C7:75A8]   A:0000 X:0090 Y:75A8 P:envmxdIzc - the 07 here is the width - change to D
            outRom[0x775A9] = 0x0D;
            // $C0/248F BE 00 00    LDX $0000,y[$C7:75A6]   A:0000 X:0000 Y:75A6 P:envMxdIZc - 0x90 here is the ram position - move back to x84
            outRom[0x775A6] = 0x84;
            // $C0/255C 9D 00 90    STA $9000,x[$7E:9090]  writing the name border

            /*
$C0/3198 AD CC A1    LDA $A1CC  [$7E:A1CC]   A:0000 X:0000 Y:0004 P:envMxdIZc
$C0/319B C9 06       CMP #$06                A:0006 X:0000 Y:0004 P:envMxdIzc
             */
            // entry length check - change to C
            outRom[0x319C] = 0x0C;
            // $C7/50A5 A2 D2 00    LDX #$00D2              A:0000 X:000C Y:0000 P:envmxdIZc - change to C6 to look earlier in RAM for letters
            outRom[0x750A6] = 0xC6;

            // 35ea, text saying you can enter 6 letters
            outRom[0x35EA] = 0xB6;
            outRom[0x35EB] = 0xB7;

            outRom[0x35EC] = 0x80;

            outRom[0x35ED] = 0xA6;
            outRom[0x35EE] = 0x9F;
            outRom[0x35EF] = 0xAE;
            outRom[0x35F0] = 0xAE;
            outRom[0x35F1] = 0x9F;
            outRom[0x35F2] = 0xAC;
            outRom[0x35F3] = 0xAD;

            outRom[0x35F4] = 0x80;

            outRom[0x35F5] = 0xA6;
            outRom[0x35F6] = 0xA9;
            outRom[0x35F7] = 0xA8;
            outRom[0x35F8] = 0xA1;

            return true;
        }
    }
}
