using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.fix;
using SoMRandomizer.processing.hacks.common.music;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.processing.hacks.common.qol;
using SoMRandomizer.processing.hacks.openworld;
using SoMRandomizer.processing.openworld.events;
using SoMRandomizer.processing.openworld.randomization;
using System;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// Top-level class used to generate open world ROMs.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldGenerator : RomGenerator
    {
        public OpenWorldGenerator()
        {
            addModeSpecificHack(new OpenWorldCharacterSelection());
            // process goal settings
            addModeSpecificHack(new OpenWorldGoalProcessor());
            // process character class selection
            addModeSpecificHack(new OpenWorldClassSelection());
            // randomize spell orb elements and publish them to the context
            addModeSpecificHack(new ElementSwaps());
            // randomize bosses
            addModeSpecificHack(new BossSwaps());
            // should be after BossSwaps since that sets the name of mech rider to mech rider 1/2/3
            addModeSpecificHack(new OpenWorldSillyEnemyNamePicker());
            // determine number of seeds for MTR and publish to the context
            addModeSpecificHack(new OpenWorldMtrSeedNumSelection());
            // swap/randomize enemies
            addModeSpecificHack(new EnemySwaps());
            // swap weapon orbs
            addModeSpecificHack(new OrbSwaps());
            // modify music to 0 volume - this allows no-music but with vanilla loadtimes
            addModeSpecificHack(new CustomMusic());
            // some minor world map fixes
            addModeSpecificHack(new IceCountryLandableFixes());
            // christmas mode stuff
            addModeSpecificHack(new OpenWorldXmasHacks());
            // fix for slow status softlock
            addModeSpecificHack(new InvisClearsSlow());
            // fixes for z positioning of bosses
            addModeSpecificHack(new BossZPosition());
            // processing of values for difficulty
            addModeSpecificHack(new OpenWorldDifficultyProcessor());
            // set crystal orb color and names to match their element
            addModeSpecificHack(new CrystalOrbColors());
            // set up events for open world randomization
            addModeSpecificHack(new OpenWorldEvents());
            // supporting hack for displaying enemy level
            addModeSpecificHack(new CustomTopOfScreenText());
            // min/max level options for open world
            addModeSpecificHack(new MinMaxLevel());
            // enemy scaling for open world
            addModeSpecificHack(new EnemiesAtYourLevel());
            // do the main randomizations for open world
            addModeSpecificHack(new OpenWorldRandomizer());
            // fix issue where you get stuck on the light fixture in the minotaur room sometimes
            addModeSpecificHack(new FirePalaceDoorFix());
            // automatically save on door transitions
            addModeSpecificHack(new Autosave());
            // randomize gear in shops
            addModeSpecificHack(new ShopRandomizer());
            // fix for watts issue
            addModeSpecificHack(new WattsWeaponLevelFix());
            // delicious slimes
            addModeSpecificHack(new LimeSlimeFlavors());
            // drop changes for open world
            addModeSpecificHack(new DropOnlyConsumables());
            // change shops to only show gear as usable if it's better than what you've got now
            addModeSpecificHack(new MenuIconsIfArmorImprovement());
            // magic rope goes to a fixed location for every map, instead of saving the door and going back there
            addModeSpecificHack(new MagicRopeRefactor());
            // handle characters joining in open world
            addModeSpecificHack(new CharacterJoinChanges());
            // randomize map palettes
            addModeSpecificHack(new MapPaletteRandomizations());
            // AI rando; this doesn't work particularly well yet
            addModeSpecificHack(new AISwap());
            // dumb option to black-out maps
            addModeSpecificHack(new BlackMapPalettes());
            // nesoberi plush with hints
            addModeSpecificHack(new Nesoberi());
            // hack so you just have to step on whip posts, and have the whip, to get by them
            addModeSpecificHack(new FastWhipPosts());
            // make that rabite on map #111 indestructible
            addModeSpecificHack(new InvincibleRabite());
            // disable reset after timeout on first menu screen
            addModeSpecificHack(new DontAutoResetAtFirstMenu());
            // change character "classes" - stats and spells, if specified
            addModeSpecificHack(new CharacterClasses());
            // show the seed as part of the intro menu screen
            // this needs to be the LAST thing since it's printing the state of our rng
            addModeSpecificHack(new TitleMenuSeedHashDisplay());
        }
        protected override void generate(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context)
        {
            string romMarker = "open world v" + settings.get(CommonSettings.PROPERTYNAME_VERSION);
            foreach (char c in romMarker)
            {
                outRom[context.workingOffset++] = (byte)c;
            }
            outRom[context.workingOffset++] = 0;
            // extract plando string into individual properties for other hacks
            PlandoUtils.processPlandoSetting(settings, context);
            applyHacks(origRom, outRom, seed, settings, context);
        }
    }
}
