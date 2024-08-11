using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Make spells level 4x faster for less of a grind than vanilla.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FasterMagicLevels : RandoProcessor
    {
        protected override string getName()
        {
            return "Faster magic levels";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_FASTER_MAGIC_LEVELS))
            {
                return false;
            }

            // increment magic level in vanilla..
            // subtract level from 9 to determine increment amount
            // $D0/4DEC A9 09       LDA #$09                A:0800 X:0000 Y:0200 P:eNvMxdIzc
            // $D0/4DEE 38          SEC                     A:0809 X:0000 Y:0200 P:envMxdIzc
            // $D0/4DEF E5 9D       SBC $9D[$00:039D]       A:0809 X:0000 Y:0200 P:envMxdIzC
            // $D0/4DF1 85 AC       STA $AC[$00:03AC]       A:0809 X:0000 Y:0200 P:envMxdIzC

            outRom[0x104DEC] = 0x22;
            outRom[0x104DED] = (byte)(context.workingOffset);
            outRom[0x104DEE] = (byte)(context.workingOffset >> 8);
            outRom[0x104DEF] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x104DF0] = 0xEA;
            outRom[0x104DF1] = 0xEA;
            outRom[0x104DF2] = 0xEA;

            // LDA #09 (vanilla)
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x09;
            // SEC A (vanilla)
            outRom[context.workingOffset++] = 0x38;
            // SBC $9D (vanilla)
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x9D;
            // ASL twice to multiply by 4
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            // STA $AC (vanilla)
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xAC;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
