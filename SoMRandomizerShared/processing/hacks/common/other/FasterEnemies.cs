using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// An experimental hack to make regular enemies more aggressive.  Could use a little work still.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FasterEnemies : RandoProcessor
    {
        protected override string getName()
        {
            return "Aggressive non-boss enemies";
        }

        // MOPPLE: this needs to be documented.  i don't actually remember what half of this is even doing anymore
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_AGGRESSIVE_ENEMIES))
            {
                return false;
            }

            bool playerWeaponsDontTakeStamina = settings.getBool(CommonSettings.PROPERTYNAME_NO_WEAPON_STAMINA_COST);

            byte minSpeed = 3;

            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 0x200);

            // movement speeds table
            int tableOffset = context.workingOffset;
            int tableOffset1 = context.workingOffset + 1;
            for (int i=0; i < 0x70; i++)
            {
                byte v = outRom[0x2AF05 + i];
                if (v == 0x01)
                {
                    v = 0x02;
                }
                if (v == 0x81)
                {
                    v = 0x82;
                }
                if (v == 0x02)
                {
                    v = 0x03;
                }
                if (v == 0x82)
                {
                    v = 0x83;
                }
                outRom[context.workingOffset++] = v;
            }

            // replace 1D698 - 5 bytes
            outRom[0x1D698] = 0x22;
            outRom[0x1D699] = (byte)(context.workingOffset);
            outRom[0x1D69A] = (byte)(context.workingOffset >> 8);
            outRom[0x1D69B] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1D69C] = 0xEA;

            // new subr: (8 bit A, 16 bit xy)
            // CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BGE over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x06;
            // (replaced code) - non-enemy target - return immediately
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x0F;
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xC7;
            // (rtl)
            outRom[context.workingOffset++] = 0x6B;
            // over:
            // //////////////////////////
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA E180,x - check target species
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xE1;
            // CMP #x54 - normal enemy range
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x54;
            // BLT over2
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x07;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // (replaced code) - non-normal-enemy target - return immediately
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x0F;
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xC7;
            // (rtl)
            outRom[context.workingOffset++] = 0x6B;
            // over2:
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // //////////////////////////
            // STA $0F (replaced)
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x0F;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // TYX
            outRom[context.workingOffset++] = 0xBB;
            // LDA $tableAddr, x - load modified movement speed for enemies
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)(tableOffset);
            outRom[context.workingOffset++] = (byte)(tableOffset >> 8);
            outRom[context.workingOffset++] = (byte)((tableOffset >> 16) + 0xC0);
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // RTL - done
            outRom[context.workingOffset++] = 0x6B;

            /*
$01/D6BD B9 01 C7    LDA $C701,y[$7E:C735]   A:0080 X:0800 Y:0034 P:envMxdIZC
$01/D6C0 C9 80       CMP #$80                A:0081 X:0800 Y:0034 P:eNvMxdIzC
             */

            // replace 1D6BD - 5 bytes
            outRom[0x1D6BD] = 0x22;
            outRom[0x1D6BE] = (byte)(context.workingOffset);
            outRom[0x1D6BF] = (byte)(context.workingOffset >> 8);
            outRom[0x1D6C0] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1D6C1] = 0xEA;

            // new subr: (8 bit A, 16 bit xy)
            // CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BGE over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x06;
            // (replaced code)
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0xC7;
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x80;
            // (rtl)
            outRom[context.workingOffset++] = 0x6B;
            // over:
            // //////////////////////////
            // LDA E180,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xE1;
            // CMP #x54
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x54;
            // BLT over2
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x06;
            // (replaced code)
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0xC7;
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x80;
            // (rtl)
            outRom[context.workingOffset++] = 0x6B;
            // over2:
            // //////////////////////////
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // TYX
            outRom[context.workingOffset++] = 0xBB;
            // LDA $tableAddr, x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)(tableOffset1);
            outRom[context.workingOffset++] = (byte)(tableOffset1 >> 8);
            outRom[context.workingOffset++] = (byte)((tableOffset1 >> 16) + 0xC0);
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // CMP #80 (replaced)
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x80;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            /*
             * animations
$01/D530 B7 00       LDA [$00],y[$D1:B45B]   A:B4F0 X:0800 Y:0001 P:envmxdIzc
$01/D532 85 0C       STA $0C    [$00:000C]   A:0901 X:0800 Y:0001 P:envmxdIzc

            ..

$01/D557 A5 0C       LDA $0C    [$00:000C]   A:B480 X:0800 Y:0001 P:eNvMxdIzC
$01/D559 29 0F       AND #$0F                A:B401 X:0800 Y:0001 P:envMxdIzC

             */
            outRom[0x1D557] = 0x22;
            outRom[0x1D558] = (byte)(context.workingOffset);
            outRom[0x1D559] = (byte)(context.workingOffset >> 8);
            outRom[0x1D55A] = (byte)((context.workingOffset >> 16) + 0xC0);

            // LDA $0C
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x0C;
            // AND #0F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // CPX #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BGE over1 (1)
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // over1:
            // CMP #00
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            // BNE over2 (1)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // over2:
            // CMP #04
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = minSpeed;
            // BGE over (2)
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x02;
            // LDA #04
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = minSpeed;
            // CMP 0x10 - minSpeed
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = (byte)(0x10 - minSpeed);
            // BLT over3 (2)
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;
            // LDA 0x10 - minSpeed
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)(0x10 - minSpeed);
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            /*
$01/D56F A5 0C       LDA $0C    [$00:000C]   A:B481 X:0800 Y:0001 P:envMxdIzC
$01/D571 4A          LSR A                   A:B401 X:0800 Y:0001 P:envMxdIzC
$01/D572 4A          LSR A                   A:B400 X:0800 Y:0001 P:envMxdIZC
$01/D573 4A          LSR A                   A:B400 X:0800 Y:0001 P:envMxdIZc
$01/D574 4A          LSR A                   A:B400 X:0800 Y:0001 P:envMxdIZc
             */

            outRom[0x1D56F] = 0x22;
            outRom[0x1D570] = (byte)(context.workingOffset);
            outRom[0x1D571] = (byte)(context.workingOffset >> 8);
            outRom[0x1D572] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1D573] = 0xEA;
            outRom[0x1D574] = 0xEA;

            // LDA $0C
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x0C;
            // LSR x4
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            // CPX #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BGE over1 (1)
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // over1:
            // CMP #00
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            // BNE over2 (1)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // over2:
            // CMP #04
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = minSpeed;
            // BGE over (2)
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x02;
            // LDA #04
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = minSpeed;
            // CMP 0x10 - minSpeed
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = (byte)(0x10 - minSpeed);
            // BLT over3 (2)
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;
            // LDA 0x10 - minSpeed
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)(0x10 - minSpeed);
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // 30 80 6D 00 -> FF
            // "niceness" check? / weapon recharge for enemies
            for(int i=0x100000; i < 0x110000; i++)
            {
                if(outRom[i] == 0x30 && outRom[i + 1] == 0x80 && outRom[i + 2] == 0x6D && outRom[i + 3] == 0x00)
                {
                    outRom[i + 3] = 0xFF;
                }
            }

            // $C1/207F 90 05       BCC $05    [$2086]      A:EAFF X:003B Y:EA00 P:envMxdIZC
            // remove to skip delays in ai?
            outRom[0x1207f] = 0xEA;
            outRom[0x12080] = 0xEA;

            // $00/F528 D0 03       BNE $03    [$F52D]      A:0037 X:0800 Y:0000 P:envMxdIzC
            outRom[0xF528] = 0xEA;
            outRom[0xF529] = 0xEA;

            outRom[0xF942] = 0x22;
            outRom[0xF943] = (byte)(context.workingOffset);
            outRom[0xF944] = (byte)(context.workingOffset >> 8);
            outRom[0xF945] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0xF946] = 0xEA;

            if (!playerWeaponsDontTakeStamina)
            {
                // CPX #600
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x06;
                // BGE over (6)
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0x06;
                // (removed code)
                outRom[context.workingOffset++] = 0x69;
                outRom[context.workingOffset++] = 0x32;
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0xED;
                outRom[context.workingOffset++] = 0xE1;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            // over:
            // LDA #01 for enemies
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA the same
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xED;
            outRom[context.workingOffset++] = 0xE1;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            outRom[0x2C248] = 0x22;
            outRom[0x2C249] = (byte)(context.workingOffset);
            outRom[0x2C24A] = (byte)(context.workingOffset >> 8);
            outRom[0x2C24B] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x2C24C] = 0xEA;

            if (!playerWeaponsDontTakeStamina)
            {
                // CPX #600
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x06;
                // BGE over (6)
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0x06;
                // (removed code)
                outRom[context.workingOffset++] = 0x69;
                outRom[context.workingOffset++] = 0x32;
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0xED;
                outRom[context.workingOffset++] = 0xE1;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            // over:
            // LDA #01 for enemies
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA the same
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xED;
            outRom[context.workingOffset++] = 0xE1;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            return true;
        }
    }
}
