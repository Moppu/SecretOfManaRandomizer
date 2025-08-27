using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Modify drop tables for open world to only drop consumables and money.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DropOnlyConsumables : RandoProcessor
    {
        // and gold!
        protected override string getName()
        {
            return "Open world enemies only drop consumables and money";
        }

        // drop table
        // D0/3A50:	08 10 F8 40 04 [00: Rabite]
        //                      ^^ rare drop id or gold amount
        //                   ^^ common drop id or gold amount
        //                ^^ control byte; 0xC0 control whether the next two bytes are item or gold
        //             ^^ not exactly sure how these bits work, but this is related to traps chance
        //          ^^ 0x80 controls whether a chest walks around; 0x0F is drop chance
        // a bunch of the above is currently unknown - feel free to fill in
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;
            int codeSize = 200;
            int dropTableSize = 0;
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, codeSize + dropTableSize);
            int dropTableOffset = 0x103A50;
            string trapMode = settings.get(OpenWorldSettings.PROPERTYNAME_CHEST_TRAPS);
            bool noWalkingChests = settings.getBool(OpenWorldSettings.PROPERTYNAME_NO_WALKING_CHESTS);
            string chestFreq = settings.get(OpenWorldSettings.PROPERTYNAME_CHEST_FREQUENCY);
            // 50 * multiplier gold per level.  max of 255 since it's an 8 bit load.
            int goldMultiplier = settings.getInt(OpenWorldSettings.PROPERTYNAME_NUMERIC_GOLD_DROP_MULTIPLIER) * 50;
            goldMultiplier = DataUtil.clampToEndpoints(goldMultiplier, 0, 255);
            outRom[0x428E] = 0x0F; // more drops

            // drop items 40-45, 4A (consumables)
            byte[] items = new byte[] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x4A };
            for(int i=0; i <= 0x53; i++)
            {
                bool item1Gold = (r.Next() % 2) == 0;
                bool item2Gold = (r.Next() % 2) == 0;
                /*
                 *   controls drop types as follows:
                               common     rare
                     xx000000   item      gold
                     x0000000   gold      item
                     0x000000   item      item
                     00000000   gold      gold
                 */
                byte itemTypeControl = 0x00;
                if (item1Gold && item2Gold)
                {
                    itemTypeControl = 0x00;
                }
                else if (item1Gold && !item2Gold)
                {
                    itemTypeControl = 0x80;
                }
                else if (!item1Gold && !item2Gold)
                {
                    itemTypeControl = 0x40;
                }
                else if (!item1Gold && item2Gold)
                {
                    itemTypeControl = 0xC0;
                }

                outRom[dropTableOffset + i * 5 + 2] = 0x38;
                if(item1Gold)
                {
                    outRom[dropTableOffset + i * 5 + 3] = 250;
                }
                else
                {
                    outRom[dropTableOffset + i * 5 + 3] = items[r.Next() % items.Length];
                }
                if (item2Gold)
                {
                    outRom[dropTableOffset + i * 5 + 4] = 250;
                }
                else
                {
                    outRom[dropTableOffset + i * 5 + 4] = items[r.Next() % items.Length];
                }

                outRom[dropTableOffset + i * 5 + 2] |= itemTypeControl;

                if (noWalkingChests)
                {
                    // turn off 0x80 bit so none of them get up and walk
                    outRom[dropTableOffset + i * 5 + 0] &= 0x7F;
                }

                if(trapMode == "none")
                {
                    outRom[dropTableOffset + i * 5 + 1] = 0x10;
                }
                else if(trapMode == "many")
                {
                    outRom[dropTableOffset + i * 5 + 1] = 0xF3;
                }

                switch(chestFreq)
                {
                    case "none":
                        // no drops
                        outRom[dropTableOffset + i * 5 + 0] &= 0xF0;
                        break;
                    case "low":
                        // not many.
                        outRom[dropTableOffset + i * 5 + 0] &= 0xF0;
                        outRom[dropTableOffset + i * 5 + 0] |= 0x02;
                        break;
                    case "normal":
                        // no change
                        break;
                    case "high":
                        // rabite odds
                        outRom[dropTableOffset + i * 5 + 0] &= 0xF0;
                        outRom[dropTableOffset + i * 5 + 0] |= 0x08;
                        break;
                    case "highest":
                        // lots.
                        outRom[dropTableOffset + i * 5 + 0] &= 0xF0;
                        outRom[dropTableOffset + i * 5 + 0] |= 0x0F;
                        break;
                }
            }
            

            // gold drop amount dependent on level
            // $C8/E1E0 22 CF 39 C0 JSL $C039CF[$C0:39CF]   A:00FA X:0078 Y:0000 P:envmxdIzc - add gold
            outRom[0x8e1e0] = 0x22;
            outRom[0x8e1e1] = (byte)(context.workingOffset);
            outRom[0x8e1e2] = (byte)(context.workingOffset >> 8);
            outRom[0x8e1e3] = (byte)((context.workingOffset >> 16) + 0xC0);
            // LDA OPENWORLD_CURRENT_ENEMY_LEVEL
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.OPENWORLD_CURRENT_ENEMY_LEVEL;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.OPENWORLD_CURRENT_ENEMY_LEVEL >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.OPENWORLD_CURRENT_ENEMY_LEVEL >> 16);
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // STA 4202 (mul A)
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // LDA #x32
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)goldMultiplier;
            // STA 4203 (mul B)
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // 8 nops to let it multiply
            for (int i = 0; i < 8; i++)
            {
                outRom[context.workingOffset++] = 0xEA;
            }
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 4216 (result)
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x16;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // overwrite the stack value we stored off earlier
            outRom[context.workingOffset++] = 0x83;
            outRom[context.workingOffset++] = 0x04;
            // call the gold thing we replaced
            // 22 CF 39 C0
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0xCF;
            outRom[context.workingOffset++] = 0x39;
            outRom[context.workingOffset++] = 0xC0;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            
            // capture and tweak a few vanilla subroutines for modified drop logic
            // this fixed a glitch where gold drop values > 255 were not possible due to a
            // PHA/PLA only keeping the lower 8 bits of A around

            // take vanilla subroutine 5c2a-5c37 and convert it to a long call
            List<byte> subr5c2a_5c37 = new List<byte>();
            for(int i=0x5c2a; i <= 0x5c37; i++)
            {
                byte b = outRom[i];
                if(i == 0x5c37)
                {
                    b = 0x6B; // rts -> rtl
                }
                subr5c2a_5c37.Add(b);
            }
            int subr5c2a_5c37_newLoc = context.workingOffset;
            foreach(byte b in subr5c2a_5c37)
            {
                outRom[context.workingOffset++] = b;
            }

            // take vanilla subroutine 5c6c-5cc4 and convert it to a long call
            List<byte> subr5c6c_5cc4 = new List<byte>();
            for (int i = 0x5c6c; i <= 0x5cc4; i++)
            {
                byte b = outRom[i];
                if (i == 0x5cc4)
                {
                    b = 0x6B; // rts -> rtl
                }
                subr5c6c_5cc4.Add(b);
            }
            int subr5c6c_5cc4_newLoc = context.workingOffset;
            foreach (byte b in subr5c6c_5cc4)
            {
                outRom[context.workingOffset++] = b;
            }


            // take vanilla subroutine 5d33-5d45 and convert it to a long call
            List<byte> subr5d33_5d45 = new List<byte>();
            for (int i = 0x5d33; i <= 0x5d45; i++)
            {
                byte b = outRom[i];
                if (i == 0x5d45)
                {
                    b = 0x6B; // rts -> rtl
                }
                subr5d33_5d45.Add(b);
            }
            int subr5d33_5d45_newLoc = context.workingOffset;
            foreach (byte b in subr5d33_5d45)
            {
                outRom[context.workingOffset++] = b;
            }


            // 5d08 - rewrite manually to change the JSR $5D33 to a long subr call and return long; otherwise no changes from vanilla
            int subr5d08_newLoc = context.workingOffset;
            // LDA $A2
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA2;
            // DEC A
            outRom[context.workingOffset++] = 0x3A;
            // BPL $02
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x02;
            // LDA #0B
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x0B;
            // STA $A2
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA2;
            // STA $004202 - multiply
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // LDA #1E
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x1E;
            // STA $004203 - multiply
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // NOP to wait for multiply
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            outRom[context.workingOffset++] = 0xEA;
            // LDA $004216 - load result
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x16;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0x00;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // JSR $5D33 -> JSL subr5d33_5d45_newLoc
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(subr5d33_5d45_newLoc);
            outRom[context.workingOffset++] = (byte)(subr5d33_5d45_newLoc >> 8);
            outRom[context.workingOffset++] = (byte)((subr5d33_5d45_newLoc >> 16) + 0xC0);
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #60
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x60;
            // STA $FE00,y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xFE;
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // INC $9F
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x9F;
            // RTS -> RTL
            outRom[context.workingOffset++] = 0x6B;


            // 58c8 - rewrite manually
            /*
            $C0/58C8 48          PHA                     A:041A X:00C8 Y:0000 P:envMxdIzC - 8 bit
            $C0/58C9 20 08 5D    JSR $5D08  [$C0:5D08]   A:041A X:00C8 Y:0000 P:envMxdIzC
            $C0/58CC 68          PLA                     A:0060 X:00C8 Y:003D P:envMxdIzC
            $C0/58CD 85 D0       STA $D0    [$00:03D0]   A:001A X:00C8 Y:003D P:envMxdIzC - value we're printing
            $C0/58CF 64 D1       STZ $D1    [$00:03D1]   A:001A X:00C8 Y:003D P:envMxdIzC - msb of value (zero)
            $C0/58D1 20 6C 5C    JSR $5C6C  [$C0:5C6C]   A:001A X:00C8 Y:003D P:envMxdIzC - print 16 bit value from $D0
            $C0/58D4 A2 94 62    LDX #$6294              A:00BB X:00C8 Y:003F P:envMxdIzC - " GP inside!" message
            $C0/58D7 20 2A 5C    JSR $5C2A  [$C0:5C2A]   A:00BB X:6294 Y:003F P:envMxdIzC
            $C0/58DA 6B          RTL                     A:0000 X:629F Y:004A P:envMxdIZC
             */
            int subr58c8_newLoc = context.workingOffset;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // add sep 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // JSR $5D08 -> JSL subr5d08_newLoc
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(subr5d08_newLoc);
            outRom[context.workingOffset++] = (byte)(subr5d08_newLoc >> 8);
            outRom[context.workingOffset++] = (byte)((subr5d08_newLoc >> 16) + 0xC0);
            // add rep 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // STA $D0
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xD0;
            // add sep 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // JSR $5C6C -> JSL subr5c6c_5cc4_newLoc - print 16-bit value from $D0
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(subr5c6c_5cc4_newLoc);
            outRom[context.workingOffset++] = (byte)(subr5c6c_5cc4_newLoc >> 8);
            outRom[context.workingOffset++] = (byte)((subr5c6c_5cc4_newLoc >> 16) + 0xC0);
            // LDX #6294 - " GP inside!" message
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x94;
            outRom[context.workingOffset++] = 0x62;
            // JSR $5C2A -> JSL subr5c2a_5c37_newLoc
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(subr5c2a_5c37_newLoc);
            outRom[context.workingOffset++] = (byte)(subr5c2a_5c37_newLoc >> 8);
            outRom[context.workingOffset++] = (byte)((subr5c2a_5c37_newLoc >> 16) + 0xC0);
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // 8e1e8 - jump to our new 58c8 instead
            /*
                $C8/E1E6 E2 20       SEP #$20                A:041A X:00C8 Y:0000 P:envmxdIzC
                $C8/E1E8 22 C8 58 C0 JSL $C058C8[$C0:58C8]   A:041A X:00C8 Y:0000 P:envMxdIzC
             */
            // the sep 20 gets removed here so that we save the whole 16-bit A inside.
            // the other subroutines are vanilla and need to be grabbed so they can be called
            // from this other bank (and returned to properly)
            outRom[0x8e1e6] = 0xEA;
            outRom[0x8e1e7] = 0xEA;
            outRom[0x8e1e8] = 0x22;
            outRom[0x8e1e9] = (byte)(subr58c8_newLoc);
            outRom[0x8e1ea] = (byte)(subr58c8_newLoc >> 8);
            outRom[0x8e1eb] = (byte)((subr58c8_newLoc >> 16) + 0xC0);

            return true;
        }
    }
}
