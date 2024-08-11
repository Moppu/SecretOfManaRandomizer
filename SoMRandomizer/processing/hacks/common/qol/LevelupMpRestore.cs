using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// A quick hack to restore MP as well as HP when you level up.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class LevelupMpRestore : RandoProcessor
    {
        protected override string getName()
        {
            return "Levelups restore MP";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_LEVELUPS_RESTORE_MP))
            {
                return false;
            }

            // hp restore at levelup
            // $C0/44D3 BD 84 E1    LDA $E184,x[$7E:E184]   A:0000 X:0000 Y:004D P:envmxdIzc
            // $C0/44D6 9D 82 E1    STA $E182,x[$7E:E182]   A:003B X:0000 Y:004D P:envmxdIzc
            // 86,87 are mp, and 8bit

            // 22 to thingy
            outRom[0x44D3] = 0x22;
            outRom[0x44D4] = (byte)(context.workingOffset);
            outRom[0x44D5] = (byte)(context.workingOffset >> 8);
            outRom[0x44D6] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x44D7] = 0xEA;
            outRom[0x44D8] = 0xEA;

            // original - hp
            // LDA $E184,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x84;
            outRom[context.workingOffset++] = 0xE1;
            // STA $E182,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0xE1;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // new - mp
            // LDA $E187,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x87;
            outRom[context.workingOffset++] = 0xE1;
            // STA $E186,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0xE1;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
