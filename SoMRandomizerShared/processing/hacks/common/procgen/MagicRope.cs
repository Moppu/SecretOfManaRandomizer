using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// Change magic rope to warp you to the beginning of the floor if you get stuck. For procgen modes only.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MagicRope : RandoProcessor
    {
        protected override string getName()
        {
            return "Magic rope as un-stuck for non-vanilla modes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // replace a small amount of code associated with using consumables, to check for magic rope
            // C1 / DC48: AD F3 19   LDA $19F3
            // C1 / DC4B: 29 03 0A   AND #$0A03
            // change it to execute door 0xFFFF, which the procgen DoorExpansion hack defines as procgen floor entry door

            outRom[0x1DC48] = 0x22;
            outRom[0x1DC49] = (byte)(context.workingOffset);
            outRom[0x1DC4A] = (byte)(context.workingOffset >> 8);
            outRom[0x1DC4B] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1DC4C] = 0xEA;
            outRom[0x1DC4D] = 0xEA;
            // (8bit a, 16bit x/y)

            // CMP #06 - magic rope id
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x06;
            // BEQ 07
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x07;
            // (replaced code) LDA $19F3
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0xF3;
            outRom[context.workingOffset++] = 0x19;
            // (replaced code) AND #$0A03
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x0A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            ////////////////////////
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // LDA #FFFF - continue door ID
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0xFF;
            // JSL C1E76D - run a door
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x6D;
            outRom[context.workingOffset++] = 0xE7;
            outRom[context.workingOffset++] = 0xC1;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // name of magic rope
            List<byte> nameBytes = VanillaEventUtil.getBytes("Stuck?");
            for (int i = 0xaa12e; i <= 0xaa137; i++)
            {
                outRom[i] = 0;
            }
            int off = 0xAA12E;
            foreach (byte b in nameBytes)
            {
                outRom[off++] = b;
            }
            return true;
        }
    }
}
