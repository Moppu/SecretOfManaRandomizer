using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Randomize drops with mostly consumables and 1/16 chance of random gear pieces.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class XmasDrops
    {
        public void process(byte[] outRom, ref int newCodeOffset)
        {
            // increase drop chance
            outRom[0x428E] = 0x0F;

            // 40-45, 4A = consumables
            // 01-14 = hats
            // 16-29 = armors
            // 2b-3d = accessories
            List<byte> allNormalDrops = new List<byte>();
            List<byte> allGearDrops = new List<byte>();
            for (int i = 0; i < 20; i++)
            {
                for (byte b = 0x40; b <= 0x45; b++)
                {
                    allNormalDrops.Add(b);
                }
                allNormalDrops.Add(0x4A);
            }

            // a few consumables in gear table
            for (byte b = 0x40; b <= 0x45; b++)
            {
                allGearDrops.Add(b);
            }
            allGearDrops.Add(0x4A);
            for (byte b = 0x01; b <= 0x14; b++)
            {
                allGearDrops.Add(b);
            }
            for (byte b = 0x16; b <= 0x29; b++)
            {
                allGearDrops.Add(b);
            }
            for (byte b = 0x2b; b <= 0x3d; b++)
            {
                allGearDrops.Add(b);
            }
            allGearDrops.Remove(22); // remove starter armors
            allGearDrops.Remove(23);
            allGearDrops.Remove(24);
            allGearDrops.Add(0x40); // one extra for 64 total
            int itemTableLocationA = newCodeOffset;
            for (int i = 0; i < 64; i++)
            {
                outRom[newCodeOffset++] = allNormalDrops[i];
            }
            int itemTableLocationB = newCodeOffset;
            for (int i = 0; i < 64; i++)
            {
                outRom[newCodeOffset++] = allGearDrops[i];
            }

            for (int i = 0x8E12C; i < 0x8E17E; i++)
            {
                outRom[i] = 0xEA;
            }

            // this was to fix a weird loading bug that sometimes triggered non-drop events/text when opening a chest
            // $C8/E11E 89 01       BIT #$01                A:0FAA X:00D7 Y:0000 P:envMxdIzC
            // $C8/E120 D0 E7       BNE $E7[$E109]          A:0FAA X:00D7 Y:0000 P:envMxdIZC -> change to 0x80 to branch always
            // $C8/E122 A6 85       LDX $85[$00:0385]       A:0FAA X:00D7 Y:0000 P:envMxdIZC
            outRom[0x8E120] = 0x80;

            // 22 to thingy
            outRom[0x8E12C] = 0x22;
            outRom[0x8E12D] = (byte)(newCodeOffset);
            outRom[0x8E12E] = (byte)(newCodeOffset >> 8);
            outRom[0x8E12F] = (byte)((newCodeOffset >> 16) + 0xC0);
            // JSL to vanilla RNG subroutine C0389C
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = 0x9C;
            outRom[newCodeOffset++] = 0x38;
            outRom[newCodeOffset++] = 0xC0;
            // REP 20
            outRom[newCodeOffset++] = 0xC2;
            outRom[newCodeOffset++] = 0x20;
            // AND #003F
            outRom[newCodeOffset++] = 0x29;
            outRom[newCodeOffset++] = 0x3F;
            outRom[newCodeOffset++] = 0x00;
            // TAX - index of which drop to select
            outRom[newCodeOffset++] = 0xAA;
            // SEP 20
            outRom[newCodeOffset++] = 0xE2;
            outRom[newCodeOffset++] = 0x20;
            // JSL to vanilla RNG subroutine C0389C
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = 0x9C;
            outRom[newCodeOffset++] = 0x38;
            outRom[newCodeOffset++] = 0xC0;
            // AND #0F
            outRom[newCodeOffset++] = 0x29;
            outRom[newCodeOffset++] = 0x0F; // 1/16 chance of gear
            // BEQ - if zero, drop gear from itemTableLocationB
            outRom[newCodeOffset++] = 0xF0;
            outRom[newCodeOffset++] = 0x07;
            // LDA itemTableLocationA,X
            outRom[newCodeOffset++] = 0xBF;
            outRom[newCodeOffset++] = (byte)itemTableLocationA;
            outRom[newCodeOffset++] = (byte)(itemTableLocationA >> 8);
            outRom[newCodeOffset++] = (byte)(0xC0 + (itemTableLocationA >> 16));
            // STA $A4
            outRom[newCodeOffset++] = 0x85;
            outRom[newCodeOffset++] = 0xA4;
            // RTL
            outRom[newCodeOffset++] = 0x6B;
            // LDA itemTableLocationB,X
            outRom[newCodeOffset++] = 0xBF;
            outRom[newCodeOffset++] = (byte)itemTableLocationB;
            outRom[newCodeOffset++] = (byte)(itemTableLocationB >> 8);
            outRom[newCodeOffset++] = (byte)(0xC0 + (itemTableLocationB >> 16));
            // STA $A4
            outRom[newCodeOffset++] = 0x85;
            outRom[newCodeOffset++] = 0xA4;
            // RTL
            outRom[newCodeOffset++] = 0x6B;
        }
    }
}
