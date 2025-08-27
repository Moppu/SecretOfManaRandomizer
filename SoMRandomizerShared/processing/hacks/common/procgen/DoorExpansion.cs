using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.ancientcave;
using SoMRandomizer.processing.bossrush;
using SoMRandomizer.processing.chaos;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.util;
using System.Linq;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// Quick hack to move door data from 83xxx to 68xxx and allow more than 0x400 of them to be referenced
    /// by map triggers.  Events are still unable to access ones over >= 0x400, but I don't really care.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DoorExpansion : RandoProcessor
    {
        public const string START_DOOR_OFFSET = "startDoorOffset";
        public const string START_MUSIC_OFFSET = "startMusicOffset";
        protected override string getName()
        {
            return "Door changes for non-vanilla modes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            int numFloors = 0;
            if (randoMode == AncientCaveSettings.MODE_KEY)
            {
                numFloors = AncientCaveGenerator.LENGTH_CONVERSIONS[settings.get(AncientCaveSettings.PROPERTYNAME_LENGTH)];
            }
            else if (randoMode == BossRushSettings.MODE_KEY)
            {
                numFloors = BossOrderRandomizer.allPossibleBosses.Count;
            }
            else if(randoMode == ChaosSettings.MODE_KEY)
            {
                numFloors = ChaosRandomizer.MAPNUM_SETTING_VALUES[settings.get(ChaosSettings.PROPERTYNAME_NUM_FLOORS)];
            }
            else
            {
                Logging.log("Unsupported mode for door expansion");
                return false;
            }

            // move the doors for procgen modes
            context.workingData.setInt(DoorReplacer.PROPERTY_DOOR_LOCATION, 0x68000);

            // doors from 83xxx to 68xxx+
            int startDoorsOffset = context.workingOffset;
            context.workingData.setInt(START_DOOR_OFFSET, startDoorsOffset);
            int startDoorsRomOffset = startDoorsOffset + 0xC00000;
            context.workingOffset += numFloors * 2; // 16bit door, 16bit intro event

            int startMusicOffset = context.workingOffset;
            context.workingData.setInt(START_MUSIC_OFFSET, startMusicOffset);
            int startMusicRomOffset = startMusicOffset + 0xC00000;
            context.workingOffset += numFloors * 4;

            int startMusicRomOffsetP1 = startMusicRomOffset + 1;
            int startMusicRomOffsetP2 = startMusicRomOffset + 2;
            
            // first copy the existing doors
            for (int i = 0; i < 0x1000; i++)
            {
                outRom[0x68000 + i] = outRom[0x83000 + i];
            }

            // remove this BCS: i actually don't know what trigger IDs > 0xC00 do - i think nothing, so we don't support them anymore here
            // $01/E7C9 C9 00 0C    CMP #$0C00          A:080B X:0000 Y:0028 P:envmxdIzC
            // $01/E7CC B0 35       BCS $35[$E803]      A:080B X:0000 Y:0028 P:eNvmxdIzc 
            outRom[0x1E7CC] = 0xEA;
            outRom[0x1E7CD] = 0xEA;

            // replace this block with a custom handler for the door value
            // $01/E7D6 29 FF 03     AND #$03FF            A:080B X:FFFE Y:0090 P:envmxdIzC
            // $01/E7D9 8D 0E 01     STA $010E[$7E:010E]   A:000B X:FFFE Y:0090 P:envmxdIzC
            outRom[0x1E7D6] = 0x22;
            outRom[0x1E7D7] = (byte)(context.workingOffset);
            outRom[0x1E7D8] = (byte)(context.workingOffset >> 8);
            outRom[0x1E7D9] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1E7DA] = 0xEA;
            outRom[0x1E7DB] = 0xEA;

            // special-case door 0xFFFF, and load the start door for the current floor instead of doing the sbc below

            // C9 FF FF      CMP FFFF    
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0xFF;
            // D0 16         BNE past this shit
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x44;
            // AF DC 00 7E   LDA 7E00DC ; load mapnum
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            // AND 00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;

            // LSR
            outRom[context.workingOffset++] = 0x4A;

            // specialcase manabeast map - FD/2 = 7E - load numMaps-1 instead
            // CMP #007E
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x7E;
            outRom[context.workingOffset++] = 0x00;

            // BNE 03
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x03;

            // LDA #numMaps-1
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)(numFloors - 1);
            outRom[context.workingOffset++] = 0x00;

            // ASL: mul by two for 16 bit offset
            outRom[context.workingOffset++] = 0x0A;
            // then two again because we have 4 bytes now
            outRom[context.workingOffset++] = 0x0A;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // TAX
            outRom[context.workingOffset++] = 0xAA;

            // first, play the song
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA #01
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;

            // STA 7E1E00
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x1E;
            outRom[context.workingOffset++] = 0x7E;

            // LDA songloc, x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)startMusicRomOffset;
            outRom[context.workingOffset++] = (byte)(startMusicRomOffset >> 8);
            outRom[context.workingOffset++] = (byte)(startMusicRomOffset >> 16);
            // STA 7E1E01
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x1E;
            outRom[context.workingOffset++] = 0x7E;
            // LDA songlocp1, x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)startMusicRomOffsetP1;
            outRom[context.workingOffset++] = (byte)(startMusicRomOffsetP1 >> 8);
            outRom[context.workingOffset++] = (byte)(startMusicRomOffsetP1 >> 16);
            // STA 7E1E02
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x1E;
            outRom[context.workingOffset++] = 0x7E;
            // LDA songlocp2, x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)startMusicRomOffsetP2;
            outRom[context.workingOffset++] = (byte)(startMusicRomOffsetP2 >> 8);
            outRom[context.workingOffset++] = (byte)(startMusicRomOffsetP2 >> 16);
            // STA 7E1E03
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x1E;
            outRom[context.workingOffset++] = 0x7E;
            // 22 04 00 C3 JSL $C30004 ; play song subr
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xC3;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // TXA
            outRom[context.workingOffset++] = 0x8A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            
            // BF xx xx xx   LDA startDoorsOffset,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)startDoorsRomOffset;
            outRom[context.workingOffset++] = (byte)(startDoorsRomOffset>>8);
            outRom[context.workingOffset++] = (byte)(startDoorsRomOffset>>16);

            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // BRA to the STA
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x04;

            // SEC
            outRom[context.workingOffset++] = 0x38;

            // SBC 0x800
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x08;

            // STA from original block
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x0E;
            outRom[context.workingOffset++] = 0x01;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // change the rom load addrs: rom offset 0x68000 (C6:8000)
            outRom[0x1E7E1] = 0x80;
            outRom[0x1E7E2] = 0xC6;

            outRom[0x1E7EA] = 0x80;
            outRom[0x1E7EB] = 0xC6;

            outRom[0x1E7F4] = 0x80;
            outRom[0x1E7F5] = 0xC6;

            // now, handle this with event code 0x18 (formerly 19, 1a, 1b also, these will no longer be used)
            // instead of a single byte, this will now take two, and we'll execute that door.

            // replace this block, which loads the 0x18 xx, 0x19 xx, etc and extracts the doorway by &ing it with 0x3FF and adding 0x800:
            // $01/EB09 E6 D1 INC $D1[$00:00D1] A:BC30 X:0030 Y:00FE P:envmXdIzc
            // $01/EB0B E6 D1 INC $D1[$00:00D1] A:BC30 X:0030 Y:00FE P:envmXdIzc
            // $01/EB0D A5 04       LDA $04    [$00:0004]
            // $01/EB0F EB          XBA A:BC18 X:0030 Y:00FE P:eNvmXdIzc
            // $01/EB10 29 FF 03    AND #$03FF              A:18BC X:0030 Y:00FE P:eNvmXdIzc
            // $01/EB13 09 00 08    ORA #$0800              A:00BC X:0030 Y:00FE P:envmXdIzc

            // change to:
            // E6 D1  INC $D1
            // A7 D1  LDA [$D1]
            // E6 D1  INC $D1
            // E6 D1  INC $D1
            // starting at 0x1EB09 (13 src bytes -> 8)
            // doors in events will now have to be [18 BC 08] for mana beast door 0xBC for example

            // skip ahead to the 16-bit door value parameter
            // INC $D1
            outRom[0x1EB09] = 0xE6;
            outRom[0x1EB0A] = 0xD1;

            // load the 16-bit door value
            // LDA [$D1]
            outRom[0x1EB0B] = 0xA7;
            outRom[0x1EB0C] = 0xD1;

            // skip the door value for the next event command
            // INC $D1
            outRom[0x1EB0D] = 0xE6;
            outRom[0x1EB0E] = 0xD1;

            // INC $D1
            outRom[0x1EB0F] = 0xE6;
            outRom[0x1EB10] = 0xD1;

            // remove the rest
            outRom[0x1EB11] = 0xEA;
            outRom[0x1EB12] = 0xEA;
            outRom[0x1EB13] = 0xEA;
            outRom[0x1EB14] = 0xEA;
            outRom[0x1EB15] = 0xEA;
            return true;
        }
    }
}
