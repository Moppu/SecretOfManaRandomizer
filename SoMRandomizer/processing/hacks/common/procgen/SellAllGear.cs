using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// Procgen hack that adds a neko command to sell all the gear you aren't wearing, as a time-saver.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SellAllGear : RandoProcessor
    {
        // name of command to register with the CustomEventManager
        public const string EVENT_COMMAND_NAME_SELL_ALL = "sell all";

        protected override string getName()
        {
            return "Custom event to sell all unused gear in non-vanilla modes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // in ram:
            // 7ecc24 hats, two msbs indicate equipped char
            // 7ecc30 armors
            // 7ecc3c accessories
            
            // note this doesn't replace any vanilla code directly, but just establishes a custom event subroutine that can be called from nekos and such

            int mulTwoThirdsSubr = context.workingOffset;
            makeMulTwoThirdsSubr(outRom, context);

            int subrStart = context.workingOffset;
            // 57 06 -> sell all unequipped shit
            context.eventHackMgr.registerNewEvent(EVENT_COMMAND_NAME_SELL_ALL, subrStart);

            // PHP            ; save off all the shit including status
            outRom[context.workingOffset++] = 0x08;
            // rep 30         ; make sure we save full values
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x30;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // PHY
            outRom[context.workingOffset++] = 0x5A;

            // for each type of gear
            for (int i = 0; i < 3; i++)
            {
                int itemOffset1 = 0x7ECC24 + i * 12;
                int itemOffset2 = itemOffset1 + 1;
                int itemOffset3 = itemOffset2 + 1;
                //
                // SEP 20         ; 8-bit A for hat indeces
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;

                // LDX #0000      ; hat index
                outRom[context.workingOffset++] = 0xA2;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x00;
                int jumpDest_loop1 = context.workingOffset;
                // loop1:         ; hat loop
                // LDA 7ECC24,x   ; load hat #x
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = (byte)itemOffset1;
                outRom[context.workingOffset++] = (byte)(itemOffset1 >> 8);
                outRom[context.workingOffset++] = (byte)(itemOffset1 >> 16);
                // BIT #C0        ; check 0xC0 bits that indicate who's using it
                outRom[context.workingOffset++] = 0x89;
                outRom[context.workingOffset++] = 0xC0;
                // BEQ sell       ; sell item if 0xC0 bits clear
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x02;
                // BRA endloop1   ; skip the selling if someone's wearing it - note that empties are 0xFF and this will also skip those.
                int jumpSource_endloop = context.workingOffset;
                outRom[context.workingOffset++] = 0x80;
                outRom[context.workingOffset++] = 0x00;
                // sell:          ; sell if no one using it
                // DEX            ; decrement x, so we check the same spot next time after we shift things
                outRom[context.workingOffset++] = 0xCA;
                // PHX            ; save off our loop index
                outRom[context.workingOffset++] = 0xDA;
                // REP 20         ; allow 16-bit gold value in A
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // ASL            ; multiply by 2 since buy prices are 16bit
                outRom[context.workingOffset++] = 0x0A;
                // AND 00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
                // TAX            ; use as index
                outRom[context.workingOffset++] = 0xAA;
                // LDA D8FBB4,x   ; load buy price
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = 0xB4;
                outRom[context.workingOffset++] = 0xFB;
                outRom[context.workingOffset++] = 0xD8;
                // multiply by 2/3 for sell price
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = (byte)mulTwoThirdsSubr;
                outRom[context.workingOffset++] = (byte)(mulTwoThirdsSubr >> 8);
                outRom[context.workingOffset++] = (byte)((mulTwoThirdsSubr >> 16) + 0xC0);

                //                ; add to current gold - 7ecc6a
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC $7ECC6A    ; lsb and middle byte of gold
                outRom[context.workingOffset++] = 0x6F;
                outRom[context.workingOffset++] = 0x6A;
                outRom[context.workingOffset++] = 0xCC;
                outRom[context.workingOffset++] = 0x7E;
                // STA $7ECC6A
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x6A;
                outRom[context.workingOffset++] = 0xCC;
                outRom[context.workingOffset++] = 0x7E;
                // SEP 20         ; 8 bit A for msb
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA #00
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x00;
                // ADC $7ECC6C    ; 24-bit msb of gold w/ carry from prev addition
                outRom[context.workingOffset++] = 0x6F;
                outRom[context.workingOffset++] = 0x6C;
                outRom[context.workingOffset++] = 0xCC;
                outRom[context.workingOffset++] = 0x7E;
                // STA $7ECC6C
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x6C;
                outRom[context.workingOffset++] = 0xCC;
                outRom[context.workingOffset++] = 0x7E;
                // PLX            ; restore our loop index
                outRom[context.workingOffset++] = 0xFA;
                // PHX
                outRom[context.workingOffset++] = 0xDA;
                // remove the item, and shift the others back one slot
                // startloop:
                int jumpDest_startloop = context.workingOffset;
                // CPX #000A (B-1 from dex)
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = 0x0A;
                outRom[context.workingOffset++] = 0x00;
                // BNE over       ; check other values
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x08;
                // LDA #FF        ; last item, load FF
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0xFF;
                // STA 7ECC24,x
                outRom[context.workingOffset++] = 0x9F;
                outRom[context.workingOffset++] = (byte)(itemOffset1);
                outRom[context.workingOffset++] = (byte)(itemOffset1 >> 8);
                outRom[context.workingOffset++] = (byte)(itemOffset1 >> 16);
                // BRA out
                int jumpSource_out1 = context.workingOffset;
                outRom[context.workingOffset++] = 0x80;
                outRom[context.workingOffset++] = 0x00;
                // over:          
                // CPX #000B        ; break out if was 0x0C
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = 0x0B;
                outRom[context.workingOffset++] = 0x00;
                // BEQ out
                int jumpSource_out2 = context.workingOffset;
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x00;
                // LDA 7ECC25,x   ; default - load next value and shove it in this value

                // INX
                outRom[context.workingOffset++] = 0xE8;

                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = (byte)(itemOffset2);
                outRom[context.workingOffset++] = (byte)(itemOffset2 >> 8);
                outRom[context.workingOffset++] = (byte)(itemOffset2 >> 16);
                // STA 7ECC24,x
                outRom[context.workingOffset++] = 0x9F;
                outRom[context.workingOffset++] = (byte)(itemOffset1);
                outRom[context.workingOffset++] = (byte)(itemOffset1 >> 8);
                outRom[context.workingOffset++] = (byte)(itemOffset1 >> 16);
                // BRA startloop
                int jumpSource_startLoop = context.workingOffset;
                outRom[context.workingOffset++] = 0x80;
                outRom[context.workingOffset++] = 0x00;
                // out:
                int jumpDest_out = context.workingOffset;
                // PLX
                outRom[context.workingOffset++] = 0xFA;
                // endloop1:      ; increment index and reloop, executed regardless of logic
                int jumpDest_endloop1 = context.workingOffset;
                // INX
                outRom[context.workingOffset++] = 0xE8;
                // CPX #000C
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = 0x0C;
                outRom[context.workingOffset++] = 0x00;
                // BNE loop1
                int jumpSource_loop1 = context.workingOffset;
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x00;
                //
                // down here: the same but with 7ECC30, 7ECC3C for armors & accessories
                makeJump(outRom, jumpSource_endloop, jumpDest_endloop1);
                makeJump(outRom, jumpSource_out1, jumpDest_out);
                makeJump(outRom, jumpSource_out2, jumpDest_out);
                makeJump(outRom, jumpSource_startLoop, jumpDest_startloop);
                makeJump(outRom, jumpSource_loop1, jumpDest_loop1);
            }

            // end:
            // rep 30         ; restore full values, same size we pushed above
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x30;
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // PLP
            outRom[context.workingOffset++] = 0x28;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }

        private void makeJump(byte[] outRom, int source, int dest)
        {
            // insert at source+1, dest - source
            outRom[source + 1] = (byte)(dest - source - 2);
        }

        private void makeMulTwoThirdsSubr(byte[] outRom, RandoContext context)
        {
            // STA $4204
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #03
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x03;
            // STA $4206
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x06;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // eight nops
            for (int i=0; i < 8; i++)
            {
                outRom[context.workingOffset++] = 0xEA;
            }

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $4214
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // AND 00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
        }
    }
}
