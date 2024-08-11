using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Hack that allows cup of wishes to be used on a character who's sitting at 0 HP but hasn't fully died yet, to save time.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CupWishesOnZeroHp : RandoProcessor
    {
        protected override string getName()
        {
            return "Allow cup of wishes on any 0 HP characters";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_CUP_AT_ZERO_HP))
            {
                return false;
            }

            /*
             * replace:
               $D0/DB2A BF 91 01 7E LDA $7E0191,x[$7E:E391] A:0005 X:E200 Y:1814 P:envMxdIZC ** (change to new subr)
               $D0/DB2E 10 1D       BPL $1D    [$DB4D]      A:0080 X:E200 Y:1814 P:eNvMxdIzC ** (change to BCS/BCC on whatever we determine in ^); note 8bit A here
             */

            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 30); // idk a bunch

            outRom[0x10db2a] = 0x22;
            outRom[0x10db2b] = (byte)(context.workingOffset);
            outRom[0x10db2c] = (byte)(context.workingOffset >> 8);
            outRom[0x10db2d] = (byte)((context.workingOffset >> 16) + 0xC0);

            // BCS for error case (can't revive) to skip
            outRom[0x10db2e] = 0xB0;

            // removed code; check dead status
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x91;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x7E;
            // BPL over (2)
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x02;
            // CLC - can revive since dead
            outRom[context.workingOffset++] = 0x18;
            // RTL - blorf
            outRom[context.workingOffset++] = 0x6B;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7e0182,x - additionally check hp
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x7E;
            // BNE over (4)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x04;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // CLC - can revive - not dead yet, but at 0 hp
            outRom[context.workingOffset++] = 0x18;
            // RTL - blerf
            outRom[context.workingOffset++] = 0x6B;

            // default - can't use
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // SEC - indicate inability to use
            outRom[context.workingOffset++] = 0x38;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
