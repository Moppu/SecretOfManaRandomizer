using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.enhancement;
using SoMRandomizer.processing.hacks.common.fix;
using SoMRandomizer.processing.hacks.common.music;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.processing.hacks.common.qol;
using SoMRandomizer.processing.hacks.vanillarando;
using System;

namespace SoMRandomizer.processing.vanillarando
{
    /// <summary>
    /// Top-level class used to generate vanilla rando ROMs.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaRandoGenerator : RomGenerator
    {
        public VanillaRandoGenerator()
        {
            // randomize spell orb elements and publish them to the context
            addModeSpecificHack(new ElementSwaps());
            // speed up vanilla events
            addModeSpecificHack(new FasterDialogueEvents());
            // swap bosses if selected
            addModeSpecificHack(new BossSwaps());
            // swap enemies if selected
            addModeSpecificHack(new EnemySwaps());
            // bug fix for getting caught in some transitions while slowed
            addModeSpecificHack(new InvisClearsSlow());
            // make bosses hittable on all maps
            addModeSpecificHack(new BossZPosition());
            // apply halloween mode if selected
            addModeSpecificHack(new FallTheme());
            // apply christmas mode if selected
            addModeSpecificHack(new WinterTheme());
            // exp multiplier
            addModeSpecificHack(new ExperienceAdjust());
            // gold multiplier
            addModeSpecificHack(new GoldAdjust());
            // swap orb elements if selected
            addModeSpecificHack(new OrbSwaps());
            // add mode and my name on title screen
            addModeSpecificHack(new TitleScreenAddition());
            // randomize music
            addModeSpecificHack(new CustomMusic());
            // optional difficulty scaling for enemies
            addModeSpecificHack(new EnemyScaling());
            // modify minotaur door so you don't get stuck on the torch
            addModeSpecificHack(new FirePalaceDoorFix());
            // autosave on most door transitions
            addModeSpecificHack(new Autosave());
        }

        protected override void generate(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context)
        {
            applyHacks(origRom, outRom, seed, settings, context);
        }
    }
}
