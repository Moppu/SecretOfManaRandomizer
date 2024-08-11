using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Randomize gear sold in shops.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ShopRandomizer : RandoProcessor
    {
        // 16 vanilla shops, in shop id order
        private string[] shopNames = new string[] 
        {
            "Potos",
            "Potos Area Neko",
            "Pandora Before Ruins",
            "Kippo",
            "Gaia's Navel Town",
            "Pandora After Ruins",
            "Upper Land Neko",
            "Matango",
            "Ice Country Todo/Neko",
            "Kakkara",
            "South Town",
            "North Town",
            "Mountains Neko",
            "Gold City",
            "Tasnica",
            "Lost Continent Neko",
        };
        protected override string getName()
        {
            return "Open world shop randomizer";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(OpenWorldSettings.PROPERTYNAME_RANDOMIZE_SHOPS))
            {
                return false;
            }

            new NoNekoOverflow().add(origRom, outRom, seed, settings, context);

            /*
                16 vanilla shops
                D8/FC52:	BABEBF 7C 91 FF
                D8/FC58:	BABBBCBDBEBFC4 7C 7D 7E B9 FF
                D8/FC64:	BABEBF 7D 92 B9 FF
                D8/FC6B:	BABEBF 7D 94 96 A7 FF
                D8/FC73:	BABEBF 7E 7F 93 94 95 B9 A7 A8 FF
                D8/FC7F:	BABEBF 7F 95 96 A7 A8 A9 FF
                D8/FC89:	BABBBDBEBF 7F 80 95 96 A8 A9 FF
                D8/FC95:	BABBBDBEBF 80 81 97 AA FF
                D8/FC9F:	BBBDBEBF 82 83 98 99 AB FF
                D8/FCA9:	BBBDBEBF 82 83 98 99 AB FF
                D8/FCB3:	BBBDBEBF 82 83 84 98 99 9A AB AC AD FF
                D8/FCC1:	BBBDBEBF 84 85 9A 9B 9C AB AC AD FF
                D8/FCCE:	BABBBDBEBF 86 9D AE FF
                D8/FCD7:	BBBDBEBF 88 89 8A 9F A0 B0 B1 FF
                D8/FCE3:	BABBBCBDBEBF 89 8A 9E A0 AF B1 FF
                D8/FCF0:	BABBBCBDBEBFC4 8B A1 B3 FF
             */

            // value BA or higher is consumable; randomize the others
            // 7c-8f hats (20)
            // 91-a4 armors - don't use 91,92,93 the starting armors (17)
            // a6-b9 rings (20)
            // 57 items total
            // above: 83 gear pieces in shops

            // 18fc32 - 16x 16-bit offsets

            List<int> armorPieceOffsets = new List<int>();
            List<byte> remainingChoices = new List<byte>();
            Dictionary<int, int> offsetToShopId = new Dictionary<int, int>();

            for (int i=0; i < 16; i++)
            {
                int shopOffset = 0x180000 + DataUtil.ushortFromBytes(outRom, 0x18fc32 + i * 2);
                int offset = shopOffset;
                byte shopVal = outRom[offset];
                int itemIndex = 0;
                // shop 10 (southtown) actually exceeds 12 items and doesn't show the last one
                while(shopVal != 0xFF && itemIndex < 12)
                {
                    if(shopVal < 0xBA)
                    {
                        armorPieceOffsets.Add(offset);
                        offsetToShopId[offset] = i;
                    }

                    offset++;
                    shopVal = outRom[offset];
                    itemIndex++;
                }
            }

            Random r = context.randomFunctional;
            foreach(int offset in armorPieceOffsets)
            {
                if(remainingChoices.Count == 0)
                {
                    remainingChoices.AddRange(getAllArmorChoices());
                }
                int index = r.Next() % remainingChoices.Count;
                byte gearValue = remainingChoices[index];
                outRom[offset] = gearValue;
                Logging.log("Shop rando at offset " + offset.ToString("X6") + " = " + gearValue.ToString("X2"), "debug");
                // name is in event 0x7FF + the item id
                string armorPieceName = VanillaEventUtil.getVanillaThingName(origRom, 0x7FF + gearValue);
                Logging.log("Shop " + offsetToShopId[offset] + " (" + shopNames[offsetToShopId[offset]] + ") has " + armorPieceName, "spoiler");
                remainingChoices.RemoveAt(index);
            }
            return true;
        }

        private List<byte> getAllArmorChoices()
        {
            List<byte> randomGearValues = new List<byte>();
            // hats
            for (int i = 0x7c; i <= 0x8f; i++)
            {
                randomGearValues.Add((byte)i);
            }
            // armors
            for (int i = 0x94; i <= 0xa4; i++)
            {
                randomGearValues.Add((byte)i);
            }
            // rings
            for (int i = 0xa6; i <= 0xb9; i++)
            {
                randomGearValues.Add((byte)i);
            }
            return randomGearValues;
        }
    }
}
