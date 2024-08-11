using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Swap weapon orb rewards for vanilla rando or open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OrbSwaps : RandoProcessor
    {
        protected override string getName()
        {
            return "Weapon orb reward swaps";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            if (randoMode == VanillaRandoSettings.MODE_KEY)
            {
                processVanillaRando(outRom, context.randomFunctional);
            }
            else if (randoMode == OpenWorldSettings.MODE_KEY)
            {
                processOpenWorld(outRom, context.randomFunctional, context.replacementEvents);
            }
            else
            {
                Logging.log("Unsupported mode for weapon orb swap hack");
                return false;
            }
            return true;
        }

        public void processVanillaRando(byte[] rom, Random r)
        {
            // skip 1 - sword
            List<int> orbNums = new int[] { 0, 2, 3, 4, 5, 6, 7 }.ToList();

            Dictionary<int, int> randoOrbs = new Dictionary<int, int>();

            randoOrbs[1] = 1;

            int oldId = 0;
            while(orbNums.Count > 0)
            {
                int newId = r.Next() % orbNums.Count;
                randoOrbs[oldId] = orbNums[newId];
                oldId++;
                if(oldId == 1)
                {
                    oldId++;
                }
                orbNums.RemoveAt(newId);
            }

            List<int> offsets = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                offsets.Add(VanillaEventUtil.getEventStartOffset(rom, 0x500 + i));
            }

            // swap offsets
            for (int i = 0; i < 8; i++)
            {
                rom[0xA0000 + (i + 0x100) * 2] = (byte)offsets[randoOrbs[i]];
                rom[0xA0000 + (i + 0x100) * 2 + 1] = (byte)(offsets[randoOrbs[i]]>>8);
            }

            // swap visibility flags for chests
            for (int mapNum = 16; mapNum < 500; mapNum++)
            {
                int objsOffset1 = 0x80000 + rom[0x87000 + mapNum * 2] + (rom[0x87000 + mapNum * 2 + 1] << 8);
                int objsOffset2 = 0x80000 + rom[0x87000 + mapNum * 2 + 2] + (rom[0x87000 + mapNum * 2 + 3] << 8);
                objsOffset1 += 8; // skip map header
                while (objsOffset1 < objsOffset2)
                {
                    int objVisFlag = rom[objsOffset1 + 0];
                    if (objVisFlag >= 0xB8 && objVisFlag <= 0xBF)
                    {
                        rom[objsOffset1 + 0] = (byte)(randoOrbs[objVisFlag - 0xB8] + 0xB8);
                    }

                    objsOffset1 += 8;
                }
            }
        }

        public void processOpenWorld(byte[] rom, Random r, Dictionary<int, List<byte>> replacementEvents)
        {
            // skip 1 - sword
            List<int> orbNums = new int[] { 0, 2, 3, 4, 5, 6, 7 }.ToList();

            Dictionary<int, int> randoOrbs = new Dictionary<int, int>();

            randoOrbs[1] = 1;

            int oldId = 0;
            while (orbNums.Count > 0)
            {
                int newId = r.Next() % orbNums.Count;
                randoOrbs[oldId] = orbNums[newId];
                oldId++;
                if (oldId == 1)
                {
                    oldId++;
                }
                orbNums.RemoveAt(newId);
            }

            // reconstruct events
            for(int i=0; i < 8; i++)
            {
                // modified version of this from OrbRewardFix
                byte flag1 = (byte)(0xb8 + randoOrbs[i]);
                byte flag2 = (byte)(0xc0 + randoOrbs[i]);
                List<byte> newEvent = new List<byte>();
                replacementEvents[0x500 + i] = newEvent;
                newEvent.Add(EventCommandEnum.EVENT_LOGIC.Value);
                newEvent.Add(flag1);
                newEvent.Add(0x08);
                newEvent.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                newEvent.Add(flag1);

                newEvent.Add(EventCommandEnum.EVENT_LOGIC.Value);
                newEvent.Add(flag2);
                newEvent.Add(0x08);
                newEvent.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                newEvent.Add(flag2);

                newEvent.Add(EventCommandEnum.OPEN_DIALOGUE.Value);
                newEvent.Add(0x7F); // enter
                newEvent.Add(0x59);
                newEvent.Add(weaponByteIndent(randoOrbs[i])); // text shift
                List<byte> dialogue = VanillaEventUtil.getBytes("Got " + SomVanillaValues.weaponByteToName(randoOrbs[i]));
                foreach (byte b in dialogue)
                {
                    newEvent.Add(b);
                }
                newEvent.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x05));
                newEvent.Add(0x09); // 's orb
                newEvent.Add(EventCommandEnum.END.Value);

            }

            // swap visibility flags for chests
            for (int mapNum = 16; mapNum < 500; mapNum++)
            {
                int objsOffset1 = 0x80000 + rom[0x87000 + mapNum * 2] + (rom[0x87000 + mapNum * 2 + 1] << 8);
                int objsOffset2 = 0x80000 + rom[0x87000 + mapNum * 2 + 2] + (rom[0x87000 + mapNum * 2 + 3] << 8);
                objsOffset1 += 8; // skip map header
                while (objsOffset1 < objsOffset2)
                {
                    int objVisFlag = rom[objsOffset1 + 0];
                    if (objVisFlag >= 0xB8 && objVisFlag <= 0xBF)
                    {
                        rom[objsOffset1 + 0] = (byte)(randoOrbs[objVisFlag - 0xB8] + 0xB8);
                    }

                    objsOffset1 += 8;
                }
            }
        }

        private byte weaponByteIndent(int weapon)
        {
            // centering for weapon orb message
            return (byte)(8 - SomVanillaValues.weaponByteToName(weapon).Length / 2);
        }
    }
}
