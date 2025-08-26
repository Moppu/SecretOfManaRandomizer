using SoMRandomizer.config.settings;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Workaround hack for issue where being "slowed" makes event 0x780 only send you halfway through gaia's navel walls. 
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class InvisClearsSlow : RandoProcessor
    {
        protected override string getName()
        {
            return "Invisible passageways clear slow condition";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // C1/E92A:	26EA		[Command 04]
            // 7ee190 0x04 slow

            // C1 / EA26:	AD0EE0  	LDA $E00E
            // C1 / EA29:	4980        EOR #$80		[Toggle bit #$80]
            // C1 / EA2B:	8D0EE0      STA $E00E

            outRom[0x1EA26] = 0x22;
            outRom[0x1EA27] = (byte)context.workingOffset;
            outRom[0x1EA28] = (byte)(context.workingOffset >> 8);
            outRom[0x1EA29] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1EA2A] = 0xEA;
            outRom[0x1EA2B] = 0xEA;
            outRom[0x1EA2C] = 0xEA;
            outRom[0x1EA2D] = 0xEA;
            // original code first
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x0E;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x49;
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x0E;
            outRom[context.workingOffset++] = 0xE0;

            // LDA E190
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE1;

            // AND FB
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFB;

            // STA E190
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE1;

            // LDA E390
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE3;

            // AND FB
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFB;

            // STA E390
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE3;

            // LDA E590
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE5;

            // AND FB
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFB;

            // STA E590
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE5;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
