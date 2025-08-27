using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Hack that allows off-screen targets to be selected.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FreeTargetting : RandoProcessor
    {
        protected override string getName()
        {
            return "Off-screen targetting";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_EXTENDED_TARGETTING))
            {
                return false;
            }
            // screen x/y checks when processing targets
            /*
$D0/DAB8 BF 22 00 7E LDA $7E0022,x[$7E:E622] A:0045 X:E600 Y:1812 P:eNvmxdIzc
$D0/DABC C9 D8 00    CMP #$00D8              A:00C7 X:E600 Y:1812 P:envmxdIzc
$D0/DABF B0 28       BCS $28    [$DAE9]      A:00C7 X:E600 Y:1812 P:eNvmxdIzc **
$

$D0/DAAE C9 08 00    CMP #$0008              A:0045 X:E600 Y:1812 P:envmxdIzC
$D0/DAB1 90 36       BCC $36    [$DAE9]      A:0045 X:E600 Y:1812 P:envmxdIzC **
$D0/DAB3 C9 F8 00    CMP #$00F8              A:0045 X:E600 Y:1812 P:envmxdIzC
$D0/DAB6 B0 31       BCS $31    [$DAE9]      A:0045 X:E600 Y:1812 P:eNvmxdIzc **
             */
            outRom[0x10DABF] = 0xEA;
            outRom[0x10DAC0] = 0xEA;

            outRom[0x10DAB1] = 0xEA;
            outRom[0x10DAB2] = 0xEA;

            outRom[0x10DAB6] = 0xEA;
            outRom[0x10DAB7] = 0xEA;


            outRom[0x10DC9F] = 0x22;
            outRom[0x10DCA0] = (byte)(context.workingOffset);
            outRom[0x10DCA1] = (byte)(context.workingOffset >> 8);
            outRom[0x10DCA2] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x10DCA3] = 0xEA;
            
            int XMIN = 0x01;
            int XMAX = 0xE4;
            int YMIN = 0x01;
            int YMAX = 0xBC;

            // PHA
            outRom[context.workingOffset++] = 0x48;

            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // STA RamOffsets.X_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.X_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 16));
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // XBA
            outRom[context.workingOffset++] = 0xEB;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // STA RamOffsets.Y_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.Y_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 16));
            //
            // -- check if msbs set and we're way out of bounds
            // BF 21 00 7E   LDA $7E0021,x[$7E:E621] - screen x msb
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x21;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // BMI negative
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x04;
            // BNE positive
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x0A;
            // BRA overx
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x0E;
            // negative:
            // LDA #XMIN
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)XMIN;
            // STA RamOffsets.X_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.X_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 16));
            // BRA overx
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x06;
            // positive:
            // LDA #XMAX
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)XMAX;
            // STA RamOffsets.X_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.X_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 16));
            // overx:
            //
            // BF 23 00 7E   LDA $7E0023,x[$7E:E623] - screen y msb
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x23;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // BMI negative
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x04;
            // BNE positive
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x0A;
            // BRA overy
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x0E;
            // negative:
            // LDA #YMIN
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)YMIN;
            // STA RamOffsets.Y_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.Y_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 16));
            // BRA overy
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x06;
            // positive:
            // LDA #YMAX
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)YMAX;
            // STA RamOffsets.Y_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.Y_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 16));
            // overy:
            // 
            // - bounds check the x/y values
            // LDA RamOffsets.X_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.X_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 16));
            // CMP #XMIN
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = (byte)XMIN;
            // BGE overA
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x06;
            // LDA #XMIN
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)XMIN;
            // STA RamOffsets.X_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.X_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 16));
            // overA:
            //
            // CMP #XMAX
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = (byte)XMAX;
            // BLT overB
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x06;
            // LDA #XMAX
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)XMAX;
            // STA RamOffsets.X_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.X_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 16));
            // overB:
            //
            // LDA RamOffsets.Y_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.Y_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 16));
            // CMP #YMIN
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = (byte)YMIN;
            // BGE overC
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x06;
            // LDA #YMIN
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)YMIN;
            // STA RamOffsets.Y_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.Y_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 16));
            // overC:
            //
            // CMP #YMAX
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = (byte)YMAX;
            // BLT overD
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x06;
            // LDA #YMAX
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)YMAX;
            // STA RamOffsets.Y_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.Y_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.Y_SCREEN_TEMP_LOCATION >> 16));
            // overD:
            //
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // XBA
            outRom[context.workingOffset++] = 0xEB;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA RamOffsets.X_SCREEN_TEMP_LOCATION
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.X_SCREEN_TEMP_LOCATION;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.X_SCREEN_TEMP_LOCATION >> 16));
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            //
            // removed code:
            // AE 32 18    LDX $1832  [$00:1832]   A:0A6B X:E600 Y:0120 P:envmxdIzc
            outRom[context.workingOffset++] = 0xAE;
            outRom[context.workingOffset++] = 0x32;
            outRom[context.workingOffset++] = 0x18;
            // 95 00       STA $00,x  [$00:082C]    A:0A6B X:082C Y:0120 P:envmxdIzc
            outRom[context.workingOffset++] = 0x95;
            outRom[context.workingOffset++] = 0x00;
            // 6B RTL
            outRom[context.workingOffset++] = 0x6B;

            // fix: 4 pixels of overflow
            // $D0/DC8C BF 20 00 7E LDA $7E0020,x[$7E:EA20] A:6EEA X:EA00 Y:0120 P:envMxdIzC
            // $D0 / DC90 18          CLC A:6EFD X:EA00 Y:0120 P: eNvMxdIzC
            // $D0/DC91 69 04       ADC #$04                A:6EFD X:EA00 Y:0120 P:eNvMxdIzc

            outRom[0x10DC8C] = 0x22;
            outRom[0x10DC8D] = (byte)(context.workingOffset);
            outRom[0x10DC8E] = (byte)(context.workingOffset >> 8);
            outRom[0x10DC8F] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x10DC90] = 0xEA;
            outRom[0x10DC91] = 0xEA;
            outRom[0x10DC92] = 0xEA;

            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            outRom[context.workingOffset++] = 0x18;

            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x04;

            // BCC over
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;

            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)XMAX;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // also on y axis
            // $D0/DC7C   BF 22 00 7E LDA $7E0022,x[$7E:EA22] A:EA00 X:EA00 Y:0020 P:eNvMxdIzc
            // $D0 / DC80 38          SEC A:EA84 X:EA00 Y:0020 P: eNvMxdIzc
            // $D0 / DC81 E9 16       SBC #$16                A:EA84 X:EA00 Y:0020 P:eNvMxdIzC
            outRom[0x10DC7C] = 0x22;
            outRom[0x10DC7D] = (byte)(context.workingOffset);
            outRom[0x10DC7E] = (byte)(context.workingOffset >> 8);
            outRom[0x10DC7F] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x10DC80] = 0xEA;
            outRom[0x10DC81] = 0xEA;
            outRom[0x10DC82] = 0xEA;
            outRom[0x10DC83] = 0xEA;
            outRom[0x10DC84] = 0xEA;
            outRom[0x10DC85] = 0xEA;
            outRom[0x10DC86] = 0xEA;
            outRom[0x10DC87] = 0xEA;

            // (replaced code)
            // LDA $7E0022,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #16
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x16;

            // BPL over -> BCS over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x02;

            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)YMIN;

            // $D0/DC83 38          SEC                     A:EA01 X:EA00 Y:0020 P:envMxdIzc
            outRom[context.workingOffset++] = 0x38;
            // $D0/DC84 FF 45 00 7E SBC $7E0045,x[$7E:EA45] A:EA01 X:EA00 Y:0020 P:envMxdIzC
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x45;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            // BPL over -> BCS over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x02;

            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)YMIN;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
