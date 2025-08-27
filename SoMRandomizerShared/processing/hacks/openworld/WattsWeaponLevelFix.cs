using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// A fix to an issue where weapons were not properly refreshed after upgrading at Watts until swapping them off and back on.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class WattsWeaponLevelFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Watts weapon level fix";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // $C0/7C20 9F E8 E1 7E STA $7EE1E8,x[$7E:E1E8] A:001C X:0000 Y:0003 P:envMxdIzc
            // basically just need to update 1E3, too
            outRom[0x7C20] = 0x22;
            outRom[0x7C21] = (byte)(context.workingOffset);
            outRom[0x7C22] = (byte)(context.workingOffset >> 8);
            outRom[0x7C23] = (byte)((context.workingOffset >> 16) + 0xC0);
            // STA $7EE1E8
            outRom[context.workingOffset++] = 0x9F; // replaced code
            outRom[context.workingOffset++] = 0xE8; // replaced code
            outRom[context.workingOffset++] = 0xE1; // replaced code
            outRom[context.workingOffset++] = 0x7E; // replaced code
            // STA $7EE1E3
            outRom[context.workingOffset++] = 0x9F; // new code
            outRom[context.workingOffset++] = 0xE3; // new code
            outRom[context.workingOffset++] = 0xE1; // new code
            outRom[context.workingOffset++] = 0x7E; // new code
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            return true;
        }
    }
}
