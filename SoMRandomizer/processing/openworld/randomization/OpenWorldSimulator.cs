using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// Given a set of item placements for open world, determine whether we can complete the ROM.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldSimulator
    {
        public class SimResult
        {
            public double score = 0.0;
            public List<List<string>> collectionCycles = new List<List<string>>();
        }

        public static SimResult runSimulation(Dictionary<PrizeLocation, PrizeItem> itemPlacements, RandoSettings settings, RandoContext context)
        {
            SimResult result = new SimResult();
            bool startWithBoy = context.workingData.getBool(OpenWorldCharacterSelection.START_WITH_BOY);
            bool startWithGirl = context.workingData.getBool(OpenWorldCharacterSelection.START_WITH_GIRL);
            bool startWithSprite = context.workingData.getBool(OpenWorldCharacterSelection.START_WITH_SPRITE);
            string boyClass = context.workingData.get(OpenWorldClassSelection.BOY_CLASS);
            string girlClass = context.workingData.get(OpenWorldClassSelection.GIRL_CLASS);
            string spriteClass = context.workingData.get(OpenWorldClassSelection.SPRITE_CLASS);
            bool allowLockedItems = settings.getBool(OpenWorldSettings.PROPERTYNAME_ALLOW_MISSED_ITEMS);
            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            int mtrManaSeeds = context.workingData.getInt(OpenWorldMtrSeedNumSelection.MANA_SEEDS_REQUIRED);
            bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
            Dictionary<int, byte> crystalOrbColorMap = ElementSwaps.getCrystalOrbElementMap(context);
            int boyStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.BOY_START_WEAPON_INDEX);
            int girlStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.GIRL_START_WEAPON_INDEX);
            int spriteStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.SPRITE_START_WEAPON_INDEX);
            bool anySpellTriggers = context.workingData.getBool(OpenWorldClassSelection.ANY_MAGIC_EXISTS);

            string _upperLandElement = "no";
            if (flammieDrumInLogic)
            {
                _upperLandElement = (!anySpellTriggers) ? "no" : SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[41], false);
            }

            Dictionary<string, List<string>> plandoSettings = context.plandoSettings;
            List<PrizeLocation> visitedLocations = new List<PrizeLocation>();
            List<string> gottenPrizeNames = new List<string>();
            bool bypassValidation = false;
            // process plando, including stuff we start with
            if (plandoSettings.Count > 0)
            {
                if (plandoSettings.ContainsKey("bypassValidation") && plandoSettings["bypassValidation"].Count > 0 && plandoSettings["bypassValidation"][0] == "yes")
                {
                    // this is the plando override to basically not run any of the simulation and assume everything works.
                    // this should only be used if you think something's wrong here or you're making a broken rom on purpose.
                    Logging.log("Short-circuiting open world simulator because plando told us to.  Possible that this ROM is unbeatable!");
                    bypassValidation = true;
                }
            }
            bool simulating = true;
            int simCycle = 0;
            // process other starter stuff, including character weapons
            if (startWithBoy)
            {
                gottenPrizeNames.Add("boy");
                gottenPrizeNames.Add(SomVanillaValues.weaponByteToName(boyStarterWeapon).ToLower());
            }
            if (startWithGirl)
            {
                gottenPrizeNames.Add("girl");
                gottenPrizeNames.Add(SomVanillaValues.weaponByteToName(girlStarterWeapon).ToLower());
            }
            if (startWithSprite)
            {
                gottenPrizeNames.Add("sprite");
                gottenPrizeNames.Add(SomVanillaValues.weaponByteToName(spriteStarterWeapon).ToLower());
            }

            List<string> girlSpellsChars = new List<string>();
            List<string> spriteSpellsChars = new List<string>();
            if (boyClass == "OGgirl")
            {
                girlSpellsChars.Add("boy");
            }
            if (girlClass == "OGgirl")
            {
                girlSpellsChars.Add("girl");
            }
            if (spriteClass == "OGgirl")
            {
                girlSpellsChars.Add("sprite");
            }
            if (boyClass == "OGsprite")
            {
                spriteSpellsChars.Add("boy");
            }
            if (girlClass == "OGsprite")
            {
                spriteSpellsChars.Add("girl");
            }
            if (spriteClass == "OGsprite")
            {
                spriteSpellsChars.Add("sprite");
            }

            List<string> girlSpellChecks = new List<string>();
            if (boyClass == "OGgirl")
            {
                girlSpellChecks.Add("boy");
            }
            if (girlClass == "OGgirl")
            {
                girlSpellChecks.Add("girl");
            }
            if (spriteClass == "OGgirl")
            {
                girlSpellChecks.Add("sprite");
            }
            List<string> spriteSpellChecks = new List<string>();
            if (boyClass == "OGsprite")
            {
                spriteSpellChecks.Add("boy");
            }
            if (girlClass == "OGsprite")
            {
                spriteSpellChecks.Add("girl");
            }
            if (spriteClass == "OGsprite")
            {
                spriteSpellChecks.Add("sprite");
            }

            while (simulating)
            {
                // add here instead of the one we check to make the cycles more accurate - don't use stuff we found in this cycle to clear this cycle
                List<string> gottenPrizesTemp = new List<string>();

                int visitedSizeBefore = visitedLocations.Count;
                List<string> locationsVisitedThisCycle = new List<string>();
                foreach (PrizeLocation location in itemPlacements.Keys)
                {
                    if (!visitedLocations.Contains(location))
                    {
                        string[] reqs = location.getLockedByPrizes(flammieDrumInLogic, _upperLandElement, goal);
                        bool haveAllReqs = true;
                        foreach (string req in reqs)
                        {
                            bool haveThisReq = false;
                            if (req == "no spells" || req == " spells")
                            {
                                // for boy-only
                                haveThisReq = true;
                            }
                            else if (req == OpenWorldLocations.DEPENDENCY_ELINEE_ENTRY)
                            {
                                // axe one way, whip + cutting weapon the other
                                if (gottenPrizeNames.Contains("axe") || (gottenPrizeNames.Contains("whip") && (gottenPrizeNames.Contains("axe") || gottenPrizeNames.Contains("sword"))))
                                {
                                    haveThisReq = true;
                                }
                            }
                            else if (req == OpenWorldLocations.DEPENDENCY_GIRL_SPELLS)
                            {
                                foreach (string girlCaster in girlSpellsChars)
                                {
                                    if (gottenPrizeNames.Contains(girlCaster))
                                    {
                                        haveThisReq = true;
                                    }
                                }
                            }
                            else if (req == OpenWorldLocations.DEPENDENCY_SPRITE_SPELLS)
                            {
                                foreach (string spriteCaster in spriteSpellsChars)
                                {
                                    if (gottenPrizeNames.Contains(spriteCaster))
                                    {
                                        haveThisReq = true;
                                    }
                                }
                            }
                            else
                            {
                                string[] girlOnlySpells = new string[] { "lumina" };
                                string[] bothSpells = new string[] { "sylphid", "salamando" };
                                string[] spriteOnlySpells = new string[] { "undine", "gnome", "shade", "dryad", "luna" };
                                foreach (string item in gottenPrizeNames)
                                {
                                    if (item == req)
                                    {
                                        haveThisReq = true;
                                        if (item.Contains(" spells"))
                                        {
                                            string elementName = item.Split(new char[] { ' ' })[0];
                                            if (girlOnlySpells.Contains(elementName))
                                            {
                                                bool haveAny = false;
                                                foreach (string girlCaster in girlSpellChecks)
                                                {
                                                    if (gottenPrizeNames.Contains(girlCaster))
                                                    {
                                                        haveAny = true;
                                                    }
                                                }
                                                if (!haveAny)
                                                {
                                                    haveThisReq = false;
                                                }
                                            }
                                            else if (spriteOnlySpells.Contains(elementName))
                                            {
                                                bool haveAny = false;
                                                foreach (string spriteCaster in spriteSpellChecks)
                                                {
                                                    if (gottenPrizeNames.Contains(spriteCaster))
                                                    {
                                                        haveAny = true;
                                                    }
                                                }
                                                if (!haveAny)
                                                {
                                                    haveThisReq = false;
                                                }
                                            }
                                            else if (bothSpells.Contains(elementName))
                                            {
                                                bool haveAny = false;
                                                foreach (string girlCaster in girlSpellChecks)
                                                {
                                                    if (gottenPrizeNames.Contains(girlCaster))
                                                    {
                                                        haveAny = true;
                                                    }
                                                }
                                                foreach (string spriteCaster in spriteSpellChecks)
                                                {
                                                    if (gottenPrizeNames.Contains(spriteCaster))
                                                    {
                                                        haveAny = true;
                                                    }
                                                }
                                                if (!haveAny)
                                                {
                                                    haveThisReq = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (!haveThisReq)
                            {
                                haveAllReqs = false;
                            }
                        }

                        if (haveAllReqs || bypassValidation)
                        {
                            double locationScore = location.reachability;
                            double itemScore = itemPlacements[location].value;
                            result.score += locationScore * itemScore;

                            visitedLocations.Add(location);
                            locationsVisitedThisCycle.Add(location.locationName);
                            gottenPrizesTemp.Add(itemPlacements[location].prizeName);
                            if (itemPlacements[location].prizeName == "boy")
                            {
                                gottenPrizesTemp.Add(SomVanillaValues.weaponByteToName(boyStarterWeapon).ToLower());
                            }
                            if (itemPlacements[location].prizeName == "girl")
                            {
                                gottenPrizesTemp.Add(SomVanillaValues.weaponByteToName(girlStarterWeapon).ToLower());
                            }
                            if (itemPlacements[location].prizeName == "sprite")
                            {
                                gottenPrizesTemp.Add(SomVanillaValues.weaponByteToName(spriteStarterWeapon).ToLower());
                            }
                        }
                    }
                }
                int visitedSizeAfter = visitedLocations.Count;

                // nothing this cycle; we're done
                if (visitedSizeBefore == visitedSizeAfter)
                {
                    simulating = false;
                }

                result.collectionCycles.Add(locationsVisitedThisCycle);
                gottenPrizeNames.AddRange(gottenPrizesTemp);
                simCycle++;
            }

            bool simSuccessCondition = visitedLocations.Count == itemPlacements.Count;
            // determine if we can get to what we consider the end of the run even if we can't get everything
            if (allowLockedItems)
            {
                if (goal == OpenWorldGoalProcessor.GOAL_MANABEAST)
                {
                    // vanilla - can reach dread slime (basically finish manafort)
                    simSuccessCondition = false;
                    foreach (PrizeLocation loc in visitedLocations)
                    {
                        if (loc.locationName == "dread slime (new item)")
                        {
                            simSuccessCondition = true;
                        }
                    }
                }
                else if (goal == OpenWorldGoalProcessor.GOAL_MTR)
                {
                    // MTR - we have the necessary seeds, and we can reach the final boss of purelands
                    bool canReachManaTree = false;
                    foreach (PrizeLocation loc in visitedLocations)
                    {
                        if (loc.locationName == "blue dragon (new item)")
                        {
                            canReachManaTree = true;
                        }
                    }
                    int numSeeds = 0;
                    foreach (string prize in gottenPrizeNames)
                    {
                        if (prize.Contains(" seed"))
                        {
                            numSeeds++;
                        }
                    }
                    simSuccessCondition = canReachManaTree && numSeeds >= mtrManaSeeds;
                }
                else if (goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
                {
                    // sants gift mode - we can give the final gift, and have reached the others
                    bool gotLastSantaPrize = false;
                    foreach (PrizeLocation loc in visitedLocations)
                    {
                        if (loc.locationName == "santa gift 6")
                        {
                            gotLastSantaPrize = true;
                        }
                    }

                    string lastXmasGift = context.workingData.get(OpenWorldLocationPrizeMatcher.GIFT_SELECTION_PREFIX + 7);
                    // since there's no final location, determine if we can get to it by checking if we got the item required
                    // for the last gift, if it's something we can't just buy at the store, or money
                    if (lastXmasGift.EndsWith("spells") || lastXmasGift == "moogle belt" || lastXmasGift == "midge mallet")
                    {
                        gotLastSantaPrize &= gottenPrizeNames.Contains(lastXmasGift);
                    }
                }
                // no option for reindeer mode, keep all visited locations
            }

            if(!simSuccessCondition)
            {
                result.score = -1;
            }
            return result;
        }
    }
}
