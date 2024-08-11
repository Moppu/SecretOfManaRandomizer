using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that continually poisons your characters.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AlwaysPoisoned : RandoProcessor
    {
        protected override string getName()
        {
            return "Players are always poisoned";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_PERMANENT_POISON))
            {
                return false;
            }
            /*
            $C0/3B21 22 01 E3 C8 JSL $C8E301[$C8:E301]   A:0000 X:0600 Y:0200 P:envMxdIZc
             */
            outRom[0x3B21] = 0x22;
            outRom[0x3B22] = (byte)context.workingOffset;
            outRom[0x3B23] = (byte)(context.workingOffset >> 8);
            outRom[0x3B24] = (byte)((context.workingOffset >> 16) + 0xC0);

            // here: load 7ecf00, if zero, skip
            // LDA 7ecf00
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // BEQ skip (8+5)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08 + 0x05;

            // CPX #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;

            // BGE skip
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x08;

            // LDA #20 - poison bit
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x20;

            // ORA E191,x - or with existing status
            outRom[context.workingOffset++] = 0x1D;
            outRom[context.workingOffset++] = 0x91;
            outRom[context.workingOffset++] = 0xE1;

            // STA E191,x - write back with poison
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x91;
            outRom[context.workingOffset++] = 0xE1;

            // skip:
            // removed code to process statuses (see also StatusGlow)
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0xC8;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
