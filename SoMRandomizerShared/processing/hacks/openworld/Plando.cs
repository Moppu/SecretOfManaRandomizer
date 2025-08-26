using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.openworld.randomization;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.openworld.PlandoProperties;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Plando processing for open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class Plando
    {
        static Dictionary<string, string> plandoLocNamesToOpenWorld;
        static Dictionary<string, string> plandoPrizeNamesToOpenWorld;
        static Dictionary<string, List<string>> plandoRandomLocNames = new Dictionary<string, List<string>>();
        static Dictionary<string, List<string>> plandoRandomPrizeNames = new Dictionary<string, List<string>>();
        static Plando()
        {
            // mapping of location / prize keys from PlandoProperties (and PlandoForm) to items from OpenWorldLocations / OpenWorldPrizes

            // locations
            // pandora area
            plandoLocNamesToOpenWorld = new Dictionary<string, string>();
            plandoLocNamesToOpenWorld[KEY_LOCATION_POTOS_CHEST] = "potos chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_MANTIS_ANT] = "mantis ant (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_PANDORA_GIRL] = "girl";
            plandoLocNamesToOpenWorld[KEY_LOCATION_PANDORA_CHEST_1] = "pandora chest 1";
            plandoLocNamesToOpenWorld[KEY_LOCATION_PANDORA_CHEST_2] = "pandora chest 2";
            plandoLocNamesToOpenWorld[KEY_LOCATION_PANDORA_CHEST_3] = "pandora chest 3";
            plandoLocNamesToOpenWorld[KEY_LOCATION_PANDORA_CHEST_4] = "pandora chest 4";
            plandoLocNamesToOpenWorld[KEY_LOCATION_PANDORA_CHEST_5] = "pandora sword orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_PANDORA_CHEST_6] = "pandora spear orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_LUKA_1] = "luka item 1 (spear)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_LUKA_2] = "luka item 2 (undine seed)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_UNDINE_1] = "undine item 1 (spells)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_UNDINE_2] = "undine item 2 (javelin)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_ELINEE_CHEST_1] = "whip chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_ELINEE_CHEST_2] = "chest next to whip chest";
            // sword pedestal (flammie drum in logic only)
            plandoLocNamesToOpenWorld[KEY_LOCATION_SWORD_PEDESTAL] = "sword pedestal";

            // gaia navel
            plandoLocNamesToOpenWorld[KEY_LOCATION_MAGIC_ROPE_CHEST] = "magic rope chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_WATTS] = "watts (axe)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_DWARF_ELDER] = "dwarf elder (midge mallet)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_TROPICALLO_1] = "tropicallo (sprite)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_TROPICALLO_2] = "tropicallo (bow)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_GNOME_1] = "gnome item 1 (spells)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_GNOME_2] = "gnome item 2 (seed)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_KILROY] = "kilroy (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_KILROY_CHEST] = "kilroy whip orb chest";

            // upper land
            plandoLocNamesToOpenWorld[KEY_LOCATION_MOOGLE_TOWN_CHEST_1] = "moogle village glove orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_MOOGLE_TOWN_CHEST_2] = "moogle village axe orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_SYLPHID_1] = "sylphid item 1 (spells)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_SYLPHID_2] = "sylphid item 2 (seed)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_MATANGO_INN_CHEST] = "matango inn javelin orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_MATANGO_CAVE_FLAMMIE] = "matango flammie";

            // empire
            plandoLocNamesToOpenWorld[KEY_LOCATION_SOUTHTOWN_MARA] = "mara (tower key)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_NTR_WEST_CHEST] = "northtown ruins spear orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_NTR_EAST_CHEST] = "northtown ruins bow orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_NTR_INNER_CHEST] = "northtown ruins sword orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_NTR_WALL] = "doom wall";
            plandoLocNamesToOpenWorld[KEY_LOCATION_NTR_VAMPIRE] = "vampire";
            plandoLocNamesToOpenWorld[KEY_LOCATION_NTC_EAST_CHEST_1] = "northtown castle axe orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_NTC_EAST_CHEST_2] = "northtown castle whip orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_NTC_METAL_MANTIS] = "metal mantis (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_NTC_INNER_CHEST] = "northtown castle chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_NTC_MECH_RIDER] = "mech rider 2";
            plandoLocNamesToOpenWorld[KEY_LOCATION_GOLD_TOWER_CHEST_1] = "lumina tower spear orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_GOLD_TOWER_CHEST_2] = "lumina tower axe orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_LUMINA_1] = "lumina spells";
            plandoLocNamesToOpenWorld[KEY_LOCATION_LUMINA_2] = "lumina seed";

            // ice country
            plandoLocNamesToOpenWorld[KEY_LOCATION_SALAMANDO_STOVE] = "salamando";
            plandoLocNamesToOpenWorld[KEY_LOCATION_TRIPLE_TONPOLE] = "triple tonpole";
            plandoLocNamesToOpenWorld[KEY_LOCATION_ICE_CASTLE_CHEST] = "ice castle glove orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_FROST_GIGAS] = "santa (new item)";

            // desert
            plandoLocNamesToOpenWorld[KEY_LOCATION_SEA_HARE_TAIL_GIFT] = "kakkara (moogle belt)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_FIRE_PALACE_CHEST_1] = "fire palace chest 1";
            plandoLocNamesToOpenWorld[KEY_LOCATION_FIRE_PALACE_CHEST_2] = "fire palace chest 2";
            plandoLocNamesToOpenWorld[KEY_LOCATION_FIRE_PALACE_CHEST_3] = "fire palace axe orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_FIRE_PALACE_SALAMANDO] = "fire seed";
            plandoLocNamesToOpenWorld[KEY_LOCATION_LUNA_1] = "luna item 1 (spells)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_LUNA_2] = "luna item 2 (seed)";

            // mountains
            plandoLocNamesToOpenWorld[KEY_LOCATION_POD_GATED_CHEST] = "shade palace glove orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_POD_HALLWAY_CHEST] = "shade palace chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_SHADE_1] = "shade spells";
            plandoLocNamesToOpenWorld[KEY_LOCATION_SHADE_2] = "shade seed";
            plandoLocNamesToOpenWorld[KEY_LOCATION_DOPPEL] = "jehk";

            // lost continent
            plandoLocNamesToOpenWorld[KEY_LOCATION_WATERMELON] = "watermelon (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_DRYAD_1] = "dryad spells";
            plandoLocNamesToOpenWorld[KEY_LOCATION_DRYAD_2] = "dryad seed";
            plandoLocNamesToOpenWorld[KEY_LOCATION_HEXAS] = "hexas (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_SPELL_ORB_AREA_CHEST] = "sunken continent sword orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_UNDERSEA_AREA_CHEST] = "sunken continent boomerang orb chest";
            plandoLocNamesToOpenWorld[KEY_LOCATION_HYDRA] = "hydra (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_KETTLE_KIN] = "kettlekin (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_MECH_RIDER_3] = "mech rider 3 (new item)";

            // pure lands
            plandoLocNamesToOpenWorld[KEY_LOCATION_DRAGON_WORM] = "dragon worm (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_SNOW_DRAGON] = "snow dragon (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_AXE_BEAK] = "axe beak (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_RED_DRAGON] = "red dragon (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_THUNDER_GIGAS] = "thunder gigas (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_BLUE_DRAGON] = "blue dragon (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_MANA_TREE] = "mana tree (new item)";

            // misc
            plandoLocNamesToOpenWorld[KEY_LOCATION_TURTLE_ISLAND] = "turtle island (sea hare tail)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_LIGHTHOUSE] = "lighthouse (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_TASNICA_MINIBOSS] = "jema at tasnica";
            plandoLocNamesToOpenWorld[KEY_LOCATION_BUFFY] = "buffy (new item)";
            plandoLocNamesToOpenWorld[KEY_LOCATION_DREAD_SLIME] = "dread slime (new item)";

            plandoRandomLocNames[KEY_LOCATION_ANY_PANDORA] = new string[] {
            KEY_LOCATION_POTOS_CHEST,
            KEY_LOCATION_MANTIS_ANT,
            KEY_LOCATION_PANDORA_GIRL,
            KEY_LOCATION_PANDORA_CHEST_1,
            KEY_LOCATION_PANDORA_CHEST_2,
            KEY_LOCATION_PANDORA_CHEST_3,
            KEY_LOCATION_PANDORA_CHEST_4,
            KEY_LOCATION_PANDORA_CHEST_5,
            KEY_LOCATION_PANDORA_CHEST_6,
            KEY_LOCATION_LUKA_1,
            KEY_LOCATION_LUKA_2,
            KEY_LOCATION_UNDINE_1,
            KEY_LOCATION_UNDINE_2,
            KEY_LOCATION_ELINEE_CHEST_1,
            KEY_LOCATION_ELINEE_CHEST_2,
            }.ToList();

            plandoRandomLocNames[KEY_LOCATION_ANY_GAIAS_NAVEL] = new string[] {
            KEY_LOCATION_MAGIC_ROPE_CHEST,
            KEY_LOCATION_WATTS,
            KEY_LOCATION_DWARF_ELDER,
            KEY_LOCATION_TROPICALLO_1,
            KEY_LOCATION_TROPICALLO_2,
            KEY_LOCATION_GNOME_1,
            KEY_LOCATION_GNOME_2,
            KEY_LOCATION_KILROY,
            KEY_LOCATION_KILROY_CHEST,
            }.ToList();

            plandoRandomLocNames[KEY_LOCATION_ANY_UPPER_LAND] = new string[] {
            KEY_LOCATION_MOOGLE_TOWN_CHEST_1,
            KEY_LOCATION_MOOGLE_TOWN_CHEST_2,
            KEY_LOCATION_SYLPHID_1,
            KEY_LOCATION_SYLPHID_2,
            KEY_LOCATION_MATANGO_INN_CHEST,
            KEY_LOCATION_MATANGO_CAVE_FLAMMIE,
            }.ToList();

            plandoRandomLocNames[KEY_LOCATION_ANY_EMPIRE] = new string[] {
            KEY_LOCATION_SOUTHTOWN_MARA,
            KEY_LOCATION_NTR_WEST_CHEST,
            KEY_LOCATION_NTR_EAST_CHEST,
            KEY_LOCATION_NTR_INNER_CHEST,
            KEY_LOCATION_NTR_WALL,
            KEY_LOCATION_NTR_VAMPIRE,
            KEY_LOCATION_NTC_EAST_CHEST_1,
            KEY_LOCATION_NTC_EAST_CHEST_2,
            KEY_LOCATION_NTC_METAL_MANTIS,
            KEY_LOCATION_NTC_INNER_CHEST,
            KEY_LOCATION_NTC_MECH_RIDER,
            KEY_LOCATION_GOLD_TOWER_CHEST_1,
            KEY_LOCATION_GOLD_TOWER_CHEST_2,
            KEY_LOCATION_LUMINA_1,
            KEY_LOCATION_LUMINA_2
            }.ToList();

            plandoRandomLocNames[KEY_LOCATION_ANY_ICE_COUNTRY] = new string[] {
            KEY_LOCATION_SALAMANDO_STOVE,
            KEY_LOCATION_TRIPLE_TONPOLE,
            KEY_LOCATION_ICE_CASTLE_CHEST,
            KEY_LOCATION_FROST_GIGAS,
            }.ToList();

            plandoRandomLocNames[KEY_LOCATION_ANY_DESERT] = new string[] {
            KEY_LOCATION_SEA_HARE_TAIL_GIFT,
            KEY_LOCATION_FIRE_PALACE_CHEST_1,
            KEY_LOCATION_FIRE_PALACE_CHEST_2,
            KEY_LOCATION_FIRE_PALACE_CHEST_3,
            KEY_LOCATION_FIRE_PALACE_SALAMANDO,
            KEY_LOCATION_LUNA_1,
            KEY_LOCATION_LUNA_2,
            }.ToList();

            plandoRandomLocNames[KEY_LOCATION_ANY_MOUNTAINS] = new string[] {
            KEY_LOCATION_POD_GATED_CHEST,
            KEY_LOCATION_POD_HALLWAY_CHEST,
            KEY_LOCATION_SHADE_1,
            KEY_LOCATION_SHADE_2,
            KEY_LOCATION_DOPPEL,
            }.ToList();

            plandoRandomLocNames[KEY_LOCATION_ANY_LOST_CONTINENT] = new string[] {
            KEY_LOCATION_WATERMELON,
            KEY_LOCATION_DRYAD_1,
            KEY_LOCATION_DRYAD_2,
            KEY_LOCATION_HEXAS,
            KEY_LOCATION_SPELL_ORB_AREA_CHEST,
            KEY_LOCATION_UNDERSEA_AREA_CHEST,
            KEY_LOCATION_HYDRA,
            KEY_LOCATION_KETTLE_KIN,
            KEY_LOCATION_MECH_RIDER_3,
            }.ToList();

            plandoRandomLocNames[KEY_LOCATION_ANY_PURE_LANDS] = new string[] {
            KEY_LOCATION_DRAGON_WORM,
            KEY_LOCATION_SNOW_DRAGON,
            KEY_LOCATION_AXE_BEAK,
            KEY_LOCATION_RED_DRAGON,
            KEY_LOCATION_THUNDER_GIGAS,
            KEY_LOCATION_BLUE_DRAGON,
            KEY_LOCATION_MANA_TREE,
            }.ToList();

            plandoRandomLocNames[KEY_LOCATION_ANY_MANA_SEED_PEDESTAL] = new string[] {
            KEY_LOCATION_LUKA_1,
            KEY_LOCATION_LUKA_2,
            KEY_LOCATION_GNOME_1,
            KEY_LOCATION_GNOME_2,
            KEY_LOCATION_SYLPHID_1,
            KEY_LOCATION_SYLPHID_2,
            KEY_LOCATION_FIRE_PALACE_SALAMANDO,
            KEY_LOCATION_LUNA_1,
            KEY_LOCATION_LUNA_1,
            KEY_LOCATION_SHADE_1,
            KEY_LOCATION_SHADE_2,
            KEY_LOCATION_LUMINA_1,
            KEY_LOCATION_LUMINA_2,
            KEY_LOCATION_DRYAD_1,
            KEY_LOCATION_DRYAD_2,
            }.ToList();

            plandoRandomLocNames[KEY_LOCATION_ANY_BOSS] = new string[] {
            KEY_LOCATION_MANTIS_ANT,
            KEY_LOCATION_TROPICALLO_1,
            KEY_LOCATION_TROPICALLO_2,
            KEY_LOCATION_NTR_WALL,
            KEY_LOCATION_NTR_VAMPIRE,
            KEY_LOCATION_NTC_METAL_MANTIS,
            KEY_LOCATION_NTC_MECH_RIDER,
            KEY_LOCATION_TRIPLE_TONPOLE,
            KEY_LOCATION_FROST_GIGAS,
            KEY_LOCATION_DOPPEL,
            KEY_LOCATION_WATERMELON,
            KEY_LOCATION_HEXAS,
            KEY_LOCATION_HYDRA,
            KEY_LOCATION_KETTLE_KIN,
            KEY_LOCATION_MECH_RIDER_3,
            KEY_LOCATION_DRAGON_WORM,
            KEY_LOCATION_SNOW_DRAGON,
            KEY_LOCATION_AXE_BEAK,
            KEY_LOCATION_RED_DRAGON,
            KEY_LOCATION_THUNDER_GIGAS,
            KEY_LOCATION_BLUE_DRAGON,
            }.ToList();

            // prizes
            plandoPrizeNamesToOpenWorld = new Dictionary<string, string>();
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_GLOVE] = "glove";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_SWORD] = "sword";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_SPEAR] = "spear";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_AXE] = "axe";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_WHIP] = "whip";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_BOW] = "bow";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_BOOMERANG] = "boomerang";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_JAVELIN] = "javelin";

            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_BOY] = "boy";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_GIRL] = "girl";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_SPRITE] = "sprite";

            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_UNDINE] = "undine spells";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_GNOME] = "gnome spells";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_SYLPHID] = "sylphid spells";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_SALAMANDO] = "salamando spells";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_LUMINA] = "lumina spells";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_SHADE] = "shade spells";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_LUNA] = "luna spells";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_DRYAD] = "dryad spells";

            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_WATER_SEED] = "water seed";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_EARTH_SEED] = "earth seed";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_WIND_SEED] = "wind seed";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_FIRE_SEED] = "fire seed";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_LIGHT_SEED] = "light seed";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_DARK_SEED] = "dark seed";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_MOON_SEED] = "moon seed";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_DRYAD_SEED] = "dryad seed";

            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_MOOGLE_BELT] = "moogle belt";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_MIDGE_MALLET] = "midge mallet";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_SEA_HARE_TAIL] = "sea hare tail";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_GOLD_KEY] = "gold tower key";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_FLAMMIE_DRUM] = "flammie drum";

            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_GLOVE_ORB] = "Glove orb";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_SWORD_ORB] = "Sword orb";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_AXE_ORB] = "Axe orb";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_SPEAR_ORB] = "Spear orb";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_WHIP_ORB] = "Whip orb";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_BOW_ORB] = "Bow orb";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_BOOMERANG_ORB] = "Boomerang orb";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_JAVELIN_ORB] = "Javelin orb";
            plandoPrizeNamesToOpenWorld[VALUE_PRIZE_NOTHING] = "nothing";

            // (Any Character)
            plandoRandomPrizeNames[VALUE_PRIZE_ANY_CHARACTER] = new string[] {
            VALUE_PRIZE_BOY,
            VALUE_PRIZE_GIRL,
            VALUE_PRIZE_SPRITE,
            }.ToList();

            plandoRandomPrizeNames[VALUE_PRIZE_OTHER_CHARACTER_1] = new string[] {
            VALUE_PRIZE_BOY,
            VALUE_PRIZE_GIRL,
            VALUE_PRIZE_SPRITE,
            }.ToList();

            plandoRandomPrizeNames[VALUE_PRIZE_OTHER_CHARACTER_2] = new string[] {
            VALUE_PRIZE_BOY,
            VALUE_PRIZE_GIRL,
            VALUE_PRIZE_SPRITE,
            }.ToList();

            // (Any Weapon)
            plandoRandomPrizeNames[VALUE_PRIZE_ANY_WEAPON] = new string[] {
            VALUE_PRIZE_GLOVE,
            VALUE_PRIZE_SWORD,
            VALUE_PRIZE_SPEAR,
            VALUE_PRIZE_AXE,
            VALUE_PRIZE_WHIP,
            VALUE_PRIZE_BOW,
            VALUE_PRIZE_BOOMERANG,
            VALUE_PRIZE_JAVELIN,
            }.ToList();

            // (Cutting Weapon)
            plandoRandomPrizeNames[VALUE_PRIZE_CUTTING_WEAPON] = new string[] {
            VALUE_PRIZE_SWORD,
            VALUE_PRIZE_AXE,
            }.ToList();

            // (Any Spells)
            plandoRandomPrizeNames[VALUE_PRIZE_ANY_SPELLS] = new string[] {
            VALUE_PRIZE_UNDINE,
            VALUE_PRIZE_GNOME,
            VALUE_PRIZE_SYLPHID,
            VALUE_PRIZE_SALAMANDO,
            VALUE_PRIZE_LUMINA,
            VALUE_PRIZE_SHADE,
            VALUE_PRIZE_LUNA,
            VALUE_PRIZE_DRYAD,
            }.ToList();

            // (Any Seed)
            plandoRandomPrizeNames[VALUE_PRIZE_ANY_SEED] = new string[] {
            VALUE_PRIZE_WATER_SEED,
            VALUE_PRIZE_EARTH_SEED,
            VALUE_PRIZE_WIND_SEED,
            VALUE_PRIZE_FIRE_SEED,
            VALUE_PRIZE_LIGHT_SEED,
            VALUE_PRIZE_DARK_SEED,
            VALUE_PRIZE_MOON_SEED,
            VALUE_PRIZE_DRYAD,
            }.ToList();

            // (Any Orb)
            plandoRandomPrizeNames[VALUE_PRIZE_ANY_WEAPON_ORB] = new string[] {
            VALUE_PRIZE_GLOVE_ORB,
            VALUE_PRIZE_SWORD_ORB,
            VALUE_PRIZE_SPEAR_ORB,
            VALUE_PRIZE_AXE_ORB,
            VALUE_PRIZE_WHIP_ORB,
            VALUE_PRIZE_BOW_ORB,
            VALUE_PRIZE_BOOMERANG_ORB,
            VALUE_PRIZE_JAVELIN_ORB,
            }.ToList();
        }

        // replace "any" prize with randomly chosen one from that category
        public static void processAnyPrizes(List<string> startingItems, RandoContext context)
        {
            int boyStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.BOY_START_WEAPON_INDEX);
            int girlStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.GIRL_START_WEAPON_INDEX);
            int spriteStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.SPRITE_START_WEAPON_INDEX);
            Random r = context.randomFunctional;

            // (seeds)
            List<string> anySeedPrizes = new string[] { VALUE_PRIZE_WATER_SEED, VALUE_PRIZE_EARTH_SEED, VALUE_PRIZE_WIND_SEED, VALUE_PRIZE_FIRE_SEED,
                    VALUE_PRIZE_DARK_SEED, VALUE_PRIZE_LIGHT_SEED, VALUE_PRIZE_MOON_SEED, VALUE_PRIZE_DRYAD_SEED }.ToList();
            // if you plando more than 8 any seeds, just ignore them
            while (startingItems.Contains(VALUE_PRIZE_ANY_SEED) && anySeedPrizes.Count > 0)
            {
                startingItems.Remove(VALUE_PRIZE_ANY_SEED);
                int seedNum = r.Next() % anySeedPrizes.Count;
                startingItems.Add(anySeedPrizes[seedNum]);
                anySeedPrizes.RemoveAt(seedNum);
            }

            // (spells)
            List<string> anySpellPrizes = new string[] { VALUE_PRIZE_UNDINE, VALUE_PRIZE_GNOME, VALUE_PRIZE_SYLPHID, VALUE_PRIZE_SALAMANDO,
                    VALUE_PRIZE_SHADE, VALUE_PRIZE_LUMINA, VALUE_PRIZE_LUNA, VALUE_PRIZE_DRYAD }.ToList();
            // if you plando more than 8 any spells, just ignore them
            while (startingItems.Contains(VALUE_PRIZE_ANY_SPELLS) && anySpellPrizes.Count > 0)
            {
                startingItems.Remove(VALUE_PRIZE_ANY_SPELLS);
                int spellNum = r.Next() % anySpellPrizes.Count;
                startingItems.Add(anySpellPrizes[spellNum]);
                anySpellPrizes.RemoveAt(spellNum);
            }

            // (weapons)
            List<string> anyWeaponPrizes = new string[] { VALUE_PRIZE_GLOVE, VALUE_PRIZE_SWORD, VALUE_PRIZE_AXE, VALUE_PRIZE_SPEAR,
                    VALUE_PRIZE_WHIP, VALUE_PRIZE_BOW, VALUE_PRIZE_BOOMERANG, VALUE_PRIZE_JAVELIN }.ToList();
            List<string> anyCuttingWeaponPrizes = new string[] { VALUE_PRIZE_SWORD, VALUE_PRIZE_AXE }.ToList();
            // don't repeat starter weapons
            if (boyStarterWeapon >= 0)
            {
                anyWeaponPrizes.Remove(SomVanillaValues.weaponByteToName(boyStarterWeapon));
                anyCuttingWeaponPrizes.Remove(SomVanillaValues.weaponByteToName(boyStarterWeapon));
            }
            if (girlStarterWeapon >= 0)
            {
                anyWeaponPrizes.Remove(SomVanillaValues.weaponByteToName(girlStarterWeapon));
                anyCuttingWeaponPrizes.Remove(SomVanillaValues.weaponByteToName(girlStarterWeapon));
            }
            if (spriteStarterWeapon >= 0)
            {
                anyWeaponPrizes.Remove(SomVanillaValues.weaponByteToName(spriteStarterWeapon));
                anyCuttingWeaponPrizes.Remove(SomVanillaValues.weaponByteToName(spriteStarterWeapon));
            }
            // if you plando more than 8 any weapons, just ignore them
            while (startingItems.Contains(VALUE_PRIZE_ANY_WEAPON) && anyWeaponPrizes.Count > 0)
            {
                startingItems.Remove(VALUE_PRIZE_ANY_WEAPON);
                int weaponNum = r.Next() % anyWeaponPrizes.Count;
                startingItems.Add(anyWeaponPrizes[weaponNum]);
                anyWeaponPrizes.RemoveAt(weaponNum);
            }
            while (startingItems.Contains(VALUE_PRIZE_CUTTING_WEAPON) && anyCuttingWeaponPrizes.Count > 0)
            {
                startingItems.Remove(VALUE_PRIZE_CUTTING_WEAPON);
                int weaponNum = r.Next() % anyCuttingWeaponPrizes.Count;
                startingItems.Add(anyCuttingWeaponPrizes[weaponNum]);
                anyCuttingWeaponPrizes.RemoveAt(weaponNum);
            }

            // (weapon orbs)
            List<string> anyWeaponOrbPrizes = new string[] { VALUE_PRIZE_GLOVE_ORB, VALUE_PRIZE_SWORD_ORB, VALUE_PRIZE_AXE_ORB, VALUE_PRIZE_SPEAR_ORB,
                    VALUE_PRIZE_WHIP_ORB, VALUE_PRIZE_BOW_ORB, VALUE_PRIZE_BOOMERANG_ORB, VALUE_PRIZE_JAVELIN_ORB }.ToList();
            while (startingItems.Contains(VALUE_PRIZE_ANY_WEAPON_ORB))
            {
                startingItems.Remove(VALUE_PRIZE_ANY_WEAPON_ORB);
                int weaponOrbNum = r.Next() % anyWeaponOrbPrizes.Count;
                startingItems.Add(anyWeaponOrbPrizes[weaponOrbNum]);
            }
        }

        public static bool doPlando(Random r, Dictionary<string, List<string>> plandoSettings, Dictionary<PrizeLocation, PrizeItem> itemPlacements, Dictionary<PrizeLocation, List<PrizeItem>> allAvailablePrizes, List<PrizeItem> allPrizes, List<string> plandoLocations)
        {
            // prizesForLookup - location name -> prize name
            // prizeObjectsForLookup - location name -> prize object .. will have to pass those in here too probably
            // prizeHintsForLookup - location name -> prizeitem.hintName
            // plandoLocations - output from here for placing the rest

            bool bypassValidation = false;
            if (plandoSettings.ContainsKey(KEY_BYPASS_VALIDATION) && plandoSettings[KEY_BYPASS_VALIDATION].Contains("yes"))
            {
                // ignore all failures below if we specify this, since user thinks plando locations will be solvable
                bypassValidation = true;
            }

            Logging.log("Doing plando.", "debug");

            // fill in the definite ones first
            for (int locationPass = 0; locationPass < 2; locationPass++)
            {
                foreach (string plandoLocation in plandoSettings.Keys)
                {
                    // ignore non-prize plando stuff like boss settings, elements, etc
                    if (plandoLocation.StartsWith(KEY_PREFIX_PRIZE))
                    {
                        if (plandoLocation != KEY_LOCATION_START_WITH) // process elsewhere
                        {
                            string internalLocName = "";
                            if (plandoLocNamesToOpenWorld.ContainsKey(plandoLocation))
                            {
                                if (locationPass == 0)
                                {
                                    Logging.log("    Processing " + plandoLocation + " as direct location.", "debug");
                                    internalLocName = plandoLocNamesToOpenWorld[plandoLocation];
                                    Logging.log("    Found internal plando location: " + internalLocName, "debug");
                                }
                            }
                            else if (plandoRandomLocNames.ContainsKey(plandoLocation))
                            {
                                if (locationPass == 1)
                                {
                                    Logging.log("    Processing " + plandoLocation + " as random choice location.", "debug");
                                    // random choice location - these should be chosen after the others
                                    List<string> matchingInternalLocations = new List<string>();
                                    foreach (string locName in plandoRandomLocNames[plandoLocation])
                                    {
                                        string internalName = plandoLocNamesToOpenWorld[locName];
                                        if (!plandoLocations.Contains(internalName))
                                        {
                                            matchingInternalLocations.Add(internalName);
                                        }
                                    }

                                    if (matchingInternalLocations.Count == 0)
                                    {
                                        Logging.log("ERROR - No locations left for: " + plandoLocation);
                                        return false;
                                    }

                                    internalLocName = matchingInternalLocations[r.Next() % matchingInternalLocations.Count];
                                    Logging.log("    Picked " + internalLocName, "debug");
                                }
                            }
                            else
                            {
                                if (plandoLocation != KEY_LOCATION_NON_EXISTENT)
                                {
                                    Logging.log("Warning - Unable to identify location " + plandoLocation);
                                }
                            }

                            // found somewhere to stick it - try to identify a prize
                            if (internalLocName != "")
                            {
                                // fill in the definite ones first
                                for (int prizePass = 0; prizePass < 2; prizePass++)
                                {
                                    foreach (string plandoPrize in plandoSettings[plandoLocation])
                                    {
                                        // single location
                                        PrizeItem foundPrizeItem = null;
                                        string internalPrizeName = "";
                                        if (plandoPrizeNamesToOpenWorld.ContainsKey(plandoPrize))
                                        {
                                            if (prizePass == 0)
                                            {
                                                Logging.log("    Processing " + plandoPrize + " as direct prize.", "debug");
                                                // single prize
                                                internalPrizeName = plandoPrizeNamesToOpenWorld[plandoPrize];
                                                Logging.log("    Found internal plando prize: " + internalPrizeName, "debug");
                                                if (bypassValidation)
                                                {
                                                    // check everything even if not valid for location
                                                    foreach (PrizeItem pi in allPrizes)
                                                    {
                                                        if (pi.prizeName == internalPrizeName)
                                                        {
                                                            foundPrizeItem = pi;
                                                            Logging.log("    Found prize item while bypassing location.", "debug");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // check what's available for location
                                                    foreach (PrizeLocation pl in allAvailablePrizes.Keys)
                                                    {
                                                        if (pl.locationName == internalLocName)
                                                        {
                                                            foreach (PrizeItem pi in allAvailablePrizes[pl])
                                                            {
                                                                if (pi.prizeName == internalPrizeName)
                                                                {
                                                                    foundPrizeItem = pi;
                                                                    Logging.log("    Found prize item.", "debug");
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if (foundPrizeItem == null)
                                                {
                                                    Logging.log("ERROR - Unable to place " + plandoPrize + " at " + plandoLocation + " for plando! Item not suitable for location.");
                                                    return false;
                                                }
                                            }
                                        }
                                        else if (plandoRandomPrizeNames.ContainsKey(plandoPrize))
                                        {
                                            if (prizePass == 1)
                                            {
                                                Logging.log("    Processing " + plandoPrize + " as random choice prize.", "debug");
                                                // random choice prize - these should be chosen after the others
                                                List<string> matchingInternalPrizes = new List<string>();
                                                foreach (string prizeName in plandoRandomPrizeNames[plandoPrize])
                                                {
                                                    string internalName = plandoPrizeNamesToOpenWorld[prizeName];
                                                    if (!prizeAlreadyPlaced(itemPlacements, internalName))
                                                    {
                                                        matchingInternalPrizes.Add(internalName);
                                                    }
                                                }

                                                if (matchingInternalPrizes.Count == 0)
                                                {
                                                    Logging.log("ERROR - No prizes left for: " + plandoPrize);
                                                    return false;
                                                }

                                                internalPrizeName = matchingInternalPrizes[r.Next() % matchingInternalPrizes.Count];
                                                Logging.log("    Picked " + internalPrizeName, "debug");
                                                if (bypassValidation)
                                                {
                                                    // check everything even if not valid for location
                                                    foreach (PrizeItem pi in allPrizes)
                                                    {
                                                        if (pi.prizeName == internalPrizeName)
                                                        {
                                                            foundPrizeItem = pi;
                                                            Logging.log("    Found prize item while bypassing location.", "debug");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // check what's available for location
                                                    foreach (PrizeLocation pl in allAvailablePrizes.Keys)
                                                    {
                                                        if (pl.locationName == internalLocName)
                                                        {
                                                            foreach (PrizeItem pi in allAvailablePrizes[pl])
                                                            {
                                                                if (pi.prizeName == internalPrizeName)
                                                                {
                                                                    foundPrizeItem = pi;
                                                                    Logging.log("    Found prize item.", "debug");
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if (foundPrizeItem == null)
                                                {
                                                    Logging.log("ERROR - Unable to place " + plandoPrize + " at " + plandoLocation + " for plando! Item not suitable for location.");
                                                    return false;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            if (plandoPrize == VALUE_PRIZE_MONEY)
                                            {
                                                // search for "GP " and pick a random one
                                                List<PrizeItem> allGpPrizes = new List<PrizeItem>();
                                                foreach (PrizeItem pi in allPrizes)
                                                {
                                                    if (pi.prizeName.StartsWith("GP ") && !itemPlacements.ContainsValue(pi))
                                                    {
                                                        allGpPrizes.Add(pi);
                                                    }
                                                }
                                                if (allGpPrizes.Count == 0)
                                                {
                                                    Logging.log("ERROR - No GP prizes left!");
                                                    return false;
                                                }
                                                foundPrizeItem = allGpPrizes[r.Next() % allGpPrizes.Count];
                                                internalPrizeName = foundPrizeItem.prizeName;
                                            }
                                            else
                                            {
                                                Logging.log("ERROR - Unable to identify prize " + plandoPrize);
                                                return false;
                                            }
                                        }

                                        // found a prize - stick it in the location
                                        if (internalPrizeName != "")
                                        {
                                            if (plandoLocation != KEY_LOCATION_NON_EXISTENT)
                                            {
                                                Logging.log("    * Mapping " + internalLocName + " to " + foundPrizeItem.prizeName, "debug");
                                                itemPlacements[lookupLocationByName(allAvailablePrizes, internalLocName)] = foundPrizeItem;
                                                plandoLocations.Add(internalLocName);
                                            }
                                            // remove from possibility everywhere now that we placed it
                                            clearPrize(internalPrizeName, allAvailablePrizes);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Logging.log("Ignoring non-prize plando location: " + plandoLocation, "debug");
                    }
                }
            }

            List<string> clearedPrizes = new List<string>();
            // non-existent - remove from prize pools everywhere
            foreach (string plandoLocation in plandoSettings.Keys)
            {
                if (plandoLocation == KEY_LOCATION_NON_EXISTENT)
                {
                    Logging.log("  Processing non-existent items", "debug");
                    foreach (string plandoPrize in plandoSettings[plandoLocation])
                    {
                        Logging.log("    Processing non-existent prize: " + plandoPrize, "debug");
                        if (plandoPrizeNamesToOpenWorld.ContainsKey(plandoPrize))
                        {
                            Logging.log("    Processing as single prize.", "debug");
                            string internalPrizeName = plandoPrizeNamesToOpenWorld[plandoPrize];
                            Logging.log("    Found internal prize name: " + internalPrizeName, "debug");
                            clearPrize(internalPrizeName, allAvailablePrizes);
                            clearedPrizes.Add(internalPrizeName);
                        }
                        else if (plandoRandomPrizeNames.ContainsKey(plandoPrize))
                        {
                            Logging.log("    Processing as random choice prize.", "debug");
                            List<string> matchingInternalPrizes = new List<string>();
                            foreach (string prizeName in plandoRandomPrizeNames[plandoPrize])
                            {
                                string internalName = plandoPrizeNamesToOpenWorld[prizeName];
                                if (!prizeAlreadyPlaced(itemPlacements, internalName))
                                {
                                    matchingInternalPrizes.Add(internalName);
                                }
                            }

                            if (matchingInternalPrizes.Count == 0)
                            {
                                Logging.log("ERROR - No prizes left for: " + plandoPrize);
                                return false;
                            }

                            string internalPrizeName = matchingInternalPrizes[r.Next() % matchingInternalPrizes.Count];
                            Logging.log("    Removing " + internalPrizeName + " from the prize pool.", "debug");
                            clearPrize(internalPrizeName, allAvailablePrizes);
                            clearedPrizes.Add(internalPrizeName);
                        }
                    }
                }
            }

            Logging.log("Done with plando!", "debug");

            return true;
        }

        private static PrizeLocation lookupLocationByName(Dictionary<PrizeLocation, List<PrizeItem>> allAvailablePrizes, string locName)
        {
            foreach(PrizeLocation loc in allAvailablePrizes.Keys)
            {
                if(loc.locationName == locName)
                {
                    return loc;
                }
            }
            return null;
        }

        private static bool prizeAlreadyPlaced(Dictionary<PrizeLocation, PrizeItem> itemPlacements, string name)
        {
            foreach(PrizeItem item in itemPlacements.Values)
            {
                if(item.prizeName == name)
                {
                    return true;
                }
            }
            return false;
        }

        private static void clearPrize(string internalPrizeName, Dictionary<PrizeLocation, List<PrizeItem>> allAvailablePrizes)
        {
            foreach (PrizeLocation pl in allAvailablePrizes.Keys)
            {
                PrizeItem foundItem = null;
                foreach (PrizeItem pi in allAvailablePrizes[pl])
                {
                    if (pi.prizeName == internalPrizeName)
                    {
                        foundItem = pi;
                    }
                }
                if (foundItem != null)
                {
                    allAvailablePrizes[pl].Remove(foundItem);
                }
            }
        }
    }
}
