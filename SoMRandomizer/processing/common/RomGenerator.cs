using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.hacks.common.enhancement;
using SoMRandomizer.processing.hacks.common.fix;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.processing.hacks.common.qol;
using SoMRandomizer.processing.hacks.common.util;
using SoMRandomizer.processing.hacks.openworld;
using SoMRandomizer.util.rng;
using System;
using System.Collections.Generic;
using System.IO;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Parent class to all modes' rom generators.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public abstract class RomGenerator
    {
        // rando current version.  this should be updated for every release, and should always be numeric for comparison's sake.
        public static string VERSION_NUMBER = "1.38";

        private List<RandoProcessor> commonHacks = new List<RandoProcessor>();
        private List<RandoProcessor> modeSpecificHacks = new List<RandoProcessor>();

        public RomGenerator()
        {
            // supported common hacks.  most of these are controlled by a settings toggle, and though they are
            // all added here, do not actually apply themselves unless the toggle has been enabled.

            // randomize weapons
            commonHacks.Add(new WeaponRandomizer());
            // add name of mode to title screen
            commonHacks.Add(new TitleScreenAddition());
            // 4x magic leveling speed
            commonHacks.Add(new FasterMagicLevels());
            // 4x weapon leveling speed
            commonHacks.Add(new FasterWeaponLevels());
            // speed up status messages
            commonHacks.Add(new FasterTopOfScreenDialogue());
            // restore MP at levelup
            commonHacks.Add(new LevelupMpRestore());
            // faster chest opening
            commonHacks.Add(new ChestOpeningSpeedup());
            // change some mp costs etc
            commonHacks.Add(new MagicRebalance());
            // make statuses last longer
            commonHacks.Add(new StatusLengths());
            // random color rabites
            commonHacks.Add(new RabiteColorRandomizer());
            // set player color palettes
            commonHacks.Add(new CharacterPaletteRandomizer());
            // allow p2/p3 to scroll off screen
            commonHacks.Add(new FreeWalking());
            // disable overcharge glitch
            commonHacks.Add(new OverchargeFix());
            // disable some sound effects
            commonHacks.Add(new NoFootstepSound());
            // only allow sound effects on spc channels 6,7,8
            commonHacks.Add(new SoundEffectsReduction());
            // speed up mana sword pulling animation in intro
            commonHacks.Add(new NoSwordPullAnimation());
            // max items increased from 4 to 7
            commonHacks.Add(new MaxItemsIncrease());
            // shrug
            commonHacks.Add(new HexasFix());
            // boss AI speedup
            commonHacks.Add(new FastBosses());
            // speedup spell is haste
            commonHacks.Add(new Speedup());
            // palette glow for statuses
            commonHacks.Add(new StatusGlow());
            // vampire hittable when flying
            commonHacks.Add(new HittableVampire());
            // include starter gear pieces
            commonHacks.Add(new StarterGear());
            // fix some issue with weapon orb rewards
            commonHacks.Add(new OrbRewardFix());
            // fix softlock with character animations at boss death 
            commonHacks.Add(new BossDeathFix());
            // allow enemy type damage on weapons
            commonHacks.Add(new EnemyTypeDamage());
            // allow elemental damage on weapons
            commonHacks.Add(new ElementalDamage());
            // remove fade-out from transitions
            commonHacks.Add(new FastTransitions());
            // flammie graphical bug fix
            commonHacks.Add(new Mode7EdgesFix());
            // more accurate damage percentages
            commonHacks.Add(new DamagePercentFix());
            // running doesn't cost stamina
            commonHacks.Add(new NoEnergyToRun());
            // gigases don't turn into dust
            commonHacks.Add(new GigasesNeverSplit());
            // fix some issues with whip coordinates
            commonHacks.Add(new WhipFix());
            // allow offscreen targetting
            commonHacks.Add(new FreeTargetting());
            // buy multiple consumables at once
            commonHacks.Add(new BuyMultipleItems());
            // fixed charge bonus for mana magic instead of based on weapon levels
            commonHacks.Add(new ManaMagicFix());
            // OHKO
            commonHacks.Add(new OneHitKo());
            // fix issues with summoning enemies despawning important stuff
            commonHacks.Add(new SummonDespawnFix());
            // walk through walls
            commonHacks.Add(new NoCollision());
            // fix confusion staying on forever if you save
            commonHacks.Add(new ConfuseFix());
            // make spring beak easier to hit
            commonHacks.Add(new BeakShieldDisable());
            // fix loading of enemy names for ring menu
            commonHacks.Add(new HighLevelEnemyNameFix());
            // fix map glitches when mech rider dies
            commonHacks.Add(new MechRiderDeathFix());
            // move enemies that are near doors
            commonHacks.Add(new EnemyPositionAdjustments());
            // attacks don't cost stamina
            commonHacks.Add(new NoEnergyToAttack());
            // aggressive enemies 
            commonHacks.Add(new FasterEnemies());
            // players are permanently poisoned
            commonHacks.Add(new AlwaysPoisoned());
            // poison works by percentage
            commonHacks.Add(new PercentagePoison());
            // don't show damage numbers
            commonHacks.Add(new ObscureDamage());
            // don't show own HP
            commonHacks.Add(new ObscureOwnHp());
            // don't show own gold
            commonHacks.Add(new ObscureGoldAmount());
            // change how defense applies to damage
            commonHacks.Add(new DefenseRefactor());
            // show zero damage hits for evades
            commonHacks.Add(new DisplayEvadesAsZero());
            // keep all damage numbers on screen
            commonHacks.Add(new NumbersOnScreen());
            // randomize boss elements
            commonHacks.Add(new BossElementRando());
            // allow 12 letter names
            commonHacks.Add(new NameEntryChanges());
            // enemies have infinite mp
            commonHacks.Add(new EnemyInfiniteMp());
            // fix damage canceling with items
            commonHacks.Add(new DamageCancelFix());
            // add flammie minimap
            commonHacks.Add(new Ff6StyleMiniMap());
            // allow cup of wishes on zero HP living character
            commonHacks.Add(new CupWishesOnZeroHp());
            // allow candies on zero HP living character
            commonHacks.Add(new CandyHealouts());
            // fix issue where you can die while using magic rope
            commonHacks.Add(new MagicRopeDeathFix());
            // set initial values when starting
            commonHacks.Add(new InitialValues());
            // change the mushroom enemies into vineshrooms
            commonHacks.Add(new Vineshroom());
        }

        protected void addModeSpecificHack(RandoProcessor modeSpecificHack)
        {
            modeSpecificHacks.Add(modeSpecificHack);
        }

        // note that this will overwrite destPath if it exists. if this is undesirable, it should be checked first before calling
        public static void initGeneration(string sourcePath, string destPath, string seed, Dictionary<string, RomGenerator> generatorsByRomType, CommonSettings commonSettings, Dictionary<string, RandoSettings> settingsByRomType)
        {
            if (sourcePath == null || sourcePath == "" || destPath == null || destPath == "")
            {
                throw new Exception("No ROM selected.");
            }
            else if (sourcePath == destPath)
            {
                throw new Exception("Failed - source and destination file are the same!");
            }
            else
            {
                try
                {
                    FileStream fs = new FileStream(destPath, FileMode.Create, FileAccess.Write);
                    if (!fs.CanWrite)
                    {
                        throw new Exception("Can't write to destination file.");
                    }
                    fs.Close();
                }
                catch (Exception)
                {
                    throw new Exception("Can't write to destination file.");
                }
            }
            byte[] origRom = null;
            try
            {
                origRom = File.ReadAllBytes(sourcePath);
            }
            catch (Exception e)
            {
                throw new Exception("Cannot open source file: " + e.Message);
            }
            if (origRom == null)
            {
                throw new Exception("Cannot open source file");
            }
            if (origRom.Length != 2 * 1024 * 1024 && origRom.Length != 2 * 1024 * 1024 + 0x200)
            {
                throw new Exception("Source file had unexpected size; should be 16 Megabit ROM");
            }

            int headerSize = 0;
            if (origRom.Length != 2 * 1024 * 1024)
            {
                headerSize = 0x200;
            }
            byte[] outFile = new byte[4 * 1024 * 1024];
            for (int i = 0; i < 2 * 1024 * 1024; i++)
            {
                outFile[i] = origRom[i + headerSize];
            }

            char[] nameChars = new char[14];
            // remove header from origRom too
            origRom = new byte[outFile.Length];
            for (int i = 0; i < 2 * 1024 * 1024; i++)
            {
                origRom[i] = outFile[i];
            }
            headerSize = 0;

            for (int i = 0; i < nameChars.Length; i++)
            {
                nameChars[i] = (char)origRom[i + 0xFFC0 + headerSize];
            }
            if (new string(nameChars) != "Secret of MANA")
            {
                throw new Exception("Name of game did not match the expected \"Secret of MANA\"");
            }
            if (origRom[0xFFD9 + headerSize] != 1)
            {
                throw new Exception("Please use US ROM only.");
            }
            if (origRom[0xFFDB + headerSize] != 0)
            {
                throw new Exception("Please use version 1.0 only.");
            }

            // this is just for logging later
            commonSettings.set("SourceROMName", sourcePath);
            commonSettings.set("TargetROMName", destPath);

            try
            {
                string assemblyVersion = "" + typeof(RomGenerator).Assembly.GetName().Version;
                string[] versionTokens = assemblyVersion.Split(new char[] { '.' });
                string days = versionTokens[2];
                string minutes = versionTokens[3];
                // baseline is 01/01/2000
                DateTime date = new DateTime(2000, 1, 1)
                // build is number of days after baseline
                .AddDays(Int32.Parse(days))
                // revision is half the number of seconds into the day
                .AddSeconds(Int32.Parse(minutes) * 2);
                commonSettings.set(CommonSettings.PROPERTYNAME_BUILD_DATE, "" + date);
            }
            catch (Exception)
            {
                // shrug i guess; can't determine build date
            }

            string romType = commonSettings.get(CommonSettings.PROPERTYNAME_MODE);
            commonSettings.setInt(CommonSettings.PROPERTYNAME_CURRENT_PROGRESS, 0);
            bool success = generatorsByRomType[romType].generate(origRom, outFile, seed, settingsByRomType[romType]);
            if (success)
            {
                File.WriteAllBytes(destPath, outFile);
            }
            else
            {
                throw new Exception("Error encountered! Check the log for details.");
            }
            commonSettings.setInt(CommonSettings.PROPERTYNAME_CURRENT_PROGRESS, 0);
        }

        public bool generate(byte[] origRom, byte[] outRom, String seed, RandoSettings settings)
        {
            RandoContext context = new RandoContext();
            string mode = settings.get(CommonSettings.PROPERTYNAME_MODE);

            // https://github.com/dotnet/coreclr/blob/release/1.1.0/src/mscorlib/src/System/Random.cs/
            // https://github.com/mono/mono/blob/master/mcs/class/Mono.C5/C5/Random.cs
            context.randomFunctional = new DotNet110Random(HashcodeUtil.GetDeterministicHashCode(seed));
            // different random for cosmetics, so you can change them and not impact the rando
            context.randomCosmetic = new DotNet110Random(HashcodeUtil.GetDeterministicHashCode(seed + "_cosmetic"));
            context.namesOfThings = new NamesOfThings(outRom);

            context.originalRom = origRom;
            context.outputRom = outRom;

            // open log files
            Logging fileLogger = null;
            Logging fileLoggerSpoiler = null;
            Logging fileLoggerDebug = null;

            String filenameSeed = seed;
            char[] badChars = new char[] { '\\', '/', ':', '<', '>', '\'', '\"', '*', '?', '|' };
            foreach (char c in badChars)
            {
                filenameSeed = filenameSeed.Replace(c, '_');
            }
            Logging.ClearLoggers();
            if (settings.getBool(CommonSettings.PROPERTYNAME_TEST_ONLY))
            {
                fileLogger = new DebugLogger();
                fileLoggerSpoiler = new DebugLogger();
                fileLoggerDebug = new DebugLogger();
            }
            else
            {
                fileLogger = new FileLogger("./log_" + filenameSeed + ".txt");
                if (settings.getBool(CommonSettings.PROPERTYNAME_SPOILER_LOG))
                {
                    fileLoggerSpoiler = new FileLogger("./log_" + filenameSeed + "_SPOILER.txt");
                }
                else
                {
                    fileLoggerSpoiler = new NullWriter();
                }
                if (settings.getBool(CommonSettings.PROPERTYNAME_DEBUG_LOG))
                {
                    fileLoggerDebug = new FileLogger("./log_" + filenameSeed + "_DEBUG.txt");
                }
                else
                {
                    fileLoggerDebug = new NullWriter();
                }
            }
            Logging.AddLogger(fileLogger);
            Logging.AddLogger("spoiler", fileLoggerSpoiler);
            Logging.AddLogger("debug", fileLoggerDebug);
            Logging.AddLogger(fileLoggerDebug); // include general messages in the debug log, too
            Logging.debugEnabled = true;

            // log all the incoming settings, and seed
            Logging.log("-------------------------------------------------");
            Logging.log("Begin ROM generation at " + DateTime.Now + " with version " + settings.get(CommonSettings.PROPERTYNAME_VERSION));
            Logging.log("Seed = " + seed);
            Logging.log("Options = " + settings);
            Logging.log("-------------------------------------------------");

            try
            {
                // main rom generation
                generate(origRom, outRom, seed, settings, context);
                // a few post-processing steps with data defined by the above
                context.namesOfThings.setAllNames(outRom, ref context.workingOffset);
                if (context.replacementEvents.Count > 0)
                {
                    EventExpander eventExpander = new EventExpander();
                    eventExpander.process(outRom, context.replacementEvents, ref context.workingOffset);
                }
                if (context.replacementMapPieces.Count > 0)
                {
                    MapPieceExpander mapPieceExpander = new MapPieceExpander();
                    mapPieceExpander.process(outRom, context.replacementMapPieces, ref context.workingOffset);
                }
                if (context.replacementDoors.Count > 0)
                {
                    DoorReplacer doorReplacer = new DoorReplacer();
                    doorReplacer.process(outRom, context);
                }
                if(context.generatedMaps.Count > 0)
                {
                    FullMapReplacer fullMapReplacer = new FullMapReplacer();
                    fullMapReplacer.process(outRom, context);
                }
                if(context.replacementMapPalettes.Count > 0)
                {
                    MapPaletteSetReplacer mapPaletteSetReplacer = new MapPaletteSetReplacer();
                    mapPaletteSetReplacer.process(context);
                }

                context.eventHackMgr.process(outRom, ref context.workingOffset);
                Logging.log("Done!");
                return true;
            }
            catch(Exception e)
            {
                Logging.log("Exception encountered! Exception message: " + e.Message + "\nStack trace:\n" + e.StackTrace);
                return false;
            }
            finally
            {
                if (fileLogger != null)
                {
                    fileLogger.close();
                }
                if (fileLoggerSpoiler != null)
                {
                    fileLoggerSpoiler.close();
                }
                if (fileLoggerDebug != null)
                {
                    fileLoggerDebug.close();
                }
            }
        }

        protected abstract void generate(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context);

        private void applySharedHacks(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context)
        {
            
        }

        private void applyModeSpecificHacks(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context)
        {

        }

        protected void applyHacks(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context)
        {
            foreach(RandoProcessor commonHack in commonHacks)
            {
                commonHack.add(origRom, outRom, seed, settings, context);
            }
            foreach (RandoProcessor modeSpecificHack in modeSpecificHacks)
            {
                modeSpecificHack.add(origRom, outRom, seed, settings, context);
            }
        }
    }
}
