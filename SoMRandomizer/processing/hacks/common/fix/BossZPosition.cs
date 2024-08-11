using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.hacks.common.fix
{
    /// <summary>
    /// Hack that adjusts boss's "z" positions to make them consistently hittable across maps they aren't normally meant to be on.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BossZPosition : RandoProcessor
    {
        protected override string getName()
        {
            return "Boss Z-position determination";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(settings.getBool(CommonSettings.PROPERTYNAME_BOSSES_AT_Z_0))
            {
                outRom[0x216B0] = 0xEA;
                outRom[0x216B1] = 0xEA;
                outRom[0x216B2] = 0xEA;

                outRom[0x2750E] = 0xEA;
                outRom[0x2750F] = 0xEA;
                outRom[0x27510] = 0xEA;
                return true;
            }

            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 500); // idk
            // snakes and hydras on tropicallo map (x122) do not work right and z should be locked to 0
            // $C2/16B0 9D 0B 00    STA $000B,x[$7E:E60B]   A:0003 X:E600 Y:02A4 P:envMxdIzc
            // $C2/16B3 C2 20       REP #$20                A:0003 X:E600 Y:02A4 P:envMxdIzc

            // hydras, snakes, tropicallos, spring beaks
            List<byte> reallyBrokenBosses = new byte[] {
                SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_HYDRA,
                SomVanillaValues.BOSSID_GREATVIPER,
                SomVanillaValues.BOSSID_DRAGONWORM,
                SomVanillaValues.BOSSID_TROPICALLO,
                SomVanillaValues.BOSSID_BOREAL,
                SomVanillaValues.BOSSID_AXEBEAK,
                SomVanillaValues.BOSSID_SPRINGBEAK
            }.ToList();

            int reallyBrokenSubr = context.workingOffset;
            foreach (byte bossNum in reallyBrokenBosses)
            {
                // next:
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA 01E7,x - note that x is E600, E800, etc - load species
                outRom[context.workingOffset++] = 0xBD;
                outRom[context.workingOffset++] = 0xE7;
                outRom[context.workingOffset++] = 0x01;
                // CMP #bossNum
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = bossNum;
                // BNE next
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x08;
                // LDA #00
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x00;
                // STA $000B,x
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x0B;
                outRom[context.workingOffset++] = 0x00;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            // next:
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            outRom[0x216B0] = 0x22;
            outRom[0x216B1] = (byte)(context.workingOffset);
            outRom[0x216B2] = (byte)(context.workingOffset >> 8);
            outRom[0x216B3] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x216B4] = 0xEA;

            // (removed code)
            // STA $000B,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0x00;
            // REP #$20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // PHA
            outRom[context.workingOffset++] = 0x48;

            // fix "really broken" bosses above by setting their Z to 0
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(reallyBrokenSubr);
            outRom[context.workingOffset++] = (byte)(reallyBrokenSubr >> 8);
            outRom[context.workingOffset++] = (byte)((reallyBrokenSubr >> 16) + 0xC0);

            // PLA
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // the snake uses this one
            /*
            C2/7508:	A90200  	LDA #$0002
            C2/750B:	9D1E00  	STA $001E,X
            C2/750E:	9D0B00  	STA $000B,X **
            C2/7511:	9E6600  	STZ $0066,X **
             */

            outRom[0x2750e] = 0x22;
            outRom[0x2750f] = (byte)(context.workingOffset);
            outRom[0x27510] = (byte)(context.workingOffset >> 8);
            outRom[0x27511] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x27512] = 0xEA;
            outRom[0x27513] = 0xEA;

            // removed code
            // STA $000B,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0x00;
            // STZ $0066,x
            outRom[context.workingOffset++] = 0x9E;
            outRom[context.workingOffset++] = 0x66;
            outRom[context.workingOffset++] = 0x00;

            // same shit as above for the other non-snake Z-setter
            // PHA
            outRom[context.workingOffset++] = 0x48;

            // fix broken bosses by setting Z to 0
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(reallyBrokenSubr);
            outRom[context.workingOffset++] = (byte)(reallyBrokenSubr >> 8);
            outRom[context.workingOffset++] = (byte)((reallyBrokenSubr >> 16) + 0xC0);

            // PLA
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
