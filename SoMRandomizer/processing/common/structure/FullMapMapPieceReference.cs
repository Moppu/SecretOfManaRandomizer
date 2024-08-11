namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// A set of map piece references for a composite map.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FullMapMapPieceReference
    {
        public int indexOnMap;
        public bool isForegroundPiece;
        public int pieceIndex;
        public byte xPos;
        public byte yPos;
        public byte eventVisibilityFlagId;
        public byte eventVisibilityValueLow;
        public byte eventVisibilityValueHigh;

        public override string ToString()
        {
            return "[Map Piece Ref: " +
                "x=" + xPos.ToString("X2") + "; " +
                "y=" + yPos.ToString("X2") + "; " +
                "piece=" + pieceIndex.ToString("X4") + "; " +
                "vis flag=" + eventVisibilityFlagId.ToString("X2") + "; " +
                "vis low=" + eventVisibilityValueLow.ToString("X2") + "; " +
                "vis high=" + eventVisibilityValueHigh.ToString("X2") + 
                "]";
        }
    }
}
