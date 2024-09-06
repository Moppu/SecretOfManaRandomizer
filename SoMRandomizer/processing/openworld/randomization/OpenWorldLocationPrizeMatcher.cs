using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.openworld;
using SoMRandomizer.processing.openworld.events;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.hacks.openworld.XmasRandoData;
using static SoMRandomizer.processing.openworld.PlandoProperties;

namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// Open world logic to pair locations with prizes.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldLocationPrizeMatcher
    {
        public const string GIFT_SELECTION_PREFIX = "giftSelection";

        public static bool placeItems(List<PrizeLocation> allLocations, List<PrizeItem> allPrizes, RandoSettings settings, RandoContext context, Dictionary<PrizeLocation, PrizeItem> itemPlacements)
        {
            Random r = context.randomFunctional;
            Dictionary<string, List<string>> plandoSettings = context.plandoSettings;
            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            List<string> plandoLocations = new List<string>();
            bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
            Dictionary<int, byte> crystalOrbColorMap = ElementSwaps.getCrystalOrbElementMap(context);
            bool force100PercentPossible = !settings.getBool(OpenWorldSettings.PROPERTYNAME_ALLOW_MISSED_ITEMS);
            bool anySpellTriggers = context.workingData.getBool(OpenWorldClassSelection.ANY_MAGIC_EXISTS);
            string _upperLandElement = "no";
            if (flammieDrumInLogic)
            {
                _upperLandElement = (!anySpellTriggers) ? "no" : SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_UPPERLAND], false);
            }

            List<PrizeLocation> filteredLocations = goal == OpenWorldGoalProcessor.GOAL_GIFTMODE ? GiftModeProcessing.processLocations(settings, context, allLocations, allPrizes) : allLocations;

            Dictionary<PrizeLocation, List<PrizeItem>> allAvailablePrizes = new Dictionary<PrizeLocation, List<PrizeItem>>();
            // default to everything available everywhere
            foreach (PrizeLocation prizeLocation in filteredLocations)
            {
                List<PrizeItem> availablePrizes = new List<PrizeItem>(allPrizes);
                // don't allow flammie drum to be at locations that it's required for.
                if (OpenWorldPrizes.flammieRequiredLocations.Contains(prizeLocation.locationName))
                {
                    PrizeItem flammiePrize = null;
                    foreach (PrizeItem pi in availablePrizes)
                    {
                        if (pi.prizeName == "flammie drum")
                        {
                            flammiePrize = pi;
                        }
                    }
                    if (flammiePrize != null)
                    {
                        availablePrizes.Remove(flammiePrize);
                    }
                }
                allAvailablePrizes[prizeLocation] = availablePrizes;
            }

            if (plandoSettings.Count > 0)
            {
                if (!Plando.doPlando(r, plandoSettings, itemPlacements, allAvailablePrizes, allPrizes, plandoLocations))
                {
                    throw new Exception("Plando specified impossible settings! See log for details.");
                }
            }

            bool bypassValidation = plandoSettings.ContainsKey(KEY_BYPASS_VALIDATION) && plandoSettings[KEY_BYPASS_VALIDATION].Contains("yes");

            for (int i = 0; i < 7; i++)
            {
                foreach (PrizeLocation loc in filteredLocations)
                {
                    if (loc.locationName == "santa gift " + i)
                    {
                        loc.lockedByPrizes = new string[] { };
                    }
                }
            }

            foreach (PrizeLocation prizeLocation in filteredLocations)
            {
                // skip if we plandoed it
                if (plandoLocations.Contains(prizeLocation.locationName))
                {
                    continue;
                }
                List<PrizeItem> availablePrizes = allAvailablePrizes[prizeLocation];

                if (prizeLocation.locationName == "starter weapon (main)")
                {
                    // special case these to match
                    availablePrizes.Clear();
                    availablePrizes.Add(lookupByName(allPrizes, "starter weapon (main)"));
                }
                else if (prizeLocation.locationName == "starter weapon (alt)")
                {
                    // special case these to match
                    availablePrizes.Clear();
                    availablePrizes.Add(lookupByName(allPrizes, "starter weapon (alt)"));
                }
                else
                {
                    // don't allow these to show up anywhere else.
                    removeByName(availablePrizes, "starter weapon (main)");
                    removeByName(availablePrizes, "starter weapon (alt)");
                }

                if (prizeLocation.locationName.Contains("chest"))
                {
                    // don't allow "nothing" in chests since we don't have a flag for getting it
                    removeByName(availablePrizes, "nothing");
                }
                else if (prizeLocation.locationName == "watts (axe)")
                {
                    // watts can't have nothing either, otherwise his shop never shows
                    removeByName(availablePrizes, "nothing");
                }

                // remove stuff directly locked here
                string[] plLocked = prizeLocation.getLockedByPrizes(flammieDrumInLogic, _upperLandElement, goal);
                foreach (string lockPrize in plLocked)
                {
                    removeByName(availablePrizes, lockPrize);
                    if (lockPrize == "lumina spells")
                    {
                        removeByName(availablePrizes, "girl");
                    }
                    else if (lockPrize.Contains(" spells"))
                    {
                        removeByName(availablePrizes, "sprite");
                    }
                }

                // nothing? oh boy. start over.
                if (availablePrizes.Count == 0)
                {
                    // this is a big problem that not even bypassValidation can fix.
                    Logging.log("uh oh.  Ran out of prizes when we got to: " + prizeLocation.locationName);
                    return false;
                }

                // pick something for here
                PrizeItem thisPrize = availablePrizes[r.Next() % availablePrizes.Count];
                itemPlacements[prizeLocation] = thisPrize;

                foreach (PrizeLocation prizeLocation2 in filteredLocations)
                {
                    string[] pl2Locked = prizeLocation2.getLockedByPrizes(flammieDrumInLogic, _upperLandElement, goal);
                    // don't allow the prize to go anywhere else now that we put it here.
                    allAvailablePrizes[prizeLocation2].Remove(thisPrize);
                    if (pl2Locked.ToList().Contains(thisPrize.prizeName))
                    {
                        // now, anything that is locked by this item can't also have the locks to this spot
                        List<PrizeItem> availablePrizes2 = allAvailablePrizes[prizeLocation2];
                        string existingPrize = "";
                        if (itemPlacements.ContainsKey(prizeLocation2))
                        {
                            existingPrize = itemPlacements[prizeLocation2].prizeName;
                        }
                        if (force100PercentPossible)
                        {
                            foreach (string lockPrize in plLocked)
                            {
                                removeByName(availablePrizes2, lockPrize);
                                // not sure if this can actually happen, but it's bad if it does
                                if (existingPrize == lockPrize && !bypassValidation)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            
            // inject santa gifts' dependencies based on what all we picked
            // prizesForLookup: location name -> prize name
            // prizeObjectsForLookup location name -> prize obj
            if (goal == OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                List<string> xmasGifts = new List<string>();
                List<string> possibleGifts = new List<string>();
                possibleGifts.AddRange(GiftModeProcessing.possibleXmasGifts.Keys);
                for (int i = 0; i < 8; i++)
                {
                    int id = r.Next() % possibleGifts.Count;
                    // hints and simulator use this, so stick it on the context
                    xmasGifts.Add(possibleGifts[id]);
                    context.workingData.set(GIFT_SELECTION_PREFIX + i, possibleGifts[id]);
                    possibleGifts.RemoveAt(id);
                }
                HashSet<string> runningItemDependencies = new HashSet<string>();
                // the last prize is finishing the game, so we don't add dependencies to that
                int numGifts = settings.getInt(OpenWorldSettings.PROPERTYNAME_NUM_XMAS_GIFTS);
                for (int i = 0; i < numGifts - 1; i++)
                {
                    int npcId = context.workingData.getInt(GiftDeliveryIntroEvent.GIFT_DELIVERY_INDEX_PREFIX + i);
                    NpcLocationData npc = GiftDeliveryIntroEvent.DELIVERY_LOCATIONS[npcId];

                    // add any extra dependencies for the npc (elinee for example has axe/sword/whip to visit)
                    foreach (string dep in npc.itemDependencies)
                    {
                        runningItemDependencies.Add(dep);
                    }
                    // if we're gifting to a known location (like salamando), add his location dependencies
                    foreach (PrizeLocation pl in allLocations)
                    {
                        string[] plLocked = pl.getLockedByPrizes(flammieDrumInLogic, _upperLandElement, goal);
                        if (pl.eventNum == npc.eventId)
                        {
                            foreach (string dep in plLocked)
                            {
                                runningItemDependencies.Add(dep);
                            }
                        }
                    }
                    // add dependencies of location where the prize is
                    foreach (PrizeLocation location in itemPlacements.Keys)
                    {
                        if (location.locationName == "santa gift " + i)
                        {
                            foreach (PrizeLocation loc in filteredLocations)
                            {
                                string[] locLocked = loc.getLockedByPrizes(flammieDrumInLogic, _upperLandElement, goal);
                                if (loc.locationName == location.locationName)
                                {
                                    foreach (string dep in locLocked)
                                    {
                                        runningItemDependencies.Add(dep);
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }

                    if (xmasGifts[i].EndsWith("spells") || xmasGifts[i] == "moogle belt" || xmasGifts[i] == "midge mallet")
                    {
                        runningItemDependencies.Add(xmasGifts[i]);
                        foreach (PrizeLocation location in itemPlacements.Keys)
                        {
                            if (itemPlacements[location].prizeName == xmasGifts[i])
                            {
                                foreach (PrizeLocation loc in filteredLocations)
                                {
                                    string[] locLocked = loc.getLockedByPrizes(flammieDrumInLogic, _upperLandElement, goal);
                                    if (loc.locationName == location.locationName)
                                    {
                                        foreach (string dep in locLocked)
                                        {
                                            runningItemDependencies.Add(dep);
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    foreach (PrizeLocation loc in filteredLocations)
                    {
                        if (loc.locationName == "santa gift " + i)
                        {
                            loc.lockedByPrizes = runningItemDependencies.ToArray();
                        }
                    }
                }
            }

            return true;
        }

        private static PrizeItem lookupByName(List<PrizeItem> allItems, string name)
        {
            foreach(PrizeItem item in allItems)
            {
                if(item.prizeName == name)
                {
                    return item;
                }
            }
            return null;
        }

        private static PrizeLocation lookupByName(List<PrizeLocation> allLocations, string name)
        {
            foreach (PrizeLocation loc in allLocations)
            {
                if (loc.locationName == name)
                {
                    return loc;
                }
            }
            return null;
        }

        private static void removeByName(List<PrizeItem> availablePrizes, string removal)
        {
            List<PrizeItem> f = new List<PrizeItem>();
            foreach (PrizeItem item in availablePrizes)
            {
                if (item.prizeName == removal)
                {
                    f.Add(item);
                }
            }
            foreach (PrizeItem ff in f)
            {
                availablePrizes.Remove(ff);
            }
        }

    }
}
