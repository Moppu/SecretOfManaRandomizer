using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Some sort of fix for hexas that I don't really remember.  May or may not be needed still.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class HexasFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Hexas fix";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // i don't actually know what this does but it's been in the common settings for ages, so we still include it
            // MOPPLE: maybe someone should figure out what this does

            /*
                C2/0638:	BFF005C2	LDA $C205F0,X
                C2/063C:	395A00  	AND $005A,Y
                C2/063F:	C220    	REP #$20
                C2/0641:	D049    	BNE $068C
                C2/0643:	A552    	LDA $52
             */
            outRom[0x20641] = 0xEA;
            outRom[0x20642] = 0xEA;
            return true;
        }
    }
}
