using System.Collections.Generic;

namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// Representation of a "composite" map.  Contains multiple portions, stored in different parts of the ROM.  See VanillaMapUtil for details.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FullMap
    {
        public MapHeader mapHeader;
        public List<MapObject> mapObjects = new List<MapObject>();
        public FullMapMapPieces mapPieces;
        public List<ushort> mapTriggers = new List<ushort>();
    }
}
