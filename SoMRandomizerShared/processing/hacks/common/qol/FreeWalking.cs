using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// An attempt at allowing you to leave the screen even if P2 and P3 are stuck on something.
    /// Currently this... 1. removes the scroll prevention for character distance, 2. disallows 
    /// x-button menus from opening for off-screen characters, 3. disallows picking those
    /// characters using the select button, and 4. teleports characters who have been off 
    /// screen for a while back to p1.
    /// It has some minor issues and probably could have a better implementation given more time
    /// and effort.  It also hasn't been tested with a second player.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FreeWalking : RandoProcessor
    {
        protected override string getName()
        {
            return "Allow characters to be off screen";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_SCROLL_FIX))
            {
                return false;
            }

            // //////////////////////////////////////////////////////////
            // Removal of scrolling limitation
            // //////////////////////////////////////////////////////////

            int teleportTimerInFrames = 60;

            // this STZ stops you from moving horizontally if separated from the other characters - remove it
            //$00 / D9E5 64 1E       STZ $1E[$00:001E]   A: 00C8 X:0400 Y: 0067 P: envMxdIZC
            outRom[0xD9E5] = 0xEA;
            outRom[0xD9E6] = 0xEA;
            // same for moving vertically
            outRom[0xDA6B] = 0xEA;
            outRom[0xDA6C] = 0xEA;


            // //////////////////////////////////////////////////////////
            // Limitation for selecting a character based on screen x/y
            // //////////////////////////////////////////////////////////

            /*
             * replace:
               $00 / BD72 A6 04       LDX $04[$00:0004]       A: 0000 X: 0400 Y: 0000 P: envMxdIZc
               $00 / BD74 9E 2C E0    STZ $E02C,x[$7E:E22C]   A: 0000 X: 0200 Y: 0000 P: envMxdIzc

               note that 7EEx2C for a player indicates which player number they are, and is 0 for npc control
             */
            outRom[0xBD72] = 0xEA;
            outRom[0xBD73] = 0x22;
            outRom[0xBD74] = (byte)(context.workingOffset);
            outRom[0xBD75] = (byte)(context.workingOffset >> 8);
            outRom[0xBD76] = (byte)((context.workingOffset >> 16) + 0xC0);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7EE020,X - this is the screen x position of the character
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // BMI fixWindowPosition
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x18;
            // CMP #00E0
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            // BCS/BGE fixWindowPosition
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x13;
            // LDA 7EE020,Y - this is the screen y position of the character
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // BMI fixWindowPosition
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x0D;
            // CMP #00E0
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            // BCS/BGE fixWindowPosition
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x08;
            // (removed code) LDX $04
            outRom[context.workingOffset++] = 0xA6;
            outRom[context.workingOffset++] = 0x04;
            // (removed code) STZ $E02C,x
            outRom[context.workingOffset++] = 0x9E;
            outRom[context.workingOffset++] = 0x2C;
            outRom[context.workingOffset++] = 0xE0;
            // SEP 20, since we REP 20'ed earlier
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // RTL - vanilla behavior if screen position isn't out of bounds
            outRom[context.workingOffset++] = 0x6B;


            // fixWindowPosition:
            // just mark the camera as moving, and every frame we shift toward the target (whichever of 7eex2c is == 1) if it's == 1
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #01
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;
            // STA FREE_MOVE_PLAYER_SELECT_BYTE
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE >> 16);
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // (removed code) LDX $04
            outRom[context.workingOffset++] = 0xA6;
            outRom[context.workingOffset++] = 0x04;
            // (removed code) STZ $E02C,x
            outRom[context.workingOffset++] = 0x9E;
            outRom[context.workingOffset++] = 0x2C;
            outRom[context.workingOffset++] = 0xE0;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

           
            /*
             * replace:
               $00/BD3D BD 00 E0    LDA $E000,x[$7E:E400]   A:7002 X:0400 Y:0000 P:envMxdIzC
               $00/BD40 F0 4D       BEQ $4D[$BD8F]          A:7001 X:0400 Y:0000 P:envMxdIzC
               $00/BD42 30 4B       BMI $4B[$BD8F]          A:7001 X:0400 Y:0000 P:envMxdIzC
             */
            outRom[0xBD3D] = 0x22;
            outRom[0xBD3E] = (byte)(context.workingOffset);
            outRom[0xBD3F] = (byte)(context.workingOffset >> 8);
            outRom[0xBD40] = (byte)((context.workingOffset >> 16) + 0xC0);
            // BNE $BD8F
            outRom[0xBD41] = 0xD0;
            outRom[0xBD42] = 0x4C;
            // NOP
            outRom[0xBD43] = 0xEA;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA $7ECF1E
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE >> 16);
            // LDA $E000,x[$7E:E000]
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xE0;
            // BEQ error
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x1A;
            // BMI error
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x18;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $E020,x[$7E:E020]
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0xE0;
            // BMI error
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x11;
            // CMP #00E0
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            // BCS error
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x0C;
            // LDA $E022,x[$7E:E022]
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0xE0;
            // BMI error
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x07;
            // CMP #00D0
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x00;
            // BCS error
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x02; // error
            // BRA end
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x08; // end
            // error:
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #01
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;
            // STA FREE_MOVE_PLAYER_SELECT_BYTE
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE >> 16);
            // end:
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA FREE_MOVE_PLAYER_SELECT_BYTE
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER_SELECT_BYTE >> 16);
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // //////////////////////////////////////////////////////////
            // Teleport characters who have been off screen for a while
            // //////////////////////////////////////////////////////////

            /*
             * replace:
                 $00/E4C3 BD 84 E0    LDA $E084,x[$7E:E684]   A:0200 X:0600 Y:0006 P:envMxdIZC
                 $00/E4C6 29 0F       AND #$0F                A:0200 X:0600 Y:0006 P:envMxdIZC
                 $00/E4C8 85 02       STA $02[$00:0002]       A:0200 X:0600 Y:0006 P:envMxdIZC
                 this goes off for enemies too, so we have to check object index
            */
            outRom[0xE4C3] = 0x22;
            outRom[0xE4C4] = (byte)(context.workingOffset);
            outRom[0xE4C5] = (byte)(context.workingOffset >> 8);
            outRom[0xE4C6] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0xE4C7] = 0xEA;
            outRom[0xE4C8] = 0xEA;
            outRom[0xE4C9] = 0xEA;
            // (A 8bit, XY 16bit, X is object index and should be preserved, A/Y can be corrupted)
            // CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BCC over - player character
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x08;
            // (removed code) LDA $E084,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x84;
            outRom[context.workingOffset++] = 0xE0;
            // (removed code) AND #$0F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // (removed code) STA $02
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x02;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // over: 
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // TXA
            outRom[context.workingOffset++] = 0x8A;
            // XBA -- A should be 0000, 0002, or 0004 for boy/girl/sprite
            outRom[context.workingOffset++] = 0xEB;
            // TAY -- now it's in Y.
            outRom[context.workingOffset++] = 0xA8;
            // LDA 7EE020, X -- load screen x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // BMI error
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x20;
            // CMP #00E0
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            // BCS error
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x1B;
            // LDA 7EE022, X -- load screen y
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // BMI error
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0x15;
            // CMP #00D0
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x00;
            // BCS error
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x10;
            // if we made it here, the player is on screen, so don't teleport them, and if the timer is active, cancel it
            // LDA #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // 99 20 CF   STA FREE_MOVE_PLAYER1_COUNTER_16BIT,Y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER1_COUNTER_16BIT);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER1_COUNTER_16BIT >> 8);
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // (removed code) LDA $E084,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x84;
            outRom[context.workingOffset++] = 0xE0;
            // (removed code) AND #$0F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // (removed code) STA $02
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x02;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // error:
            // if we made it here, the character is off screen.
            // increment the offscreen counter; if it's been long enough, find p1 and warp them to it (and reset the counter)
            // B9 20 CF    LDA CF20, Y
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER1_COUNTER_16BIT);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER1_COUNTER_16BIT >> 8);
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // CMP #teleportTimerInFrames
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = (byte)teleportTimerInFrames;
            outRom[context.workingOffset++] = (byte)(teleportTimerInFrames>>8);
            // BCS warpPlayer -- branch if greater/equal
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x0D;
            // if we got here, the character is off screen, but it's not time to teleport them yet.  store the counter back and wait.
            // STA FREE_MOVE_PLAYER1_COUNTER_16BIT,Y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER1_COUNTER_16BIT);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER1_COUNTER_16BIT >> 8);
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // (removed code) LDA $E084,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x84;
            outRom[context.workingOffset++] = 0xE0;
            // (removed code) AND #$0F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // (removed code) STA $02
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x02;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // warpPlayer:
            // if we got here, the character's been off screen for a while; teleport them back to p1 and reset the counter
            // LDA #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // STA FREE_MOVE_PLAYER1_COUNTER_16BIT,Y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER1_COUNTER_16BIT);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.FREE_MOVE_PLAYER1_COUNTER_16BIT >> 8);
            // LDA 7ee02c - player control number
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x2C;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // AND 00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // CMP #0001 - is the boy player 1?
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BNE tryGirlChar - nope, check the girl
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x22;
            // if we got here, warp to boy character since he's p1, then return
            // LDA 7ee002 - x position
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7ee002,x - x position
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // LDA 7ee004 - y position
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7ee004,x - y position
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7ee00B - z position
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7ee00B,x - z position
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // (removed code) LDA $E084,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x84;
            outRom[context.workingOffset++] = 0xE0;
            // (removed code) AND #$0F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // (removed code) STA $02
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x02;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // tryGirlChar:
            // basically the same as the above but we're checking whether the girl is p1, and teleporting to her if so.
            // LDA 7ee22c - girl player num
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x2C;
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x7E;
            // AND 00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // CMP #0001
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BNE trySpriteChar - nope, check the sprite
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x22;
            // if we got here, warp to girl character since she's p1, then return
            // LDA 7ee202 - x position
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7ee002,x - x position
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // LDA 7ee204 - y position
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7ee004, x - y position
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7ee20B - z position
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7ee00B,x - z position
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // (replaced code) LDA $E084,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x84;
            outRom[context.workingOffset++] = 0xE0;
            // (replaced code) AND #$0F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // (replaced code) STA $02
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x02;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // trySpriteChar:
            // basically the same as the above but at this point we have to assume the sprite is p1 since no one else is, so we don't bother to check
            // LDA 7ee402 - x position
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xE4;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7ee002,x - x position
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // LDA 7ee404 - y position
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0xE4;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7ee004,x - y position
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7ee40B - z position
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0xE4;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7ee00B,x - z position
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x0B;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // (replaced code) LDA $E084,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x84;
            outRom[context.workingOffset++] = 0xE0;
            // (replaced code) AND #$0F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // (replaced code) STA $02
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x02;
            // (replaced code) RTL
            outRom[context.workingOffset++] = 0x6B;


            // ///////////////////////////////////
            // fix menu positioning for offscreen characters
            // ///////////////////////////////////

            // note that if menus show up close enough to the edge of the screen, sometimes they will spin forever
            // and softlock the game.  definitely don't want this!

            /*
             * replace:
                 $C0/713C 8D 1A 18    STA $181A  [$00:181A]   A:0078 X:0200 Y:FFEB P:envmxdIzC
                 $C0/713F 5A          PHY                     A:0078 X:0200 Y:FFEB P:envmxdIzC
                 $C0/7140 BF 22 E0 7E LDA $7EE022,x[$7E:E222] A:0078 X:0200 Y:FFEB P:envmxdIzC
                 $C0/7144 18          CLC                     A:0064 X:0200 Y:FFEB P:envmxdIzC
                 $C0/7145 63 01       ADC $01,s[$00:01E4]     A:0064 X:0200 Y:FFEB P:envmxdIzc
                 $C0/7147 8D 1C 18    STA $181C  [$00:181C]   A:004F X:0200 Y:FFEB P:envmxdIzC
                 $C0/714A 7A PLY                              A:004F X:0200 Y:FFEB P:envmxdIzC
            */
            outRom[0x713C] = 0x22;
            outRom[0x713D] = (byte)(context.workingOffset);
            outRom[0x713E] = (byte)(context.workingOffset >> 8);
            outRom[0x713F] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x7140] = 0xEA;
            outRom[0x7141] = 0xEA;
            outRom[0x7142] = 0xEA;
            outRom[0x7143] = 0xEA;
            outRom[0x7144] = 0xEA;
            outRom[0x7145] = 0xEA;
            outRom[0x7146] = 0xEA;
            outRom[0x7147] = 0xEA;
            outRom[0x7148] = 0xEA;
            outRom[0x7149] = 0xEA;
            outRom[0x714A] = 0xEA;

            byte xMin = 0x30;
            byte xMax = 0xC0;
            byte yMin = 0x30;
            byte yMax = 0xA8;

            // BPL 05
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x05;
            // LDA #0020 - negative replacement
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = xMin;
            outRom[context.workingOffset++] = 0x00;
            // BRA yCheck
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x12;
            // CMP #00C0 - right screen bounds
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = xMax;
            outRom[context.workingOffset++] = 0x00;
            // BCC/BLT skip1
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x05;
            // LDA #00C0 - set position
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = xMax;
            outRom[context.workingOffset++] = 0x00;
            // BRA yCheck
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x08;
            // skip1:
            // CMP #0020 - left screen bounds
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = xMin;
            outRom[context.workingOffset++] = 0x00;
            // BCS/BGE skip2
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x03;
            // LDA #0020
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = xMin;
            outRom[context.workingOffset++] = 0x00;
            // skip2:
            // yCheck:
            // STA $181A - write x
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x1A;
            outRom[context.workingOffset++] = 0x18;
            // PHY
            outRom[context.workingOffset++] = 0x5A;
            // LDA $7EE022,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC $01,s
            outRom[context.workingOffset++] = 0x63;
            outRom[context.workingOffset++] = 0x01;
            // BPL 05
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x05;
            // LDA #0020 - negative replacement
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = yMin;
            outRom[context.workingOffset++] = 0x00;
            // BRA yCheck
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x12;
            // CMP #00C0 - bottom screen bounds
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = yMax;
            outRom[context.workingOffset++] = 0x00;
            // BCC/BLT skip1
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x05;
            // LDA #00C0 - set position
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = yMax;
            outRom[context.workingOffset++] = 0x00;
            // BRA end
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x08;
            // skip1:
            // CMP #0020 - top screen bounds
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = yMin;
            outRom[context.workingOffset++] = 0x00;
            // BCS/BGE skip2
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x03;
            // LDA #0020
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = yMin;
            outRom[context.workingOffset++] = 0x00;
            // end:
            // STA $181C
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x1C;
            outRom[context.workingOffset++] = 0x18;
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
