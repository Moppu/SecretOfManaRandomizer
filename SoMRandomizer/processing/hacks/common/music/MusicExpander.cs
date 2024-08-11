using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.music
{
    /// <summary>
    /// Allow more than the vanilla number of songs and samples to be loaded 
    /// by moving all the tables associated with these items and expanding 
    /// them to account for however many extra we decide we need.
    /// <para />
    /// Extra data is filled with zeros for now; new song processing should fill in
    /// this data as it's loaded.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MusicExpander
    {
        public static SongExpansionInfo expandMusic(byte[] rom, ref int offset, int numNewSongs, int numNewSamples)
        {
            // music offsets normally at 33D39 (33DE4 mana beast) (headerless) - pointed to from:
            //$C3 / 0230 BF 39 3D C3 LDA $C33D39,x[$C3: 3DE4] A: 00AB X: 00AB Y:0010 P: envMxdIzc
            //$C3 / 0234 85 14       STA $14[$00:1E14]   A: 00F8 X: 00AB Y:0010 P: eNvMxdIzc
            //$C3 / 0236 BF 3A 3D C3 LDA $C33D3A,x[$C3: 3DE5] A: 00F8 X: 00AB Y:0010 P: eNvMxdIzc
            //$C3 / 023A 85 15       STA $15[$00:1E15]   A: 00E9 X: 00AB Y:0010 P: eNvMxdIzc
            //$C3 / 023C BF 3B 3D C3 LDA $C33D3B,x[$C3: 3DE6] A: 00E9 X: 00AB Y:0010 P: eNvMxdIzc
            //$C3 / 0240 85 16       STA $16[$00:1E16]   A: 00C5 X:00AB Y:0010 P: eNvMxdIzc
            // so change these offsets, and move the song pointers all to.. say, 380000 in the rom.
            // along with the new samples, and the song data for this song.
            int musicOffsetsOffset = offset;
            // code changes
            rom[0x30231] = (byte)musicOffsetsOffset;
            rom[0x30232] = (byte)(musicOffsetsOffset >> 8);
            rom[0x30233] = (byte)((musicOffsetsOffset >> 16) + 0xC0);
            rom[0x30237] = (byte)(musicOffsetsOffset + 1);
            rom[0x30238] = (byte)((musicOffsetsOffset + 1) >> 8);
            rom[0x30239] = (byte)(((musicOffsetsOffset + 1) >> 16) + 0xC0);
            rom[0x3023D] = (byte)(musicOffsetsOffset + 2);
            rom[0x3023E] = (byte)((musicOffsetsOffset + 2) >> 8);
            rom[0x3023F] = (byte)(((musicOffsetsOffset + 2) >> 16) + 0xC0);
            // copy offset table so it can be added to
            for (int i = 0; i < 64 + numNewSongs; i++)
            {
                // note: 59-63 are blanks
                if (i < 64)
                {
                    rom[offset++] = rom[VanillaRomOffsets.MUSIC_OFFSETS + i * 3];
                    rom[offset++] = rom[VanillaRomOffsets.MUSIC_OFFSETS + i * 3 + 1];
                    rom[offset++] = rom[VanillaRomOffsets.MUSIC_OFFSETS + i * 3 + 2];
                }
                else
                {
                    // this can be filled in later
                    rom[offset++] = 0;
                    rom[offset++] = 0;
                    rom[offset++] = 0;
                }
            }


            // any other data associated with the song that needs to move? oh yeah, the sample table

            // 33D39 -> 33DF8 inclusive - vanilla music offset table - move to expanded section & add to as needed

            // sample tables 33F22, 32 bytes each; 16 bit values for some reason
            // $C3/02B6 BF 22 3F C3 LDA $C33F22,x[$C3:4122] A:1ECA X:0200 Y:537B P:envmxdIzc
            int sampleTableOffset = offset;
            rom[0x302B7] = (byte)sampleTableOffset;
            rom[0x302B8] = (byte)(sampleTableOffset >> 8);
            rom[0x302B9] = (byte)((sampleTableOffset >> 16) + 0xC0);

            for (int i = 0; i < 64 + numNewSongs; i++)
            {
                for (int s = 0; s < 32; s++)
                {
                    if (i < 64)
                    {
                        rom[offset++] = rom[0x33F22 + i * 32 + s];
                    }
                    else
                    {
                        // this can be filled in later
                        rom[offset++] = 0;
                    }
                }
            }

            // sample frequencies 33E5C->33E9D=0x21 16 bit values, loop points 33E9E->33EDF=0x21 16 bit values
            // ^ nope, swap that
            // $C3/050A 7F 5C 3E C3 ADC $C33E5C,x[$C3:3E5C] A:84E4 X:0000 Y:0008 P:enVmxdIzc
            int sampleLoopOffset = offset;
            rom[0x3050B] = (byte)sampleLoopOffset;
            rom[0x3050C] = (byte)(sampleLoopOffset >> 8);
            rom[0x3050D] = (byte)((sampleLoopOffset >> 16) + 0xC0);
            for (int i = 0; i < 33 + numNewSamples; i++)
            {
                if (i < 33)
                {
                    rom[offset++] = rom[VanillaRomOffsets.SAMPLE_LOOPS_OFFSET + i * 2];
                    rom[offset++] = rom[VanillaRomOffsets.SAMPLE_LOOPS_OFFSET + i * 2 + 1];
                }
                else
                {
                    // this can be filled in later
                    rom[offset++] = 0;
                    rom[offset++] = 0;
                }
            }

            // $C3/04F6 BF 9E 3E C3 LDA $C33E9E,x[$C3:3E9E] A:0000 X:0000 Y:0008 P:enVmxdIZc
            int sampleFrequencyOffset = offset;
            rom[0x304F7] = (byte)sampleFrequencyOffset;
            rom[0x304F8] = (byte)(sampleFrequencyOffset >> 8);
            rom[0x304F9] = (byte)((sampleFrequencyOffset >> 16) + 0xC0);
            for (int i = 0; i < 33 + numNewSamples; i++)
            {
                if (i < 33)
                {
                    rom[offset++] = rom[VanillaRomOffsets.SAMPLE_BASEFREQS_OFFSET + i * 2];
                    rom[offset++] = rom[VanillaRomOffsets.SAMPLE_BASEFREQS_OFFSET + i * 2 + 1];
                }
                else
                {
                    // this can be filled in later
                    rom[offset++] = 0;
                    rom[offset++] = 0;
                }
            }

            // adsr data for samples 33EE0->33F21? 0x21 more 16 bit values
            // $C3/0514 BF E0 3E C3 LDA $C33EE0,x[$C3:3EE0] A:8B26 X:0000 Y:0008 P:envmxdIzc
            int sampleAdsrOffset = offset;
            rom[0x30515] = (byte)sampleAdsrOffset;
            rom[0x30516] = (byte)(sampleAdsrOffset >> 8);
            rom[0x30517] = (byte)((sampleAdsrOffset >> 16) + 0xC0);

            for (int i = 0; i < 33 + numNewSamples; i++)
            {
                if (i < 33)
                {
                    rom[offset++] = rom[VanillaRomOffsets.SAMPLE_ADSRS_OFFSET + i * 2];
                    rom[offset++] = rom[VanillaRomOffsets.SAMPLE_ADSRS_OFFSET + i * 2 + 1];
                }
                else
                {
                    // this can be filled in later
                    rom[offset++] = 0;
                    rom[offset++] = 0;
                }
            }

            // samples
            // 33ff9 with header for offsets
            // i think i crammed them down into the same sample space, didn't expand
            // these are loaded at ..
            // $C3 / 0304 BF F9 3D C3 LDA $C33DF9,x[$C3:3DF9] A: 0000 X: 0000 Y: 230D P: envMxdIZc
            // $C3 / 0308 85 14       STA $14[$00:1E14]   A: 0082 X: 0000 Y: 230D P: eNvMxdIzc
            // $C3 / 030A BF FA 3D C3 LDA $C33DFA,x[$C3: 3DFA] A: 0082 X: 0000 Y: 230D P: eNvMxdIzc
            // $C3 / 030E 85 15       STA $15[$00:1E15]   A: 0047 X: 0000 Y: 230D P: envMxdIzc
            // $C3 / 0310 BF FB 3D C3 LDA $C33DFB,x[$C3: 3DFB] A: 0047 X: 0000 Y: 230D P: envMxdIzc
            // $C3 / 0314 85 16       STA $16[$00:1E16]   A: 00C3 X:0000 Y: 230D P: eNvMxdIzc
            // from 33DF9->33E5B (inclusive) is the whole vanilla offset table - move this to new section
            int sampleOffsetsOffset = offset;
            // code changes
            rom[0x30305] = (byte)sampleOffsetsOffset;
            rom[0x30306] = (byte)(sampleOffsetsOffset >> 8);
            rom[0x30307] = (byte)((sampleOffsetsOffset >> 16) + 0xC0);
            rom[0x3030B] = (byte)(sampleOffsetsOffset + 1);
            rom[0x3030C] = (byte)((sampleOffsetsOffset + 1) >> 8);
            rom[0x3030D] = (byte)(((sampleOffsetsOffset + 1) >> 16) + 0xC0);
            rom[0x30311] = (byte)(sampleOffsetsOffset + 2);
            rom[0x30312] = (byte)((sampleOffsetsOffset + 2) >> 8);
            rom[0x30313] = (byte)(((sampleOffsetsOffset + 2) >> 16) + 0xC0);
            // $C3/044E BF F9 3D C3 LDA $C33DF9,x[$C3:3E5C] A:0063 X:0063 Y:000A P:envMxdIzc
            //$C3 / 0452 85 14       STA $14[$00:1E14]   A: 0042 X: 0063 Y: 000A P:envMxdIzc
            //$C3 / 0454 BF FA 3D C3 LDA $C33DFA,x[$C3: 3E5D] A: 0042 X: 0063 Y: 000A P:envMxdIzc
            //$C3 / 0458 85 15       STA $15[$00:1E15]   A: 0006 X: 0063 Y: 000A P:envMxdIzc
            //$C3 / 045A BF FB 3D C3 LDA $C33DFB,x[$C3: 3E5E] A: 0006 X: 0063 Y: 000A P:envMxdIzc
            //$C3 / 045E 85 16       STA $16[$00:1E16]   A: 0002 X: 0063 Y: 000A P:envMxdIzc
            rom[0x3044F] = (byte)sampleOffsetsOffset;
            rom[0x30450] = (byte)(sampleOffsetsOffset >> 8);
            rom[0x30451] = (byte)((sampleOffsetsOffset >> 16) + 0xC0);
            rom[0x30455] = (byte)(sampleOffsetsOffset + 1);
            rom[0x30456] = (byte)((sampleOffsetsOffset + 1) >> 8);
            rom[0x30457] = (byte)(((sampleOffsetsOffset + 1) >> 16) + 0xC0);
            rom[0x3045B] = (byte)(sampleOffsetsOffset + 2);
            rom[0x3045C] = (byte)((sampleOffsetsOffset + 2) >> 8);
            rom[0x3045D] = (byte)(((sampleOffsetsOffset + 2) >> 16) + 0xC0);

            // copy offset table so it can be added to
            for (int i = 0; i < 33 + numNewSamples; i++)
            {
                if (i < 33)
                {
                    rom[offset++] = rom[VanillaRomOffsets.SAMPLE_OFFSETS + i * 3];
                    rom[offset++] = rom[VanillaRomOffsets.SAMPLE_OFFSETS + i * 3 + 1];
                    rom[offset++] = rom[VanillaRomOffsets.SAMPLE_OFFSETS + i * 3 + 2];
                }
                else
                {
                    // this can be filled in later
                    rom[offset++] = 0;
                    rom[offset++] = 0;
                    rom[offset++] = 0;
                }
            }

            SongExpansionInfo info = new SongExpansionInfo();
            info.sampleFrequenciesOffset = sampleFrequencyOffset;
            info.sampleLoopsOffset = sampleLoopOffset;
            info.sampleOffsetsOffset = sampleOffsetsOffset;
            info.sampleAdsrOffset = sampleAdsrOffset;
            info.songOffsetsOffset = musicOffsetsOffset;
            info.sampleTablesOffset = sampleTableOffset;
            info.totalSamples = 33 + numNewSamples;
            info.totalSongs = 64 + numNewSongs;
            return info;
        }
    }
}
