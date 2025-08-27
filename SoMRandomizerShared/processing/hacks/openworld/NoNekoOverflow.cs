using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Fix Neko's price doubling to allow him to sell items more expensive than 32768 gold.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NoNekoOverflow : RandoProcessor
    {
        protected override string getName()
        {
            return "Neko price overflow fix";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            /*
             *  replace:
                $C0/7CEC C2 20       REP #$20                A:0099 X:FBA0 Y:0064 P:envMxdIZC
                $C0/7CEE 0E 22 18    ASL $1822  [$00:1822]   A:0099 X:FBA0 Y:0064 P:envmxdIZC
             */
            outRom[0x7CEC] = 0x22;
            outRom[0x7CED] = (byte)(context.workingOffset);
            outRom[0x7CEE] = (byte)(context.workingOffset >> 8);
            outRom[0x7CEF] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x7CF0] = 0xEA;

            outRom[context.workingOffset++] = 0xC2; // replaced code
            outRom[context.workingOffset++] = 0x20; // replaced code
            outRom[context.workingOffset++] = 0x0E; // replaced code
            outRom[context.workingOffset++] = 0x22; // replaced code
            outRom[context.workingOffset++] = 0x18; // replaced code
            // BCS over (01) - check overflow on the shift
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // over: (overflow here)
            // LDA #FFFF .. actually use 65000 since FFFF seems to fuck up and show no price at all
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xFD;
            // STA $1822
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x18;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
