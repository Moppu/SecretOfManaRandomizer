using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that makes poison ticks deal 1/32 of your max HP.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class PercentagePoison : RandoProcessor
    {
        protected override string getName()
        {
            return "Poison deals percentage damage";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_PERCENTAGE_POISON))
            {
                return false;
            }
            
            bool poisonKillsYou = settings.getBool(CommonSettings.PROPERTYNAME_PERMANENT_POISON);

            // replace two blocks below; add one subroutine to subtract HP based on a percentage, and one
            // to check HP afterward to figure out if poison should be turned off

            /*
                 $C0/3D6D 09 40       ORA #$40                  ***
                 $C0/3D6F 85 80       STA $80    [$00:0380]     ***
                 $C0/3D71 C2 20       REP #$20                  
                 $C0/3D73 BD 82 E1    LDA $E182,x[$7E:E182]     ***
                 $C0/3D76 F0 0E       BEQ $0E    [$3D86]        ***
                 $C0/3D78 3A          DEC A                     ---
                 $C0/3D79 F0 0B       BEQ $0B    [$3D86]        ---
             */
            outRom[0x3D6D] = 0x22;
            outRom[0x3D6E] = (byte)(context.workingOffset);
            outRom[0x3D6F] = (byte)(context.workingOffset >> 8);
            outRom[0x3D70] = (byte)((context.workingOffset >> 16) + 0xC0);

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA $E184,x - max hp
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x84;
            outRom[context.workingOffset++] = 0xE1;
            // LSR x5 - divide by 32
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA $E182,x - current hp
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0xE1;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC sr,1 - subtract the value we pushed earlier
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x01;
            // BCS/BGE over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x03;
            // if we went to 0 or negative with poison, LDA #0001 or #0000 based on whether poison should be able to kill you
            if (poisonKillsYou)
            {
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x00;
            }
            else
            {
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x01;
                outRom[context.workingOffset++] = 0x00;
            }
            // over:
            // STA $E182,x - write current HP back
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0xE1;
            // PLA, PLA - even out the stack
            outRom[context.workingOffset++] = 0x68;
            outRom[context.workingOffset++] = 0x68;
            // SEP #20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // ORA #40 (replaced code)
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x40;
            // STA $80 (replaced code)
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x80;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // determine whether poison should be turned off based on your current HP
            // SEC for yes, CLC for no, and then we BCS after the return to do similar logic to what vanilla did when HP == 0
            outRom[0x3D73] = 0x22;
            outRom[0x3D74] = (byte)(context.workingOffset);
            outRom[0x3D75] = (byte)(context.workingOffset >> 8);
            outRom[0x3D76] = (byte)((context.workingOffset >> 16) + 0xC0);

            // LDA $E182,x - current hp
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0xE1;

            if(!poisonKillsYou)
            {
                // CMP #0001
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = 0x01;
                outRom[context.workingOffset++] = 0x00;
                // BNE over
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x02;
                // SEC (for branch after RTL) - disable poison since we're at minimum hp
                outRom[context.workingOffset++] = 0x38;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            // over:
            // CMP #0000
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            // SEC (for branch after RTL) - disable poison because we're dead
            outRom[context.workingOffset++] = 0x38;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // CLC (for branch after RTL) - not dead; stay poisoned
            outRom[context.workingOffset++] = 0x18;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // BCS/BGE 0D - branch to where C0/3D76 used to branch for 0 hp, but based on whether we set the carry bit
            // this branch location turns off poison because you're either at 1 hp in vanilla (and it can't poison anymore)
            // or you're already dead
            outRom[0x3D77] = 0xB0;
            outRom[0x3D78] = 0x0D;
            // remove the BEQ to the same spot after DEC A for basic poison, which has been removed
            outRom[0x3D79] = 0xEA;
            outRom[0x3D7A] = 0xEA;

            return true;
        }
    }
}
