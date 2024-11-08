using SoMRandomizer.config.settings;
using SoMRandomizer.config.ui;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SoMRandomizer.forms
{
    // parts of MainForm related to setting up user-selectable properties for every mode.
    public partial class MainForm
    {
        private void initCommonComponents()
        {
            initTabControl(tabMode, "mode", commonSettings); // mode selector

            generalOptionsCategory = new OptionsCategoryForm(propertyManager, "General options", "General options - categories");

            propertyManager.makePropertyClass("generalDifficulty", "General - difficulty options");
            propertyManager.makePropertyClass("generalBugFix", "General - vanilla bugfixes");
            propertyManager.makePropertyClass("generalEnhancements", "General - game enhancements");
            propertyManager.makePropertyClass("generalMiscMechanical", "General - misc mechanic changes");
            propertyManager.makePropertyClass("generalCosmetic", "General - cosmetic changes");
            propertyManager.makePropertyClass("generalRandomizations", "General - misc randomizations");
            propertyManager.makePropertyClass("generalStupid", "General - dumb stuff");

            generalOptionsCategory.addCategory("Enhancements", "Additions to vanilla game mechanics", "generalEnhancements");
            generalOptionsCategory.addCategory("Bug fixes", "Fixes to vanilla SoM bugs", "generalBugFix");
            generalOptionsCategory.addCategory("Difficulty", "Options that adjust the difficulty of the game", "generalDifficulty");
            generalOptionsCategory.addCategory("Misc. mechanics changes", "Other changes to vanilla mechanics", "generalMiscMechanical");
            generalOptionsCategory.addCategory("Cosmetic changes", "Changes to the appearance or sound of the game", "generalCosmetic");
            generalOptionsCategory.addCategory("Misc. randomizations", "Other randomizations", "generalRandomizations");
            generalOptionsCategory.addCategory("Stupid stuff", "Probably just don't use these.", "generalStupid");
            // difficulty
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_AGGRESSIVE_BOSSES, "Aggressive bosses", "Bosses act every frame instead of every five.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_AGGRESSIVE_ENEMIES, "Aggressive enemies", "(Experimental) Normal enemies will be more aggressive.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_HITTABLE_VAMPIRE, "Hittable Vampires", "Vampire bosses are still hittable with physical attacks in their cloaked form.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_GIGAS_FIX, "Gigases don't split", "To save some time in boss fights, gigases won't do their animation where they just fly around for a while.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_BEAK_DISABLE, "Beak Shield Disable", "Disable Spring and Axe Beak's beak-shield so you can hit them from the front.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_NO_ENERGY_TO_RUN, "No stamina cost to run", "Modify running to not cost stamina.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_ENEMY_POSITION_FIX, "Enemy position adjustment", "Adjust some vanilla enemy positions to be away from doorways to reduce stunlocks after walking in a room.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_OBSCURE_DAMAGE, "Obscure damage", "Show all damage dealt to both enemies and players as zero.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_OBSCURE_OWN_HP, "Obscure own HP", "Don't show your own HP at the screen bottom or on the status screen.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_OBSCURE_GOLD, "Obscure own gold", "Don't show your own gold amount at shops or on the status screen.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalDifficulty", CommonSettings.PROPERTYNAME_NO_WEAPON_STAMINA_COST, "Attacks don't cost stamina", "Full damage with every attack!  Very satisfying.", commonSettings);
            // bugfix
            propertyManager.makeBooleanValueProperty("generalBugFix", CommonSettings.PROPERTYNAME_OVERCHARGE_FIX, "Overcharge fix", "Fix overcharge glitches activated by swapping weapons or changing AI charge levels.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalBugFix", CommonSettings.PROPERTYNAME_ORB_REWARD_FIX, "Orb reward fix", "Fix an issue where having too many of a weapon orb can softlock when you are given another.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalBugFix", CommonSettings.PROPERTYNAME_BOSS_DEATH_FIX, "Boss death fix", "Fix an issue where character positioning at the time a boss dies could cause softlocks.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalBugFix", CommonSettings.PROPERTYNAME_WHIP_COORDINATE_CORRUPTION_FIX, "Whip coordinate corruption fix", "Certain enemy animations corrupt the whip post coordinates, making these posts not work well while the enemies are alive.  This should fix the issue.  Also extends the downward reach of the whip, to work around another occasional whip problem.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalBugFix", CommonSettings.PROPERTYNAME_SUMMONING_FIX, "Summoning Despawn Fix", "Don't allow summoning enemies to despawn chests, spell orbs, or Neko, even if they are off-screen.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalBugFix", CommonSettings.PROPERTYNAME_CONFUSION_FIX, "Confusion Fix", "Don't save control inversion to SaveRAM to avoid the bug where confusion gets stuck on indefinitely. Don't invert controls in menus and on Flammie when confused/inverted.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalBugFix", CommonSettings.PROPERTYNAME_MODE7_EDGES_FIX, "Mode 7 edges fix", "Fix a vanilla issue that occasionally made the corners of the Mode-7 views display the wrong tiles", commonSettings);
            propertyManager.makeBooleanValueProperty("generalBugFix", CommonSettings.PROPERTYNAME_MECHRIDER_DEATH_FIX, "Mech rider death fix", "Mech riders 1 and 2 won't run away, sometimes causing the graphics layers to become offset, and occasionally not allow you to enter doorways. Credit to Regrs for this hack.", commonSettings);
            propertyManager.makeMultiStringValueProperty("generalBugFix", CommonSettings.PROPERTYNAME_DAMAGE_CANCEL_ALLOW_TYPE, "Damage cancel", "Allow damage canceling for the selected item types:"
                + Environment.NewLine + "- All: Vanilla behavior; cancel damage for all item uses."
                + Environment.NewLine + "- Consumables: Disallow for moogle belt and midge mallet."
                + Environment.NewLine + "- None: Disallow for all items. When using a healing consumable, damage will be taken after the healing finishes."
                , commonSettings);
            propertyManager.makeBooleanValueProperty("generalBugFix", CommonSettings.PROPERTYNAME_MAGIC_ROPE_DEATH_FIX, "Magic rope death fix", "Fix a vanilla issue that always made the boy invulnerable when using the magic rope, instead of who used it.", commonSettings);
            // enhancement
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_SPEEDUP_CHANGE, "Speedup improvements", "Gnome speedup spell improves movement rate, recharge rate, and weapon charge rate for player characters.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_LEVELUPS_RESTORE_MP, "MP Restore at level up", "Level ups restore MP as well as HP.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_ENEMY_SPECIES_DAMAGE, "Enemy species damage", "1.5 times attack against targets whose species is weak to the weapon you have equipped.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_WEAPON_ELEMENTAL_DAMAGE, "Elemental melee damage", "Elemental sabers will cause 1.5 times damage against the correct element, and half damage against the wrong element.  Random weapons will sometimes have their own elements, too.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_EXTENDED_TARGETTING, "Extended targetting", "Allow off-screen targets to be selected for spells and items.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_MAX_7_ITEMS, "7 items max", "Increase max consumables from 4 to 7.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_SCROLL_FIX, "Allow leaving characters behind", "Allow scrolling characters off the screen. NOT recommended for multi-player runs!", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_FASTER_MAGIC_LEVELS, "Faster magic levels", "Spells level at 4x the speed of vanilla.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_FASTER_WEAPON_LEVELS, "Faster weapon levels", "Weapons level at 4x the speed of vanilla.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_FASTER_CHESTS, "Faster chest opening", "Remove the shake animation from chests when opening them.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_FASTER_TOP_SCREEN_DIALOGUE, "Faster messages", "Messages at the top of the screen will scroll by faster than normal.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_BUY_MULTIPLE_CONSUMABLES, "Buy multiple consumables", "Allow buying multiple consumables at once.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_SHOW_EVADES_AS_ZERO, "Show evades", "Vanilla displays no damage for some evaded attacks, making it difficult to tell if you hit within a hitbox or not. This shows them as zero damage.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalEnhancements", CommonSettings.PROPERTYNAME_MINIMAP, "Show minimap", "Display minimap in the lower-right when flying on Flammie.", commonSettings);
            // misc mechanical
            propertyManager.makeBooleanValueProperty("generalMiscMechanical", CommonSettings.PROPERTYNAME_STATUS_LENGTHS, "Status length increase", "Increase the length of time that buffs and debuffs stay active.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalMiscMechanical", CommonSettings.PROPERTYNAME_MAGIC_REBALANCE, "Magic rebalance", "Modify the power and MP cost of some spells to account for getting them earlier in the game.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalMiscMechanical", CommonSettings.PROPERTYNAME_DAMAGE_PERCENT_FIX, "Accurate damage percentages", "The vanilla game divides non-100% weapon damage by two. This removes that for more accurate damage numbers.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalMiscMechanical", CommonSettings.PROPERTYNAME_MANAMAGIC_FIX, "Mana Magic change", "Instead of using weapon levels in a strange way to calculate Mana Magic's damage bonus, use a decently high fixed value.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalMiscMechanical", CommonSettings.PROPERTYNAME_PERCENTAGE_POISON, "Percentage Damage Poison", "Instead of ticking down 1 HP, poison ticks 1/32 of your max HP (rounded down)", commonSettings);
            propertyManager.makeBooleanValueProperty("generalMiscMechanical", CommonSettings.PROPERTYNAME_DEFENSE_REFACTOR, "Defense Refactor", "(BETA) Damage dealt is not purely subtractive by defense anymore, but has a multiplicative component as well.  This is intended to make armor less overpowered.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalMiscMechanical", CommonSettings.PROPERTYNAME_STARTER_GEAR, "Starter gear", "Start with the weakest piece of all 3 armor types, instead of just the chest piece.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalMiscMechanical", CommonSettings.PROPERTYNAME_BOSSES_AT_Z_0, "Bosses at Fixed Z Position", "Randomized bosses will always be at Z coordinate = 0. This is how boss rando used to work, which broke a few bosses' movements and attacks, and is provided now only as a temporary workaround in case the updated boss positioning is even more busted.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalMiscMechanical", CommonSettings.PROPERTYNAME_CUP_AT_ZERO_HP, "Cup of Wishes at 0 HP", "Don't have to wait for a character at 0 HP to fully die before being able to use a cup of wishes (or revive spell) on them.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalMiscMechanical", CommonSettings.PROPERTYNAME_CANDY_HEALOUTS, "Candy heal-outs", "In vanilla, only Royal Jam (and Cure Water) can heal a character who is at 0 HP but hasn't \"died\" yet. This allows Candy and Chocolate to also heal a character at the brink of death.", commonSettings);
            // cosmetic
            propertyManager.makeBooleanValueProperty("generalCosmetic", CommonSettings.PROPERTYNAME_STATUSGLOW, "Status glow", "Glow player characters' palettes when they have certain statuses.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalCosmetic", CommonSettings.PROPERTYNAME_FOOTSTEP_SOUND, "Sound effects reduction", "Removes the footstep and player damage sounds, for less music channel loss.  Additionally, modify SPC code to not allow sound effects on channels 1-5.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalCosmetic", CommonSettings.PROPERTYNAME_RABITE_COLOR_RANDOMIZER, "Random Rabite colors", "Rabites will be random colors instead of yellow.  I guess sometimes still yellow, too.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalCosmetic", CommonSettings.PROPERTYNAME_FAST_TRANSITIONS, "Fast transitions", "Remove fading effect from screen transitions to make them a little faster and a little uglier.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalCosmetic", CommonSettings.PROPERTYNAME_MUTE_MUSIC, "Mute all music", "Provide your own soundtrack; this mutes all in-game music. Overrides any \"Randomize music\" settings.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalCosmetic", CommonSettings.PROPERTYNAME_NAME_ENTRY_CHANGES, "Name entry changes", "Extend the max length of entered names to 12, and remove an annoying delay in moving the name entry cursor around.", commonSettings);
            Dictionary<String, Form> playerColorForms = new Dictionary<string, Form>();
            playerColorForms["Specify..."] = characterDesigner;
            propertyManager.makeMultiStringValueProperty("generalCosmetic", CommonSettings.PROPERTYNAME_RANDOMIZE_CHAR_COLORS, "Character colors", "Randomize or specify the palettes of the player characters.", commonSettings, playerColorForms);
            // randomizations
            propertyManager.makeBooleanValueProperty("generalRandomizations", CommonSettings.PROPERTYNAME_BOSS_ELEMENT_RANDO, "Randomize Boss Elements", "Randomize defense element, color, and spells cast by most bosses.  This is still a work in progress!", commonSettings);
            propertyManager.makeBooleanValueProperty("generalRandomizations", CommonSettings.PROPERTYNAME_SPOILER_LOG, "Spoiler log", "Generate a spoiler log for the seed, with the same name as the normal log + _SPOILER.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalRandomizations", CommonSettings.PROPERTYNAME_RACE_MODE, "Race mode", "Produces different game/hash on the same seed that only match with other players on race mode and also turns off spoiler log.", commonSettings);
            // stupid stuff
            propertyManager.makeBooleanValueProperty("generalStupid", CommonSettings.PROPERTYNAME_OHKO, "One-hit KO", "Drops your HP to 1 and defense to zero.  Why would you want this?", commonSettings);
            propertyManager.makeBooleanValueProperty("generalStupid", CommonSettings.PROPERTYNAME_WALK_THROUGH_WALLS, "Walk through walls", "Well, go on.  See what's in that unreachable door in North Town.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalStupid", CommonSettings.PROPERTYNAME_PERMANENT_POISON, "Poisoned players", "All your players are poisoned all the time!  Also poison can kill you now", commonSettings);
            propertyManager.makeBooleanValueProperty("generalStupid", CommonSettings.PROPERTYNAME_ENEMY_INFINITE_MP, "Infinite enemy MP", "All enemies will have an endless supply of MP.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalStupid", CommonSettings.PROPERTYNAME_INCLUDE_TRASH_WEAPONS, "Trash weapons", "Weapon rando will rarely roll joke weapons with low accuracy.", commonSettings);
            propertyManager.makeBooleanValueProperty("generalStupid", CommonSettings.PROPERTYNAME_DEBUG_LOG, "Debug logging", "Enable a debug log with WAY more detail. Only really useful to me.", commonSettings);

            t.SetToolTip(txtInputROM, "Please only use \"Secret of Mana (U).smc\".\nCountry: US\nVersion: 1.0\nSize: 16 Megabit (2.0 MB)");
            t.SetToolTip(txtOutputROM, "This is where the generated ROM will be written.");
            t.SetToolTip(txtSeed, "Any string is acceptable as a seed.  This is used to determine the\n" +
                "construction of the maps, gear that can be dropped, and other static RNG.");
            t.SetToolTip(txtOptions, "Paste options to quickly select randomizer settings, or copy to share them.\nTyping in here is allowed, but not recommended.");

            string buildDateIndicator = "Build date: ";
            try
            {
                string assemblyVersion = "" + typeof(MainForm).Assembly.GetName().Version;
                string[] versionTokens = assemblyVersion.Split(new char[] { '.' });
                string days = versionTokens[2];
                string minutes = versionTokens[3];
                DateTime date = new DateTime(2000, 1, 1)     // baseline is 01/01/2000
                .AddDays(Int32.Parse(days))             // build is number of days after baseline
                .AddSeconds(Int32.Parse(minutes) * 2);    // revision is half the number of seconds into the day
                buildDateIndicator += "" + date;
            }
            catch (Exception e)
            {
                // shrug i guess
                buildDateIndicator += "???";
            }
            t.SetToolTip(picCatBread,
                "Version " + RomGenerator.VERSION_NUMBER +
                "\n" + buildDateIndicator +
                "\nBy Mop!" +
                "\numokumok@gmail.com" +
                "\ntwitch.tv/moppleton" +
                "\n\nSpecial thanks for research and hacks:" +
                "\nEikigou" +
                "\nEmberling" +
                "\nKethinov" +
                "\nMathOnNapkins" +
                "\nQm" +
                "\nRegrs" +
                "\nThanatos-Zero" +
                "\nZhade" +
                "\n\nand for testing/streaming/ideas:" +
                "\nBOWIEtheHERO, Captain_Duck, Crow, eLmaGus," +
                "\nFalexxx, Farolicious, Finamenon, Holysmith," +
                "\niwatchgamessometimes, Lufia, NYRambler, Shadou, Siberianbull," +
                "\nSolarcell007, StingerPA, Vandaeron, Vitasia, Xusilak, xxxRufxxx," +
                "\nYagamoth, Zheal," +
                "\n\nand the rest of the very supportive Twitch SoM community.");

            t.SetToolTip(picDiscord, "Join the Secret of Mana Randomizer Discord server.");
            picDiscord.Click += picDiscord_Click;
            t.SetToolTip(picTwitch, "Visit my Twitch channel for occasional development streams\nand other unrelated stuff.");
            picTwitch.Click += picTwitch_Click;
            t.SetToolTip(picBlogspot, "Blogspot with all Secret of Mana Randomizer releases and basic info.");
            picBlogspot.Click += picBlogspot_Click;
            t.SetToolTip(picMail, "Email me: umokumok@gmail.com");
            picMail.Click += picMail_Click;

            txtOptions.Text = optionsManager.getOptionsString();
            clearOptionsError();
            txtOptions.TextChanged += txtOptions_TextChanged;
        }

        private void initVanillaRandoComponents()
        {
            // rando
            initCheckbox(chkVanillaRandomizeBosses, VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_BOSSES, vanillaRandoSettings);
            t.SetToolTip(chkVanillaRandomizeBosses, "Randomize bosses.  Stats from existing boss are\napplied to whoever replaces him.  Mode-7\nbosses (Slimes, Manabeast) are not randomized.");
            initCheckbox(chkVanillaRandomizeEnemies, VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_ENEMIES, vanillaRandoSettings);
            t.SetToolTip(chkVanillaRandomizeEnemies, "Randomize enemies.  Stats are maintained.\nUnhittable enemies are randomized only\namongst themselves.");
            initCheckbox(chkVanillaRandomizeWeaponOrbs, VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_WEAPON_ORBS, vanillaRandoSettings);
            t.SetToolTip(chkVanillaRandomizeWeaponOrbs, "Randomize weapon orbs.  You should still\ncap out at 9 for each weapon\nexcept Sword.");
            initCheckbox(chkVanillaRandomizeElements, VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_ELEMENTS, vanillaRandoSettings);
            t.SetToolTip(chkVanillaRandomizeElements, "Randomize elemental summons.  Crystal orbs\nwill be modified to require spells\nthat got randomized in.");
            initCheckbox(chkVanillaRandomizeWeapons, VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_WEAPONS, vanillaRandoSettings);
            t.SetToolTip(chkVanillaRandomizeWeapons, "Randomize names and properties of weapons.\nMana sword will be preserved.");
            initCheckbox(chkVanillaRandomizeMusic, VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_MUSIC, vanillaRandoSettings);
            t.SetToolTip(chkVanillaRandomizeMusic, "Mix up songs a bit and bring in some songs from other games.");
            initCheckbox(chkVanillaAutoSave, VanillaRandoSettings.PROPERTYNAME_AUTOSAVE, vanillaRandoSettings);
            t.SetToolTip(chkVanillaAutoSave, "Replace save slot 4 with an automatic save every time you change maps.");
            initCheckbox(chkVanillaDialogCuts, VanillaRandoSettings.PROPERTYNAME_DIALOGUE_CUTS, vanillaRandoSettings);
            t.SetToolTip(chkVanillaDialogCuts, "Removes long story events and shortens\nall required dialogue significantly for\na faster run.");
            initCheckbox(chkVanillaPreserveEarlyBosses, VanillaRandoSettings.PROPERTYNAME_PRESERVE_EARLY_BOSSES, vanillaRandoSettings);
            t.SetToolTip(chkVanillaPreserveEarlyBosses, "Check to keep all bosses before getting magic in their normal place,\nto prevent unusually hard encounters.\nIgnored if boss rando disabled.");
            initTrackbar(trbVanillaEnemyScaling, VanillaRandoSettings.PROPERTYNAME_ENEMY_SCALING, vanillaRandoSettings); // rando enemy difficulty
            t.SetToolTip(trbVanillaEnemyScaling, "Boost or reduce enemy and boss stats.\nIncludes basic stats and defense/evade stats.\nAffects HP, but less than other stats.\nDoes not affect MP.");
            initCombobox(cmbVanillaExpMultiplier, VanillaRandoSettings.PROPERTYNAME_EXP_MULTIPLIER, vanillaRandoSettings); // exp in rando
            t.SetToolTip(cmbVanillaExpMultiplier, "Experience adjustments for basic difficulty changes.");
            initCombobox(cmbVanillaGoldMultiplier, VanillaRandoSettings.PROPERTYNAME_GOLD_MULTIPLIER, vanillaRandoSettings); // gold in rando
            t.SetToolTip(cmbVanillaGoldMultiplier, "You can adjust gold drops too.");
            initCombobox(cmbVanillaSpecialMode, VanillaRandoSettings.PROPERTYNAME_SPECIAL_MODE, vanillaRandoSettings); // holiday mode in rando
            t.SetToolTip(cmbVanillaSpecialMode, "Silly holiday modes and stuff");
            initCombobox(cmbVanillaStatusAilments, VanillaRandoSettings.PROPERTYNAME_STATUS_AILMENTS, vanillaRandoSettings);
            t.SetToolTip(cmbVanillaStatusAilments, "Status ailments caused by enemies are determined by:\n\n" +
                "- Location: an enemy will cause the same conditions as the enemy that was originally there.\n" +
                "- Enemy type: every enemy retains its status conditions regardless of location.\n" +
                "- Random easy: randomize status effects with about the same distribution as vanilla.\n" +
                "- Random annoying: randomize with about double the status ailments as vanilla.\n" +
                "- Random awful: every enemy causes status ailments");
            t.SetToolTip(picVanillaDetails, "More details about this mode!"); // rando
        }

        private void initOpenWorldComponents()
        {
            // open world stuff
            openWorldOptionsCategory = new OptionsCategoryForm(propertyManager, "Open world options", "Open world options - categories");
            // create options subforms
            propertyManager.makePropertyClass("openDifficulty", "Open world - difficulty");
            propertyManager.makePropertyClass("openMtr", "Open world - MTR");
            propertyManager.makePropertyClass("openMisc", "Open world - misc");
            propertyManager.makePropertyClass("openCharacters", "Open world - characters");
            propertyManager.makePropertyClass("openNumeric", "Open world - numeric options");
            // create categories in main open world option form
            openWorldOptionsCategory.addCategory("Difficulty", "Open world difficulty options", "openDifficulty");
            openWorldOptionsCategory.addCategory("Characters", "Open world character options", "openCharacters");
            openWorldOptionsCategory.addCategory("MTR", "Open world Mana Tree Revival mode options", "openMtr");
            openWorldOptionsCategory.addCategory("Misc", "Misc. other open world options", "openMisc");
            openWorldOptionsCategory.addCategory("Numeric", "Open world numeric options", "openNumeric");

            propertyManager.makeMultiStringValueProperty("openDifficulty", OpenWorldSettings.PROPERTYNAME_COMPLEXITY, "Complexity", "Use \"Easy\" to try to put useful items in easy places, or \"Hard\" to try to put them in hard to reach places.  No guarantees.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openDifficulty", OpenWorldSettings.PROPERTYNAME_MANA_BEAST_SCALING, "Mana Beast Stats", "Choose \"Vanilla\" to have vanilla (strong!) mana beast stats, or \"Scaled\" to have him follow the stat pattern of other bosses, except a little stronger.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openDifficulty", OpenWorldSettings.PROPERTYNAME_CHEST_TRAPS, "Chest traps", "Change frequency of chest traps.  Normal: same as vanilla, based on enemy type/location.  None: no traps.  Many: well, many traps.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openDifficulty", OpenWorldSettings.PROPERTYNAME_CHEST_FREQUENCY, "Chest drop frequency", "Change the frequency of chest drops in open world.  They are slightly boosted to begin with.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openDifficulty", OpenWorldSettings.PROPERTYNAME_XMAS_DROPS, "Random drops", "Use the Christmas-style item drop method even if not doing the Christmas goals.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openDifficulty", OpenWorldSettings.PROPERTYNAME_MIN_ENEMY_LEVELS, "Minimum enemy level", "Enemies will be at least an area-appropriate vanilla level; ie, Pure Lands will have enemies that are at least level 45.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openDifficulty", OpenWorldSettings.PROPERTYNAME_MAX_ENEMY_LEVELS, "Maximum enemy level", "Enemies will be at most an area-appropriate vanilla level; ie, Gaia's Navel enemies will never be higher than level 10.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openDifficulty", OpenWorldSettings.PROPERTYNAME_NO_HINTS, "Disable hints", "Useful hints will not be shown.  All hint locations will have jokes and other crap.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openDifficulty", OpenWorldSettings.PROPERTYNAME_OBSCURE_MAP_DATA, "Black out map data", "It's really, really dark outside.  And inside.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openMtr", OpenWorldSettings.PROPERTYNAME_MANA_SEEDS_REQUIRED, "Mana Tree Revival seeds required", "The number of mana seeds you need to bring to the Mana Tree to complete the Mana Tree Revival goal.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openMtr", OpenWorldSettings.PROPERTYNAME_MANA_SEEDS_REQUIRED_MIN, "Minimum Mana Tree Revival seeds required", "If you picked \"Random\" number of seeds required, this determines the minimum it can pick.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openMtr", OpenWorldSettings.PROPERTYNAME_MANA_SEEDS_REQUIRED_MAX, "Maximum Mana Tree Revival seeds required", "If you picked \"Random\" number of seeds required, this determines the maximum it can pick.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_SHOW_ENEMY_LEVEL, "Show enemy level", "Show the targetted enemy's level as part of their name when casting a spell or using the targetting menu item.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_AUTOSAVE, "Autosave", "Automatically save to the 4th save slot on door transitions.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_REFACTOR_MAGIC_ROPE, "Magic Rope Refactor", "Change how magic rope works to allow it to work in more places.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_EVERY_ENEMY, "But why owls?", "Okay, why don't you tell me which enemy you DO want to see everywhere, then? (Only does anything if enemy mode is \"Oops all owls\")", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_NO_FUTURE_LEVEL, "\"No Future\" level", "Maybe you'd like to have a little future, as a treat.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_FORCE_START_WEAPON, "Force starting weapon", "If you want to start with a certain weapon type, pick it here.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_NO_WALKING_CHESTS, "Chests don't run away", "Enable this to make treasure chests never run away.  Man, how annoying was that?", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_AI_RANDO, "Enemy AI mode", "Choose something other than \"Vanilla\" to randomize non-boss enemy AI.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_PAUSE_TIMER_IN_MENU, "Pause timer in menu", "For timed enemy growth, pause the timer if you're in a menu, in dialogue, or flying on Flammie.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_XMAS_DECO, "Christmas theme", "Use the Christmas-colored maps and world map even if not doing the Christmas goals.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_RANDOMIZE_GRANDPALACE_ELEMENTS, "Randomize Grand Palace elements", "Randomize elements needed to open the orb section of the purple continent.  May be overlapping elements, so it will likely be easier to get into than with vanilla settings.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_ALLOW_MISSED_ITEMS, "Allow locked items", "Some items not necessary to reach the goal may be unreachable by open world logic.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_START_WITH_ALL_WEAPON_ORBS, "Start with all weapon orbs", "Every weapon has the potential to grow to level 9 immediately.  Weapon orb checks are replaced by gold.", openWorldSettings);
            propertyManager.makeBooleanValueProperty("openMisc", OpenWorldSettings.PROPERTYNAME_FAST_WHIP_POSTS, "Fast whip posts", "Trigger whip jumps just by stepping on the tile and having the whip.  Don't have to equip or swing it.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openCharacters", OpenWorldSettings.PROPERTYNAME_BOY_CLASS, "Boy character role", "Change or randomize the boy character's role in the run. Boy character will inherit spells and stats of selected vanilla character.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openCharacters", OpenWorldSettings.PROPERTYNAME_GIRL_CLASS, "Girl character role", "Change or randomize the girl character's role in the run. Girl character will inherit spells and stats of selected vanilla character.", openWorldSettings);
            propertyManager.makeMultiStringValueProperty("openCharacters", OpenWorldSettings.PROPERTYNAME_SPRITE_CLASS, "Sprite character role", "Change or randomize the sprite character's role in the run. Sprite character will inherit spells and stats of selected vanilla character.", openWorldSettings);
            propertyManager.makeNumericValueProperty("openNumeric", OpenWorldSettings.PROPERTYNAME_OPEN_WORLD_EXP_MULTIPLIER, "Experience multiplier", "Adjust multiplier to experience gained from killing enemies.", openWorldSettings);
            propertyManager.makeNumericValueProperty("openNumeric", OpenWorldSettings.PROPERTYNAME_OPEN_WORLD_GOLD_MULTIPLIER, "Gold multiplier", "Adjust multiplier to gold gained from killing enemies.", openWorldSettings);
            propertyManager.makeNumericValueProperty("openNumeric", OpenWorldSettings.PROPERTYNAME_NUMERIC_GOLD_CHECK_MULTIPLIER, "Gold check multiplier", "At 1.0, gold checks in open world have 100-1000 gold.", openWorldSettings);
            propertyManager.makeNumericValueProperty("openNumeric", OpenWorldSettings.PROPERTYNAME_NUMERIC_GOLD_DROP_MULTIPLIER, "Gold drop multiplier", "Adjust multiplier to enemy level to determine how much gold enemies drop in chests.", openWorldSettings);
            propertyManager.makeNumericValueProperty("openNumeric", OpenWorldSettings.PROPERTYNAME_NUMERIC_START_GOLD, "Starting gold", "Start with this much gold.", openWorldSettings);
            propertyManager.makeNumericValueProperty("openNumeric", OpenWorldSettings.PROPERTYNAME_STARTING_LEVEL, "Starting level", "Minimum starting level of characters joining the party. Note that if enemy leveling is set to \"Match player,\" this could make the beginning of the game more difficult.", openWorldSettings);
            propertyManager.makeNumericValueProperty("openNumeric", OpenWorldSettings.PROPERTYNAME_NUM_XMAS_GIFTS, "Xmas gifts", "For the Christmas gift goal, the number of gifts you have to deliver.", openWorldSettings);

            initCombobox(cmbOpenStatGrowth, OpenWorldSettings.PROPERTYNAME_ENEMY_STAT_GROWTH, openWorldSettings);
            t.SetToolTip(cmbOpenStatGrowth,
                "Decide how enemy levels and stats will be determined.\n\n" +
                "- Match player: Enemies will match the level of the highest\nplayer, and stats will scale up or down according to\nvanilla stat distribution of that enemy.\n\n" +
                "- Increase after bosses: Enemy levels and stats will increase\nafter every boss defeated.\nDifficulty level will control the increase after each boss.\n\n" +
                "- Timed: Enemy levels and stats will increase on a timer.\nDifficulty level will control how fast the timer runs.\n\n" +
                "- No Future: All enemies are level 99 (or chosen level in More Options). Good luck.\n\n" +
                "- None (vanilla): Enemy levels/stats based on location\nlike Rando mode. May be grindy!\n\n"
                );
            initCombobox(cmbOpenStatGrowthDifficulty, OpenWorldSettings.PROPERTYNAME_ENEMY_STAT_GROWTH_DIFFICULTY, openWorldSettings);
            t.SetToolTip(cmbOpenStatGrowthDifficulty, "Choose how aggressively the stats of enemies will increase.\nIf you can complete \"impossible\" then I didn't make it hard enough.");
            initCombobox(cmbOpenOtherChars, OpenWorldSettings.PROPERTYNAME_START_WITH_GIRL_AND_SPRITE, openWorldSettings);
            t.SetToolTip(cmbOpenOtherChars, "How will you find the non-starting characters?");
            initCombobox(cmbOpenStatusAilments, OpenWorldSettings.PROPERTYNAME_STATUS_AILMENTS, openWorldSettings);
            t.SetToolTip(cmbOpenStatusAilments, "Status ailments caused by enemies are determined by:\n\n" +
                "- Location: an enemy will cause the same conditions as the enemy that was originally there.\n" +
                "- Enemy type: every enemy retains its status conditions regardless of location.\n" +
                "- Random easy: randomize status effects with about the same distribution as vanilla.\n" +
                "- Random annoying: randomize with about double the status ailments as vanilla.\n" +
                "- Random awful: every enemy causes status ailments");
            initCombobox(cmbOpenGoal, OpenWorldSettings.PROPERTYNAME_GOAL, openWorldSettings);
            t.SetToolTip(cmbOpenGoal, "Set the goal of the run:\n\n" +
                "- Vanilla short: Beat the Mana Beast at the Mana Fort.  Mana Fort is accessible from the start.\n" +
                "- Vanilla long: Beat the Mana Beast at the Mana Fort.  Mana Fort is accessible once you finish Grand Palace.\n" +
                "- Mana tree revival: Bring the eight Mana Seeds to the Mana Tree.\n" +
                "- Gift exchange: (Xmas 2020) Deliver random gifts to random NPCs.\n" +
                "- Reindeer search: (Xmas 2020) Find Santa's eight reindeer, which replace random enemies/NPCs.\n"
                );
            initCombobox(cmbOpenRandomizeEnemies, OpenWorldSettings.PROPERTYNAME_RANDOMIZE_ENEMIES, openWorldSettings);
            t.SetToolTip(cmbOpenRandomizeEnemies, "Determines how normal enemies are placed.\nNote that their stats are determined by \"Enemy stat growth\" based on their location:\n\n" +
                "- Vanilla: all enemies are in their original places.\n" +
                "- Swap: every enemy species is randomly swapped with another.\n" +
                "- Random spawns: every enemy spawn will be a random species every time.\n" +
                "- Oops all owls: all enemies are the same type as determined by More Options. Default Nemesis Owl.\n" +
                "- None: with a few exceptions, there are no normal enemies.");
            initCombobox(cmbOpenRandomizeBosses, OpenWorldSettings.PROPERTYNAME_RANDOMIZE_BOSSES, openWorldSettings);
            initCombobox(cmbOpenLogicMode, OpenWorldSettings.PROPERTYNAME_LOGIC_MODE, openWorldSettings);
            t.SetToolTip(cmbOpenLogicMode, "Configure logic for the run:\n\n" +
                "- Basic: Logic includes randomized spell orbs, whip/axe/sword for some locations, and gold key/sea hare's tail.\n" +
                "- Restrictive: Additionally, Mana Palaces will only grant rewards once their Mana Seed is restored.\n"
                );
            initCombobox(cmbOpenStartingChar, OpenWorldSettings.PROPERTYNAME_STARTING_CHAR, openWorldSettings);
            initCheckbox(chkOpenRandomizeMusic, OpenWorldSettings.PROPERTYNAME_RANDOMIZE_MUSIC, openWorldSettings);
            t.SetToolTip(chkOpenRandomizeMusic, "Mix up songs a bit and bring in some songs from other games.");
            initCheckbox(chkOpenRandomizeWeapons, OpenWorldSettings.PROPERTYNAME_RANDOMIZE_WEAPONS, openWorldSettings);
            t.SetToolTip(chkOpenRandomizeWeapons, "Randomize names and properties of weapons.\nMana sword will be preserved.");
            initCheckbox(chkOpenRandomizeShops, OpenWorldSettings.PROPERTYNAME_RANDOMIZE_SHOPS, openWorldSettings); // random shops in open world
            t.SetToolTip(chkOpenRandomizeShops, "Randomize gear available in shops.");
            initCheckbox(chkOpenRandomMapColors, OpenWorldSettings.PROPERTYNAME_RANDOMIZE_COLORS, openWorldSettings); // random colors in open world
            t.SetToolTip(chkOpenRandomMapColors, "Randomize the hue/saturation/luminance of map colors.");
            initCheckbox(chkOpenFlammieDrumLogic, OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC, openWorldSettings); // flammie drum found
            t.SetToolTip(chkOpenFlammieDrumLogic, "Don't start with Flammie Drum - find it as a check instead.\nRestricts your options at the beginning of the run.\nChanges start location to Potos so you don't need a cutting weapon.");

            t.SetToolTip(picOpenDetails, "More details about this mode!"); // open

            plandoForm = new PlandoForm();
            optionsManager.addPlando(plandoForm, OpenWorldSettings.PROPERTYNAME_PLANDO, openWorldSettings);
        }

        private void initAncientCaveComponents()
        {
            initCheckbox(chkACProfanityFilter, AncientCaveSettings.PROPERTYNAME_PROFANITY_FILTER, ancientCaveSettings); // filter
            t.SetToolTip(chkACProfanityFilter, "Mitch Hedberg isn't for everybody.");
            initCheckbox(chkACIncludeBoyChar, AncientCaveSettings.PROPERTYNAME_INCLUDE_BOY_CHARACTER, ancientCaveSettings); // boy, girl, sprite for ancient cave
            initCheckbox(chkACIncludeGirlChar, AncientCaveSettings.PROPERTYNAME_INCLUDE_GIRL_CHARACTER, ancientCaveSettings);
            initCheckbox(chkACIncludeSpriteChar, AncientCaveSettings.PROPERTYNAME_INCLUDE_SPRITE_CHARACTER, ancientCaveSettings);
            initCheckedListbox(cklACBiomeTypes, AncientCaveSettings.PROPERTYNAME_BIOME_TYPES, ancientCaveSettings);
            initCombobox(cmbACDifficulty, AncientCaveSettings.PROPERTYNAME_DIFFICULTY, ancientCaveSettings); // AC difficulty
            t.SetToolTip(cmbACDifficulty, "Enemy stat progression level.");
            initCombobox(cmbACDialogSource, AncientCaveSettings.PROPERTYNAME_DIALOGUE_SOURCE, ancientCaveSettings); // AC npc dialogue
            initCombobox(cmbACLength, AncientCaveSettings.PROPERTYNAME_LENGTH, ancientCaveSettings); // AC length
            t.SetToolTip(cmbACLength, "The floors are much longer and more time-consuming than Lufia 2's.");
            initCombobox(cmbACBossFrequency, AncientCaveSettings.PROPERTYNAME_BOSS_FREQUENCY, ancientCaveSettings); // AC bosses
            t.SetToolTip(cmbACBossFrequency, "All ancient caves end with the Mana Beast fight.");
            initCheckbox(chkACRandomMusic, AncientCaveSettings.PROPERTYNAME_RANDOM_MUSIC, ancientCaveSettings); // ac random music
            t.SetToolTip(picACDetails, "More details about this mode!"); // ac
        }

        private void initBossRushComponents()
        {
            propertyManager.makePropertyClass("bossrush", "Boss rush options");
            propertyManager.makeBooleanValueProperty("bossrush", BossRushSettings.PROPERTYNAME_LIMIT_MP_STEAL, "Limit MP Absorb", "MP Absorb will grant the sprite 3 MP max, but still damage the enemy's MP by the original amount.", bossRushSettings);
            initCheckbox(chkBossRushIncludeBoyChar, BossRushSettings.PROPERTYNAME_INCLUDE_BOY_CHARACTER, bossRushSettings); // boy, girl, sprite for boss rush mode
            initCheckbox(chkBossRushIncludeGirlChar, BossRushSettings.PROPERTYNAME_INCLUDE_GIRL_CHARACTER, bossRushSettings);
            initCheckbox(chkBossRushIncludeSpriteChar, BossRushSettings.PROPERTYNAME_INCLUDE_SPRITE_CHARACTER, bossRushSettings);
            initCombobox(cmbBossRushDifficulty, BossRushSettings.PROPERTYNAME_DIFFICULTY, bossRushSettings); // boss rush difficulty
            t.SetToolTip(picBossRushDetails, "More details about this mode!"); // bossrush
        }

        private void initChaosComponents()
        {
            propertyManager.makePropertyClass("chaos", "Chaos mode options");

            propertyManager.makeBooleanValueProperty("chaos", ChaosSettings.PROPERTYNAME_PRIORITIZE_HEAL_SPELLS, "Prioritize heal spells", "Chances of getting Undine toward the beginning of the run are heavily in your favor, and those of Dryad are slightly in your favor.", chaosSettings);
            propertyManager.makeBooleanValueProperty("chaos", ChaosSettings.PROPERTYNAME_SAFER_EARLY_FLOORS, "Safer early floors", "Extend the number of normal floors early in the run before you encounter the first boss.", chaosSettings);

            initCheckbox(chkChaosIncludeBoyChar, ChaosSettings.PROPERTYNAME_INCLUDE_BOY_CHARACTER, chaosSettings); // boy, girl, sprite for chaos mode
            initCheckbox(chkChaosIncludeGirlChar, ChaosSettings.PROPERTYNAME_INCLUDE_GIRL_CHARACTER, chaosSettings);
            initCheckbox(chkChaosIncludeSpriteChar, ChaosSettings.PROPERTYNAME_INCLUDE_SPRITE_CHARACTER, chaosSettings);
            initCombobox(cmbChaosDifficulty, ChaosSettings.PROPERTYNAME_DIFFICULTY, chaosSettings); // chaos difficulty
            t.SetToolTip(cmbChaosDifficulty, "Enemy stat progression level.");
            initTrackbar(trbChaosFloors, ChaosSettings.PROPERTYNAME_NUM_FLOORS, chaosSettings); // chaos floors
            t.SetToolTip(trbChaosFloors, "Shortest around 20 maps, longest around 100.\nNote that enemies are scaled regardless of length,\nso shorter ROMs will be harder!");
            initTrackbar(trbChaosBosses, ChaosSettings.PROPERTYNAME_NUM_BOSSES, chaosSettings); // chaos bosses
            t.SetToolTip(trbChaosBosses, "Far left for no bosses (except the mana beast), far right for a bunch of bosses.");
            initRadioButtons(new RadioButton[] { rdoChaosPalatteNone, rdoChaosPalatteReasonable, rdoChaosPalatteRidiculous }, ChaosSettings.PROPERTYNAME_PALETTE_SWAP_TYPE, chaosSettings); // chaos colors
            t.SetToolTip(rdoChaosPalatteNone, "Don't screw with the palettes.");
            t.SetToolTip(rdoChaosPalatteReasonable, "Some semi-normal looking BG palette swaps.");
            t.SetToolTip(rdoChaosPalatteRidiculous, "Just don't pick this.");

            t.SetToolTip(picChaosDetails, "More details about this mode!"); // chaos
        }
    }
}
