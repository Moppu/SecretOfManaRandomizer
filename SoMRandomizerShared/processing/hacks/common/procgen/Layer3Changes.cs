using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// ROM hacks to draw floor number and timer on layer 3.  The timer only draws every 4 frames to
    /// reduce lag, since SoM is laggy enough already.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class Layer3Changes : RandoProcessor
    {
        public const string DIV10_SUBROUTINE_OFFSET = "div10SubrOffset";

        protected override string getName()
        {
            return "Floor number and timer on layer 3";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool showFloorNum = false;
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            if (randoMode == AncientCaveSettings.MODE_KEY)
            {
                showFloorNum = true;
            }
            else if (randoMode == BossRushSettings.MODE_KEY)
            {
                showFloorNum = true;
            }
            else if (randoMode == ChaosSettings.MODE_KEY)
            {
                showFloorNum = false;
            }
            else
            {
                Logging.log("Unsupported mode for ancient cave-style enemy drops");
                return false;
            }

            // For VRAM and multiply/divide address reference, see: https://en.wikibooks.org/wiki/Super_NES_Programming/SNES_Hardware_Registers

            // copy 01/E3FA subroutine to divide by 10, but as a JSL sub
            // calculates: A = Y % 10
            // also, $4214 is the divide result if you need that too
            int div10SubrOffset = context.workingOffset;
            context.workingData.setInt(DIV10_SUBROUTINE_OFFSET, context.workingOffset);
            // STY $4204
            outRom[context.workingOffset++] = 0x8C;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0x42;
            // LDA #0A
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x0A;
            // STA $4206
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x06;
            outRom[context.workingOffset++] = 0x42;
            // wait for the div/mod calculation
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            // LDA $4216
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x16;
            outRom[context.workingOffset++] = 0x42;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // font data for "FLOR:." so we can show floor number .. number characters already exist
            byte[] fontData = new byte[] {
                0xFF, 0x00, 0x81, 0x7E, 0xFF, 0x60, 0xFC, 0x78, 0xFC, 0x60, 0xF0, 0x60, 0x90, 0x60, 0xF0, 0x00, // F
                0xF0, 0x00, 0x90, 0x60, 0xF0, 0x60, 0xF0, 0x60, 0xF0, 0x60, 0xFF, 0x60, 0x81, 0x7E, 0xFF, 0x00, // L
                0x3C, 0x00, 0x42, 0x3C, 0xFF, 0x66, 0xFF, 0x66, 0xFF, 0x66, 0xFF, 0x66, 0xC3, 0x3C, 0x3C, 0x00, // O
                0xFE, 0x00, 0x83, 0x7C, 0xFF, 0x66, 0xFF, 0x66, 0xFE, 0x7C, 0xFF, 0x66, 0x99, 0x66, 0xFF, 0x00, // R
                0x00, 0x00, 0x3C, 0x00, 0x24, 0x18, 0x3C, 0x00, 0x00, 0x00, 0x3C, 0x00, 0x24, 0x18, 0x3C, 0x00, // :
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3C, 0x00, 0x24, 0x18, 0x3C, 0x00, // .
            };


            // upload custom font characters and show floor number (if applicable)
            
            int showMapNumSubrOffset = context.workingOffset;
            // LDX #5AE7
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0xE7;
            outRom[context.workingOffset++] = 0x5A;
            // STX $2116
            outRom[context.workingOffset++] = 0x8E;
            outRom[context.workingOffset++] = 0x16;
            outRom[context.workingOffset++] = 0x21;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // XBA
            outRom[context.workingOffset++] = 0xEB;

            // LDA $7E00DC - load mapnum
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            // CMP #B4 - check for credits map
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xB4;
            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x08;
            // -- replaced code
            // E2 20      SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // A9 43      LDA #43
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x43;
            // 8D 55 0A   STA $0A55
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x55;
            outRom[context.workingOffset++] = 0x0A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // over:
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // A = Y % 10
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);
            // STA $0A55 - least significant digit of floor number
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x55;
            outRom[context.workingOffset++] = 0x0A;
            // LDA $4214 - the divide result, and most significant digit of floor number
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #2008 - for tile number reference
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x20;

            if (showFloorNum)
            {
                // STA $2118
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x18;
                outRom[context.workingOffset++] = 0x21;
                // LDA $0A55
                outRom[context.workingOffset++] = 0xAD;
                outRom[context.workingOffset++] = 0x55;
                outRom[context.workingOffset++] = 0x0A;
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC #2008 - for tile number reference
                outRom[context.workingOffset++] = 0x69;
                outRom[context.workingOffset++] = 0x08;
                outRom[context.workingOffset++] = 0x20;
                // STA $2118
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x18;
                outRom[context.workingOffset++] = 0x21;
            }

            // LDX #4008 - VRAM target address, in 16bit chunks - $4008 = $8010
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x40;
            // STX $2116 - VRAM transfer
            outRom[context.workingOffset++] = 0x8E;
            outRom[context.workingOffset++] = 0x16;
            outRom[context.workingOffset++] = 0x21;

            for (int i = 0; i < fontData.Length; i += 2)
            {
                // LDA #fontDataByte
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = fontData[i];
                outRom[context.workingOffset++] = fontData[i + 1];
                // STA $2118
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x18;
                outRom[context.workingOffset++] = 0x21;
            }

            // show the "floor" character by character
            if (showFloorNum)
            {
                // LDX #5AE1
                outRom[context.workingOffset++] = 0xA2;
                outRom[context.workingOffset++] = 0xE1;
                outRom[context.workingOffset++] = 0x5A;
                // STX $2116
                outRom[context.workingOffset++] = 0x8E;
                outRom[context.workingOffset++] = 0x16;
                outRom[context.workingOffset++] = 0x21;
                // LDA #2001 "F"
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x01;
                outRom[context.workingOffset++] = 0x20;
                // STA $2118
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x18;
                outRom[context.workingOffset++] = 0x21;
                // LDA #2002 "L"
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x02;
                outRom[context.workingOffset++] = 0x20;
                // STA $2118
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x18;
                outRom[context.workingOffset++] = 0x21;
                // LDA #2003 "O"
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x03;
                outRom[context.workingOffset++] = 0x20;
                // STA $2118
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x18;
                outRom[context.workingOffset++] = 0x21;
                // LDA #2003 "O"
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x03;
                outRom[context.workingOffset++] = 0x20;
                // STA $2118
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x18;
                outRom[context.workingOffset++] = 0x21;
                // LDA #2003 "R"
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x04;
                outRom[context.workingOffset++] = 0x20;
                // STA $2118
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x18;
                outRom[context.workingOffset++] = 0x21;
            }
            
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // (replaced code) LDA 7EE000
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // replace this vanilla call with the above:
            // $01/E11E AF 00 E0 7E LDA $7EE000[$7E:E000]   A:5B01 X:00FF Y:0000 P:envMxdIzc
            outRom[0x1E11E] = 0x22;
            outRom[0x1E11F] = (byte)showMapNumSubrOffset;
            outRom[0x1E120] = (byte)(showMapNumSubrOffset >> 8);
            outRom[0x1E121] = (byte)((showMapNumSubrOffset >> 16) + 0xC0);
            
            // add timer increment code to a couple vblank handlers.
            // The first is this one:
            // ***NMI
            //$00 / 0100 5C A8 C0 00 JMP $00C0A8[$00:C0A8]   A: 0001 X: 0030 Y: FFFE P:envMxdIzC
            //$00 / C0A8 C2 30       REP #$30                A:0001 X:0030 Y:FFFE P:envMxdIzC
            //$00 / C0AA 48          PHA A:0001 X: 0030 Y: FFFE P:envmxdIzC
            //$00 / C0AB DA PHX                     A: 0001 X: 0030 Y: FFFE P:envmxdIzC
            //$00 / C0AC 5A PHY                     A: 0001 X: 0030 Y: FFFE P:envmxdIzC
            //$00 / C0AD 0B          PHD A:0001 X: 0030 Y: FFFE P:envmxdIzC
            //$00 / C0AE 8B PHB                     A: 0001 X: 0030 Y: FFFE P:envmxdIzC
            //$00 / C0AF A9 00 00    LDA #$0000              A:0001 X:0030 Y:FFFE P:envmxdIzC
            //$00 / C0B2 5B TCD                     A: 0000 X: 0030 Y: FFFE P:envmxdIZC

            int incrementTimerSubrOffset = context.workingOffset;
            outRom[0xC0AF] = 0x22;
            outRom[0xC0B0] = (byte)incrementTimerSubrOffset;
            outRom[0xC0B1] = (byte)(incrementTimerSubrOffset >> 8);
            outRom[0xC0B2] = (byte)((incrementTimerSubrOffset >> 16) + 0xC0);
            generateIncrementCode(outRom, context, new byte[] { 0xC2, 0x20, 0xA9, 0x00, 0x00, 0x5B }, 0x7ecf18, 0x7ecf19, 0x7ecf1a, 0x7ecf1b);

            // and another here:
            // $C0/20B2 AF 10 42 00 LDA $004210[$00:4210]   A:007E X:0006 Y:0004 P:envMxdIzc
            int incrementTimerSubrOffsetAltVBlank = context.workingOffset;
            outRom[0x20B2] = 0x22;
            outRom[0x20B3] = (byte)incrementTimerSubrOffsetAltVBlank;
            outRom[0x20B4] = (byte)(incrementTimerSubrOffsetAltVBlank >> 8);
            outRom[0x20B5] = (byte)((incrementTimerSubrOffsetAltVBlank >> 16) + 0xC0);

            generateIncrementCode(outRom, context, new byte[] { 0xAF, 0x10, 0x42, 0x00 }, 0x7ecf18, 0x7ecf19, 0x7ecf1a, 0x7ecf1b);


            // to render every frame, we replace this block:
            // $00/C144 C2 20       REP #$20                A:0001 X:0220 Y:FFFE P:envMxdIzC
            // $00/C146 E6 F4       INC $F4    [$00:00F4]   A:0001 X:0220 Y:FFFE P:envmxdIzC

            int showTimerSubrOffset = context.workingOffset;
            outRom[0xC144] = 0x22;
            outRom[0xC145] = (byte)showTimerSubrOffset;
            outRom[0xC146] = (byte)(showTimerSubrOffset >> 8);
            outRom[0xC147] = (byte)((showTimerSubrOffset >> 16) + 0xC0);

            // LDA 7ECF1B - the frames portion of our counter, which runs 00->3B
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x1B;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            // AND #03 - only render when the result is 0, so we aren't running all of this every frame
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x03;
            // BEQ over
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x03;
            // (replaced code) INC $F4
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0xF4;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // over:
            // LDA $7E00DC - load mapnum
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // CMP #B4 - credits map
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xB4;
            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x03;
            // (replaced code) INC $F4
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0xF4;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // over:
            // LDX #5AC1 
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0xF4;
            outRom[context.workingOffset++] = 0x5A;
            // STX $2116
            outRom[context.workingOffset++] = 0x8E;
            outRom[context.workingOffset++] = 0x16;
            outRom[context.workingOffset++] = 0x21;

            // (hours)
            // LDA $7ECF18 - hours value
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // A = Y % 10
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #2008 - for tile id values
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x20;
            // PHA - hold onto it for now
            outRom[context.workingOffset++] = 0x48;
            // LDA $4214 - the 10s digit of the calculation
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #2008 - for tile id values
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x20;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;
            // PLA - grab the ones digit back out
            outRom[context.workingOffset++] = 0x68;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;
            // LDA #2005 - ":"
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x05;
            outRom[context.workingOffset++] = 0x20;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;

            // (minutes)
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $7ECF19 - minutes value
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x19;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // A = Y % 10
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #2008 - for tile id values
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x20;
            // PHA - hold onto it for now
            outRom[context.workingOffset++] = 0x48;
            // LDA $4214 - the 10s digit of the calculation
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #2008 - for tile id values
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x20;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;
            // PLA - grab the ones digit back out
            outRom[context.workingOffset++] = 0x68;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;
            // LDA #2005 - ":"
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x05;
            outRom[context.workingOffset++] = 0x20;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;


            // (seconds)
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $7ECF1A - seconds value
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x1A;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // A = Y % 10
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #2008 - for tile id values
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x20;
            // PHA - hold onto it for now
            outRom[context.workingOffset++] = 0x48;
            // LDA $4214 - the 10s digit of the calculation
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #2008 - for tile id values
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x20;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;
            // PLA - grab the ones digit back out
            outRom[context.workingOffset++] = 0x68;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;
            // LDA #2006 - "."
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x06;
            outRom[context.workingOffset++] = 0x20;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;


            // (frames)
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $7ECF1B - frames value
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x1B;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            // ---
            // quick conversion from 0-60 to 0-100 using bitwise ops
            // shift right once, add (90)
            // shift right twice more, add (97.5ish)
            // close enough.
            // PHA
            // LSR
            // 8F 1C CF 7E    STA 7ECF1C
            // LSR
            // LSR
            // 8F 1D CF 7E    STA 7ECF1D
            // PLA
            // ADC 7ECF1D
            // ADC 7ECF1C

            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // STA $7ECF1C
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x1C;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // LSR, LSR
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            // STA $7ECF1D
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x1D;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // ADC $7ECF1D
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = 0x1C;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // ADC $7ECF1C
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = 0x1D;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // ---
            // now more of the same.
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // A = Y % 10
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #2008 - for tile id values
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x20;
            // PHA - hold onto it for now
            outRom[context.workingOffset++] = 0x48;
            // LDA $4214 - the 10s digit of the calculation
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #2008 - for tile id values
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x20;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;
            // PLA - grab the ones digit back out
            outRom[context.workingOffset++] = 0x68;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;
            // LDA #2000 - " "
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x20;
            // STA $2118 - write to vram
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x21;
            // (replaced code) INC $F4
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0xF4;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // ///////////////////////////////////////////////////////////
            // Modify number chars to look okay when not above a player bar
            // ///////////////////////////////////////////////////////////

            // take the line out of the number graphics
            // 127F00, 16 bytes each
            byte[] numberCharsChange = new byte[]
            {
                0x3C, 0x00, 0x42, 0x3C, 0xFF, 0x66, 0xFF, 0x66, 0xFF, 0x66, 0xFF, 0x66, 0x42, 0x3C, 0x3C, 0x00,
                0x18, 0x00, 0x24, 0x18, 0x7C, 0x38, 0x3C, 0x18, 0x3C, 0x18, 0x3C, 0x18, 0x42, 0x3C, 0x3C, 0x00,
                0x3C, 0x00, 0x42, 0x3C, 0xFF, 0x46, 0xFF, 0x06, 0x3E, 0x18, 0x7E, 0x20, 0x81, 0x7E, 0x7E, 0x00,
                0x3C, 0x00, 0x43, 0x3C, 0xFF, 0x66, 0x7E, 0x0C, 0x7F, 0x06, 0xFF, 0x66, 0x42, 0x3C, 0x3C, 0x00,
                0x0C, 0x00, 0x12, 0x0C, 0x3E, 0x1C, 0x7E, 0x2C, 0xFE, 0x4C, 0xFF, 0x7E, 0x72, 0x0C, 0x0C, 0x00,
                0x7E, 0x00, 0x81, 0x7E, 0xFE, 0x60, 0xFE, 0x7C, 0x7F, 0x06, 0xFF, 0x66, 0x42, 0x3C, 0x3C, 0x00,
                0x3C, 0x00, 0x42, 0x3C, 0xFC, 0x60, 0xFE, 0x7C, 0xFF, 0x66, 0xFF, 0x66, 0x42, 0x3C, 0x3C, 0x00,
                0x7E, 0x00, 0x81, 0x7E, 0xFF, 0x66, 0x7E, 0x04, 0x1C, 0x08, 0x3C, 0x18, 0x24, 0x18, 0x18, 0x00,
                0x3C, 0x00, 0x42, 0x3C, 0xFF, 0x66, 0x7E, 0x3C, 0xFF, 0x66, 0xFF, 0x66, 0x42, 0x3C, 0x3C, 0x00,
                0x3C, 0x00, 0x42, 0x3C, 0xFF, 0x66, 0xFF, 0x66, 0x7F, 0x3E, 0x3F, 0x06, 0x42, 0x3C, 0x3C, 0x00,
            };

            int fontOffset = 0x127F00;
            foreach (byte b in numberCharsChange)
            {
                outRom[fontOffset++] = b;
            }

            // a little extra code to handle display of timer when in the ring menu
            // replace:
            // $C0 / 6D1E A2 C0 18    LDX #$18C0            A:0800 X:0038 Y:0004 P:envmxdIZc
            // $C0 / 6D21 AD 0B 18    LDA $180B[$00:180B]   A:0800 X:18C0 Y:0004 P:envmxdIzc
            outRom[0x6D1E] = 0x22;
            outRom[0x6D1F] = (byte)context.workingOffset;
            outRom[0x6D20] = (byte)(context.workingOffset >> 8);
            outRom[0x6D21] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x6D22] = 0xEA;
            outRom[0x6D23] = 0xEA;

            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // JSL showTimerSubrOffset
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)showTimerSubrOffset;
            outRom[context.workingOffset++] = (byte)(showTimerSubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((showTimerSubrOffset >> 16) + 0xC0);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // (replaced code) LDX #18C0
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x18;
            // (replaced code) LDA $180B
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0x18;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }

        private void generateIncrementCode(byte[] outRom, RandoContext context, byte[] replacedCode, int hoursLoc, int minutesLoc, int secondsLoc, int framesLoc)
        {
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // use event flag x27 to turn timer on and off
            // LDA 7ECF27
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = EventFlags.PROCGEN_MODE_TIMER_RUNNING_FLAG;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            // BEQ skip -> 6 + 19 + 19 + 9 + 13 = x42
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x42;


            // LDA framesLocation
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)framesLoc;
            outRom[context.workingOffset++] = (byte)(framesLoc >> 8);
            outRom[context.workingOffset++] = (byte)((framesLoc >> 16));
            // INC A - add a frame, here in the vblank handler code, after one frame has passed 
            outRom[context.workingOffset++] = 0x1A;
            // STA framesLocation
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)framesLoc;
            outRom[context.workingOffset++] = (byte)(framesLoc >> 8);
            outRom[context.workingOffset++] = (byte)((framesLoc >> 16));
            // CMP #3C
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x3C;
            // BNE skip -> 6 + 19 + 19 + 9
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x35;
            // at this point, the frames have rolled over, so we set them to 0 and increment the seconds.
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA framesLocation
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x1B;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            
            // LDA secondsLocation
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)secondsLoc;
            outRom[context.workingOffset++] = (byte)(secondsLoc >> 8);
            outRom[context.workingOffset++] = (byte)((secondsLoc >> 16));
            // INC A - add one second to the timer
            outRom[context.workingOffset++] = 0x1A;
            // STA secondsLocation
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)secondsLoc;
            outRom[context.workingOffset++] = (byte)(secondsLoc >> 8);
            outRom[context.workingOffset++] = (byte)((secondsLoc >> 16));
            // CMP #3C
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x3C;
            // BNE skip -> 6 + 19 + 9
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x22;
            // at this point, the seconds have rolled over, so we set them to 0 and increment the minutes.
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA secondsLocation
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x1A;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            
            // LDA minutesLocation
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)minutesLoc;
            outRom[context.workingOffset++] = (byte)(minutesLoc >> 8);
            outRom[context.workingOffset++] = (byte)((minutesLoc >> 16));
            // INC A - add one minute to the timer
            outRom[context.workingOffset++] = 0x1A;
            // STA minutesLocation
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)minutesLoc;
            outRom[context.workingOffset++] = (byte)(minutesLoc >> 8);
            outRom[context.workingOffset++] = (byte)((minutesLoc >> 16));
            // CMP #3C
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x3C;
            // BNE skip -> 6 + 9
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x0F;
            // at this point, the minutes have rolled over, so we set them to 0 and increment the hours.
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA minutesLocation
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x19;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;


            // LDA hoursLocation
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)hoursLoc;
            outRom[context.workingOffset++] = (byte)(hoursLoc >> 8);
            outRom[context.workingOffset++] = (byte)((hoursLoc >> 16));
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // STA hoursLocation
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)hoursLoc;
            outRom[context.workingOffset++] = (byte)(hoursLoc >> 8);
            outRom[context.workingOffset++] = (byte)((hoursLoc >> 16));


            // put back the code code we replaced:
            foreach (byte b in replacedCode)
            {
                outRom[context.workingOffset++] = b;
            }

            outRom[context.workingOffset++] = 0x6B;
        }
    }
}
