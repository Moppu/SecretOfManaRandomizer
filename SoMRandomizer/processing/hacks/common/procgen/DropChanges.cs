using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.ancientcave;
using SoMRandomizer.processing.chaos;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// Changes to how items are dropped to increase the chance of treasure chests and have them drop
    /// random floor-appropriate items as chosen from a new table.
    /// This is only used for procgen modes and is based on the "floor number."
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DropChanges : RandoProcessor
    {
        protected override string getName()
        {
            return "Enemy drop changes for non-vanilla modes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            int granularity = 1;
            int dropTableLocation = 0;
            if (randoMode == AncientCaveSettings.MODE_KEY)
            {
                int numMaps = AncientCaveGenerator.LENGTH_CONVERSIONS[settings.get(AncientCaveSettings.PROPERTYNAME_LENGTH)];
                granularity = (int)Math.Ceiling(24.0 / numMaps);
                dropTableLocation = -1;
            }
            else if (randoMode == BossRushSettings.MODE_KEY)
            {
                granularity = 1;
                dropTableLocation = -1;
            }
            else if (randoMode == ChaosSettings.MODE_KEY)
            {
                granularity = 1;
                dropTableLocation = context.workingData.getInt(ChaosRandomizer.DROP_TABLE_LOCATION);
            }
            else
            {
                Logging.log("Unsupported mode for ancient cave-style enemy drops");
                return false;
            }

            Random r = context.randomFunctional;
            int itemTableOffset = context.workingOffset;

            // drop tables
            // 64 possible drops per floor
            // [14] x01-x14 hats
            // [14] x16-x29 armor
            // [14] x2b-x3e accesories
            // [14] x40-x4b usables
            // [08] x80-x87 orbs

            // new code:
            // load floor num
            // >> 1
            // << 6
            // index into new table with that
            // add rng % 64
            // use that item

            // base id for each floor
            byte[] hatIds = new byte[64];
            byte[] armorIds = new byte[64];
            byte[] accessoryIds = new byte[64];

            // in order
            for (int i = 0; i < 64; i++)
            {
                hatIds[i] = (byte)(0x01 + ((0x14 - 0x01 + 1) * i / 64.0));
                armorIds[i] = (byte)(0x16 + ((0x29 - 0x16 + 1) * i / 64.0));
                accessoryIds[i] = (byte)(0x2b + ((0x3e - 0x2b + 1) * i / 64.0));
                // faerie ring and wrist band wtf
                if (accessoryIds[i] == 0x3E)
                {
                    accessoryIds[i] = 0x2B;
                }
                else if (accessoryIds[i] == 0x2B)
                {
                    accessoryIds[i] = 0x3E;
                }
                // overalls, kung fu suit, midge robe .. never drop since we start with them
                if(armorIds[i] == 0x16 || armorIds[i] == 0x17 || armorIds[i] == 0x18)
                {
                    // chain vest instead
                    armorIds[i] = 0x19;
                }
            }

            // should add up to 64.
            int numHatsInTable = 9;
            int numArmorsInTable = 9;
            int numAccInTable = 9;
            int numUsablesInTable = 21; // should be a multiple of 7
            int numOrbsInTable = 16; // should be a multiple of 8

            byte[] usefulUsables = new byte[] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x4A };
            byte[] usableIds = new byte[numUsablesInTable];
            // 00: Candy
            // 01: Chocolate
            // 02: Royal Jam
            // 03: Faerie Walnut
            // 04: Medical Herb
            // 05: Cup of Wishes
            // * 06: Magic Rope -> Stuck
            // * 07: Flammie Drum -> Change Music
            // * 08: Moogle Belt
            // * 09: Midge Mallet
            // 0A: Barrel
            // * 0B: ? (Dummied Out)
            for (int i = 0; i < numUsablesInTable; i++)
            {
                usableIds[i] = usefulUsables[i % usefulUsables.Length];
            }

            // chance of any on any floor
            byte[] orbIds = new byte[numOrbsInTable];
            for (int i = 0; i < numOrbsInTable; i++)
            {
                orbIds[i] = (byte)(0x80 + (i % 8));
            }

            // 64 floors
            for (int i = 0; i < 32; i++)
            {
                List<int> itemShifts = new List<int>();
                // easy: -3 -> 12
                int spread = 12;
                int lowLimit = 3;
                for (int id = 0; id < spread; id++)
                {
                    for (int j = id; j <= spread; j++)
                    {
                        int val1 = i * 2 * granularity + j;
                        if (val1 < 64)
                        {
                            itemShifts.Add(val1);
                        }
                        int val2 = i * 2 * granularity - j;
                        if (val2 >= 0 && j <= lowLimit)
                        {
                            itemShifts.Add(val2);
                        }
                    }
                }

                // hats
                for (int item = 0; item < numHatsInTable; item++)
                {
                    int hatIndex = itemShifts[(r.Next() % itemShifts.Count)];
                    if (hatIndex < 0)
                    {
                        hatIndex = 0;
                    }
                    if (hatIndex > 63)
                    {
                        hatIndex = 63;
                    }
                    outRom[itemTableOffset + i * 64 + item] = hatIds[hatIndex];
                }

                // armors
                for (int item = 0; item < numArmorsInTable; item++)
                {
                    int armorIndex = itemShifts[(r.Next() % itemShifts.Count)];
                    if (armorIndex < 0)
                    {
                        armorIndex = 0;
                    }
                    if (armorIndex > 63)
                    {
                        armorIndex = 63;
                    }
                    outRom[itemTableOffset + i * 64 + item + numHatsInTable] = armorIds[armorIndex];
                }

                for (int item = 0; item < numAccInTable; item++)
                {
                    int accIndex = itemShifts[(r.Next() % itemShifts.Count)];
                    if (accIndex < 0)
                    {
                        accIndex = 0;
                    }
                    if (accIndex > 63)
                    {
                        accIndex = 63;
                    }
                    outRom[itemTableOffset + i * 64 + item + numHatsInTable + numArmorsInTable] = accessoryIds[accIndex];
                }

                for (int item = 0; item < numUsablesInTable; item++)
                {
                    outRom[itemTableOffset + i * 64 + item + numHatsInTable + numArmorsInTable + numAccInTable] = usableIds[item];
                }
                for (int item = 0; item < numOrbsInTable; item++)
                {
                    outRom[itemTableOffset + i * 64 + item + numHatsInTable + numArmorsInTable + numAccInTable + numUsablesInTable] = orbIds[item];
                }
            }

            // item table done
            context.workingOffset += 32 * 64;

            // remove a block of vanilla code that reads drop tables to determine the item dropped
            // ...
            // $C8 / E156 BF 53 3A D0 LDA $D03A53,x[$D0:3A53] A:0FC0 X:0000 Y:0000 P:envMxdIzC - table read
            // $C8 / E15A 80 22       BRA $22[$E17E]          A:0F40 X:0000 Y:0000 P:envMxdIzC
            // ...
            // $C8 / E17E 85 A4       STA $A4[$00:03A4]       A:0F40 X:0000 Y:0000 P:envMxdIzC (keep - this ends up being the id of the drop)
            // $C8 / E180 C9 00       CMP #$00                A:0F40 X:0000 Y:0000 P:envMxdIzC

            for (int i = 0x8E12C; i < 0x8E17E; i++)
            {
                outRom[i] = 0xEA;
            }

            // 22 to thingy
            outRom[0x8E12C] = 0x22;
            outRom[0x8E12D] = (byte)(context.workingOffset);
            outRom[0x8E12E] = (byte)(context.workingOffset >> 8);
            outRom[0x8E12F] = (byte)((context.workingOffset >> 16) + 0xC0);

            // this was to fix a weird loading bug that sometimes triggered non-drop events/text when opening a chest
            // $C8/E11E 89 01       BIT #$01                A:0FAA X:00D7 Y:0000 P:envMxdIzC
            // $C8/E120 D0 E7       BNE $E7[$E109]          A:0FAA X:00D7 Y:0000 P:envMxdIZC -> change to 0x80 to branch always
            // $C8/E122 A6 85       LDX $85[$00:0385]       A:0FAA X:00D7 Y:0000 P:envMxdIZC

            outRom[0x8E120] = 0x80;
            
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7E00DC - map number
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            if (dropTableLocation < 0)
            {
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
            }

            if (dropTableLocation < 0)
            {
                // LSR - shift right since two maps per floor
                outRom[context.workingOffset++] = 0x4A;
            }
            else
            {
                // look up floorTableLocation first
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // LDA floorTableLocation,X
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = (byte)(dropTableLocation);
                outRom[context.workingOffset++] = (byte)(dropTableLocation >> 8);
                outRom[context.workingOffset++] = (byte)((dropTableLocation >> 16) + 0xC0);
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
            }

            // ASL 6x for * 64, the size of the drop table per floor
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // JSL C0389C - vanilla rng
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x9C;
            outRom[context.workingOffset++] = 0x38;
            outRom[context.workingOffset++] = 0xC0;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // AND #003F - rng value 0->3F (size of drop table)
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x3F;
            outRom[context.workingOffset++] = 0x00;
            // STA $A4 - write the drop index temporarily
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC $A4
            outRom[context.workingOffset++] = 0x65;
            outRom[context.workingOffset++] = 0xA4;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA tableOffset,x - load the value in the drop table
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)itemTableOffset;
            outRom[context.workingOffset++] = (byte)(itemTableOffset>>8);
            outRom[context.workingOffset++] = (byte)(0xC0 + (itemTableOffset >> 16));
            // 29 FF 00       AND 00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // RTL (then write to $A4 once we return, with vanilla code, to set the drop)
            outRom[context.workingOffset++] = 0x6B;

            // drop rates up for now
            for (int i = 0; i < 128; i++)
            {
                outRom[0x103A50 + i * 5 + 2] = 0xF8;
            }

            // change drops, more often
            outRom[0x428E] = 0x0F;

            // $C0/4292 BD CE E1    LDA $E1CE,x[$7E:E7CE]   A:0F06 X:0600 Y:0058 P:envMxdIzC
            // $C0 / 4295 85 A4 STA $A4[$00:03A4]   A: 0F84 X: 0600 Y: 0058 P: eNvMxdIzC
            // LDA #??   84 for fish, this did not work well

            // this is part of a check for current orb level to decide whether weapon orbs should be allowed to drop
            // change to lda #08 to just allow them to always drop.
            // $C8 / E1A9: B9C0CF   LDA $CFC0,Y

            // LDA #08
            outRom[0x8e1a9] = 0xa9;
            outRom[0x8e1aa] = 0x08;
            // NOP
            outRom[0x8e1ab] = 0xea;

            return true;
        }
    }
}
