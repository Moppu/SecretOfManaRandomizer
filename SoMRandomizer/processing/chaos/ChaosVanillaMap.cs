using System.Collections.Generic;

namespace SoMRandomizer.processing.chaos
{
    /// <summary>
    /// A vanilla map used for chaos mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosVanillaMap
    {
        public int mapId;
        public List<ChaosDoor> doors;
        public bool layer2Collision;
        public byte[] altPaletteSets;
        public bool removeWalkonEvent;
        public int[] objectsToRemove;
        public bool townMap;
        public int[] triggersToRemove;

        public ChaosVanillaMap(int mapId, List<ChaosDoor> doors, 
            bool layer2Collision, byte[] altPaletteSets, bool removeWalkonEvent,
            int[] objectsToRemove, bool townMap, int[] triggersToRemove)
        {
            this.mapId = mapId;
            this.doors = doors;
            this.layer2Collision = layer2Collision;
            this.altPaletteSets = altPaletteSets;
            this.removeWalkonEvent = removeWalkonEvent;
            this.objectsToRemove = objectsToRemove;
            this.townMap = townMap;
            this.triggersToRemove = triggersToRemove;
        }
    }
}
