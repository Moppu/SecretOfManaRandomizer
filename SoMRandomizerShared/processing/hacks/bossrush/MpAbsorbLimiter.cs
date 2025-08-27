using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.bossrush
{
    /// <summary>
    /// Hack that only allows you to absorb 3 mp max to prevent the spell from overpowering boss rush mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MpAbsorbLimiter : RandoProcessor
    {
        protected override string getName()
        {
            return "Limit MP absorb to 3 mp";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(BossRushSettings.PROPERTYNAME_LIMIT_MP_STEAL))
            {
                return false;
            }

            byte limit = 3;

            // $C8/EC8A 65 A4       ADC $A4    [$00:03A4]   A:0000 X:0600 Y:0400 P:envMxdIZc
            // $C8 / EC8C 99 F6 E1    STA $E1F6,y[$7E:E5F6]   A: 000F X: 0600 Y: 0400 P: envMxdIzc
            // ^ writing the amount of "damage" that mp steal does.
            outRom[0x8EC8A] = 0x22;
            outRom[0x8EC8B] = (byte)(context.workingOffset);
            outRom[0x8EC8C] = (byte)(context.workingOffset >> 8);
            outRom[0x8EC8D] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8EC8E] = 0xEA;

            // code we took out
            outRom[context.workingOffset++] = 0x65;
            outRom[context.workingOffset++] = 0xA4;

            // cmp #limit
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = limit;

            // bcc skip
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;

            // lda #limit 
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = limit;

            // more code we took out
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0xF6;
            outRom[context.workingOffset++] = 0xE1;

            // rtl
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
