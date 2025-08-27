using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Supporting hack for showing custom status text, like the level increments in open world timed mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CustomTopOfScreenText : RandoProcessor
    {
        public static string OFFSET_HIROM = "customTopOfScreenTextOffset";

        protected override string getName()
        {
            return "Custom top of screen text";
        }

        // so to use this, we write a 24 bit offset with the dialogue we want to RamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT
        // and then just call it from anywhere. See EnemiesAtYourLevel for an example
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            int subrOffset = context.workingOffset;
            context.workingData.setInt(OFFSET_HIROM, subrOffset);
            // read from RamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT and push it to the SoM message queue

            // PHB - save off bank
            outRom[context.workingOffset++] = 0x8B;
            // PHY - save before status so we know we have the right size
            outRom[context.workingOffset++] = 0x5A;
            // PHP - save off status (a/x/y size)
            outRom[context.workingOffset++] = 0x08;

            // SEP 20 - 8bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // REP 10 - 16bit x/y
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x10;
            //
            // determine y value
            // LDA 7E03A1 
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x7E;
            // STA 004202  8F xx xx xx
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // LDA #1E     A9 1E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x1E;
            // STA 004203
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // REP #20     C2 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // NOP, NOP, NOP  EA EA EA
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            // LDA 004216  AF xx xx xx
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x16;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // TAY         A8
            outRom[context.workingOffset++] = 0xA8;
            // 
            // clear out the space
            // PHY
            outRom[context.workingOffset++] = 0x5A;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // A9 00 00    LDA #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // LDX #000F
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x0F;
            outRom[context.workingOffset++] = 0x00;
            // back:
            // 99 00 FE    STA $FE00,y[$7E:FE00]
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xFE;
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // DEX
            outRom[context.workingOffset++] = 0xCA;
            // BNE back
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xF8;

            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // write text
            // SEP 20      E2 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA RamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT - bank
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 2) >> 16);
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PLB - load bank
            outRom[context.workingOffset++] = 0xAB;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // LDA16bit $(RamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT + 1) - load first text value .. later increment this and store it back
            // loop until data is 00
            // LDA RamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOP_OF_SCREEN_DIALOGUE_LOCATION_24BIT) >> 16);
            // TYX
            outRom[context.workingOffset++] = 0xBB;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #30
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x30;
            // STA $FE00,y -> STA 7EFE00,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xFE;
            outRom[context.workingOffset++] = 0x7E;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // back:
            // LDA 0000,x -> y
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // BEQ out
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // STA $FE00,y -> STA 7EFE00,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xFE;
            outRom[context.workingOffset++] = 0x7E;
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // BRA back
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xF3;

            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // increment thingies
            // LDA 7E03A1 
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x7E;
            // INC
            outRom[context.workingOffset++] = 0x1A;
            // CMP #0C
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x0C;
            // BNE over (02)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA 7E03A1 
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x7E;
            // LDA 7E039F
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x7E;
            // INC
            outRom[context.workingOffset++] = 0x1A;
            // STA 7E039F 
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x7E;
            // PLP - restore status
            outRom[context.workingOffset++] = 0x28;
            // PLY - restore after status so we know we have the right size
            outRom[context.workingOffset++] = 0x7A;
            // PLB - restore bank
            outRom[context.workingOffset++] = 0xAB;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            return true;
        }
    }
}
