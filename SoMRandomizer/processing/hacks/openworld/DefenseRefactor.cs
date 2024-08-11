using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Change defense formula to try to balance early/late game damage and spell splitting better.
    /// This is highly experimental and still needs work.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DefenseRefactor : RandoProcessor
    {
        protected override string getName()
        {
            return "Defense refactor";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_DEFENSE_REFACTOR))
            {
                return false;
            }

            // MOPPLE: this all needs work still! its main purpose was to reduce the effect of splitting
            // spells to multiple targets by using a multiplicative defense method, rather than vanilla's
            // pure subtractive one.  a better version of this probably incorporates both a multiplicative
            // and subtractive factor based on the raw defense value, leaning more toward multiplicative
            // later on in the game (higher def values).  this version is pure multiplicative to replace
            // the pure subtractive in vanilla, and is mostly an experiment as-is.
            
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 100);

            // utility method to do a 16 bit * 8 bit multiply, which isn't directly supported by SNES architecture
            // when calling this, use 16bit A (20), 8bit X/Y (10)
            // X is ignored, and corrupted by the call
            // performs: (16bit) A *= (8bit) Y
            // example values below for multiplying:
            // A = 0x1234
            // Y = 0x56
            // Result = 0x61D78
            // Resulting A = 0x1D78 (lower 16 bits)
            // Resulting Y = 0x06 (upper 8 bits)
            int mul16BitSubrOffset = context.workingOffset;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // TYX
            outRom[context.workingOffset++] = 0xBB; // X = [0x56] (copy of Y)
            // PHB - save off previous bank
            outRom[context.workingOffset++] = 0x8B;
            // PHA - save off A temporarily
            outRom[context.workingOffset++] = 0x48;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // PHA - push 0 to restore as the bank
            outRom[context.workingOffset++] = 0x48;
            // PLB - set bank to 0
            outRom[context.workingOffset++] = 0xAB;
            // PLA - pull saved A
            outRom[context.workingOffset++] = 0x68;
            // multiply lower 8 bits of A * Y
            // sta 4202
            outRom[context.workingOffset++] = 0x8D; // $4202 = [0x34]
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x42;
            // sty 4203
            outRom[context.workingOffset++] = 0x8C; // $4203 = [0x56]
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x42;
            // nop to wait
            outRom[context.workingOffset++] = 0xEA; // $4216 = [0x78] (LSB of result 0x34 * 0x56 = 0x1178)
            outRom[context.workingOffset++] = 0xEA; // $4217 = [0x11] (MSB of result 0x34 * 0x56 = 0x1178)
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            // lda 4216 - A = LSB of result
            outRom[context.workingOffset++] = 0xAD; // A = [0x12,0x78] (set LSB only since we're SEP 20)
            outRom[context.workingOffset++] = 0x16;
            outRom[context.workingOffset++] = 0x42;
            // ldy 4217 - Y = MSB of result
            outRom[context.workingOffset++] = 0xAC; // Y = [0x11]
            outRom[context.workingOffset++] = 0x17;
            outRom[context.workingOffset++] = 0x42;
            // xba - swap LSB and MSB of A
            outRom[context.workingOffset++] = 0xEB; // A = [0x78,0x12]
            // multiply upper 8 bits of A * Y (copied in X)
            // sta 4202
            outRom[context.workingOffset++] = 0x8D; // $4202 = [0x12]
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x42;
            // stx 4203
            outRom[context.workingOffset++] = 0x8E; // $4203 = [0x56]
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x42;
            // nop to wait for the multiply
            outRom[context.workingOffset++] = 0xEA; // $4216 = [0x0C] (LSB of result 0x12 * 0x56 = 0x60C)
            outRom[context.workingOffset++] = 0xEA; // $4217 = [0x06] (MSB of result 0x12 * 0x56 = 0x60C)
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            // tya 
            outRom[context.workingOffset++] = 0x98; // A = [0x78,0x11] (again, set LSB only because 8-bit A mode)
            // clc
            outRom[context.workingOffset++] = 0x18; // prepare to add 0x0C to
            // adc 4216
            outRom[context.workingOffset++] = 0x6D; // A = [0x78,0x1D]
            outRom[context.workingOffset++] = 0x16;
            outRom[context.workingOffset++] = 0x42;
            // LDY $4217
            outRom[context.workingOffset++] = 0xAC; // Y = [0x06]
            outRom[context.workingOffset++] = 0x17;
            outRom[context.workingOffset++] = 0x42;
            // BCC/BLT over the INY
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x01;
            // INY for addition overflow (numeric example here doesn't encounter this)
            outRom[context.workingOffset++] = 0xC8; // Y = [0x06]
            // XBA
            outRom[context.workingOffset++] = 0xEB; // A = [0x1D,0x78]
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLB - restore previous bank
            outRom[context.workingOffset++] = 0xAB;
            // RTL - done
            outRom[context.workingOffset++] = 0x6B;

            // table of values representing the percentage of damage allowed through defense at each defense value
            // these are values out of 64, starting near 64 (damageMaximum) for low values of defense (allowing most damage through)
            // and collapsing down to near 0 (damageMinimum) for high values of defense (reject most damage)
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 1000);
            int defenseTableOffset = context.workingOffset;
            int maxDef = 500;
            for(int i=0; i < 1000; i++)
            {
                int def = i;
                if(def > maxDef)
                {
                    def = maxDef;
                }

                // for now - probably want to scale up fast early
                double ratio = Math.Pow(((maxDef + 1) - def) / (double)maxDef, 1.5);

                // damage amount / 64
                int damageMinimum = 1;
                int damageMaximum = 63;
                outRom[context.workingOffset++] = (byte)(damageMinimum + ratio * (damageMaximum - damageMinimum));
            }

            // this is the same as the above table, but for magic damage.
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 1000);
            int magicDefenseTableOffset = context.workingOffset;
            for (int i = 0; i < 1000; i++)
            {
                int def = i;
                if (def > maxDef)
                {
                    def = maxDef;
                }

                // for now - probably want to scale up fast early
                double ratio = Math.Pow(((maxDef + 1) - def) / (double)maxDef, 1.1);

                // damage amount / 64
                int damageMinimum = 4;
                int damageMaximum = 63;
                outRom[context.workingOffset++] = (byte)(damageMinimum + ratio * (damageMaximum - damageMinimum));
            }

            // the two subroutines below (for phys, and magic damage respectively) load the value in the above table associated
            // with your current defense, multiply by the incoming damage (using the 16x8 bit multiply subroutine above),
            // and divide the result by 64 (via shifts), effectively giving us a damage scale of incomingDamage * tableValue / 64

            /*
             *  replace: (load damage in $89, and subtract phys defense in $94)
             *  note the max that can be in $89 is 999 (0x3e7) for max damage
                $C0/516B A5 89       LDA $89    [$00:0389]   A:0F00 X:0000 Y:0600 P:envmxdIZC
                $C0/516D 38          SEC                     A:000D X:0000 Y:0600 P:envmxdIzC - max 3e7 (999) .. can safely be multiplied by x40 (64) and fit
                $C0/516E E5 94       SBC $94    [$00:0394]   A:000D X:0000 Y:0600 P:envmxdIzC - max 500ish from gear? some con component too, hard caps at 999
             */
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 100);

            outRom[0x516B] = 0x22;
            outRom[0x516C] = (byte)(context.workingOffset);
            outRom[0x516D] = (byte)(context.workingOffset >> 8);
            outRom[0x516E] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x516F] = 0xEA;

            injectNewDefCode(outRom, context, defenseTableOffset, mul16BitSubrOffset, 0x0c);


            /*
             *  replace: (load damage in $89, and subtract magic defense in $94)
             *  note the max that can be in $89 is 999 (0x3e7) for max damage
                $C8/E9E1 A5 89       LDA $89    [$00:0389]   A:0038 X:0000 Y:0600 P:envmxdIzC
                $C8/E9E3 38          SEC                     A:00E2 X:0000 Y:0600 P:envmxdIzC
                $C8/E9E4 E5 94       SBC $94    [$00:0394]   A:00E2 X:0000 Y:0600 P:envmxdIzC
             */
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 100);

            outRom[0x8e9e1] = 0x22;
            outRom[0x8e9e2] = (byte)(context.workingOffset);
            outRom[0x8e9e3] = (byte)(context.workingOffset >> 8);
            outRom[0x8e9e4] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8e9e5] = 0xEA;

            injectNewDefCode(outRom, context, magicDefenseTableOffset, mul16BitSubrOffset, 0x04);

            return true;
        }


        private void injectNewDefCode(byte[] outRom, RandoContext context, int defenseTableOffset, int mul16BitSubrOffset, byte subtractor)
        {
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // PHY
            outRom[context.workingOffset++] = 0x5A;
            // LDA $94 - defense
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x94;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA defenseTableOffset,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)(defenseTableOffset);
            outRom[context.workingOffset++] = (byte)(defenseTableOffset >> 8);
            outRom[context.workingOffset++] = (byte)((defenseTableOffset >> 16) + 0xC0);

            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;

            // TAY
            outRom[context.workingOffset++] = 0xA8;

            // SEP #10
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x10;

            // LDA $89
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x89;

            // JSL mul16BitSubrOffset
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(mul16BitSubrOffset);
            outRom[context.workingOffset++] = (byte)(mul16BitSubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((mul16BitSubrOffset >> 16) + 0xC0);
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // REP #10
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x10;

            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // apply rng too
            // JSL C03872
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x72;
            outRom[context.workingOffset++] = 0x38;
            outRom[context.workingOffset++] = 0xC0;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND #000F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            outRom[context.workingOffset++] = 0x00;
            // STA $89
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x89;
            // PLA
            outRom[context.workingOffset++] = 0x68;

            // CMP #0040
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x40;
            outRom[context.workingOffset++] = 0x00;
            // BGE over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x00;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA $89
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x89;
            // AND #0003
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x00;
            // STA $89
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x89;
            // PLA
            outRom[context.workingOffset++] = 0x68;

            // over:
            // CLC
            outRom[context.workingOffset++] = 0x38;
            // ADC $89
            outRom[context.workingOffset++] = 0x65;
            outRom[context.workingOffset++] = 0x89;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #0008 -> 18
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = subtractor;
            outRom[context.workingOffset++] = 0x00;

            // more experimentals
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA $94
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x94;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // STA $89
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x89;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC $89
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x89;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
        }
    }
}
