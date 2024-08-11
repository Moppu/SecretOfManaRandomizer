using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.bossrush
{
    /// <summary>
    /// Hack that upgrades charge level of a weapon to the max for the weapon's level as soon as it's upgraded. Used in boss rush mode where
    /// there's not a lot of opportunity to kill stuff and level weapons up.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class InstantWeaponLevels : RandoProcessor
    {
        protected override string getName()
        {
            return "Level weapons for everyone as soon as they are upgraded";
        }

        // make it so as soon as we upgrade watts levels of weapons, everybody can charge it to that level.
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // initial charge level should be 1? do in char init somewhere - add to StartingLevel hack

            // 7EE1Cx for boy weapon levels .. 0-3, one nibble each
            // 7EE1Dx is the partial amount .. 0-7
            // 7EE1E8 is boy player's current weapon
            // 7ECC55 sword level
            // here's watts leveling up the boomerang in 5A:
            // $C0/7BF4 9F 00 CC 7E STA $7ECC00,x[$7E:CC5A] A:00C1 X:005A Y:00FF P:eNvMxdIzC
            outRom[0x7BF4] = 0x22;
            outRom[0x7BF5] = (byte)(context.workingOffset);
            outRom[0x7BF6] = (byte)(context.workingOffset >> 8);
            outRom[0x7BF7] = (byte)((context.workingOffset >> 16) + 0xC0);


            // replaced bit to write weapon level
            // 9F 00 CC 7E STA $7ECC00,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;

            // - preserve these; they seem to be used again except maybe y
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // PHY
            outRom[context.workingOffset++] = 0x5A;

            // TXA
            outRom[context.workingOffset++] = 0x8A;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #54 - get weapon type index
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x54;
            // LDY #0000
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // loop:
            // CMP #02
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x02;
            // BLT out
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x06;
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #02
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x02;
            // BRA loop
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xF6;
            // out:
            // - now y is the byte index, and A is the nibble index
            // CMP #00
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            // BNE oddNibble
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x4F;

            // evenNibble:
            // PHX [1]
            outRom[context.workingOffset++] = 0xDA;
            // TYX [1]
            outRom[context.workingOffset++] = 0xBB;
            // - remove the existing data
            // LDA 7EE1C0,X [4]
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // AND #0F      [2]
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // STA 7EE1C0,X [4]
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // LDA 7EE3C0,X [4]
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // AND #0F      [2]
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // STA 7EE3C0,X [4]
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // LDA 7EE5C0,X [4]
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // AND #0F      [2]
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // STA 7EE5C0,X [4]
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // PLX          [1]
            outRom[context.workingOffset++] = 0xFA;
            // - now we need to load the actual weapon level
            // LDA $7ECC00,x[4]
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            // INC A        [1]
            outRom[context.workingOffset++] = 0x1A;
            // AND #0F      [2]
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;

            // ***
            // CMP #08
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x08;
            // BLT over (2)
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;
            // LDA #08
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x08;
            // ***

            // - store in upper nibble
            // ASL          [1]
            outRom[context.workingOffset++] = 0x0A;
            // ASL          [1]
            outRom[context.workingOffset++] = 0x0A;
            // ASL          [1]
            outRom[context.workingOffset++] = 0x0A;
            // ASL          [1]
            outRom[context.workingOffset++] = 0x0A;
            // PHX          [1]
            outRom[context.workingOffset++] = 0xDA;
            // TYX          [1]
            outRom[context.workingOffset++] = 0xBB;
            // ORA 7EE1C0,X [4]
            outRom[context.workingOffset++] = 0x1F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EE1C0,X [4]
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // ORA 7EE3C0,X [4]
            outRom[context.workingOffset++] = 0x1F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EE3C0,X [4]
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // ORA 7EE5C0,X [4]
            outRom[context.workingOffset++] = 0x1F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EE5C0,X [4]
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // PLX          [1]
            outRom[context.workingOffset++] = 0xFA;
            // BRA end      [2]
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x49;
            //              [73 dec, 49+6 hex]

            // oddNibble:
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // TYX
            outRom[context.workingOffset++] = 0xBB;
            // - remove the existing data
            // LDA 7EE1C0,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // AND #F0
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xF0;
            // STA 7EE1C0,X
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // LDA 7EE3C0,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // AND #F0
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xF0;
            // STA 7EE3C0,X
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // LDA 7EE5C0,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // AND #F0
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xF0;
            // STA 7EE5C0,X
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // - now we need to load the actual weapon level
            // LDA $7ECC00,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // AND #0F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;

            // ***
            // CMP #08
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x08;
            // BLT over (2)
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;
            // LDA #08
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x08;
            // ***

            // - store in lower nibble
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // TYX
            outRom[context.workingOffset++] = 0xBB;
            // ORA 7EE1C0,X
            outRom[context.workingOffset++] = 0x1F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EE1C0,X
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // ORA 7EE3C0,X
            outRom[context.workingOffset++] = 0x1F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EE3C0,X
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // ORA 7EE5C0,X
            outRom[context.workingOffset++] = 0x1F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EE5C0,X
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // PLX
            outRom[context.workingOffset++] = 0xFA;

            //
            // end:
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
