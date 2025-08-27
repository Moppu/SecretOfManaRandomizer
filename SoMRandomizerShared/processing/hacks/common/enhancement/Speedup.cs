using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Hack that turns speedup into haste, improving weapon charge speed, stamina recharge speed, and movement speed.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class Speedup : RandoProcessor
    {
        protected override string getName()
        {
            return "Gnome speedup spell enhancements";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_SPEEDUP_CHANGE))
            {
                return false;
            }

            // ////////////////////////////////////////
            // make speedup spell give + movement speed
            // ////////////////////////////////////////

            // B717 -> B730 - rewrite to handle movement speed
            // $00/B717 F0 08       BEQ $08 - not moving horizontally
            // $00/B719 A2 02       LDX #$02 ; right speed
            // $00/B71B 29 01       AND #$01
            // $00/B71D D0 02       BNE $02
            // $00/B71F A2 82       LDX #$82 ; left speed
            // 
            // $00/B721 A5 10       LDA $10
            // $00/B723 29 0C       AND #$0C
            // $00/B725 F0 08       BEQ $08 - not moving vertically
            // $00/B727 A0 02       LDY #$02 ; down speed
            // $00/B729 29 04       AND #$04
            // $00/B72B D0 02       BNE $02
            // $00/B72D A0 82       LDY #$82 ; up speed
            //
            // $00/B72F 24 10       BIT $10

            // B719 - load right-moving speed
            // $00 / B719 A2 02       LDX #$02                A:0001 X:0000 Y:0000 P:envMXdIzc
            // $00 / B71B 29 01       AND #$01                A:0001 X:0002 Y:0000 P:envMXdIzc
            int rightOffset = context.workingOffset;
            writeWalkSpeedCheck(outRom, context, new byte[] { 0xA2, 0x02 }, new byte[] { 0xA2, 0x03  });

            // up
            // $00/B72D A0 82       LDY #$82                A:0000 X:0000 Y:0002 P:envMXdIZc
            // $00 / B72F 24 10       BIT $10[$00:0010]   A: 0000 X: 0000 Y: 0082 P: eNvMXdIzc
            int upOffset = context.workingOffset;
            writeWalkSpeedCheck(outRom, context, new byte[] { 0xA0, 0x82 }, new byte[] { 0xA0, 0x83 });

            // down
            // $00/B727 A0 02       LDY #$02                A:0004 X:0000 Y:0000 P:envMXdIzc
            // $00 / B729 29 04       AND #$04                A:0004 X:0000 Y:0002 P:envMXdIzc
            int downOffset = context.workingOffset;
            writeWalkSpeedCheck(outRom, context, new byte[] { 0xA0, 0x02 }, new byte[] { 0xA0, 0x03 });

            // left
            // $00/B71F A2 82       LDX #$82                A:0000 X:0003 Y:0000 P:envMXdIZc
            // $00 / B721 A5 10       LDA $10[$00:0010]   A: 0000 X: 0082 Y: 0000 P: eNvMXdIzc
            int leftOffset = context.workingOffset;
            writeWalkSpeedCheck(outRom, context, new byte[] { 0xA2, 0x82 }, new byte[] { 0xA2, 0x83 });
            // E02C,x on player char is whether they're p1


            for(int i=0xB717; i <= 0xB730; i++)
            {
                outRom[i] = 0xEA;
            }

            outRom[0xB717] = 0x22;
            outRom[0xB718] = (byte)(context.workingOffset);
            outRom[0xB719] = (byte)(context.workingOffset >> 8);
            outRom[0xB71A] = (byte)((context.workingOffset >> 16) + 0xC0);

            // recreate original sequence above, but with generated code that checks for haste bits and moves faster if they're enabled
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x0C;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(rightOffset);
            outRom[context.workingOffset++] = (byte)(rightOffset >> 8);
            outRom[context.workingOffset++] = (byte)((rightOffset >> 16) + 0xC0);

            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x01;

            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x04;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(leftOffset);
            outRom[context.workingOffset++] = (byte)(leftOffset >> 8);
            outRom[context.workingOffset++] = (byte)((leftOffset >> 16) + 0xC0);

            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x10;

            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0C;

            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x0C;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(downOffset);
            outRom[context.workingOffset++] = (byte)(downOffset >> 8);
            outRom[context.workingOffset++] = (byte)((downOffset >> 16) + 0xC0);

            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x04;

            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x04;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(upOffset);
            outRom[context.workingOffset++] = (byte)(upOffset >> 8);
            outRom[context.workingOffset++] = (byte)((upOffset >> 16) + 0xC0);

            outRom[context.workingOffset++] = 0x24;
            outRom[context.workingOffset++] = 0x10;

            outRom[context.workingOffset++] = 0x6B;


            // ////////////////////////////////////////
            // faster stamina recharge
            // ////////////////////////////////////////

            // $00/EB5F 3A          DEC A                   A:0043 X:0000 Y:0000 P:envMxdIzc
            // $00/EB60 9D ED E1    STA $E1ED,x[$7E:E1ED]   A:0053 X:0000 Y:0000 P:envMxdIzC

            outRom[0xEB5F] = 0x22;
            outRom[0xEB60] = (byte)(context.workingOffset);
            outRom[0xEB61] = (byte)(context.workingOffset >> 8);
            outRom[0xEB62] = (byte)((context.workingOffset >> 16) + 0xC0);

            // replaced code
            outRom[context.workingOffset++] = 0x3A;
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xED;
            outRom[context.workingOffset++] = 0xE1;

            // pha
            outRom[context.workingOffset++] = 0x48;
            // lda $E1B0,x - check if haste is up
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;
            // and #14 - check if haste is up
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x14;
            // beq over - it's not
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x0B;
            // lda $E1ED,x - load stamina remaining until max
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xED;
            outRom[context.workingOffset++] = 0xE1;
            // CMP #02 - do nothing if near minimum
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x02;
            // blt over
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x04;
            // DEC A - hasten stamina recharge
            outRom[context.workingOffset++] = 0x3A;
            // STA $E1ED,x - write it back
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xED;
            outRom[context.workingOffset++] = 0xE1;
            // over:
            // pla - even out the stack
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // ////////////////////////////////////////
            // faster weapon charging
            // ////////////////////////////////////////

            // $00/B3DF C2 20       REP #$20                A:000E X:0000 Y:0000 P:eNvMxdIzc
            // $00 / B3E1 9F 1A E0 7E STA $7EE01A,x[$7E:E01A] A: 000E X: 0000 Y: 0000 P: eNvmxdIzc
            // $00 / B3E5 E2 20       SEP #$20                A:000E X:0000 Y:0000 P:eNvmxdIzc
            outRom[0xB3DF] = 0x22;
            outRom[0xB3E0] = (byte)(context.workingOffset);
            outRom[0xB3E1] = (byte)(context.workingOffset >> 8);
            outRom[0xB3E2] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0xB3E3] = 0xEA;
            outRom[0xB3E4] = 0xEA;
            outRom[0xB3E5] = 0xEA;
            outRom[0xB3E6] = 0xEA;

            // (replaced code)
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // STA $7ee01a,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x1A;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // pha
            outRom[context.workingOffset++] = 0x48;
            // lda 7ee1b0,x - check if hasted
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // and #14 - check if hasted
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x14;
            // beq over
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x0D;
            // lda 7ee01a,x - load weapon charge
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x1A;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // cmp #2B - at max
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x2A;
            // bge over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x05;
            // inc A
            outRom[context.workingOffset++] = 0x1A;
            // sta 7ee01a,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x1A;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // over:
            // pla
            outRom[context.workingOffset++] = 0x68;
            // rtl
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }

        // check if p1 has haste status, and run "newCode" block if so, otherwise run "origCode" block
        private void writeWalkSpeedCheck(byte[] outRom, RandoContext context, byte[] origCode, byte[] newCode)
        {
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA $7ee02c - boy player number
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x2C;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BNE overp1
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x06;
            // LDA $7ee1b0 - boy status byte including speedup flags
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // BRA end
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x12;
            // overp1:
            // LDA $7ee22c - girl player number
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x2C;
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x7E;
            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BNE overp2
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x06;
            // LDA $7ee3b0 - girl status byte including speedup flags
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // BRA end
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x04;
            // LDA $7ee5b0 - sprite status byte including speedup flags
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // end:
            // AND 0x14 to check for speedup buffs
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x14;
            // BEQ noChange
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = (byte)(newCode.Length + 2);
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // haste is active - use running speed code
            foreach(byte b in newCode)
            {
                outRom[context.workingOffset++] = b;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // haste is not active - use walking speed code
            foreach (byte b in origCode)
            {
                outRom[context.workingOffset++] = b;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;

        }
    }
}
