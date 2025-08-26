using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.util
{
    /// <summary>
    /// Change names of enemies, items, etc
    /// These are defined with the same text commands as events, but in their own block starting at 0x800 after the events 0x000 - 0x7ff
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NamesOfThings
    {
        // events 0x800 - 94E
        public static int INDEX_SPELLS_START = 0; // 0x800
        public static int INDEX_ELEMENTS_START = 0x2A; // 0x82A
        public static int INDEX_WEAPONS_START = 0x32; // 0x832
        public static int INDEX_ARMORS_START = 0x7A; // 0x87A
        public static int INDEX_CONSUMABLES_START = 0xBA; // 0x8BA
        public static int INDEX_MENU_START = 0xC6; // 0x8C6
        public static int INDEX_ENEMIES_START = 0xCF; // 0x8CF
        // 0x94E is final one .. 14F total

        private Dictionary<int, string> originalNames = new Dictionary<int, string>();
        private Dictionary<int, string> newNames = new Dictionary<int, string>();

        public NamesOfThings(byte[] rom)
        {
            for (int eventId = 0x800; eventId <= 0x94E; eventId++)
            {
                int itemId = eventId - 0x800;
                // read original offset
                int _eventOffsetOffset = 0xA0000 + (eventId - 0x400) * 2;
                int _eventOffset = DataUtil.ushortFromBytes(rom, _eventOffsetOffset) + 0xA0000;

                int eventOffsetOffset = 0xA0000 + (eventId - 0x400) * 2;

                byte eventValue = rom[_eventOffset];
                // transfer data
                originalNames[itemId] = "";
                while (eventValue != 0)
                {
                    originalNames[itemId] += VanillaEventUtil.getChar(eventValue);
                    _eventOffset++;
                    eventValue = rom[_eventOffset];
                }
            }
        }

        public void setName(int index, String name)
        {
            newNames[index] = name;
        }

        public string getOriginalName(int index)
        {
            return originalNames[index];
        }

        public void setAllNames(byte[] outRom, ref int newCodeOffset)
        {
            // if nothing set, do nothing
            if (newNames.Count > 0)
            {
                // this is for magic rando later; not used currently
                List<int> newSpellIds = new List<int>();
                foreach (int key in newNames.Keys)
                {
                    if (key >= 0x1000)
                    {
                        newSpellIds.Add(key);
                    }
                }

                // increment bank if close to end, for safety
                int newCodeOffset2 = newCodeOffset + 0x2000; // high estimate of total string length A0800 - A0A9C = 98E1->A6FA in original
                if ((newCodeOffset & 0xFFFF0000) != (newCodeOffset2 & 0xFFFF0000))
                {
                    newCodeOffset = (int)(newCodeOffset2 & 0xFFFF0000);
                }

                int newSpellNamesTableOffset = newCodeOffset;
                int newSpellNamesDataOffset = newCodeOffset + 0x2A * 2;

                if (newSpellIds.Count > 0)
                {
                    // custom spell stuff, if enabled
                    newCodeOffset = newSpellNamesDataOffset;
                    int i = 0;
                    for(int id = 0; id <= 0x29; id++)
                    {
                        int spellId = id + 0x1000;
                        List<byte> spellNameBytes;
                        if (newSpellIds.Contains(spellId))
                        {
                            int eventId = spellId - 0x1000;
                            Logging.log("Putting custom spell name " + eventId.ToString("X4") + " (" + newNames[spellId] + ") at " + newCodeOffset.ToString("X6"), "debug");
                            spellNameBytes = VanillaEventUtil.getBytes(newNames[spellId]);
                        }
                        else
                        {
                            spellNameBytes = VanillaEventUtil.getBytes("undefined");
                            int eventId = spellId - 0x1000;
                            Logging.log("Putting missing spell name " + eventId.ToString("X4") + " at " + newCodeOffset.ToString("X"), "debug");
                        }
                        ushort romOffset = (ushort)newCodeOffset;
                        // don't put in Axxxx header since these are new IDs; make 24 bit offsets
                        DataUtil.ushortToBytes(outRom, newSpellNamesTableOffset + i * 2, romOffset);
                        foreach (byte b in spellNameBytes)
                        {
                            outRom[newCodeOffset++] = b;
                        }
                        outRom[newCodeOffset++] = 0x00;
                        i++;
                    }

                    // compare index (in x) and overwrite A with the 16bit offset if needed, then rtl
                    // 1064, 1066, 1068...

                    // x here is 
                }

                // 0xA0000 indexed with event id - 0x400, 16 bit
                outRom[0x1931] = 0x22;
                outRom[0x1932] = (byte)(newCodeOffset);
                outRom[0x1933] = (byte)(newCodeOffset >> 8);
                outRom[0x1934] = (byte)((newCodeOffset >> 16) + 0xC0);

                // A9 C6       LDA #C6
                outRom[newCodeOffset++] = 0xA9;
                outRom[newCodeOffset++] = (byte)((newCodeOffset >> 16) + 0xC0);

                // 48          PHA
                outRom[newCodeOffset++] = 0x48;

                // AB          PLB
                outRom[newCodeOffset++] = 0xAB;

                if (newSpellIds.Count > 0)
                {
                    // CPX #1064
                    outRom[newCodeOffset++] = 0xE0;
                    outRom[newCodeOffset++] = 0x64;
                    outRom[newCodeOffset++] = 0x10;
                    // BGE newSpellHandling (1)
                    outRom[newCodeOffset++] = 0xB0;
                    outRom[newCodeOffset++] = 0x01;
                    // RTL
                    outRom[newCodeOffset++] = 0x6B;
                    // PHX
                    outRom[newCodeOffset++] = 0xDA;
                    // REP 20
                    outRom[newCodeOffset++] = 0xC2;
                    outRom[newCodeOffset++] = 0x20;
                    // TXA
                    outRom[newCodeOffset++] = 0x8A;
                    // SEC
                    outRom[newCodeOffset++] = 0x38;
                    // SBC #1064
                    outRom[newCodeOffset++] = 0xE9;
                    outRom[newCodeOffset++] = 0x64;
                    outRom[newCodeOffset++] = 0x10;
                    // TAX
                    outRom[newCodeOffset++] = 0xAA;
                    // LDA $spellIndexOffsets,x
                    outRom[newCodeOffset++] = 0xBF;
                    outRom[newCodeOffset++] = (byte)(newSpellNamesTableOffset);
                    outRom[newCodeOffset++] = (byte)(newSpellNamesTableOffset >> 8);
                    outRom[newCodeOffset++] = (byte)((newSpellNamesTableOffset >> 16) + 0xC0);
                    // $E4/44EB 8F 01 1D 00 STA $001D01[$00:1D01]   A:47F6 X:0860 Y:19D3 P:envmxdIzc
                    outRom[newCodeOffset++] = 0x8F;
                    outRom[newCodeOffset++] = 0x01;
                    outRom[newCodeOffset++] = 0x1D;
                    outRom[newCodeOffset++] = 0x00;
                    // $C0 / 192E A8 TAY                     A: 47F6 X: 0860 Y: 19D3 P: eNvmxdIzc
                    outRom[newCodeOffset++] = 0xA8;
                    // PLX
                    outRom[newCodeOffset++] = 0xFA;
                    // SEP 20
                    outRom[newCodeOffset++] = 0xE2;
                    outRom[newCodeOffset++] = 0x20;
                    // (rtl follows)
                }

                // 6B          RTL
                outRom[newCodeOffset++] = 0x6B;

                for (int eventId = 0x800; eventId <= 0x94E; eventId++)
                {
                    int itemId = eventId - 0x800;
                    if (newNames.ContainsKey(itemId))
                    {
                        Logging.log("Putting thing name " + eventId.ToString("X4") + "(" + newNames[itemId] + ") at " + newCodeOffset.ToString("X6"), "debug");
                        List<byte> weaponNameBytes = VanillaEventUtil.getBytes(newNames[itemId]);
                        ushort romOffset = (ushort)newCodeOffset;
                        // put in Axxxx header
                        DataUtil.ushortToBytes(outRom, 0xA0000 + (eventId - 0x400) * 2, romOffset);
                        foreach (byte b in weaponNameBytes)
                        {
                            outRom[newCodeOffset++] = b;
                        }
                        outRom[newCodeOffset++] = 0x00;
                    }
                    else
                    {
                        Logging.log("Putting thing name " + eventId.ToString("X4") + " at " + newCodeOffset.ToString("X6"), "debug");
                        // read original offset
                        int _eventOffsetOffset = 0xA0000 + (eventId - 0x400) * 2;
                        int _eventOffset = DataUtil.ushortFromBytes(outRom, _eventOffsetOffset) + 0xA0000;

                        // write new offset
                        ushort romOffset = (ushort)newCodeOffset;
                        // put in Axxxx header
                        DataUtil.ushortToBytes(outRom, 0xA0000 + (eventId - 0x400) * 2, romOffset);

                        byte eventValue = outRom[_eventOffset];
                        // transfer data
                        while (eventValue != 0)
                        {
                            outRom[newCodeOffset++] = eventValue;
                            _eventOffset++;
                            eventValue = outRom[_eventOffset];
                        }

                        // zero at the end
                        outRom[newCodeOffset++] = eventValue;
                    }
                }
            }
        }
    }
}
