using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Hack that makes every enemy on the wind palace map invincible, in support of a separate change that adds a rabite to that map for spell leveling.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class InvincibleRabite : RandoProcessor
    {
        protected override string getName()
        {
            return "Indestructible rabite in the wind palace";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            /*
             * replace part of code that is subtracting hp for damage:
                $C0/40B2 BD 82 E1    LDA $E182,x[$7E:E582]   A:0F50 X:0400 Y:001F P:eNvmxdIzC - for map xxx (273?), just load 1000 here
                $C0/40B5 38          SEC                     A:001F X:0400 Y:001F P:envmxdIzC
                $C0/40B6 FD F1 E1    SBC $E1F1,x[$7E:E5F1]   A:001F X:0400 Y:001F P:envmxdIzC
                $C0/40B9 B0 03       BCS $03    [$40BE]      A:FFF0 X:0800 Y:0001 P:eNvmxdIzc  - check for death
                $C0/40BB A9 00 00    LDA #$0000              A:FFF0 X:0800 Y:0001 P:eNvmxdIzc
                modify to check for the wind palace map, and make any enemy there indestructible by not processing this damage. 
             */

            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 100); // - a bunch of space

            outRom[0x40B2] = 0x22;
            outRom[0x40B3] = (byte)(context.workingOffset);
            outRom[0x40B4] = (byte)(context.workingOffset >> 8);
            outRom[0x40B5] = (byte)((context.workingOffset >> 16) + 0xC0);

            // LDA 7E00DC
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            ushort mapNum = SomVanillaValues.MAPNUM_WINDSEED;
            // CMP #111
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = (byte)(mapNum);
            outRom[context.workingOffset++] = (byte)(mapNum >> 8);
            // BEQ over (4)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x05;
            // (removed code)
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x38;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // over:
            // CPX #0600 - only do this for enemies, not players
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BGE over2
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x05;
            // (same code)
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x38;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // over2:
            // LDA #03E8 - give the rabite 1000 hp every time damage is processed, so you basically cannot possibly kill it
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0x03;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // See OpenWorldSupportingMapChanges for the placement of this rabite on the map.

            return true;
        }
    }
}
