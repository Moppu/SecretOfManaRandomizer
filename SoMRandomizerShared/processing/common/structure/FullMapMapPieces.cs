using SoMRandomizer.util;
using System.Collections.Generic;

namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// A set of both foreground and background map piece references for a composite map.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FullMapMapPieces
    {
        public List<FullMapMapPieceReference> bgPieces = new List<FullMapMapPieceReference>();
        public List<FullMapMapPieceReference> fgPieces = new List<FullMapMapPieceReference>();

        public override string ToString()
        {
            return "[Map Piece Refs: " +
                "BG=" + DataUtil.ListToString(bgPieces) + "; " +
                "FG=" + DataUtil.ListToString(fgPieces) + "]";
        }
    }
}
