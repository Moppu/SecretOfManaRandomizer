using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.music
{
    /// <summary>
    /// pick a random song any time a song for a given category is played.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class RandomMusicHack
    {
        public void process(byte[] outRom, ref int newCodeOffset, Dictionary<string, List<List<byte>>> songReplacements, Dictionary<byte, string> originalSongTypes)
        {
            // for each song type, make a subroutine that picks a random id of a song within that type.
            Dictionary<byte, int> songSubrs = new Dictionary<byte, int>();
            foreach (byte origSong in originalSongTypes.Keys)
            {
                string songType = originalSongTypes[origSong];
                List<List<byte>> possibleNewSongs = songReplacements[songType];
                int subrSize = 44 + possibleNewSongs.Count * 20;
                CodeGenerationUtils.ensureSpaceInBank(ref newCodeOffset, subrSize);

                songSubrs[origSong] = newCodeOffset;
                if (possibleNewSongs.Count > 0)
                {
                    // inside the xx xx xx subroutines:
                    // 
                    // phd
                    outRom[newCodeOffset++] = 0x0B;
                    // rep 20
                    outRom[newCodeOffset++] = 0xC2;
                    outRom[newCodeOffset++] = 0x20;
                    // LDA #0300 - set DP for rng function call
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
                    // JSL rng
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

                    // LDA [songs.length]
                    outRom[newCodeOffset++] = 0xA9;
                    outRom[newCodeOffset++] = (byte)(possibleNewSongs.Count);

                    // STA $004206
                    outRom[newCodeOffset++] = 0x8F;
                    outRom[newCodeOffset++] = 0x06;
                    outRom[newCodeOffset++] = 0x42;
                    outRom[newCodeOffset++] = 0x00;

                    // NOP 8 times to wait on the divide
                    outRom[newCodeOffset++] = 0xEA;
                    outRom[newCodeOffset++] = 0xEA;
                    outRom[newCodeOffset++] = 0xEA;
                    outRom[newCodeOffset++] = 0xEA;
                    outRom[newCodeOffset++] = 0xEA;
                    outRom[newCodeOffset++] = 0xEA;
                    outRom[newCodeOffset++] = 0xEA;
                    outRom[newCodeOffset++] = 0xEA;

                    // LDA $004216 - load the result, an index into the table of songs
                    outRom[newCodeOffset++] = 0xAF;
                    outRom[newCodeOffset++] = 0x16;
                    outRom[newCodeOffset++] = 0x42;
                    outRom[newCodeOffset++] = 0x00;

                    // map the index to a song id
                    for (int i = 0; i < possibleNewSongs.Count; i++)
                    {
                        // CMP #i
                        outRom[newCodeOffset++] = 0xC9;
                        outRom[newCodeOffset++] = (byte)i;
                        // BNE over
                        outRom[newCodeOffset++] = 0xD0;
                        outRom[newCodeOffset++] = 0x10;
                        // LDA #possibleNewSongs[i][0]
                        outRom[newCodeOffset++] = 0xA9;
                        outRom[newCodeOffset++] = possibleNewSongs[i][0];
                        // STA $1E01
                        outRom[newCodeOffset++] = 0x8D;
                        outRom[newCodeOffset++] = 0x01;
                        outRom[newCodeOffset++] = 0x1E;
                        // LDA #possibleNewSongs[i][1]
                        outRom[newCodeOffset++] = 0xA9;
                        outRom[newCodeOffset++] = possibleNewSongs[i][1];
                        // STA $1E02
                        outRom[newCodeOffset++] = 0x8D;
                        outRom[newCodeOffset++] = 0x02;
                        outRom[newCodeOffset++] = 0x1E;
                        // LDA #possibleNewSongs[i][2]
                        outRom[newCodeOffset++] = 0xA9;
                        outRom[newCodeOffset++] = possibleNewSongs[i][2];
                        // STA $1E03
                        outRom[newCodeOffset++] = 0x8D;
                        outRom[newCodeOffset++] = 0x03;
                        outRom[newCodeOffset++] = 0x1E;
                        // RTL
                        outRom[newCodeOffset++] = 0x6B;
                        // over:
                    }
                }
                // don't forget default rtl and shit down here
            }

            // A5 02 29 F0 is what used to be here.
            outRom[0x301AD] = 0x22;
            outRom[0x301AE] = (byte)(newCodeOffset);
            outRom[0x301AF] = (byte)(newCodeOffset >> 8);
            outRom[0x301B0] = (byte)((newCodeOffset >> 16) + 0xC0);

            // REP 20
            outRom[newCodeOffset++] = 0xC2;
            outRom[newCodeOffset++] = 0x20;

            // PHA
            outRom[newCodeOffset++] = 0x48;

            // SEP 20
            outRom[newCodeOffset++] = 0xE2;
            outRom[newCodeOffset++] = 0x20;

            // LDA $1E01
            outRom[newCodeOffset++] = 0xAD;
            outRom[newCodeOffset++] = 0x01;
            outRom[newCodeOffset++] = 0x1E;

            // CMP MUSICRANDO_LASTSONG_BYTE
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = (byte)(CustomRamOffsets.MUSICRANDO_LASTSONG_BYTE);
            outRom[newCodeOffset++] = (byte)(CustomRamOffsets.MUSICRANDO_LASTSONG_BYTE>>8);
            outRom[newCodeOffset++] = (byte)(CustomRamOffsets.MUSICRANDO_LASTSONG_BYTE>>16);

            // BNE over
            outRom[newCodeOffset++] = 0xD0;
            outRom[newCodeOffset++] = 0x17;
            // orig code plus restore (REP 20)
            outRom[newCodeOffset++] = 0xC2;
            outRom[newCodeOffset++] = 0x20;

            // PLA
            outRom[newCodeOffset++] = 0x68;

            // SEP 20
            outRom[newCodeOffset++] = 0xE2;
            outRom[newCodeOffset++] = 0x20;

            // LDA $02
            outRom[newCodeOffset++] = 0xA5;
            outRom[newCodeOffset++] = 0x02;

            // AND #F0
            outRom[newCodeOffset++] = 0x29;
            outRom[newCodeOffset++] = 0xF0;

            // $7E/D2E2 20 0D D2 
            // skip doing our RTL by sucking the three bytes out of the stack
            outRom[newCodeOffset++] = 0x68;
            outRom[newCodeOffset++] = 0x68;
            outRom[newCodeOffset++] = 0x68;

            // here => rtl if it skips loading the song
            // REP 20
            outRom[newCodeOffset++] = 0xC2;
            outRom[newCodeOffset++] = 0x20;

            // REP 10
            outRom[newCodeOffset++] = 0xC2;
            outRom[newCodeOffset++] = 0x10;

            // PLY
            outRom[newCodeOffset++] = 0x7A;
            // PLX
            outRom[newCodeOffset++] = 0xFA;
            // PLA
            outRom[newCodeOffset++] = 0x68;
            // PLP
            outRom[newCodeOffset++] = 0x28;
            // PLD
            outRom[newCodeOffset++] = 0x2B;
            // PLB
            outRom[newCodeOffset++] = 0xAB;

            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // over: (continue)

            // STA MUSICRANDO_LASTSONG_BYTE
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = (byte)(CustomRamOffsets.MUSICRANDO_LASTSONG_BYTE);
            outRom[newCodeOffset++] = (byte)(CustomRamOffsets.MUSICRANDO_LASTSONG_BYTE >> 8);
            outRom[newCodeOffset++] = (byte)(CustomRamOffsets.MUSICRANDO_LASTSONG_BYTE >> 16);

            foreach (byte b in originalSongTypes.Keys)
            {
                if (songSubrs.ContainsKey(b))
                {
                    // nextIter:
                    // CMP b
                    outRom[newCodeOffset++] = 0xC9;
                    outRom[newCodeOffset++] = b;
                    // BNE nextIter
                    outRom[newCodeOffset++] = 0xD0;
                    outRom[newCodeOffset++] = 0x0E;
                    // JSL subr - pick a random song from the category
                    int subr = songSubrs[b];
                    outRom[newCodeOffset++] = 0x22;
                    outRom[newCodeOffset++] = (byte)(subr);
                    outRom[newCodeOffset++] = (byte)(subr >> 8);
                    outRom[newCodeOffset++] = (byte)((subr >> 16) + 0xC0);
                    // REP 20
                    outRom[newCodeOffset++] = 0xC2;
                    outRom[newCodeOffset++] = 0x20;
                    // PLA
                    outRom[newCodeOffset++] = 0x68;
                    // SEP 20
                    outRom[newCodeOffset++] = 0xE2;
                    outRom[newCodeOffset++] = 0x20;

                    // replaced code here
                    // LDA $02
                    outRom[newCodeOffset++] = 0xA5;
                    outRom[newCodeOffset++] = 0x02;
                    // AND #F0
                    outRom[newCodeOffset++] = 0x29;
                    outRom[newCodeOffset++] = 0xF0;
                    // RTL
                    outRom[newCodeOffset++] = 0x6B;
                }
            }

            // REP 20
            outRom[newCodeOffset++] = 0xC2;
            outRom[newCodeOffset++] = 0x20;
            // PLA
            outRom[newCodeOffset++] = 0x68;
            // SEP 20
            outRom[newCodeOffset++] = 0xE2;
            outRom[newCodeOffset++] = 0x20;

            // replaced code here
            // LDA $02
            outRom[newCodeOffset++] = 0xA5;
            outRom[newCodeOffset++] = 0x02;
            // AND #F0
            outRom[newCodeOffset++] = 0x29;
            outRom[newCodeOffset++] = 0xF0;
            // RTL
            outRom[newCodeOffset++] = 0x6B;
            
        }
    }
}
