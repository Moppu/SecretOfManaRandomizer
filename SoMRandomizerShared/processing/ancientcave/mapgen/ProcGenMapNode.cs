using System.Collections.Generic;

namespace SoMRandomizer.processing.ancientcave.mapgen
{
    /// <summary>
    /// A point of interest used when generating maps.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ProcGenMapNode
    {
        // corner location
        public int x;
        public int y;
        // center pos, for spawn spots
        public int centerX;
        public int centerY;
        public string name;
        public List<ProcGenMapNode> connections = new List<ProcGenMapNode>();
        // other optional properties
        public Dictionary<string, int> values = new Dictionary<string, int>();
    }
}
