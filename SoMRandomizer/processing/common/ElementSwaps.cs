using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.openworld;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.openworld.PlandoProperties;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing
{
    /// <summary>
    /// Hack to swap elements & orbs for vanilla rando, or randomize spell orbs for open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ElementSwaps : RandoProcessor
    {
        public const string ORBELEMENT_PREFIX = "orbElement";
        public const string VANILLARANDO_ELEMENTLIST = "vanillaRandoElementList";
        public static List<string> elementNames = new string[] { "Gnome", "Undine", "Salamando", "Sylphid", "Lumina", "Shade", "Luna", "Dryad" }.ToList();

        // keys into the crystal orb colors map - all the vanilla maps that have orbs on them
        public const int ORBMAP_MATANGO = MAPNUM_MATANGOCAVE_GNOME_ORB; // 307
        public const int ORBMAP_EARTHPALACE = MAPNUM_EARTHPALACE_ORB; // 291
        public const int ORBMAP_FIREPALACE3 = MAPNUM_FIREPALACE_UNDINE_ORB; // 240
        public const int ORBMAP_FIREPALACE1 = MAPNUM_FIREPALACE_FIRST_ORB; // 348
        public const int ORBMAP_FIREPALACE2 = MAPNUM_FIREPALACE_G; // 345
        public const int ORBMAP_LUNAPALACE = MAPNUM_LUNAPALACE_SPACE; // 35
        public const int ORBMAP_UPPERLAND = MAPNUM_UPPERLAND_SOUTHEAST; // 41
        public const int ORBMAP_GRANDPALACE1 = MAPNUM_GRANDPALACE_GNOME_ORB; // 420
        public const int ORBMAP_GRANDPALACE2 = MAPNUM_GRANDPALACE_UNDINE_ORB; // 421
        public const int ORBMAP_GRANDPALACE3 = MAPNUM_GRANDPALACE_SYLPHID_ORB; // 422
        public const int ORBMAP_GRANDPALACE4 = MAPNUM_GRANDPALACE_SALAMANDO_ORB; // 423
        public const int ORBMAP_GRANDPALACE5 = MAPNUM_GRANDPALACE_LUMINA_ORB; // 424
        public const int ORBMAP_GRANDPALACE6 = MAPNUM_GRANDPALACE_SHADE_ORB; // 425
        public const int ORBMAP_GRANDPALACE7 = MAPNUM_GRANDPALACE_LUNA_ORB; // 426
        public const int ORBMAP_GRANDPALACE_FIRST = ORBMAP_GRANDPALACE1; // 420
        public const int ORBMAP_GRANDPALACE_LAST = ORBMAP_GRANDPALACE7; // 426

        protected override string getName()
        {
            return "Spell orb element randomizer";
        }

        public static Dictionary<int, byte> getCrystalOrbElementMap(RandoContext context)
        {
            List<string> elementOrbSettings = context.workingData.keysStartingWith(ORBELEMENT_PREFIX);
            Dictionary<int, byte> elementValuesByMapNum = new Dictionary<int, byte>();
            foreach (string key in elementOrbSettings)
            {
                int mapNum = Int32.Parse(key.Replace(ElementSwaps.ORBELEMENT_PREFIX, ""));
                byte elementValue = (byte)context.workingData.getInt(key);
                elementValuesByMapNum[mapNum] = elementValue;
            }
            return elementValuesByMapNum;
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            if (randoMode == VanillaRandoSettings.MODE_KEY)
            {
                List<int> elementList = process(outRom, context.randomFunctional, !settings.getBool(VanillaRandoSettings.PROPERTYNAME_DIALOGUE_CUTS));
                Logging.log("Element randomization:", "spoiler");
                for(int i=0; i < 8; i++)
                {
                    Logging.log("  " + elementNames[i] + " => " + elementNames[elementList[i]], "spoiler");
                }
                context.workingData.setIntArray(VANILLARANDO_ELEMENTLIST, elementList.ToArray());
            }
            else if (randoMode == OpenWorldSettings.MODE_KEY)
            {
                bool girlMagicExists = context.workingData.getBool(OpenWorldClassSelection.GIRL_MAGIC_EXISTS);
                bool spriteMagicExists = context.workingData.getBool(OpenWorldClassSelection.SPRITE_MAGIC_EXISTS);
                Dictionary<int, byte> crystalOrbElementMap = new Dictionary<int, byte>();
                bool randomizeGrandPalace = settings.getBool(OpenWorldSettings.PROPERTYNAME_RANDOMIZE_GRANDPALACE_ELEMENTS);
                bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
                processOpenWorld(outRom, context.randomFunctional, crystalOrbElementMap, context.replacementEvents, girlMagicExists, spriteMagicExists, randomizeGrandPalace, context.plandoSettings, flammieDrumInLogic);
                foreach(int mapNum in crystalOrbElementMap.Keys)
                {
                    context.workingData.setInt(ORBELEMENT_PREFIX + mapNum, crystalOrbElementMap[mapNum]);
                }
            }
            else
            {
                Logging.log("Unsupported mode for element randomizer");
                return false;
            }
            return true;
        }

        // for vanilla rando - randomly swap vanilla element locations & associated orbs
        public List<int> process(byte[] rom, Random r, bool modifyExistingEvents)
        {
            // locations of spell NPCs to randomize
            Dictionary<int, List<int>> mapNums = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> objNums = new Dictionary<int, List<int>>();

            // gnome
            mapNums[0] = new int[] { MAPNUM_EARTHPALACE_INTERIOR_C, MAPNUM_EARTHSEED }.ToList();
            objNums[0] = new int[] { 0, 2 }.ToList();
            // undine
            mapNums[1] = new int[] { MAPNUM_UNDINE }.ToList();
            objNums[1] = new int[] { 0 }.ToList();
            // salamando
            mapNums[2] = new int[] { MAPNUM_ICECOUNTRY_PARADISETOWN_WARM }.ToList();
            objNums[2] = new int[] { 0 }.ToList();
            // sylphid
            mapNums[3] = new int[] { MAPNUM_WINDSEED }.ToList();
            objNums[3] = new int[] { 2 }.ToList();
            // lumina
            mapNums[4] = new int[] { MAPNUM_LIGHTSEED }.ToList();
            objNums[4] = new int[] { 2 }.ToList();
            // shade
            mapNums[5] = new int[] { MAPNUM_DARKSEED }.ToList();
            objNums[5] = new int[] { 2 }.ToList();
            // luna
            mapNums[6] = new int[] { MAPNUM_MOONSEED }.ToList();
            objNums[6] = new int[] { 2 }.ToList();
            // dryad
            mapNums[7] = new int[] { MAPNUM_DRYADSEED }.ToList();
            objNums[7] = new int[] { 2 }.ToList();

            List<int> originalEles = new List<int>();
            for(int i=0; i < 8; i++)
            {
                originalEles.Add(i);
            }

            List<int> newEles = new List<int>();
            while(originalEles.Count > 0)
            {
                int id = r.Next() % originalEles.Count;
                newEles.Add(originalEles[id]);
                originalEles.RemoveAt(id);
            }

            // change npcs
            // map 278 undine obj 0 npc 0x11 (+0x80)
            // map 314 gnome obj 0 npc 0x10; map 276 obj 2 also
            // map 273 sylphid obj 2 npc 0x13
            // map 194 salamando obj 0 npc 0x12
            // map 120 lumina obj 2 npc 0x14
            // map 375 shade obj 2 npc 0x15
            // map 351 luna obj 2 npc 0x16
            // map 431 dryad obj 2 npc 0x17

            for(int i=0; i < 8; i++)
            {
                for (int j = 0; j < mapNums[i].Count; j++)
                {
                    int objsOffset = 0x80000 + rom[0x87000 + mapNums[i][j] * 2] + (rom[0x87000 + mapNums[i][j] * 2 + 1] << 8);
                    objsOffset += 8; // skip header
                    objsOffset += objNums[i][j] * 8; // skip other objs
                    rom[objsOffset + 5] = (byte)(newEles[i] + 0x90); // set npc graphic
                }
            }

            // change orb spells

            // gnome orbs [350]:
            // * 27e [matango earth slide], 570 [mana palace]
            // undine orbs [351]:
            // * 239 [entrance to earth palace], 2c8 [fire palace special message, no clear], 571 [mana palace], * 6ce [fire palace]
            // sylphid orbs [352]:
            // * 25a [upper land], 572 [mana palace]
            // salamando orbs [353]:
            // * 2c8 [fire palace entrance], 573 [mana palace], * 6cb [fire palace]
            // lumina orbs [354]:
            // * 2c4 [luna palace], 574 [mana palace]
            // shade orbs [355]:
            // 575 [mana palace]
            // luna orbs [356]:
            // 576 [mana palace]
            // dryad orbs [357]:
            // 577 [mana palace]

            // why is every list of elements in a slightly different order
            int[] eventConversions = new int[] { 0, 1, 3, 2, 4, 5, 6, 7 };
            // event flags 98, 99, ...
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x27e) + 1] = (byte)(0x50 + eventConversions[newEles[0]]); // gnome orb in matango cave
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x27e) + 8] = (byte)(0x98 + eventConversions[newEles[0]]);
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x27e) + 9] = 0x13;

            rom[VanillaEventUtil.getEventStartOffset(rom, 0x239) + 1] = (byte)(0x50 + eventConversions[newEles[1]]); // undine orb in gaia's navel
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x239) + 8] = (byte)(0x98 + eventConversions[newEles[1]]);
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x239) + 9] = 0x13;

            rom[VanillaEventUtil.getEventStartOffset(rom, 0x6ce) + 1] = (byte)(0x50 + eventConversions[newEles[1]]); // undine orb in fire palace
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x6ce) + 3] = (byte)(0x98 + eventConversions[newEles[1]]);
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x6ce) + 4] = 0x13;

            rom[VanillaEventUtil.getEventStartOffset(rom, 0x25a) + 1] = (byte)(0x50 + eventConversions[newEles[3]]); // sylphid orb in upper land
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x25a) + 3] = (byte)(0x98 + eventConversions[newEles[3]]);
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x25a) + 4] = 0x13;

            rom[VanillaEventUtil.getEventStartOffset(rom, 0x2c8) + 1] = (byte)(0x50 + eventConversions[newEles[2]]); // salamando orb in fire palace entrance
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x2c8) + 3] = (byte)(0x98 + eventConversions[newEles[2]]);
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x2c8) + 4] = 0x13;

            rom[VanillaEventUtil.getEventStartOffset(rom, 0x6cb) + 1] = (byte)(0x50 + eventConversions[newEles[2]]); // salamando orb in fire palace
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x6cb) + 3] = (byte)(0x98 + eventConversions[newEles[2]]);
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x6cb) + 4] = 0x13;

            rom[VanillaEventUtil.getEventStartOffset(rom, 0x2c4) + 1] = (byte)(0x50 + eventConversions[newEles[4]]); // lumina orb in luna palace
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x2c4) + 3] = (byte)(0x98 + eventConversions[newEles[4]]);
            rom[VanillaEventUtil.getEventStartOffset(rom, 0x2c4) + 4] = 0x13;

            // if not generating new events for dialogue shortening, inject randomized elements' spells granted at each element location into existing events.
            if (modifyExistingEvents)
            {
                // 0x38 for girl spells, 0x07 for sprite spells
                // if dryad, 0x18 and 0x03
                // if shade/lumina, 00 for the char who doesn't get it
                List<int> attributeValues = new int[] { 0xC8, 0xC9, 0xCA, 0xCB, 0xCF, 0xCE, 0xCC, 0xCD }.ToList(); // gnome, undine, sala, sylph, lumina, shade, luna, dryad
                // events 0x580 - 0x587 to grant spells to characters
                List<int> girlOffsets = new int[] { 0xA6B90, 0xA6E35, 0xA6F60, 0xA6F6A, 0xA7218, 0xA708C, 0xA6F9D, 0xA7063, }.ToList();
                List<int> spriteOffsets = new int[] { 0xA6B94, 0xA6E39, 0xA6F64, 0xA6F6E, 0xA721C, 0xA7090, 0xA6FA1, 0xA7067, }.ToList();

                for (int i = 0; i < 8; i++)
                {
                    int newEle = eventConversions[newEles[i]];
                    int newAttribute = attributeValues[newEle];
                    rom[girlOffsets[i]] = (byte)attributeValues[newEle];
                    rom[spriteOffsets[i]] = (byte)attributeValues[newEle];

                    if (newEle == 4)
                    {
                        // lumina
                        rom[girlOffsets[i] + 1] = 0x38;
                        rom[spriteOffsets[i] + 1] = 0;
                    }
                    else if(newEle == 5)
                    {
                        // shade
                        rom[girlOffsets[i] + 1] = 0;
                        rom[spriteOffsets[i] + 1] = 0x07;
                    }
                    else if(newEle == 7)
                    {
                        // dryad
                        rom[girlOffsets[i] + 1] = 0x18;
                        rom[spriteOffsets[i] + 1] = 0x03;
                    }
                    else
                    {
                        // normal
                        rom[girlOffsets[i] + 1] = 0x38;
                        rom[spriteOffsets[i] + 1] = 0x07;
                    }
                }
            }

            return newEles;
        }

        // for open world, set the orb elements to whatever was randomized for them.
        // don't swap spell rewards to match like in vanilla rando; randomized prize locations & logic will determine a new path through them
        public void processOpenWorld(byte[] rom, Random r, Dictionary<int, byte> crystalOrbColorMap, Dictionary<int, List<byte>> replacementEvents, bool girlExists, bool spriteExists, bool randomizeGrandPalace, Dictionary<string, List<string>> plando, bool flammieDrumInLogic)
        {
            // note that lumina and sylphid seem like they've been swapped in vanilla and no one ever noticed?
            // 358 gnome = 81
            // 359 undine = 82
            // 35a sylphid = 84
            // 35b salamando = 83
            // 35c lumina = 85
            // 35d shade = 86
            // 35e luna = 87
            // 35f dryad = 88

            // x81->x350 = gnome
            // x82->x351 = undine
            // x83->x353 = salamando
            // x84->x354 = lumina
            // x85->x352 = sylphid
            // x86->x355 = shade
            // x87->x356 = luna
            // x88->x357 = dryad
            List<byte> orbElementsAvailable = GetValidOrbElements(girlExists, spriteExists);

            // boy only - remove these switches in event modifier and just set them all to none
            if(orbElementsAvailable.Count == 0)
            {
                orbElementsAvailable.Add(0xFF);
            }

            // gnome orb to open matango cave
            if (plando.ContainsKey(KEY_MATANGO_ORB_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_MATANGO] = SomVanillaValues.elementOrbNameToByte(plando[KEY_MATANGO_ORB_ELEMENT][0]);
            }
            else
            {
                crystalOrbColorMap[ORBMAP_MATANGO] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
            }

            // undine orb to open earth palace
            if (plando.ContainsKey(KEY_EARTH_PALACE_ORB_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_EARTHPALACE] = SomVanillaValues.elementOrbNameToByte(plando[KEY_EARTH_PALACE_ORB_ELEMENT][0]);
            }
            else
            {
                crystalOrbColorMap[ORBMAP_EARTHPALACE] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
            }

            // undine orb to open end of fire palace
            if (plando.ContainsKey(KEY_FIRE_PALACE_ORB_3_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_FIREPALACE3] = SomVanillaValues.elementOrbNameToByte(plando[KEY_FIRE_PALACE_ORB_3_ELEMENT][0]);
            }
            else
            {
                crystalOrbColorMap[ORBMAP_FIREPALACE3] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
            }

            // salamando orb to open fire palace -- should remove palette change here so we can see what fucking color it is
            // ^^ OpenWorldSupportingEvents does this now; event 0x63d
            if (plando.ContainsKey(KEY_FIRE_PALACE_ORB_1_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_FIREPALACE1] = SomVanillaValues.elementOrbNameToByte(plando[KEY_FIRE_PALACE_ORB_1_ELEMENT][0]);
            }
            else
            {
                crystalOrbColorMap[ORBMAP_FIREPALACE1] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
            }

            // salamando orb to continue through fire palace
            if (plando.ContainsKey(KEY_FIRE_PALACE_ORB_2_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_FIREPALACE2] = SomVanillaValues.elementOrbNameToByte(plando[KEY_FIRE_PALACE_ORB_2_ELEMENT][0]);
            }
            else
            {
                crystalOrbColorMap[ORBMAP_FIREPALACE2] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
            }

            // lumina orb to get through luna palace
            if (plando.ContainsKey(KEY_LUNA_PALACE_ORB_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_LUNAPALACE] = SomVanillaValues.elementOrbNameToByte(plando[KEY_LUNA_PALACE_ORB_ELEMENT][0]);
            }
            else
            {
                crystalOrbColorMap[ORBMAP_LUNAPALACE] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
            }

            // sunken continent orbs in the 8 colored rooms; these will likely not be randomized
            crystalOrbColorMap[ORBMAP_GRANDPALACE1] = (byte)(spriteExists ? 0x81 : 0x84); // gnome
            crystalOrbColorMap[ORBMAP_GRANDPALACE2] = (byte)(spriteExists ? 0x82 : 0x84); // undine
            crystalOrbColorMap[ORBMAP_GRANDPALACE3] = (byte)(0x85); // sylphid
            crystalOrbColorMap[ORBMAP_GRANDPALACE4] = (byte)(0x83); // sala
            crystalOrbColorMap[ORBMAP_GRANDPALACE5] = (byte)(girlExists ? 0x84 : 0x86); // lumina
            crystalOrbColorMap[ORBMAP_GRANDPALACE6] = (byte)(spriteExists ? 0x86 : 0x84); // shade
            crystalOrbColorMap[ORBMAP_GRANDPALACE7] = (byte)(spriteExists ? 0x87 : 0x84); // luna

            // sylphid orb in upper land overworld
            crystalOrbColorMap[ORBMAP_UPPERLAND] = 0xFF;
            if (flammieDrumInLogic)
            {
                if (plando.ContainsKey(KEY_UPPER_LAND_ORB_ELEMENT))
                {
                    crystalOrbColorMap[ORBMAP_UPPERLAND] = SomVanillaValues.elementOrbNameToByte(plando[KEY_UPPER_LAND_ORB_ELEMENT][0]);
                }
                else
                {
                    // upper land one
                    crystalOrbColorMap[ORBMAP_UPPERLAND] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
                }
            }

            if(plando.ContainsKey(KEY_GRAND_PALACE_ORB_1_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_GRANDPALACE1] = SomVanillaValues.elementOrbNameToByte(plando[KEY_GRAND_PALACE_ORB_1_ELEMENT][0]);
            }
            if (plando.ContainsKey(KEY_GRAND_PALACE_ORB_2_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_GRANDPALACE2] = SomVanillaValues.elementOrbNameToByte(plando[KEY_GRAND_PALACE_ORB_2_ELEMENT][0]);
            }
            if (plando.ContainsKey(KEY_GRAND_PALACE_ORB_3_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_GRANDPALACE3] = SomVanillaValues.elementOrbNameToByte(plando[KEY_GRAND_PALACE_ORB_3_ELEMENT][0]);
            }
            if (plando.ContainsKey(KEY_GRAND_PALACE_ORB_4_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_GRANDPALACE4] = SomVanillaValues.elementOrbNameToByte(plando[KEY_GRAND_PALACE_ORB_4_ELEMENT][0]);
            }
            if (plando.ContainsKey(KEY_GRAND_PALACE_ORB_5_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_GRANDPALACE5] = SomVanillaValues.elementOrbNameToByte(plando[KEY_GRAND_PALACE_ORB_5_ELEMENT][0]);
            }
            if (plando.ContainsKey(KEY_GRAND_PALACE_ORB_6_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_GRANDPALACE6] = SomVanillaValues.elementOrbNameToByte(plando[KEY_GRAND_PALACE_ORB_6_ELEMENT][0]);
            }
            if (plando.ContainsKey(KEY_GRAND_PALACE_ORB_7_ELEMENT))
            {
                crystalOrbColorMap[ORBMAP_GRANDPALACE7] = SomVanillaValues.elementOrbNameToByte(plando[KEY_GRAND_PALACE_ORB_7_ELEMENT][0]);
            }

            if (randomizeGrandPalace)
            {
                Dictionary<byte, byte> palSets = new Dictionary<byte, byte>();
                palSets[0x81] = 89;
                palSets[0x82] = 88;
                palSets[0x83] = 91;
                palSets[0x84] = 93;
                palSets[0x85] = 47;
                palSets[0x86] = 92;
                palSets[0x87] = 95;
                palSets[0x88] = 97;
                palSets[0xFF] = 0xFF;
                
                if (!plando.ContainsKey(KEY_GRAND_PALACE_ORB_1_ELEMENT))
                {
                    crystalOrbColorMap[ORBMAP_GRANDPALACE1] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
                }
                rom[0x8DD19] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE1]]);

                if (!plando.ContainsKey(KEY_GRAND_PALACE_ORB_2_ELEMENT))
                {
                    crystalOrbColorMap[ORBMAP_GRANDPALACE2] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
                }
                rom[0x8DD41] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE2]]);

                if (!plando.ContainsKey(KEY_GRAND_PALACE_ORB_3_ELEMENT))
                {
                    crystalOrbColorMap[ORBMAP_GRANDPALACE3] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
                }
                rom[0x8DD69] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE3]]);

                if (!plando.ContainsKey(KEY_GRAND_PALACE_ORB_4_ELEMENT))
                {
                    crystalOrbColorMap[ORBMAP_GRANDPALACE4] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
                }
                rom[0x8DD91] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE4]]);

                if (!plando.ContainsKey(KEY_GRAND_PALACE_ORB_5_ELEMENT))
                {
                    crystalOrbColorMap[ORBMAP_GRANDPALACE5] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
                }
                rom[0x8DDB9] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE5]]);

                if (!plando.ContainsKey(KEY_GRAND_PALACE_ORB_6_ELEMENT))
                {
                    crystalOrbColorMap[ORBMAP_GRANDPALACE6] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
                }
                rom[0x8DDE1] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE6]]);

                if (!plando.ContainsKey(KEY_GRAND_PALACE_ORB_7_ELEMENT))
                {
                    crystalOrbColorMap[ORBMAP_GRANDPALACE7] = orbElementsAvailable[(r.Next() % orbElementsAvailable.Count)];
                }
                rom[0x8DE09] = (byte)(0x80 + palSets[crystalOrbColorMap[ORBMAP_GRANDPALACE7]]);
            }

            // x81->x350 = gnome
            // x82->x351 = undine
            // x83->x353 = salamando
            // x84->x354 = lumina
            // x85->x352 = sylphid
            // x86->x355 = shade
            // x87->x356 = luna
            // x88->x357 = dryad
            int[] eventConversions = new int[] { 0, 1, 3, 4, 2, 5, 6, 7 };


            // event types 4E and 49
            // they check analyzer in there too; should be able to use that as a template
            int spellId = 0;
            int[] spellIds = new int[] {
                0, 1, 2, 3, 4, 5, // gnome
                6, 7, 8, 9, 10, 11, // undine
                18, 19, 20, 21, 22, 23, // sylphid
                12, 13, 14, 15, 16, 17, // salamando
                39, 40, 41, // lumina
                36, 37, 38, // shade
                24, 25, 26, 27, 28, 29, // luna
                30, 31, 32, 33, 34, 35, // dryad
            };
            for(int i=0; i < 8; i++)
            {
                EventScript ev = new EventScript();
                replacementEvents[0x350 + i] = ev;
                // orb animation
                ev.Add(EventCommandEnum.CHARACTER_ANIM.Value);
                ev.Add(0x04);
                ev.Add(0x80);
                // wait for animation
                ev.Add(EventCommandEnum.WAIT_FOR_ANIM.Value);

                // set flag to 1 initially
                ev.SetFlag((byte)(0x98 + i), 1);
                // break out if acceptable spells
                int max = 6;
                if(i == 4 || i == 5)
                {
                    // shade, lumina
                    max = 3;
                }

                for (int j=0; j < max; j++)
                {
                    // allow selected (all) spells
                    ev.Add(0x4E);
                    ev.Add(0x04);
                    ev.Add(0x81);
                    ev.Add((byte)spellIds[spellId]);
                    ev.Add(0x02);
                    ev.Add(0x01);
                    spellId++;
                }

                // set flag to 0, didn't find correct spell
                ev.SetFlag((byte)(0x98 + i), 0);
                ev.Return();
                ev.End();
            }

            // gnome orb; matango cave
            if (crystalOrbColorMap[ORBMAP_MATANGO] != 0xFF)
            {
                EventScript ev27e = new EventScript();
                replacementEvents[0x27e] = ev27e;
                ev27e.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_MATANGO] - 0x81]); // change to check element we want
                ev27e.Logic(EventFlags.MATANGO_PROGRESS_FLAG, 0x4, 0xF, EventScript.GetJumpCmd(0));
                ev27e.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_MATANGO] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x27c)); // change to any spell acceptable (1-3)
                ev27e.Jsr(0x799); // idk sound maybe
                ev27e.End();
            }

            // undine orb; gaia's navel
            if (crystalOrbColorMap[ORBMAP_EARTHPALACE] != 0xFF)
            {
                EventScript ev239 = new EventScript();
                replacementEvents[0x239] = ev239;
                ev239.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_EARTHPALACE] - 0x81]); // change to check element we want
                ev239.Logic(EventFlags.EARTHPALACE_FLAG, 0x2, 0xF, EventScript.GetJumpCmd(0));
                ev239.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_EARTHPALACE] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x38)); // change to any spell acceptable (1-3)
                ev239.End();
            }

            // undine orb; end of fire palace
            if (crystalOrbColorMap[ORBMAP_FIREPALACE3] != 0xFF)
            {
                EventScript ev6ce = new EventScript();
                replacementEvents[0x6ce] = ev6ce;
                ev6ce.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE3] - 0x81]); // change to check element we want
                ev6ce.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE3] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x6c7)); // change to any spell acceptable (1-3)
                ev6ce.End();
            }

            // fire palace entrance salamando orb
            if (crystalOrbColorMap[ORBMAP_FIREPALACE1] != 0xFF)
            {
                EventScript ev2c8 = new EventScript();
                replacementEvents[0x2c8] = ev2c8;
                ev2c8.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE1] - 0x81]); // change to check element we want
                ev2c8.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE1] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x2c9)); // change to any spell acceptable (1-3)
                ev2c8.End();
            }

            // salamando orb; middle of fire palace
            if (crystalOrbColorMap[ORBMAP_FIREPALACE2] != 0xFF)
            {
                EventScript ev6cb = new EventScript();
                replacementEvents[0x6cb] = ev6cb;
                ev6cb.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE2] - 0x81]); // change to check element we want
                ev6cb.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_FIREPALACE2] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x6ca)); // change to any spell acceptable (1-3)
                ev6cb.End();
            }

            // lumina orb; luna palace
            if (crystalOrbColorMap[ORBMAP_LUNAPALACE] != 0xFF)
            {
                EventScript ev2c4 = new EventScript();
                replacementEvents[0x2c4] = ev2c4;
                ev2c4.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_LUNAPALACE] - 0x81]); // change to check element we want
                ev2c4.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_LUNAPALACE] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x2c5)); // change to any spell acceptable (1-3)
                ev2c4.End();
            }

            // 25a - upper land; sylphid in vanilla
            if (flammieDrumInLogic)
            {
                if (crystalOrbColorMap[ORBMAP_UPPERLAND] != 0xFF)
                {
                    EventScript ev25a = new EventScript();
                    replacementEvents[0x25a] = ev25a;
                    ev25a.Jsr(0x350 + eventConversions[crystalOrbColorMap[ORBMAP_UPPERLAND] - 0x81]); // change to check element we want
                    ev25a.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[ORBMAP_UPPERLAND] - 0x81]), 0x1, 0x3, EventScript.GetJumpCmd(0x25b)); // change to any spell acceptable (1-3)
                    ev25a.End();
                }
            }

            // set the lost continent ones
            if (!spriteExists || !girlExists || randomizeGrandPalace)
            {
                for(int mapId = ORBMAP_GRANDPALACE_FIRST; mapId <= ORBMAP_GRANDPALACE_LAST; mapId++) // skip dryad map, which is actually 419
                {
                    if (crystalOrbColorMap[mapId] != 0xFF)
                    {
                        EventScript ev = new EventScript();
                        replacementEvents[0x570 + (mapId - ORBMAP_GRANDPALACE_FIRST)] = ev;
                        ev.Jsr(0x350 + eventConversions[crystalOrbColorMap[mapId] - 0x81]); // change to check element we want
                        ev.Logic((byte)(0x98 + eventConversions[crystalOrbColorMap[mapId] - 0x81]), 0x0, 0x0, EventScript.GetJumpCmd(0)); // skip if any spell of right element cast
                        ev.IncrFlag((byte)(0xE8 + (mapId - ORBMAP_GRANDPALACE_FIRST))); // change to set element we want
                        ev.Jump(0x578);
                        ev.End();
                    }
                }
            }

            // map header[1] & 7F = palette
            // lumina palette = 93, shade palette = 92
            else if(!spriteExists && girlExists)
            {
                // set all the palettes to lumina
                for (int mapId = ORBMAP_GRANDPALACE_FIRST; mapId <= ORBMAP_GRANDPALACE_LAST; mapId++) // skip dryad map, which is actually 419
                {
                    int mapObjOffset = 0x80000 + rom[0x87000 + mapId * 2] + (rom[0x87000 + mapId * 2 + 1] << 8);
                    bool msb = (rom[mapObjOffset + 1] & 0x80) > 0;
                    rom[mapObjOffset + 1] = 93;
                    if(msb)
                    {
                        rom[mapObjOffset + 1] |= 0x80;
                    }
                }
            }

            else if (spriteExists && !girlExists)
            {
                // set the lumina palette to shade
                int mapId = MAPNUM_GRANDPALACE_LUMINA_ORB;
                int mapObjOffset = 0x80000 + rom[0x87000 + mapId * 2] + (rom[0x87000 + mapId * 2 + 1] << 8);
                bool msb = (rom[mapObjOffset + 1] & 0x80) > 0;
                rom[mapObjOffset + 1] = 92;
                if (msb)
                {
                    rom[mapObjOffset + 1] |= 0x80;
                }
            }
        }

        public static List<byte> GetValidOrbElements(bool girlExists, bool spriteExists)
        {
            List<byte> orbElementsAvailable = new List<byte>();
            if(spriteExists)
            {
                // everything but lumina
                orbElementsAvailable.Add(0x81); 
                orbElementsAvailable.Add(0x82);
                orbElementsAvailable.Add(0x83);
                orbElementsAvailable.Add(0x85);
                orbElementsAvailable.Add(0x86);
                orbElementsAvailable.Add(0x87);
                orbElementsAvailable.Add(0x88);
            }
            if (girlExists)
            {
                orbElementsAvailable.Add(0x84); // lumina
                if (!spriteExists)
                {
                    orbElementsAvailable.Add(0x83); // sala
                    orbElementsAvailable.Add(0x85); // sylphid
                }
            }

            return orbElementsAvailable;
        }
    }
}
