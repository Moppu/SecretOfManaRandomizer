using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that removes a few less-necessary sounds in favor of having more music channels playing.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NoFootstepSound : RandoProcessor
    {
        protected override string getName()
        {
            return "Sound effects reduction";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_FOOTSTEP_SOUND))
            {
                return false;
            }

            // $01/D4FD 22 04 00 C3 JSL $C30004[$C3:0004]   A:C766 X:0600 Y:0002 P:eNvMxdIzc

            // replace with filter to the sound we don't want to play.
            outRom[0x1D4FD] = 0x22;
            outRom[0x1D4FE] = (byte)(context.workingOffset);
            outRom[0x1D4FF] = (byte)(context.workingOffset >> 8);
            outRom[0x1D500] = (byte)((context.workingOffset >> 16) + 0xC0);

            // also here
            outRom[0x4172] = 0x22;
            outRom[0x4173] = (byte)(context.workingOffset);
            outRom[0x4174] = (byte)(context.workingOffset >> 8);
            outRom[0x4175] = (byte)((context.workingOffset >> 16) + 0xC0);

            // and here, for bosses
            outRom[0x230F8] = 0x22;
            outRom[0x230F9] = (byte)(context.workingOffset);
            outRom[0x230FA] = (byte)(context.workingOffset >> 8);
            outRom[0x230FB] = (byte)((context.workingOffset >> 16) + 0xC0);

            // PHP
            outRom[context.workingOffset++] = 0x08;
            // REP #20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // LDA 7E1E01
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x1E;
            outRom[context.workingOffset++] = 0x7E;

            // actually don't remember what each of these was, but one was the footstep noise when you're running or walking
            int[] soundEffectFilters = new int[] { 0x0F8B, 0x004F, 0x0F50 };

            for(int i=0; i < soundEffectFilters.Length; i++)
            {
                // CMP #soundEffectFilters[i]
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = (byte)soundEffectFilters[i];
                outRom[context.workingOffset++] = (byte)(soundEffectFilters[i]>>8);

                int numItemsLeft = soundEffectFilters.Length - i - 1;
                // BEQ end
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = (byte)(0x04 + numItemsLeft * 5);
            }

            // 22 04 00 C3 (orig code)
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xC3;

            // PLP
            outRom[context.workingOffset++] = 0x28;

            // rtl
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
