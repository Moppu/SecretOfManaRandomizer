using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.vanillarando;
using SoMRandomizer.util;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Hack that makes you automatically jump through whip posts if you have the whip.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FastWhipPosts : RandoProcessor
    {
        protected override string getName()
        {
            return "Automatic whip-post jumps if you have the whip";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(OpenWorldSettings.PROPERTYNAME_FAST_WHIP_POSTS))
            {
                // MOPPLE: it would probably be nice to make this hack work in all modes, but it hasn't been tested yet. also the config item is currently specific to open world.
                return false;
            }
            
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 400); // idk a bunch

            // 6f8 - not referenced anymore
            EventScript newEvent6f8 = new EventScript();
            context.replacementEvents[0x6f8] = newEvent6f8;
            newEvent6f8.Add(EventCommandEnum.END.Value);

            // 6d8 - stepped on whip tile - change to automatically go if you have whip and a whip post target can be found in any direction
            EventScript newEvent6d8 = new EventScript();
            context.replacementEvents[0x6d8] = newEvent6d8;
            // short circuit if no whip
            newEvent6d8.Logic(EventFlags.HAVE_WHIP, 0x0, 0x0, EventScript.GetJumpCmd(0));

            newEvent6d8.Add(0x06);

            newEvent6d8.Add(0x1F); 
            newEvent6d8.Add(0x10);

            newEvent6d8.IncrFlag(EventFlags.WALK_THROUGH_WALLS_FLAG);

            newEvent6d8.SetFlag(EventFlags.WHIP_GROUPING_FLAG, 0xF);
            
            int[] subrOffsets = new int[3];
            byte[] src = new byte[] { 0xE0, 0xE2, 0xE4 };
            byte[] dest1 = new byte[] { 0xE2, 0xE0, 0xE0 };
            byte[] dest2 = new byte[] { 0xE4, 0xE4, 0xE2 };
            int[] zeroOffsets = new int[] { 0xe008, 0xe041, 0xe006, 0xe007, 0xe013, 0xe067 };
            // three total subroutines to move other two characters to player i
            // copy x, y, screen x, screen y, z, and direction.
            for (int i = 0; i < 3; i++)
            {
                subrOffsets[i] = context.workingOffset;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // LDA 7eex02 - x
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x02;
                outRom[context.workingOffset++] = src[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex02
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x02;
                outRom[context.workingOffset++] = dest1[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex02
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x02;
                outRom[context.workingOffset++] = dest2[i];
                outRom[context.workingOffset++] = 0x7E;
                // LDA 7eex20 - screen x
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x20;
                outRom[context.workingOffset++] = src[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex20
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x20;
                outRom[context.workingOffset++] = dest1[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex20
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x20;
                outRom[context.workingOffset++] = dest2[i];
                outRom[context.workingOffset++] = 0x7E;
                // LDA 7eex04 - y
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x04;
                outRom[context.workingOffset++] = src[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex04
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x04;
                outRom[context.workingOffset++] = dest1[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex04
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x04;
                outRom[context.workingOffset++] = dest2[i];
                outRom[context.workingOffset++] = 0x7E;
                // LDA 7eex22 - screen y
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = src[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex22
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = dest1[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex22
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = dest2[i];
                outRom[context.workingOffset++] = 0x7E;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA 7eex0b - z
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x0B;
                outRom[context.workingOffset++] = src[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex0b
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x0B;
                outRom[context.workingOffset++] = dest1[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex0b
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x0B;
                outRom[context.workingOffset++] = dest2[i];
                outRom[context.workingOffset++] = 0x7E;
                // LDA 7eex10 - direction
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x10;
                outRom[context.workingOffset++] = src[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex10
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x10;
                outRom[context.workingOffset++] = dest1[i];
                outRom[context.workingOffset++] = 0x7E;
                // STA 7eex10
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x10;
                outRom[context.workingOffset++] = dest2[i];
                outRom[context.workingOffset++] = 0x7E;
                // LDA #00
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x00;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            /*
             * replace implementation for "move characters to p1" (0x03) event command, starting here:
                C1/E9FB:    A980        LDA #$80
                C1/E9FD:    854E        STA $4E
                C1/E9FF:    85D0        STA $D0
             */
            outRom[0x1E9FB] = 0x22;
            outRom[0x1E9FC] = (byte)(context.workingOffset);
            outRom[0x1E9FD] = (byte)(context.workingOffset >> 8);
            outRom[0x1E9FE] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1E9FF] = 0x60; // RTS after
            outRom[0x1EA00] = 0xEA;

            outRom[0x1EA25] = 0x6B; // RTS -> RTL

            // LDA 7ecf03 - whip grouping flag
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = EventFlags.WHIP_GROUPING_FLAG;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // CMP #0F
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x0F;
            // BEQ doStuff - for 0x0F, skip doing the usual 0x03 command
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 11;

            // replaced code + JSL to the old spot and run it as a long, then RTL, then RTS (above)
            // LDA #80
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x80;
            // STA $4E
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x4E;
            // STA $D0
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xD0;
            // JSL 01EA01
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            
            // - immediately move p2/p3 to p1, and also set direction, and 0B?
            // REP #10
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x10;
            // LDX #0000
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // CPX $D4
            outRom[context.workingOffset++] = 0xE4;
            outRom[context.workingOffset++] = 0xD4;
            // BNE notBoy
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x07;
            // JSL setGirlSprite - move girl/sprite to boy position
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(subrOffsets[0]);
            outRom[context.workingOffset++] = (byte)(subrOffsets[0] >> 8);
            outRom[context.workingOffset++] = (byte)((subrOffsets[0] >> 16) + 0xC0);
            // INC $D1
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0xD1;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // notBoy:
            // LDX #0200
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x02;
            // CPX $D4
            outRom[context.workingOffset++] = 0xE4;
            outRom[context.workingOffset++] = 0xD4;
            // BNE notGirl
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x07;
            // JSL setBoySprite - set boy/sprite to girl position
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(subrOffsets[1]);
            outRom[context.workingOffset++] = (byte)(subrOffsets[1] >> 8);
            outRom[context.workingOffset++] = (byte)((subrOffsets[1] >> 16) + 0xC0);
            // INC $D1
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0xD1;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // notGirl:
            // JSL setBoyGirl - move boy/girl to sprite position
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(subrOffsets[2]);
            outRom[context.workingOffset++] = (byte)(subrOffsets[2] >> 8);
            outRom[context.workingOffset++] = (byte)((subrOffsets[2] >> 16) + 0xC0);
            // INC $D1
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0xD1;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            newEvent6d8.Add(EventCommandEnum.MOVE_EVERYONE_TO_P1.Value);
            newEvent6d8.DecrFlag(EventFlags.WALK_THROUGH_WALLS_FLAG);
            

            // make subroutines to look for a whip post in each direction from the tile we stepped on
            int leftCheckSubrLoc = context.workingOffset;
            makeCheckDirSubr(outRom, context, 0x82, 0xFFFC);
            int rightCheckSubrLoc = context.workingOffset;
            makeCheckDirSubr(outRom, context, 0x02, 0x0004);
            int downCheckSubrLoc = context.workingOffset;
            makeCheckDirSubr(outRom, context, 0x01, 0x0200);
            int upCheckSubrLoc = context.workingOffset;
            makeCheckDirSubr(outRom, context, 0x00, 0xFE00);

            /*
             *  replace code to load event flag value for "increment event flag" event command (0x29):
                C1/EC4A:    BD00CF      LDA $CF00,X
                C1/EC4D:    1A          INC A
             */
            outRom[0x1EC4A] = 0x22;
            outRom[0x1EC4B] = (byte)(context.workingOffset);
            outRom[0x1EC4C] = (byte)(context.workingOffset >> 8);
            outRom[0x1EC4D] = (byte)((context.workingOffset >> 16) + 0xC0);

            // removed code
            // LDA $CF00,X
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCF;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // check flag
            // CPX #WHIP_GROUPING_FLAG
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = EventFlags.WHIP_GROUPING_FLAG;
            // BEQ over
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // save off a/x
            // REP 30
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x30;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PHX
            outRom[context.workingOffset++] = 0xDA;

            // here: check all 4 dirs and return if one of them worked
            // JSL leftCheckSubrLoc
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(leftCheckSubrLoc);
            outRom[context.workingOffset++] = (byte)(leftCheckSubrLoc >> 8);
            outRom[context.workingOffset++] = (byte)((leftCheckSubrLoc >> 16) + 0xC0);
            // CMP #0002
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x00;
            // BEQ checkNext (5)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x05;
            // FA, 68, E2 30, 6B - PLX, PLA, SEP 30, RTL
            outRom[context.workingOffset++] = 0xFA;
            outRom[context.workingOffset++] = 0x68;
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x6B;

            // JSL rightCheckSubrLoc
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(rightCheckSubrLoc);
            outRom[context.workingOffset++] = (byte)(rightCheckSubrLoc >> 8);
            outRom[context.workingOffset++] = (byte)((rightCheckSubrLoc >> 16) + 0xC0);
            // CMP #0002
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x00;
            // BEQ checkNext (5)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x05;
            // FA, 68, E2 30, 6B - PLX, PLA, SEP 30, RTL
            outRom[context.workingOffset++] = 0xFA;
            outRom[context.workingOffset++] = 0x68;
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x6B;

            // JSL downCheckSubrLoc
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(downCheckSubrLoc);
            outRom[context.workingOffset++] = (byte)(downCheckSubrLoc >> 8);
            outRom[context.workingOffset++] = (byte)((downCheckSubrLoc >> 16) + 0xC0);
            // CMP #0002
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x00;
            // BEQ checkRight (5)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x05;
            // FA, 68, E2 30, 6B - PLX, PLA, SEP 30, RTL
            outRom[context.workingOffset++] = 0xFA;
            outRom[context.workingOffset++] = 0x68;
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x6B;

            // JSL upCheckSubrLoc
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(upCheckSubrLoc);
            outRom[context.workingOffset++] = (byte)(upCheckSubrLoc >> 8);
            outRom[context.workingOffset++] = (byte)((upCheckSubrLoc >> 16) + 0xC0);
            // CMP #0002
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x00;
            // BEQ checkRight (5)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x05;
            // FA, 68, E2 30, 6B - PLX, PLA, SEP 30, RTL
            outRom[context.workingOffset++] = 0xFA;
            outRom[context.workingOffset++] = 0x68;
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x6B;


            // failed all 4 checks - load 02 and return
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // SEP 30
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x30;
            // indicate to caller we failed; use 02 as the event flag value
            // LDA #02
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x02;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            
            newEvent6d8.SetFlag(EventFlags.WHIP_GROUPING_FLAG, 0);
            newEvent6d8.IncrFlag(EventFlags.WHIP_GROUPING_FLAG);

            // immediately check that 03 is still set to 1, after the hack to check map stuff
            newEvent6d8.Logic(EventFlags.WHIP_GROUPING_FLAG, 0x2, 0x2, EventScript.GetJumpCmd(0));
            
            newEvent6d8.IncrFlag(EventFlags.WALK_THROUGH_WALLS_FLAG);

            // logic for if player == 0, 1, 2
            newEvent6d8.Add(0x38);
            newEvent6d8.Add(0x00);
            newEvent6d8.Jsr(0x6FC);

            newEvent6d8.Add(0x38);
            newEvent6d8.Add(0x01);
            newEvent6d8.Jsr(0x6FD);

            newEvent6d8.Add(0x38);
            newEvent6d8.Add(0x02);
            newEvent6d8.Jsr(0x6FE);

            newEvent6d8.SetFlag(EventFlags.WHIP_GROUPING_FLAG, 0xF);

            newEvent6d8.Add(EventCommandEnum.MOVE_EVERYONE_TO_P1.Value);

            newEvent6d8.SetFlag(EventFlags.WHIP_GROUPING_FLAG, 0x0);

            newEvent6d8.DecrFlag(EventFlags.WHIP_GROUPING_FLAG);

            newEvent6d8.DecrFlag(EventFlags.WALK_THROUGH_WALLS_FLAG);

            newEvent6d8.End();

            return true;
        }

        private void makeCheckDirSubr(byte[] outRom, RandoContext context, byte dirValue, int tileIndexAddValue)
        {
            // load current character id into x
            // LDA $7E00D5
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xD5;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // XBA
            outRom[context.workingOffset++] = 0xEB;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA $7EE002,X - load x position as map tile ref (divide 16)
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // LSR 4 times
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA $7EE004,X - load y position as map tile ref (divide 16)
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // LSR 4 times
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;
            // XBA
            outRom[context.workingOffset++] = 0xEB;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC $S,1 - combine with x for full tile ref id
            outRom[context.workingOffset++] = 0x63;
            outRom[context.workingOffset++] = 0x01;
            // PHA - shove it onto stack
            outRom[context.workingOffset++] = 0x48;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // REP 20 .. hmm
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #tileIndexAddValue
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)tileIndexAddValue; // 0xFFFC = 4 tiles left, for example
            outRom[context.workingOffset++] = (byte)(tileIndexAddValue>>8);
            // clc and add our tile position to the one we generated based off direction
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC $S,1
            outRom[context.workingOffset++] = 0x63;
            outRom[context.workingOffset++] = 0x01;
            // even out the stack by pulling x twice, and use our calculated value as x
            // PLX, PLX
            outRom[context.workingOffset++] = 0xFA;
            outRom[context.workingOffset++] = 0xFA;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // check for map layer 1 or 2 for collisions
            // LDA $7E00DF
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // AND #01
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x01;
            // BEQ 06
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x06;
            // layer 2 collision - load from 7F:4000
            // LDA $7F4000,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x40;
            outRom[context.workingOffset++] = 0x7F;
            // BRA 04
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x04;
            // layer 1 collision - load from 7F:0000
            // LDA $7F0000,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7F;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // use the tile value * 4 as an index into a table at 7F:B800; if the [2,3] value is 6FF8 (which is an event reference on the collision data) we found a whip post
            // ASL, ASL
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA $7FB802,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xB8;
            outRom[context.workingOffset++] = 0x7F;
            // CMP #6FF8 - check for whip destination tile
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xF8;
            outRom[context.workingOffset++] = 0x6F;
            // BEQ 04 - jump to success case if match
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x04;
            // LDA #0002
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x00;
            // RTL
            outRom[context.workingOffset++] = 0x6B; // return from fail case

            // extra stuff here .. 
            // heal statuses so we don't softlock when trying to teleport, take out everything but dead (8000)
            // success case: set the status flags that are normally cleared by event commands; it's faster to do it directly here than in the event

            // LDA $E190 - boy
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE1;
            // AND #8000
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x80;
            // STA $E190 - boy
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE1;

            // LDA $E390 - girl
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE3;
            // AND #8000
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x80;
            // STA $E390 - girl
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE3;

            // LDA $E590 - sprite
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE5;
            // AND #8000
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x80;
            // STA $E590 - sprite
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE5;

            // set direction here
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA #dir
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = dirValue;

            // STA $E010 - boy
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0xE0;
            // STA $E210 - girl
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0xE2;
            // STA $E410 - sprite
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0xE4;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #1 - indicate success
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // RTL
            outRom[context.workingOffset++] = 0x6B; // return from success case
        }
    }
}
