using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Hack that allows randomized weapons to deal bonus damage to particular enemy species types.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EnemyTypeDamage : RandoProcessor
    {
        protected override string getName()
        {
            return "Enemy type/species damage";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_ENEMY_SPECIES_DAMAGE))
            {
                return false;
            }
            // read atk
            // $C0/51F7 B9 98 E1    LDA $E198,y[$7E:E198]   A:0000 X:0A00 Y:0000 P:envMxdIZC
            // $C0/51FA 85 D4       STA $D4    [$00:03D4]   A:0012 X:0A00 Y:0000 P:envMxdIzC
            outRom[0x51F7] = 0x22;
            outRom[0x51F8] = (byte)(context.workingOffset);
            outRom[0x51F9] = (byte)(context.workingOffset >> 8);
            outRom[0x51FA] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x51FB] = 0xEA;

            // [194] is atk type
            // x is target, y is source
            // where is target species type? [192]?

            // replace above with:

            // -- B9 98 E1    LDA $E198,y[$7E:E198] 
            // -- PHA
            // B9 94 E1    LDA $E194,y[$7E:E194] 
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x94;
            outRom[context.workingOffset++] = 0xE1;
            // 3D 92 E1    AND $E192,x[$7E:EA92] 
            outRom[context.workingOffset++] = 0x3D;
            outRom[context.workingOffset++] = 0x92;
            outRom[context.workingOffset++] = 0xE1;
            // F0 12       BEQ over
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x12;
            // B9 98 E1    LDA $E198,y[$7E:E198] 
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x98;
            outRom[context.workingOffset++] = 0xE1;
            // 4A          LSR
            outRom[context.workingOffset++] = 0x4A;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 79 98 E1    ADC $E198,y[$7E:E198] 
            outRom[context.workingOffset++] = 0x79;
            outRom[context.workingOffset++] = 0x98;
            outRom[context.workingOffset++] = 0xE1;
            // D9 98 E1    CMP $E198,y[$7E:E198] 
            outRom[context.workingOffset++] = 0xD9;
            outRom[context.workingOffset++] = 0x98;
            outRom[context.workingOffset++] = 0xE1;
            // B0 02       BGE skipSet
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x02;
            // A9 FF       LDA #FF
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xFF;
            // 85 D4       STA $D4    [$00:03D4]
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xD4;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;
            // over:
            // B9 98 E1    LDA $E198,y[$7E:E198] 
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x98;
            outRom[context.workingOffset++] = 0xE1;
            // 85 D4       STA $D4    [$00:03D4]
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xD4;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
