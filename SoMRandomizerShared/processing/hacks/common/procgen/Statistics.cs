using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// Track some useless statistics to display at the end of the run.  See TimerDialogue for the display code.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class Statistics : RandoProcessor
    {
        protected override string getName()
        {
            return "Statistics tracker for non-vanilla modes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // MOPPLE: these are pretty busted, seems like maybe something is overwriting them in RAM? should be fixed someday
            // they are currently only used for procgen modes, and only ancient cave really puts anything in it
            

            // ///////////////////////////////////////////////////////
            // init all values to zero
            // ///////////////////////////////////////////////////////

            int NUM_STATISTICS_BYTES = 128;
            // this is when player characters change, should only execute at the beginning of the ac run
            // $01/ECAE 9D 00 E0    STA $E000,x[$7E:E400]   A:0001 X:0400 Y:0000 P:envmxdIzc
            // $01/ECB1 A4 3A       LDY $3A    [$00:003A]   A:0001 X:0200 Y:0000 P:envmxdIzc
            outRom[0x1ECAE] = 0x22;
            outRom[0x1ECAF] = (byte)(context.workingOffset);
            outRom[0x1ECB0] = (byte)(context.workingOffset >> 8);
            outRom[0x1ECB1] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1ECB2] = 0xEA;

            // replace with:
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // PHY
            outRom[context.workingOffset++] = 0x5A;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA $STATISTICS_BASE_OFFSET
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATISTICS_BASE_OFFSET;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATISTICS_BASE_OFFSET >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATISTICS_BASE_OFFSET >> 16);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDX #STATISTICS_BASE_OFFSET
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATISTICS_BASE_OFFSET;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATISTICS_BASE_OFFSET >> 8);
            // LDY #STATISTICS_BASE_OFFSET+1
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATISTICS_BASE_OFFSET + 1);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.STATISTICS_BASE_OFFSET + 1) >> 8);
            // LDA #NUM_STATISTICS_BYTES-2
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)(NUM_STATISTICS_BYTES - 2);
            outRom[context.workingOffset++] = (byte)((NUM_STATISTICS_BYTES - 2) >> 8);
            // MVN 7E 7E
            outRom[context.workingOffset++] = 0x54;
            outRom[context.workingOffset++] = 0x7E;
            outRom[context.workingOffset++] = 0x7E;
            // use 7ea208 as rng for the moon phase screen; normally it depends on save files, but procgen modes have no
            // saving, so it turns out the same every time
            // AF 0B A2 7E    LDA $7EA20B
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // 8F 01 60 30    STA $306001 - write to saveram checksum, which is used for the moon phase/stars
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x60;
            outRom[context.workingOffset++] = 0x30;
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // 9D 00 E0    STA $E000,x ; removed code
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xE0;
            // A4 3A       LDY $3A     ; removed code
            outRom[context.workingOffset++] = 0xA4;
            outRom[context.workingOffset++] = 0x3A;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;


            // ///////////////////////////////////////////////////////
            // Kills/deaths
            // ///////////////////////////////////////////////////////

            // $C0/4205 9E B1 E1    STZ $E1B1,x[$7E:E9B1]   A:0000 X:0800 Y:0011 P:envMxdIZc
            // $C0/4208 9E B0 E1    STZ $E1B0,x[$7E:E9B0]   A:0000 X:0800 Y:0011 P:envMxdIZc
            // gets called when something dies.  if x 0000, 0200, or 0400, it's a player

            outRom[0x4205] = 0x22;
            outRom[0x4206] = (byte)(context.workingOffset);
            outRom[0x4207] = (byte)(context.workingOffset >> 8);
            outRom[0x4208] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x4209] = 0xEA;
            outRom[0x420A] = 0xEA;

            // so replace this with:
            // 9E B1 E1    STZ $E1B1,x    ; original A
            outRom[context.workingOffset++] = 0x9E;
            outRom[context.workingOffset++] = 0xB1;
            outRom[context.workingOffset++] = 0xE1;
            // 9E B0 E1    STZ $E1B0,x    ; original B
            outRom[context.workingOffset++] = 0x9E;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;
            // E0 00 06    CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // B0 xx       BCS/BGE enemy
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x0E;
            // C2 20       REP #20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AF xx xx xx LDA $PLAYER_KILLS_OFFSET_16BIT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.PLAYER_KILLS_OFFSET_16BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.PLAYER_KILLS_OFFSET_16BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.PLAYER_KILLS_OFFSET_16BIT >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA $PLAYER_KILLS_OFFSET_16BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.PLAYER_KILLS_OFFSET_16BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.PLAYER_KILLS_OFFSET_16BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.PLAYER_KILLS_OFFSET_16BIT >> 16);
            // E2 20       SEP #20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;
            // enemy:
            // C2 20       REP #20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AF xx xx xx LDA $ENEMY_KILLS_OFFSET_16BIT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.ENEMY_KILLS_OFFSET_16BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.ENEMY_KILLS_OFFSET_16BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.ENEMY_KILLS_OFFSET_16BIT >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA $ENEMY_KILLS_OFFSET_16BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.ENEMY_KILLS_OFFSET_16BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.ENEMY_KILLS_OFFSET_16BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.ENEMY_KILLS_OFFSET_16BIT >> 16);
            // E2 20       SEP #20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;


            // chest drop
            // $C0 / 42A8 DE 82 E1    DEC $E182,x[$7E:E982]   A:0F84 X:0800 Y:007F P:eNvMxdIzC
            // $C0 / 42AB A9 80       LDA #$80                A:0F84 X:0800 Y:007F P:eNvMxdIzC
            outRom[0x42A6] = 0xEA;
            outRom[0x42A7] = 0xEA;
            outRom[0x42A8] = 0x22;
            outRom[0x42A9] = (byte)(context.workingOffset);
            outRom[0x42AA] = (byte)(context.workingOffset >> 8);
            outRom[0x42AB] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x42AC] = 0xEA;

            // replace with:
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AF xx xx xx LDA $NUM_CHESTS_OFFSET_16BIT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.NUM_CHESTS_OFFSET_16BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.NUM_CHESTS_OFFSET_16BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.NUM_CHESTS_OFFSET_16BIT >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA $NUM_CHESTS_OFFSET_16BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.NUM_CHESTS_OFFSET_16BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.NUM_CHESTS_OFFSET_16BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.NUM_CHESTS_OFFSET_16BIT >> 16);
            // E2 20       SEP #20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // A9 80       LDA #$80    ; replaced code
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x80;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;

            // ///////////////////////////////////////////////////////
            // Walking distance
            // ///////////////////////////////////////////////////////

            // (x axis movement)
            //$00 / D8EF 9D 02 E0    STA $E002,x[$7E:EA02]   A:0263 X:0A00 Y:0232 P:eNvmxdIzc
            //$00 / D8F2 E0 00 06    CPX #$0600              A:0142 X:0800 Y:0202 P:envmxdIzC
            outRom[0xD8EF] = 0x22;
            outRom[0xD8F0] = (byte)(context.workingOffset);
            outRom[0xD8F1] = (byte)(context.workingOffset >> 8);
            outRom[0xD8F2] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0xD8F3] = 0xEA;
            outRom[0xD8F4] = 0xEA;
            // E0 00 06    CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // 90 xx       BCC/BLT player
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x07;
            // (replaced code)
            // 9D 02 E0    STA $E002,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xE0;
            // E0 00 06    CPX #$0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;
            // player:
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // E2 20       SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // BD 2C E0    LDA $E02C,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x2C;
            outRom[context.workingOffset++] = 0xE0;
            // F0          BEQ npc
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x33;
            // C2 20       REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // 38          SEC
            outRom[context.workingOffset++] = 0x38;
            // FD 02 E0    SBC $E002,x
            outRom[context.workingOffset++] = 0xFD;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xE0;
            // 10 xx       BPL skip
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x05;
            // 49 FF FF    EOR #FFFF
            outRom[context.workingOffset++] = 0x49;
            outRom[context.workingOffset++] = 0xFF;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 69 01       ADC #01
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x01;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND 000F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            outRom[context.workingOffset++] = 0x00;
            // skip:
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 6F xx xx xx ADC $MOVE_DIST_24BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.MOVE_DIST_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT >> 16);
            // CMP #2710
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // BCC/BLT noOverflow
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0F;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // AF xx xx xx LDA $MOVE_DIST_24BIT+2
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT +2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.MOVE_DIST_32BIT +2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.MOVE_DIST_32BIT +2) >> 16);
            // 1A          INA
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA $MOVE_DIST_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.MOVE_DIST_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.MOVE_DIST_32BIT + 2) >> 16);
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #2710
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // noOverflow:
            // 8F xx xx xx STA $MOVE_DIST_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.MOVE_DIST_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT >> 16);
            // npc:
            // C2 20       REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // (replaced code)
            // 9D 02 E0    STA $E002,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xE0;
            // E0 00 06    CPX #$0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;

            // (y axis movement)
            // $00 / D93E 9D 04 E0    STA $E004,x[$7E:E804]   A:01C9 X:0800 Y:01C2 P:eNvmxdIzc
            // $00 / D941 E0 00 06    CPX #$0600              A:01D3 X:0800 Y:0202 P:eNvmxdIzc
            outRom[0xD93E] = 0x22;
            outRom[0xD93F] = (byte)(context.workingOffset);
            outRom[0xD940] = (byte)(context.workingOffset >> 8);
            outRom[0xD941] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0xD942] = 0xEA;
            outRom[0xD943] = 0xEA;
            // for y
            // E0 00 06    CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // 90 xx       BCC/BLT player
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x07;
            // (replaced code)
            // 9D 02 E0    STA $E004,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0xE0;
            // E0 00 06    CPX #$0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;
            //
            // player:
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // E2 20       SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // BD 2C E0    LDA $E02C,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x2C;
            outRom[context.workingOffset++] = 0xE0;
            // F0          BEQ npc
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x33;
            // C2 20       REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // *****
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // *****
            // 38          SEC
            outRom[context.workingOffset++] = 0x38;
            // FD 02 E0    SBC $E004,x
            outRom[context.workingOffset++] = 0xFD;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0xE0;
            // 10 xx       BPL skip
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x05;
            // 49 FF FF    EOR #FFFF
            outRom[context.workingOffset++] = 0x49;
            outRom[context.workingOffset++] = 0xFF;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 69 01       ADC #01
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x01;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND 000F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            outRom[context.workingOffset++] = 0x00;
            // skip:
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 6F xx xx xx ADC $MOVE_DIST_24BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.MOVE_DIST_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT >> 16);
            // CMP #2710
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // BCC/BLT noOverflow
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0F;

            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // AF xx xx xx LDA $MOVE_DIST_24BIT+2
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.MOVE_DIST_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.MOVE_DIST_32BIT + 2) >> 16);
            // 1A          INA
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA $MOVE_DIST_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.MOVE_DIST_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.MOVE_DIST_32BIT + 2) >> 16);
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #2710
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // noOverflow:
            // 8F xx xx xx STA $MOVE_DIST_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.MOVE_DIST_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.MOVE_DIST_32BIT >> 16);
            // npc:
            // C2 20       REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // (replaced code)
            // 9D 02 E0    STA $E004,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0xE0;
            // E0 00 06    CPX #$0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;


            // ///////////////////////////////////////////////////////
            // Physical damage dealt/taken
            // ///////////////////////////////////////////////////////

            // $C0/51E1 9D F1 E1    STA $E1F1,x[$7E:E1F1]   A:000A X:0000 Y:0600 P:envmxdIzc (physical damage)
            // $C0/51E4 E2 20       SEP #$20                A:000A X:0000 Y:0600 P:envmxdIzc
            outRom[0x51E1] = 0x22;
            outRom[0x51E2] = (byte)(context.workingOffset);
            outRom[0x51E3] = (byte)(context.workingOffset >> 8);
            outRom[0x51E4] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x51E5] = 0xEA;

            // replaced code A
            // 9D F1 E1    STA $E1F1,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xF1;
            outRom[context.workingOffset++] = 0xE1;
            // E0 00 06    CPX #$0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // 90 xx       BCC/BLT player
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x3F;
            // (enemy)
            // ; total damage dealt
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 6F xx xx xx ADC TOTAL_DAMAGE_DEALT_24BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT >> 16);
            // CMP #2710
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // BCC/BLT noOverflow
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0F;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // AF xx xx xx LDA TOTAL_DAMAGE_DEALT_24BIT+2
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2) >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA TOTAL_DAMAGE_DEALT_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2) >> 16);
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #2710
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // noOverflow:
            // 8F xx xx xx STA TOTAL_DAMAGE_DEALT_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT >> 16);
            // ; now total physical damage dealt too
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 6F xx xx xx ADC TOTAL_P_DAMAGE_DEALT_24BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT >> 16);
            // CMP #2710
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // BCC/BLT noPOverflow
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0F;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // AF xx xx xx LDA TOTAL_P_DAMAGE_DEALT_24BIT+2
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT + 2) >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA TOTAL_P_DAMAGE_DEALT_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT + 2) >> 16);
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #2710
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // noPOverflow:
            // 8F xx xx xx STA TOTAL_DAMAGE_DEALT_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_DEALT_32BIT >> 16);
            // ; end
            // E2 20       SEP #$20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;

            // player:
            // ; total damage taken
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 6F xx xx xx ADC TOTAL_DAMAGE_TAKEN_24BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT >> 16);
            // CMP #2710
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // BCC/BLT noOverflow
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0F;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // AF xx xx xx LDA TOTAL_DAMAGE_TAKEN_24BIT+2
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2) >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA TOTAL_DAMAGE_TAKEN_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2) >> 16);
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #2710
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // noOverflow:
            // 8F xx xx xx STA TOTAL_DAMAGE_TAKEN_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT >> 16);
            // ; now total physical damage taken too
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 6F xx xx xx ADC TOTAL_P_DAMAGE_TAKEN_24BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT >> 16);
            // CMP #2710
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // BCC/BLT noPOverflow
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0F;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // AF xx xx xx LDA TOTAL_P_DAMAGE_TAKEN_24BIT+2
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT + 2) >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA TOTAL_P_DAMAGE_TAKEN_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT + 2) >> 16);
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #2710
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // noPOverflow:
            // 8F xx xx xx STA TOTAL_DAMAGE_TAKEN_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_P_DAMAGE_TAKEN_32BIT >> 16);
            // ; end
            // E2 20       SEP #$20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;


            // ///////////////////////////////////////////////////////
            // Magical damage dealt/taken
            // ///////////////////////////////////////////////////////

            // $C8/EB90 9D F1 E1    STA $E1F1,x[$7E:E1F1]   A:0028 X:0000 Y:0800 P:eNvmxdIzc (magical damage)
            // $C8/EB93 A5 A4       LDA $A4    [$00:03A4]   A:0029 X:0000 Y:0800 P:eNvmxdIzc

            // $C8/E9F6 9D F1 E1    STA $E1F1,x[$7E:E1F1]   A:000B X:0000 Y:0A00 P:envmxdIzc also for burst
            // $C8/E9F9 E2 20       SEP #$20                A:0013 X:0000 Y:0A00 P:envmxdIzc

            // energy absorb, etc
            outRom[0x8EB90] = 0x22;
            outRom[0x8EB91] = (byte)(context.workingOffset);
            outRom[0x8EB92] = (byte)(context.workingOffset >> 8);
            outRom[0x8EB93] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8EB94] = 0xEA;

            injectMagicDamageTracker(outRom, context, new byte[] { 0xA5, 0xA4 });

            // burst, exploder
            outRom[0x8E9F6] = 0x22;
            outRom[0x8E9F7] = (byte)(context.workingOffset);
            outRom[0x8E9F8] = (byte)(context.workingOffset >> 8);
            outRom[0x8E9F9] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8E9FA] = 0xEA;

            injectMagicDamageTracker(outRom, context, new byte[] { 0xE2, 0x20 });
            
            return true;
        }

        private void injectMagicDamageTracker(byte[] outRom, RandoContext context, byte[] replacedCode)
        {
            // replaced code A
            // 9D F1 E1    STA $E1F1,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xF1;
            outRom[context.workingOffset++] = 0xE1;
            // E0 00 06    CPX #$0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // 90 xx       BCC/BLT player
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x3F;
            // (enemy)
            // ; total damage dealt
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 6F xx xx xx ADC TOTAL_DAMAGE_DEALT_24BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT >> 16);
            // CMP #2710
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // BCC/BLT noOverflow
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0F;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // AF xx xx xx LDA TOTAL_DAMAGE_DEALT_24BIT+2
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2) >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA TOTAL_DAMAGE_DEALT_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT + 2) >> 16);
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #2710
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;

            // noOverflow:
            // 8F xx xx xx STA TOTAL_DAMAGE_DEALT_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_DEALT_32BIT >> 16);
            // ; now total physical damage dealt too
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 6F xx xx xx ADC TOTAL_P_DAMAGE_DEALT_24BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT >> 16);
            // CMP #2710
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // BCC/BLT noPOverflow
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0F;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // AF xx xx xx LDA TOTAL_P_DAMAGE_DEALT_24BIT+2
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT + 2) >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA TOTAL_P_DAMAGE_DEALT_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT + 2) >> 16);
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #2710
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // noPOverflow:
            // 8F xx xx xx STA TOTAL_DAMAGE_DEALT_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_DEALT_32BIT >> 16);
            // ; end (replaced code)
            foreach (byte b in replacedCode)
            {
                outRom[context.workingOffset++] = b;
            }
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;

            // player:
            // ; total damage taken
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 6F xx xx xx ADC TOTAL_DAMAGE_TAKEN_24BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT >> 16);
            // CMP #2710
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // BCC/BLT noOverflow
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0F;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // AF xx xx xx LDA TOTAL_DAMAGE_TAKEN_24BIT+2
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2) >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA TOTAL_DAMAGE_TAKEN_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT + 2) >> 16);
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #2710
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // noOverflow:
            // 8F xx xx xx STA TOTAL_DAMAGE_TAKEN_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_DAMAGE_TAKEN_32BIT >> 16);
            // ; now total physical damage taken too
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // 18          CLC
            outRom[context.workingOffset++] = 0x18;
            // 6F xx xx xx ADC TOTAL_P_DAMAGE_TAKEN_24BIT
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT >> 16);
            // CMP #2710
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // BCC/BLT noPOverflow
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0F;
            // 48          PHA
            outRom[context.workingOffset++] = 0x48;
            // AF xx xx xx LDA TOTAL_P_DAMAGE_TAKEN_24BIT+2
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT + 2) >> 16);
            // 1A          INC A
            outRom[context.workingOffset++] = 0x1A;
            // 8F xx xx xx STA TOTAL_P_DAMAGE_TAKEN_24BIT+2
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT + 2);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT + 2) >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT + 2) >> 16);
            // 68          PLA
            outRom[context.workingOffset++] = 0x68;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #2710
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x27;
            // noPOverflow:
            // 8F xx xx xx STA TOTAL_DAMAGE_TAKEN_24BIT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.TOTAL_M_DAMAGE_TAKEN_32BIT >> 16);
            // ; end
            foreach(byte b in replacedCode)
            {
                outRom[context.workingOffset++] = b;
            }
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;
        }
    }
}
