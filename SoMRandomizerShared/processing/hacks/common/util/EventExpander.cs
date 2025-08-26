using SoMRandomizer.logging;
using SoMRandomizer.util;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.util
{
    /// <summary>
    /// Hack that makes a new table of 24-bit event offsets so events can be moved to an expanded area of the ROM.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EventExpander
    {
        public void process(byte[] rom, Dictionary<int, List<byte>> replacementEvents, ref int newCodeOffset)
        {
            // overwrite the ones we specify with pointers to new block, and dump those there
            // modify code to read offsets from 24 bit new block instead of C9xxxx CAxxxx
            // this should be minimal impact to existing stuff
            // write 24bit offsets to new area for all existing events
            
            // bank selection stuff:
            // $01/E77E C9 00 08    CMP #$0800              A:0100 X:4406 Y:0016 P:envmxdIZc
            // $01/E781 B0 46       BCS $46    [$E7C9]      A:0100 X:4406 Y:0016 P:eNvmxdIzc
            // ^ door stuff
            // v non-door stuff
            // $01/E783 85 00       STA $00    [$00:0000]   A:0100 X:4406 Y:0016 P:eNvmxdIzc
            // $01/E785 A5 D0       LDA $D0    [$00:00D0]   A:0100 X:4406 Y:0016 P:eNvmxdIzc
            // $01/E787 29 FF 00    AND #$00FF              A:0000 X:4406 Y:0016 P:envmxdIZc
            // $01/E78A F0 08       BEQ $08    [$E794]      A:0000 X:4406 Y:0016 P:envmxdIZc

            // **************
            // $01/E794 A5 00       LDA $00    [$00:0000]   A:0000 X:4406 Y:0016 P:envmxdIZc
            // vvv
            // $01/E796 C9 00 04    CMP #$0400              A:0100 X:4406 Y:0016 P:envmxdIzc
            // $01/E799 08          PHP                     A:0100 X:4406 Y:0016 P:eNvmxdIzc
            // $01/E79A 29 FF 03    AND #$03FF              A:0100 X:4406 Y:0016 P:eNvmxdIzc
            // ^^^ *** remove
            // $01/E79D 0A          ASL A                   A:0100 X:4406 Y:0016 P:envmxdIzc
            // ^^^ *** need to multiply by 3 instead of 2, because we're using 24 bit offsets now
            //         just need CLC ADC $00 i think
            // $01/E79E AA          TAX                     A:0200 X:4406 Y:0016 P:envmxdIzc
            // vvv
            // $01/E79F 28          PLP                     A:0200 X:0200 Y:0016 P:envmxdIzc
            // ^^^ *** remove

            // from e796 CMP
            // vvv
            // $01/E7A0 B0 0C       BCS $0C    [$E7AE]      A:0200 X:0200 Y:0016 P:eNvmxdIzc
            // ^^^ *** replace with unconditional BRA (0x80)

            // $01/E7A2 BF 00 00 C9 LDA $C90000,x[$C9:0200] A:0200 X:0200 Y:0016 P:eNvmxdIzc ***
            // $01/E7A6 85 D1       STA $D1    [$00:00D1]   A:256D X:0200 Y:0016 P:envmxdIzc
            // $01/E7A8 E2 20       SEP #$20                A:256D X:0200 Y:0016 P:envmxdIzc
            // $01/E7AA A9 C9       LDA #$C9                A:256D X:0200 Y:0016 P:envMxdIzc ***
            // $01/E7AC 80 0A       BRA $0A    [$E7B8]      A:25C9 X:0200 Y:0016 P:eNvMxdIzc

            // ^^^ *** this becomes dead code

            // vvv *** change this to wherever we stick the new offsets -- need more than that, to handle > 0x10000 bytes
            // load first byte and stick it in bank reg (DB i think), then load 16 bit?
            // then restore bank
            // actually for offsets we don't need that, but that fixed LDA #C9 #CA business needs to be changed

            // $01/E7AE BF 00 00 CA LDA $CA0000,x[$CA:0000] A:0000 X:0000 Y:0072 P:envmxdIZC ***
            // $01/E7B2 85 D1       STA $D1    [$00:00D1]   A:0C02 X:0000 Y:0072 P:envmxdIzC
            // $01/E7B4 E2 20       SEP #$20                A:0C02 X:0000 Y:0072 P:envmxdIzC
            // vvv
            // $01/E7B6 A9 CA       LDA #$CA                A:0C02 X:0000 Y:0072 P:envMxdIzC ***
            // ^^^ *** this has to come from the same spot instead of being hardcoded

            // $01/E7B8 85 D3       STA $D3    [$00:00D3]   A:25C9 X:0200 Y:0016 P:eNvMxdIzc

            // 1E783 -> 1E7C8 = relevant code to replace here
            // whole thing:
            // $01/E783 85 00       STA $00    [$00:0000]   A:0100 X:4406 Y:0016 P:eNvmxdIzc
            // $01/E785 A5 D0       LDA $D0    [$00:00D0]   A:0100 X:4406 Y:0016 P:eNvmxdIzc
            // $01/E787 29 FF 00    AND #$00FF              A:0000 X:4406 Y:0016 P:envmxdIZc
            // $01/E78A F0 08       BEQ $08    [$E794]      A:0000 X:4406 Y:0016 P:envmxdIZc
            // $01/E78C C9 FF 00    CMP #$00FF              A:00FF X:004E Y:0000 P:envmxdIzc
            // $01/E78F F0 03       BEQ $03    [$E794]      A:00FF X:004E Y:0000 P:envmxdIZC
            // $01/E791 82 ED 00    BRL $00ED  [$E881]      A:0001 X:4406 Y:0016 P:eNvmxdIzc --
            // replace starting here?
            // vv
            // $01/E794 A5 00       LDA $00    [$00:0000]   A:0000 X:4406 Y:0016 P:envmxdIZc
            //    - $01/E796 C9 00 04    CMP #$0400              A:0100 X:4406 Y:0016 P:envmxdIzc
            //    - $01/E799 08          PHP                     A:0100 X:4406 Y:0016 P:eNvmxdIzc
            //    - $01/E79A 29 FF 03    AND #$03FF              A:0100 X:4406 Y:0016 P:eNvmxdIzc
            // $01/E79D 0A          ASL A                   A:0100 X:4406 Y:0016 P:envmxdIzc
            // + 18 CLC
            // + 65 00 ADC $00
            // $01/E79E AA          TAX                     A:0200 X:4406 Y:0016 P:envmxdIzc
            //    - $01/E79F 28          PLP                     A:0200 X:0200 Y:0016 P:envmxdIzc
            //    - $01/E7A0 B0 0C       BCS $0C    [$E7AE]      A:0200 X:0200 Y:0016 P:eNvmxdIzc
            //    - $01/E7A2 BF 00 00 C9 LDA $C90000,x[$C9:0200] A:0200 X:0200 Y:0016 P:eNvmxdIzc
            //    - $01/E7A6 85 D1       STA $D1    [$00:00D1]   A:256D X:0200 Y:0016 P:envmxdIzc
            //    - $01/E7A8 E2 20       SEP #$20                A:256D X:0200 Y:0016 P:envmxdIzc
            //    - $01/E7AA A9 C9       LDA #$C9                A:256D X:0200 Y:0016 P:envMxdIzc
            //    - $01/E7AC 80 0A       BRA $0A    [$E7B8]      A:25C9 X:0200 Y:0016 P:eNvMxdIzc
            //    - $01/E7AE BF 00 00 CA LDA $CA0000,x[$CA:0000] A:0000 X:0000 Y:0072 P:envmxdIZC
            //    - $01/E7B2 85 D1       STA $D1    [$00:00D1]   A:0C02 X:0000 Y:0072 P:envmxdIzC
            //    - $01/E7B4 E2 20       SEP #$20                A:0C02 X:0000 Y:0072 P:envmxdIzC
            //    - $01/E7B6 A9 CA       LDA #$CA                A:0C02 X:0000 Y:0072 P:envMxdIzC
            //    - $01/E7B8 85 D3       STA $D3    [$00:00D3]   A:25C9 X:0200 Y:0016 P:eNvMxdIzc
            // + BF xx xx xx   LDA $offsetsNewStart,x -- load 16bit offset
            // + 85 D1         STA $D1
            // + E2 20         SEP #20
            // + BF xx xx xx   LDA $(offsetsNewStart+2),x -- load bank
            // + 85 D3         STA $D3
            // ^^
            // $01/E7BA A5 D0       LDA $D0    [$00:00D0]   A:28C9 X:020C Y:00FE P:eNvMxdIzc
            // $01/E7BC C9 01       CMP #$01                A:2800 X:020C Y:00FE P:envMxdIZc
            // $01/E7BE A9 01       LDA #$01                A:2800 X:020C Y:00FE P:eNvMxdIzc
            // $01/E7C0 85 D0       STA $D0    [$00:00D0]   A:2801 X:020C Y:00FE P:envMxdIzc
            // $01/E7C2 90 02       BCC $02    [$E7C6]      A:2801 X:020C Y:00FE P:envMxdIzc
            // $01/E7C4 FA          PLX                     A:9401 X:067A Y:0000 P:envMxdIzC
            // $01/E7C5 60          RTS                     A:9401 X:004E Y:0000 P:envMxdIzC
            // $01/E7C6 82 C4 00    BRL $00C4  [$E88D]      A:2801 X:020C Y:00FE P:envMxdIzc --
            //
            // don't even need a new block for this, it can fit in the old one

            // no event changes?  don't fuck with anything
            if (replacementEvents.Count > 0)
            {
                List<int> newOffsets = new List<int>();
                // first grab all the old offsets
                // 0x400 in 0x90000
                // 0x601 in 0xA0000
                for(int i=0; i < 0x400; i++)
                {
                    byte o1 = rom[0x90000 + i * 2];
                    byte o2 = rom[0x90000 + i * 2 + 1];
                    int fullOffset = 0x90000 + o1 + (o2 << 8);
                    newOffsets.Add(fullOffset);
                }
                for (int i = 0; i < 0x601; i++)
                {
                    byte o1 = rom[0xA0000 + i * 2];
                    byte o2 = rom[0xA0000 + i * 2 + 1];
                    int fullOffset = 0xA0000 + o1 + (o2 << 8);
                    newOffsets.Add(fullOffset);
                }

                foreach (int eventId in replacementEvents.Keys)
                {
                    List<byte> eventData = replacementEvents[eventId];
                    CodeGenerationUtils.ensureSpaceInBank(ref newCodeOffset, eventData.Count);
                    newOffsets[eventId] = newCodeOffset;
                    Logging.log("Putting event " + eventId.ToString("X4") + " at " + newCodeOffset.ToString("X6"), "debug");
                    foreach(byte b in eventData)
                    {
                        rom[newCodeOffset++] = b;
                    }
                }

                int offsetsNewStart = newCodeOffset + 0xC00000;
                int offsetsNewStart2 = offsetsNewStart + 2;
                for (int i=0; i < 0x400 + 0x601; i++)
                {
                    rom[newCodeOffset++] = (byte)newOffsets[i];
                    rom[newCodeOffset++] = (byte)(newOffsets[i] >> 8);
                    rom[newCodeOffset++] = (byte)(0xC0 + (newOffsets[i] >> 16));
                }

                // modify code as specified above to look at offsetsNewStart
                rom[0x1E796] = 0xEA;
                rom[0x1E797] = 0xEA;
                rom[0x1E798] = 0xEA;
                rom[0x1E799] = 0xEA;
                rom[0x1E79A] = 0xEA;
                rom[0x1E79B] = 0xEA;
                rom[0x1E79C] = 0xEA;

                rom[0x1E79E] = 0x18;

                rom[0x1E79F] = 0x65;
                rom[0x1E7A0] = 0x00;

                rom[0x1E7A1] = 0xAA;

                rom[0x1E7A2] = 0xBF;
                rom[0x1E7A3] = (byte)offsetsNewStart;
                rom[0x1E7A4] = (byte)(offsetsNewStart >> 8);
                rom[0x1E7A5] = (byte)(offsetsNewStart >> 16);

                rom[0x1E7A6] = 0x85;
                rom[0x1E7A7] = 0xD1;

                rom[0x1E7A8] = 0xE2;
                rom[0x1E7A9] = 0x20;

                rom[0x1E7AA] = 0xBF;
                rom[0x1E7AB] = (byte)offsetsNewStart2;
                rom[0x1E7AC] = (byte)(offsetsNewStart2 >> 8);
                rom[0x1E7AD] = (byte)(offsetsNewStart2 >> 16);

                rom[0x1E7AE] = 0x85;
                rom[0x1E7AF] = 0xD3;

                rom[0x1E7B0] = 0xEA;
                rom[0x1E7B1] = 0xEA;
                rom[0x1E7B2] = 0xEA;
                rom[0x1E7B3] = 0xEA;
                rom[0x1E7B4] = 0xEA;
                rom[0x1E7B5] = 0xEA;
                rom[0x1E7B6] = 0xEA;
                rom[0x1E7B7] = 0xEA;
                rom[0x1E7B8] = 0xEA;
                rom[0x1E7B9] = 0xEA;
            }
        }
    }
}
