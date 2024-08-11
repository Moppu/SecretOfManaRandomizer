using SoMRandomizer.processing.common;

namespace SoMRandomizer.config.settings
{
    /// <summary>
    /// Enumeration of settings and defaults common to all modes.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CommonSettings : StringValueSettings
    {
        public const string PROPERTYNAME_ALL_ENTERED_OPTIONS = "allOptions";
        public const string PROPERTYNAME_VERSION = "version";
        public const string PROPERTYNAME_BUILD_DATE = "buildDate";
        public const string PROPERTYNAME_DEBUG_LOG = "debugLoggingEnabled";
        public const string PROPERTYNAME_MODE = "mode";
        public const string PROPERTYNAME_CURRENT_PROGRESS = "currentGenerateProgress";
        public const string PROPERTYNAME_TEST_ONLY = "testOnly";
        public const string PROPERTYNAME_OVERCHARGE_FIX = "ovrchrgFix";
        public const string PROPERTYNAME_SCROLL_FIX = "leavePlayersBehind";
        public const string PROPERTYNAME_RANDOMIZE_CHAR_COLORS = "characterColors";
        public const string PROPERTYNAME_FOOTSTEP_SOUND = "seReduction";
        public const string PROPERTYNAME_AGGRESSIVE_BOSSES = "aggBosses";
        public const string PROPERTYNAME_SPEEDUP_CHANGE = "funSpeedup";
        public const string PROPERTYNAME_RABITE_COLOR_RANDOMIZER = "rainbowRabites";
        public const string PROPERTYNAME_STATUS_LENGTHS = "longStatus";
        public const string PROPERTYNAME_FASTER_CHESTS = "fasterChests";
        public const string PROPERTYNAME_MAGIC_REBALANCE = "magicRebalance";
        public const string PROPERTYNAME_LEVELUPS_RESTORE_MP = "mpAtLevel";
        public const string PROPERTYNAME_FASTER_MAGIC_LEVELS = "fasterMagicLevel";
        public const string PROPERTYNAME_FASTER_WEAPON_LEVELS = "fasterWeaponLevel";
        public const string PROPERTYNAME_FASTER_TOP_SCREEN_DIALOGUE = "fasterMessages";
        public const string PROPERTYNAME_MAX_7_ITEMS = "moreItems";
        public const string PROPERTYNAME_STARTER_GEAR = "starterGear";
        public const string PROPERTYNAME_HITTABLE_VAMPIRE = "noBsVampires";
        public const string PROPERTYNAME_ORB_REWARD_FIX = "orbFix";
        public const string PROPERTYNAME_BOSS_DEATH_FIX = "bossDeathFix";
        public const string PROPERTYNAME_ENEMY_SPECIES_DAMAGE = "speciesDamage";
        public const string PROPERTYNAME_WEAPON_ELEMENTAL_DAMAGE = "elemDamage";
        public const string PROPERTYNAME_FAST_TRANSITIONS = "fastTransition";
        public const string PROPERTYNAME_DAMAGE_PERCENT_FIX = "accuratePercent";
        public const string PROPERTYNAME_NO_ENERGY_TO_RUN = "noStaminaRun";
        public const string PROPERTYNAME_STATUSGLOW = "statusGlow";
        public const string PROPERTYNAME_MODE7_EDGES_FIX = "mode7Fix";
        public const string PROPERTYNAME_GIGAS_FIX = "noGigasSplit";
        public const string PROPERTYNAME_WHIP_COORDINATE_CORRUPTION_FIX = "whipFix";
        public const string PROPERTYNAME_EXTENDED_TARGETTING = "offscreenTarget";
        public const string PROPERTYNAME_BUY_MULTIPLE_CONSUMABLES = "buyMultiple";
        public const string PROPERTYNAME_MANAMAGIC_FIX = "manaMagicFix";
        public const string PROPERTYNAME_OHKO = "ohko";
        public const string PROPERTYNAME_SUMMONING_FIX = "noSummonDespawn";
        public const string PROPERTYNAME_WALK_THROUGH_WALLS = "noclip";
        public const string PROPERTYNAME_CONFUSION_FIX = "confusionFix";
        public const string PROPERTYNAME_BEAK_DISABLE = "hittableBeaks";
        public const string PROPERTYNAME_MECHRIDER_DEATH_FIX = "mechRiderDeathFix";
        public const string PROPERTYNAME_ENEMY_POSITION_FIX = "adjustEnemyPos";
        public const string PROPERTYNAME_NO_WEAPON_STAMINA_COST = "attacksForever";
        public const string PROPERTYNAME_PERMANENT_POISON = "deathByPoison";
        public const string PROPERTYNAME_PERCENTAGE_POISON = "percentPoison";
        public const string PROPERTYNAME_AGGRESSIVE_ENEMIES = "aggEnemies";
        public const string PROPERTYNAME_OBSCURE_DAMAGE = "obscureDamage";
        public const string PROPERTYNAME_OBSCURE_OWN_HP = "obscureHp";
        public const string PROPERTYNAME_OBSCURE_GOLD = "obscureGold";
        public const string PROPERTYNAME_DEFENSE_REFACTOR = "defRefactor";
        public const string PROPERTYNAME_MUTE_MUSIC = "muteMusic";
        public const string PROPERTYNAME_SHOW_EVADES_AS_ZERO = "showEvades";
        public const string PROPERTYNAME_BOSSES_AT_Z_0 = "bossFixedZ";
        public const string PROPERTYNAME_BOSS_ELEMENT_RANDO = "bossElementRando";
        public const string PROPERTYNAME_NAME_ENTRY_CHANGES = "nameEntryFix";
        public const string PROPERTYNAME_ENEMY_INFINITE_MP = "enemyInfiniteMp";
        public const string PROPERTYNAME_INCLUDE_TRASH_WEAPONS = "trashWeapons";
        public const string PROPERTYNAME_DAMAGE_CANCEL_ALLOW_TYPE = "dmgCancelAllow";
        public const string PROPERTYNAME_MINIMAP = "showMinimap";
        public const string PROPERTYNAME_CUP_AT_ZERO_HP = "cupAtZeroHp";
        public const string PROPERTYNAME_CANDY_HEALOUTS = "candyHealouts";
        public const string PROPERTYNAME_MAGIC_ROPE_DEATH_FIX = "magicRopeDeathFix";
        public const string PROPERTYNAME_SPOILER_LOG = "spoilerLog";

        // used by Form1 and CharacterPaletteRandomizer to coordinate custom colors for characters
        public const string PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS = "CustomCharColor";

        public CommonSettings()
        {
            // default common settings

            // boolean settings
            setInitial(PROPERTYNAME_DEBUG_LOG, false); // no debug logging by default
            setInitial(PROPERTYNAME_OVERCHARGE_FIX, true); // don't allow overcharge glitch to be used
            setInitial(PROPERTYNAME_SCROLL_FIX, true); // allow p2/p3 to be scrolled off screen and teleport back
            setInitial(PROPERTYNAME_FOOTSTEP_SOUND, true); // mute a few less useful sound effects so you can hear the music better
            setInitial(PROPERTYNAME_AGGRESSIVE_BOSSES, false); // boss AI runs way faster
            setInitial(PROPERTYNAME_SPEEDUP_CHANGE, true); // gnome speedup spell is just straight-up haste instead of, well, nothing
            setInitial(PROPERTYNAME_RABITE_COLOR_RANDOMIZER, true); // rabites get assigned a random r/g/b every spawn
            setInitial(PROPERTYNAME_STATUS_LENGTHS, true); // status conditions last longer than in vanilla
            setInitial(PROPERTYNAME_FASTER_CHESTS, true); // no shake animation for chests
            setInitial(PROPERTYNAME_MAGIC_REBALANCE, true); // change a few mp costs and stuff
            setInitial(PROPERTYNAME_LEVELUPS_RESTORE_MP, true); // vanilla only restores hp; this gives mp too
            setInitial(PROPERTYNAME_FASTER_MAGIC_LEVELS, true); // spells level 4x faster than vanilla
            setInitial(PROPERTYNAME_FASTER_WEAPON_LEVELS, true); // weapons level 4x faster than vanilla
            setInitial(PROPERTYNAME_FASTER_TOP_SCREEN_DIALOGUE, true); // faster movement of status messages
            setInitial(PROPERTYNAME_MAX_7_ITEMS, true); // max of a consumable 4 -> 7
            setInitial(PROPERTYNAME_STARTER_GEAR, false); // start with gear in each slot
            setInitial(PROPERTYNAME_HITTABLE_VAMPIRE, true); // vampire bosses are hittable with physicals when cloaked
            setInitial(PROPERTYNAME_ORB_REWARD_FIX, true); // fix for a softlock related to gaining weapon orbs
            setInitial(PROPERTYNAME_BOSS_DEATH_FIX, true); // fix for a softlock related to boss deaths
            setInitial(PROPERTYNAME_ENEMY_SPECIES_DAMAGE, true); // species type on weapon grants bonus damage to matching enemies
            setInitial(PROPERTYNAME_WEAPON_ELEMENTAL_DAMAGE, true); // elemental sabers and randomized elemental weapons deal more or less damage to enemies based on their def ele
            setInitial(PROPERTYNAME_FAST_TRANSITIONS, false); // remove fades on transitions in favor of speed; sometimes gives some undesirable graphical artifacts
            setInitial(PROPERTYNAME_DAMAGE_PERCENT_FIX, false); // make weapon stamina percents accurate; vanilla deals half if you aren't 100%
            setInitial(PROPERTYNAME_NO_ENERGY_TO_RUN, false); // don't consume stamina when running
            setInitial(PROPERTYNAME_STATUSGLOW, true); // glow character palettes to indicate certain statuses
            setInitial(PROPERTYNAME_MODE7_EDGES_FIX, true); // fix a minor graphical artifact with flammie flight
            setInitial(PROPERTYNAME_GIGAS_FIX, true); // gigases don't split up into sparkles to waste time, but also results in more aggresive behavior due to this
            setInitial(PROPERTYNAME_WHIP_COORDINATE_CORRUPTION_FIX, true); // fix an issue that made whip posts sometimes have unpredictable behavior
            setInitial(PROPERTYNAME_EXTENDED_TARGETTING, true); // allow offscreen enemies/allies (including element orbs, and mana beast) to be targetted for spells
            setInitial(PROPERTYNAME_BUY_MULTIPLE_CONSUMABLES, true); // modify shops to allow you to buy multiple consumables at once
            setInitial(PROPERTYNAME_MANAMAGIC_FIX, true); // mana magic gives a fixed charge level bonus of 6 instead of weird vanilla calculation
            setInitial(PROPERTYNAME_OHKO, false); // you die in one hit
            setInitial(PROPERTYNAME_SUMMONING_FIX, true); // don't allow certain npcs to despawn due to summoning enemies
            setInitial(PROPERTYNAME_WALK_THROUGH_WALLS, false); // walk through walls
            setInitial(PROPERTYNAME_CONFUSION_FIX, true); // fix issue where confusion inverted flammie controls and saving with confusion could result in permanent reversed controls
            setInitial(PROPERTYNAME_BEAK_DISABLE, false); // disable beak shield on bird bosses
            setInitial(PROPERTYNAME_MECHRIDER_DEATH_FIX, true); // mech rider doesn't speed away on death and sometimes take a layer of the map with him
            setInitial(PROPERTYNAME_ENEMY_POSITION_FIX, true); // move a few enemies to be away from doorways to prevent frustration and autosave softlocks
            setInitial(PROPERTYNAME_NO_WEAPON_STAMINA_COST, false); // weapon attacks do not cost stamina and are always 100% damage hits
            setInitial(PROPERTYNAME_PERMANENT_POISON, false); // your characters are poisoned forever
            setInitial(PROPERTYNAME_PERCENTAGE_POISON, false); // all poison deals a percent of max hp instead of 1/tick
            setInitial(PROPERTYNAME_AGGRESSIVE_ENEMIES, false); // enemies are faster and more aggressive
            setInitial(PROPERTYNAME_OBSCURE_DAMAGE, false); // show all damage numbers as zero
            setInitial(PROPERTYNAME_OBSCURE_OWN_HP, false); // hide your own health
            setInitial(PROPERTYNAME_OBSCURE_GOLD, false); // hide your gold amount
            setInitial(PROPERTYNAME_DEFENSE_REFACTOR, false); // experimental refactor to damage formula
            setInitial(PROPERTYNAME_MUTE_MUSIC, false); // mute all music but still load it for equivalent loadtimes
            setInitial(PROPERTYNAME_SHOW_EVADES_AS_ZERO, true); // show zero for some evaded attacks where vanilla showed nothing
            setInitial(PROPERTYNAME_BOSSES_AT_Z_0, false); // old method of making bosses work on all maps; only really provided for debugging
            setInitial(PROPERTYNAME_BOSS_ELEMENT_RANDO, false); // bosses get random defense element/palette/spells; doesn't work on all bosses yet
            setInitial(PROPERTYNAME_NAME_ENTRY_CHANGES, true); // allow 12 character name entries and fix the weird control delay when entering it
            setInitial(PROPERTYNAME_ENEMY_INFINITE_MP, false); // enemies have infinite mp
            setInitial(PROPERTYNAME_INCLUDE_TRASH_WEAPONS, true); // weapon rando rarely gives useless weapons
            setInitial(PROPERTYNAME_MINIMAP, true); // show minimap on sprite layer when flying on flammie
            setInitial(PROPERTYNAME_CUP_AT_ZERO_HP, true); // don't have to be fully dead to use cup wishes; just have 0 hp
            setInitial(PROPERTYNAME_CANDY_HEALOUTS, true); // healing consumables can be used on still-alive 0 hp character
            setInitial(PROPERTYNAME_MAGIC_ROPE_DEATH_FIX, true); // fix invulnerability when using magic rope
            setInitial(PROPERTYNAME_SPOILER_LOG, true); // generate spoiler log
            setInitial(PROPERTYNAME_TEST_ONLY, false); // for automated tests; changes how we log

            // enumerations
            setInitial(PROPERTYNAME_VERSION, new string[] { }, new string[] { }, RomGenerator.VERSION_NUMBER );
            setInitial(PROPERTYNAME_BUILD_DATE, new string[] { }, new string[] { }, "unknown");
            // selected game mode
            setInitial(PROPERTYNAME_MODE, new string[] {
                // keys
                VanillaRandoSettings.MODE_KEY,
                OpenWorldSettings.MODE_KEY,
                AncientCaveSettings.MODE_KEY,
                BossRushSettings.MODE_KEY,
                ChaosSettings.MODE_KEY }, 
                // display strings
                new string[] {
                VanillaRandoSettings.MODE_NAME,
                OpenWorldSettings.MODE_NAME,
                AncientCaveSettings.MODE_NAME,
                BossRushSettings.MODE_NAME,
                ChaosSettings.MODE_NAME }, 
                // default
                OpenWorldSettings.MODE_KEY);
            // for custom, the UI pushes a set of custom keys in for each color that are read by the palette rando hack, prefixed by PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS
            setInitial(PROPERTYNAME_RANDOMIZE_CHAR_COLORS, new string[] { "none", "rando", "custom" }, new string[] { "No change", "Randomize", "Specify..." }, "rando");
            setInitial(PROPERTYNAME_DAMAGE_CANCEL_ALLOW_TYPE, new string[] { "all", "consumables", "none" }, new string[] { "All", "Consumables", "None" }, "all");

            // progress of longer running generation modes like ancient cave
            setInitial(PROPERTYNAME_CURRENT_PROGRESS, 0);
        }
    }
}
