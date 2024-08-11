using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.util
{
    /// <summary>
    /// Modify the map piece loader to load from 24-bit offsets, and allow selected map pieces to be replaced with a new one of any size.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MapPieceExpander
    {
        public void process(byte[] rom, Dictionary<int, List<byte>> replacementMaps, ref int newCodeOffset)
        {
            // vanilla map pieces:
            // 0xD0000: id where we start reading from bank 0xE0000 (0xA3)
            // 0xD0002: id where we start reading from bank 0xF0000 (0x206)
            // 0xD0004: id where we start reading from bank 0xC0000 (0x300)
            // 0xD0006: 16-bit offset for map 0 (0xD0804)
            // 0x3FF total 16-bit offsets, except the vanilla loader starts at 0xD0004, so map 0 is just a non-map
            // that uses the last bank threshold as its offset
            if (replacementMaps.Count > 0)
            {
                List<int> newOffsets = new List<int>();
                // existing map piece offsets
                for (int i = 0; i < 0x400; i++)
                {
                    newOffsets.Add(VanillaMapUtil.getMapPieceOffset(rom, i));
                }

                // write replacement pieces and track their offsets
                foreach (int mapPieceId in replacementMaps.Keys)
                {
                    List<byte> mapPieceData = replacementMaps[mapPieceId];
                    CodeGenerationUtils.ensureSpaceInBank(ref newCodeOffset, mapPieceData.Count);
                    newOffsets[mapPieceId] = newCodeOffset;
                    Logging.log("Replacement map piece " + mapPieceId.ToString("X4") + "@ " + newCodeOffset.ToString("X6"), "debug");
                    foreach (byte b in mapPieceData)
                    {
                        rom[newCodeOffset++] = b;
                    }
                }

                // dump list of offsets, 24-bit hirom, out to rom in one contiguous block
                CodeGenerationUtils.ensureSpaceInBank(ref newCodeOffset, newOffsets.Count * 3);
                int offsetsOffset = newCodeOffset + 0xC00000;
                int offsetsOffsetP2 = offsetsOffset + 2;
                for (int i = 0; i < newOffsets.Count; i++)
                {
                    rom[newCodeOffset++] = (byte)newOffsets[i];
                    rom[newCodeOffset++] = (byte)(newOffsets[i] >> 8);
                    rom[newCodeOffset++] = (byte)(0xC0 + (newOffsets[i] >> 16));
                }

                // now custom map loader to grab them from 24-bit locations instead of the weird split bank thing vanilla is doing
                /*
                $00/CC79 A2 CD       LDX #$CD                A:0001 X:0000 Y:0004 P:envmXdIzc **
                $00/CC7B CF 00 00 CD CMP $CD0000[$CD:0000]   A:0001 X:00CD Y:0004 P:eNvmXdIzc **
                $00/CC7F 90 10       BCC $10    [$CC91]      A:0001 X:00CD Y:0004 P:eNvmXdIzc **
                (other bank thresholds checked in here)                                       **
                $00/CC91 86 1F       STX $1F    [$00:001F]   A:0001 X:00CD Y:0004 P:eNvmXdIzc **
                $00/CC93 C2 10       REP #$10                A:0001 X:00CD Y:0004 P:eNvmXdIzc **
                $00/CC95 0A          ASL A                   A:0001 X:00CD Y:0004 P:eNvmxdIzc **
                $00/CC96 AA          TAX                     A:0002 X:00CD Y:0004 P:envmxdIzc **
                $00/CC97 BF 04 00 CD LDA $CD0004,x[$CD:0006] A:0002 X:0002 Y:0004 P:envmxdIzc **
                $00/CC9B 85 1D       STA $1D    [$00:001D]   A:0804 X:0002 Y:0004 P:envmxdIzc ** -> remove
                $00/CC9D E2 30       SEP #$30                A:0804 X:0002 Y:0004 P:envmxdIzc 
                $00/CC9F A0 00       LDY #$00                A:0804 X:0002 Y:0004 P:envMXdIzc 
                $00/CCA1 B7 1D       LDA [$1D],y[$CD:0804]   A:0804 X:0002 Y:0000 P:envMXdIZc

                replace with:
                C2 20       REP #30
                85 1D       STA $1D
                0A          ASL A
                18          CLC
                65 1D       ADC $1D # multiply index by 3
                AA          TAX
                BF xx xx xx LDA $(offsetsOffset + 1),x
                85 1D       STA $1D
                E2 20       SEP #20
                BF xx xx xx LDA $(offsetsOffset),x
                85 1F       STA $1F
                 */
                for (int o = 0xCC79; o <= 0xCC9C; o++)
                {
                    rom[o] = 0xEA;
                }
                int off = 0xCC79;
                rom[off++] = 0xC2;
                rom[off++] = 0x30;

                rom[off++] = 0x85;
                rom[off++] = 0x1D;

                rom[off++] = 0x0A;

                rom[off++] = 0x18;

                rom[off++] = 0x65;
                rom[off++] = 0x1D;

                rom[off++] = 0xAA;

                rom[off++] = 0xBF;
                rom[off++] = (byte)offsetsOffset;
                rom[off++] = (byte)(offsetsOffset >> 8);
                rom[off++] = (byte)(offsetsOffset >> 16);

                rom[off++] = 0x85;
                rom[off++] = 0x1D;

                rom[off++] = 0xE2;
                rom[off++] = 0x20;

                rom[off++] = 0xBF;
                rom[off++] = (byte)offsetsOffsetP2;
                rom[off++] = (byte)(offsetsOffsetP2 >> 8);
                rom[off++] = (byte)(offsetsOffsetP2 >> 16);

                rom[off++] = 0x85;
                rom[off++] = 0x1F;

            }
        }
    }
}
