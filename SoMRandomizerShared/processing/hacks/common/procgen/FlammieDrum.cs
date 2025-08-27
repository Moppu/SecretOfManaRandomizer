using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.music;
using SoMRandomizer.processing.hacks.common.util;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// Make flammie drum play a random song instead of summoning flammie.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FlammieDrum
    {
        public void process(byte[] outRom, ref int newCodeOffset, SongExpansionInfo songInfo, NamesOfThings namesOfThings)
        {
            // three bytes per song
            // [#1] (song indicator) -> 7E1E00
            // [00] (song id) -> 7E1E01
            // [01] [02] DSP/echo params -> 7E1E02, 7E1E03
            // leave out non-looping tracks: 17, 18, 19, 1A, 1E, 21, 22, 23, 28, 2D, 2E, 2F, 30, 33, 35
            // the DSP params are derived from the vanilla 7xx events that play these songs.
            byte[] vanillaSongs = new byte[] // 3 bytes each
            {
            0x00, 0x1A, 0x8F,
            0x01, 0x09, 0xFF,
            0x02, 0x0D, 0xFF,
            0x03, 0x0B, 0xFF,
            0x04, 0x04, 0xFF,
            0x05, 0x10, 0x8F,
            0x06, 0x13, 0x8F,
            0x07, 0x18, 0x8F,
            0x08, 0x1B, 0x8F,
            0x09, 0x15, 0x8F,
            0x0A, 0x13, 0x8F,
            0x0B, 0x17, 0x8F,
            0x0C, 0x06, 0xFF,
            0x0D, 0x12, 0x8F,
            0x0E, 0x1B, 0x8F,
            0x0F, 0x18, 0x8F,
            0x10, 0x1B, 0x8F,
            0x11, 0x12, 0x8F,
            0x12, 0x42, 0x4F,
            0x13, 0x1A, 0x8F,
            0x14, 0x10, 0x8F,
            0x15, 0x12, 0x8F,
            0x16, 0x1A, 0x8F,
            0x1B, 0x13, 0x8F,
            0x1C, 0x1A, 0x8F,
            0x1D, 0x1B, 0x8F,
            0x1F, 0x1A, 0x8F,
            0x20, 0x1A, 0x8F,
            0x24, 0x1C, 0x8F,
            0x25, 0x14, 0x8F,
            0x26, 0x15, 0x8F,
            0x27, 0x1C, 0x8F,
            0x29, 0x1C, 0x8F,
            0x2A, 0x18, 0x8F,
            0x2B, 0x0C, 0x8F,
            0x2C, 0x4F, 0x8F,
            0x31, 0x16, 0x8F,
            0x34, 0x1B, 0x8F,
            0x36, 0x16, 0x8F,
            0x37, 0x12, 0x8F,
            0x38, 0x1F, 0x8F,
            0x39, 0x14, 0xFF,
            0x3A, 0x1B, 0x8F,
            };

            List<byte> allSongs = new List<byte>();
            foreach(byte b in vanillaSongs)
            {
                allSongs.Add(b);
            }
            if (songInfo != null)
            {
                // add custom music to the list, with default DSP values
                for (int i = 64; i < songInfo.totalSongs; i++)
                {
                    allSongs.Add((byte)i);
                    allSongs.Add(0x13);
                    allSongs.Add(0x8F);
                }
            }

            int songsLoc = newCodeOffset + 0xC00000;
            int songsLocP1 = songsLoc + 1;
            foreach (byte b in allSongs)
            {
                outRom[newCodeOffset++] = b;
            }

            // remove vanilla block for flammie drum usage:
            /*
                C1/DBCD:   ADF219     LDA $19F2
                C1/DBD0:   2903       AND #$03
                C1/DBD2:   0A         ASL A
                C1/DBD3:   EB         XBA 
                C1/DBD4:   A900       LDA #$00
                C1/DBD6:   AA         TAX 
                C1/DBD7:   8614       STX $14
                C1/DBD9:   206CCA     JSR $CA6C
                C1/DBDC:   A97F       LDA #$7F
                C1/DBDE:   9D4FE0     STA $E04F,X
                C1/DBE1:   9E04E1     STZ $E104,X
                C1/DBE4:   A905       LDA #$05
                C1/DBE6:   9D2FE0     STA $E02F,X
                C1/DBE9:   9E0CE0     STZ $E00C,X
                C1/DBEC:   9E84E0     STZ $E084,X
                C1/DBEF:   9C06E0     STZ $E006
                C1/DBF2:   9C07E0     STZ $E007
                C1/DBF5:   9C06E2     STZ $E206
                C1/DBF8:   9C07E2     STZ $E207
                C1/DBFB:   9C06E4     STZ $E406
                C1/DBFE:   9C07E4     STZ $E407
                C1/DC01:   A5F4       LDA $F4
                C1/DC03:   291F       AND #$1F
                C1/DC05:   0920       ORA #$20
                C1/DC07:   85F4       STA $F4
                C1/DC09:   20D885     JSR $85D8
                C1/DC0C:   C230       REP #$30
                C1/DC0E:   A94000     LDA #$0040
                C1/DC11:   8508       STA $08
                C1/DC13:   A90000     LDA #$0000
                C1/DC16:   8500       STA $00
                C1/DC18:   A05028     LDY #$2850
                C1/DC1B:   A9D400     LDA #$00D4
                C1/DC1E:   2299C501   JSR $01C599
                ^^ remove above
                C1/DC22:   C220       REP #$20         
                vv remove below, and replace with musicPlayCodeOffset
                C1/DC24:   A5DC       LDA $DC          
                C1/DC26:   29FF00     AND #$00FF       
                C1/DC29:   09000C     ORA #$0C00       
                C1/DC2C:   226DE701   JSR $01E76D
                C1/DC30:   60         RTS
             */
            int musicPlayCodeOffset = newCodeOffset + 0xC00000;
            for(int i=0x1DBCD; i <= 0x1DC21; i++)
            {
                outRom[i] = 0xEA;
            }

            outRom[0x1DC24] = 0x22;
            outRom[0x1DC25] = (byte)musicPlayCodeOffset;
            outRom[0x1DC26] = (byte)(musicPlayCodeOffset>>8);
            outRom[0x1DC27] = (byte)(musicPlayCodeOffset>>16);

            outRom[0x1DC28] = 0xEA;
            outRom[0x1DC29] = 0xEA;
            outRom[0x1DC2A] = 0xEA;
            outRom[0x1DC2B] = 0xEA;
            outRom[0x1DC2C] = 0xEA;
            outRom[0x1DC2D] = 0xEA;
            outRom[0x1DC2E] = 0xEA;
            outRom[0x1DC2F] = 0xEA;

            // (16bit x and y coming in)
            // SEP 20   ;8bit
            outRom[newCodeOffset++] = 0xE2;
            outRom[newCodeOffset++] = 0x20;

            // LDA #01
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x01;

            // STA 7E1E00
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0x1E;
            outRom[newCodeOffset++] = 0x7E;

            // STA 7ECF28 ; event flag
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = EventFlags.PROCGEN_MODE_CUSTOM_MUSIC_FLAG;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;

            // call random number gen
            // 22 9C 38 C0    JSL C0389C 
            // phd
            outRom[newCodeOffset++] = 0x0B;
            // rep 20
            outRom[newCodeOffset++] = 0xC2;
            outRom[newCodeOffset++] = 0x20;
            // LDA #0300
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0x03;
            // PHA
            outRom[newCodeOffset++] = 0x48;
            // PLD
            outRom[newCodeOffset++] = 0x2B;
            // sep 20
            outRom[newCodeOffset++] = 0xE2;
            outRom[newCodeOffset++] = 0x20;
            // JSL to RNG subroutine
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = 0x9C;
            outRom[newCodeOffset++] = 0x38;
            outRom[newCodeOffset++] = 0xC0;
            // PLD
            outRom[newCodeOffset++] = 0x2B;

            // REP 20
            outRom[newCodeOffset++] = 0xC2;
            outRom[newCodeOffset++] = 0x20;

            // AND #00FF
            outRom[newCodeOffset++] = 0x29;
            outRom[newCodeOffset++] = 0xFF;
            outRom[newCodeOffset++] = 0x00;

            // STA $004204
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x04;
            outRom[newCodeOffset++] = 0x42;
            outRom[newCodeOffset++] = 0x00;

            // SEP 20
            outRom[newCodeOffset++] = 0xE2;
            outRom[newCodeOffset++] = 0x20;

            // LDA [songs.length / 3]
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = (byte)(allSongs.Count / 3);

            // STA $004206
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x06;
            outRom[newCodeOffset++] = 0x42;
            outRom[newCodeOffset++] = 0x00;

            // NOP 8 times
            outRom[newCodeOffset++] = 0xEA;
            outRom[newCodeOffset++] = 0xEA;
            outRom[newCodeOffset++] = 0xEA;
            outRom[newCodeOffset++] = 0xEA;
            outRom[newCodeOffset++] = 0xEA;
            outRom[newCodeOffset++] = 0xEA;
            outRom[newCodeOffset++] = 0xEA;
            outRom[newCodeOffset++] = 0xEA;

            // LDA $004216
            outRom[newCodeOffset++] = 0xAF;
            outRom[newCodeOffset++] = 0x16;
            outRom[newCodeOffset++] = 0x42;
            outRom[newCodeOffset++] = 0x00;

            // ASL
            outRom[newCodeOffset++] = 0x0A;

            // ADC $004216 ; to mul by 3 and index into songs table
            outRom[newCodeOffset++] = 0x6F;
            outRom[newCodeOffset++] = 0x16;
            outRom[newCodeOffset++] = 0x42;
            outRom[newCodeOffset++] = 0x00;

            // REP 20 ;16 bit
            outRom[newCodeOffset++] = 0xC2;
            outRom[newCodeOffset++] = 0x20;

            // PHX
            outRom[newCodeOffset++] = 0xDA;

            // TAX
            outRom[newCodeOffset++] = 0xAA;

            // LDA [songs array location], x
            outRom[newCodeOffset++] = 0xBF;
            outRom[newCodeOffset++] = (byte)songsLoc;
            outRom[newCodeOffset++] = (byte)(songsLoc>>8);
            outRom[newCodeOffset++] = (byte)(songsLoc>>16);

            // STA 7E1E01
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x01;
            outRom[newCodeOffset++] = 0x1E;
            outRom[newCodeOffset++] = 0x7E;

            // LDA [songs array location + 1], x
            outRom[newCodeOffset++] = 0xBF;
            outRom[newCodeOffset++] = (byte)songsLocP1;
            outRom[newCodeOffset++] = (byte)(songsLocP1 >> 8);
            outRom[newCodeOffset++] = (byte)(songsLocP1 >> 16);

            // STA 7E1E02
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x02;
            outRom[newCodeOffset++] = 0x1E;
            outRom[newCodeOffset++] = 0x7E;

            // PLX
            outRom[newCodeOffset++] = 0xFA;

            // SEP 20 ; for music call
            outRom[newCodeOffset++] = 0xE2;
            outRom[newCodeOffset++] = 0x20;

            // 22 04 00 C3 JSL $C30004 ; play song subr
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = 0x04;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xC3;

            // rep 20
            outRom[newCodeOffset++] = 0xC2;
            outRom[newCodeOffset++] = 0x20;

            // 6B          RTL
            outRom[newCodeOffset++] = 0x6B;
            
            namesOfThings.setName(NamesOfThings.INDEX_CONSUMABLES_START + 7, "Random Music");
        }
    }
}
