using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Data structure for setting up gifts for open world Christmas mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class XmasRandoData
    {
        public List<NpcLocationData> npcs = new List<NpcLocationData>();
        public int introEventId;
        public List<int> santaGiftEventIds = new List<int>(); 
        public List<int> santaNpcGiftNameEventIds = new List<int>(); 

        public class NpcLocationData
        {
            public string key;
            public string description;
            public int eventId;
            public int newEventBeforeGift;
            public int newEventGiveGift;
            public bool female;
            public string[] itemDependencies;

            public NpcLocationData(string key, string description, int eventId, bool female, string[] itemDependencies)
            {
                this.key = key;
                this.description = description;
                this.eventId = eventId;
                this.female = female;
                this.itemDependencies = itemDependencies;
            }
        }

        public class ReplacementObject
        {
            public int mapNum;
            public int index;
            public string[] hintLocations;

            public int[] indexExceptions = new int[] { };
            public ReplacementObject(int mapNum, int index, string[] hintLocations)
            {
                this.mapNum = mapNum;
                this.index = index;
                this.hintLocations = hintLocations;
            }
            public ReplacementObject(int mapNum, int index, string[] hintLocations, int[] indexExceptions)
            {
                this.mapNum = mapNum;
                this.index = index;
                this.hintLocations = hintLocations;
                this.indexExceptions = indexExceptions;
            }
        }
    }
}
