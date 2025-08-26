using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Hack that replaces "Okay?" with "How many? [x]" for buying consumables.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BuyMultipleItems : RandoProcessor
    {
        protected override string getName()
        {
            return "Buy multiple consumables at once";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_BUY_MULTIPLE_CONSUMABLES))
            {
                return false;
            }

            byte maxItems = 4;
            if(settings.getBool(CommonSettings.PROPERTYNAME_MAX_7_ITEMS))
            {
                maxItems = 7;
            }

            context.initialValues[CustomRamOffsets.CONSUMABLE_BUY_MAX] = 0;
            context.initialValues[CustomRamOffsets.CONSUMABLE_BUY_CURRENT] = 0;
            context.initialValues[CustomRamOffsets.CONSUMABLE_INPUT_TIMER] = 0;
            context.initialValues[CustomRamOffsets.BUYING_MULTIPLE_ITEMS] = 0;

           
            // ///////////////////////////////////////////////////////////////////
            // logic for initialization handling
            // first, the replaced code:
            // 0A ASL
            // AA TAX
            // C2 30 REP 30
            // now, if x == 0 we're in the buy menu, so do:
            //     CONSUMABLE_BUY_MAX = 0
            //     CONSUMABLE_BUY_CURRENT = 0
            //     CONSUMABLE_INPUT_TIMER = 0
            //     look up the item value at D8[$183C] + $180A
            //     if it's >= 0xBA, then it's a consumable, and we do:
            //        check how many we currently have of this; CONSUMABLE_BUY_MAX (new 8bit thing in RamOffsets) = 7 - that
            //            CONSUMABLE_BUY_CURRENT = CONSUMABLE_BUY_MAX
            //        check how many we can afford given our current gold.  if it's < CONSUMABLE_BUY_MAX, then CONSUMABLE_BUY_MAX = that
            //            CONSUMABLE_BUY_CURRENT = CONSUMABLE_BUY_MAX
            // ///////////////////////////////////////////////////////////////////

            /*
             * replace code run when you select something in a menu:
                $C0/7AC3 3A          DEC A                   A:1802 X:0070 Y:0004 P:envMXdIzC              **
                $C0/7AC4 0A          ASL A                   A:1801 X:0070 Y:0004 P:envMXdIzC              **
                $C0/7AC5 AA          TAX                     A:1802 X:0070 Y:0004 P:envMXdIzc              **
                $C0/7AC6 C2 30       REP #$30                A:1802 X:0002 Y:0004 P:envMXdIzc              **
                $C0/7AC8 7C CB 7A    JMP ($7ACB,x)[$C0:7B17] A:1802 X:0002 Y:0004 P:envmxdIzc (vanilla handler; keep)
             */
            outRom[0x7AC3] = 0x22;
            outRom[0x7AC4] = (byte)(context.workingOffset);
            outRom[0x7AC5] = (byte)(context.workingOffset >> 8);
            outRom[0x7AC6] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x7AC7] = 0xEA;

            outRom[context.workingOffset++] = 0x3A; // replaced code
            outRom[context.workingOffset++] = 0x0A; // replaced code
            outRom[context.workingOffset++] = 0xAA; // replaced code
            outRom[context.workingOffset++] = 0xC2; // replaced code
            outRom[context.workingOffset++] = 0x30; // replaced code

            // CPX #0000 - this will be 0 for buy, 2 for sell
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;

            // BEQ over 
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x01;

            // RTL if not buy menu
            outRom[context.workingOffset++] = 0x6B;

            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // PHY
            outRom[context.workingOffset++] = 0x5A;
            // PHA
            outRom[context.workingOffset++] = 0x48;

            // SEP #20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;

            // STA CONSUMABLE_BUY_MAX
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_MAX;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 16);

            // STA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);

            // STA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // LDA 7E183C
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x3C;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x7E;

            // CLC
            outRom[context.workingOffset++] = 0x18;

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // ADC 7E180A
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x7E;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // TAX
            outRom[context.workingOffset++] = 0xAA;

            // SEP #20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA $D80000,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xD8;

            // CMP #BA (candy ID - first consumable)
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xBA;

            // BCS/BGE over - keep processing if we selected a consumable
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x06;

            // otherwise, selected an armor or something - even out the stack and return
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // here: we continue because it's a consumable
            // so .. these are at 7ECC48 .. the amount is the 3 MSBs, the id is the lower 5; 0xFF means end list

            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #BA - candy id; first consumable
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0xBA;
            // PHA - store the item id we're trying to buy
            outRom[context.workingOffset++] = 0x48;
            // LDA #07 (or 4)
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = maxItems;
            // STA CONSUMABLE_BUY_MAX
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_MAX;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 16);
            // LDX #FFFF
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0xFF;
            // backToCheck:
            // INX - to zero initially, +1 further iters
            outRom[context.workingOffset++] = 0xE8;
            // LDA 7ECC48,x - check inventory
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x48;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            // CMP #FF - if it's FF, we're at the end of the inventory
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xFF;
            // BEQ doneCheckingAmounts
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x13 + 7;
            // AND #1F - extract item id from full value, which contains the count also
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x1F;
            // CMP $01,s - compare against the item id we pushed above
            outRom[context.workingOffset++] = 0xC3;
            outRom[context.workingOffset++] = 0x01;
            // BNE backToCheck - not equal; branch back and try the next item
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xF1;
            // LDA 7ECC48,x - reload the item with count
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x48;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            // LSR - extract count by shifting the id off
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // subtract from 7 (or 4)
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA #07 (or 4)
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = maxItems;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC $01,s
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x01;

            // STA CONSUMABLE_BUY_MAX - the most of this item we can buy, given the number we currently have
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_MAX;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 16);

            // PLA - even out the stack
            outRom[context.workingOffset++] = 0x68;

            // doneCheckingAmounts:
            // PLA - even out the stack for the comparison value we pushed earlier
            outRom[context.workingOffset++] = 0x68;

            // now.. check gold (7ecc6a, 24 bit)
            // D8:FB9C 16 bit value item costs
            // 00: Candy = 000A
            // 01: Chocolate = 001E
            // 02: Royal Jam = 0064
            // 03: Faerie Walnut = 01F4
            // 04: Medical Herb = 000A
            // 05: Cup of Wishes = 0096
            // * 06: Magic Rope = FFFF
            // * 07: Flammie Drum = FFFF
            // * 08: Moogle Belt = FFFF
            // * 09: Midge Mallet = FFFF
            // 0A: Barrel = 01C2
            // * 0B: ? (Dummied Out) = FFFF

            // so..
            // ASL - shift item id << 1
            outRom[context.workingOffset++] = 0x0A;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA D8:FB9C,x - load the item's price
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x9C;
            outRom[context.workingOffset++] = 0xFB;
            outRom[context.workingOffset++] = 0xD8;
            // PHA - push cost of what we're buying
            outRom[context.workingOffset++] = 0x48;
            // -------------------------
            // check for neko
            // -------------------------
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #99 - neko sprite id
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x99;
            // CMP $185B - sprite we talked to
            outRom[context.workingOffset++] = 0xCD;
            outRom[context.workingOffset++] = 0x5B;
            outRom[context.workingOffset++] = 0x18;
            // BNE skipNekoMultiply
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLA - pull cost
            outRom[context.workingOffset++] = 0x68;
            // ASL - multiply cost by 2 since we're at a neko
            outRom[context.workingOffset++] = 0x0A;
            // PHA - push back
            outRom[context.workingOffset++] = 0x48;
            // skipNekoMultiply:
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDY #0000 - this ends up being how many we can afford
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // LDA 7ecc6a - 16-bit gold LSBs (it's 24 bits total)
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x6A;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            // back:
            // CMP $01,s - compare gold to item cost
            outRom[context.workingOffset++] = 0xC3;
            outRom[context.workingOffset++] = 0x01;
            // BLT/BCC out - don't have enough - branch out
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0B;
            // INY - can afford - increment y
            outRom[context.workingOffset++] = 0xC8;
            // CPY #0007 - if we can afford the max we could buy, break out
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = maxItems;
            outRom[context.workingOffset++] = 0x00;
            // BEQ out
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x05;
            // SEC - subtract the cost and check again with what's remaining
            outRom[context.workingOffset++] = 0x38;
            // SBC $01,s
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x01;
            // BRA back - loop back to see if we can afford another
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xF1;
            // out:
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7ecc6c - check the MSB of gold
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x6C;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            // BEQ notRich - no MSB; go with the calculation from above
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x03;
            // LDY #0007 - assume if we have > 65k gold, we can afford 7 of anything
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = maxItems;
            outRom[context.workingOffset++] = 0x00;
            // notRich:
            // TYA - put number we can afford back into A
            outRom[context.workingOffset++] = 0x98;
            // CMP CONSUMABLE_BUY_MAX
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_MAX;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 16);
            // BGE over - can afford more than we can hold?  don't care
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x04;
            // STA CONSUMABLE_BUY_MAX
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_MAX;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 16);
            // over:
            // LDA CONSUMABLE_BUY_MAX
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_MAX;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 16);
            // STA CONSUMABLE_BUY_CURRENT - default to the max we can buy
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);
            // REP 20 - for PLA plus we want to leave it in that state
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLA - remove the comparison value we pushed earlier
            outRom[context.workingOffset++] = 0x68;

            // that's it! restore the shit we saved at the beginning; i think the only one they care about is X but not sure
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            
            // from 0x1664 - takes text buffer from $A1A4,x and flushes it to screen
            // should be 8bit a and 16bit x
            // original "Okay?" from 0x19FEEC: 7F 52 A9 8B 81 99 CA 00
            String howManyString = "How many? (";
            List<byte> howManyBytes = VanillaEventUtil.getBytes(howManyString);
            int stringLocation = context.workingOffset;
            foreach (byte b in howManyBytes)
            {
                outRom[context.workingOffset++] = b;
            }

            // copy a few vanilla subroutines and move them for easier calling outside of their original banks
            byte[] refreshScreenSubrCopy = vanillaSubrToLongSubr(origRom, 0x1664, 0x16B7).ToArray();
            byte[] refreshScreenSubr2Copy = vanillaSubrToLongSubr(origRom, 0x15B9, 0x162B).ToArray();
            byte[] refreshScreenSubr2ACopy = vanillaSubrToLongSubr(origRom, 0x1539, 0x159D).ToArray();
            byte[] refreshScreenSubr2BCopy = vanillaSubrToLongSubr(origRom, 0x14C6, 0x14DA).ToArray();
            byte[] refreshScreenSubr3Copy = vanillaSubrToLongSubr(origRom, 0x1D24, 0x1D51).ToArray();

            int refreshScreenSubr1Location = context.workingOffset;
            foreach (byte b in refreshScreenSubrCopy)
            {
                outRom[context.workingOffset++] = b;
            }
            int refreshScreenSubr2Location = context.workingOffset;
            foreach (byte b in refreshScreenSubr2Copy)
            {
                outRom[context.workingOffset++] = b;
            }
            int refreshScreenSubr2ALocation = context.workingOffset;
            foreach (byte b in refreshScreenSubr2ACopy)
            {
                outRom[context.workingOffset++] = b;
            }
            int refreshScreenSubr2BLocation = context.workingOffset;
            foreach (byte b in refreshScreenSubr2BCopy)
            {
                outRom[context.workingOffset++] = b;
            }
            int refreshScreenSubr3Location = context.workingOffset;
            foreach (byte b in refreshScreenSubr3Copy)
            {
                outRom[context.workingOffset++] = b;
            }

            int redrawTextLocation = context.workingOffset;

            // PHB
            outRom[context.workingOffset++] = 0x8B;
            // LDA #7E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x7E;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // LDA #80
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x80;
            // LDX #0018
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x00;
            // back:
            // STA $A1A4,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xA4;
            outRom[context.workingOffset++] = 0xA1;
            // DEX
            outRom[context.workingOffset++] = 0xCA;
            // BNE back
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xFA;
            // LDY howManyBytes.Count (0x0B)
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = (byte)howManyBytes.Count;
            outRom[context.workingOffset++] = 0x00;
            // back2:
            // LDA stringLocation,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)(stringLocation);
            outRom[context.workingOffset++] = (byte)(stringLocation >> 8);
            outRom[context.workingOffset++] = (byte)((stringLocation >> 16) + 0xC0);
            // STA $A1A4,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xA4;
            outRom[context.workingOffset++] = 0xA1;
            // DEX
            outRom[context.workingOffset++] = 0xCA;
            // BPL instead to include zero
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0xF6;
            // LDA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #B5
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0xB5;
            // STA 7EA1A4 + howManyBytes.Count
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(0xA4 + howManyBytes.Count);
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0x7E;
            // LDA EventUtil.getByte(')')
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = VanillaEventUtil.getByte(')');
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // STA 7EA1A4 + howManyBytes.Count + 1
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)(0xA4 + howManyBytes.Count + 1);
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0x7E;
            // JSL refreshScreenSubr1Location, refreshScreenSubr2Location
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(refreshScreenSubr1Location);
            outRom[context.workingOffset++] = (byte)(refreshScreenSubr1Location >> 8);
            outRom[context.workingOffset++] = (byte)((refreshScreenSubr1Location >> 16) + 0xC0);
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(refreshScreenSubr2Location);
            outRom[context.workingOffset++] = (byte)(refreshScreenSubr2Location >> 8);
            outRom[context.workingOffset++] = (byte)((refreshScreenSubr2Location >> 16) + 0xC0);

            // LDY #0000
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // code from $C0/151D
            // LDA #0E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x0E;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // JSL refreshScreenSubr2ALocation
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(refreshScreenSubr2ALocation);
            outRom[context.workingOffset++] = (byte)(refreshScreenSubr2ALocation >> 8);
            outRom[context.workingOffset++] = (byte)((refreshScreenSubr2ALocation >> 16) + 0xC0);

            // from $C0/152A: loop and call the above
            // REP #20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // TYA
            outRom[context.workingOffset++] = 0x98;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #0010
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x00;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // SEP #20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // DEC A
            outRom[context.workingOffset++] = 0x3A;
            // BNE to the PHA .. same dist +1 because JSL - ED
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xED;
            // LDY #0010
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x00;
            // code from $C0/151D
            // LDA #0E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x0E;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // JSL refreshScreenSubr2ALocation
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(refreshScreenSubr2ALocation);
            outRom[context.workingOffset++] = (byte)(refreshScreenSubr2ALocation >> 8);
            outRom[context.workingOffset++] = (byte)((refreshScreenSubr2ALocation >> 16) + 0xC0);

            // from $C0/152A: loop and call the above
            // REP #20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // TYA
            outRom[context.workingOffset++] = 0x98;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #0010
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x00;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // SEP #20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // DEC A
            outRom[context.workingOffset++] = 0x3A;
            // BNE to the PHA .. same dist +1 because JSL - ED
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xED;

            // prepare for next call
            // LDA #38
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x38;
            // STA $A171
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x71;
            outRom[context.workingOffset++] = 0xA1;
            // LDY #9000
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x90;
            // LDX #9400
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x94;
            // JSL refreshScreenSubr2BLocation
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(refreshScreenSubr2BLocation);
            outRom[context.workingOffset++] = (byte)(refreshScreenSubr2BLocation >> 8);
            outRom[context.workingOffset++] = (byte)((refreshScreenSubr2BLocation >> 16) + 0xC0);
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // LDA #x67
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x67;
            // STA 7E1D04 - trigger VRAM upload next frame
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x04;
            outRom[context.workingOffset++] = 0x1D;
            outRom[context.workingOffset++] = 0x7E;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            int handleRightLocation = context.workingOffset;
            // LDA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // CMP #00
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            // BEQ over (1)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // LDA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);
            // CMP CONSUMABLE_BUY_MAX
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_MAX;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 16);
            // BCC/BLT handleInput (1)
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // STA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);
            // LDA #1E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x1E;
            // STA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // JSL redrawText
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(redrawTextLocation);
            outRom[context.workingOffset++] = (byte)(redrawTextLocation >> 8);
            outRom[context.workingOffset++] = (byte)((redrawTextLocation >> 16) + 0xC0);
            // JSL $D0CF10 - play menu beep sound
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0xD0;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            int handleLeftLocation = context.workingOffset;
            // LDA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // CMP #00
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            // BEQ over (1)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // LDA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);
            // CMP #02
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x02;
            // BCS/BGE handleInput (1)
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // DEC A
            outRom[context.workingOffset++] = 0x3A;
            // STA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);
            // LDA #1E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x1E;
            // STA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // JSL redrawText
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(redrawTextLocation);
            outRom[context.workingOffset++] = (byte)(redrawTextLocation >> 8);
            outRom[context.workingOffset++] = (byte)((redrawTextLocation >> 16) + 0xC0);
            // JSL $D0CF10 - play menu beep sound
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0xD0;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            int handleUpLocation = context.workingOffset;
            // LDA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // CMP #00
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            // BEQ over (1)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // LDA CONSUMABLE_BUY_MAX
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_MAX;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 16);
            // STA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);
            // LDA #1E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x1E;
            // STA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // JSL redrawText
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(redrawTextLocation);
            outRom[context.workingOffset++] = (byte)(redrawTextLocation >> 8);
            outRom[context.workingOffset++] = (byte)((redrawTextLocation >> 16) + 0xC0);
            // JSL $D0CF10 - play menu beep sound
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0xD0;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            int handleDownLocation = context.workingOffset;
            // LDA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // CMP #00
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            // BEQ over (1)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // LDA #01
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;
            // STA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);
            // LDA #1E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x1E;
            // STA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // JSL redrawText
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(redrawTextLocation);
            outRom[context.workingOffset++] = (byte)(redrawTextLocation >> 8);
            outRom[context.workingOffset++] = (byte)((redrawTextLocation >> 16) + 0xC0);
            // JSL $D0CF10 - play menu beep sound
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0xD0;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            /*
             * replace:
                 $C0/7E83 E2 20       SEP #$20                A:0000 X:0000 Y:002C P:envmxdIZc
                 $C0/7E85 A9 40       LDA #$40                A:0000 X:0000 Y:002C P:envMxdIZc
            */
            outRom[0x7E83] = 0x22;
            outRom[0x7E84] = (byte)(context.workingOffset);
            outRom[0x7E85] = (byte)(context.workingOffset >> 8);
            outRom[0x7E86] = (byte)((context.workingOffset >> 16) + 0xC0);
            // main sub:
            // 8 bit A, 16 bit x/y
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA CONSUMABLE_BUY_MAX
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_MAX;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 16);
            // CMP #00
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            // BNE over (6)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // replaced code
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x40;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // over:
            // LDA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // CMP #00
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x00;
            // BEQ over2 (7)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x07;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #01
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x01;
            // STA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // over2:
            // LDA 7E1801
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x7E;
            // AND #01
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x01;
            // BEQ 04
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x04;
            // JSL handleRight
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(handleRightLocation);
            outRom[context.workingOffset++] = (byte)(handleRightLocation >> 8);
            outRom[context.workingOffset++] = (byte)((handleRightLocation >> 16) + 0xC0);
            // LDA 7E1801
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x7E;
            // AND #02
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x02;
            // BEQ 04
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x04;
            // JSL handleLeft
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(handleLeftLocation);
            outRom[context.workingOffset++] = (byte)(handleLeftLocation >> 8);
            outRom[context.workingOffset++] = (byte)((handleLeftLocation >> 16) + 0xC0);
            // LDA 7E1801
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x7E;
            // AND #04
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x04;
            // BEQ 04
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x04;
            // JSL handleDown
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(handleDownLocation);
            outRom[context.workingOffset++] = (byte)(handleDownLocation >> 8);
            outRom[context.workingOffset++] = (byte)((handleDownLocation >> 16) + 0xC0);
            // LDA 7E1801
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x7E;
            // AND #08
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x08;
            // BEQ 04
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x04;
            // JSL handleUp
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(handleUpLocation);
            outRom[context.workingOffset++] = (byte)(handleUpLocation >> 8);
            outRom[context.workingOffset++] = (byte)((handleUpLocation >> 16) + 0xC0);
            // LDA 7E1801
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x7E;
            // AND #0F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;
            // BNE xx (6)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x06;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // xx:
            // replaced code
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x40;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            
            // ///////////////////////////////////////////////////////////////////
            // logic for confirmation handling - gold subtraction
            // if CONSUMABLE_BUY_CURRENT > 0
            //   gold -= CONSUMABLE_BUY_CURRENT * price (use LDA $01,s then multiply stuff)
            //   BUYING_MULTIPLE_ITEMS = 1 - mark this here so the item addition below knows the difference between a buy and a drop
            // else
            //   do original subtraction - 7 bytes from above
            // ///////////////////////////////////////////////////////////////////
            // when sbc, carry is cleared if we over(under)flow

            /*
             *  replace:
                $C0/7C8C AF 6A CC 7E LDA $7ECC6A[$7E:CC6A]   A:000A X:0000 Y:000A P:envmxdIZc
                $C0/7C90 38          SEC                     A:000A X:0000 Y:000A P:envmxdIzc
                $C0/7C91 E3 01       SBC $01,s  [$00:01E3]   A:000A X:0000 Y:000A P:envmxdIzC
             */
            outRom[0x7C8C] = 0x22;
            outRom[0x7C8D] = (byte)(context.workingOffset);
            outRom[0x7C8E] = (byte)(context.workingOffset >> 8);
            outRom[0x7C8F] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x7C90] = 0xEA;
            outRom[0x7C91] = 0xEA;
            outRom[0x7C92] = 0xEA;

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA 7E1847 - buy menu type
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x47;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x7E;

            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;

            // BEQ over
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x0A;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // replaced code [7 bytes]
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x6A;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            outRom[context.workingOffset++] = 0x38;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x04; // modified due to stack thing
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // LDA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);

            // CMP #02
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x02;
            // BGE over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x0A;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // replaced code [7 bytes]
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x6A;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            outRom[context.workingOffset++] = 0x38;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x04; // modified due to stack thing
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // over:
            // LDA #01
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;

            // STA BUYING_MULTIPLE_ITEMS
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.BUYING_MULTIPLE_ITEMS;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.BUYING_MULTIPLE_ITEMS >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.BUYING_MULTIPLE_ITEMS >> 16);

            // PHX - save old x
            outRom[context.workingOffset++] = 0xDA;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);

            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA 7ecc6a - current gold LSBs
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x6A;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            // loop:
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC $06,s - price already pushed outside the modified code
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x06;
            // BCS skipUnderflow
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x13 + 3;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7ecc6c - handle underflow; subtract one from gold msb
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x6C;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            // DEC A
            outRom[context.workingOffset++] = 0x3A;
            // BPL noError
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x05;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // PLX - underflowed gold; return
            outRom[context.workingOffset++] = 0xFA;
            // RTL -
            outRom[context.workingOffset++] = 0x6B;
            // noError:
            // STA 7ecc6c - store gold msb back
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x6C;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // skipUnderflow:
            // DEX
            outRom[context.workingOffset++] = 0xCA;
            // CPX #0001 - to fix off by one
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BEQ done (2)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x02;
            // BRA loop
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xE0 - 3;
            // done:
            // STA 7ECC6A - save current gold
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x6A;
            outRom[context.workingOffset++] = 0xCC;
            outRom[context.workingOffset++] = 0x7E;
            // SEC - so the next code doesn't handle underflow
            outRom[context.workingOffset++] = 0x38;
            // PLX - value we pushed earlier
            outRom[context.workingOffset++] = 0xFA;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // ///////////////////////////////////////////////////////////////////
            // logic for confirmation handling - item addition
            // if CONSUMABLE_BUY_CURRENT > 0 and BUYING_MULTIPLE_ITEMS == 1
            //   LDA $7ECC00,x[$7E:CC48] to pull current value as in replaced code
            //   while CONSUMABLE_BUY_CURRENT > 0
            //     ADC #20
            //     CONSUMABLE_BUY_CURRENT--
            //   CONSUMABLE_BUY_MAX = 0
            //   CONSUMABLE_INPUT_TIMER = 0
            //   BUYING_MULTIPLE_ITEMS = 0
            // else
            //   do original code - 7 bytes
            // ///////////////////////////////////////////////////////////////////

            /*
                replace:
                $C0/657E 0A          ASL A                   A:0000 X:0048 Y:0000 P:envMxdIZc
                $C0/657F 0A          ASL A                   A:0000 X:0048 Y:0000 P:envMxdIZc
                $C0/6580 0A          ASL A                   A:0000 X:0048 Y:0000 P:envMxdIZc
                $C0/6581 0A          ASL A                   A:0000 X:0048 Y:0000 P:envMxdIZc
             */
            outRom[0x657E] = 0x22;
            outRom[0x657F] = (byte)(context.workingOffset);
            outRom[0x6580] = (byte)(context.workingOffset >> 8);
            outRom[0x6581] = (byte)((context.workingOffset >> 16) + 0xC0);

            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA BUYING_MULTIPLE_ITEMS
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.BUYING_MULTIPLE_ITEMS;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.BUYING_MULTIPLE_ITEMS >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.BUYING_MULTIPLE_ITEMS >> 16);

            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;

            // BNE to the BRA
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x08;


            // LDA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);
            // CMP #02
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x02;
            // BNE over [C] -> BGE
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x02;

            // BRA end
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x15;

            // over:
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $xx,s
            outRom[context.workingOffset++] = 0xA3;
            outRom[context.workingOffset++] = 0x07;
            // back:
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #20
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x20;
            // DEX
            outRom[context.workingOffset++] = 0xCA;
            // CPX #0001 - to fix off by one
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BNE back
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xF7;
            // STA $xx,s
            outRom[context.workingOffset++] = 0x83;
            outRom[context.workingOffset++] = 0x07;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA CONSUMABLE_BUY_MAX
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_MAX;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_MAX >> 16);
            // STA CONSUMABLE_BUY_CURRENT
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_BUY_CURRENT;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_BUY_CURRENT >> 16);
            // STA CONSUMABLE_INPUT_TIMER
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.CONSUMABLE_INPUT_TIMER;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.CONSUMABLE_INPUT_TIMER >> 16);
            // STA BUYING_MULTIPLE_ITEMS
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.BUYING_MULTIPLE_ITEMS;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.BUYING_MULTIPLE_ITEMS >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.BUYING_MULTIPLE_ITEMS >> 16);
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // replaced code
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // buying confirmation text

            /*
             *  replace:
                $C0/17C1 E2 20       SEP #$20                A:FEF3 X:0005 Y:FEF3 P:eNvmxdIzc
                $C0/17C3 9C D0 A1    STZ $A1D0  [$7E:A1D0]   A:FEF3 X:0005 Y:FEF3 P:eNvMxdIzc
             */
            outRom[0x17C1] = 0x22;
            outRom[0x17C2] = (byte)(context.workingOffset);
            outRom[0x17C3] = (byte)(context.workingOffset >> 8);
            outRom[0x17C4] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x17C5] = 0xEA;

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // STZ $A1D0
            outRom[context.workingOffset++] = 0x9C;
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xA1;
            // CPY #FEF3 - i don't remember what value this represents - some state being checked to make sure we're buying items
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xF3;
            outRom[context.workingOffset++] = 0xFE;
            // BEQ over
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // LDA #09
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x09;
            // STA $A1D0
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xA1;
            // LDA $1847
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x47;
            outRom[context.workingOffset++] = 0x18;
            // CMP #01 - buy menu id
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BEQ 01
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x01;
            // RTL - not in buy menu
            outRom[context.workingOffset++] = 0x6B;

            // also have to check that we're buying a consumable
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7E183C - shop offset
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x3C;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x7E;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // ADC 7E180A - ring menu selection index
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x7E;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // TAX - shop offset + ring menu selection index to x
            outRom[context.workingOffset++] = 0xAA;
            // SEP #20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $D80000,x - load from shop data
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xD8;
            // CMP #BA - check for consumable id
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xBA;
            // BCS/BGE over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x01;
            // RTL if not a consumable
            outRom[context.workingOffset++] = 0x6B;
            // JSL redrawText - render the new text
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(redrawTextLocation);
            outRom[context.workingOffset++] = (byte)(redrawTextLocation >> 8);
            outRom[context.workingOffset++] = (byte)((redrawTextLocation >> 16) + 0xC0);
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            return true;
        }

        // convert a vanilla short-range subroutine (ending with 0x60 RTS) to a long-range one (ending with 0x6b RTL) for easier injection & calling
        private List<byte> vanillaSubrToLongSubr(byte[] rom, int startAddr, int returnAddr)
        {
            List<byte> subrData = new List<byte>();
            for (int i = startAddr; i < returnAddr; i++)
            {
                subrData.Add(rom[i]);
            }
            // replace original 0x60 with 0x6B since we're repurposing this as a long subroutine
            subrData.Add(0x6B);
            return subrData;
        }
    }
}
