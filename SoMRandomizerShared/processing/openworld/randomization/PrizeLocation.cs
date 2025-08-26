using System.Collections.Generic;

namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// A location at which open world "checks" can occur.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class PrizeLocation
    {
        // name of the location - these are unique
        public string locationName;
        // map number
        public int mapNum;
        // npc object number on map; -1 for none
        public int objNum;
        // event parts to replace with item we're giving
        public int eventNum;
        public int eventReplacementIndex;
        // empty if no restrictions
        public string[] prizeTypeOptions;

        public string[] locationHints;

        public string[] lockedByPrizes;
        // higher is easy to get
        public double reachability = 0;

        public PrizeLocation(string name, int map, int obj, int evNum, int evReplaceIndex, string[] typeOptions, string[] hints, string[] lockedBy, double locationReachability)
        {
            // chest constructor, with event replacement for object to ensure it disappears
            locationName = name;
            mapNum = map;
            objNum = obj;
            eventNum = evNum;
            eventReplacementIndex = evReplaceIndex;
            prizeTypeOptions = typeOptions;
            locationHints = hints;
            lockedByPrizes = lockedBy;
            reachability = locationReachability;
        }

        public PrizeLocation(string name, int evNum, int evReplaceIndex, string[] typeOptions, string[] hints, string[] lockedBy, double locationReachability) : this(name, -1, -1, evNum, evReplaceIndex, typeOptions, hints, lockedBy, locationReachability)
        {
            // non-chest constructor
        }

        public PrizeLocation(string name, int evNum, int evReplaceIndex, string[] typeOptions, string[] lockedBy, double locationReachability) : this(name, evNum, evReplaceIndex, typeOptions, new string[] { }, lockedBy, locationReachability)
        {
            // no hints constructor for starting stuff
        }

        public string[] getLockedByPrizes(bool flammieDrumInLogic, string _upperLandElement, string goal)
        {
            List<string> allPrizes = new List<string>();
            allPrizes.AddRange(lockedByPrizes);
            if (flammieDrumInLogic)
            {
                if (OpenWorldPrizes.flammieRequiredLocations.Contains(locationName))
                {
                    allPrizes.Add("flammie drum");
                    if (_upperLandElement != "no")
                    {
                        allPrizes.Add(_upperLandElement + " spells");
                    }
                }
                else
                {
                    if (goal == OpenWorldGoalProcessor.GOAL_GIFTMODE || goal == OpenWorldGoalProcessor.GOAL_REINDEER)
                    {
                        // reverse logic since we start in ice country
                        if (!OpenWorldPrizes.upperLandOrbRequiredLocations.Contains(locationName))
                        {
                            if (_upperLandElement != "no")
                            {
                                allPrizes.Add(_upperLandElement + " spells");
                            }
                        }
                    }
                    else
                    {
                        if (OpenWorldPrizes.upperLandOrbRequiredLocations.Contains(locationName))
                        {
                            if (_upperLandElement != "no")
                            {
                                allPrizes.Add(_upperLandElement + " spells");
                            }
                        }
                    }
                }
            }
            return allPrizes.ToArray();
        }

        public override bool Equals(object obj)
        {
            return obj is PrizeLocation && locationName == ((PrizeLocation)obj).locationName;
        }

        public override int GetHashCode()
        {
            // assume unique names
            return locationName.GetHashCode();
        }
    }
}
