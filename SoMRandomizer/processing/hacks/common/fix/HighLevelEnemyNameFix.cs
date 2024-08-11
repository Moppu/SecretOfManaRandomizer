using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.fix
{
    /// <summary>
    /// Fix an issue where loading the names of high-level enemies for the ring menu display would break.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class HighLevelEnemyNameFix : RandoProcessor
    {
        protected override string getName()
        {
            return "High-level enemy name fix";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // add an AND #00FF to this since we're loading an 8-bit value (enemy species)
            // and we don't want to shove the next byte (enemy level) into $19D4, which for high values
            // (enemy levels that vanilla can never reach) causes corruptions of the menu display.

            /*            
$D0/D43B BF 80 01 7E LDA $7E0180,x[$7E:E980] A:0041 X:E800 Y:0120 P:eNvmxdIzc
$D0/D43F 4C D9 D4    JMP $D4D9  [$D0:D4D9]   A:6241 X:E800 Y:0120 P:envmxdIzc
$D0/D4D9 8D D3 19    STA $19D3  [$00:19D3]   A:6241 X:E800 Y:0120 P:envmxdIzc
             */

            outRom[0x10D43B] = 0x22;
            outRom[0x10D43C] = (byte)context.workingOffset;
            outRom[0x10D43D] = (byte)(context.workingOffset >> 8);
            outRom[0x10D43E] = (byte)((context.workingOffset >> 16) + 0xC0);

            // replaced code
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x7E;

            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
