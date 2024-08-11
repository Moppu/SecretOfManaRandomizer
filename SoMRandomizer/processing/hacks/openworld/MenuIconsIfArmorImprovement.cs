using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Only show the little faces when buying gear if the gear is an improvement in defense.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MenuIconsIfArmorImprovement : RandoProcessor
    {
        protected override string getName()
        {
            return "Only show armor as wearable if it's an improvement";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // here: reading the character wearability value
            // $C0/7A1A BF D5 3E D0 LDA $D03ED5,x[$D0:3F93] A:0000 X:00BE Y:008E P:envMxdIZc - replace this
            // $C0/7A1E 29 E0       AND #$E0                A:0080 X:00BE Y:008E P:eNvMxdIzc
            outRom[0x7A1A] = 0x22;
            outRom[0x7A1B] = (byte)(context.workingOffset);
            outRom[0x7A1C] = (byte)(context.workingOffset >> 8);
            outRom[0x7A1D] = (byte)((context.workingOffset >> 16) + 0xC0);

            // 7ee[1,3,5]e[0,1,2] for current equips - hat, armor, accessory
            // < C9, it's a hat, otherwise < 19B, it's an armor, otherwise accessory

            // PHB [B]
            outRom[context.workingOffset++] = 0x8B;
            // PHA [BA]
            outRom[context.workingOffset++] = 0x48;
            // LDA #7E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x7E;
            // PHA [BAA]
            outRom[context.workingOffset++] = 0x48;
            // PLB [BA]
            outRom[context.workingOffset++] = 0xAB;
            // PLA [B]
            outRom[context.workingOffset++] = 0x68;

            // removed code
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xD5;
            outRom[context.workingOffset++] = 0x3E;
            outRom[context.workingOffset++] = 0xD0;

            // if 7E1847 is not 01 (buy menu), we quit
            // PHA [BA]
            outRom[context.workingOffset++] = 0x48;
            // LDA $1847
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x47;
            outRom[context.workingOffset++] = 0x18;
            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BEQ dontReturn (3)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x03;
            // PLA [B]
            outRom[context.workingOffset++] = 0x68;
            // PLB []
            outRom[context.workingOffset++] = 0xAB;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // dontReturn:
            // PLA [B]
            outRom[context.workingOffset++] = 0x68;

            // PHY [BYy]
            outRom[context.workingOffset++] = 0x5A;
            // PHX [BYyXx]
            outRom[context.workingOffset++] = 0xDA;
            // LDY #0000
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // CPX #C9
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            // BLT over
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0B;
            // LDY #0001
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // CPX #19B
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x9B;
            outRom[context.workingOffset++] = 0x01;
            // BLT over
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x03;
            // LDY #0002
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x00;

            // bits on item id indicating usable by character
            // 0x80 for boy
            // 0x40 for girl
            // 0x20 for sprite
            byte[] masks1 = new byte[] { 0x80, 0x40, 0x20 };
            // inverse of above - &ed with the item to indicate unusable by character, because its defense is ass
            byte[] masks2 = new byte[] { 0x7F, 0xBF, 0xDF };
            byte[] playerDataOffset = new byte[] { 0xE1, 0xE3, 0xE5 };

            for (int i = 0; i < 3; i++)
            {
                // over:
                // BIT #masks1
                outRom[context.workingOffset++] = 0x89;
                outRom[context.workingOffset++] = masks1[i];
                // BEQ doneWithChar - not usable to begin with, don't bother
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x34 + 0x0D;
                // PHA [BYyXxA]
                outRom[context.workingOffset++] = 0x48;
                // -- multiply by 10
                // LDA ExE1,y
                outRom[context.workingOffset++] = 0xB9;
                outRom[context.workingOffset++] = 0xE1;
                outRom[context.workingOffset++] = playerDataOffset[i];
                // PHA [BYyXxAA]
                outRom[context.workingOffset++] = 0x48;
                // LDA #00
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x00;
                // STA ExE1,y
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = 0xE1;
                outRom[context.workingOffset++] = playerDataOffset[i];
                // LDA ExE0,y
                outRom[context.workingOffset++] = 0xB9;
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = playerDataOffset[i];

                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
                // ASL
                outRom[context.workingOffset++] = 0x0A;
                // ASL
                outRom[context.workingOffset++] = 0x0A;
                // ASL
                outRom[context.workingOffset++] = 0x0A;
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC ExE0,y
                outRom[context.workingOffset++] = 0x79;
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = playerDataOffset[i];
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC ExE0,y
                outRom[context.workingOffset++] = 0x79;
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = playerDataOffset[i];
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // PLA [BYyXxA]
                outRom[context.workingOffset++] = 0x68;
                // STA ExE1,y
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = 0xE1;
                outRom[context.workingOffset++] = playerDataOffset[i];
                // LDA $D03ED1,x - def of shop item -> current item
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = 0xD1;
                outRom[context.workingOffset++] = 0x3E;
                outRom[context.workingOffset++] = 0xD0;
                // PHA - for comparison [BYyXxAA]
                outRom[context.workingOffset++] = 0x48;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // LDA $03,s for original x value
                outRom[context.workingOffset++] = 0xA3;
                outRom[context.workingOffset++] = 0x03;
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA $D03ED1,x - existing def -> shop def
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = 0xD1;
                outRom[context.workingOffset++] = 0x3E;
                outRom[context.workingOffset++] = 0xD0;
                // DEC
                outRom[context.workingOffset++] = 0x3A;
                // CMP $01,s
                outRom[context.workingOffset++] = 0xC3;
                outRom[context.workingOffset++] = 0x01;
                // BLT skip (5) -> BGE skip
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0x06;
                // PLA - shop def comparison [BYyXxA]
                outRom[context.workingOffset++] = 0x68;
                // PLA - original a [BYyXx]
                outRom[context.workingOffset++] = 0x68;
                // AND #masks2 - mark this as unusable for the character
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = masks2[i];
                // BRA 02 - skip the PLAs
                outRom[context.workingOffset++] = 0x80;
                outRom[context.workingOffset++] = 0x02;

                // skip:
                // PLA - shop def comparison [BYyXxA]
                outRom[context.workingOffset++] = 0x68;
                // PLA - original a [BYyXx]
                outRom[context.workingOffset++] = 0x68;
                // doneWithChar:
            }

            // PLX [BYy]
            outRom[context.workingOffset++] = 0xFA;
            // PLY [B]
            outRom[context.workingOffset++] = 0x7A;
            // PLB []
            outRom[context.workingOffset++] = 0xAB;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
