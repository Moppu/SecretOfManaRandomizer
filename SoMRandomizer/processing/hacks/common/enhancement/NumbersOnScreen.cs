using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Hack that forces damage numbers to be displayed on-screen, so damage dealt to off-screen targets can be seen.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NumbersOnScreen : RandoProcessor
    {
        protected override string getName()
        {
            return "Always show damage numbers on-screen";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            /*
             * replacement code, where it determines sprite positions attached to the character, adding the character screen position (E020, E022)
$00/E52D 7D 20 E0    ADC $E020,x[$7E:E620]   A:FFFC X:0600 Y:0000 P:eNvmxdIzc ** replace - x position for sprite
$00/E530 85 04       STA $04    [$00:0004]   A:0078 X:0600 Y:0000 P:envmxdIzC **
...
$00/E543 7D 22 E0    ADC $E022,x[$7E:E622]   A:FFDD X:0600 Y:0000 P:eNvmxdIzC ** replace - y position for sprite
$00/E546 85 06       STA $06    [$00:0006]   A:0016 X:0600 Y:0000 P:envmxdIzC **
             */

            // from FreeWalking hack - edges of screen
            byte xMenuLowerLimit = 0x08;
            byte xMenuUpperLimit = 0xE0;
            byte yMenuLowerLimit = 0x00;
            byte yMenuUpperLimit = 0xC8;

            outRom[0xE52D] = 0x22;
            outRom[0xE52E] = (byte)context.workingOffset;
            outRom[0xE52F] = (byte)(context.workingOffset >> 8);
            outRom[0xE530] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0xE531] = 0xEA;

            // (replaced code)
            // ADC $E020,x
            outRom[context.workingOffset++] = 0x7D;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0xE0;

            // PHA
            outRom[context.workingOffset++] = 0x48;

            // LDA $E175,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x75;
            outRom[context.workingOffset++] = 0xE1;

            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;

            // CMP #00D8 .. i think this is a palette or sprite comparison?
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xD8;
            outRom[context.workingOffset++] = 0x00;

            // BEQ showingHp
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x0F;

            // LDA E04F,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x4F;
            outRom[context.workingOffset++] = 0xE0;

            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;

            // CMP #00D0 .. sprite palette is 0xD0?
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x00;

            // BEQ showingHp
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x04;

            // PLA
            outRom[context.workingOffset++] = 0x68;

            // (removed code)
            // STA $04
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x04;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // showingHp:
            // PLA - pull x position
            outRom[context.workingOffset++] = 0x68;

            // CMP #8000 - check if negative x
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x80;

            // BCC/BLT past loading the minimum
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x03;

            // LDA xMenuLowerLimit
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = xMenuLowerLimit;
            outRom[context.workingOffset++] = 0x00;

            // CMP xMenuUpperLimit
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = xMenuUpperLimit;
            outRom[context.workingOffset++] = 0x00;

            // BCC/BLT past loading the maximum
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x03;

            // LDA xMenuUpperLimit
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = xMenuUpperLimit;
            outRom[context.workingOffset++] = 0x00;

            // CMP xMenuLowerLimit
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = xMenuLowerLimit;
            outRom[context.workingOffset++] = 0x00;

            // BCS/BGE past loading the minimum
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x03;

            // LDA xMenuLowerLimit
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = xMenuLowerLimit;
            outRom[context.workingOffset++] = 0x00;

            // (Replaced code)
            // STA $04
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x04;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // basically the same as above but for the y location of the number sprite on the screen instead of the x
            outRom[0xE543] = 0x22;
            outRom[0xE544] = (byte)context.workingOffset;
            outRom[0xE545] = (byte)(context.workingOffset >> 8);
            outRom[0xE546] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0xE547] = 0xEA;

            // (replaced code)
            // ADC $E022,x
            outRom[context.workingOffset++] = 0x7D;
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0xE0;

            // PHA
            outRom[context.workingOffset++] = 0x48;

            // LDA $E175,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x75;
            outRom[context.workingOffset++] = 0xE1;

            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;

            // CMP #00D8 .. i think this is a palette or sprite comparison?
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xD8;
            outRom[context.workingOffset++] = 0x00;

            // BEQ showingHp
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x0F;

            // LDA E04F,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x4F;
            outRom[context.workingOffset++] = 0xE0;

            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;

            // CMP #00D0 .. sprite palette is 0xD0?
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x00;

            // BEQ showingHp
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x04;

            // PLA
            outRom[context.workingOffset++] = 0x68;

            // (removed code)
            // STA $04
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x06;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // showingHp:
            // PLA - pull y position
            outRom[context.workingOffset++] = 0x68;

            // CMP #8000 - check if negative y
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x80;

            // BCC/BLT past loading the minimum
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x03;

            // LDA yMenuLowerLimit
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = yMenuLowerLimit;
            outRom[context.workingOffset++] = 0x00;

            // CMP yMenuUpperLimit
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = yMenuUpperLimit;
            outRom[context.workingOffset++] = 0x00;

            // BCC/BLT past loading the maximum
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x03;

            // LDA yMenuUpperLimit
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = yMenuUpperLimit;
            outRom[context.workingOffset++] = 0x00;

            // CMP yMenuLowerLimit
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = yMenuLowerLimit;
            outRom[context.workingOffset++] = 0x00;

            // BCC/BLT past loading the minimum
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x03;

            // LDA yMenuLowerLimit
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = yMenuLowerLimit;
            outRom[context.workingOffset++] = 0x00;

            // (Replaced code)
            // STA $04
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x06;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
