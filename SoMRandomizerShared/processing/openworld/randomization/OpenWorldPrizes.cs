using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.openworld;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.openworld.PlandoProperties;

namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// A collection of open world prizes, that can be filtered by selected options.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldPrizes
    {
        // all locations that you need flammie for if starting in potos region
        public static List<string> flammieRequiredLocations = new string[] {
                "mech rider 3 (new item)",
                "buffy (new item)",
                "dread slime (new item)",
                "luna item 1 (spells)",
                "luna item 2 (seed)",
                "lumina spells",
                "lumina seed",
                "shade palace glove orb chest",
                "sunken continent sword orb chest",
                "lumina tower axe orb chest",
                "lumina tower spear orb chest",
                "sunken continent boomerang orb chest",
                "shade spells",
                "shade seed",
                "thunder gigas (new item)",
                "red dragon (new item)",
                "blue dragon (new item)",
                "mana tree (new item)",
                "jehk",
                "hydra (new item)",
                "kettlekin (new item)",
                "shade palace chest",
                "dryad spells",
                "dryad seed",
                "turtle island (sea hare tail)",
                "lighthouse (new item)",
                "axe beak (new item)",
                "snow dragon (new item)",
                "dragon worm (new item)",
                "watermelon (new item)",
                "hexas (new item)",
                "jema at tasnica",
            }.ToList();

        // all locations that you need to break the upper land orb for if you don't have flammie
        public static List<string> upperLandOrbRequiredLocations = new string[] {
                "fire seed",
                "kakkara (moogle belt)",
                "fire palace axe orb chest",
                "fire palace chest 1",
                "fire palace chest 2",
                "santa (new item)",
                "matango flammie",
                "ice castle glove orb chest",
                "northtown ruins sword orb chest",
                "northtown castle axe orb chest",
                "northtown ruins spear orb chest",
                "santa spear orb chest",
                "northtown castle whip orb chest",
                "northtown ruins bow orb chest",
                "northtown castle chest",
                "matango inn javelin orb chest",
                "salamando",
                "mara (tower key)",
                "doom wall",
                "vampire",
                "mech rider 2",
                "metal mantis (new item)",
                "triple tonpole",
            }.ToList();

        public static List<PrizeItem> getForSelectedOptions(RandoSettings settings, RandoContext context, List<PrizeLocation> allLocations)
        {
            List<PrizeItem> allPrizes = new List<PrizeItem>();
            // pull options from settings and processed data from other hacks to determine which prizes we include
            Dictionary<string, List<string>> plandoSettings = context.plandoSettings;
            bool boyExists = context.workingData.getBool(OpenWorldCharacterSelection.BOY_EXISTS);
            bool girlExists = context.workingData.getBool(OpenWorldCharacterSelection.GIRL_EXISTS);
            bool spriteExists = context.workingData.getBool(OpenWorldCharacterSelection.SPRITE_EXISTS);
            bool startWithBoy = context.workingData.getBool(OpenWorldCharacterSelection.START_WITH_BOY);
            bool startWithGirl = context.workingData.getBool(OpenWorldCharacterSelection.START_WITH_GIRL);
            bool startWithSprite = context.workingData.getBool(OpenWorldCharacterSelection.START_WITH_SPRITE);
            bool findBoy = context.workingData.getBool(OpenWorldCharacterSelection.BOY_IN_LOGIC);
            bool findGirl = context.workingData.getBool(OpenWorldCharacterSelection.GIRL_IN_LOGIC);
            bool findSprite = context.workingData.getBool(OpenWorldCharacterSelection.SPRITE_IN_LOGIC);
            string startingChar = context.workingData.get(OpenWorldCharacterSelection.STARTING_CHARACTER);
            int forceStartWeapon = settings.getInt(OpenWorldSettings.PROPERTYNAME_FORCE_START_WEAPON);
            string boyClass = context.workingData.get(OpenWorldClassSelection.BOY_CLASS);
            string girlClass = context.workingData.get(OpenWorldClassSelection.GIRL_CLASS);
            string spriteClass = context.workingData.get(OpenWorldClassSelection.SPRITE_CLASS);
            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            bool girlSpellsExist = context.workingData.getBool(OpenWorldClassSelection.GIRL_MAGIC_EXISTS);
            bool spriteSpellsExist = context.workingData.getBool(OpenWorldClassSelection.SPRITE_MAGIC_EXISTS);
            bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
            bool anyCharsAdded = !context.workingData.getBool(OpenWorldCharacterSelection.START_SOLO);
            bool skipWeaponOrbs = settings.getBool(OpenWorldSettings.PROPERTYNAME_START_WITH_ALL_WEAPON_ORBS);

            Random r = context.randomFunctional;

            string[] prizePrefixes = new string[]
            {
                "%1!",
                "%1.",
                "%1!",
                "You got %1!",
                "You got %1.",
                "You got %1!",
                "Got %1!",
                "Got %1.",
                "Got %1!",
                "It's %1!",
                "It's %1.",
                "Found %1.",
                "Found %1!",
                "Found %1.",
                "Received %1.",
                "Received %1!",
                "Received %1.",
                "Nice, it's %1!",
                "Finally, it's %1!",
                "Just what you always wanted: %1.",
                "Oh hey, it's %1.",
                "Just for you: %1.",
                "Well, if it isn't %1!",
            };


            // grab starting weapons from context
            int boyStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.BOY_START_WEAPON_INDEX);
            int girlStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.GIRL_START_WEAPON_INDEX);
            int spriteStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.SPRITE_START_WEAPON_INDEX);
            // weapons we give as prizes - don't include starter ones
            List<int> weaponIndexes = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }.ToList();
            weaponIndexes.Remove(boyStarterWeapon);
            weaponIndexes.Remove(girlStarterWeapon);
            weaponIndexes.Remove(spriteStarterWeapon);

            List<byte> altStarterWeaponFlags = new List<byte>();
            int starterWeaponId = -1;

            if (startWithBoy && startingChar != "boy")
            {
                altStarterWeaponFlags.Add((byte)(0xC0 + boyStarterWeapon));
                altStarterWeaponFlags.Add((byte)(0xC8 + boyStarterWeapon));
            }
            if (startWithGirl && startingChar != "girl")
            {
                altStarterWeaponFlags.Add((byte)(0xC0 + girlStarterWeapon));
                altStarterWeaponFlags.Add((byte)(0xC8 + girlStarterWeapon));
            }
            if (startWithSprite && startingChar != "sprite")
            {
                altStarterWeaponFlags.Add((byte)(0xC0 + spriteStarterWeapon));
                altStarterWeaponFlags.Add((byte)(0xC8 + spriteStarterWeapon));
            }

            byte starterWeaponFlag = 0;

            if (startingChar == "boy")
            {
                starterWeaponFlag = (byte)(0xC0 + boyStarterWeapon);
                starterWeaponId = boyStarterWeapon;
            }
            if (startingChar == "girl")
            {
                starterWeaponFlag = (byte)(0xC0 + girlStarterWeapon);
                starterWeaponId = girlStarterWeapon;
            }
            if (startingChar == "sprite")
            {
                starterWeaponFlag = (byte)(0xC0 + spriteStarterWeapon);
                starterWeaponId = spriteStarterWeapon;
            }

            // for getting the "sword" in event 0x103, plus whatever we plando to start with
            List<byte> startingRewardsEventData = new List<byte>();
            startingRewardsEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
            startingRewardsEventData.Add(starterWeaponFlag); // ?? at L1
            startingRewardsEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
            startingRewardsEventData.Add((byte)(starterWeaponFlag + 8)); // gotten weapon marker flag
            startingRewardsEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
            startingRewardsEventData.Add(0x08); // used for "update weapons" here
            startingRewardsEventData.Add(EventCommandEnum.HEAL.Value);
            startingRewardsEventData.Add(0x44); // full heal, presumably

            startingRewardsEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the " + SomVanillaValues.weaponByteToName(starterWeaponId)));

            // for anything we plando to start with, remove it from the pool and add its event data to the starting event.
            List<string> startingItems = new List<string>();
            if (plandoSettings.ContainsKey(KEY_LOCATION_START_WITH))
            {
                startingItems = plandoSettings[KEY_LOCATION_START_WITH];
                // Process starting with "Any" for each prize type.
                Plando.processAnyPrizes(startingItems, context);
            }

            List<byte> girlSpriteStarterWeaponEventData = new List<byte>();
            foreach (byte flag in altStarterWeaponFlags)
            {
                girlSpriteStarterWeaponEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                girlSpriteStarterWeaponEventData.Add(flag); // ?? at L1
            }
            girlSpriteStarterWeaponEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
            girlSpriteStarterWeaponEventData.Add(0x08); // used for "update weapons" here
            girlSpriteStarterWeaponEventData.Add(EventCommandEnum.HEAL.Value);
            girlSpriteStarterWeaponEventData.Add(0x44); // full heal, presumably

            if (anyCharsAdded)
            {
                PrizeItem girlSpriteStarterWeapon = new PrizeItem("starter weapon (alt)", "weapon", girlSpriteStarterWeaponEventData.ToArray(), "", 0x00, 1.0);
                allPrizes.Add(girlSpriteStarterWeapon);
            }

            // ////////////////////////
            // GLOVE
            // ////////////////////////
            if (weaponIndexes.Contains(0))
            {
                List<byte> gloveEventData = new List<byte>();
                gloveEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                gloveEventData.Add(0xC8);
                gloveEventData.Add(0x1F);
                gloveEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                gloveEventData.Add(0x96); // event 396 is our "already got it" event
                gloveEventData.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                gloveEventData.Add(0x80);
                gloveEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                gloveEventData.Add(0xC0); // glove at L1
                gloveEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                gloveEventData.Add(0xC8); // glove at L1
                gloveEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                gloveEventData.Add(0x08); // used for "update weapons" here
                gloveEventData.Add(EventCommandEnum.HEAL.Value);
                gloveEventData.Add(0x44); // full heal, presumably
                gloveEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Glove"));
                if (startingItems.Contains(VALUE_PRIZE_GLOVE))
                {
                    startingRewardsEventData.AddRange(gloveEventData.ToArray());
                }
                else
                {
                    allPrizes.Add(new PrizeItem("glove", "weapon", gloveEventData.ToArray(), "the glove", 0xC8, 0.8));
                }
            }


            // ////////////////////////
            // SWORD
            // ////////////////////////
            if (weaponIndexes.Contains(1))
            {
                List<byte> swordEventData = new List<byte>();
                swordEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                swordEventData.Add(0xC9);
                swordEventData.Add(0x1F);
                swordEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                swordEventData.Add(0x96); // event 396 is our "already got it" event
                swordEventData.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                swordEventData.Add(0x89);
                swordEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                swordEventData.Add(0xC1); // sword at L1
                swordEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                swordEventData.Add(0xC9); // sword at L1
                swordEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                swordEventData.Add(0x08); // used for "update weapons" here
                swordEventData.Add(EventCommandEnum.HEAL.Value);
                swordEventData.Add(0x44); // full heal, presumably
                if (startingItems.Contains(VALUE_PRIZE_SWORD))
                {
                    startingRewardsEventData.AddRange(swordEventData.ToArray());
                }
                else
                {
                    swordEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Sword"));
                    allPrizes.Add(new PrizeItem("sword", "weapon", swordEventData.ToArray(), "the sword", 0xc9, 1.5));
                }
            }


            // ////////////////////////
            // AXE
            // ////////////////////////
            if (weaponIndexes.Contains(2))
            {
                List<byte> axeEventData = new List<byte>();
                axeEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                axeEventData.Add(0xCA);
                axeEventData.Add(0x1F);
                axeEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                axeEventData.Add(0x96); // event 396 is our "already got it" event
                axeEventData.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                axeEventData.Add(0x92);
                axeEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                axeEventData.Add(0xC2); // axe at L1
                axeEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                axeEventData.Add(0xCA); // axe at L1
                axeEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                axeEventData.Add(0x08); // used for "update weapons" here
                axeEventData.Add(EventCommandEnum.HEAL.Value);
                axeEventData.Add(0x44); // full heal, presumably
                if (startingItems.Contains(VALUE_PRIZE_AXE))
                {
                    startingRewardsEventData.AddRange(axeEventData.ToArray());
                }
                else
                {
                    axeEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Axe"));
                    allPrizes.Add(new PrizeItem("axe", "weapon", axeEventData.ToArray(), "the axe", 0xca, 2.0));
                }
            }

            // ////////////////////////
            // SPEAR
            // ////////////////////////
            if (weaponIndexes.Contains(3))
            {
                List<byte> spearEventData = new List<byte>();
                spearEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                spearEventData.Add(0xCB);
                spearEventData.Add(0x1F);
                spearEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                spearEventData.Add(0x96); // event 396 is our "already got it" event
                spearEventData.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                spearEventData.Add(0x9B);
                spearEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                spearEventData.Add(0xC3); // spear at L1
                spearEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                spearEventData.Add(0xCB); // spear at L1
                spearEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                spearEventData.Add(0x08); // used for "update weapons" here
                spearEventData.Add(EventCommandEnum.HEAL.Value);
                spearEventData.Add(0x44); // full heal, presumably
                if (startingItems.Contains(VALUE_PRIZE_SPEAR))
                {
                    startingRewardsEventData.AddRange(spearEventData.ToArray());
                }
                else
                {
                    spearEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Spear"));
                    allPrizes.Add(new PrizeItem("spear", "weapon", spearEventData.ToArray(), "the spear", 0xcb, 0.8));
                }
            }


            // ////////////////////////
            // WHIP
            // ////////////////////////
            if (weaponIndexes.Contains(4))
            {
                List<byte> whipEventData = new List<byte>();
                whipEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                whipEventData.Add(0xCC);
                whipEventData.Add(0x1F);
                whipEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                whipEventData.Add(0x96); // event 396 is our "already got it" event
                whipEventData.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                whipEventData.Add(0xA4);
                whipEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                whipEventData.Add(0xC4); // spear at L1
                whipEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                whipEventData.Add(0xCC); // spear at L1
                whipEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                whipEventData.Add(0x08); // used for "update weapons" here
                whipEventData.Add(EventCommandEnum.HEAL.Value);
                whipEventData.Add(0x44); // full heal, presumably
                if (startingItems.Contains(VALUE_PRIZE_WHIP))
                {
                    startingRewardsEventData.AddRange(whipEventData.ToArray());
                }
                else
                {
                    whipEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Whip"));
                    allPrizes.Add(new PrizeItem("whip", "weapon", whipEventData.ToArray(), "the whip", 0xcc, 2.0));
                }
            }

            // ////////////////////////
            // BOW
            // ////////////////////////
            if (weaponIndexes.Contains(5))
            {
                List<byte> bowEventData = new List<byte>();
                bowEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                bowEventData.Add(0xCD);
                bowEventData.Add(0x1F);
                bowEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                bowEventData.Add(0x96); // event 396 is our "already got it" event
                bowEventData.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                bowEventData.Add(0xAD);
                bowEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                bowEventData.Add(0xC5); // bow at L1
                bowEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                bowEventData.Add(0xCD); // bow at L1
                bowEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                bowEventData.Add(0x08); // used for "update weapons" here
                bowEventData.Add(EventCommandEnum.HEAL.Value);
                bowEventData.Add(0x44); // full heal, presumably
                if (startingItems.Contains(VALUE_PRIZE_BOW))
                {
                    startingRewardsEventData.AddRange(bowEventData.ToArray());
                }
                else
                {
                    bowEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Bow"));
                    allPrizes.Add(new PrizeItem("bow", "weapon", bowEventData.ToArray(), "the bow", 0xcd, 0.8));
                }
            }

            // ////////////////////////
            // BOOMERANG
            // ////////////////////////
            if (weaponIndexes.Contains(6))
            {
                List<byte> boomerangEventData = new List<byte>();
                boomerangEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                boomerangEventData.Add(0xCE);
                boomerangEventData.Add(0x1F);
                boomerangEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                boomerangEventData.Add(0x96); // event 396 is our "already got it" event
                boomerangEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                boomerangEventData.Add(0xC6); // boomerang at L1
                boomerangEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                boomerangEventData.Add(0xCE); // boomerang at L1
                boomerangEventData.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                boomerangEventData.Add(0xB6); // pole dart
                boomerangEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                boomerangEventData.Add(0x08); // used for "update weapons" here
                boomerangEventData.Add(EventCommandEnum.HEAL.Value);
                boomerangEventData.Add(0x44); // full heal, presumably
                if (startingItems.Contains(VALUE_PRIZE_BOOMERANG))
                {
                    startingRewardsEventData.AddRange(boomerangEventData.ToArray());
                }
                else
                {
                    boomerangEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Boomerang"));
                    allPrizes.Add(new PrizeItem("boomerang", "weapon", boomerangEventData.ToArray(), "the boomerang", 0xce, 0.8));
                }
            }


            // ////////////////////////
            // JAVELIN
            // ////////////////////////
            if (weaponIndexes.Contains(7))
            {
                List<byte> javelinEventData = new List<byte>();
                javelinEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                javelinEventData.Add(0xCF);
                javelinEventData.Add(0x1F);
                javelinEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                javelinEventData.Add(0x96); // event 396 is our "already got it" event
                javelinEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                javelinEventData.Add(0xC7); // javelin at L1
                javelinEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                javelinEventData.Add(0xCF); // javelin at L1
                javelinEventData.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                javelinEventData.Add(0xBF); // pole dart
                javelinEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                javelinEventData.Add(0x08); // used for "update weapons" here
                javelinEventData.Add(EventCommandEnum.HEAL.Value);
                javelinEventData.Add(0x44); // full heal, presumably
                if (startingItems.Contains(VALUE_PRIZE_JAVELIN))
                {
                    startingRewardsEventData.AddRange(javelinEventData.ToArray());
                }
                else
                {
                    javelinEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Javelin"));
                    allPrizes.Add(new PrizeItem("javelin", "weapon", javelinEventData.ToArray(), "the javelin", 0xcf, 1.0));
                }
            }


            // weigh mana seeds higher for MTR
            double seedScore = 1.0;
            if (goal == OpenWorldGoalProcessor.GOAL_MTR)
            {
                seedScore = 4.0;
            }


            // ////////////////////////
            // WATER SEED
            // ////////////////////////
            List<byte> undineSeedEventData = new List<byte>();
            undineSeedEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            undineSeedEventData.Add(0x90);
            undineSeedEventData.Add(0x1F);
            undineSeedEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            undineSeedEventData.Add(0x96); // event 396 is our "already got it" event
            // have seed at full power
            undineSeedEventData.Add(EventCommandEnum.SET_FLAG.Value);
            undineSeedEventData.Add(0x90);
            undineSeedEventData.Add(0x01);
            // increment total mana power
            undineSeedEventData.Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + 0x05));
            undineSeedEventData.Add(0x94);
            if (startingItems.Contains(VALUE_PRIZE_WATER_SEED))
            {
                startingRewardsEventData.AddRange(undineSeedEventData.ToArray());
            }
            else
            {
                undineSeedEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the water seed"));
                allPrizes.Add(new PrizeItem("water seed", "seed", undineSeedEventData.ToArray(), "the water seed", 0x90, seedScore));
            }


            // ////////////////////////
            // EARTH SEED
            // ////////////////////////
            List<byte> gnomeSeedEventData = new List<byte>();
            gnomeSeedEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            gnomeSeedEventData.Add(0x91);
            gnomeSeedEventData.Add(0x1F);
            gnomeSeedEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            gnomeSeedEventData.Add(0x96); // event 396 is our "already got it" event
            // have seed at full power
            gnomeSeedEventData.Add(EventCommandEnum.SET_FLAG.Value);
            gnomeSeedEventData.Add(0x91);
            gnomeSeedEventData.Add(0x01);
            // increment total mana power
            gnomeSeedEventData.Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + 0x05));
            gnomeSeedEventData.Add(0x94);
            if (startingItems.Contains(VALUE_PRIZE_EARTH_SEED))
            {
                startingRewardsEventData.AddRange(gnomeSeedEventData.ToArray());
            }
            else
            {
                gnomeSeedEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the earth seed"));
                allPrizes.Add(new PrizeItem("earth seed", "seed", gnomeSeedEventData.ToArray(), "the earth seed", 0x91, seedScore));
            }


            // ////////////////////////
            // WIND SEED
            // ////////////////////////
            List<byte> sylphidSeedEventData = new List<byte>();
            sylphidSeedEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            sylphidSeedEventData.Add(0x92);
            sylphidSeedEventData.Add(0x1F);
            sylphidSeedEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            sylphidSeedEventData.Add(0x96); // event 396 is our "already got it" event
            // have seed at full power
            sylphidSeedEventData.Add(EventCommandEnum.SET_FLAG.Value);
            sylphidSeedEventData.Add(0x92);
            sylphidSeedEventData.Add(0x01);
            // increment total mana power
            sylphidSeedEventData.Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + 0x05));
            sylphidSeedEventData.Add(0x94);
            if (startingItems.Contains(VALUE_PRIZE_WIND_SEED))
            {
                startingRewardsEventData.AddRange(sylphidSeedEventData.ToArray());
            }
            else
            {
                sylphidSeedEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the wind seed"));
                allPrizes.Add(new PrizeItem("wind seed", "seed", sylphidSeedEventData.ToArray(), "the wind seed", 0x92, seedScore));
            }


            // ////////////////////////
            // FIRE SEED
            // ////////////////////////
            List<byte> salamandoSeedEventData = new List<byte>();
            salamandoSeedEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            salamandoSeedEventData.Add(0x93);
            salamandoSeedEventData.Add(0x1F);
            salamandoSeedEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            salamandoSeedEventData.Add(0x96); // event 396 is our "already got it" event
            // have seed at full power
            salamandoSeedEventData.Add(EventCommandEnum.SET_FLAG.Value);
            salamandoSeedEventData.Add(0x93);
            salamandoSeedEventData.Add(0x01);
            // increment total mana power
            salamandoSeedEventData.Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + 0x05));
            salamandoSeedEventData.Add(0x94);
            if (startingItems.Contains(VALUE_PRIZE_FIRE_SEED))
            {
                startingRewardsEventData.AddRange(salamandoSeedEventData.ToArray());
            }
            else
            {
                salamandoSeedEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the fire seed"));
                allPrizes.Add(new PrizeItem("fire seed", "seed", salamandoSeedEventData.ToArray(), "the fire seed", 0x93, seedScore));
            }


            // ////////////////////////
            // LIGHT SEED
            // ////////////////////////
            List<byte> luminaSeedEventData = new List<byte>();
            luminaSeedEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            luminaSeedEventData.Add(0x94);
            luminaSeedEventData.Add(0x1F);
            luminaSeedEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            luminaSeedEventData.Add(0x96); // event 396 is our "already got it" event
            // have seed at full power
            luminaSeedEventData.Add(EventCommandEnum.SET_FLAG.Value);
            luminaSeedEventData.Add(0x94);
            luminaSeedEventData.Add(0x01);
            // increment total mana power
            luminaSeedEventData.Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + 0x05));
            luminaSeedEventData.Add(0x94);
            if (startingItems.Contains(VALUE_PRIZE_LIGHT_SEED))
            {
                startingRewardsEventData.AddRange(luminaSeedEventData.ToArray());
            }
            else
            {
                luminaSeedEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the light seed"));
                allPrizes.Add(new PrizeItem("light seed", "seed", luminaSeedEventData.ToArray(), "the light seed", 0x94, seedScore));
            }


            // ////////////////////////
            // DARK SEED
            // ////////////////////////
            List<byte> shadeSeedEventData = new List<byte>();
            shadeSeedEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            shadeSeedEventData.Add(0x95);
            shadeSeedEventData.Add(0x1F);
            shadeSeedEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            shadeSeedEventData.Add(0x96); // event 396 is our "already got it" event
            // have seed at full power
            shadeSeedEventData.Add(EventCommandEnum.SET_FLAG.Value);
            shadeSeedEventData.Add(0x95);
            shadeSeedEventData.Add(0x01);
            // increment total mana power
            shadeSeedEventData.Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + 0x05));
            shadeSeedEventData.Add(0x94);
            if (startingItems.Contains(VALUE_PRIZE_DARK_SEED))
            {
                startingRewardsEventData.AddRange(shadeSeedEventData.ToArray());
            }
            else
            {
                shadeSeedEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the dark seed"));
                allPrizes.Add(new PrizeItem("dark seed", "seed", shadeSeedEventData.ToArray(), "the dark seed", 0x95, seedScore));
            }


            // ////////////////////////
            // MOON SEED
            // ////////////////////////
            List<byte> lunaSeedEventData = new List<byte>();
            lunaSeedEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            lunaSeedEventData.Add(0x96);
            lunaSeedEventData.Add(0x1F);
            lunaSeedEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            lunaSeedEventData.Add(0x96); // event 396 is our "already got it" event
            // have seed at full power
            lunaSeedEventData.Add(EventCommandEnum.SET_FLAG.Value);
            lunaSeedEventData.Add(0x96);
            lunaSeedEventData.Add(0x01);
            // increment total mana power
            lunaSeedEventData.Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + 0x05));
            lunaSeedEventData.Add(0x94);
            if (startingItems.Contains(VALUE_PRIZE_MOON_SEED))
            {
                startingRewardsEventData.AddRange(lunaSeedEventData.ToArray());
            }
            else
            {
                lunaSeedEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the moon seed"));
                allPrizes.Add(new PrizeItem("moon seed", "seed", lunaSeedEventData.ToArray(), "the moon seed", 0x96, seedScore));
            }


            // ////////////////////////
            // DRYAD SEED
            // ////////////////////////
            List<byte> dryadSeedEventData = new List<byte>();
            dryadSeedEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            dryadSeedEventData.Add(0x97);
            dryadSeedEventData.Add(0x1F);
            dryadSeedEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            dryadSeedEventData.Add(0x96); // event 396 is our "already got it" event
            // have seed at full power
            dryadSeedEventData.Add(EventCommandEnum.SET_FLAG.Value);
            dryadSeedEventData.Add(0x97);
            dryadSeedEventData.Add(0x01);
            // increment total mana power
            dryadSeedEventData.Add((byte)(EventCommandEnum.JUMP_SUBR_BASE.Value + 0x05));
            dryadSeedEventData.Add(0x94);
            if (startingItems.Contains(VALUE_PRIZE_DRYAD_SEED))
            {
                startingRewardsEventData.AddRange(dryadSeedEventData.ToArray());
            }
            else
            {
                dryadSeedEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the dryad seed"));
                allPrizes.Add(new PrizeItem("dryad seed", "seed", dryadSeedEventData.ToArray(), "the dryad seed", 0x97, seedScore));
            }


            // event bitfield values for adding spells
            // girl:   ..xxx...
            // sprite: .....xxx
            Dictionary<int, int> spellGrantValues = new Dictionary<int, int>();
            if (boyClass == "OGgirl")
            {
                spellGrantValues[1] = 0x38;
            }
            else if (boyClass == "OGsprite")
            {
                spellGrantValues[1] = 0x07;
            }
            if (girlClass == "OGgirl")
            {
                spellGrantValues[2] = 0x38;
            }
            else if (girlClass == "OGsprite")
            {
                spellGrantValues[2] = 0x07;
            }
            if (spriteClass == "OGgirl")
            {
                spellGrantValues[3] = 0x38;
            }
            else if (spriteClass == "OGsprite")
            {
                spellGrantValues[3] = 0x07;
            }


            // ////////////////////////
            // UNDINE SPELLS
            // ////////////////////////
            if (girlSpellsExist || spriteSpellsExist || goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                List<byte> undineSpellEventData = new List<byte>();
                undineSpellEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                undineSpellEventData.Add(EventFlags.ELEMENT_UNDINE_FLAG);
                undineSpellEventData.Add(0x1F);
                undineSpellEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                undineSpellEventData.Add(0x96); // event 396 is our "already got it" event
                foreach (int key in spellGrantValues.Keys)
                {
                    undineSpellEventData.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    undineSpellEventData.Add((byte)key); // char
                    undineSpellEventData.Add(0xC9); // undine
                    undineSpellEventData.Add((byte)spellGrantValues[key]); // all 3 spells
                }
                undineSpellEventData.Add(EventCommandEnum.SET_FLAG.Value);
                undineSpellEventData.Add(EventFlags.ELEMENT_UNDINE_FLAG);
                undineSpellEventData.Add(0x01);
                if (startingItems.Contains(VALUE_PRIZE_UNDINE))
                {
                    startingRewardsEventData.AddRange(undineSpellEventData.ToArray());
                }
                else
                {
                    undineSpellEventData.AddRange(getRandomGiftText(r, prizePrefixes, "Undine magic"));
                    allPrizes.Add(new PrizeItem("undine spells", "element", undineSpellEventData.ToArray(), "undine magic", EventFlags.ELEMENT_UNDINE_FLAG, 2.0));
                }
            }


            // ////////////////////////
            // GNOME SPELLS
            // ////////////////////////
            if (girlSpellsExist || spriteSpellsExist || goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                List<byte> gnomeSpellEventData = new List<byte>();
                gnomeSpellEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                gnomeSpellEventData.Add(EventFlags.ELEMENT_GNOME_FLAG);
                gnomeSpellEventData.Add(0x1F);
                gnomeSpellEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                gnomeSpellEventData.Add(0x96); // event 396 is our "already got it" event
                foreach (int key in spellGrantValues.Keys)
                {
                    gnomeSpellEventData.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    gnomeSpellEventData.Add((byte)key); // char
                    gnomeSpellEventData.Add(0xC8); // gnome
                    gnomeSpellEventData.Add((byte)spellGrantValues[key]); // all 3 spells
                }
                gnomeSpellEventData.Add(EventCommandEnum.SET_FLAG.Value);
                gnomeSpellEventData.Add(EventFlags.ELEMENT_GNOME_FLAG);
                gnomeSpellEventData.Add(0x01);
                if (startingItems.Contains(VALUE_PRIZE_GNOME))
                {
                    startingRewardsEventData.AddRange(gnomeSpellEventData.ToArray());
                }
                else
                {
                    gnomeSpellEventData.AddRange(getRandomGiftText(r, prizePrefixes, "Gnome magic"));
                    allPrizes.Add(new PrizeItem("gnome spells", "element", gnomeSpellEventData.ToArray(), "gnome magic", EventFlags.ELEMENT_GNOME_FLAG, 2.0));
                }
            }


            // ////////////////////////
            // SYLPHID SPELLS
            // ////////////////////////
            if (girlSpellsExist || spriteSpellsExist || goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                List<byte> sylphidSpellEventData = new List<byte>();
                sylphidSpellEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                sylphidSpellEventData.Add(EventFlags.ELEMENT_SYLPHID_FLAG);
                sylphidSpellEventData.Add(0x1F);
                sylphidSpellEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                sylphidSpellEventData.Add(0x96); // event 396 is our "already got it" event
                foreach (int key in spellGrantValues.Keys)
                {
                    sylphidSpellEventData.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    sylphidSpellEventData.Add((byte)key); // char
                    sylphidSpellEventData.Add(0xCB); // sylphid
                    sylphidSpellEventData.Add((byte)spellGrantValues[key]); // all 3 spells
                }
                sylphidSpellEventData.Add(EventCommandEnum.SET_FLAG.Value);
                sylphidSpellEventData.Add(EventFlags.ELEMENT_SYLPHID_FLAG);
                sylphidSpellEventData.Add(0x01);
                if (startingItems.Contains(VALUE_PRIZE_SYLPHID))
                {
                    startingRewardsEventData.AddRange(sylphidSpellEventData.ToArray());
                }
                else
                {
                    sylphidSpellEventData.AddRange(getRandomGiftText(r, prizePrefixes, "Sylphid magic"));
                    allPrizes.Add(new PrizeItem("sylphid spells", "element", sylphidSpellEventData.ToArray(), "sylphid magic", EventFlags.ELEMENT_SYLPHID_FLAG, 1.5));
                }
            }


            // ////////////////////////
            // SALAMANDO SPELLS
            // ////////////////////////
            if (girlSpellsExist || spriteSpellsExist || goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                List<byte> salamandoSpellEventData = new List<byte>();
                salamandoSpellEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                salamandoSpellEventData.Add(EventFlags.ELEMENT_SALAMANDO_FLAG);
                salamandoSpellEventData.Add(0x1F);
                salamandoSpellEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                salamandoSpellEventData.Add(0x96); // event 396 is our "already got it" event
                foreach (int key in spellGrantValues.Keys)
                {
                    salamandoSpellEventData.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    salamandoSpellEventData.Add((byte)key); // char
                    salamandoSpellEventData.Add(0xCA); // salamando
                    salamandoSpellEventData.Add((byte)spellGrantValues[key]); // all 3 spells
                }
                salamandoSpellEventData.Add(EventCommandEnum.SET_FLAG.Value);
                salamandoSpellEventData.Add(EventFlags.ELEMENT_SALAMANDO_FLAG);
                salamandoSpellEventData.Add(0x01);
                if (startingItems.Contains(VALUE_PRIZE_SALAMANDO))
                {
                    startingRewardsEventData.AddRange(salamandoSpellEventData.ToArray());
                }
                else
                {
                    salamandoSpellEventData.AddRange(getRandomGiftText(r, prizePrefixes, "Salamando magic"));
                    allPrizes.Add(new PrizeItem("salamando spells", "element", salamandoSpellEventData.ToArray(), "salamando magic", EventFlags.ELEMENT_SALAMANDO_FLAG, 1.5));
                }
            }


            // ////////////////////////
            // LUMINA SPELLS
            // ////////////////////////
            if (girlSpellsExist || goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                List<byte> luminaSpellEventData = new List<byte>();
                luminaSpellEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                luminaSpellEventData.Add(EventFlags.ELEMENT_LUMINA_FLAG);
                luminaSpellEventData.Add(0x1F);
                luminaSpellEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                luminaSpellEventData.Add(0x96); // event 396 is our "already got it" event
                foreach (int key in spellGrantValues.Keys)
                {
                    if (spellGrantValues[key] == 0x38) // girl spell list only
                    {
                        luminaSpellEventData.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                        luminaSpellEventData.Add((byte)key); // char
                        luminaSpellEventData.Add(0xCF); // lumina
                        luminaSpellEventData.Add((byte)spellGrantValues[key]); // all 3 spells
                    }
                }
                luminaSpellEventData.Add(EventCommandEnum.SET_FLAG.Value);
                luminaSpellEventData.Add(EventFlags.ELEMENT_LUMINA_FLAG);
                luminaSpellEventData.Add(0x01);
                if (startingItems.Contains(VALUE_PRIZE_LUMINA))
                {
                    startingRewardsEventData.AddRange(luminaSpellEventData.ToArray());
                }
                else
                {
                    luminaSpellEventData.AddRange(getRandomGiftText(r, prizePrefixes, "Lumina magic"));
                    allPrizes.Add(new PrizeItem("lumina spells", "element", luminaSpellEventData.ToArray(), "lumina magic", EventFlags.ELEMENT_LUMINA_FLAG, 2.0));
                }
            }


            // ////////////////////////
            // SHADE SPELLS
            // ////////////////////////
            if (spriteSpellsExist || goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                List<byte> shadeSpellEventData = new List<byte>();
                shadeSpellEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                shadeSpellEventData.Add(EventFlags.ELEMENT_SHADE_FLAG);
                shadeSpellEventData.Add(0x1F);
                shadeSpellEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                shadeSpellEventData.Add(0x96); // event 396 is our "already got it" event
                foreach (int key in spellGrantValues.Keys)
                {
                    if (spellGrantValues[key] == 0x07) // sprite spell list only
                    {
                        shadeSpellEventData.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                        shadeSpellEventData.Add((byte)key); // char
                        shadeSpellEventData.Add(0xCE); // shade
                        shadeSpellEventData.Add((byte)spellGrantValues[key]); // all 3 spells
                    }
                }
                shadeSpellEventData.Add(EventCommandEnum.SET_FLAG.Value);
                shadeSpellEventData.Add(EventFlags.ELEMENT_SHADE_FLAG);
                shadeSpellEventData.Add(0x01);
                if (startingItems.Contains(VALUE_PRIZE_SHADE))
                {
                    startingRewardsEventData.AddRange(shadeSpellEventData.ToArray());
                }
                else
                {
                    shadeSpellEventData.AddRange(getRandomGiftText(r, prizePrefixes, "Shade magic"));
                    allPrizes.Add(new PrizeItem("shade spells", "element", shadeSpellEventData.ToArray(), "shade magic", EventFlags.ELEMENT_SHADE_FLAG, 1.5));
                }
            }


            // ////////////////////////
            // LUNA SPELLS
            // ////////////////////////
            if (girlSpellsExist || spriteSpellsExist || goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                List<byte> lunaSpellEventData = new List<byte>();
                lunaSpellEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                lunaSpellEventData.Add(EventFlags.ELEMENT_LUNA_FLAG);
                lunaSpellEventData.Add(0x1F);
                lunaSpellEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                lunaSpellEventData.Add(0x96); // event 396 is our "already got it" event
                foreach (int key in spellGrantValues.Keys)
                {
                    lunaSpellEventData.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    lunaSpellEventData.Add((byte)key); // char
                    lunaSpellEventData.Add(0xCC); // luna
                    lunaSpellEventData.Add((byte)spellGrantValues[key]); // all 3 spells
                }
                lunaSpellEventData.Add(EventCommandEnum.SET_FLAG.Value);
                lunaSpellEventData.Add(EventFlags.ELEMENT_LUNA_FLAG);
                lunaSpellEventData.Add(0x01);
                if (startingItems.Contains(VALUE_PRIZE_LUNA))
                {
                    startingRewardsEventData.AddRange(lunaSpellEventData.ToArray());
                }
                else
                {
                    lunaSpellEventData.AddRange(getRandomGiftText(r, prizePrefixes, "Luna magic"));
                    allPrizes.Add(new PrizeItem("luna spells", "element", lunaSpellEventData.ToArray(), "luna magic", EventFlags.ELEMENT_LUNA_FLAG, 2.0));
                }
            }


            // ////////////////////////
            // DRYAD SPELLS
            // ////////////////////////
            if (girlSpellsExist || spriteSpellsExist || goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                List<byte> dryadSpellEventData = new List<byte>();
                dryadSpellEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                dryadSpellEventData.Add(EventFlags.ELEMENT_DRYAD_FLAG);
                dryadSpellEventData.Add(0x1F);
                dryadSpellEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                dryadSpellEventData.Add(0x96); // event 396 is our "already got it" event
                foreach (int key in spellGrantValues.Keys)
                {
                    int val = spellGrantValues[key];
                    // clip out mana magic
                    if (val == 0x38)
                    {
                        val = 0x18;
                    }
                    if (val == 0x07)
                    {
                        val = 0x03;
                    }

                    dryadSpellEventData.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                    dryadSpellEventData.Add((byte)key); // char
                    dryadSpellEventData.Add(0xCD); // dryad
                    dryadSpellEventData.Add((byte)val); // all 3 spells
                }

                dryadSpellEventData.Add(EventCommandEnum.SET_FLAG.Value);
                dryadSpellEventData.Add(EventFlags.ELEMENT_DRYAD_FLAG);
                dryadSpellEventData.Add(0x01);

                if (startingItems.Contains(VALUE_PRIZE_DRYAD))
                {
                    startingRewardsEventData.AddRange(dryadSpellEventData.ToArray());
                }
                else
                {
                    dryadSpellEventData.AddRange(getRandomGiftText(r, prizePrefixes, "Dryad magic"));
                    allPrizes.Add(new PrizeItem("dryad spells", "element", dryadSpellEventData.ToArray(), "dryad magic", EventFlags.ELEMENT_DRYAD_FLAG, 1.5));
                }
            }


            // ////////////////////////
            // LUMINA TOWER KEY
            // ////////////////////////
            List<byte> towerKeyEventData = new List<byte>();
            towerKeyEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            towerKeyEventData.Add(0x37);
            towerKeyEventData.Add(0x1F);
            towerKeyEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            towerKeyEventData.Add(0x96); // event 396 is our "already got it" event
            towerKeyEventData.Add(EventCommandEnum.SET_FLAG.Value);
            towerKeyEventData.Add(0x37); // gold tower unlock flag
            towerKeyEventData.Add(0x01); // unlocked
            if (startingItems.Contains(VALUE_PRIZE_GOLD_KEY))
            {
                startingRewardsEventData.AddRange(towerKeyEventData.ToArray());
            }
            else
            {
                towerKeyEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Gold Key"));
                allPrizes.Add(new PrizeItem("gold tower key", "item", towerKeyEventData.ToArray(), "the tower key", 0x37, 1.0));
            }


            // ////////////////////////
            // SEA HARE'S TAIL
            // ////////////////////////
            List<byte> seaHareTailEventData = new List<byte>();
            seaHareTailEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            seaHareTailEventData.Add(0x29);
            seaHareTailEventData.Add(0x1F);
            seaHareTailEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            seaHareTailEventData.Add(0x96); // event 396 is our "already got it" event
            seaHareTailEventData.Add(EventCommandEnum.SET_FLAG.Value);
            seaHareTailEventData.Add(0x29); // flag
            seaHareTailEventData.Add(0x01); // unlocked
            if (startingItems.Contains(VALUE_PRIZE_SEA_HARE_TAIL))
            {
                startingRewardsEventData.AddRange(seaHareTailEventData.ToArray());
            }
            else
            {
                seaHareTailEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Sea Hare Tail"));
                allPrizes.Add(new PrizeItem("sea hare tail", "item", seaHareTailEventData.ToArray(), "the sea hare's tail", 0x29, 1.0));
            }


            // ////////////////////////
            // MOOGLE BELT
            // ////////////////////////
            List<byte> moogleBeltEventData = new List<byte>();
            moogleBeltEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            moogleBeltEventData.Add(EventFlags.OPENWORLD_MOOGLE_BELT_FLAG);
            moogleBeltEventData.Add(0x1F);
            moogleBeltEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            moogleBeltEventData.Add(0x96); // event 396 is our "already got it" event
            moogleBeltEventData.Add(EventCommandEnum.SET_FLAG.Value);
            moogleBeltEventData.Add(EventFlags.OPENWORLD_MOOGLE_BELT_FLAG); // flag
            moogleBeltEventData.Add(0x01); // unlocked
            moogleBeltEventData.Add(0x1E);
            moogleBeltEventData.Add(0x48); // add moogle belt
            if (startingItems.Contains(VALUE_PRIZE_MOOGLE_BELT))
            {
                startingRewardsEventData.AddRange(moogleBeltEventData.ToArray());
            }
            else
            {
                moogleBeltEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Moogle Belt"));
                allPrizes.Add(new PrizeItem("moogle belt", "item", moogleBeltEventData.ToArray(), "the moogle belt", EventFlags.OPENWORLD_MOOGLE_BELT_FLAG, 0.8));
            }


            // ////////////////////////
            // MIDGE MALLET
            // ////////////////////////
            List<byte> midgeMalletEventData = new List<byte>();
            midgeMalletEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
            midgeMalletEventData.Add(EventFlags.MIDGE_MALLET_FLAG);
            midgeMalletEventData.Add(0x1F);
            midgeMalletEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
            midgeMalletEventData.Add(0x96); // event 396 is our "already got it" event
            midgeMalletEventData.Add(EventCommandEnum.SET_FLAG.Value);
            midgeMalletEventData.Add(EventFlags.MIDGE_MALLET_FLAG); // flag
            midgeMalletEventData.Add(0x01); // unlocked
            midgeMalletEventData.Add(0x1E);
            midgeMalletEventData.Add(0x49); // add midge mallet
            if (startingItems.Contains(VALUE_PRIZE_MIDGE_MALLET))
            {
                startingRewardsEventData.AddRange(midgeMalletEventData.ToArray());
            }
            else
            {
                midgeMalletEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Midge Mallet"));
                allPrizes.Add(new PrizeItem("midge mallet", "item", midgeMalletEventData.ToArray(), "the midge mallet", EventFlags.MIDGE_MALLET_FLAG, 0.8));
            }

            List<byte> goldEventFlags = new List<byte>(EventFlags.OPENWORLD_GOLD_FLAGS);

            if (flammieDrumInLogic)
            {
                byte flammieDrumFlag = goldEventFlags[0];
                goldEventFlags.RemoveAt(0);
                // ////////////////////////
                // FLAMMIE DRUM
                // ////////////////////////
                List<byte> flammieDrumEventData = new List<byte>();
                flammieDrumEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                flammieDrumEventData.Add(flammieDrumFlag);
                flammieDrumEventData.Add(0x1F);
                flammieDrumEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                flammieDrumEventData.Add(0x96); // event 396 is our "already got it" event
                flammieDrumEventData.Add(EventCommandEnum.SET_FLAG.Value);
                flammieDrumEventData.Add(flammieDrumFlag); // flag
                flammieDrumEventData.Add(0x01); // unlocked
                flammieDrumEventData.Add(0x1E);
                flammieDrumEventData.Add(0x47); // add flammie drum
                flammieDrumEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Flammie Drum"));
                allPrizes.Add(new PrizeItem("flammie drum", "item", flammieDrumEventData.ToArray(), "the flammie drum", flammieDrumFlag, 4.0));
            }


            if (findBoy && boyExists)
            {
                // ////////////////////////
                // BOY
                // ////////////////////////
                List<byte> boyEventData = new List<byte>();
                boyEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                boyEventData.Add(0x0C);
                boyEventData.Add(0x1F);
                boyEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                boyEventData.Add(0x96); // event 396 is our "already got it" event
                // doing name character with dialogue box open breaks it a bit; close first
                boyEventData.Add(EventCommandEnum.CLOSE_DIALOGUE.Value);
                boyEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                boyEventData.Add(0x00); // name the boy
                boyEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                boyEventData.Add(0x0C); // boy gotten flag
                boyEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                boyEventData.Add((byte)(boyStarterWeapon + 0xC8)); // weapon gotten
                boyEventData.Add(EventCommandEnum.ADD_CHARACTER.Value);
                boyEventData.Add(0x00); // add the boy
                boyEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                boyEventData.Add(0x08); // used for "update weapons" here
                boyEventData.Add(EventCommandEnum.HEAL.Value);
                boyEventData.Add(0x44); // full heal, presumably
                boyEventData.Add(EventCommandEnum.OPEN_DIALOGUE.Value);
                boyEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Boy, " + VanillaEventUtil.BOY_NAME_INDICATOR));
                allPrizes.Add(new PrizeItem("boy", "item", boyEventData.ToArray(), "the boy", 0x0C, 3.5));
            }


            if (findGirl && girlExists)
            {
                // ////////////////////////
                // GIRL
                // ////////////////////////
                List<byte> girlEventData = new List<byte>();
                girlEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                girlEventData.Add(0x0D);
                girlEventData.Add(0x1F);
                girlEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                girlEventData.Add(0x96); // event 396 is our "already got it" event
                // doing name character with dialogue box open breaks it a bit; close first
                girlEventData.Add(EventCommandEnum.CLOSE_DIALOGUE.Value);
                girlEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                girlEventData.Add(0x01); // name the girl
                girlEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                girlEventData.Add(0x0D); // girl gotten flag
                girlEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                girlEventData.Add((byte)(girlStarterWeapon + 0xC8)); // weapon gotten
                girlEventData.Add(EventCommandEnum.ADD_CHARACTER.Value);
                girlEventData.Add(0x01); // add the girl
                girlEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                girlEventData.Add(0x08); // used for "update weapons" here
                girlEventData.Add(EventCommandEnum.HEAL.Value);
                girlEventData.Add(0x44); // full heal, presumably
                girlEventData.Add(EventCommandEnum.OPEN_DIALOGUE.Value);
                girlEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Girl, " + VanillaEventUtil.GIRL_NAME_INDICATOR));
                allPrizes.Add(new PrizeItem("girl", "item", girlEventData.ToArray(), "the girl", 0x0D, 3.5));
            }


            if (findSprite && spriteExists)
            {
                // ////////////////////////
                // SPRITE
                // ////////////////////////
                List<byte> spriteEventData = new List<byte>();
                spriteEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                spriteEventData.Add(0x0E);
                spriteEventData.Add(0x1F);
                spriteEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                spriteEventData.Add(0x96); // event 396 is our "already got it" event
                spriteEventData.Add(EventCommandEnum.CLOSE_DIALOGUE.Value);
                spriteEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                spriteEventData.Add(0x02); // name the sprite
                spriteEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                spriteEventData.Add(0x0E); // sprite gotten flag
                spriteEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                spriteEventData.Add((byte)(spriteStarterWeapon + 0xC8)); // weapon gotten
                spriteEventData.Add(EventCommandEnum.ADD_CHARACTER.Value);
                spriteEventData.Add(0x02); // add the sprite
                spriteEventData.Add(EventCommandEnum.NAME_CHARACTER.Value);
                spriteEventData.Add(0x08); // used for "update weapons" here
                spriteEventData.Add(EventCommandEnum.HEAL.Value);
                spriteEventData.Add(0x44); // full heal, presumably
                spriteEventData.Add(EventCommandEnum.OPEN_DIALOGUE.Value);
                spriteEventData.AddRange(getRandomGiftText(r, prizePrefixes, "the Sprite, " + VanillaEventUtil.SPRITE_NAME_INDICATOR));
                allPrizes.Add(new PrizeItem("sprite", "item", spriteEventData.ToArray(), "the sprite", 0x0E, 3.5));
            }
            
            List<PrizeLocation> filteredLocations = goal == OpenWorldGoalProcessor.GOAL_GIFTMODE ? GiftModeProcessing.processLocations(settings, context, allLocations, allPrizes) : allLocations;

            int numGoldEvents = 0;
            // subtract one for boy starter weapon
            int maxPrizes = filteredLocations.Count;

            // repurpose spell flags if they don't exist
            if (goal != OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                if (!girlSpellsExist)
                {
                    goldEventFlags.Add(EventFlags.ELEMENT_LUMINA_FLAG);
                }
                if (!spriteSpellsExist)
                {
                    goldEventFlags.Add(EventFlags.ELEMENT_SHADE_FLAG);
                }
                if (!girlSpellsExist && !spriteSpellsExist)
                {
                    goldEventFlags.Add(EventFlags.ELEMENT_UNDINE_FLAG);
                    goldEventFlags.Add(EventFlags.ELEMENT_GNOME_FLAG);
                    goldEventFlags.Add(EventFlags.ELEMENT_SYLPHID_FLAG);
                    goldEventFlags.Add(EventFlags.ELEMENT_SALAMANDO_FLAG);
                    goldEventFlags.Add(EventFlags.ELEMENT_LUNA_FLAG);
                    goldEventFlags.Add(EventFlags.ELEMENT_DRYAD_FLAG);
                }
            }

            int maxGoldEvents = goldEventFlags.Count;

            double goldMultiplier = settings.getDouble(OpenWorldSettings.PROPERTYNAME_NUMERIC_GOLD_CHECK_MULTIPLIER);
            while (numGoldEvents < maxGoldEvents && allPrizes.Count < maxPrizes)
            {
                List<byte> give1000GoldEventData = new List<byte>();
                give1000GoldEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                give1000GoldEventData.Add(goldEventFlags[numGoldEvents]);
                give1000GoldEventData.Add(0x1F);
                // event 396 is our "already got it" event
                give1000GoldEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                give1000GoldEventData.Add(0x96);
                int goldAmount = (int)(100 * goldMultiplier + (r.Next() % 10) * 100 * goldMultiplier);
                if (goldAmount > 65535)
                {
                    goldAmount = 65535;
                }
                give1000GoldEventData.Add(0x36);
                give1000GoldEventData.Add((byte)(goldAmount));
                give1000GoldEventData.Add((byte)(goldAmount >> 8));
                give1000GoldEventData.AddRange(getRandomGiftText(r, prizePrefixes, goldAmount + " gold"));
                give1000GoldEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                give1000GoldEventData.Add(goldEventFlags[numGoldEvents]);

                allPrizes.Add(new PrizeItem("GP " + numGoldEvents + ": " + goldAmount, "", give1000GoldEventData.ToArray(), "some gold", goldEventFlags[numGoldEvents], 0.4));
                numGoldEvents++;
            }

            int orbEventNum = 0x500;
            byte[] orbEventFlags = EventFlags.OPENWORLD_ORB_FLAGS;

            // do this down here since it can have plando stuff now
            allPrizes.Add(new PrizeItem("starter weapon (main)", "weapon", startingRewardsEventData.ToArray(), "", 0x00, 1.0));

            int orbNum = 0;
            while (allPrizes.Count < maxPrizes && orbNum < orbEventFlags.Length)
            {
                int weaponId = orbEventNum - 0x500;
                List<byte> orbEventData = new List<byte>();
                // reconstruct instead so it's dialogue box friendly here
                orbEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                orbEventData.Add(orbEventFlags[orbNum]);
                orbEventData.Add(0x1F);
                // event 396 is our "already got it" event
                orbEventData.Add((byte)(EventCommandEnum.JUMP_BASE.Value + 0x03));
                orbEventData.Add(0x96);
                orbEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                orbEventData.Add(orbEventFlags[orbNum]);

                if (skipWeaponOrbs)
                {
                    int goldAmount = (int)(100 * goldMultiplier + (r.Next() % 10) * 100 * goldMultiplier);
                    if (goldAmount > 65535)
                    {
                        goldAmount = 65535;
                    }

                    orbEventData.Add(0x36);
                    orbEventData.Add((byte)(goldAmount));
                    orbEventData.Add((byte)(goldAmount >> 8));
                    orbEventData.AddRange(getRandomGiftText(r, prizePrefixes, goldAmount + " gold"));
                    allPrizes.Add(new PrizeItem("GP " + numGoldEvents + ": " + goldAmount, "", orbEventData.ToArray(), "some gold", orbEventFlags[orbNum], 0.4));
                    numGoldEvents++;
                    orbNum++;
                }
                else
                {
                    // this will exit the whole event; not desirable since could be other stuff happening - fixed?
                    orbEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                    orbEventData.Add((byte)(0xB8 + weaponId));
                    orbEventData.Add(0x08);
                    orbEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                    orbEventData.Add((byte)(0xb8 + weaponId));

                    orbEventData.Add(EventCommandEnum.EVENT_LOGIC.Value);
                    orbEventData.Add((byte)(0xC0 + weaponId));
                    orbEventData.Add(0x08);
                    orbEventData.Add(EventCommandEnum.INCREMENT_FLAG.Value);
                    orbEventData.Add((byte)(0xC0 + weaponId));

                    string prefix = "a";
                    if (weaponId == 2)
                    {
                        prefix = "an";
                    }
                    orbEventData.AddRange(getRandomGiftText(r, prizePrefixes, prefix + " " + SomVanillaValues.weaponByteToName(weaponId) + "'s Orb"));

                    orbEventNum++;
                    if (orbEventNum == 0x508)
                    {
                        orbEventNum = 0x500;
                    }

                    allPrizes.Add(new PrizeItem(SomVanillaValues.weaponByteToName(weaponId) + " orb", "", orbEventData.ToArray(), "a weapon orb", orbEventFlags[orbNum], 0.5));
                    orbNum++;
                }
            }

            // pad the prize list with nothings to make up for all our plando non-existents
            int numNonExistent = 0;
            if (plandoSettings.ContainsKey("(NON-EXISTENT)"))
            {
                numNonExistent = plandoSettings["(NON-EXISTENT)"].Count;
            }
            while (allPrizes.Count < maxPrizes + numNonExistent)
            {
                int weaponId = orbEventNum - 0x500;
                List<byte> nothingEventData = new List<byte>();
                string[] nothingPhrases = new string[]
                {
                    "Nothing!",
                    "Sorry nothing",
                    "Oops no item",
                    "Someone else got your item",
                    "Nothing at all!"
                };
                if ((r.Next()) % 2 == 0)
                {
                    nothingEventData.AddRange(getRandomGiftText(r, prizePrefixes, "Nothing"));
                }
                else
                {
                    string nothingPhrase = nothingPhrases[r.Next() % nothingPhrases.Length];
                    nothingEventData.AddRange(VanillaEventUtil.getBytes(nothingPhrase));
                }
                allPrizes.Add(new PrizeItem("nothing", "", nothingEventData.ToArray(), "nothing", 0x00, 0.1));
            }

            return allPrizes;
        }

        private static List<byte> getRandomGiftText(Random r, string[] formats, string prizeName)
        {
            string format = formats[r.Next() % formats.Length];
            format = format.Replace("%1", prizeName);
            // capitalize
            if (format[0] >= 'a' && format[0] <= 'z')
            {
                format = ((char)(((format[0] - 'a') + 'A'))) + format.Substring(1);
            }
            return VanillaEventUtil.getBytes(VanillaEventUtil.wordWrapText(format));
        }

    }
}
