using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Hack that modifies damage of a weapon based on its element versus the target's element.  Includes both saber
    /// spells for temporary elements and generated weapons with automatic, permanent elements.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ElementalDamage : RandoProcessor
    {
        protected override string getName()
        {
            return "Elemental damage";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_WEAPON_ELEMENTAL_DAMAGE))
            {
                return false;
            }

            // if we did weapon rando, grab the location of those elements
            int weaponElementsLocation = context.workingData.getInt(WeaponRandomizer.ELEMENTS_TABLE_OFFSET_HIROM);

            // See EnemyTypeDamage
            // replace these:
            // $C0/51FC 64 D5       STZ $D5    [$00:03D5]   A:000A X:0000 Y:0600 P:envMxdIzC
            // $C0/51FE E2 20       SEP #$20                A:000A X:0000 Y:0600 P:envMxdIzC
            // damage is in $D4, 8 bit
            outRom[0x51FC] = 0x22;
            outRom[0x51FD] = (byte)(context.workingOffset);
            outRom[0x51FE] = (byte)(context.workingOffset >> 8);
            outRom[0x51FF] = (byte)((context.workingOffset >> 16) + 0xC0);


            // 7EE195
            // 01 gnome
            // 02 sylphid
            // 04 undine
            // 08 salamando
            // 20 lumina
            // 40 luna

            // 7EE1A1 .. sourced from 193
            // enemy defenses
            // 01 weak to gnome
            // 02 weak to sylphid
            // 04 weak to undine
            // 08 weak to salamando
            // 10 weak to shade
            // 20 weak to lumina
            // 40 weak to luna
            // 80 weak to dryad

            // X is target, Y is source

            // -- first, if either one is zero just do nothing here
            // LDA E195, Y
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x95;
            outRom[context.workingOffset++] = 0xE1;
            // BNE attackElement [5]
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // STZ $D5
            outRom[context.workingOffset++] = 0x64;
            outRom[context.workingOffset++] = 0xD5;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // attackElement:
            // LDA E1A1, X
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0xE1;
            // BNE defenseElement [5]
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // STZ $D5
            outRom[context.workingOffset++] = 0x64;
            outRom[context.workingOffset++] = 0xD5;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // defenseElement:
            // -- now do actual checks
            // LDA E195, Y
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x95;
            outRom[context.workingOffset++] = 0xE1;
            // AND E1A1, X
            outRom[context.workingOffset++] = 0x3D;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0xE1;
            // BEQ skipExtraDamage [x13]
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x13;
            // -- here: increase damage, then removed code+rtl
            // LDA $D4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xD4;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC $D4
            outRom[context.workingOffset++] = 0x65;
            outRom[context.workingOffset++] = 0xD4;
            // CMP $D4
            outRom[context.workingOffset++] = 0xC5;
            outRom[context.workingOffset++] = 0xD4;
            // BGE over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x02;
            // LDA #FF
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xFF;
            // over:
            // STA $D4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xD4;
            // STZ $D5
            outRom[context.workingOffset++] = 0x64;
            outRom[context.workingOffset++] = 0xD5;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // skipExtraDamage:
            // -- here: check for decreased damage
            // LDA E1A1, X
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0xE1;
            // AND #AA
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xAA;
            // BEQ shiftLeft
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x06;
            // -- shift right
            // LDA E1A1, X
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0xE1;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // BRA doneShift
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x04;
            // shiftLeft:
            // LDA E1A1, X
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0xE1;
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // doneShift:
            // AND E195, Y
            outRom[context.workingOffset++] = 0x39;
            outRom[context.workingOffset++] = 0x95;
            outRom[context.workingOffset++] = 0xE1;
            // BEQ noDecrease [5]
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x05;
            // LDA $D4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xD4;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // STA $D4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xD4;
            // noDecrease:
            // STZ $D5
            outRom[context.workingOffset++] = 0x64;
            outRom[context.workingOffset++] = 0xD5;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            if (weaponElementsLocation != -1)
            {
                // 7ee1e3 is weapon id
                // $C0 / 5200 B9 9B E1    LDA $E19B,y[$7E:E79B]   A:000A X:0000 Y:0600 P:envMxdIzC
                // $C0 / 5203 0A ASL A A:0000 X: 0000 Y: 0600 P: envMxdIZC
                outRom[0x5200] = 0x22;
                outRom[0x5201] = (byte)(context.workingOffset);
                outRom[0x5202] = (byte)(context.workingOffset >> 8);
                outRom[0x5203] = (byte)((context.workingOffset >> 16) + 0xC0);

                // CPY #0600
                outRom[context.workingOffset++] = 0xC0;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x06;
                // BLT playerAttacking [5]
                outRom[context.workingOffset++] = 0x90;
                outRom[context.workingOffset++] = 0x05;
                // LDA $E19B,y
                outRom[context.workingOffset++] = 0xB9;
                outRom[context.workingOffset++] = 0x9B;
                outRom[context.workingOffset++] = 0xE1;
                // ASL
                outRom[context.workingOffset++] = 0x0A;
                // RTL
                outRom[context.workingOffset++] = 0x6B;

                // playerAttacking:
                // LDA E195, Y
                outRom[context.workingOffset++] = 0xB9;
                outRom[context.workingOffset++] = 0x95;
                outRom[context.workingOffset++] = 0xE1;
                // BEQ checkWeapon [5]
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x05;
                // LDA $E19B,y
                outRom[context.workingOffset++] = 0xB9;
                outRom[context.workingOffset++] = 0x9B;
                outRom[context.workingOffset++] = 0xE1;
                // ASL
                outRom[context.workingOffset++] = 0x0A;
                // RTL
                outRom[context.workingOffset++] = 0x6B;

                // checkWeapon:
                // PHX
                outRom[context.workingOffset++] = 0xDA;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // PHA
                outRom[context.workingOffset++] = 0x48;
                // LDA E1E3, Y
                outRom[context.workingOffset++] = 0xB9;
                outRom[context.workingOffset++] = 0xE3;
                outRom[context.workingOffset++] = 0xE1;
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // PLA
                outRom[context.workingOffset++] = 0x68;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA weaponElementsLocation, X
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = (byte)(weaponElementsLocation);
                outRom[context.workingOffset++] = (byte)(weaponElementsLocation>>8);
                outRom[context.workingOffset++] = (byte)((weaponElementsLocation >> 16)+0xC0);

                // BNE doThings [6]
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x06;
                // -- zero element, exit
                // PLX
                outRom[context.workingOffset++] = 0xFA;
                // LDA $E19B,y
                outRom[context.workingOffset++] = 0xB9;
                outRom[context.workingOffset++] = 0x9B;
                outRom[context.workingOffset++] = 0xE1;
                // ASL
                outRom[context.workingOffset++] = 0x0A;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
                // doThings:
                // PLX
                outRom[context.workingOffset++] = 0xFA;
                // -- pretty much the same thing we do above

                // PHA
                outRom[context.workingOffset++] = 0x48;
                // AND E1A1, X
                outRom[context.workingOffset++] = 0x3D;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0xE1;
                // BEQ skipExtraDamage [x13]
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x14;
                // -- here: increase damage, then removed code+rtl
                // LDA $D4
                outRom[context.workingOffset++] = 0xA5;
                outRom[context.workingOffset++] = 0xD4;
                // LSR
                outRom[context.workingOffset++] = 0x4A;
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC $D4
                outRom[context.workingOffset++] = 0x65;
                outRom[context.workingOffset++] = 0xD4;
                // CMP $D4
                outRom[context.workingOffset++] = 0xC5;
                outRom[context.workingOffset++] = 0xD4;
                // BGE over
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0x02;
                // LDA #FF
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0xFF;
                // over:
                // STA $D4
                outRom[context.workingOffset++] = 0x85;
                outRom[context.workingOffset++] = 0xD4;
                // PLA to even out the stack; we saved it for the less damage check
                outRom[context.workingOffset++] = 0x68; 

                // LDA $E19B,y
                outRom[context.workingOffset++] = 0xB9;
                outRom[context.workingOffset++] = 0x9B;
                outRom[context.workingOffset++] = 0xE1;
                // ASL
                outRom[context.workingOffset++] = 0x0A;

                // RTL
                outRom[context.workingOffset++] = 0x6B;
                // skipExtraDamage:
                // -- here: check for decreased damage
                // LDA E1A1, X
                outRom[context.workingOffset++] = 0xBD;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0xE1;
                // AND #AA
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xAA;
                // BEQ shiftLeft
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x06;
                // -- shift right
                // LDA E1A1, X
                outRom[context.workingOffset++] = 0xBD;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0xE1;
                // LSR
                outRom[context.workingOffset++] = 0x4A;
                // BRA doneShift
                outRom[context.workingOffset++] = 0x80;
                outRom[context.workingOffset++] = 0x04;
                // shiftLeft:
                // LDA E1A1, X
                outRom[context.workingOffset++] = 0xBD;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0xE1;
                // ASL
                outRom[context.workingOffset++] = 0x0A;
                // AND stack relative .. to the weapon thing we pushed earlier
                outRom[context.workingOffset++] = 0x23;
                outRom[context.workingOffset++] = 0x01;

                // BEQ noDecrease [5]
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x05;
                // LDA $D4
                outRom[context.workingOffset++] = 0xA5;
                outRom[context.workingOffset++] = 0xD4;
                // LSR
                outRom[context.workingOffset++] = 0x4A;
                // STA $D4
                outRom[context.workingOffset++] = 0x85;
                outRom[context.workingOffset++] = 0xD4;
                // PLA to even out stack
                outRom[context.workingOffset++] = 0x68;

                // LDA $E19B,y
                outRom[context.workingOffset++] = 0xB9;
                outRom[context.workingOffset++] = 0x9B;
                outRom[context.workingOffset++] = 0xE1;
                // ASL
                outRom[context.workingOffset++] = 0x0A;

                // RTL
                outRom[context.workingOffset++] = 0x6B;

                // 7EE1A1 .. sourced from 193
                // enemy defenses
                // 01 weak to gnome
                // 02 weak to sylphid
                // 04 weak to undine
                // 08 weak to salamando
                // 10 weak to shade
                // 20 weak to lumina
                // 40 weak to luna
                // 80 weak to dryad

                // E193:
                // 01 sylphid weak
                // 02 gnome weak
                // 04 salamando weak
                // 08 undine weak
                // 10 lumina weak
                // 20 shade weak
                // 40 dryad weak
                // 80 luna weak
            }

            // $C0/4A6F 29 3F       AND #$3F                A:0008 X:0600 Y:0600 P:envMxdIzc <<
            // $C0 / 4A71 9D A1 E1    STA $E1A1,x[$7E:E7A1]   A: 0008 X: 0600 Y: 0600 P: envMxdIzc <<
            outRom[0x4A6F] = 0x22;
            outRom[0x4A70] = (byte)(context.workingOffset);
            outRom[0x4A71] = (byte)(context.workingOffset >> 8);
            outRom[0x4A72] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x4A73] = 0xEA;

            // orig:
            // 29 3F       AND #$3F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x3F;
            // 9D A1 E1    STA $E1A1,x[$7E:E7A1]
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0xE1;
            // new code:
            // BD 93 E1    LDA $E193,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x93;
            outRom[context.workingOffset++] = 0xE1;
            // 29 40       AND #40
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x40;
            // F0 08       BEQ over [8]
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // BD A1 E1    LDA $E1A1,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0xE1;
            // 09 80       ORA #80
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x80;
            // 9D A1 E1    STA $E1A1,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0xE1;
            // over:
            // BD 93 E1    LDA $E193,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x93;
            outRom[context.workingOffset++] = 0xE1;
            // 29 80       AND #80
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x80;
            // F0 08       BEQ over [8]
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // BD A1 E1    LDA $E1A1,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0xE1;
            // 09 40       ORA #40
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x40;
            // 9D A1 E1    STA $E1A1,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0xE1;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
