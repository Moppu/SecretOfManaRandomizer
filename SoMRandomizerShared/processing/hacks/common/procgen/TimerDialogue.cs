using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// Add event command 0x57 xx to print the current value of the timer.
    /// Also add 16-bit and 32-bit value printing commands in support of the Statistics hack.
    /// Used in the dialogue after you beat the mana beast.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class TimerDialogue : RandoProcessor
    {
        // names of commands to register with the CustomEventManager
        public const string EVENT_COMMAND_NAME_PRINT_TIMER = "print timer";
        public const string EVENT_COMMAND_NAME_PRINT_16_BIT_VALUE = "print 16 bit value";
        public const string EVENT_COMMAND_NAME_PRINT_32_BIT_VALUE = "print 32 bit value";

        protected override string getName()
        {
            return "Custom text printing events for non-vanilla modes";
        }

        //public void process(byte[] outRom, ref int context.workingOffset, Layer3Changes layer3, CustomEventManager customEventManager)
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // note this hack currently has a dependency on Layer3Changes because it uses this subroutine
            int div10SubrOffset = context.workingData.getInt(Layer3Changes.DIV10_SUBROUTINE_OFFSET);
            int print16BitSubrOffset = context.workingOffset;
            print16BitValueToDialogue(outRom, context, div10SubrOffset);
            int print32BitSubrOffset = context.workingOffset;
            print32BitValueToDialogue(outRom, context, div10SubrOffset);
            
            int printTimerSubrOffset = context.workingOffset;
            // print timer value event command
            context.eventHackMgr.registerNewEvent(EVENT_COMMAND_NAME_PRINT_TIMER, printTimerSubrOffset);
            // print 16 bit value event command
            context.eventHackMgr.registerNewEvent(EVENT_COMMAND_NAME_PRINT_16_BIT_VALUE, print16BitSubrOffset);
            // print 32 bit value event command
            context.eventHackMgr.registerNewEvent(EVENT_COMMAND_NAME_PRINT_32_BIT_VALUE, print32BitSubrOffset);

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // stz 4207 (why?)
            outRom[context.workingOffset++] = 0x9C;
            outRom[context.workingOffset++] = 0x07;
            outRom[context.workingOffset++] = 0x42;
            // 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // lda #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // tay
            outRom[context.workingOffset++] = 0xA8;
            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // lda hours
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;
            // tay (for subroutine)
            outRom[context.workingOffset++] = 0xA8;
            // call div10 subroutine
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);
            // result = mod result - low digit
            // clc
            outRom[context.workingOffset++] = 0x18;
            // adc #B5
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;
            // pha
            outRom[context.workingOffset++] = 0x48;
            // LDA 4214 - divide result - high digit
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #B5
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;
            // STA 7EA22F - this is the dialogue thing i guess - first digit
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x2F;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // PLA - second digit
            outRom[context.workingOffset++] = 0x68;
            // STA 7EA230 - second digit
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // LDA #C5 - this is a colon character i think
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xC5;
            // STA 7EA231 - third character
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x31;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            // ////////////////////////////////////////////////////////
            // AF 19 CF 7E  LDA $7ECF19 minutes
            // .. code essentially the same as above for hours
            // ////////////////////////////////////////////////////////
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x19;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            outRom[context.workingOffset++] = 0xA8;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);

            outRom[context.workingOffset++] = 0x18;

            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;

            outRom[context.workingOffset++] = 0x48;

            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;

            outRom[context.workingOffset++] = 0x18;

            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;

            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x32;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            outRom[context.workingOffset++] = 0x68;

            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x33;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xC5;

            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x34;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            // ////////////////////////////////////////////////////////
            // AF 1A CF 7E  LDA $7ECF1A seconds
            // .. code essentially the same as above for hours
            // ////////////////////////////////////////////////////////
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x1A;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            outRom[context.workingOffset++] = 0xA8;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);

            outRom[context.workingOffset++] = 0x18;

            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;

            outRom[context.workingOffset++] = 0x48;

            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;

            outRom[context.workingOffset++] = 0x18;

            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;

            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x35;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            outRom[context.workingOffset++] = 0x68;

            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x36;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xBF;

            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x37;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            // ////////////////////////////////////////////////////////
            // AF 1B CF 7E  LDA $7ECF1B frames
            // .. roughly the same as above except we convert to 0-99, similarly to in Layer3Changes
            // ////////////////////////////////////////////////////////
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x1B;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            // ---
            // conversion from 0-60 to 0-100
            // shift right once, add (90)
            // shift right twice more, add (97.5ish)
            // close enough.
            // PHA
            outRom[context.workingOffset++] = 0x48;

            // LSR
            outRom[context.workingOffset++] = 0x4A;

            // 8F 1C CF 7E    STA 7ECF1C
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x1C;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            // LSR
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            outRom[context.workingOffset++] = 0x4A;

            // 8F 1D CF 7E    STA 7ECF1D
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x1D;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            // PLA
            outRom[context.workingOffset++] = 0x68;

            // ADC 7ECF1D
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = 0x1C;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            // ADC 7ECF1C
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = 0x1D;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x7E;

            // ---

            // now the same code as above for printing to dialogue.
            outRom[context.workingOffset++] = 0xA8;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);

            outRom[context.workingOffset++] = 0x18;

            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;

            outRom[context.workingOffset++] = 0x48;

            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;

            outRom[context.workingOffset++] = 0x18;

            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;

            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x38;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            outRom[context.workingOffset++] = 0x68;

            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x39;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;

            // zero at 7EA23A to end the string
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x3A;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            // now just restore all the shit and return

            // LDA #7E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x7E;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PLB
            outRom[context.workingOffset++] = 0xAB;

            outRom[context.workingOffset++] = 0x6B;
            
            return true;
        }

        private void print16BitValueToDialogue(byte[] outRom, RandoContext context, int div10SubrOffset)
        {
            // pull two event byte params
            // use these to index 7E to get the value to print
            // 10000 -> 7EA22F
            //  1000 -> 7EA230
            //   100 -> 7EA231
            //    10 -> 7EA232
            //     1 -> 7EA233
            // shift back if zero in any high digit

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;

            // STA 7EA234
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x34;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            // LDA #80
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x80;
            // STA 7EA22F
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x2F;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA230
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA231
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x31;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA232
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x32;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA233
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x33;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;


            // basically just divide by 10 until we get zero, then stop.

            // PHX
            outRom[context.workingOffset++] = 0xDA;

            // 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // LDA 7EA1D1
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xD1;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0x7E;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // STA 7EA1D1
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0xD1;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0x7E;
            // LDA C90000,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xC9;

            // TAX
            outRom[context.workingOffset++] = 0xAA;

            // BF 00 00 7E LDA 7E0000,x ; this is the value we print
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            // PHA ; store it off for now
            outRom[context.workingOffset++] = 0x48;

            // LDX #A233 ; target position
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x33;
            outRom[context.workingOffset++] = 0xA2;

            // loop start:
            int loopOffset = context.workingOffset;

            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // stz 4207 (why?)
            outRom[context.workingOffset++] = 0x9C;
            outRom[context.workingOffset++] = 0x07;
            outRom[context.workingOffset++] = 0x42;
            // 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // lda #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // tay
            outRom[context.workingOffset++] = 0xA8;
            // pla with the value from earlier that we want to print
            outRom[context.workingOffset++] = 0x68;
            // tay (for subroutine)
            outRom[context.workingOffset++] = 0xA8;

            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // call div10 subroutine
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);
            // mod result in a; divide result in 4214
            // if it's 0, we're done
            // otherwise, dex and go again

            // clc
            outRom[context.workingOffset++] = 0x18;
            // adc #B5
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;
            // STA 7E0000,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            // dex
            outRom[context.workingOffset++] = 0xCA;

            // cpx #A22E
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x2E;
            outRom[context.workingOffset++] = 0xA2;

            // beq out
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x0A;

            // 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // LDA 4214 - divide result
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;

            // beq out
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x03;

            // PHA to push the new, div10'ed value and use that next time
            outRom[context.workingOffset++] = 0x48;

            int loopSrcOffset = context.workingOffset + 2;
            // bra back (or jmp)
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = (byte)(loopOffset- loopSrcOffset);

            // out:

            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // restore stuff at the end
            // PLX
            outRom[context.workingOffset++] = 0xFA;

            outRom[context.workingOffset++] = 0x6B;

        }

        private void print32BitValueToDialogue(byte[] outRom, RandoContext context, int div10SubrOffset)
        {
            // pull two event byte params
            // use these to index 7E to get the value to print
            // 10000000 -> 7EA22F
            //  1000000 -> 7EA230
            //   100000 -> 7EA231
            //    10000 -> 7EA232
            //     1000 -> 7EA233
            //      100 -> 7EA234
            //       10 -> 7EA235
            //        1 -> 7EA236
            // shift back if zero in any high digit

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;

            // STA 7EA234
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x37;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            // LDA #80
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x80;
            // STA 7EA22F
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x2F;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA230
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA231
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x31;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA232
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x32;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA233
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x33;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA234
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x34;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA235
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x35;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;
            // STA 7EA236
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x36;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            // basically just divide by 10 until we get zero, then stop.

            // PHX
            outRom[context.workingOffset++] = 0xDA;

            // 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // LDA 7EA1D1
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xD1;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0x7E;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // STA 7EA1D1
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0xD1;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0x7E;
            // LDA C90000,X
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xC9;

            // PHA ; save the address off 
            outRom[context.workingOffset++] = 0x48;

            // TAX
            outRom[context.workingOffset++] = 0xAA;

            // BF 00 00 7E LDA 7E0000,x ; this is the value we print
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            // PHA ; store it off for now
            outRom[context.workingOffset++] = 0x48;

            // LDX #A236 ; target position
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x36;
            outRom[context.workingOffset++] = 0xA2;

            // loop start:
            int loopOffset = context.workingOffset;

            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // stz 4207 (why?)
            outRom[context.workingOffset++] = 0x9C;
            outRom[context.workingOffset++] = 0x07;
            outRom[context.workingOffset++] = 0x42;
            // 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // lda #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // tay
            outRom[context.workingOffset++] = 0xA8;
            // pla with the value from earlier that we want to print
            outRom[context.workingOffset++] = 0x68;
            // tay (for subroutine)
            outRom[context.workingOffset++] = 0xA8;

            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // call div10 subroutine
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);
            // mod result in a; divide result in 4214
            // if it's 0, we're done
            // otherwise, dex and go again

            // clc
            outRom[context.workingOffset++] = 0x18;
            // adc #B5
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;
            // STA 7E0000,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            // 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // dex
            outRom[context.workingOffset++] = 0xCA;

            // cpx #A232
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x32;
            outRom[context.workingOffset++] = 0xA2;

            // beq nextByte
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x0D;

            // LDA 4214 - divide result
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;

            // bne in
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;

            // PLA - take address off the stack
            outRom[context.workingOffset++] = 0x68;
            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // in:

            // PHA to push the new, div10'ed value and use that next time
            outRom[context.workingOffset++] = 0x48;

            int loopSrcOffset = context.workingOffset + 2;
            // bra back (or jmp)
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = (byte)(loopOffset - loopSrcOffset);

            // nextByte:

            // PLA - address saved off from beginning
            outRom[context.workingOffset++] = 0x68;
            // INC A ; move on
            outRom[context.workingOffset++] = 0x1A;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // TAX
            outRom[context.workingOffset++] = 0xAA;

            // basically the same shit from above

            // BF 00 00 7E LDA 7E0000,x ; this is the value we print
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // zero in whole second value? skip this
            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x04;
            // 8bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // over:

            // PHA ; store it off for now
            outRom[context.workingOffset++] = 0x48;

            // LDX #A232 ; target position
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x32;
            outRom[context.workingOffset++] = 0xA2;

            // loop start:
            loopOffset = context.workingOffset;

            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // stz 4207 (why?)
            outRom[context.workingOffset++] = 0x9C;
            outRom[context.workingOffset++] = 0x07;
            outRom[context.workingOffset++] = 0x42;
            // 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // lda #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // tay
            outRom[context.workingOffset++] = 0xA8;
            // pla with the value from earlier that we want to print
            outRom[context.workingOffset++] = 0x68;
            // tay (for subroutine)
            outRom[context.workingOffset++] = 0xA8;

            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // call div10 subroutine
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)div10SubrOffset;
            outRom[context.workingOffset++] = (byte)(div10SubrOffset >> 8);
            outRom[context.workingOffset++] = (byte)((div10SubrOffset >> 16) + 0xC0);
            // mod result in a; divide result in 4214
            // if it's 0, we're done
            // otherwise, dex and go again

            // clc
            outRom[context.workingOffset++] = 0x18;
            // adc #B5
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;
            // STA 7E0000,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            // 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // LDA 4214 - divide result
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;

            // bne in
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x04;

            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            outRom[context.workingOffset++] = 0x6B;

            // in:

            // dex
            outRom[context.workingOffset++] = 0xCA;

            // cpx #A22E
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x2E;
            outRom[context.workingOffset++] = 0xA2;

            // beq out
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x03;


            // PHA to push the new, div10'ed value and use that next time
            outRom[context.workingOffset++] = 0x48;

            loopSrcOffset = context.workingOffset + 2;
            // bra back (or jmp)
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = (byte)(loopOffset - loopSrcOffset);

            // out:

            // 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // restore stuff at the end
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
        }

    }
}
