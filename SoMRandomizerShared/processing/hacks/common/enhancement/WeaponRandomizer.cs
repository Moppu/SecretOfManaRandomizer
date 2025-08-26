using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.util;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Generate randomized player weapons to replace all of the vanilla ones.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class WeaponRandomizer : RandoProcessor
    {
        // offset for table of weapon elements written here, and used by the element damage hack (ElementDamage) to determine damage boost
        public static string ELEMENTS_TABLE_OFFSET_HIROM = "weaponElementsTableOffset";

        List<StatusCondition> statusConditions = new List<StatusCondition>();
        List<WeaponCategory> weaponCategories = new List<WeaponCategory>();
        // for color names and shit like that
        List<StatusCondition> extraNoneCategories = new List<StatusCondition>();
        // + stats
        List<StatusCondition> statBoostCategories = new List<StatusCondition>();
        StatusCondition none;
        WeaponCategory sharedSpecialNames;

        StatusCondition none_Garbage = new StatusCondition();

        protected override string getName()
        {
            return "Weapon randomizer";
        }

        // christmas mode gives more fire shit, because it makes a bunch of the enemies ice type
        private void initWeapons(bool christmasMode)
        {
            statusConditions.Clear();
            weaponCategories.Clear();
            extraNoneCategories.Clear();
            statBoostCategories.Clear();
            // pairings of weapon sprite palette and weapon menu palette.  most of these were discovered via experimentation or
            // looking at existing vanilla values.
            WeaponPaletteSet dullGreen = new WeaponPaletteSet(0x02, 11);
            WeaponPaletteSet greenBrown = new WeaponPaletteSet(0x00, 11);
            WeaponPaletteSet manaSwordColorMaybe = new WeaponPaletteSet(0x04, 2);
            WeaponPaletteSet manaSwordColorSimilar = new WeaponPaletteSet(0x04, 59);
            WeaponPaletteSet pink = new WeaponPaletteSet(0x0E, 74);
            WeaponPaletteSet darkDullPurple = new WeaponPaletteSet(0x10, 12);
            WeaponPaletteSet plainPurple = new WeaponPaletteSet(0x14, 94);
            WeaponPaletteSet lavender = new WeaponPaletteSet(0x16, 101);
            WeaponPaletteSet dirtColor = new WeaponPaletteSet(0x1C, 6);
            WeaponPaletteSet whiteish = new WeaponPaletteSet(0x20, 85);
            WeaponPaletteSet white_fork_thing = new WeaponPaletteSet(0x20, 32);
            WeaponPaletteSet lightBlue = new WeaponPaletteSet(0x22, 103);
            WeaponPaletteSet pastelPink = new WeaponPaletteSet(0x30, 117);
            WeaponPaletteSet niceGreen = new WeaponPaletteSet(0x40, 91);
            WeaponPaletteSet purpleGray = new WeaponPaletteSet(0x42, 94);
            WeaponPaletteSet gray = new WeaponPaletteSet(0x44, 5);
            WeaponPaletteSet demonBlood = new WeaponPaletteSet(0x3A, 41);
            WeaponPaletteSet pearlSpear = new WeaponPaletteSet(0x22, 22);
            WeaponPaletteSet garnetGauntlet = new WeaponPaletteSet(0x3A, 152);
            WeaponPaletteSet purpleRainbow = new WeaponPaletteSet(0x0A, 10);
            WeaponPaletteSet orangeGoldish = new WeaponPaletteSet(0x1A, 4);
            WeaponPaletteSet brightYellow = new WeaponPaletteSet(0x1E, 30);
            WeaponPaletteSet grayBlue = new WeaponPaletteSet(0x0C, 17);
            WeaponPaletteSet redBanana = new WeaponPaletteSet(0x3C, 66);
            WeaponPaletteSet thunderSaberColor = new WeaponPaletteSet(0x26, 24);
            WeaponPaletteSet brightPink = new WeaponPaletteSet(0x2E, 74);
            WeaponPaletteSet brightLightBlue = new WeaponPaletteSet(0x32, 36);
            WeaponPaletteSet stick = new WeaponPaletteSet(0x18, 27);
            WeaponPaletteSet bluePinkRainbow = new WeaponPaletteSet(0x08, 7);
            WeaponPaletteSet grayish = new WeaponPaletteSet(0x20, 85);
            WeaponPaletteSet akkhan = new WeaponPaletteSet(0x20, 53);
            WeaponPaletteSet rusty = new WeaponPaletteSet(0x18, 15);
            WeaponPaletteSet rustySword = new WeaponPaletteSet(0x18, 4);
            WeaponPaletteSet redCleaver = new WeaponPaletteSet(0x24, 66);
            WeaponPaletteSet magicalBoomerang = new WeaponPaletteSet(0x24, 115);
            WeaponPaletteSet darkPurple = new WeaponPaletteSet(0x12, 14);
            WeaponPaletteSet lumina = new WeaponPaletteSet(0x28, 36);
            WeaponPaletteSet glowyOrange = new WeaponPaletteSet(0x2A, 30);
            WeaponPaletteSet neonGreen = new WeaponPaletteSet(0x2C, 91);
            WeaponPaletteSet fiery = new WeaponPaletteSet(0x24, 115);
            WeaponPaletteSet icyBlueAndPurple = new WeaponPaletteSet(0x3E, 62);
            WeaponPaletteSet blueAndGreen = new WeaponPaletteSet(0x06, 7);
            // original weapon data at 0x101000:
            // (gloves)
            //             vv pal
            // 00 00 C0 00 02 00 00 4B 02 00 00 50 ?? palettes for these?
            // 00 00 C0 00 1A 04 00 4B 06 00 00 50 
            // 00 00 C0 00 18 00 00 4B 0B 10 00 50 
            // 00 05 C0 00 0E 00 00 4B 11 00 00 50 
            // 00 00 C0 00 0C 04 00 4B 18 00 00 50 
            // 00 00 C0 00 16 10 00 4B 1E 00 00 50 
            // 00 00 C0 00 12 00 00 4B 26 00 20 50 
            // 00 00 C0 00 04 80 00 4B 2F 00 00 50
            // 00 00 C0 00 06 00 00 4B 35 00 00 50 
            // (swords)
            // 01 00 C0 00 18 00 00 4B 03 00 00 50 rusty
            // 01 40 C0 00 0C 00 00 4B 08 00 00 50 
            // 01 00 C0 00 02 10 00 4B 0E 00 00 50 
            // 01 00 C0 00 1E 04 00 4B 14 00 00 50 
            // 01 00 C0 00 16 20 00 4B 1B 00 00 50 
            // 01 00 C0 00 08 00 0A 4B 23 00 00 50 
            // 01 00 C1 00 10 00 00 4B 2B 00 00 50
            // 01 00 C0 00 1A 80 00 4B 34 00 00 50 
            // 01 80 C2 00 04 00 00 63 7F 00 00 63
            // (axes)
            // 02 00 C0 00 00 00 00 4B 04 00 00 50 brown and green
            // 02 00 C0 00 1C 02 00 4B 0B 00 00 50 
            // 02 10 C0 00 16 00 00 4B 10 00 00 50 
            // 02 00 C0 00 1A 02 00 4B 17 00 00 50 
            // 02 00 C0 00 1E 04 00 4B 1D 00 00 50 
            // 02 00 C0 00 12 08 00 4B 26 00 00 50 
            // 02 00 C0 00 0C 02 00 4B 2E 00 00 50 
            // 02 00 C2 00 08 00 00 4B 36 00 00 50 
            // 02 00 C0 00 14 00 00 4B 38 00 00 50
            // (spears)
            // 03 00 C0 00 00 00 00 4B 04 00 00 50 
            // 03 00 C0 00 0C 00 00 4B 0B 00 00 50 
            // 03 00 C0 00 0E 00 00 4B 10 00 01 50 
            // 03 00 C0 00 12 10 00 4B 17 00 00 50 
            // 03 00 C0 00 1A 00 00 4B 1D 80 00 50 
            // 03 00 C0 00 18 00 00 4B 26 10 00 50 
            // 03 00 C1 00 04 00 00 4B 2E 00 00 50 
            // 03 00 C0 00 1C 80 00 4B 36 00 00 50 
            // 03 00 C0 00 08 00 00 4B 38 00 00 50
            // (whips)
            // 04 00 C0 00 1C 00 00 4B 02 00 00 50 
            // 04 00 C0 00 10 00 00 4B 06 04 00 50 
            // 04 40 C0 00 04 00 00 4B 0C 00 00 50 
            // 04 00 C0 00 02 10 00 4B 11 00 00 50 
            // 04 00 C0 00 0C 20 00 4B 18 00 00 50 
            // 04 00 C0 00 16 04 00 4B 1E 00 00 50 
            // 04 00 C0 00 16 00 00 4B 26 04 00 50 
            // 04 00 C0 00 08 04 0A 4B 2F 00 00 50 
            // 04 00 C0 00 06 00 00 4B 35 00 00 50
            // (bows)
            // 05 00 C0 01 1C 00 00 4B 03 00 00 50 
            // 05 00 C0 01 00 00 00 4B 08 00 00 50 
            // 05 00 C0 01 0C 08 00 4B 0E 00 00 50 
            // 05 00 C0 01 1E 00 00 4B 14 80 00 50 
            // 05 00 C0 01 16 20 00 4B 1B 00 00 50 
            // 05 05 C0 01 02 00 00 4B 23 00 00 50 
            // 05 00 C0 01 0A 00 00 4B 2B 80 00 50 
            // 05 00 C0 01 08 00 14 4B 34 00 00 50 
            // 05 00 C0 01 06 00 00 4B 36 00 00 50
            // (boomerangs)
            // 06 00 C0 03 00 00 00 4B 02 00 00 50 
            // 06 00 C0 05 02 10 00 4B 06 00 00 50 
            // 06 00 C0 03 04 00 00 4B 0C 04 00 50 
            // 06 00 C0 05 06 00 00 4B 11 00 00 50 
            // 06 00 C0 03 0E 04 00 4B 18 00 00 50 
            // 06 00 C0 03 0A 00 00 4B 1E 00 20 50 
            // 06 00 C0 05 10 10 00 4B 26 00 00 50 
            // 06 00 C0 05 0C 00 0A 4B 2F 00 00 50 
            // 06 00 C0 05 08 00 0A 4B 35 00 00 50
            // (javelins)
            // 07 00 C0 02 1C 00 00 4B 02 00 00 50 
            // 07 00 C0 02 02 00 00 4B 06 00 00 50 
            // 07 00 C0 02 04 08 00 4B 0C 00 00 50 
            // 07 00 C0 02 16 00 00 4B 11 00 01 50 
            // 07 00 C0 02 0C 20 00 4B 18 00 00 50 
            // 07 00 C0 02 1A 00 00 4B 1E 00 01 50 
            // 07 05 C0 02 1E 00 00 4B 26 00 00 50 
            // 07 00 C0 02 0E 80 00 4B 2F 00 00 50 
            // 07 00 C0 02 08 00 00 4B 35 00 00 50
            // ^ base graphic
            //          ^ projectile graphic (different for boomerangs & discs)
            //             ^ palette
            //                ^ effective enemy type attribute
            //                   ^ crit
            //                      ^ hit
            //                         ^ damage
            //                            ^^^^ status condition
            //                                  ^^ condition chance - should make this increase with levels
            // note that the menu palette is not part of this, and can be found in a block at 18FDE2 with a bunch of other menu-related stuff.

            // ///////////////////////////////////////////////////////////////////////////
            // possible status conditions, enemy types, & elements that can roll on weapons, their descriptions, and possible palettes
            // ///////////////////////////////////////////////////////////////////////////

            // plain - no elemental, enemy type damage, or status condition
            none = new StatusCondition();
            none.bitmask = 0x0000;
            none.weaponDescription = "";
            none.name = "none";
            none.paletteSets = new WeaponPaletteSet[]
            {
                dullGreen,
                greenBrown,
                manaSwordColorMaybe,
                pink,
                darkDullPurple,
                plainPurple,
                lavender,
                dirtColor,
                whiteish,
                lightBlue,
                pastelPink,
                purpleGray,
                gray,
            };

            // electric weapon + sleep when hit
            StatusCondition stun = new StatusCondition();
            stun.bitmask = 0x0010;
            stun.weaponDescription = "Causes stun/Thunder damage. ";
            stun.name = "slow";
            stun.element = 0x02;
            stun.paletteSets = new WeaponPaletteSet[]
            {
                thunderSaberColor,
            };
            statusConditions.Add(stun);

            // ice weapon + slow (instead of snowman, which is fucking annoying)
            StatusCondition freeze = new StatusCondition();
            freeze.bitmask = 0x0004;
            freeze.weaponDescription = "Causes slow/Ice damage. ";
            freeze.name = "freeze";
            freeze.paletteSets = new WeaponPaletteSet[]
            {
                icyBlueAndPurple,
                grayBlue,
                brightLightBlue,
            };
            freeze.element = 0x04;
            statusConditions.Add(freeze);

            // fire weapon + engulf + increase damage to plants and fish
            StatusCondition engulf = new StatusCondition();
            engulf.bitmask = 0x4000;
            engulf.weaponDescription = "Causes engulf/Fire damage. ";
            engulf.enemyEffect = 0x02; // plants
            engulf.name = "engulf";
            engulf.paletteSets = new WeaponPaletteSet[]
            {
                fiery,
                orangeGoldish,
            };
            engulf.element = 0x08;
            statusConditions.Add(engulf);
            if(christmasMode)
            {
                statusConditions.Add(engulf);
                statusConditions.Add(engulf);
            }

            // dryad weapon + poison
            StatusCondition poison = new StatusCondition();
            poison.bitmask = 0x2000;
            poison.weaponDescription = "Causes poison/Dryad damage. ";
            poison.name = "poison";
            poison.element = 0x80;
            poison.paletteSets = new WeaponPaletteSet[]
            {
                neonGreen,
                purpleRainbow,
                niceGreen,
            };
            statusConditions.Add(poison);

            // cause mute/confuse
            StatusCondition confuse = new StatusCondition();
            confuse.bitmask = 0x0080;
            confuse.weaponDescription = "Chance to confuse. ";
            confuse.name = "confuse";
            statusConditions.Add(confuse);

            // earth weapon + petrify
            StatusCondition stone = new StatusCondition();
            stone.bitmask = 0x0040;
            stone.weaponDescription = "Causes stone/Earth damage. ";
            stone.name = "stone";
            stone.paletteSets = new WeaponPaletteSet[]
            {
                grayish,
                whiteish,
                gray,
            };
            stone.element = 0x01;
            statusConditions.Add(stone);

            // gold color + luna damage
            StatusCondition none_golden = new StatusCondition();
            none_golden.bitmask = 0x0000;
            none_golden.weaponDescription = "Luna damage. ";
            none_golden.name = "golden";
            none_golden.paletteSets = new WeaponPaletteSet[]
            {
                glowyOrange,
                brightYellow,
                orangeGoldish,
            };
            none_golden.element = 0x40;
            extraNoneCategories.Add(none_golden);

            // red color + beast damage
            StatusCondition none_red = new StatusCondition();
            none_red.bitmask = 0x0000;
            none_red.weaponDescription = "Adds Beast damage. ";
            none_red.enemyEffect = 0x08; // beast
            none_red.name = "beast";
            none_red.paletteSets = new WeaponPaletteSet[]
            {
                rusty,
                redBanana,
            };
            extraNoneCategories.Add(none_red);

            // holy damage + damage to evil/undead
            StatusCondition none_holy = new StatusCondition();
            none_holy.bitmask = 0x0000;
            none_holy.weaponDescription = "Adds light/undead damage. ";
            none_holy.enemyEffect = 0x60; // undead, ghosts, etc
            none_holy.name = "holy";
            none_holy.paletteSets = new WeaponPaletteSet[]
            {
                lumina,
                bluePinkRainbow,
                brightYellow,
                brightPink,
            };
            none_holy.element = 0x20;
            extraNoneCategories.Add(none_holy);

            // black color + shade damage + damage against humanoid targets
            StatusCondition none_black = new StatusCondition();
            none_black.bitmask = 0x0000;
            none_black.weaponDescription = "Adds dark/humanoid damage. ";
            none_black.enemyEffect = 0x01; // humanoid
            none_black.name = "black";
            none_black.paletteSets = new WeaponPaletteSet[]
            {
                darkPurple,
                redBanana,
            };
            none_black.element = 0x10;
            extraNoneCategories.Add(none_black);

            // increased damage to dragons
            StatusCondition none_dragon = new StatusCondition();
            none_dragon.bitmask = 0x0000;
            none_dragon.weaponDescription = "Adds Dragon damage. ";
            none_dragon.enemyEffect = 0x80; // dragon
            none_dragon.name = "dragon";
            none_dragon.paletteSets = new WeaponPaletteSet[]
            {
                blueAndGreen,
            };
            extraNoneCategories.Add(none_dragon);

            // no element or type, but increse crit rate
            StatusCondition none_crit = new StatusCondition();
            none_crit.bitmask = 0x0000;
            none_crit.weaponDescription = "Increased crit. ";
            none_crit.enemyEffect = 0x00;
            none_crit.name = "crit";
            none_crit.critRate = 30;
            none_crit.paletteSets = new WeaponPaletteSet[]
            {
                purpleRainbow,
                icyBlueAndPurple,
                blueAndGreen,
            };
            extraNoneCategories.Add(none_crit);

            // no element or type, but 100% hit rate
            StatusCondition none_hit = new StatusCondition();
            none_hit.bitmask = 0x0000;
            none_hit.weaponDescription = "Perfect hitrate. ";
            none_hit.enemyEffect = 0x00;
            none_hit.name = "hit";
            none_hit.hitRate = 100;
            none_hit.paletteSets = none.paletteSets;
            extraNoneCategories.Add(none_hit);

            // shitty hit rate but high crit rate, for effectively a wide damage range
            // i believe crit rate is / 255 (the 100 here is not guaranteed crits)
            StatusCondition none_supercrit = new StatusCondition();
            none_supercrit.bitmask = 0x0000;
            none_supercrit.weaponDescription = "High damage range. ";
            none_supercrit.enemyEffect = 0x00;
            none_supercrit.name = "100crit";
            none_supercrit.hitRate = 50;
            none_supercrit.critRate = 100;
            none_supercrit.paletteSets = none.paletteSets;
            extraNoneCategories.Add(none_supercrit);

            // agi boost
            StatusCondition none_agiPlus = new StatusCondition();
            none_agiPlus.bitmask = 0x0000;
            none_agiPlus.weaponDescription = "Adds agi. ";
            none_agiPlus.boostStat = 0;
            none_agiPlus.name = "agi boost";
            statBoostCategories.Add(none_agiPlus);
            statBoostCategories.Add(none_agiPlus);
            statBoostCategories.Add(none_agiPlus);

            // con boost
            StatusCondition none_conPlus = new StatusCondition();
            none_conPlus.bitmask = 0x0000;
            none_conPlus.weaponDescription = "Adds con. ";
            none_conPlus.boostStat = 1;
            none_conPlus.name = "con boost";
            statBoostCategories.Add(none_conPlus);
            statBoostCategories.Add(none_conPlus);
            statBoostCategories.Add(none_conPlus);

            // int/wis boost
            StatusCondition none_intPlus = new StatusCondition();
            none_intPlus.bitmask = 0x0000;
            none_intPlus.weaponDescription = "Adds int/wis. ";
            none_intPlus.boostStat = 2;
            none_intPlus.name = "int/wis boost";
            statBoostCategories.Add(none_intPlus);
            statBoostCategories.Add(none_intPlus);
            statBoostCategories.Add(none_intPlus);

            // str boost
            StatusCondition none_strPlus = new StatusCondition();
            none_strPlus.bitmask = 0x0000;
            none_strPlus.weaponDescription = "Adds str. ";
            none_strPlus.boostStat = 4;
            none_strPlus.name = "str boost";
            statBoostCategories.Add(none_strPlus);
            statBoostCategories.Add(none_strPlus);
            statBoostCategories.Add(none_strPlus);

            // shit hitrate, gag weapon
            // this will rarely just replace an existing weapon, if allowed
            none_Garbage.bitmask = 0x0000;
            none_Garbage.weaponDescription = "Trash. ";
            none_Garbage.name = "trash";
            none_Garbage.hitRate = 10;

            // now all possible weapons, each with a type from the above.

            ////////////////////////////////////////////////////////////////

            // FIST
            WeaponCategory fistWeapon = new WeaponCategory();
            fistWeapon.indexStart = 0;
            fistWeapon.ranged = false;
            fistWeapon.allMenuIcons = new byte[] { 99, 100, 101, 102 };
            fistWeapon.addWeaponSpec(new WeaponSpec("Obsidian", null, none, "A monk's dark artifact weapon.")); // darkages
            fistWeapon.addWeaponSpec(new WeaponSpec("Tilian Claw", new byte[] { 101 }, none, "A treasure of Loures Canals.")); // darkages
            fistWeapon.addWeaponSpec(new WeaponSpec("Kalkuri", null, none, "A master monk's weapon.")); // darkages
            fistWeapon.addWeaponSpec(new WeaponSpec("Sura's Rampage", null, none, "Best used against Porings.")); // RO
            fistWeapon.addWeaponSpec(new WeaponSpec("Bone Fist", null, none, "Like most fists, it's made of bone."));
            fistWeapon.addWeaponSpec(new WeaponSpec("Cestus", new byte[] { 99 }, none, "Some sort of spiky hand thing."));
            fistWeapon.addWeaponSpec(new WeaponSpec("Titan's Mitt", new byte[] { 99 }, none, "You can pick up level 2 rocks now!")); // lttp
            fistWeapon.addWeaponSpec(new WeaponSpec("Cestus", new byte[] { 99 }, none, "Some sort of spiky hand thing."));
            fistWeapon.addWeaponSpec(new WeaponSpec("Garnet's Gauntlet", new byte[] { 102 }, none_strPlus, "A fusion of two other weapons.", garnetGauntlet)); // steven universe
            fistWeapon.addWeaponSpec(new WeaponSpec("Dragon Claws", new byte[] { 101 }, none_dragon));
            fistWeapon.addWeaponSpec(new WeaponSpec("Baghnakhs", new byte[] { 101 }, none_agiPlus, "Less gross than Terraria's [Fetid Baghnakhs.]")); // terraria
            fistWeapon.addWeaponSpec(new WeaponSpec("Power Glove", new byte[] { 100 }, none_strPlus, "It's so bad."));
            fistWeapon.addWeaponSpec(new WeaponSpec("Heavy Glove", new byte[] { 100 }, none_strPlus, "Not actually very heavy."));
            fistWeapon.addWeaponSpec(new WeaponSpec("Work Gloves", new byte[] { 100 }, none, "You could probably sell this for a lot of mesos.", whiteish)); // maplestory
            fistWeapon.addWeaponSpec(new WeaponSpec("Chakra Hand", null, none_intPlus));
            fistWeapon.addWeaponSpec(new WeaponSpec("Hyper Fist", null, none_agiPlus));
            fistWeapon.addWeaponSpec(new WeaponSpec("Aura Glove", new byte[] { 102 }, none_golden));
            fistWeapon.addWeaponSpec(new WeaponSpec("Deathreach", null, stone)); // diablo
            fistWeapon.addWeaponSpec(new WeaponSpec("Mitten", new byte[] { 100 }, none_Garbage, "It's just a soft, fuzzy mitten.", pastelPink));
            fistWeapon.addWeaponSpec(new WeaponSpec("Super Slap", new byte[] { 100 }, none_agiPlus, "Princess Peach's mega-slap.", pastelPink)); // SMRPG
            fistWeapon.addWeaponSpec(new WeaponSpec("Sticky Glove", null, stun, "Mallow's sticky glove.", whiteish)); // SMRPG
            fistWeapon.addWeaponSpec(new WeaponSpec("Kaiser", new byte[] { 101 }, none_holy, "You feel like you could suplex a train.")); // FF6
            fistWeapon.addWeaponSpec(new WeaponSpec("Godhand", new byte[] { 102 }, none_holy, "Sorry, there's no materia slotted.")); // FF7
            fistWeapon.addWeaponSpec(new WeaponSpec("Flame Claws", new byte[] { 101 }, engulf, "Way warmer than mittens."));
            fistWeapon.addWeaponSpec(new WeaponSpec("Ice Claws", new byte[] { 101 }, freeze, "AKA the [Santa Claws]."));
            fistWeapon.addWeaponSpec(new WeaponSpec("Touch of Death", null, stone, "Not too much death--just a touch."));
            fistWeapon.addWeaponSpec(new WeaponSpec("Griffin Claws", new byte[] { 101 }, poison, "I guess griffins have poisonous claws.")); // mana
            fistWeapon.addWeaponSpec(new WeaponSpec("Moogle Claws", new byte[] { 101 }, poison, "I didn't even know moogles had claws.")); // mana
            fistWeapon.addWeaponSpec(new WeaponSpec("Crisis Arm", new byte[] { 102 }, confuse, "Advanced weapon from 2300 AD.")); // CT
            fistWeapon.mainImage = 0;
            fistWeapon.projectileImages.Add(99, 0);
            fistWeapon.projectileImages.Add(100, 0);
            fistWeapon.projectileImages.Add(101, 0);
            fistWeapon.projectileImages.Add(102, 0);
            fistWeapon.name = "fist";
            weaponCategories.Add(fistWeapon);


            // SWORD
            WeaponCategory swordWeapon = new WeaponCategory();
            swordWeapon.indexStart = 9;
            swordWeapon.ranged = false;
            swordWeapon.allMenuIcons = new byte[] { 84, 85, 86, 87, 88 };
            swordWeapon.addWeaponSpec(new WeaponSpec("Zweihander", null, stun, "A German two-handed sword."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Icebrand", null, freeze, "Use it quickly before it melts."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Vorpal", null, freeze, "A colder version of the Flamberge.")); // phantasia
            swordWeapon.addWeaponSpec(new WeaponSpec("Frostmourne", null, freeze, "Actually just a replica from Dalaran.")); // wow
            swordWeapon.addWeaponSpec(new WeaponSpec("Frostbrand", null, freeze, "Thanks for choosing Frost Brand Swords. Since 1972.")); // terraria
            swordWeapon.addWeaponSpec(new WeaponSpec("Shalla'tor", null, freeze, "Half of the Shalamayne.")); // wow
            swordWeapon.addWeaponSpec(new WeaponSpec("Flamberge", null, engulf, "A warmer version of the Vorpal.")); // phantasia
            swordWeapon.addWeaponSpec(new WeaponSpec("Ashbringer", null, engulf)); // wow
            swordWeapon.addWeaponSpec(new WeaponSpec("Laevatain", null, engulf, "Not to be confused with Leviathan.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Marsil", null, engulf, "A fiery blade from Dracula's castle.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Ellemayne", null, engulf, "The other half of the Shalamayne.")); // wow
            swordWeapon.addWeaponSpec(new WeaponSpec("Ashenrazor", null, engulf)); // everquest
            swordWeapon.addWeaponSpec(new WeaponSpec("Sizzle Sword", null, engulf, "Found on floor 87 of another game's Ancient Cave.")); // lufia2
            swordWeapon.addWeaponSpec(new WeaponSpec("Hrunting", null, poison, "Not a typo of [Grunting.]")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Terminus", null, poison)); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Tizona", null, poison)); // terraria
            swordWeapon.addWeaponSpec(new WeaponSpec("Milican's Sword", null, stone, "An old stoner's weapon.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Executioner", null, stone));
            swordWeapon.addWeaponSpec(new WeaponSpec("Gorgon Blade", null, stone));
            swordWeapon.addWeaponSpec(new WeaponSpec("Joyeuse", null, none_golden, "Just holding it makes you happier.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Vjaya", null, none_golden, "Not as dirty as it sounds."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Quel'Zaram", null, none_golden, "")); // wow
            swordWeapon.addWeaponSpec(new WeaponSpec("Tournesol", null, none_golden, "Features both a sun and moon on the hilt.")); // FF12
            swordWeapon.addWeaponSpec(new WeaponSpec("Hy-Brasyl Escalon", null, none_golden, "A master warrior's weapon.")); // darkages
            swordWeapon.addWeaponSpec(new WeaponSpec("Crimson Blade", null, none_red, "It's a red sword."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Crissaegrim", null, confuse)); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Valmanway", null, confuse)); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Moon Falux", null, confuse, "Found in Dhaos's Castle.")); // phantasia
            swordWeapon.addWeaponSpec(new WeaponSpec("Moon Blade", null, confuse, "A larger version of a sword found in Lagoon Castle.")); // lagoon
            swordWeapon.addWeaponSpec(new WeaponSpec("Masamune", new byte[] { 87 }, none, "Famously wielded by Sephiroth. And Frog. And Edge..."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Muramasa", new byte[] { 87 }, none, "Almost as good as a Masamune."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Eclipse", null, none, "A warrior's dark artifact weapon.")); // darkages
            swordWeapon.addWeaponSpec(new WeaponSpec("Shalamayne", null, none, "Varian Wrynn's legendary sword.")); // wow
            swordWeapon.addWeaponSpec(new WeaponSpec("Tsurugi", null, none, "A double-edged Japanese blade."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Anathema", null, none));
            swordWeapon.addWeaponSpec(new WeaponSpec("Light Saber", null, none, "It isn't very heavy at all!", brightLightBlue));
            swordWeapon.addWeaponSpec(new WeaponSpec("Harkon's Sword", null, none, "You're not a vampire, so it doesn't absorb health.")); // skyrim
            swordWeapon.addWeaponSpec(new WeaponSpec("Atma Weapon", null, none, "It's pure energy.", brightLightBlue)); // FF6
            swordWeapon.addWeaponSpec(new WeaponSpec("Sting", null, none, "Every move you make, it'll be watching you.")); // lotr
            swordWeapon.addWeaponSpec(new WeaponSpec("Buster Sword", new byte[] { 87 }, none, "Cloud hasn't grown into it yet.")); // FF7
            swordWeapon.addWeaponSpec(new WeaponSpec("Sword of Kings", null, none, "It took seven hours of Stonehenge grinding to find.")); // earthbound
            swordWeapon.addWeaponSpec(new WeaponSpec("Rusty Sword", new byte[] { 86 }, none, "Beware of tetanus.", rustySword)); // mana
            swordWeapon.addWeaponSpec(new WeaponSpec("Broad Sword", null, none, "Men can wield it too."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Herald Sword", null, none, "It's long been heralded as.. a pretty good sword.")); // mana
            swordWeapon.addWeaponSpec(new WeaponSpec("Claymore", null, none, "A marked improvement over the Clayless."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Sword Sword", null, none, "Ticondera's greatest treasure.")); // 7th saga
            swordWeapon.addWeaponSpec(new WeaponSpec("Edge of Duality", null, none, "A two-handed Hylian sword.")); // botw
            swordWeapon.addWeaponSpec(new WeaponSpec("Grass Sword", null, poison, "May or may not be cursed.", greenBrown)); // adventure time
            swordWeapon.addWeaponSpec(new WeaponSpec("Demon Blood Sword", new byte[] { 88 }, none, "Finn and Jake's family sword.", demonBlood)); // adventure time
            swordWeapon.addWeaponSpec(new WeaponSpec("Gryffindor Sword", null, none, "May present itself to any worthy Gryffindor.", greenBrown)); // harry potter
            swordWeapon.addWeaponSpec(new WeaponSpec("Butter Knife", new byte[] { 84 }, none_Garbage, "It barely cuts butter, let alone Rabites.", rustySword));
            swordWeapon.addWeaponSpec(new WeaponSpec("Excalipur", new byte[] { 85 }, none_Garbage, "Well, it was good enough for Gilgamesh.", rustySword)); // FF5
            swordWeapon.addWeaponSpec(new WeaponSpec("Dragon Buster", null, none_dragon)); // mana
            swordWeapon.addWeaponSpec(new WeaponSpec("Excalibur", null, none_holy, "You may have seen it in every other game, ever."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Claimh Solais", null, none_holy, "A sword with a sparkling blade.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Luminus", null, none_holy, "A holy sword wielded by Alucard.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Mystletain", null, none_holy, "It's made of wood, so be careful with it.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Master Sword", null, none_holy, "The legendary blade of evil's bane.")); // zelda
            swordWeapon.addWeaponSpec(new WeaponSpec("Einlanzer", null, none_holy, "A holy sword found at the Isle of the Damned.")); // chrono cross
            swordWeapon.addWeaponSpec(new WeaponSpec("Glamdring", null, none_holy, "A rune-inscribed sword wielded by Gandalf.")); // lotr
            swordWeapon.addWeaponSpec(new WeaponSpec("Gurthang", null, none_black, "Alucard's demonic blood-draining sword.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Dainslef", null, none_black, "Alucard's demonic blood-draining sword.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Kaladbolg", null, none_black, "A demonic sword wielded by Soma Cruz.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Mormegil", null, none_black, "A black blade once owned by Alucard.")); // castlevania
            swordWeapon.addWeaponSpec(new WeaponSpec("Tyrfingr", null, none_black, "A blade of Norse legend."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Katzbalger", null, none_black, "A short sword with an S-shaped guard."));
            swordWeapon.addWeaponSpec(new WeaponSpec("Shadowblade", null, none_black));
            swordWeapon.addWeaponSpec(new WeaponSpec("Shard of Hate", null, none_black)); // diablo
            swordWeapon.addWeaponSpec(new WeaponSpec("Defender", null, none_conPlus)); // FF6
            swordWeapon.addWeaponSpec(new WeaponSpec("Mablung", null, none_conPlus)); // castlevania / lotr
            swordWeapon.addWeaponSpec(new WeaponSpec("Quel'Serrar", null, none_conPlus)); // wow
            swordWeapon.addWeaponSpec(new WeaponSpec("Schweizersabel", null, none_conPlus, "It was a steal at 500,000 Zeny.")); // RO
            swordWeapon.addWeaponSpec(new WeaponSpec("Enhancer", null, none_intPlus)); // FF6
            swordWeapon.addWeaponSpec(new WeaponSpec("Apocalypse", null, none_intPlus, "Triple materia growth!")); // FF7
            swordWeapon.addWeaponSpec(new WeaponSpec("Haedonggum", null, none_intPlus)); // ragnarok
            swordWeapon.addWeaponSpec(new WeaponSpec("Rune Blade", null, none_intPlus)); // ff6
            swordWeapon.addWeaponSpec(new WeaponSpec("Ragnarok", null, none_strPlus));
            swordWeapon.addWeaponSpec(new WeaponSpec("Quel'Delar", null, none_strPlus)); // wow
            swordWeapon.addWeaponSpec(new WeaponSpec("Byeollungum", null, none_strPlus)); // RO
            swordWeapon.addWeaponSpec(new WeaponSpec("Krasnaya", null, none_strPlus)); // RO
            swordWeapon.addWeaponSpec(new WeaponSpec("Dekar Blade", null, none_strPlus, "Best used against jelly enemies.")); // lufia 2
            swordWeapon.addWeaponSpec(new WeaponSpec("Gades Blade", null, none_strPlus, "It looked bigger when Gades was holding it.")); // lufia 2
            swordWeapon.addWeaponSpec(new WeaponSpec("Gigas Sword", null, none_strPlus));
            swordWeapon.addWeaponSpec(new WeaponSpec("In-Geom", null, none_strPlus, "A Nephalem's legendary weapon.")); // diablo
            swordWeapon.addWeaponSpec(new WeaponSpec("Rainbow", new byte[] { 86 }, none_strPlus, "Melchior's finest weapon.", bluePinkRainbow)); // CT
            swordWeapon.addWeaponSpec(new WeaponSpec("Arkhalis", null, none_agiPlus, "A light sword designed for high-speed attacks.")); // terraria
            swordWeapon.addWeaponSpec(new WeaponSpec("Thunderfury", null, none_agiPlus)); // wow/diablo
            swordWeapon.addWeaponSpec(new WeaponSpec("Trisection", null, none_agiPlus, "One of the better FFT battle themes.")); // FFT
            swordWeapon.addWeaponSpec(new WeaponSpec("Swallow", null, none_agiPlus, "Once used by Crono. Or Serge, I guess.")); // CT
            swordWeapon.addWeaponSpec(new WeaponSpec("Slasher", null, none_agiPlus)); // CT
            swordWeapon.addWeaponSpec(new WeaponSpec("Anemoi", new byte[] { 87 }, stun, "Infused with wind power extracted from Honkai cores.", manaSwordColorSimilar)); // honkai mei weapon
            swordWeapon.addWeaponSpec(new WeaponSpec("7th Sacred Relic", new byte[] { 87 }, none_holy, "An ancient sword powered by a Herrscher Core.", brightYellow)); // honkai himeko weapon
            swordWeapon.addWeaponSpec(new WeaponSpec("Swan Lake", new byte[] { 86 }, freeze, "Liliya Olenyeva's blade as smooth as a calm lake.", blueAndGreen)); // honkai liliya weapon
            swordWeapon.addWeaponSpec(new WeaponSpec("Sleeping Beauty", new byte[] { 86 }, stun, "Rosaliya Olenyeva's sleeping blade.", brightPink)); // honkai rosaliya weapon
            swordWeapon.addWeaponSpec(new WeaponSpec("Godslayer Surtr", new byte[] { 85 }, engulf, "Wielded by the Vermillion Knight in her final battle.", orangeGoldish)); // honkai himeko weapon
            swordWeapon.addWeaponSpec(new WeaponSpec("Jizo Mitama", new byte[] { 87 }, none_black, "A sword corrupted by Herrscher energy.", stick)); // honkai mei weapon
            swordWeapon.addWeaponSpec(new WeaponSpec("Crystal Edge", new byte[] { 87 }, freeze, "Raiden Mei's frozen blade.", pearlSpear)); // honkai mei weapon
            swordWeapon.addWeaponSpec(new WeaponSpec("Balmung", new byte[] { 86 }, none_dragon, "Sword capable of cutting through dragon scales.", darkPurple)); // honkai mei weapon
            swordWeapon.mainImage = 1;
            swordWeapon.projectileImages.Add(84, 0);
            swordWeapon.projectileImages.Add(85, 0);
            swordWeapon.projectileImages.Add(86, 0);
            swordWeapon.projectileImages.Add(87, 0);
            swordWeapon.projectileImages.Add(88, 0);
            swordWeapon.name = "sword";
            weaponCategories.Add(swordWeapon);


            // AXE
            WeaponCategory axeWeapon = new WeaponCategory();
            axeWeapon.indexStart = 18;
            axeWeapon.ranged = false;
            axeWeapon.allMenuIcons = new byte[] { 89, 90, 91, 92 };
            axeWeapon.addWeaponSpec(new WeaponSpec("Bhuj", null, none, "Named after the sound it makes when it hits stuff.")); // castlevania
            axeWeapon.addWeaponSpec(new WeaponSpec("Voulge", null, none));
            axeWeapon.addWeaponSpec(new WeaponSpec("Ogre Killer", null, none, "The reason they stopped making Shrek movies."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Watts's Axe", null, none, "In a way, every axe is Watts's axe.")); // mana
            axeWeapon.addWeaponSpec(new WeaponSpec("Lode Axe", null, none, "Loading Axe..."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Great Axe", null, none, "It's really great."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Durin's Axe", null, none, "Recovered from deep in the Mines of Moria.")); // lotr
            axeWeapon.addWeaponSpec(new WeaponSpec("Battle Axe", null, none, "An axe used for battling."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Gorehowl", null, none)); // wow
            axeWeapon.addWeaponSpec(new WeaponSpec("Shadowmourne", null, none)); // wow
            axeWeapon.addWeaponSpec(new WeaponSpec("Doom Axe", null, stone, "DOOOOOOOOOOOOOOOOOOOM!"));
            axeWeapon.addWeaponSpec(new WeaponSpec("Bahamut's Tail", null, none_black, "Found in Morlia Gallery.")); // phantasia
            axeWeapon.addWeaponSpec(new WeaponSpec("Talgonite Axe", null, none_black, "It's not hy-brasyl, but it'll do.")); // darkages
            axeWeapon.addWeaponSpec(new WeaponSpec("Were-Buster", null, none_red, "For turning Weres into Weren'ts."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Berdysz", null, none_conPlus)); // RO
            axeWeapon.addWeaponSpec(new WeaponSpec("Crimson Battle Axe", null, none_red, "It's just a big red axe."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Redner", null, none_red, "The Red Nirg's Versalian axe."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Rune Axe", null, none_intPlus, "An axe with runes."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Francisca", null, none_intPlus, "Whoever she is, they named this axe after her."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Giant's Axe", null, poison, "Not only is it giant, it's also poisonous.")); // mana
            axeWeapon.addWeaponSpec(new WeaponSpec("Skorn", null, poison, "Hell hath no fury like... well, this.")); // diablo
            axeWeapon.addWeaponSpec(new WeaponSpec("Golden Axe", null, none_golden, "They made a whole game series about this thing."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Helios", null, none_golden, "From the tower by the same name."));  // maplestory
            axeWeapon.addWeaponSpec(new WeaponSpec("Hy-Brasyl Battle Axe", null, none_golden, "The Carnun Champion's axe.")); // darkages
            axeWeapon.addWeaponSpec(new WeaponSpec("Mega Ax", null, none_strPlus, "Better than a Kilo Ax, worse than a Giga Ax.")); // lufia 2
            axeWeapon.addWeaponSpec(new WeaponSpec("Trisection", null, none_agiPlus, "One of the better battle themes from FFT.")); // FFT song
            axeWeapon.addWeaponSpec(new WeaponSpec("Atom Smasher", null, none_strPlus, "Evermore's best axe. Toaster dog is still stronger.")); // evermore
            axeWeapon.addWeaponSpec(new WeaponSpec("Gigas Axe", null, none_strPlus, "A really big axe."));
            axeWeapon.addWeaponSpec(new WeaponSpec("Stout Axe", null, none_conPlus));
            axeWeapon.addWeaponSpec(new WeaponSpec("Great Value Axe", null, none_Garbage, "Big sale on these at Pandora Walmart."));
            axeWeapon.mainImage = 2;
            axeWeapon.projectileImages.Add(89, 0);
            axeWeapon.projectileImages.Add(90, 0);
            axeWeapon.projectileImages.Add(91, 0);
            axeWeapon.projectileImages.Add(92, 0);
            axeWeapon.name = "axe";
            weaponCategories.Add(axeWeapon);


            // SPEAR
            WeaponCategory spearWeapon = new WeaponCategory();
            spearWeapon.indexStart = 27;
            spearWeapon.ranged = false;
            spearWeapon.allMenuIcons = new byte[] { 103, 104, 105, 106 };
            spearWeapon.addWeaponSpec(new WeaponSpec("Luin of Celtchar", null, engulf, "A flaming spear belonging to Celtchar mac Uthechar.")); // mythological
            spearWeapon.addWeaponSpec(new WeaponSpec("Areadbhair", null, engulf, "Once belonged to Lugh of Tuatha De Danann.")); // mythological
            spearWeapon.addWeaponSpec(new WeaponSpec("Flame Lance", null, engulf, "Keep it away from the Ice Lance."));
            spearWeapon.addWeaponSpec(new WeaponSpec("Ice Lance", null, freeze, "Possibly just a big icicle."));
            spearWeapon.addWeaponSpec(new WeaponSpec("North Pole", null, freeze, "From the weapons division of Santa's Workshop."));
            spearWeapon.addWeaponSpec(new WeaponSpec("Spear", null, none, "It's just a spear."));
            spearWeapon.addWeaponSpec(new WeaponSpec("Heavy Spear", null, none, "Heavier than your average spear."));
            spearWeapon.addWeaponSpec(new WeaponSpec("Partisan", new byte[] { 104 }, none));
            spearWeapon.addWeaponSpec(new WeaponSpec("Halberd", new byte[] { 104 }, none));
            spearWeapon.addWeaponSpec(new WeaponSpec("Bovine Bardiche", new byte[] { 104 }, none, "Not from the cow level.")); // diablo
            spearWeapon.addWeaponSpec(new WeaponSpec("Amenonuhoko", null, none, "An ancient Japanese spear."));
            spearWeapon.addWeaponSpec(new WeaponSpec("Sky Piercer", null, none));
            spearWeapon.addWeaponSpec(new WeaponSpec("Rhongomyniad", null, none));
            spearWeapon.addWeaponSpec(new WeaponSpec("Dragonslayer", null, none, "A spear born from the soul of Ornstein.")); // dark souls
            spearWeapon.addWeaponSpec(new WeaponSpec("Zodiac Spear", null, none, "You missed it in FF12, so now you can use it here instead.")); // FF12
            spearWeapon.addWeaponSpec(new WeaponSpec("Tjungkuletti", null, none, "Said to drain the souls of the vicious porings it defeats.")); // RO
            spearWeapon.addWeaponSpec(new WeaponSpec("Mecha-Halberd", null, none, "The spoils of a really long walk to Midgard.")); // phantasia
            spearWeapon.addWeaponSpec(new WeaponSpec("Pearl's Spear", new byte[] { 107 }, none_intPlus, "It's [strong in the real way.]", pearlSpear)); // steven universe
            spearWeapon.addWeaponSpec(new WeaponSpec("Dragon Lance", null, none_dragon));
            spearWeapon.addWeaponSpec(new WeaponSpec("Ceremonial Trident", new byte[] { 108 }, none, "Found in the water around Zora's Domain.")); // botw
            spearWeapon.addWeaponSpec(new WeaponSpec("Lightscale Trident", new byte[] { 108 }, none, "Mipha's legendary Zora spear.")); // botw
            spearWeapon.addWeaponSpec(new WeaponSpec("Tonbogiri", null, poison)); // terraria
            spearWeapon.addWeaponSpec(new WeaponSpec("Gungnir", null, none_holy, "Borrowed from Odin.")); // mythological
            spearWeapon.addWeaponSpec(new WeaponSpec("Longinus", null, none_holy, ""));
            spearWeapon.addWeaponSpec(new WeaponSpec("Holy Lance", null, none_holy));
            spearWeapon.addWeaponSpec(new WeaponSpec("Pearl Lance", null, none_holy, "Blame Ted Woolsey.")); // FF6
            spearWeapon.addWeaponSpec(new WeaponSpec("Daybreak", null, none_golden, "Now you're ready to take on the Moon Lord.")); // terraria
            spearWeapon.addWeaponSpec(new WeaponSpec("Brionac", null, none_black)); // castlevania
            spearWeapon.addWeaponSpec(new WeaponSpec("Chauve-souris", null, none_black)); // castlevania
            spearWeapon.addWeaponSpec(new WeaponSpec("Ahlspiess", null, none_black)); // RO
            spearWeapon.addWeaponSpec(new WeaponSpec("Geiborg", null, none_intPlus)); // castlevania
            spearWeapon.addWeaponSpec(new WeaponSpec("Zephyrus", null, none_intPlus)); // RO
            spearWeapon.addWeaponSpec(new WeaponSpec("Daedalus Lance", new byte[] { 106 }, none_strPlus, "The best spear from vanilla SoM.", brightLightBlue)); // mana
            spearWeapon.addWeaponSpec(new WeaponSpec("Spear of Justice", new byte[] { 105 }, none_conPlus, "It's filled with determination.", lightBlue)); // undertale song
            spearWeapon.addWeaponSpec(new WeaponSpec("Gigas Lance", null, none_strPlus, "A really big lance."));
            spearWeapon.addWeaponSpec(new WeaponSpec("Crimson Lance", null, none_red, "It's a red lance."));
            spearWeapon.addWeaponSpec(new WeaponSpec("Sprite's Spear", null, confuse, "The other two characters can use it too.")); // mana
            spearWeapon.addWeaponSpec(new WeaponSpec("Oceanid Spear", null, confuse));
            spearWeapon.mainImage = 3;
            spearWeapon.projectileImages.Add(103, 0);
            spearWeapon.projectileImages.Add(104, 0);
            spearWeapon.projectileImages.Add(105, 0);
            spearWeapon.projectileImages.Add(106, 0);
            spearWeapon.name = "spear";
            weaponCategories.Add(spearWeapon);


            // WHIP
            WeaponCategory whipWeapon = new WeaponCategory();
            whipWeapon.indexStart = 36;
            whipWeapon.ranged = true;
            whipWeapon.allMenuIcons = new byte[] { 93, 94, 95, 96, 97, 98 };
            whipWeapon.addWeaponSpec(new WeaponSpec("Whip", null, none, "Just your standard whip."));
            whipWeapon.addWeaponSpec(new WeaponSpec("Chain Whip", null, none, ""));
            whipWeapon.addWeaponSpec(new WeaponSpec("Witch's Locks", null, engulf, "A treasure of the Grand Archives.", darkPurple)); // dark souls
            whipWeapon.addWeaponSpec(new WeaponSpec("Morning Star", new byte[] { 97 }, none, "Doubles as a mattock.")); // FFA
            whipWeapon.addWeaponSpec(new WeaponSpec("Hammer Flail", new byte[] { 96 }, none));
            whipWeapon.addWeaponSpec(new WeaponSpec("Amethyst's Whip", new byte[] { 97 }, none_agiPlus, "Found in a huge room full of garbage.", purpleGray)); // steven universe
            whipWeapon.addWeaponSpec(new WeaponSpec("Flametongue", null, engulf)); // terraria
            whipWeapon.addWeaponSpec(new WeaponSpec("Flamelash", null, engulf)); // terraria
            whipWeapon.addWeaponSpec(new WeaponSpec("Sunfury", null, engulf)); // terraria
            whipWeapon.addWeaponSpec(new WeaponSpec("Shooting Star", null, engulf));
            whipWeapon.addWeaponSpec(new WeaponSpec("Cool Whip", null, freeze, "Why are you putting so much emphasis on the H?")); // family guy
            whipWeapon.addWeaponSpec(new WeaponSpec("Black Whip", null, none_black));
            whipWeapon.addWeaponSpec(new WeaponSpec("Mortal Coil", null, none_black)); // diablo
            whipWeapon.addWeaponSpec(new WeaponSpec("Vampire Killer", null, none_holy, "Normally reserved for Belmonts.")); // castlevania
            whipWeapon.addWeaponSpec(new WeaponSpec("Nebula", null, none_holy, "Soma's evil-seeking chain whip.")); // castlevania
            whipWeapon.addWeaponSpec(new WeaponSpec("Flail of Hope", null, none_holy, "I sure hope it's good."));
            whipWeapon.addWeaponSpec(new WeaponSpec("Rosevine", null, none_red, "Found somewhere near Prontera.")); // RO
            whipWeapon.addWeaponSpec(new WeaponSpec("Crimson Flail", null, none_red, "It's a red whip."));
            whipWeapon.addWeaponSpec(new WeaponSpec("Stem of Nephentes", null, none_intPlus)); // RO
            whipWeapon.addWeaponSpec(new WeaponSpec("Akkhan's Addendum", new byte[] { 97 }, none_intPlus, "", akkhan)); // diablo
            whipWeapon.addWeaponSpec(new WeaponSpec("Nimbus Chain", null, none_agiPlus));
            whipWeapon.addWeaponSpec(new WeaponSpec("Backhand Whip", null, none_agiPlus, "It's just a normal whip, but held awkwardly."));
            whipWeapon.addWeaponSpec(new WeaponSpec("Dragon Whisker", null, none_dragon, "I'm not even sure they have whiskers."));
            whipWeapon.addWeaponSpec(new WeaponSpec("Gigas Flail", null, none_strPlus));
            whipWeapon.mainImage = 4;
            whipWeapon.projectileImages.Add(93, 0);
            whipWeapon.projectileImages.Add(94, 0);
            whipWeapon.projectileImages.Add(95, 0);
            whipWeapon.projectileImages.Add(96, 0);
            whipWeapon.projectileImages.Add(97, 0);
            whipWeapon.projectileImages.Add(98, 0);
            whipWeapon.name = "whip";
            weaponCategories.Add(whipWeapon);


            // BOW
            WeaponCategory bowWeapon = new WeaponCategory();
            bowWeapon.indexStart = 45;
            bowWeapon.ranged = true;
            bowWeapon.allMenuIcons = new byte[] { 111, 112, 113, 114 };
            bowWeapon.addWeaponSpec(new WeaponSpec("Chobin's Bow", null, none, "It's pronounced Sho-BAN.")); // mana
            bowWeapon.addWeaponSpec(new WeaponSpec("Short Bow", null, none, "Shorter than the long bow."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Long Bow", null, none, "Quite a bit longer than the short bow."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Great Bow", null, none, "It's pretty great."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Valkyrie", null, none)); // CT
            bowWeapon.addWeaponSpec(new WeaponSpec("Avelyn", null, none, "Sorry, it only fires one arrow.")); // dark souls
            bowWeapon.addWeaponSpec(new WeaponSpec("Phrenic Bow", null, none, "An old Sheikah weapon.")); // botw
            bowWeapon.addWeaponSpec(new WeaponSpec("Trueshot", null, none, "Way more accurate than the Falseshot.")); // CT
            bowWeapon.addWeaponSpec(new WeaponSpec("Garuda Buster", null, none_strPlus, "If you manage to find a garuda, it won't be happy.")); // mana
            bowWeapon.addWeaponSpec(new WeaponSpec("Ancient Bow", null, none, "Worth the 1,000 Rupees.")); // botw
            bowWeapon.addWeaponSpec(new WeaponSpec("Hellwing", null, engulf)); // terraria
            bowWeapon.addWeaponSpec(new WeaponSpec("Firebolt", null, engulf, "It fires bolts."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Rain of Fire", null, engulf, "You're going to need a stronger umbrella."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Shooting Star", null, engulf));
            bowWeapon.addWeaponSpec(new WeaponSpec("Starfall", null, engulf));
            bowWeapon.addWeaponSpec(new WeaponSpec("Flamecannon", null, engulf));
            bowWeapon.addWeaponSpec(new WeaponSpec("Flare Spread", null, engulf, "Somehow you won that awful race in Alvanista.")); // phantasia
            bowWeapon.addWeaponSpec(new WeaponSpec("Broken Bow", null, none_Garbage, "You may as well just throw the arrows."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Phantasm", null, confuse)); // terraria
            bowWeapon.addWeaponSpec(new WeaponSpec("Chaos Bolt", null, confuse));
            bowWeapon.addWeaponSpec(new WeaponSpec("Wing Bow", null, confuse));
            bowWeapon.addWeaponSpec(new WeaponSpec("Doom Bow", null, stone, "DOOOOOOOOOOOOOOOOOOOM!"));
            bowWeapon.addWeaponSpec(new WeaponSpec("Demonbolt", null, none_black));
            bowWeapon.addWeaponSpec(new WeaponSpec("Shadow Bolt", null, none_black));
            bowWeapon.addWeaponSpec(new WeaponSpec("Ebonbolt", null, none_black));
            bowWeapon.addWeaponSpec(new WeaponSpec("Danetta's Revenge", null, none_black)); // diablo
            bowWeapon.addWeaponSpec(new WeaponSpec("Danetta's Spite", null, none_black)); // diablo
            bowWeapon.addWeaponSpec(new WeaponSpec("Kridershot", null, none_black)); // diablo
            bowWeapon.addWeaponSpec(new WeaponSpec("Black Arrow", null, none_black)); // lotr
            bowWeapon.addWeaponSpec(new WeaponSpec("Red Arrow", null, none_red, "Also includes a bow.")); // lotr
            bowWeapon.addWeaponSpec(new WeaponSpec("Golden Arrow", null, none_golden, "Complementary gift from a hotel in Lake Placid."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Darkmoon Bow", null, none_golden, "It took 5,000 souls to craft.")); // dark souls
            bowWeapon.addWeaponSpec(new WeaponSpec("Bow of Hope", null, none_holy, "I sure hope you like this bow."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Bow & Silver Arrows", null, none_holy, "Only needed one to kill Ganon, these are leftovers.")); // zelda
            bowWeapon.addWeaponSpec(new WeaponSpec("Bow of Light", null, none_holy, "Legendary bow that took out Calamity Ganon.")); // botw
            bowWeapon.addWeaponSpec(new WeaponSpec("Auriel's Bow", null, none_holy, "Legendary undead-killer of ancient Tamriel.")); // skyrim
            bowWeapon.addWeaponSpec(new WeaponSpec("Yoichi's Bow", null, none_strPlus, "A famous samurai's bow.")); // various
            bowWeapon.addWeaponSpec(new WeaponSpec("Ballista", null, none_strPlus, "I guess it's a tiny handheld ballista."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Arbalest", null, none_agiPlus)); // RO
            bowWeapon.addWeaponSpec(new WeaponSpec("Elven Bow", null, none_agiPlus, "Made by elves, for elves, from elves."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Wondershot", null, none_agiPlus, "Forged by Taban himself.")); // CT
            bowWeapon.addWeaponSpec(new WeaponSpec("Artea's Bow", null, none_intPlus, "Artea's stuck with a Cursed Bow, so you can use this for now.")); // lufia 2
            bowWeapon.addWeaponSpec(new WeaponSpec("Elfin Bow", null, none_intPlus, "Made by elves in a hollow tree."));
            bowWeapon.addWeaponSpec(new WeaponSpec("Siren", null, stun, "Marle's favorite bow.")); // CT
            bowWeapon.mainImage = 5;
            bowWeapon.projectileImages.Add(111, 1);
            bowWeapon.projectileImages.Add(112, 1);
            bowWeapon.projectileImages.Add(113, 1);
            bowWeapon.projectileImages.Add(114, 1);
            bowWeapon.name = "bow";
            weaponCategories.Add(bowWeapon);


            // BOOMERANG
            WeaponCategory boomerangWeapon = new WeaponCategory();
            boomerangWeapon.indexStart = 54;
            boomerangWeapon.ranged = true;
            boomerangWeapon.allMenuIcons = new byte[] { 115, 116, 120, 121, 122, 123 }; // boomerang is 115, 116,120,122 are discs, 121,123 are spiky
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Eclipse", new byte[] { 116, 122 }, none, "", darkPurple)); // darkages sword
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Valkyrie", null, none));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Crescent", new byte[] { 116, 122 }, none));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Boomerang", new byte[] { 115 }, none, "Best used on octoroks, tektites, and kangaroos."));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Chakram", new byte[] { 116 }, none));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Lode Boomerang", new byte[] { 115 }, none));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Red Cleaver", new byte[] { 115 }, none_red, "", redCleaver));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Lizal Boomerang", new byte[] { 116 }, none, "Dropped by a Lizalfos.")); // botw
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Cobra Shuttle", new byte[] { 121 }, poison, "A shuttle, with snakes."));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Frizbar", new byte[] { 120 }, none, "For extra-hardcore games of [Ultimate Frisbee.]"));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Shuriken", new byte[] { 123 }, none, "What? This isn't a boomerang at all."));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Ninja's Trump", new byte[] { 123 }, none_strPlus, "As far as Trumps go, you could do way worse."));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Amarok", new byte[] { 116 }, freeze, "It's actually just a yoyo.")); // terraria
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Flamarang", new byte[] { 115 }, engulf, "A flaming boomerang made from Hellstone bars.")); // terraria
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Rising Sun", new byte[] { 120, 122 }, engulf, "When it comes back, it's the Setting Sun.")); // FF7
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Shooting Star", new byte[] { 123 }, engulf));
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Moonring", new byte[] { 116, 122 }, confuse)); // FF5
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Steven's Shield", new byte[] { 120 }, none_conPlus, "A shield made of Rose Quartz. Doubles as a thrown weapon.", pink)); // steven universe
            boomerangWeapon.addWeaponSpec(new WeaponSpec("Magical Boomerang", new byte[] { 115 }, none_intPlus, "If a rupee drops, you'll be ready.", magicalBoomerang)); // zelda
            boomerangWeapon.mainImage = 6;
            boomerangWeapon.projectileImages.Add(115, 3);
            boomerangWeapon.projectileImages.Add(116, 5);
            boomerangWeapon.projectileImages.Add(120, 5);
            boomerangWeapon.projectileImages.Add(121, 5);
            boomerangWeapon.projectileImages.Add(122, 5);
            boomerangWeapon.projectileImages.Add(123, 5);
            boomerangWeapon.name = "boomerang";
            weaponCategories.Add(boomerangWeapon);


            // JAVELIN
            WeaponCategory javelinWeapon = new WeaponCategory();
            javelinWeapon.indexStart = 63;
            javelinWeapon.ranged = true;
            javelinWeapon.allMenuIcons = new byte[] { 107, 108, 109, 110 };
            javelinWeapon.addWeaponSpec(new WeaponSpec("Pole Dart", null, none)); // mana
            javelinWeapon.addWeaponSpec(new WeaponSpec("Javelin", null, none)); // mana
            javelinWeapon.addWeaponSpec(new WeaponSpec("Light Trident", null, none, "So light you can throw it at stuff.")); // mana
            javelinWeapon.addWeaponSpec(new WeaponSpec("Lode Javelin", null, none)); // mana
            javelinWeapon.addWeaponSpec(new WeaponSpec("Fork of Hope", null, none_holy)); // mana
            javelinWeapon.addWeaponSpec(new WeaponSpec("Imp's Fork", null, none_black)); // mana
            javelinWeapon.addWeaponSpec(new WeaponSpec("Elf's Harpoon", null, none_intPlus)); // mana
            javelinWeapon.addWeaponSpec(new WeaponSpec("Dragon Dart", null, none_dragon)); // mana
            javelinWeapon.addWeaponSpec(new WeaponSpec("Valkyrian", null, none_strPlus)); // mana
            javelinWeapon.addWeaponSpec(new WeaponSpec("Valkyrie", null, none));
            javelinWeapon.addWeaponSpec(new WeaponSpec("Shooting Star", null, engulf));
            javelinWeapon.addWeaponSpec(new WeaponSpec("Starfall", null, engulf));
            javelinWeapon.addWeaponSpec(new WeaponSpec("Plastic Fork", new byte[] { 108 }, none_Garbage, "It'll probably break after the first use.", white_fork_thing));
            javelinWeapon.addWeaponSpec(new WeaponSpec("Skylar", null, none_golden, "Bought for 125,000 Mesos.")); // maplestory
            javelinWeapon.addWeaponSpec(new WeaponSpec("Machlear", null, none_black, ""));
            javelinWeapon.mainImage = 7;
            javelinWeapon.projectileImages.Add(107, 2);
            javelinWeapon.projectileImages.Add(108, 2);
            javelinWeapon.projectileImages.Add(109, 2);
            javelinWeapon.projectileImages.Add(110, 2);
            javelinWeapon.name = "javelin";
            weaponCategories.Add(javelinWeapon);


            // generic sounding names from various sources applicable to any weapon
            sharedSpecialNames = new WeaponCategory();
            sharedSpecialNames.addWeaponSpec(new WeaponSpec("Plumbus", null, none, "A regular old plumbus."));
            sharedSpecialNames.addWeaponSpec(new WeaponSpec("Big Chungus", null, none, "Oh lawd."));
            sharedSpecialNames.addWeaponSpec(new WeaponSpec("Toxicity", null, poison, "Disorder, disorder..."));
            sharedSpecialNames.addWeaponSpec(new WeaponSpec("Burninator", null, engulf, "Trogdor, the burninator."));
            sharedSpecialNames.addWeaponSpec(new WeaponSpec("Snow Halation", null, freeze, "Muse's most loved, and SilvaGunner's least loved song."));
            sharedSpecialNames.addWeaponSpec(new WeaponSpec("Apoplexy", null, stun, "Best known as that song from FFT."));
            sharedSpecialNames.addWeaponSpec(new WeaponSpec("Serenity Now", null, stun, "... Insanity later."));
            sharedSpecialNames.addWeaponSpec(new WeaponSpec("Nirvana", null, none_intPlus, "It smells like teen spirit."));
            sharedSpecialNames.addWeaponSpec(new WeaponSpec("Determination", null, none_conPlus, "Do you want to have a bad time?"));
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool doWeaponRando = true;
            bool xmas = false;
            bool vanillaStats = false;

            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            if (randoMode == VanillaRandoSettings.MODE_KEY)
            {
                xmas = settings.get(VanillaRandoSettings.PROPERTYNAME_SPECIAL_MODE) == "xmas";
                doWeaponRando = settings.getBool(VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_WEAPONS);
            }

            if(randoMode == OpenWorldSettings.MODE_KEY)
            {
                doWeaponRando = settings.getBool(OpenWorldSettings.PROPERTYNAME_RANDOMIZE_WEAPONS);
            }

            if(randoMode == VanillaRandoSettings.MODE_KEY || randoMode == OpenWorldSettings.MODE_KEY)
            {
                vanillaStats = true;
            }

            bool includeTrash = settings.getBool(CommonSettings.PROPERTYNAME_INCLUDE_TRASH_WEAPONS);

            if(!doWeaponRando)
            {
                context.workingData.setInt(ELEMENTS_TABLE_OFFSET_HIROM, -1);
                return false;
            }

            initWeapons(xmas);
            
            int elementsTableLocation = context.workingOffset;
            Random random = context.randomFunctional;
            context.workingData.setInt(ELEMENTS_TABLE_OFFSET_HIROM, elementsTableLocation);
            context.workingOffset += 72;

            List<int> statusWeapons = new List<int>();
            // two status weapons of ranged, two of nonranged
            for(int i=0; i < 2; i++)
            {
                int nonRanged = random.Next() % 4;
                while(statusWeapons.Contains(nonRanged))
                {
                    nonRanged = random.Next() % 4;
                }
                statusWeapons.Add(nonRanged);
                int ranged = 4 + (random.Next() % 4);
                while (statusWeapons.Contains(ranged))
                {
                    ranged = 4 + (random.Next() % 4);
                }
                statusWeapons.Add(ranged);
            }
            // other two will only have random effects

            List<string> usedNames = new List<string>();
            List<List<string>> allWeaponNames = new List<List<string>>();
            List<List<string>> allWeaponDescs = new List<List<string>>();
            for (int i=0; i < 8; i++)
            {
                allWeaponNames.Add(new List<string>());
                allWeaponDescs.Add(new List<string>());
            }

            // table for weapon stat effects at 4B79
            // 00 01 05 FB
            // we'll change the 5 here for greater effect
            outRom[0x4B7B] = 10;

            List<int> trashWeapons = new List<int>();
            for (int weaponNum = 0; weaponNum < 9; weaponNum++)
            {
                for (int weaponType = 0; weaponType < 8; weaponType++)
                {
                    Logging.log("weapon type " + weaponType, "spoiler");
                    Logging.log("  weapon # " + weaponNum, "spoiler");
                    WeaponCategory weapon = weaponCategories[weaponType];
                    StatusCondition enemyEffectCategory = none;
                    if (vanillaStats)
                    {
                        if((random.Next() % 2) == 0)
                        {
                            enemyEffectCategory = extraNoneCategories[random.Next() % extraNoneCategories.Count];
                        }
                    }
                    else
                    {
                        enemyEffectCategory = extraNoneCategories[random.Next() % extraNoneCategories.Count];
                    }
                    Logging.log("  enemy effect = " + enemyEffectCategory.name, "spoiler");
                    StatusCondition statEffectCategory = statBoostCategories[random.Next() % statBoostCategories.Count];

                    Logging.log("  stat effect = " + statEffectCategory.name, "spoiler");

                    List<StatusCondition> conditions = new List<StatusCondition>();
                    conditions.Add(none);
                    if(enemyEffectCategory != none)
                    {
                        conditions.Add(enemyEffectCategory);
                    }
                    if (statEffectCategory != none)
                    {
                        conditions.Add(statEffectCategory);
                    }

                    bool statusWeapon = vanillaStats ? (enemyEffectCategory == none || enemyEffectCategory.element == 0) : statusWeapons.Contains(weaponType);
                    if (statusWeapon)
                    {
                        StatusCondition statCondition = statusConditions[random.Next() % statusConditions.Count];
                        // favor 2x over none
                        conditions.Add(statCondition);
                        conditions.Add(statCondition);
                        Logging.log("  stat cond = " + statCondition.name, "spoiler");
                    }

                    List<WeaponSpec> matchingSpecs = weapon.getMatchingSpecs(conditions, sharedSpecialNames);

                    // pick which name and palette we'll use; all the conditions will still apply
                    bool nameFound = false;
                    int iters = 0;
                    string chosenName = "";
                    WeaponSpec chosenSpec = null;
                    if (vanillaStats && weaponType == 1 && weaponNum == 8)
                    {
                        StatusCondition none_strPlus = new StatusCondition();
                        none_strPlus.bitmask = 0x0000;
                        none_strPlus.weaponDescription = "Adds str. ";
                        none_strPlus.boostStat = 4;
                        none_strPlus.name = "str boost";
                        statBoostCategories.Add(none_strPlus);
                        WeaponPaletteSet manaSwordColorMaybe = new WeaponPaletteSet(0x04, 8);
                        chosenSpec = new WeaponSpec("Mana Sword", new byte[] { 88 }, none_strPlus, "It's the legendary Sword of Mana.", manaSwordColorMaybe);
                        chosenName = "Mana Sword";
                    }
                    else
                    {
                        bool trashWeapon = includeTrash && (weaponNum > 0 && weaponNum < 8 && (random.Next() % 50) == 0);
                        bool foundTrash = false;
                        bool bossKillerWeapon = false;
                        if (trashWeapon && !trashWeapons.Contains(weaponType))
                        {
                            List<WeaponSpec> trashList = weapon.getMatchingSpecs(new StatusCondition[] { none_Garbage }.ToList(), null);
                            if(trashList.Count > 0)
                            {
                                Logging.log("  Replaced with trash.", "spoiler");
                                trashWeapons.Add(weaponType);
                                chosenSpec = trashList[random.Next() % trashList.Count];
                                chosenName = chosenSpec.name;
                                foundTrash = true;
                                conditions.Clear();
                                conditions.Add(none_Garbage);
                            }
                        }

                        if (!foundTrash && !bossKillerWeapon)
                        {
                            while (!nameFound && iters < 100)
                            {
                                chosenSpec = matchingSpecs[random.Next() % matchingSpecs.Count];
                                chosenName = chosenSpec.name;
                                if (!usedNames.Contains(chosenName))
                                {
                                    nameFound = true;
                                    usedNames.Add(chosenName);
                                }
                                iters++;
                            }
                        }
                    }

                    Logging.log("  weapon " + weaponNum + " [" + statusWeapon + "] name = " + chosenName + "; iters = " + iters, "spoiler");

                    allWeaponNames[weaponType].Add(chosenName);
                    String weaponDesc = chosenSpec.description;
                    int numSpaces = 62 - weaponDesc.Length;
                    // skip empty description
                    if (numSpaces != 62)
                    {
                        for (int i = 0; i < numSpaces; i++)
                        {
                            weaponDesc += " ";
                        }
                    }

                    WeaponPaletteSet pals = null;
                    StatusCondition cond = chosenSpec.condition;
                    if (cond.paletteSets != null)
                    {
                        // override both with chosenSpec optionally
                        pals = cond.paletteSets[random.Next() % cond.paletteSets.Length];
                    }

                    if(chosenSpec.paletteOverride != null)
                    {
                        pals = chosenSpec.paletteOverride;
                    }

                    if (pals != null)
                    {
                        int menuPalOffset = 0x18FDE2 + weaponType * 18 + weaponNum * 2 + 1;
                        outRom[menuPalOffset] = pals.menuPal;

                        int weaponPalOffset = 0x101000 + weaponType * 9 * 12 + weaponNum * 12 + 4;
                        outRom[weaponPalOffset] = pals.weaponPal;
                    }

                    // override with chosenSpec optionally
                    int menuIconOffset = 0x18FDE2 + weaponType * 18 + weaponNum * 2;
                    byte icon = weapon.allMenuIcons[random.Next() % weapon.allMenuIcons.Length];
                    if (chosenSpec.icons != null)
                    {
                        icon = chosenSpec.icons[random.Next() % chosenSpec.icons.Length];
                    }
                    outRom[menuIconOffset] = icon;

                    // make sure disc boomerangs match the menu icons
                    int projectileTypeOffset = 0x101000 + weaponType * 9 * 12 + weaponNum * 12 + 3;
                    if (weapon.projectileImages.ContainsKey(icon))
                    {
                        byte projectileImage = weapon.projectileImages[icon];
                        outRom[projectileTypeOffset] = projectileImage;
                    }
                    
                    int multiplier = 8;
                    int baseVal = 5;
                    double atk = Math.Round(baseVal + Math.Pow(weaponNum + 1, 1.25) * multiplier);


                    if (weapon.ranged)
                    {
                        // ranged weapons
                        atk *= 0.9;
                    }
                    else if(weapon.indexStart == 0)
                    {
                        // fist
                        atk *= 1.1;
                    }

                    if (vanillaStats && weaponType == 1 && weaponNum == 8)
                    {
                        // mana sword
                        atk = 127;
                    }

                    byte atkPower = (byte)atk;

                    ushort fullBitmask = 0;
                    byte enemyBitmask = 0;
                    byte element = 0;
                    List<int> statEffects = new List<int>();
                    byte maxCrit = 0;
                    int maxHit = 0;

                    List<StatusCondition> descConditions = new List<StatusCondition>();
                    foreach(StatusCondition thisCond in conditions)
                    {
                        if(thisCond.bitmask != 0)
                        {
                            fullBitmask |= thisCond.bitmask;
                            // atk penalty for having status conditions
                            atkPower -= (byte)(1 + weaponNum);
                        }

                        enemyBitmask |= thisCond.enemyEffect;
                        element |= thisCond.element;

                        if (thisCond.boostStat != -1 && !statEffects.Contains(thisCond.boostStat))
                        {
                            statEffects.Add(thisCond.boostStat);
                        }

                        if(thisCond.weaponDescription != null && thisCond.weaponDescription != "" && !descConditions.Contains(thisCond))
                        {
                            weaponDesc += thisCond.weaponDescription;
                            descConditions.Add(thisCond);
                        }

                        if(thisCond.critRate > maxCrit)
                        {
                            maxCrit = thisCond.critRate;
                        }
                        if (thisCond.hitRate > maxHit)
                        {
                            maxHit = thisCond.hitRate;
                        }
                    }

                    if(maxHit == 0)
                    {
                        maxHit = 75;
                    }
                    int critOffset = 0x101000 + weaponType * 9 * 12 + weaponNum * 12 + 6;
                    outRom[critOffset] = maxCrit;
                    int hitOffset = 0x101000 + weaponType * 9 * 12 + weaponNum * 12 + 7;
                    outRom[hitOffset] = (byte)maxHit;

                    // AgCnInWi
                    int statOffset1 = 0x101000 + weaponType * 9 * 12 + weaponNum * 12 + 1;
                    // ......St
                    int statOffset2 = 0x101000 + weaponType * 9 * 12 + weaponNum * 12 + 2;
                    outRom[statOffset1] = 0;
                    outRom[statOffset2] = 0;
                    foreach (int statEffect in statEffects)
                    {
                        switch(statEffect)
                        {
                            // agi
                            case 0:
                                {
                                    outRom[statOffset1] |= 0x80;
                                }
                                break;
                            // con
                            case 1:
                                {
                                    outRom[statOffset1] |= 0x20;
                                }
                                break;
                            // int/wis
                            case 2:
                                {
                                    outRom[statOffset1] |= 0x08;
                                    outRom[statOffset1] |= 0x02;
                                }
                                break;
                            // str
                            case 4:
                                {
                                    outRom[statOffset2] |= 0x02;
                                }
                                break;
                        }
                    }

                    allWeaponDescs[weaponType].Add(weaponDesc);

                    int weaponAtkOffset = 0x101000 + weaponType * 9 * 12 + weaponNum * 12 + 8;
                    if (!vanillaStats)
                    {
                        outRom[weaponAtkOffset] = atkPower;
                    }

                    Logging.log("  atk " + atkPower, "spoiler");

                    int weaponStatusOffset = 0x101000 + weaponType * 9 * 12 + weaponNum * 12 + 9;
                    outRom[weaponStatusOffset] = (byte)fullBitmask;
                    outRom[weaponStatusOffset + 1] = (byte)(fullBitmask>>8);

                    // 40%, 50%...
                    int statusChance = (weaponNum + 4) * 10;
                    if(statusChance > 100)
                    {
                        statusChance = 100;
                    }

                    int weaponStatusChanceOffset = 0x101000 + weaponType * 9 * 12 + weaponNum * 12 + 11;
                    outRom[weaponStatusChanceOffset] = (byte)statusChance;

                    int weaponEnemyTypeOffset = 0x101000 + weaponType * 9 * 12 + weaponNum * 12 + 5;
                    outRom[weaponEnemyTypeOffset] = (byte)enemyBitmask;
                    // $C0 / 49F8 9D 94 E1 STA $E194,x[$7E:E194]   A: 0000 X: 0000 Y: 0000 P: envMxdIZc

                    // write elements table
                    outRom[elementsTableLocation + weaponType * 9 + weaponNum] = element;
                }
            }

            for(int i=0; i < 8; i++)
            {
                WeaponCategory weapon = weaponCategories[i];
                string weaponNames = "";
                for(int j=0; j < 9; j++)
                {
                    weaponNames += allWeaponNames[i][j];
                    if(j != 8)
                    {
                        weaponNames += ", ";
                    }
                }
                Logging.log(weapon.name + " [" + statusWeapons.Contains(i) + "] names: " + weaponNames, "spoiler");
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int thingsNum = NamesOfThings.INDEX_WEAPONS_START + i * 9 + j;
                    context.namesOfThings.setName(thingsNum, allWeaponNames[i][j]);
                }
            }

            // some space for the preceding code
            int totalDescriptionSize = 50;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    // space for all the weapon descriptions in one bank; having them all together makes loading a little easier.
                    totalDescriptionSize += allWeaponDescs[i][j].Length + 1;
                }
            }

            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, totalDescriptionSize);
            byte descriptionBank = (byte)((context.workingOffset >> 16) + 0xC0);

            outRom[0x76495] = 0x22;
            outRom[0x76496] = (byte)(context.workingOffset);
            outRom[0x76497] = (byte)(context.workingOffset >> 8);
            outRom[0x76498] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x76499] = 0xEA;

            // removed sep 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // code from 6ab7 (see above) except with different bank
            outRom[context.workingOffset++] = 0x8B;
            // LDA $bank
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = descriptionBank;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // LDA $0000,y
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // BEQ 08 (to PLB below)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // STA 7e9c00,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x9C;
            outRom[context.workingOffset++] = 0x7E;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // BRA $F3 (to LDA $0000,y above)
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xF3;
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            
            // event 0x94F is where descriptions start.
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    List<byte> weaponNameBytes = VanillaEventUtil.getBytes(allWeaponDescs[i][j]);
                    ushort romOffset = (ushort)context.workingOffset;
                    int eventNum = 0x94F + i * 9 + j;
                    // put in Axxxx header
                    int eventOffsetOffset = 0xA0000 + (eventNum - 0x400) * 2;
                    outRom[eventOffsetOffset] = (byte)romOffset;
                    outRom[eventOffsetOffset + 1] = (byte)(romOffset >> 8);
                    foreach (byte b in weaponNameBytes)
                    {
                        outRom[context.workingOffset++] = b;
                    }
                    outRom[context.workingOffset++] = 0x00;
                }
            }

            return true;
        }

        private class WeaponSpec
        {
            public string name;
            public byte[] icons; // if null, use associated weaponcategory's full list
            public StatusCondition condition;
            public string description;
            public WeaponPaletteSet paletteOverride;

            public WeaponSpec(string name, byte[] icons, StatusCondition condition, string description, WeaponPaletteSet paletteOverride)
            {
                this.name = name;
                this.icons = icons;
                this.condition = condition;
                this.description = description;
                this.paletteOverride = paletteOverride;
            }

            public WeaponSpec(string name, byte[] icons, StatusCondition condition, string description) : this(name, icons, condition, description, null)
            {
            }

            public WeaponSpec(string name, byte[] icons, StatusCondition condition) : this(name, icons, condition, "")
            {
            }

        }

        private class WeaponCategory
        {
            // 0 fist, 9 sword, 18 axe, 27 spear, 36 whip, 45 bow, 54 boomerang, 63 javelin (9 each)
            public int indexStart;
            // spear damage: 4, 11, 16, 23, 29, 38, 46, 54, 56
            // whip damage:  2, 6,  12, 17, 24, 30, 38, 47, 53
            public bool ranged;

            public List<WeaponSpec> weaponSpecs = new List<WeaponSpec>();

            public Dictionary<StatusCondition, byte[]> iconPalettes = new Dictionary<StatusCondition, byte[]>();
            public Dictionary<StatusCondition, byte[]> weaponPalettes = new Dictionary<StatusCondition, byte[]>();

            public byte mainImage;
            // map icon -> projectile image; this will vary only for boomerang
            public Dictionary<byte, byte> projectileImages = new Dictionary<byte, byte>();
            public string name;

            public byte[] allMenuIcons;

            public void addWeaponSpec(WeaponSpec spec)
            {
                weaponSpecs.Add(spec);
            }

            public List<WeaponSpec> getMatchingSpecs(List<StatusCondition> conditions, WeaponCategory sharedSpec)
            {
                List<WeaponSpec> specs = new List<WeaponSpec>();
                foreach(WeaponSpec spec in weaponSpecs)
                {
                    foreach(StatusCondition cond in conditions)
                    {
                        if(spec.condition == cond)
                        {
                            specs.Add(spec);
                            break;
                        }
                    }
                }

                if (sharedSpec != null)
                {
                    // prefer the specific ones 5x over the generic ones
                    specs.AddRange(specs);
                    specs.AddRange(specs);
                    specs.AddRange(specs);
                    specs.AddRange(specs);
                    // add generic ones
                    specs.AddRange(sharedSpec.getMatchingSpecs(conditions, null));
                }
                return specs;
            }
        }

        private class StatusCondition
        {
            public ushort bitmask;

            public string weaponDescription;

            public int boostStat = -1;

            public byte enemyEffect;

            public string name = "";

            public WeaponPaletteSet[] paletteSets;

            public byte critRate = 0;

            public byte hitRate;

            // 01 gnome
            // 02 sylphid
            // 04 undine
            // 08 salamando
            // 10 shade
            // 20 lumina
            // 40 luna
            // 80 dryad
            public byte element = 0;
        }

        private class WeaponPaletteSet
        {
            public byte weaponPal;
            public byte menuPal;

            public WeaponPaletteSet(byte weaponPal, byte menuPal)
            {
                this.weaponPal = weaponPal;
                this.menuPal = menuPal;
            }
        }
    }
}
