
namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Enhance event 1E so i can pass FF as the item ID, then an actual item ID, and it will try to remove that thing
    /// 
    /// if it fails (don't have it) we set event flag 0F (7ECF0F) to 01 like the remove gold event does, so we can check it via logic in events
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class RemoveItemEventCommand
    {
        public void process(byte[] outRom, ref int newCodeOffset)
        {
            // this is mainly for the christmas mode so you can give shit away.

            // subroutine to erase an item, at current "x" index in 7ECCxx, and shift all the other items back
            int subEraseItemOffset = newCodeOffset;
            // 16 bit xy, 8 bit A here
            // back:
            // LDA 7ECC01,x
            outRom[newCodeOffset++] = 0xBF;
            outRom[newCodeOffset++] = 0x01;
            outRom[newCodeOffset++] = 0xCC;
            outRom[newCodeOffset++] = 0x7E;
            // STA 7ECC00,x
            outRom[newCodeOffset++] = 0x9F;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xCC;
            outRom[newCodeOffset++] = 0x7E;
            // INX
            outRom[newCodeOffset++] = 0xE8;
            // INY
            outRom[newCodeOffset++] = 0xC8;
            // CPY #000A - last item; B is the trashcan
            outRom[newCodeOffset++] = 0xC0;
            outRom[newCodeOffset++] = 0x0A;
            outRom[newCodeOffset++] = 0x00;
            // BCS/BGE out
            outRom[newCodeOffset++] = 0xB0;
            outRom[newCodeOffset++] = 0x02;
            // BRA back
            outRom[newCodeOffset++] = 0x80;
            outRom[newCodeOffset++] = 0xEF;
            // out:
            // LDA #FF
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0xFF;
            // STA 7ECC00,x
            outRom[newCodeOffset++] = 0x9F;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xCC;
            outRom[newCodeOffset++] = 0x7E;
            // RTL
            outRom[newCodeOffset++] = 0x6B;


            // subroutine to remove a specific item by ID in our inventory. ID can be found in A.
            // set 7ECF0F based on whether we were able to remove it.  0=success, 1=fail.
            int subDirectMatchOffset = newCodeOffset;
            // loop:
            // CMP 7ECC00,x
            outRom[newCodeOffset++] = 0xDF;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xCC;
            outRom[newCodeOffset++] = 0x7E;
            // BEQ match
            outRom[newCodeOffset++] = 0xF0;
            outRom[newCodeOffset++] = 0x09;
            // INX
            outRom[newCodeOffset++] = 0xE8;
            // INY
            outRom[newCodeOffset++] = 0xC8;
            // CPY #000C
            outRom[newCodeOffset++] = 0xC0;
            outRom[newCodeOffset++] = 0x0C;
            outRom[newCodeOffset++] = 0x00;
            // BEQ nomatch
            outRom[newCodeOffset++] = 0xF0;
            outRom[newCodeOffset++] = 0x0D;
            // BRA loop
            outRom[newCodeOffset++] = 0x80;
            outRom[newCodeOffset++] = 0xF1;

            // match:
            // JSL sub_eraseItem
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subEraseItemOffset;
            outRom[newCodeOffset++] = (byte)(subEraseItemOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subEraseItemOffset >> 16) + 0xC0);
            // LDA #00
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x00;
            // STA $7ECF0F
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // RTL
            outRom[newCodeOffset++] = 0x6B;
            
            // nomatch:
            // LDA #01
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x01;
            // STA $7ECF0F
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // RTL
            outRom[newCodeOffset++] = 0x6B;


            int subRemoveConsumableOffset = newCodeOffset;
            // PHA
            outRom[newCodeOffset++] = 0x48;
            // LDX #48
            outRom[newCodeOffset++] = 0xA2;
            outRom[newCodeOffset++] = 0x48;
            outRom[newCodeOffset++] = 0x00;
            // loop:
            // LDA 7ECC00,x
            outRom[newCodeOffset++] = 0xBF;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xCC;
            outRom[newCodeOffset++] = 0x7E;
            // AND #1F
            outRom[newCodeOffset++] = 0x29;
            outRom[newCodeOffset++] = 0x1F;
            // CMP $01,s
            outRom[newCodeOffset++] = 0xC3;
            outRom[newCodeOffset++] = 0x01;
            // BEQ match
            outRom[newCodeOffset++] = 0xF0;
            outRom[newCodeOffset++] = 0x09;

            // INX
            outRom[newCodeOffset++] = 0xE8;
            // INY
            outRom[newCodeOffset++] = 0xC8;
            // CPY #000C
            outRom[newCodeOffset++] = 0xC0;
            outRom[newCodeOffset++] = 0x0C;
            outRom[newCodeOffset++] = 0x00;
            // BEQ nomatch
            outRom[newCodeOffset++] = 0xF0;
            outRom[newCodeOffset++] = 0x1F;
            // BRA loop
            outRom[newCodeOffset++] = 0x80;
            outRom[newCodeOffset++] = 0xED;

            // match:
            // PLA  - even out the stack
            outRom[newCodeOffset++] = 0x68;
            // LDA 7ECC00,x
            outRom[newCodeOffset++] = 0xBF;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xCC;
            outRom[newCodeOffset++] = 0x7E;
            // SEC
            outRom[newCodeOffset++] = 0x38;
            // SBC #20
            outRom[newCodeOffset++] = 0xE9;
            outRom[newCodeOffset++] = 0x20;
            // CMP #20
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0x20;
            // BGE/BCS hadMoreThanOne
            outRom[newCodeOffset++] = 0xB0;
            outRom[newCodeOffset++] = 0x06;

            // JSL sub_eraseItem
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subEraseItemOffset;
            outRom[newCodeOffset++] = (byte)(subEraseItemOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subEraseItemOffset >> 16) + 0xC0);
            // BRA end (04)
            outRom[newCodeOffset++] = 0x80;
            outRom[newCodeOffset++] = 0x04;
            // hadMoreThanOne:
            // STA 7ECC00,x
            outRom[newCodeOffset++] = 0x9F;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xCC;
            outRom[newCodeOffset++] = 0x7E;
            // end:
            // LDA #00
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x00;
            // STA 7ECF0F
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // nomatch:
            // PLA  - even out the stack
            outRom[newCodeOffset++] = 0x68;
            // LDA #01
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x01;
            // STA 7ECF0F
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // RTL
            outRom[newCodeOffset++] = 0x6B;


            // remove first hat in inventory
            int subRemoveHatOffset = newCodeOffset;
            // LDX #0024
            outRom[newCodeOffset++] = 0xA2;
            outRom[newCodeOffset++] = 0x24;
            outRom[newCodeOffset++] = 0x00;
            // loop:
            // LDA 7ECC00,x
            outRom[newCodeOffset++] = 0xBF;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xCC;
            outRom[newCodeOffset++] = 0x7E;
            // CMP #15
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0x15;
            // BCC/BLT found
            outRom[newCodeOffset++] = 0x90;
            outRom[newCodeOffset++] = 0x0E;

            // INX
            outRom[newCodeOffset++] = 0xE8;
            // INY
            outRom[newCodeOffset++] = 0xC8;
            // CPY #000C
            outRom[newCodeOffset++] = 0xC0;
            outRom[newCodeOffset++] = 0x0C;
            outRom[newCodeOffset++] = 0x00;
            // BNE loop
            outRom[newCodeOffset++] = 0xD0;
            outRom[newCodeOffset++] = 0xF1;

            // LDA #01
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x01;
            // STA 7ECF0F
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // found:
            // LDA #00
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x00;
            // STA 7ECF0F
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // JSL sub_eraseItem
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subEraseItemOffset;
            outRom[newCodeOffset++] = (byte)(subEraseItemOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subEraseItemOffset >> 16) + 0xC0);
            // RTL
            outRom[newCodeOffset++] = 0x6B;


            // remove first armor in inventory
            int subRemoveArmorOffset = newCodeOffset;
            // LDX #0030
            outRom[newCodeOffset++] = 0xA2;
            outRom[newCodeOffset++] = 0x30;
            outRom[newCodeOffset++] = 0x00;
            // loop:
            // LDA 7ECC00,x
            outRom[newCodeOffset++] = 0xBF;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xCC;
            outRom[newCodeOffset++] = 0x7E;
            // CMP #15
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0x15;
            // BCC/BLT increment
            outRom[newCodeOffset++] = 0x90;
            outRom[newCodeOffset++] = 0x04;

            // CMP #2A
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0x2A;
            // BCC found
            outRom[newCodeOffset++] = 0x90;
            outRom[newCodeOffset++] = 0x0E;

            // increment:
            // INX
            outRom[newCodeOffset++] = 0xE8;
            // INY
            outRom[newCodeOffset++] = 0xC8;
            // CPY #000C
            outRom[newCodeOffset++] = 0xC0;
            outRom[newCodeOffset++] = 0x0C;
            outRom[newCodeOffset++] = 0x00;
            // BNE loop
            outRom[newCodeOffset++] = 0xD0;
            outRom[newCodeOffset++] = 0xED;

            // LDA #01
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x01;
            // STA 7ECF0F
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // found:
            // LDA #00
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x00;
            // STA 7ECF0F
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // JSL sub_eraseItem
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subEraseItemOffset;
            outRom[newCodeOffset++] = (byte)(subEraseItemOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subEraseItemOffset >> 16) + 0xC0);
            // RTL
            outRom[newCodeOffset++] = 0x6B;


            // remove first accessory in inventory
            int subRemoveAccessoryOffset = newCodeOffset;
            // LDX #003C
            outRom[newCodeOffset++] = 0xA2;
            outRom[newCodeOffset++] = 0x3C;
            outRom[newCodeOffset++] = 0x00;
            // loop:
            // LDA 7ECC00,x
            outRom[newCodeOffset++] = 0xBF;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xCC;
            outRom[newCodeOffset++] = 0x7E;
            // CMP #2A
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0x2A;
            // BCC/BLT increment
            outRom[newCodeOffset++] = 0x90;
            outRom[newCodeOffset++] = 0x04;
            // CMP #3F
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0x3F;
            // BCC found
            outRom[newCodeOffset++] = 0x90;
            outRom[newCodeOffset++] = 0x0E;

            // increment:
            // INX
            outRom[newCodeOffset++] = 0xE8;
            // INY
            outRom[newCodeOffset++] = 0xC8;
            // CPY #000C
            outRom[newCodeOffset++] = 0xC0;
            outRom[newCodeOffset++] = 0x0C;
            outRom[newCodeOffset++] = 0x00;
            // BNE loop
            outRom[newCodeOffset++] = 0xD0;
            outRom[newCodeOffset++] = 0xED;

            // LDA #01
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x01;
            // STA 7ECF0F
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // found:
            // LDA #00
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x00;
            // STA 7ECF0F
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // JSL sub_eraseItem
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subEraseItemOffset;
            outRom[newCodeOffset++] = (byte)(subEraseItemOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subEraseItemOffset >> 16) + 0xC0);
            // RTL
            outRom[newCodeOffset++] = 0x6B;


            /*
             * replace for event command 1E handling:
                C1/EB5C:    225100C0    JSR $C00051
             */
            // main replacement:
            outRom[0x1EB5C] = 0x22;
            outRom[0x1EB5D] = (byte)newCodeOffset;
            outRom[0x1EB5E] = (byte)(newCodeOffset >> 8);
            outRom[0x1EB5F] = (byte)((newCodeOffset >> 16) + 0xC0);

            // CMP #FF
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0xFF;
            // BEQ over [5]
            outRom[newCodeOffset++] = 0xF0;
            outRom[newCodeOffset++] = 0x05;
            // JSL C00051 - not 0xFF; just do vanilla behavior
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = 0x51;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0xC0;
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // over:
            // LDA [$D1] - read our extra byte
            outRom[newCodeOffset++] = 0xA7;
            outRom[newCodeOffset++] = 0xD1;
            // LDX $D1
            outRom[newCodeOffset++] = 0xA6;
            outRom[newCodeOffset++] = 0xD1;
            // INX - increment the event by one more byte
            outRom[newCodeOffset++] = 0xE8;
            // STX $D1
            outRom[newCodeOffset++] = 0x86;
            outRom[newCodeOffset++] = 0xD1;
            // now a similar loop to what adding does in bank 0 (C0/64BD), except we're trying to remove
            // starting around C0/64D5:   A00000     LDY #$0000
            // LDY #0000    A0 00 00
            outRom[newCodeOffset++] = 0xA0;
            outRom[newCodeOffset++] = 0x00;
            outRom[newCodeOffset++] = 0x00;
            // CMP #15
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0x15;
            // BCS/BGE over_hats (8)
            outRom[newCodeOffset++] = 0xB0;
            outRom[newCodeOffset++] = 0x08;
            // LDX #0024
            outRom[newCodeOffset++] = 0xA2;
            outRom[newCodeOffset++] = 0x24;
            outRom[newCodeOffset++] = 0x00;
            // JSL sub_remove_direct_match
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subDirectMatchOffset;
            outRom[newCodeOffset++] = (byte)(subDirectMatchOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subDirectMatchOffset >> 16) + 0xC0);
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // over_hats:
            // CMP #2A
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0x2A;
            // BCS over_armors
            outRom[newCodeOffset++] = 0xB0;
            outRom[newCodeOffset++] = 0x08;
            // LDX #0030
            outRom[newCodeOffset++] = 0xA2;
            outRom[newCodeOffset++] = 0x30;
            outRom[newCodeOffset++] = 0x00;
            // JSL sub_remove_direct_match
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subDirectMatchOffset;
            outRom[newCodeOffset++] = (byte)(subDirectMatchOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subDirectMatchOffset >> 16) + 0xC0);
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // over_armors:
            // CMP #3F
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0x3F;
            // BCS over_acc
            outRom[newCodeOffset++] = 0xB0;
            outRom[newCodeOffset++] = 0x08;
            // LDX #003C
            outRom[newCodeOffset++] = 0xA2;
            outRom[newCodeOffset++] = 0x3C;
            outRom[newCodeOffset++] = 0x00;
            // JSL sub_remove_direct_match
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subDirectMatchOffset;
            outRom[newCodeOffset++] = (byte)(subDirectMatchOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subDirectMatchOffset >> 16) + 0xC0);
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // over_acc:
            // consumables in range 0x40 - 0x4C here
            // CMP #4C
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0x4C;
            // BCS over_consumable
            outRom[newCodeOffset++] = 0xB0;
            outRom[newCodeOffset++] = 0x07;
            // AND #1F
            outRom[newCodeOffset++] = 0x29;
            outRom[newCodeOffset++] = 0x1F;
            // JSL sub_remove_consumable
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subRemoveConsumableOffset;
            outRom[newCodeOffset++] = (byte)(subRemoveConsumableOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subRemoveConsumableOffset >> 16) + 0xC0);
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // CMP #FF - any hat
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0xFF;
            // BNE over_hat
            outRom[newCodeOffset++] = 0xD0;
            outRom[newCodeOffset++] = 0x05;
            // JSL sub_remove_hat
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subRemoveHatOffset;
            outRom[newCodeOffset++] = (byte)(subRemoveHatOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subRemoveHatOffset >> 16) + 0xC0);
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // over_hat:
            // CMP #FE - any armor
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0xFE;
            // BNE over_armor
            outRom[newCodeOffset++] = 0xD0;
            outRom[newCodeOffset++] = 0x05;
            // JSL sub_remove_armor
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subRemoveArmorOffset;
            outRom[newCodeOffset++] = (byte)(subRemoveArmorOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subRemoveArmorOffset >> 16) + 0xC0);
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // over_armor:
            // CMP #FD - any accessory
            outRom[newCodeOffset++] = 0xC9;
            outRom[newCodeOffset++] = 0xFD;
            // BNE over_accessory
            outRom[newCodeOffset++] = 0xD0;
            outRom[newCodeOffset++] = 0x05;
            // JSL sub_remove_accessory
            outRom[newCodeOffset++] = 0x22;
            outRom[newCodeOffset++] = (byte)subRemoveAccessoryOffset;
            outRom[newCodeOffset++] = (byte)(subRemoveAccessoryOffset >> 8);
            outRom[newCodeOffset++] = (byte)((subRemoveAccessoryOffset >> 16) + 0xC0);
            // RTL
            outRom[newCodeOffset++] = 0x6B;

            // over_accessory:
            // LDA #01
            outRom[newCodeOffset++] = 0xA9;
            outRom[newCodeOffset++] = 0x01;
            // STA 7eCF0F - done and we didn't do anything
            outRom[newCodeOffset++] = 0x8F;
            outRom[newCodeOffset++] = 0x0F;
            outRom[newCodeOffset++] = 0xCF;
            outRom[newCodeOffset++] = 0x7E;
            // RTL
            outRom[newCodeOffset++] = 0x6B;

        }
    }
}
