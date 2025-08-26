using SoMRandomizer.processing.common.structure;
using System.Linq;

namespace SoMRandomizer.processing.common.mapgen
{
    /// <summary>
    /// Utility used to copy the vanilla credits map.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CreditsMap
    {
        public const ushort MAP_ID = 0xB4;

        // create a copy of the credits map (vanilla 0xB4) for ancient cave etc
        public static void makeCreditsMap(byte[] origRom, RandoContext context)
        {
            // vanilla mapnum
            makeCreditsMap(origRom, context, MAP_ID);
        }

        public static void makeCreditsMap(byte[] origRom, RandoContext context, int mapNum)
        {
            MapHeader creditsHeader = VanillaMapUtil.getHeader(origRom, MAP_ID);
            FullMap creditsMap = new FullMap();
            // no objects, no triggers
            creditsMap.mapHeader = creditsHeader;
            // layer 1 map piece 897 (899 in editor) and layer 2 map piece 898 (900 in editor)
            creditsMap.mapPieces = new FullMapMapPieces();

            byte[,] creditsL1MapPiece = VanillaMapUtil.loadVanillaMapPiece(origRom, 897);
            context.replacementMapPieces[897] = SomMapCompressor.EncodeMap(creditsL1MapPiece).ToList();
            FullMapMapPieceReference creditsLayer1 = new FullMapMapPieceReference();
            creditsLayer1.eventVisibilityFlagId = 0x00;
            creditsLayer1.eventVisibilityValueLow = 0x00;
            creditsLayer1.eventVisibilityValueHigh = 0x0F;
            creditsLayer1.pieceIndex = 897;
            creditsMap.mapPieces.bgPieces.Add(creditsLayer1);

            byte[,] creditsL2MapPiece = VanillaMapUtil.loadVanillaMapPiece(origRom, 898);
            context.replacementMapPieces[898] = SomMapCompressor.EncodeMap(creditsL2MapPiece).ToList();
            FullMapMapPieceReference creditsLayer2 = new FullMapMapPieceReference();
            creditsLayer2.eventVisibilityFlagId = 0x00;
            creditsLayer2.eventVisibilityValueLow = 0x00;
            creditsLayer2.eventVisibilityValueHigh = 0x0F;
            creditsLayer2.pieceIndex = 898;
            creditsMap.mapPieces.fgPieces.Add(creditsLayer2);

            context.generatedMaps[mapNum] = creditsMap;

        }
    }
}
