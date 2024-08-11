using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// Randomize Neko shops for procgen modes.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ShopChanges : RandoProcessor
    {
        protected override string getName()
        {
            return "Shop changes for non-vanilla modes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;
            // take out neko price doubling
            outRom[0x7CEE] = 0xEA;
            outRom[0x7CEF] = 0xEA;
            outRom[0x7CF0] = 0xEA;

            // shop data
            // BA is candy (0x40 for drops .. 7A diff)
            // BE is med. herb
            // BF is cup of wishes
            // bandana is 7c (0x01 in drops .. 7B diff)
            // overalls is 91 (0x16 in drops .. 7B diff)
            // faerie ring, which for silly reasons is the first ring, is a6

            // base id for each shop
            byte[] hatIds = new byte[64];
            byte[] armorIds = new byte[64];
            byte[] accessoryIds = new byte[64];

            byte[] usefulUsables = new byte[] { 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF, 0xC4 };

            // in order
            // basically the same thing we do in DropChanges but with different item IDs
            for (int i = 0; i < 64; i++)
            {
                hatIds[i] = (byte)(0x7C + ((0x14 - 0x01 + 1) * i / 64.0));
                armorIds[i] = (byte)(0x91 + ((0x29 - 0x16 + 1) * i / 64.0));
                accessoryIds[i] = (byte)(0xA6 + ((0x3e - 0x2b + 1) * i / 64.0));
                // swap faerie ring and wrist band, which are swapped in vanilla
                if (accessoryIds[i] == 0xB9)
                {
                    accessoryIds[i] = 0xA6;
                }
                else if (accessoryIds[i] == 0xA6)
                {
                    accessoryIds[i] = 0xB9;
                }
                // overalls, kung fu suit, midge robe .. never drop since we start with them
                if (armorIds[i] == 0x91 || armorIds[i] == 0x92 || armorIds[i] == 0x93)
                {
                    // chain vest instead
                    armorIds[i] = 0x94;
                }
            }

            int numHatsInTable = 2;
            int numArmorsInTable = 2;
            int numAccInTable = 2;
            int numUsablesInTable = 3;

            // generate shops
            for (int i = 0; i < 16; i++)
            {
                List<int> shopItems = new List<int>();

                List<int> itemShifts = new List<int>();
                // easy: -3 -> 12
                int spread = 12;
                int lowLimit = 3;
                for (int id = 0; id < spread; id++)
                {
                    for (int j = id; j <= spread; j++)
                    {
                        int val1 = i * 2 + j;
                        if (val1 < 64)
                        {
                            itemShifts.Add(val1);
                        }
                        int val2 = i * 2 - j;
                        if (val2 >= 0 && j <= lowLimit)
                        {
                            itemShifts.Add(val2);
                        }
                    }
                }

                // hats
                for (int item = 0; item < numHatsInTable; item++)
                {
                    int hatIndex = 0;
                    bool ok = false;
                    int iter = 0;
                    while (!ok && iter < 10)
                    {
                        hatIndex = itemShifts[(r.Next() % itemShifts.Count)];
                        if (hatIndex < 0)
                        {
                            hatIndex = 0;
                        }
                        if (hatIndex > 63)
                        {
                            hatIndex = 63;
                        }
                        ok = !shopItems.Contains(hatIds[hatIndex]);
                        iter++;
                    }
                    if (ok)
                    {
                        shopItems.Add(hatIds[hatIndex]);
                    }
                }

                // armors
                for (int item = 0; item < numArmorsInTable; item++)
                {
                    bool ok = false;
                    int iter = 0;
                    int armorIndex = 0;
                    while (!ok && iter < 10)
                    {
                        armorIndex = itemShifts[(r.Next() % itemShifts.Count)];
                        if (armorIndex < 0)
                        {
                            armorIndex = 0;
                        }
                        if (armorIndex > 63)
                        {
                            armorIndex = 63;
                        }
                        ok = !shopItems.Contains(armorIds[armorIndex]);
                        iter++;
                    }
                    if (ok)
                    {
                        shopItems.Add(armorIds[armorIndex]);
                    }
                }

                for (int item = 0; item < numAccInTable; item++)
                {
                    bool ok = false;
                    int iter = 0;
                    int accIndex = 0;
                    while (!ok && iter < 10)
                    {
                        accIndex = itemShifts[(r.Next() % itemShifts.Count)];
                        if (accIndex < 0)
                        {
                            accIndex = 0;
                        }
                        if (accIndex > 63)
                        {
                            accIndex = 63;
                        }
                        ok = !shopItems.Contains(accessoryIds[accIndex]);
                        iter++;
                    }
                    if (ok)
                    {
                        shopItems.Add(accessoryIds[accIndex]);
                    }
                }

                for (int item = 0; item < numUsablesInTable; item++)
                {
                    int itemIndex = 0;
                    bool ok = false;
                    int iter = 0;
                    while(!ok && iter < 10)
                    {
                        itemIndex = usefulUsables[r.Next() % usefulUsables.Length];
                        ok = !shopItems.Contains(itemIndex);
                        iter++;
                    }
                    if (ok)
                    {
                        shopItems.Add(itemIndex);
                    }
                }

                // 11 bytes per; terminate with FF, 10 items
                int offset = 0xFC52 + i * 10;
                outRom[0x18FC32 + i * 2] = (byte)offset;
                outRom[0x18FC32 + i * 2 + 1] = (byte)(offset>>8);
                for(int j=0; j < 9; j++)
                {
                    if (j < shopItems.Count)
                    {
                        outRom[0x180000 + offset++] = (byte)shopItems[j];
                    }
                    else
                    {
                        outRom[0x180000 + offset++] = 0xFF;
                    }
                }
                outRom[0x180000 + offset++] = 0xFF;
            }
            
            return true;
        }
    }
}
