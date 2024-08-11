using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Every time an enemy spawns, randomly choose what type of enemy it is.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EnemyRandomizer : RandoProcessor
    {
        protected override string getName()
        {
            return "Random enemy spawns hack";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // every enemy is random

            /*
             * replace:
                $00/DD69 B7 0D       LDA [$0D],y[$C8:DC1C]   A:1B09 X:0000 Y:0004 P:envmxdIzc
                $00/DD6B 9D 0C C8    STA $C80C,x[$7E:C80C]   A:0240 X:0000 Y:0004 P:envmxdIzc
                ^ map loading; species is the 02 there
             */
            outRom[0xDD69] = 0x22;
            outRom[0xDD6A] = (byte)(context.workingOffset);
            outRom[0xDD6B] = (byte)(context.workingOffset >> 8);
            outRom[0xDD6C] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0xDD6D] = 0xEA;

            byte[] noRandomizeEnemies = new byte[] {
                    0x3F, // shadow x2
                    0x2E, // shadow x1
                    0x20, // shadow x3
                    0x09, // ghost
                    0x0F, // cloud
                    0x1D, // cloud
                    0x30, // ghost
                };

            // normal enemies: 0x53 and below, excluding the above
            // LDA [$0D],y[$C8:DC1C] - oc
            outRom[context.workingOffset++] = 0xB7;
            outRom[context.workingOffset++] = 0x0D;
            // STA $C80C,x[$7E:C80C] - oc
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x0C;
            outRom[context.workingOffset++] = 0xC8;
            // XBA
            outRom[context.workingOffset++] = 0xEB;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // CMP #54 - normal enemy limit
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x54;
            // BLT/BCC over (03)
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x03;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // check each one of these
            foreach (byte noRandomizeEnemy in noRandomizeEnemies)
            {
                // CMP #xx
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = noRandomizeEnemy;
                // BNE over (03)
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x03;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }

            // repeat: - until we get a value in the normal enemy range
            // JSL C03872 - vanilla rng
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x72;
            outRom[context.workingOffset++] = 0x38;
            outRom[context.workingOffset++] = 0xC0;
            // AND #7F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x7F;
            // CMP #54
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x54;
            // BLT/BCC over (02)
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;
            // BRA repeat
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xF4;
            int iter = 0;
            // make sure we didn't get one of these; branch back and try again if so
            foreach (byte noRandomizeEnemy in noRandomizeEnemies)
            {
                // CMP #xx
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = noRandomizeEnemy;
                // BNE over (02)
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x02;
                // BRA repeat
                outRom[context.workingOffset++] = 0x80;
                outRom[context.workingOffset++] = (byte)(0xEE - (iter * 6));
                iter++;
            }
            // 9D 0D C8   STA $C80D,x[$7E:C80D]
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x0D;
            outRom[context.workingOffset++] = 0xC8;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }

    }
}
